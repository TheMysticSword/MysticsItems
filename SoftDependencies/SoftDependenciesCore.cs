using BepInEx.Configuration;

namespace MysticsItems.SoftDependencies
{
    internal static class SoftDependenciesCore
    {
        internal static bool betterUICompatEnabled = false;
        internal static bool itemStatsCompatEnabled = false;

        internal static void Init()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.xoxfaby.BetterUI"))
            {
                try
                {
                    BetterUICompat.Init();
                    betterUICompatEnabled = true;
                }
                catch { }
            }
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("dev.ontrigger.itemstats") && GeneralConfigManager.itemStatsCompatEnabledByConfig.Value)
            {
                try
                {
                    ItemStatsCompat.Init();
                    itemStatsCompatEnabled = true;
                }
                catch { }
            }
        }
    }
}