using RoR2;

namespace MysticsItems.Items
{
    public class KeepShopTerminalOpenConsumed : BaseItem
    {
        public override void PreLoad()
        {
            itemDef.name = "KeepShopTerminalOpenConsumed";
            itemDef.tier = ItemTier.NoTier;
        }
    }
}
