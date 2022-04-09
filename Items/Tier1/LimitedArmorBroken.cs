using RoR2;
using UnityEngine;
using Rewired.ComponentControls.Effects;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.BalanceConfigManager;

namespace MysticsItems.Items
{
    public class LimitedArmorBroken : BaseItem
    {
        public static ConfigurableValue<float> brokenArmor = new ConfigurableValue<float>(
            "Item: Cutesy Bow",
            "BrokenArmor",
            2f,
            "Armor increase from Frayed Bow (the weaker version of the item)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_LIMITEDARMOR_DESC",
                "ITEM_MYSTICSITEMS_LIMITEDARMORBROKEN_DESC"
            }
        );

        internal static PickupIndex pickupIndex;

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_LimitedArmorBroken";
            itemDef.tier = ItemTier.Tier1;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.WorldUnique
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Bow/ModelFrayed.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Bow/IconFrayed.png");

            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            HopooShaderToMaterial.Standard.DisableEverything(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial);
            HopooShaderToMaterial.Standard.DisableEverything(itemDisplayPrefab.GetComponentInChildren<Renderer>().sharedMaterial);
            itemDisplayPrefab.transform.localScale *= 0.8f;
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Head", new Vector3(0.09206F, 0.33178F, -0.09136F), new Vector3(16.03024F, 63.62674F, 304.511F), new Vector3(0.02301F, 0.02301F, 0.02301F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(0.01367F, 0.21655F, -0.17495F), new Vector3(357.0015F, 65.06252F, 277.7556F), new Vector3(0.02242F, 0.02242F, 0.02242F));
                AddDisplayRule("Bandit2Body", "Head", new Vector3(0.05274F, 0.09501F, -0.10753F), new Vector3(350.0532F, 63.19105F, 297.9137F), new Vector3(0.02805F, 0.02805F, 0.02805F));
                AddDisplayRule("ToolbotBody", "Head", new Vector3(-1.5345F, 1.70927F, 2.3488F), new Vector3(25.30116F, 73.93195F, 115.0976F), new Vector3(0.31308F, 0.31308F, 0.31308F));
                AddDisplayRule("EngiBody", "HeadCenter", new Vector3(0.05794F, 0.12538F, -0.14704F), new Vector3(0.49978F, 72.40844F, 310.3805F), new Vector3(0.03243F, 0.03243F, 0.03243F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(-0.89207F, 0.72175F, -0.23478F), new Vector3(318.9225F, 103.5389F, 353.6144F), new Vector3(0.11999F, 0.11999F, 0.11999F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(-0.73947F, 1.2767F, -0.80614F), new Vector3(324.2756F, 119.1527F, 340.6565F), new Vector3(0.10744F, 0.1307F, 0.10504F) );
                AddDisplayRule("MageBody", "Head", new Vector3(0.06019F, 0.16738F, -0.14399F), new Vector3(343.2434F, 235.5832F, 58.464F), new Vector3(0.02055F, 0.02055F, 0.02055F));
                AddDisplayRule("MercBody", "Head", new Vector3(0.09912F, 0.20856F, -0.03218F), new Vector3(346.0291F, 221.2525F, 67.84089F), new Vector3(0.02243F, 0.02243F, 0.02243F));
                AddDisplayRule("TreebotBody", "MIAntennae4", new Vector3(-0.00461F, 0.14457F, 0.00345F), new Vector3(-0.00002F, 36.79062F, 272.2785F), new Vector3(0.0496F, 0.0496F, 0.0496F));
                AddDisplayRule("LoaderBody", "Head", new Vector3(0.07075F, 0.22212F, -0.05101F), new Vector3(16.6799F, 61.79085F, 306.0373F), new Vector3(0.02181F, 0.02181F, 0.02181F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(1.45411F, 0.59183F, 1.08829F), new Vector3(56.441F, 150.6051F, 281.6682F), new Vector3(0.33415F, 0.59025F, 0.33415F));
                AddDisplayRule("CaptainBody", "Chest", new Vector3(0.00136F, 0.37981F, 0.1799F), new Vector3(350.816F, 280.0829F, 276.5578F), new Vector3(0.02509F, 0.02509F, 0.02509F));
                AddDisplayRule("BrotherBody", "Head", BrotherInfection.white, new Vector3(0.18348F, 0.10434F, 0.01567F), new Vector3(0F, 0F, 295.3276F), new Vector3(0.08227F, 0.04507F, 0.04507F));
                AddDisplayRule("ScavBody", "Chest", new Vector3(3.64754F, 6.52544F, -3.91259F), new Vector3(342.9161F, 287.3514F, 19.34576F), new Vector3(1.36506F, 1.40266F, 1.36506F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Head", new Vector3(0.07554F, -0.03519F, -0.19979F), new Vector3(348.2051F, 48.22592F, 228.5832F), new Vector3(0.02543F, 0.02543F, 0.02543F));
            };

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;

            RoR2Application.onLoad += () =>
            {
                pickupIndex = PickupCatalog.FindPickupIndex(itemDef.itemIndex);
            };
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            Inventory inventory = sender.inventory;
            if (inventory)
            {
                int itemCount = inventory.GetItemCount(itemDef);
                if (itemCount > 0)
                {
                    args.armorAdd += brokenArmor * itemCount;
                }
            }
        }
    }
}
