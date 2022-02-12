using BepInEx.Configuration;

namespace MysticsItems.SoftDependencies
{
    internal static class SoftDependenciesCore
    {
        internal static bool betterUICompatEnabled = false;
        internal static ConfigEntry<bool> betterUICompatEnableOverrides = Main.configGeneral.Bind("Mod Compatibility", "BetterUICompatEnableOverrides", true, "Allow this mod to override certain BetterUI calculations (for example, adding Scratch Ticket chance bonus to the Crit Chance stat display).");
        internal static bool itemStatsCompatEnabled = false;
        internal static ConfigEntry<bool> itemStatsCompatEnabledByConfig = Main.configGeneral.Bind("Mod Compatibility", "ItemStatsCompatEnable", true, "Enable ItemStats integration");

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
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("dev.ontrigger.itemstats") && itemStatsCompatEnabledByConfig.Value)
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