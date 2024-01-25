using BepInEx.Configuration;
using MysticsRisky2Utils;
using System;

namespace MysticsItems.SoftDependencies
{
    internal static class SoftDependenciesCore
    {
        internal static bool itemStatsEnabled = false;
        internal static bool itemDisplaysSniper = false;
        internal static bool itemDisplaysDeputy = false;
        internal static bool itemDisplaysChirr = false;

        internal static void Init()
        {
            var pluginInfos = BepInEx.Bootstrap.Chainloader.PluginInfos;
            if (pluginInfos.ContainsKey("com.xoxfaby.BetterUI"))
            {
                try
                {
                    BetterUICompat.Init();
                    itemStatsEnabled = true;
                }
                catch (Exception e) { Main.logger.LogError(e); }
            }
            if (pluginInfos.ContainsKey("dev.ontrigger.itemstats"))
            {
                try
                {
                    ItemStatsCompat.Init();
                    itemStatsEnabled = true;
                }
                catch (Exception e) { Main.logger.LogError(e); }
            }
            if (pluginInfos.ContainsKey("com.KingEnderBrine.ProperSave"))
            {
                try
                {
                    ProperSaveCompat.Init();
                }
                catch (Exception e) { Main.logger.LogError(e); }
            }
            if (pluginInfos.ContainsKey("aaaa.bubbet.whatamilookingat"))
            {
                try
                {
                    WhatAmILookingAtCompat.Init();
                }
                catch (Exception e) { Main.logger.LogError(e); }
            }
            if (pluginInfos.ContainsKey("com.ThinkInvisible.TILER2"))
            {
                try
                {
                    TILER2Compat.Init();
                }
                catch (Exception e) { Main.logger.LogError(e); }
            }
            if (pluginInfos.ContainsKey("com.Moffein.ArchaicWisp"))
            {
                try
                {
                    MoffeinArchaicWispCompat.Init();
                }
                catch (Exception e) { Main.logger.LogError(e); }
            }

            itemDisplaysSniper = ConfigOptions.ConfigurableValue.CreateBool(
                ConfigManager.General.categoryGUID,
                ConfigManager.General.categoryName,
                ConfigManager.General.config,
                "Mod Compatibility",
                "Sniper Item Displays",
                true,
                "Make this mod's items show up on the Sniper added by the SniperClassic mod",
                restartRequired: true
            ) && BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.SniperClassic");

            itemDisplaysDeputy = ConfigOptions.ConfigurableValue.CreateBool(
                ConfigManager.General.categoryGUID,
                ConfigManager.General.categoryName,
                ConfigManager.General.config,
                "Mod Compatibility",
                "Deputy Item Displays",
                true,
                "Make this mod's items show up on the modded character Deputy",
                restartRequired: true
            ) && BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Bog.Deputy");

            itemDisplaysChirr = ConfigOptions.ConfigurableValue.CreateBool(
                ConfigManager.General.categoryGUID,
                ConfigManager.General.categoryName,
                ConfigManager.General.config,
                "Mod Compatibility",
                "Chirr Item Displays",
                true,
                "Make this mod's items show up on Chirr added by the Starstorm 2 mod",
                restartRequired: true
            ) && BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm.Starstorm2");
        }
    }
}