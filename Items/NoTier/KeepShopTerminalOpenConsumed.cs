using MysticsRisky2Utils.BaseAssetTypes;
using RoR2;
using UnityEngine;

namespace MysticsItems.Items
{
    public class KeepShopTerminalOpenConsumed : BaseItem
    {
        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_KeepShopTerminalOpenConsumed";
            itemDef.tier = ItemTier.NoTier;
            itemDef.canRemove = false;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy
            };
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Platinum Card Consumed/Icon.png");
        }
    }
}
