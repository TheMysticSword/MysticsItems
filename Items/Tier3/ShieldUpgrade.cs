using RoR2;
using RoR2.Orbs;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using static MysticsItems.LegacyBalanceConfigManager;
using System.Collections.Generic;

namespace MysticsItems.Items
{
    public class ShieldUpgrade : BaseItem
    {
        public static ConfigurableValue<float> passiveShield = new ConfigurableValue<float>(
            "Item: Charger Upgrade Module",
            "PassiveShield",
            6f,
            "Passive shield bonus for the first stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_SHIELDUPGRADE_DESC"
            }
        );
        public static ConfigurableValue<float> shieldBonus = new ConfigurableValue<float>(
            "Item: Charger Upgrade Module",
            "ShieldBonus",
            100f,
            "How much shield to add based on current shield (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_SHIELDUPGRADE_DESC"
            }
        );
        public static ConfigurableValue<float> shieldBonusPerStack = new ConfigurableValue<float>(
            "Item: Charger Upgrade Module",
            "ShieldBonusPerStack",
            100f,
            "How much shield to add based on current shield for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_SHIELDUPGRADE_DESC"
            }
        );
        public static ConfigurableValue<float> rechargeBoost = new ConfigurableValue<float>(
            "Item: Charger Upgrade Module",
            "RechargeBoost",
            50f,
            "How much faster should shield recharge (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_SHIELDUPGRADE_DESC"
            }
        );
        public static ConfigurableValue<float> rechargeBoostPerStack = new ConfigurableValue<float>(
            "Item: Charger Upgrade Module",
            "RechargeBoostPerStack",
            50f,
            "How much faster should shield recharge for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_SHIELDUPGRADE_DESC"
            }
        );

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_ShieldUpgrade";
            SetItemTierWhenAvailable(ItemTier.Tier3);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility
            };
            
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Shield Cell/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Shield Cell/Icon.png");
            var mat = itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial;
            HopooShaderToMaterial.Standard.Apply(mat);
            HopooShaderToMaterial.Standard.Emission(mat, 4f, new Color32(68, 100, 191, 255));
            
            itemDisplayPrefab = PrepareItemDisplayModel(PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Shield Cell/FollowerModel.prefab")));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Chest", new Vector3(-0.00492F, 0.40924F, -0.26316F), new Vector3(4.39399F, 73.47204F, 40.19987F), new Vector3(0.0952F, 0.0952F, 0.0952F));
                AddDisplayRule("HuntressBody", "Pelvis", new Vector3(0.17969F, -0.0327F, 0.00705F), new Vector3(22.22715F, 119.1057F, 183.4642F), new Vector3(0.07104F, 0.07104F, 0.07104F));
                AddDisplayRule("Bandit2Body", "MainWeapon", new Vector3(-0.0637F, 0.2521F, -0.0884F), new Vector3(0.88898F, 130.9326F, 5.30513F), new Vector3(0.04724F, 0.04724F, 0.04724F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(-0.26591F, 0.724F, -2.37459F), new Vector3(281.1822F, 354.1078F, 275.9842F), new Vector3(0.94861F, 0.94861F, 0.94861F));
                AddDisplayRule("EngiBody", "CannonHeadR", new Vector3(-0.18947F, 0.38422F, 0.19438F), new Vector3(86.84415F, 21.33805F, 67.44578F), new Vector3(0.06837F, 0.06837F, 0.06837F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0F, 1.16552F, -1.53651F), new Vector3(41.18788F, 180F, 180F), new Vector3(0.37516F, 0.37516F, 0.37516F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(-1.17808F, 0.82513F, -0.45502F), new Vector3(18.10892F, 0F, 270F), new Vector3(0.32643F, 0.32643F, 0.32643F));
                AddDisplayRule("MageBody", "Chest", new Vector3(0.0076F, 0.06786F, -0.36723F), new Vector3(0F, 90F, 7.532F), new Vector3(0.09947F, 0.09947F, 0.09947F));
                AddDisplayRule("MercBody", "Chest", new Vector3(0F, 0.19384F, -0.33253F), new Vector3(76.33701F, 180F, 180F), new Vector3(0.0591F, 0.0591F, 0.0591F));
                AddDisplayRule("TreebotBody", "PlatformBase", new Vector3(-0.67877F, -0.32711F, 0.14521F), new Vector3(342.8023F, 13.54228F, 295.1575F), new Vector3(0.11323F, 0.11323F, 0.11323F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(-0.20239F, -0.154F, -0.16236F), new Vector3(352.0321F, 86.04962F, 348.7805F), new Vector3(0.07898F, 0.07898F, 0.07898F));
                AddDisplayRule("CrocoBody", "SpineChest2", new Vector3(0.93823F, 0.57234F, -0.45359F), new Vector3(0F, 0F, 0F), new Vector3(0.78285F, 0.78285F, 0.78285F));
                AddDisplayRule("CaptainBody", "Stomach", new Vector3(-0.38752F, 0.0754F, -0.06176F), new Vector3(331.73F, 2.09717F, 350.4285F), new Vector3(0.07681F, 0.07681F, 0.07681F));
                AddDisplayRule("BrotherBody", "Pelvis", BrotherInfection.red, new Vector3(0.21854F, 0.02853F, -0.08466F), new Vector3(15.7255F, 37.56055F, 160.5855F), new Vector3(0.09165F, 0.09466F, 0.09466F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Pelvis", new Vector3(-0.28916F, 0.15725F, 0.00001F), new Vector3(0F, 0F, 349.5569F), new Vector3(0.09291F, 0.09291F, 0.09291F));
                AddDisplayRule("RailgunnerBody", "Pelvis", new Vector3(0.11127F, 0.04029F, 0.06485F), new Vector3(0F, 0F, 165.402F), new Vector3(0.06943F, 0.06943F, 0.06943F));
                AddDisplayRule("VoidSurvivorBody", "Stomach", new Vector3(0.02735F, -0.00002F, 0.10816F), new Vector3(-0.00002F, 93.88837F, 10.28838F), new Vector3(0.06575F, 0.06575F, 0.06575F));
            };

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;

            On.RoR2.UI.HealthBar.UpdateBarInfos += HealthBar_UpdateBarInfos;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var itemCount = sender.inventory.GetItemCount(itemDef);
                if (itemCount > 0) {
                    args.baseShieldAdd += sender.maxHealth * sender.cursePenalty * passiveShield / 100f;
                    args.shieldMultAdd += (shieldBonus + shieldBonusPerStack * (float)(itemCount - 1)) / 100f;
                }
            }
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            self.AddItemBehavior<MysticsItemsShieldUpgradeBehaviour>(self.inventory.GetItemCount(itemDef));
        }

        public class MysticsItemsShieldUpgradeBehaviour : CharacterBody.ItemBehavior
        {
            public HealthComponent healthComponent;

            public void Start()
            {
                healthComponent = GetComponent<HealthComponent>();
                if (healthComponent)
                {
                    if (!healthComponentsWithUpgradedShields.Contains(healthComponent))
                        healthComponentsWithUpgradedShields.Add(healthComponent);
                }
            }

            public void FixedUpdate()
            {
                body.outOfDangerStopwatch += (rechargeBoost + rechargeBoostPerStack * (float)(stack - 1)) / 100f * Time.fixedDeltaTime;
            }

            public void OnEnable()
            {
                if (healthComponent)
                {
                    if (!healthComponentsWithUpgradedShields.Contains(healthComponent))
                        healthComponentsWithUpgradedShields.Add(healthComponent);
                }
            }

            public void OnDisable()
            {
                if (healthComponent)
                {
                    if (healthComponentsWithUpgradedShields.Contains(healthComponent))
                        healthComponentsWithUpgradedShields.Remove(healthComponent);
                }
            }
        }

        public static List<HealthComponent> healthComponentsWithUpgradedShields = new List<HealthComponent>();

        private void HealthBar_UpdateBarInfos(On.RoR2.UI.HealthBar.orig_UpdateBarInfos orig, RoR2.UI.HealthBar self)
        {
            orig(self);

            if (self.source && healthComponentsWithUpgradedShields.Contains(self.source))
            {
                ref RoR2.UI.HealthBar.BarInfo shieldBarStyle = ref self.barInfoCollection.shieldBarInfo;
                Color.RGBToHSV(shieldBarStyle.color, out var shieldHue, out var shieldSat, out var shieldVal);
                shieldHue += Mathf.Sin(Time.fixedTime * 3.4f) * 0.03f;
                if (shieldHue < 0f) shieldHue += 1f;
                if (shieldHue > 1f) shieldHue -= 1f;
                shieldSat = Mathf.Clamp01(shieldSat - 0.05f + 0.1f * Mathf.Sin(Time.fixedTime * 6f));
                shieldVal = Mathf.Clamp01(shieldVal * 1.15f);
                shieldBarStyle.color = Color.HSVToRGB(shieldHue, shieldSat, shieldVal);
                shieldBarStyle.sizeDelta += 4f;
            }
        }
    }
}
