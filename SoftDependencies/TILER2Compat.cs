namespace MysticsItems.SoftDependencies
{
    internal static class TILER2Compat
    {
        internal static void Init()
        {
            RoR2.ItemCatalog.availability.CallWhenAvailable(AddItemsToFakeInventoryBlacklist);
        }

        private static void AddItemsToFakeInventoryBlacklist()
        {
            TILER2.FakeInventory.blacklist.Add(MysticsItemsContent.Items.MysticsItems_MarwanAsh1);
            TILER2.FakeInventory.blacklist.Add(MysticsItemsContent.Items.MysticsItems_MarwanAsh2);
            TILER2.FakeInventory.blacklist.Add(MysticsItemsContent.Items.MysticsItems_MarwanAsh3);
        }
    }
}
