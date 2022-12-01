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
            /*
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Voltmeter/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Voltmeter/Icon.png");
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Stomach", new Vector3(0.042F, 0.096F, -0.119F), new Vector3(4.394F, 73.472F, 2.074F), new Vector3(0.02F, 0.02F, 0.02F));
                AddDisplayRule("HuntressBody", "Pelvis", new Vector3(0.077F, -0.096F, 0.09F), new Vector3(358.51F, 111.069F, 197.846F), new Vector3(0.018F, 0.018F, 0.018F));
                AddDisplayRule("Bandit2Body", "Chest", new Vector3(0.011F, 0.206F, -0.199F), new Vector3(0.762F, 87.312F, 5.102F), new Vector3(0.022F, 0.022F, 0.022F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(0.064F, 1.168F, 3.419F), new Vector3(0F, 270F, 0F), new Vector3(0.214F, 0.214F, 0.214F));
                AddDisplayRule("EngiBody", "HandR", new Vector3(0.026F, -0.123F, 0.048F), new Vector3(275.324F, 252.038F, 18.986F), new Vector3(0.029F, 0.029F, 0.029F));
                AddDisplayRule("EngiTurretBody", "Neck", new Vector3(0F, 0.6F, -0.168F), new Vector3(0F, 90F, 0F), new Vector3(0.133F, 0.133F, 0.133F));
                AddDisplayRule("EngiWalkerTurretBody", "Neck", new Vector3(0F, 0.533F, -0.168F), new Vector3(0F, 90F, 0F), new Vector3(0.133F, 0.133F, 0.133F));
                AddDisplayRule("MageBody", "Chest", new Vector3(0F, 0.064F, -0.327F), new Vector3(0F, 90F, 7.532F), new Vector3(0.039F, 0.039F, 0.039F));
                AddDisplayRule("MercBody", "Head", new Vector3(0F, 0.105F, -0.143F), new Vector3(0F, 90F, 19.781F), new Vector3(0.02F, 0.02F, 0.02F));
                AddDisplayRule("TreebotBody", "PlatformBase", new Vector3(0.537F, -0.273F, 0.135F), new Vector3(7.036F, 343.597F, 322.203F), new Vector3(0.063F, 0.063F, 0.063F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(0F, -0.051F, -0.159F), new Vector3(0F, 90F, 0F), new Vector3(0.037F, 0.037F, 0.037F));
                AddDisplayRule("CrocoBody", "SpineChest1", new Vector3(-0.951F, 0.633F, -0.312F), new Vector3(4.342F, 131.684F, 70.801F), new Vector3(0.254F, 0.254F, 0.254F));
                AddDisplayRule("CaptainBody", "HandL", new Vector3(0.009F, 0.167F, 0.045F), new Vector3(270F, 270F, 0F), new Vector3(0.035F, 0.035F, 0.035F));
                AddDisplayRule("BrotherBody", "UpperArmL", BrotherInfection.red, new Vector3(0.124F, 0.177F, -0.056F), new Vector3(80.946F, 113.634F, 258.867F), new Vector3(0.061F, 0.063F, 0.063F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Chest", new Vector3(0.00102F, 0.07635F, -0.2573F), new Vector3(0F, 90F, 342.5916F), new Vector3(0.04182F, 0.04182F, 0.04182F));
                AddDisplayRule("RailgunnerBody", "Backpack", new Vector3(0F, 0.4146F, 0.05697F), new Vector3(0F, 90F, 0F), new Vector3(0.02963F, 0.02963F, 0.02963F));
                AddDisplayRule("VoidSurvivorBody", "Chest", new Vector3(0.09556F, 0.08424F, 0.189F), new Vector3(18.8347F, 291.3285F, 8.91174F), new Vector3(0.0247F, 0.0247F, 0.0247F));
            };
            */

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
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
            }

            public void FixedUpdate()
            {
                body.outOfDangerStopwatch += (rechargeBoost + rechargeBoostPerStack * (float)(stack - 1)) / 100f * Time.fixedDeltaTime;
            }
        }
    }
}
