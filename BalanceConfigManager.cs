using BepInEx.Configuration;
using System.Collections.Generic;

namespace MysticsItems
{
    public static class BalanceConfigManager
    {
        internal static ConfigEntry<bool> enabled;

        internal static void Init()
        {
            enabled = MysticsItemsPlugin.configBalance.Bind(
                "! Enable Balance Config",
                "EnableBalanceConfig",
                false,
                "If enabled, mod items will use values of your choice.\r\nOtherwise, the mod will use default recommended values."
            );

            On.RoR2.Language.GetLocalizedStringByToken += Language_GetLocalizedStringByToken;
        }

        private static string Language_GetLocalizedStringByToken(On.RoR2.Language.orig_GetLocalizedStringByToken orig, RoR2.Language self, string token)
        {
            var result = orig(self, token);
            foreach (var configurableValue in ConfigurableValue.instancesList.FindAll(x => x.stringsToAffect.Contains(token)))
            {
                result = result.Replace("{" + configurableValue.key + "}", configurableValue.ToString());
            }
            return result;
        }

        public abstract class ConfigurableValue
        {
            public static List<ConfigurableValue> instancesList = new List<ConfigurableValue>();

            public List<string> stringsToAffect = new List<string>();
            public string key = "";
        }

        public class ConfigurableValue<T> : ConfigurableValue
        {
            private ConfigEntry<T> bepinexConfigEntry;
            private T defaultValue;

            public ConfigurableValue(string section, string key, T defaultValue, string description = "", List<string> stringsToAffect = null)
            {
                bepinexConfigEntry = MysticsItemsPlugin.configBalance.Bind<T>(section, key, defaultValue, description);
                this.key = key;
                this.defaultValue = defaultValue;
                if (stringsToAffect != null) this.stringsToAffect = stringsToAffect;

                instancesList.Add(this);
            }

            public T Value
            {
                get
                {
                    return enabled.Value ? bepinexConfigEntry.Value : defaultValue;
                }
            }

            public override string ToString()
            {
                return System.Convert.ToString(Value, System.Globalization.CultureInfo.InvariantCulture);
            }

            public static implicit operator T(ConfigurableValue<T> configurableValue)
            {
                return configurableValue.Value;
            }
        }

        public class ConfigurableCooldown : ConfigurableValue<float>
        {
            public ConfigurableCooldown(string section, float defaultValue) : base(section, "Cooldown", defaultValue)
            {

            }
        }

        public class ConfigurableEnigmaCompatibleBool : ConfigurableValue<bool>
        {
            public ConfigurableEnigmaCompatibleBool(string section, bool defaultValue) : base(section, "EnigmaCompatible", defaultValue, "Can be rolled by the Artifact of Enigma")
            {

            }
        }
    }
}
