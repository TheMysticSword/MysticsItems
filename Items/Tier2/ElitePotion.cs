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
    public class ElitePotion : BaseItem
    {
        public static ConfigurableValue<float> radius = new ConfigurableValue<float>(
            "Item: Failed Experiment",
            "Radius",
            12f,
            "Radius of the AoE status infliction (in meters)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_ELITEPOTION_DESC"
            }
        );
        public static ConfigurableValue<float> radiusPerStack = new ConfigurableValue<float>(
            "Item: Failed Experiment",
            "RadiusPerStack",
            2.4f,
            "Radius of the AoE status infliction for each additional stack of this item (in meters)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_ELITEPOTION_DESC"
            }
        );
        public static ConfigurableValue<float> duration = new ConfigurableValue<float>(
            "Item: Failed Experiment",
            "Duration",
            4f,
            "Duration of the inflicted status (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_ELITEPOTION_DESC"
            }
        );
        public static ConfigurableValue<float> durationPerStack = new ConfigurableValue<float>(
            "Item: Failed Experiment",
            "DurationPerStack",
            2f,
            "Duration of the inflicted status for each additional stack of this item (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_ELITEPOTION_DESC"
            }
        );

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_ElitePotion";
            itemDef.tier = ItemTier.Tier2;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage,
                ItemTag.OnKillEffect,
                ItemTag.AIBlacklist
            };
            /*
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Hexahedral Monolith/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Hexahedral Monolith/Icon.png");
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "GunMeshR", new Vector3(-0.13951F, -0.01505F, 0.13151F), new Vector3(0F, 0F, 90F), new Vector3(0.00856F, 0.00856F, 0.00856F));
                AddDisplayRule("HuntressBody", "UpperArmL", new Vector3(0.06909F, 0.10681F, -0.00977F), new Vector3(3.66903F, 357.0302F, 178.0301F), new Vector3(0.01358F, 0.01358F, 0.01358F));
                AddDisplayRule("Bandit2Body", "MainWeapon", new Vector3(-0.05477F, 0.2274F, -0.04443F), new Vector3(359.4865F, 89.48757F, 206.7464F), new Vector3(0.0135F, 0.00485F, 0.00485F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(-1.77361F, 2.53066F, 1.76556F), new Vector3(0F, 90F, 90F), new Vector3(0.10065F, 0.10065F, 0.10065F));
                AddDisplayRule("EngiBody", "LowerArmR", new Vector3(0.0113F, 0.13437F, -0.05836F), new Vector3(1.34564F, 72.93568F, 188.458F), new Vector3(0.01476F, 0.01476F, 0.01476F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.035F, 0.89075F, -1.47928F), new Vector3(0F, 90F, 303.695F), new Vector3(0.07847F, 0.07847F, 0.07847F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.03562F, 1.40676F, -1.39837F), new Vector3(0F, 90F, 303.1705F), new Vector3(0.08093F, 0.09844F, 0.07912F));
                AddDisplayRule("MageBody", "Chest", new Vector3(-0.10398F, 0.07562F, -0.31389F), new Vector3(359.7522F, 90.11677F, 8.18118F), new Vector3(0.01236F, 0.01035F, 0.00964F));
                AddDisplayRule("MageBody", "Chest", new Vector3(0.11942F, 0.07423F, -0.30928F), new Vector3(359.136F, 95.88205F, 8.14244F), new Vector3(0.01236F, 0.01035F, 0.00787F));
                AddDisplayRule("MercBody", "HandL", new Vector3(0.01326F, 0.1146F, 0.04565F), new Vector3(88.10731F, 183.3846F, 89.99922F), new Vector3(0.00961F, 0.00961F, 0.00965F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(0.69564F, -0.5422F, -0.29426F), new Vector3(46.13942F, 241.7613F, 12.79626F), new Vector3(0.03647F, 0.03647F, 0.03647F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(0.01517F, -0.06288F, -0.17121F), new Vector3(90F, 90F, 0F), new Vector3(0.0207F, 0.0207F, 0.0207F));
                AddDisplayRule("CrocoBody", "SpineChest1", new Vector3(1.39693F, -0.10569F, -0.18201F), new Vector3(55.10429F, 175.6143F, 292.3791F), new Vector3(0.1379F, 0.1379F, 0.1379F));
                AddDisplayRule("CaptainBody", "MuzzleGun", new Vector3(0.00467F, 0.05642F, -0.1194F), new Vector3(357.9892F, 90.52832F, 89.76476F), new Vector3(0.05388F, 0.01322F, 0.0146F));
                AddDisplayRule("BrotherBody", "UpperArmL", BrotherInfection.green, new Vector3(0.06646F, 0.22781F, -0.00154F), new Vector3(77.05167F, 128.9087F, 289.6219F), new Vector3(0.04861F, 0.10534F, 0.10724F));
                AddDisplayRule("ScavBody", "Stomach", new Vector3(-0.92389F, 11.6509F, -5.90638F), new Vector3(20.93637F, 118.4181F, 332.9505F), new Vector3(0.24839F, 0.25523F, 0.24839F));
            };
            */

            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;

            RoR2Application.onLoad += () =>
            {
                spreadEffectInfos.Add(new SpreadEffectInfo
                {
                    eliteBuffDef = RoR2Content.Buffs.AffixRed,
                    dot = DotController.DotIndex.Burn,
                    vfx = Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXLemurianBruiserFireballImpact")
                });
                spreadEffectInfos.Add(new SpreadEffectInfo
                {
                    eliteBuffDef = RoR2Content.Buffs.AffixBlue,
                    vfx = Resources.Load<GameObject>("Prefabs/Effects/CaptainTazerSupplyDropNova"),
                    damage = 1f
                });
                spreadEffectInfos.Add(new SpreadEffectInfo
                {
                    eliteBuffDef = RoR2Content.Buffs.AffixWhite,
                    vfx = Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniImpactVFXFrozen"),
                    debuff = RoR2Content.Buffs.Slow80
                });
                spreadEffectInfos.Add(new SpreadEffectInfo
                {
                    eliteBuffDef = RoR2Content.Buffs.AffixPoison,
                    vfx = Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXUrchin"),
                    debuff = RoR2Content.Buffs.HealingDisabled
                });
                spreadEffectInfos.Add(new SpreadEffectInfo
                {
                    eliteBuffDef = RoR2Content.Buffs.AffixHaunted,
                    vfx = Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXGreaterWisp"),
                    debuff = RoR2Content.Buffs.Slow80
                });
                spreadEffectInfos.Add(new SpreadEffectInfo
                {
                    eliteBuffDef = RoR2Content.Buffs.AffixLunar,
                    vfx = Resources.Load<GameObject>("Prefabs/Effects/LunarGolemTwinShotExplosion"),
                    debuff = RoR2Content.Buffs.Cripple
                });
            };
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (NetworkServer.active)
            {
                if (damageReport.attackerBody && damageReport.attackerBody.inventory) {
                    var itemCount = damageReport.attackerBody.inventory.GetItemCount(itemDef);
                    if (itemCount > 0 && damageReport.victimIsElite)
                    {
                        var radius = ElitePotion.radius + ElitePotion.radiusPerStack * (itemCount - 1);
                        var duration = ElitePotion.duration + ElitePotion.durationPerStack * (itemCount - 1);

                        foreach (var buffIndex in BuffCatalog.eliteBuffIndices.Where(x => damageReport.victimBody.HasBuff(x)))
                        {
                            foreach (var spreadEffectInfo in spreadEffectInfos.Where(x => x.eliteBuffDef.buffIndex == buffIndex))
                            {
                                if (spreadEffectInfo.vfx)
                                {
                                    EffectManager.SpawnEffect(spreadEffectInfo.vfx, new EffectData
                                    {
                                        origin = damageReport.victimBody.corePosition,
                                        scale = radius
                                    }, true);
                                }

                                sphereSearch.origin = damageReport.victimBody.corePosition;
                                sphereSearch.mask = LayerIndex.entityPrecise.mask;
                                sphereSearch.radius = radius;
                                sphereSearch.RefreshCandidates();
                                sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(damageReport.attackerTeamIndex));
                                sphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
                                sphereSearch.OrderCandidatesByDistance();
                                sphereSearch.GetHurtBoxes(hurtBoxes);
                                sphereSearch.ClearCandidates();
                                foreach (var hurtBox in hurtBoxes)
                                {
                                    if (hurtBox.healthComponent)
                                    {
                                        if (spreadEffectInfo.debuff)
                                            hurtBox.healthComponent.body.AddTimedBuff(spreadEffectInfo.debuff, duration);
                                        if (spreadEffectInfo.dot != default(DotController.DotIndex) && spreadEffectInfo.dot != DotController.DotIndex.None)
                                            DotController.InflictDot(hurtBox.healthComponent.gameObject, damageReport.attacker, spreadEffectInfo.dot, duration, 1f);
                                    }
                                }

                                if (spreadEffectInfo.damage != 0)
                                    new BlastAttack
                                    {
                                        radius = radius,
                                        baseDamage = damageReport.attackerBody.damage * spreadEffectInfo.damage,
                                        procCoefficient = spreadEffectInfo.procCoefficient,
                                        crit = Util.CheckRoll(damageReport.attackerBody.crit, damageReport.attackerMaster),
                                        damageColorIndex = DamageColorIndex.Item,
                                        attackerFiltering = AttackerFiltering.Default,
                                        falloffModel = BlastAttack.FalloffModel.None,
                                        attacker = damageReport.attacker,
                                        teamIndex = damageReport.attackerTeamIndex,
                                        position = damageReport.victimBody.corePosition
                                    }.Fire();
                            }
                        }
                    }
                }
            }
        }

        public struct SpreadEffectInfo
        {
            public BuffDef eliteBuffDef;
            public GameObject vfx;
            public BuffDef debuff;
            public DotController.DotIndex dot;
            public float damage;
            public float procCoefficient;
        }
        public static List<SpreadEffectInfo> spreadEffectInfos = new List<SpreadEffectInfo>();

        private static SphereSearch sphereSearch = new SphereSearch();
        private static List<HurtBox> hurtBoxes = new List<HurtBox>();
    }
}
