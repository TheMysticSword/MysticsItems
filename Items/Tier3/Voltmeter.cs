using RoR2;
using RoR2.Orbs;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Items
{
    public class Voltmeter : BaseItem
    {
        public static GameObject visualEffect;

        public override void PreAdd()
        {
            itemDef.name = "Voltmeter";
            itemDef.tier = ItemTier.Tier3;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage,
                ItemTag.Utility,
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist
            };
            SetAssets("Voltmeter");
            Material matVoltmeterCoil = model.transform.Find("Цилиндр.001").GetComponent<MeshRenderer>().sharedMaterial;
            Main.HopooShaderToMaterial.Standard.Apply(matVoltmeterCoil);
            Main.HopooShaderToMaterial.Standard.Emission(matVoltmeterCoil, 2f, matVoltmeterCoil.color);
            Material matVoltmeter = model.transform.Find("Cube").GetComponent<MeshRenderer>().sharedMaterial;
            Main.HopooShaderToMaterial.Standard.Apply(matVoltmeter);
            Main.HopooShaderToMaterial.Standard.Gloss(matVoltmeter, 0.09f);
            Main.HopooShaderToMaterial.Standard.Emission(matVoltmeter, 0.2f, new Color32(52, 37, 29, 255));
            PointerAnimator pointerAnimator = model.AddComponent<PointerAnimator>();
            pointerAnimator.pointer = model.transform.Find("PointerCenter").gameObject;
            CopyModelToFollower();
        }

        public class PointerAnimator : MonoBehaviour
        {
            public GameObject pointer;
            public float minAngle = -45f;
            public float maxAngle = 90f;
            public float currentTarget = 0f;
            public float untilChangeTarget = 0f;
            public float smoothDampVelocity = 0f;

            public void Update()
            {
                untilChangeTarget -= Time.deltaTime;
                if (untilChangeTarget <= 0f)
                {
                    untilChangeTarget = 0.3f * Random.value;
                    currentTarget = Random.Range(minAngle, maxAngle);
                }
                if (pointer)
                {
                    Vector3 rotation = pointer.transform.localRotation.eulerAngles;
                    rotation.x = Mathf.SmoothDampAngle(rotation.x, currentTarget, ref smoothDampVelocity, 0.2f);
                    pointer.transform.localRotation = Quaternion.Euler(rotation);
                }
            }
        }

        public override void OnAdd()
        {
            On.RoR2.CharacterBody.Awake += (orig, self) =>
            {
                orig(self);
                self.gameObject.AddComponent<PreDamageShield>();
            };

            Main.BeforeTakeDamage += delegate (DamageInfo damage, Main.GenericCharacterInfo characterInfo)
            {
                PreDamageShield preDamageShield = characterInfo.gameObject.GetComponent<PreDamageShield>();
                if (preDamageShield && characterInfo.healthComponent) preDamageShield.value = characterInfo.healthComponent.shield;
            };

            Main.OnTakeDamage += delegate (DamageInfo damageInfo, Main.GenericCharacterInfo characterInfo)
            {
                PreDamageShield preDamageShield = characterInfo.gameObject.GetComponent<PreDamageShield>();
                if (characterInfo.inventory && characterInfo.inventory.GetItemCount(itemIndex) > 0 && preDamageShield && preDamageShield.value > 0f)
                {
                    float radius = 25f;

                    /*
                    EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/LightningStakeNova"), new EffectData
                    {
                        origin = characterInfo.body.corePosition,
                        scale = radius,
                        rotation = Util.QuaternionSafeLookRotation(damageInfo.force)
                    }, true);
                    BlastAttack blastAttack = new BlastAttack
                    {
                        position = characterInfo.body.corePosition,
                        baseDamage = damageInfo.damage * (4f + (characterInfo.inventory.GetItemCount(itemIndex) - 1)),
                        baseForce = damageInfo.force.magnitude,
                        radius = radius,
                        attacker = characterInfo.gameObject,
                        inflictor = null,
                        teamIndex = characterInfo.teamIndex,
                        crit = damageInfo.crit,
                        procChainMask = default(ProcChainMask),
                        procCoefficient = 0f,
                        damageColorIndex = DamageColorIndex.Item,
                        falloffModel = BlastAttack.FalloffModel.None,
                        damageType = DamageType.AOE,
                        attackerFiltering = AttackerFiltering.NeverHit // don't explode self with Chaos
                    };
                    blastAttack.Fire();
                    */

                    BullseyeSearch search = new BullseyeSearch
                    {
                        searchOrigin = characterInfo.gameObject.transform.position,
                        searchDirection = Vector3.zero,
                        teamMaskFilter = TeamMask.allButNeutral
                    };
                    search.teamMaskFilter.RemoveTeam(characterInfo.teamIndex);
                    search.filterByLoS = false;
                    search.sortMode = BullseyeSearch.SortMode.Distance;
                    search.maxDistanceFilter = radius;
                    search.RefreshCandidates();

                    foreach (HurtBox hurtBox in search.GetResults())
                    {
                        LightningOrb lightningOrb = new LightningOrb
                        {
                            origin = characterInfo.body.corePosition,
                            damageValue = damageInfo.damage * (8f + (characterInfo.inventory.GetItemCount(itemIndex) - 1)),
                            isCrit = damageInfo.crit,
                            bouncesRemaining = 0,
                            teamIndex = characterInfo.teamIndex,
                            attacker = characterInfo.gameObject,
                            procCoefficient = 0f,
                            lightningType = LightningOrb.LightningType.Ukulele,
                            damageColorIndex = DamageColorIndex.Item,
                            range = radius,
                            target = hurtBox
                        };
                        OrbManager.instance.AddOrb(lightningOrb);
                    }
                }
            };

            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                // shield
                if (c.TryGotoNext(
                    MoveType.AfterLabel,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdloc(43),
                    x => x.MatchCallOrCallvirt<CharacterBody>("set_maxShield")
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<System.Func<CharacterBody, float>>((characterBody) =>
                    {
                        Inventory inventory = characterBody.inventory;
                        if (inventory && inventory.GetItemCount(itemIndex) > 0)
                        {
                            return 0.04f * characterBody.maxHealth;
                        }
                        return 0;
                    });
                    c.Emit(OpCodes.Ldloc, 43);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 43);
                }
            };
        }

        public class PreDamageShield : MonoBehaviour
        {
            public float value;
        }
    }
}
