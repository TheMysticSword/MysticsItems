using RoR2;
using EntityStates;
using RoR2.Skills;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Linq;

namespace MysticsItems.Items
{
    public class CommandoRevolverDrum : BaseItem
    {
        public static SkillFamily skillFamily;
		public static SkillDef skillDef;

        public override void PreAdd()
        {
            itemDef.name = "CommandoRevolverDrum";
            itemDef.tier = ItemTier.Tier2;
            SetAssets("Revolver Drum");
            AddDisplayRule((int)Main.CommonBodyIndices.Commando, "MuzzleLeft", new Vector3(-0.008F, -0.002F, -0.109F), new Vector3(273.148F, 265.817F, 5.331F), new Vector3(0.036F, 0.046F, 0.046F));
			AddDisplayRule((int)Main.CommonBodyIndices.Commando, "MuzzleRight", new Vector3(-0.008F, -0.002F, -0.109F), new Vector3(273.148F, 265.817F, 5.331F), new Vector3(0.036F, 0.046F, 0.046F));
			
			FireBarrageRevolverDrum.effectPrefab = Resources.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/Muzzleflash1");
			FireBarrageRevolverDrum.hitEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/Hitspark1");
			FireBarrageRevolverDrum.tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoDefault");
		}

        public override void OnAdd()
        {
            CharacterItems.SetCharacterItem(this, "CommandoBody");

            On.RoR2.Skills.SkillCatalog.Init += (orig) =>
            {
                orig();

                SkillDef skillDef = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("Barrage"));
                GenericSkill genericSkill = PrefabAPI.InstantiateClone(new GameObject(), "").AddComponent<GenericSkill>();
                foreach (SkillFamily _skillFamily in SkillCatalog.allSkillFamilies)
                {
                    SkillFamily.Variant matchingVariant = _skillFamily.variants.ToList().Find(x => x.skillDef == skillDef);
                    if (!matchingVariant.Equals(default(SkillFamily.Variant)))
                    {
                        skillFamily = _skillFamily;
                        break;
                    }
                }
            };

			skillDef = ScriptableObject.CreateInstance<SkillDef>();
			skillDef.skillName = Main.TokenPrefix + "CommandoRevolverDrumBarrageQuick";
			skillDef.activationStateMachineName = "Weapon";
			skillDef.activationState = new SerializableEntityStateType(typeof(FireBarrageRevolverDrum));
			skillDef.interruptPriority = InterruptPriority.PrioritySkill;
			skillDef.baseRechargeInterval = 0f;
			skillDef.baseMaxStock = 1;
			skillDef.rechargeStock = 1;
			skillDef.isBullets = false;
			skillDef.shootDelay = 0f;
			skillDef.requiredStock = 0;
			skillDef.stockToConsume = 0;
			skillDef.noSprint = false;
			skillDef.isCombatSkill = true;
			skillDef.fullRestockOnAssign = true;
			skillDef.dontAllowPastMaxStocks = true;

			LoadoutAPI.AddSkill(typeof(FireBarrageRevolverDrum));
			LoadoutAPI.AddSkillDef(skillDef);

			FireBarrageRevolverDrum.itemIndex = itemIndex;

			On.RoR2.GenericSkill.Awake += (orig, self) =>
            {
                MysticsItemsCommandoRevolverDrumBehaviour component = self.gameObject.GetComponent<MysticsItemsCommandoRevolverDrumBehaviour>();
                if (component && component.creatingSkill > 0)
                {
                    component.creatingSkill--;
                    self.SetFieldValue("_skillFamily", skillFamily);
                }
                orig(self);
                self.SetBaseSkill(skillDef);
            };

            On.RoR2.CharacterBody.Awake += (orig, self) =>
            {
                orig(self);
                self.onInventoryChanged += () =>
                {
                    if (NetworkServer.active) self.AddItemBehavior<MysticsItemsCommandoRevolverDrumBehaviour>(self.inventory.GetItemCount(itemIndex));
                };
            };

            On.EntityStates.Commando.CommandoWeapon.FirePistol2.FireBullet += (orig, self, targetMuzzle) =>
            {
                MysticsItemsCommandoRevolverDrumBehaviour component = self.outer.gameObject.GetComponent<MysticsItemsCommandoRevolverDrumBehaviour>();
                if (component)
                {
                    component.applyAttackChanges++;
                }
                orig(self, targetMuzzle);
            };
            Main.OnHitEnemy += (damageInfo, attackerInfo, victimInfo) =>
            {
                if (attackerInfo.inventory && attackerInfo.inventory.GetItemCount(itemIndex) > 0)
                {
                    MysticsItemsCommandoRevolverDrumBehaviour component = attackerInfo.body.gameObject.GetComponent<MysticsItemsCommandoRevolverDrumBehaviour>();
                    if (component && component.applyAttackChanges > 0)
                    {
                        component.applyAttackChanges--;
                        if (Util.CheckRoll(10f, attackerInfo.master))
                        {
							component.procs++;
                        }
                    }
                }
            };
        }

        public class MysticsItemsCommandoRevolverDrumBehaviour : CharacterBody.ItemBehavior
        {
            public int applyAttackChanges = 0;
            public int creatingSkill = 0;
			public int procs = 0;
            public GenericSkill skillSlot;

            public void Start()
            {
				creatingSkill++;
                skillSlot = body.gameObject.AddComponent<GenericSkill>();
            }

            public void FixedUpdate()
            {
				if (!NetworkServer.active) return;
                applyAttackChanges = 0;
				if (procs > 0 && skillSlot.ExecuteIfReady())
                {
					procs--;
				}
            }
        }

		public class FireBarrageRevolverDrum : BaseState
		{
			public override void OnEnter()
			{
				base.OnEnter();
				characterBody.SetSpreadBloom(spreadBloomValue, false);
				duration = totalDuration;
				bulletCount = baseBulletCount;
				Inventory inventory = outer.gameObject.GetComponent<CharacterBody>().inventory;
				if (inventory)
				{
					bulletCount += stackBulletCount * (inventory.GetItemCount(itemIndex) - 1);
				}
				durationBetweenShots = (totalDuration / (float)bulletCount) / attackSpeedStat;
				
				modelAnimator = GetModelAnimator();
				modelTransform = GetModelTransform();
				PlayCrossfade("Gesture, Additive", "FireBarrage", "FireBarrage.playbackRate", duration, 0.2f);
				PlayCrossfade("Gesture, Override", "FireBarrage", "FireBarrage.playbackRate", duration, 0.2f);
				FireBullet();
			}

			private void FireBullet()
			{
				Ray aimRay = GetAimRay();
				string muzzleName = "MuzzleRight";
				if (modelAnimator)
				{
					if (effectPrefab)
					{
						EffectManager.SimpleMuzzleFlash(effectPrefab, gameObject, muzzleName, false);
					}
					PlayAnimation("Gesture Additive, Right", "FirePistol, Right");
				}
				AddRecoil(-0.8f * recoilAmplitude, -1f * recoilAmplitude, -0.1f * recoilAmplitude, 0.15f * recoilAmplitude);
				if (isAuthority)
				{
					new BulletAttack
					{
						owner = gameObject,
						weapon = gameObject,
						origin = aimRay.origin,
						aimVector = aimRay.direction,
						minSpread = minSpread,
						maxSpread = maxSpread,
						bulletCount = 1U,
						damage = damageCoefficient * damageStat,
						force = force,
						tracerEffectPrefab = tracerEffectPrefab,
						muzzleName = muzzleName,
						hitEffectPrefab = hitEffectPrefab,
						isCrit = Util.CheckRoll(critStat, characterBody.master),
						radius = bulletRadius,
						smartCollision = true
					}.Fire();
				}
				characterBody.AddSpreadBloom(spreadBloomValue);
				totalBulletsFired++;
				Util.PlaySound(fireBarrageSoundString, gameObject);
			}

			public override void FixedUpdate()
			{
				base.FixedUpdate();
				stopwatchBetweenShots += Time.fixedDeltaTime;
				if (stopwatchBetweenShots >= durationBetweenShots && totalBulletsFired < bulletCount)
				{
					stopwatchBetweenShots -= durationBetweenShots;
					FireBullet();
				}
				if (fixedAge >= duration && totalBulletsFired == bulletCount && isAuthority)
				{
					outer.SetNextStateToMain();
					return;
				}
			}

			public override InterruptPriority GetMinimumInterruptPriority()
			{
				return InterruptPriority.Skill;
			}

			public static GameObject effectPrefab;
			public static GameObject hitEffectPrefab;
			public static GameObject tracerEffectPrefab;
			public static float damageCoefficient = 1f;
			public static float force = 0f;
			public static float minSpread = 0f;
			public static float maxSpread = 4f;
			public static float totalDuration = 0.24f;
			public static float bulletRadius = 1.5f;
			public static int baseBulletCount = 6;
			public static int stackBulletCount = 6;
			public static string fireBarrageSoundString = "Play_commando_M1";
			public static float recoilAmplitude = 0.3f;
			public static float spreadBloomValue = 0.2f;
			public static ItemIndex itemIndex;
			public int totalBulletsFired;
			public int bulletCount;
			public float stopwatchBetweenShots;
			public Animator modelAnimator;
			public Transform modelTransform;
			public float duration;
			public float durationBetweenShots;
		}
	}
}
