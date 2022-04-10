using BepInEx.Configuration;

namespace MysticsItems.SoftDependencies
{
    internal static class SoftDependenciesCore
    {
        internal static bool betterUICompatEnabled = false;
        internal static bool betterUIItemStatsEnabled = false;
        internal static bool itemStatsCompatEnabled = false;
        internal static bool itemDisplaysSniper = false;

        internal static void Init()
        {
            var pluginInfos = BepInEx.Bootstrap.Chainloader.PluginInfos;
            if (pluginInfos.ContainsKey("com.xoxfaby.BetterUI"))
            {
                try
                {
                    BetterUICompat.Init();
                    betterUICompatEnabled = true;
                    betterUIItemStatsEnabled = GeneralConfigManager.betterUICompatEnableItemStats.Value;
                }
                catch { }
            }
            if (pluginInfos.ContainsKey("dev.ontrigger.itemstats") && GeneralConfigManager.itemStatsCompatEnabledByConfig.Value)
            {
                try
                {
                    ItemStatsCompat.Init();
                    itemStatsCompatEnabled = true;
                }
                catch { }
            }
            if (pluginInfos.ContainsKey("com.KingEnderBrine.ProperSave") && GeneralConfigManager.properSaveCompatEnabledByConfig.Value)
            {
                try
                {
                    ProperSaveCompat.Init();
                }
                catch { }
            }
            itemDisplaysSniper = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.SniperClassic") && GeneralConfigManager.itemDisplaysSniper.Value;
        }
    }
}