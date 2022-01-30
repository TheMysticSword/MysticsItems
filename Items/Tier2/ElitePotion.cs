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
            8f,
            "Duration of the inflicted status (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_ELITEPOTION_DESC"
            }
        );
        public static ConfigurableValue<float> durationPerStack = new ConfigurableValue<float>(
            "Item: Failed Experiment",
            "DurationPerStack",
            4f,
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

            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Failed Experiment/Model.prefab"));
            HopooShaderToMaterial.Standard.Apply(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial);
            HopooShaderToMaterial.Standard.Emission(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial, 0.7f);
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Failed Experiment/Icon.png");
            itemDisplayPrefab = PrepareItemDisplayModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Failed Experiment/DisplayModel.prefab"));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Stomach", new Vector3(0.1037F, 0.0959F, -0.18842F), new Vector3(3.75687F, 83.84097F, 352.8532F), new Vector3(0.08418F, 0.08418F, 0.08418F));
                AddDisplayRule("HuntressBody", "Pelvis", new Vector3(0.166F, -0.03903F, 0.08013F), new Vector3(358.1845F, 141.0982F, 188.2708F), new Vector3(0.07837F, 0.07837F, 0.07837F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(0.21281F, -0.0023F, 0.01347F), new Vector3(0F, 13.88173F, 1.82638F), new Vector3(0.07333F, 0.07333F, 0.07333F));
                AddDisplayRule("ToolbotBody", "Hip", new Vector3(-1.36558F, 1.03279F, 0.82603F), new Vector3(4.07176F, 4.79607F, 198.9395F), new Vector3(0.6665F, 0.6665F, 0.6665F));
                AddDisplayRule("EngiBody", "Pelvis", new Vector3(-0.063F, 0.14475F, 0.21682F), new Vector3(1.34564F, 72.93568F, 168.4204F), new Vector3(0.11607F, 0.11607F, 0.11607F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0F, 1.386F, -1.44683F), new Vector3(0F, 270F, 206.4486F), new Vector3(0.31299F, 0.31299F, 0.31299F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.03562F, 1.87043F, -1.49741F), new Vector3(0F, 270F, 216.2809F), new Vector3(0.35353F, 0.43003F, 0.34563F));
                AddDisplayRule("MageBody", "Chest", new Vector3(-0.1171F, 0.4371F, -0.15939F), new Vector3(2.02564F, 269.8611F, 171.8138F), new Vector3(0.10955F, 0.09173F, 0.08544F));
                AddDisplayRule("MageBody", "Chest", new Vector3(0.11087F, 0.44522F, -0.15883F), new Vector3(2.02564F, 269.8611F, 171.8138F), new Vector3(0.10955F, 0.09173F, 0.08544F));
                AddDisplayRule("MercBody", "Pelvis", new Vector3(0.12935F, 0.09074F, 0.09716F), new Vector3(356.3984F, 103.2658F, 167.778F), new Vector3(0.0835F, 0.0835F, 0.0835F));
                AddDisplayRule("TreebotBody", "PlatformBase", new Vector3(0.80756F, 0.12623F, -0.57676F), new Vector3(35.90203F, 63.69349F, 33.24524F), new Vector3(0.38541F, 0.38541F, 0.38541F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(0.04292F, -0.01057F, 0.46813F), new Vector3(359.9311F, 267.4907F, 0.00302F), new Vector3(0.07456F, 0.07456F, 0.07456F));
                AddDisplayRule("CrocoBody", "SpineChest2", new Vector3(-0.6392F, 1.21143F, -0.83404F), new Vector3(355.1915F, 47.17368F, 94.44273F), new Vector3(0.78694F, 0.78694F, 0.78694F));
                AddDisplayRule("CaptainBody", "Stomach", new Vector3(-0.4079F, -0.167F, -0.00044F), new Vector3(3.18647F, 174.6853F, 3.84292F), new Vector3(0.10502F, 0.10502F, 0.10502F));
                AddDisplayRule("BrotherBody", "ThighL", BrotherInfection.green, new Vector3(0.07086F, 0.01162F, -0.06094F), new Vector3(77.0517F, 128.9086F, 259.4779F), new Vector3(0.04861F, 0.10534F, 0.10724F));
                AddDisplayRule("ScavBody", "MuzzleEnergyCannon", new Vector3(0F, -6.83055F, -0.00024F), new Vector3(0F, 269.055F, 0F), new Vector3(1.98217F, 1.98217F, 1.98217F));
            };
            
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;

            RoR2Application.onLoad += () =>
            {
                spreadEffectInfos.Add(new SpreadEffectInfo
                {
                    eliteBuffDef = RoR2Content.Buffs.AffixRed,
                    dot = DotController.DotIndex.Burn,
                    vfx = Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXLemurianBruiserFireballImpact"),
                    damage = 4f,
                    procCoefficient = 0f
                });
                spreadEffectInfos.Add(new SpreadEffectInfo
                {
                    eliteBuffDef = RoR2Content.Buffs.AffixBlue,
                    vfx = Resources.Load<GameObject>("Prefabs/Effects/CaptainTazerSupplyDropNova"),
                    damage = 10f,
                    procCoefficient = 1f,
                    damageType = DamageType.Shock5s
                });
                spreadEffectInfos.Add(new SpreadEffectInfo
                {
                    eliteBuffDef = RoR2Content.Buffs.AffixWhite,
                    vfx = Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniImpactVFXFrozen"),
                    debuff = RoR2Content.Buffs.Slow80,
                    damageType = DamageType.Freeze2s
                });
                spreadEffectInfos.Add(new SpreadEffectInfo
                {
                    eliteBuffDef = RoR2Content.Buffs.AffixPoison,
                    vfx = Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXUrchin"),
                    debuff = RoR2Content.Buffs.HealingDisabled,
                    damageType = DamageType.WeakOnHit
                });
                spreadEffectInfos.Add(new SpreadEffectInfo
                {
                    eliteBuffDef = RoR2Content.Buffs.AffixHaunted,
                    vfx = Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXGreaterWisp"),
                    debuff = RoR2Content.Buffs.Slow80,
                    damageType = DamageType.Stun1s
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
                        var damageMult = 1f + 0.8f * (itemCount - 1);

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
                                        baseDamage = damageReport.attackerBody.damage * spreadEffectInfo.damage * damageMult,
                                        procCoefficient = spreadEffectInfo.procCoefficient,
                                        crit = Util.CheckRoll(damageReport.attackerBody.crit, damageReport.attackerMaster),
                                        damageType = spreadEffectInfo.damageType | DamageType.AOE,
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
            public DamageType damageType;
        }
        public static List<SpreadEffectInfo> spreadEffectInfos = new List<SpreadEffectInfo>();

        private static SphereSearch sphereSearch = new SphereSearch();
        private static List<HurtBox> hurtBoxes = new List<HurtBox>();
    }
}
