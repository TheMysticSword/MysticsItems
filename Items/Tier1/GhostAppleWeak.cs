using RoR2;
using UnityEngine;
using Rewired.ComponentControls.Effects;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Items
{
    public class GhostAppleWeak : BaseItem
    {
        public static ConfigurableValue<float> regenWeak = new ConfigurableValue<float>(
            "Item: Ghost Apple",
            "RegenWeak",
            0.1f,
            "Regen increase from Apple Stem (the weaker version of the item)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_GHOSTAPPLE_DESC",
                "ITEM_MYSTICSITEMS_GHOSTAPPLEWEAK_DESC"
            }
        );

        internal static PickupIndex pickupIndex;

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_GhostAppleWeak";
            SetItemTierWhenAvailable(ItemTier.Tier1);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Healing,
                ItemTag.WorldUnique
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Ghost Apple/ModelWeak.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Ghost Apple/IconWeak.png");

            itemDisplayPrefab = PrepareItemDisplayModel(PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Ghost Apple/FollowerModelWeak.prefab")));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Head", new Vector3(0F, 0.42494F, -0.00003F), new Vector3(0F, 75.51852F, 0F), new Vector3(0.14794F, 0.14794F, 0.14794F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(-0.00688F, 0.34558F, -0.02777F), new Vector3(0F, 141.2308F, 0F), new Vector3(0.14014F, 0.14014F, 0.14014F));
                AddDisplayRule("Bandit2Body", "Head", new Vector3(-0.00001F, 0.24706F, 0.00001F), new Vector3(0F, 75.61176F, 0F), new Vector3(0.14831F, 0.14831F, 0.14831F));
                AddDisplayRule("ToolbotBody", "Head", new Vector3(-0.00036F, 3.17333F, 2.13747F), new Vector3(342.1709F, 254.1331F, 312.8712F), new Vector3(1.24647F, 1.24647F, 1.24647F));
                AddDisplayRule("EngiBody", "HeadCenter", new Vector3(-0.00027F, 0.21095F, 0.04202F), new Vector3(350.498F, 84.28171F, 10.41403F), new Vector3(0.20297F, 0.20297F, 0.20297F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(-0.40656F, 0.82802F, 0.68119F), new Vector3(0F, 70.46212F, 0F), new Vector3(0.11999F, 0.11999F, 0.11999F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0F, 1.55558F, -0.5225F), new Vector3(0F, 84.66918F, 0F), new Vector3(0.4507F, 0.4507F, 0.4507F));
                AddDisplayRule("MageBody", "Head", new Vector3(-0.00099F, 0.19318F, 0.00167F), new Vector3(359.2163F, 83.19808F, 0.29858F), new Vector3(0.07884F, 0.07884F, 0.07884F));
                AddDisplayRule("MercBody", "Head", new Vector3(-0.00121F, 0.28759F, 0.0697F), new Vector3(1.65033F, 84.70821F, 356.9272F), new Vector3(0.09783F, 0.09783F, 0.09783F));
                AddDisplayRule("TreebotBody", "MR2UAntennae4", new Vector3(0.00418F, 0.44485F, 0.01239F), new Vector3(0.00002F, 66.39205F, -0.00001F), new Vector3(0.15085F, 0.15085F, 0.15085F));
                AddDisplayRule("LoaderBody", "Head", new Vector3(-0.00143F, 0.27172F, -0.0008F), new Vector3(359.8729F, 81.98885F, 359.5234F), new Vector3(0.09703F, 0.09703F, 0.09703F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(0.06233F, 0.7062F, 1.7446F), new Vector3(354.7835F, 269.711F, 273.1755F), new Vector3(1.4763F, 1.4763F, 1.4763F));
                AddDisplayRule("CaptainBody", "Head", new Vector3(0.00205F, 0.29816F, -0.01297F), new Vector3(351.7289F, 65.7328F, 335.7521F), new Vector3(0.12855F, 0.12855F, 0.12855F));
                AddDisplayRule("BrotherBody", "Head", BrotherInfection.white, new Vector3(-0.00336F, 0.09665F, 0.01567F), new Vector3(0F, 0F, 254.1354F), new Vector3(0.08227F, 0.04507F, 0.04507F));
                AddDisplayRule("ScavBody", "Chest", new Vector3(0.61628F, 6.26989F, 1.74307F), new Vector3(0F, 270.1664F, 338.6173F), new Vector3(3.64876F, 3.64876F, 3.64876F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Head", new Vector3(0.00045F, 0.07111F, -0.26945F), new Vector3(349.7505F, 91.35583F, 262.4232F), new Vector3(0.11431F, 0.11431F, 0.11431F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysDeputy) AddDisplayRule("DeputyBody", "Head", new Vector3(-0.07958F, 0.02501F, 0.14674F), new Vector3(291.0633F, 38.47263F, 82.53133F), new Vector3(0.07574F, 0.07574F, 0.07574F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysChirr) AddDisplayRule("ChirrBody", "Head", new Vector3(-0.41563F, 0.22439F, 0.31753F), new Vector3(347.2054F, 183.8303F, 49.95218F), new Vector3(0.29719F, 0.29719F, 0.29719F));
                AddDisplayRule("RailgunnerBody", "Head", new Vector3(-0.00157F, 0.2266F, -0.12129F), new Vector3(350.7117F, 87.64185F, 310.2009F), new Vector3(0.09104F, 0.09104F, 0.09104F));
                AddDisplayRule("VoidSurvivorBody", "Head", new Vector3(0.00001F, 0.23286F, 0.00004F), new Vector3(0F, 93.11852F, 0F), new Vector3(0.11628F, 0.11628F, 0.11628F));
            };

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;

            RoR2Application.onLoad += () =>
            {
                pickupIndex = PickupCatalog.FindPickupIndex(itemDef.itemIndex);
            };

            ConfigOptions.ConfigurableValue.CreateBool(
                ConfigManager.Balance.categoryGUID,
                ConfigManager.Balance.categoryName,
                ConfigManager.Balance.config,
                "Item: Ghost Apple",
                "Untiered Apple Stem",
                false,
                "If enabled, Apple Stem will be untiered instead of white tier, making it unscrappable.",
                useDefaultValueConfigEntry: ConfigManager.Balance.ignore.bepinexConfigEntry,
                onChanged: (newValue) =>
                {
                    SetItemTierWhenAvailable(newValue ? ItemTier.NoTier : ItemTier.Tier1);
                    itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>(newValue ? "Assets/Items/Ghost Apple/IconWeakUntiered.png" : "Assets/Items/Ghost Apple/IconWeak.png");
                }
            );
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            Inventory inventory = sender.inventory;
            if (inventory)
            {
                int itemCount = inventory.GetItemCount(itemDef);
                if (itemCount > 0)
                {
                    args.baseRegenAdd += regenWeak * itemCount * (1f + 0.2f * (sender.level - 1f));
                }
            }
        }
    }
}
