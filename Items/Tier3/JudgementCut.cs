using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API.Networking.Interfaces;
using R2API.Networking;
using static MysticsItems.BalanceConfigManager;

namespace MysticsItems.Items
{
    public class JudgementCut : BaseItem
    {
        public static ConfigurableValue<float> baseCrit = new ConfigurableValue<float>(
            "Item: Devil s Cry",
            "BaseCrit",
            5f,
            "Critical strike chance from the first stack of this item",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_JUDGEMENTCUT_DESC"
            }
        );
        public static ConfigurableValue<int> critInterval = new ConfigurableValue<int>(
            "Item: Devil s Cry",
            "CritInterval",
            5,
            "Every X crits, this item will trigger",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_JUDGEMENTCUT_PICKUP",
                "ITEM_MYSTICSITEMS_JUDGEMENTCUT_DESC"
            }
        );
        public static ConfigurableValue<float> damagePerSlash = new ConfigurableValue<float>(
            "Item: Devil s Cry",
            "DamagePerSlash",
            300f,
            "Base damage of each slash (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_JUDGEMENTCUT_DESC"
            }
        );
        public static ConfigurableValue<float> radius = new ConfigurableValue<float>(
            "Item: Devil s Cry",
            "Radius",
            6f,
            "Radius of the slash barrage (in m)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_JUDGEMENTCUT_DESC"
            }
        );
        public static ConfigurableValue<float> radiusPerStack = new ConfigurableValue<float>(
            "Item: Devil s Cry",
            "RadiusPerStack",
            1.2f,
            "Radius of the slash barrage for each additional stack of this item (in m)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_JUDGEMENTCUT_DESC"
            }
        );
        public static ConfigurableValue<float> procCoefficient = new ConfigurableValue<float>(
            "Item: Devil s Cry",
            "ProcCoefficient",
            0.5f,
            "Proc coefficient of each slash"
        );
        public static ConfigurableValue<int> slashCount = new ConfigurableValue<int>(
            "Item: Devil s Cry",
            "SlashCount",
            3,
            "Slash count for the first stack of this item",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_JUDGEMENTCUT_DESC"
            }
        );
        public static ConfigurableValue<int> slashCountPerStack = new ConfigurableValue<int>(
            "Item: Devil s Cry",
            "SlashCountPerStack",
            2,
            "Extra slash count for each additional stack of this item",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_JUDGEMENTCUT_DESC"
            }
        );
        public static ConfigurableValue<bool> alternateDamage = new ConfigurableValue<bool>(
            "Item: Devil s Cry",
            "AlternateDamage",
            false,
            "If true, slash damage will scale with total damage instead of base damage"
        );

        public static GameObject judgementCutVFX;
        public static GameObject judgementCutSingleSlashVFX;

        public static GameObject judgementCutHitterPrefab;
        
        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_JudgementCut";
            itemDef.tier = ItemTier.Tier3;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage
            };

            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Katana/Model.prefab"));
            HopooShaderToMaterial.Standard.Apply(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial);
            HopooShaderToMaterial.Standard.Emission(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial, 1f);
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Katana/IconRed.png");
            itemDisplayPrefab = PrepareItemDisplayModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Katana/DisplayModel.prefab"));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Stomach", new Vector3(0.17571F, 0.11963F, -0.01449F), new Vector3(356.4138F, 97.41946F, 44.80042F), new Vector3(1.00693F, 1.00693F, 1.00693F));
                AddDisplayRule("HuntressBody", "Pelvis", new Vector3(0.17956F, -0.10272F, 0.03562F), new Vector3(9.60948F, 69.02416F, 221.7583F), new Vector3(1.00093F, 1.00093F, 1.00093F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(0.16899F, 0.02378F, 0.12248F), new Vector3(359.1422F, 41.88795F, 50.20916F), new Vector3(0.96268F, 0.96268F, 0.96268F));
                AddDisplayRule("ToolbotBody", "Hip", new Vector3(-0.34425F, 0.10702F, 1.12287F), new Vector3(0F, 0F, 252.2824F), new Vector3(8.48073F, 8.48073F, 8.48073F));
                AddDisplayRule("EngiBody", "Pelvis", new Vector3(0.26954F, -0.08209F, -0.12108F), new Vector3(4.79829F, 90.40765F, 230.8055F), new Vector3(1.05368F, 1.05368F, 1.05368F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(1.03251F, 0.56934F, -0.27521F), new Vector3(353.7924F, 91.60346F, 77.54379F), new Vector3(1.99366F, 1.99366F, 1.99366F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.88183F, 0.86859F, -0.32553F), new Vector3(354.8636F, 93.74093F, 70.43564F), new Vector3(2.5512F, 2.5512F, 2.5512F));
                AddDisplayRule("MageBody", "Stomach", new Vector3(0.19175F, -0.00645F, -0.03362F), new Vector3(354.615F, 91.17184F, 68.68049F), new Vector3(1.09248F, 1.09248F, 1.09248F));
                AddDisplayRule("MercBody", "Pelvis", new Vector3(0.19233F, 0.02065F, -0.00453F), new Vector3(0.49602F, 82.81135F, 242.4686F), new Vector3(1.28183F, 1.28183F, 1.28183F));
                AddDisplayRule("TreebotBody", "PlatformBase", new Vector3(0.9614F, 0.36362F, 0.25789F), new Vector3(357.5135F, 95.63601F, 60.75025F), new Vector3(1.89141F, 1.89141F, 1.89141F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(0.26355F, -0.03715F, 0.28215F), new Vector3(0.06888F, 93.07963F, 60.68799F), new Vector3(1.30468F, 1.30468F, 1.30468F));
                AddDisplayRule("CrocoBody", "Hip", new Vector3(2.25328F, 0.86572F, 0.4697F), new Vector3(3.30308F, 67.34286F, 205.3175F), new Vector3(9.04507F, 9.04507F, 9.04507F));
                AddDisplayRule("CaptainBody", "Stomach", new Vector3(0.38994F, 0.00054F, -0.0457F), new Vector3(348.9698F, 95.52962F, 60.20174F), new Vector3(1.17461F, 1.17461F, 1.17461F));
                AddDisplayRule("BrotherBody", "Pelvis", BrotherInfection.green, new Vector3(0.13375F, -0.08002F, 0.04397F), new Vector3(71.65609F, 9.03364F, 115.2129F), new Vector3(0.29492F, 0.05455F, 0.10724F));
                AddDisplayRule("ScavBody", "Pelvis", new Vector3(-7.56403F, 5.21971F, 0.75358F), new Vector3(3.20935F, 93.25385F, 270.1964F), new Vector3(16.2268F, 16.2268F, 16.2268F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Pelvis", new Vector3(-0.19592F, 0.19768F, -0.01174F), new Vector3(7.61227F, 93.92657F, 55.36945F), new Vector3(1F, 1F, 1F));
            };

            {
                judgementCutVFX = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Katana/JudgementCutVFX.prefab");
                EffectComponent effectComponent = judgementCutVFX.AddComponent<EffectComponent>();
                effectComponent.applyScale = true;
                VFXAttributes vfxAttributes = judgementCutVFX.AddComponent<VFXAttributes>();
                vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.High;
                vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
                judgementCutVFX.AddComponent<DestroyOnTimer>().duration = 1f;
                MysticsItemsContent.Resources.effectPrefabs.Add(judgementCutVFX);
            }

            {
                judgementCutSingleSlashVFX = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Katana/JudgementCutSingleSlashVFX.prefab");
                EffectComponent effectComponent = judgementCutSingleSlashVFX.AddComponent<EffectComponent>();
                effectComponent.applyScale = true;
                effectComponent.soundName = "MysticsItems_Play_item_proc_katana";
                VFXAttributes vfxAttributes = judgementCutSingleSlashVFX.AddComponent<VFXAttributes>();
                vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
                vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
                judgementCutSingleSlashVFX.AddComponent<DestroyOnTimer>().duration = 0.5f;
                var setRandomRotation = judgementCutSingleSlashVFX.transform.Find("TrailParticle").gameObject.AddComponent<SetRandomRotation>();
                setRandomRotation.setRandomXRotation = true;
                setRandomRotation.setRandomYRotation = true;
                setRandomRotation.setRandomZRotation = true;
                MysticsItemsContent.Resources.effectPrefabs.Add(judgementCutSingleSlashVFX);
            }

            {
                judgementCutHitterPrefab = PrefabAPI.InstantiateClone(new GameObject(), "MysticsItems_JudgementCutHitter", false);
                judgementCutHitterPrefab.AddComponent<TeamFilter>();
                judgementCutHitterPrefab.AddComponent<MysticsItemsJudgementCutHitter>();
            }

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            GenericGameEvents.OnHitEnemy += GenericGameEvents_OnHitEnemy;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var inventory = sender.inventory;
            if (inventory)
            {
                var itemCount = inventory.GetItemCount(itemDef);
                if (itemCount > 0) args.critAdd += baseCrit;
            }
        }

        private void GenericGameEvents_OnHitEnemy(DamageInfo damageInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo attackerInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo victimInfo)
        {
            if (damageInfo.crit && !damageInfo.procChainMask.HasProc(ProcType.Behemoth) && damageInfo.procCoefficient > 0f && attackerInfo.inventory && attackerInfo.body && attackerInfo.body.inputBank)
            {
                var itemCount = attackerInfo.inventory.GetItemCount(itemDef);
                if (itemCount > 0)
                {
                    var component = attackerInfo.body.GetComponent<MysticsItemsJudgementCutCounter>();
                    if (!component) component = attackerInfo.body.gameObject.AddComponent<MysticsItemsJudgementCutCounter>();
                    component.count += damageInfo.procCoefficient;
                    if (component.count >= critInterval)
                    {
                        component.count = 0;

                        GameObject hitterObject = UnityEngine.Object.Instantiate(judgementCutHitterPrefab, damageInfo.position, Quaternion.identity);
                        MysticsItemsJudgementCutHitter hitter = hitterObject.GetComponent<MysticsItemsJudgementCutHitter>();
                        hitter.position = damageInfo.position;
                        hitter.baseDamage = (!alternateDamage ? attackerInfo.body.damage : damageInfo.damage) * damagePerSlash / 100f;
                        hitter.baseForce = 200f;
                        hitter.attacker = attackerInfo.gameObject;
                        hitter.radius = radius + radiusPerStack * (itemCount - 1);
                        hitter.crit = attackerInfo.body.RollCrit();
                        hitter.procCoefficient = procCoefficient;
                        hitter.maxTimer = 0.1f;
                        hitter.falloffModel = BlastAttack.FalloffModel.None;
                        hitter.damageColorIndex = DamageColorIndex.Item;
                        hitter.procChainMask = damageInfo.procChainMask;
                        hitter.procChainMask.AddProc(ProcType.Behemoth);
                        hitter.ammo = slashCount + slashCountPerStack * (itemCount - 1);
                        hitterObject.GetComponent<TeamFilter>().teamIndex = TeamComponent.GetObjectTeam(hitter.attacker);
                    }
                }
            }
        }

        public class MysticsItemsJudgementCutCounter : MonoBehaviour
        {
            public float count = 0;
        }

        public class MysticsItemsJudgementCutHitter : MonoBehaviour
        {
            public void Awake()
            {
                teamFilter = GetComponent<TeamFilter>();
                if (!NetworkServer.active) enabled = false;
            }

            public void FixedUpdate()
            {
                if (NetworkServer.active)
                {
                    timer += Time.fixedDeltaTime;
                    if (!initialVFXSpawned)
                    {
                        initialVFXSpawned = true;
                        EffectManager.SpawnEffect(judgementCutVFX, new EffectData
                        {
                            origin = transform.position,
                            scale = radius
                        }, true);
                    }
                    if (timer >= maxTimer && ammo > 0)
                    {
                        Detonate();
                        timer -= maxTimer;
                        ammo--;
                        if (ammo <= 0)
                            UnityEngine.Object.Destroy(gameObject);
                    }
                }
            }

            public void Detonate()
            {
                EffectManager.SpawnEffect(judgementCutSingleSlashVFX, new EffectData
                {
                    origin = transform.position,
                    scale = radius
                }, true);
                new BlastAttack
                {
                    position = position,
                    baseDamage = baseDamage,
                    baseForce = baseForce,
                    bonusForce = bonusForce,
                    radius = radius,
                    attacker = attacker,
                    inflictor = inflictor,
                    teamIndex = teamFilter.teamIndex,
                    crit = crit,
                    damageColorIndex = damageColorIndex,
                    damageType = damageType,
                    falloffModel = falloffModel,
                    procCoefficient = procCoefficient,
                    procChainMask = procChainMask
                }.Fire();
            }

            public Vector3 position;
            public GameObject attacker;
            public GameObject inflictor;
            public float baseDamage;
            public bool crit;
            public float baseForce;
            public float radius;
            public Vector3 bonusForce;
            public float maxTimer;
            public DamageColorIndex damageColorIndex;
            public BlastAttack.FalloffModel falloffModel;
            public DamageType damageType;
            public float procCoefficient = 1f;
            public ProcChainMask procChainMask;
            public int ammo = 3;
            private float timer;
            private bool initialVFXSpawned;
            private TeamFilter teamFilter;
        }
    }
}
