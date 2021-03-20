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
            Material matVoltmeterCoil = model.transform.Find("�������.001").GetComponent<MeshRenderer>().sharedMaterial;
            Main.HopooShaderToMaterial.Standard.Apply(matVoltmeterCoil);
            Main.HopooShaderToMaterial.Standard.Emission(matVoltmeterCoil, 2f, matVoltmeterCoil.color);
            Material matVoltmeter = model.transform.Find("Cube").GetComponent<MeshRenderer>().sharedMaterial;
            Main.HopooShaderToMaterial.Standard.Apply(matVoltmeter);
            Main.HopooShaderToMaterial.Standard.Gloss(matVoltmeter, 0.09f);
            Main.HopooShaderToMaterial.Standard.Emission(matVoltmeter, 0.2f, new Color32(52, 37, 29, 255));
            PointerAnimator pointerAnimator = model.AddComponent<PointerAnimator>();
            pointerAnimator.pointer = model.transform.Find("PointerCenter").gameObject;
            CopyModelToFollower();

            AddDisplayRule((int)Main.CommonBodyIndices.Commando, "Stomach", new Vector3(0.042F, 0.096F, -0.119F), new Vector3(4.394F, 73.472F, 2.074F), new Vector3(0.02F, 0.02F, 0.02F));
            AddDisplayRule("mdlHuntress", "Pelvis", new Vector3(0.077F, -0.096F, 0.09F), new Vector3(358.51F, 111.069F, 197.846F), new Vector3(0.018F, 0.018F, 0.018F));
            AddDisplayRule("mdlToolbot", "Chest", new Vector3(0.064F, 1.168F, 3.419F), new Vector3(0F, 270F, 0F), new Vector3(0.214F, 0.214F, 0.214F));
            AddDisplayRule("mdlEngi", "HandR", new Vector3(0.026F, -0.123F, 0.048F), new Vector3(275.324F, 252.038F, 18.986F), new Vector3(0.029F, 0.029F, 0.029F));
            AddDisplayRule((int)Main.CommonBodyIndices.EngiTurret, "Neck", new Vector3(0F, 0.6F, -0.168F), new Vector3(0F, 90F, 0F), new Vector3(0.133F, 0.133F, 0.133F));
            AddDisplayRule((int)Main.CommonBodyIndices.EngiWalkerTurret, "Neck", new Vector3(0F, 0.533F, -0.168F), new Vector3(0F, 90F, 0F), new Vector3(0.133F, 0.133F, 0.133F));
            AddDisplayRule("mdlMage", "Chest", new Vector3(0F, 0.064F, -0.327F), new Vector3(0F, 90F, 7.532F), new Vector3(0.039F, 0.039F, 0.039F));
            AddDisplayRule("mdlMerc", "Head", new Vector3(0F, 0.105F, -0.143F), new Vector3(0F, 90F, 19.781F), new Vector3(0.02F, 0.02F, 0.02F));
            AddDisplayRule("mdlTreebot", "PlatformBase", new Vector3(0.537F, -0.273F, 0.135F), new Vector3(7.036F, 343.597F, 322.203F), new Vector3(0.063F, 0.063F, 0.063F));
            AddDisplayRule("mdlLoader", "MechBase", new Vector3(0F, -0.051F, -0.159F), new Vector3(0F, 90F, 0F), new Vector3(0.037F, 0.037F, 0.037F));
            AddDisplayRule("mdlCroco", "SpineChest1", new Vector3(-0.951F, 0.633F, -0.312F), new Vector3(4.342F, 131.684F, 70.801F), new Vector3(0.254F, 0.254F, 0.254F));
            AddDisplayRule("mdlCaptain", "HandL", new Vector3(0.009F, 0.167F, 0.045F), new Vector3(270F, 270F, 0F), new Vector3(0.035F, 0.035F, 0.035F));
            AddDisplayRule("mdlBrother", "UpperArmL", BrotherInfection.red, new Vector3(0.124F, 0.177F, -0.056F), new Vector3(80.946F, 113.634F, 258.867F), new Vector3(0.061F, 0.063F, 0.063F));
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
