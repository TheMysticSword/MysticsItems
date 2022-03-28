using BepInEx.Configuration;
using System;

namespace MysticsItems
{
    public static class GeneralConfigManager
    {
        // Gameplay
        //public static ConfigEntry<bool> backpackEnableSkillFixes;

        // Mod Compatibility
        public static ConfigEntry<bool> betterUICompatEnableOverrides;
        public static ConfigEntry<bool> itemStatsCompatEnabledByConfig;
        public static ConfigEntry<bool> itemDisplaysSniper;
        
        // UI
        public static ConfigEntry<bool> rhythmHudUnderCrosshair;
        public static ConfigEntry<bool> rhythmHudOverSkills;
        public static ConfigEntry<bool> rhythmHudComboText;
        
        internal static void Init()
        {
            /*
            backpackEnableSkillFixes = MysticsItemsPlugin.configGeneral.Bind<bool>(
                "Gameplay",
                "BackpackEnableSkillFixes",
                true,
                "Make certain skills require pressing a key instead of holding it down while carrying the Hikers Backpack item to fix these skills consuming all charges at once."
            );
            */

            betterUICompatEnableOverrides = MysticsItemsPlugin.configGeneral.Bind("Mod Compatibility", "BetterUICompatEnableOverrides", true, "Allow this mod to override certain BetterUI calculations (for example, adding Scratch Ticket chance bonus to the Crit Chance stat display).");
            itemStatsCompatEnabledByConfig = MysticsItemsPlugin.configGeneral.Bind("Mod Compatibility", "ItemStatsCompatEnable", true, "Enable ItemStats integration");
            itemDisplaysSniper = MysticsItemsPlugin.configGeneral.Bind("Mod Compatibility", "ItemDisplaysSniper", true, "Make this mod's items show up on the Sniper added by SniperClassic mod");

            rhythmHudUnderCrosshair = MysticsItemsPlugin.configGeneral.Bind("UI", "RhythmItemHUDUnderCrosshair", true, "Enable Metronome's HUD indicator under the crosshair.");
            rhythmHudOverSkills = MysticsItemsPlugin.configGeneral.Bind("UI", "RhythmItemHUDOverSkills", true, "Enable Metronome's HUD indicator over skill cooldown icons.");
            rhythmHudComboText = MysticsItemsPlugin.configGeneral.Bind("UI", "RhythmItemHUDComboText", true, "Enable the combo counter near Metronome's HUD indicators.");
        }
    }
}