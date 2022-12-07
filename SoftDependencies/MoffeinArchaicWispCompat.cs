namespace MysticsItems.SoftDependencies
{
    internal static class MoffeinArchaicWispCompat
    {
        internal static void Init()
        {
            RoR2Application.onLoad += ReplaceArchaicMaskSpawnCard;
        }

        private static void ReplaceArchaicMaskSpawnCard()
        {
            //Add ArchaicWisps.dll as a softdependency.
            MysticsItems.Equipment.ArchaicMask.ArchWispSpawnCard = ArchaicWisp.ArchaicWispContent.ArchaicWispCard.Card.spawnCard;
        }
    }
}