using BepInEx.Configuration;
using System.Collections.Generic;
using MysticsRisky2Utils;
using System;

namespace MysticsItems
{
    public static class LegacyBalanceConfigManager
    {
        public class ConfigurableValue<T> : ConfigOptions.ConfigurableValue<T>
        {
            public ConfigurableValue(string section, string key, float defaultValue, string description = "", List<string> stringsToAffect = null, System.Action<float> onChanged = null) : base(ConfigManager.Balance.config, section, key, (T)Convert.ChangeType(defaultValue, typeof(T)), description, stringsToAffect, ConfigManager.Balance.ignore.bepinexConfigEntry)
            {
                ConfigOptions.ConfigurableValue.CreateFloat(ConfigManager.Balance.categoryGUID, ConfigManager.Balance.categoryName, ConfigManager.Balance.config, section, key, defaultValue, min: 0f, max: 1000000f, description: description, stringsToAffect: stringsToAffect, useDefaultValueConfigEntry: ConfigManager.Balance.ignore.bepinexConfigEntry, onChanged: onChanged);
            }

            public ConfigurableValue(string section, string key, int defaultValue, string description = "", List<string> stringsToAffect = null, System.Action<int> onChanged = null) : base(ConfigManager.Balance.config, section, key, (T)Convert.ChangeType(defaultValue, typeof(T)), description, stringsToAffect, ConfigManager.Balance.ignore.bepinexConfigEntry)
            {
                ConfigOptions.ConfigurableValue.CreateInt(ConfigManager.Balance.categoryGUID, ConfigManager.Balance.categoryName, ConfigManager.Balance.config, section, key, defaultValue, min: 0, max: 1000000, description: description, stringsToAffect: stringsToAffect, useDefaultValueConfigEntry: ConfigManager.Balance.ignore.bepinexConfigEntry, onChanged: onChanged);
            }

            public ConfigurableValue(string section, string key, bool defaultValue, string description = "", List<string> stringsToAffect = null, System.Action<bool> onChanged = null) : base(ConfigManager.Balance.config, section, key, (T)Convert.ChangeType(defaultValue, typeof(T)), description, stringsToAffect, ConfigManager.Balance.ignore.bepinexConfigEntry)
            {
                ConfigOptions.ConfigurableValue.CreateBool(ConfigManager.Balance.categoryGUID, ConfigManager.Balance.categoryName, ConfigManager.Balance.config, section, key, defaultValue, description: description, stringsToAffect: stringsToAffect, useDefaultValueConfigEntry: ConfigManager.Balance.ignore.bepinexConfigEntry);
            }
        }
    }
}
