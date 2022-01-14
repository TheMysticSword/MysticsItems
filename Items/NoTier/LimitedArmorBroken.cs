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
                "ITEM_MYSTICSITEMS_LIMITEDARMOR_DESC"
            }
        );

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_LimitedArmorBroken";
            itemDef.tier = ItemTier.NoTier;
            itemDef.canRemove = false;
            //itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Rift Lens Debuff/Icon.png");

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
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
