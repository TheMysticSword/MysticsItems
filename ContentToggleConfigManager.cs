using BepInEx.Configuration;
using RoR2;
using System;
using System.Linq;

namespace MysticsItems
{
    public static class ContentToggleConfigManager
    {
        internal static ConfigEntry<bool> enabled;
        internal static ConfigEntry<bool> secrets;

        internal static void Init()
        {
            enabled = MysticsItemsPlugin.configContentToggle.Bind(
                "! Enable Content Toggle Config",
                "EnableContentToggleConfig",
                false,
                "If enabled, mod content of your choice will not appear in a run.\r\nOtherwise, all mod content will be available."
            );

            secrets = MysticsItemsPlugin.configContentToggle.Bind(
                "Misc",
                "Secrets",
                true,
                "Enable secret content"
            );

            RoR2Application.onLoad += () =>
            {
                var itemDefs = typeof(MysticsItemsContent.Items).GetFields().Select(x => x.GetValue(null) as ItemDef)
                    .Where(x => !x.hidden && x.inDroppableTier && x.DoesNotContainTag(ItemTag.WorldUnique)).ToList();
                var equipmentDefs = typeof(MysticsItemsContent.Equipment).GetFields().Select(x => x.GetValue(null) as EquipmentDef)
                    .Where(x => x.canDrop).ToList();

                string GetStr(string token)
                {
                    var str = Language.english.GetLocalizedStringByToken(token);
                    str = str.Replace("'", " ");
                    return str;
                }

                foreach (var itemDef in itemDefs)
                {
                    var enabledByConfig = MysticsItemsPlugin.configContentToggle.Bind<bool>(
                        "Items",
                        GetStr(itemDef.nameToken),
                        true,
                        GetStr(itemDef.descriptionToken)
                    );

                    if (!enabledByConfig.Value && enabled.Value)
                    {
                        itemDef.tier = ItemTier.NoTier;
                        HG.ArrayUtils.ArrayAppend(ref itemDef.tags, ItemTag.WorldUnique);

                        On.RoR2.Language.GetLocalizedStringByToken += (orig, self, token) =>
                        {
                            var result = orig(self, token);
                            if (string.Equals(token, itemDef.nameToken, StringComparison.InvariantCulture))
                                result = "[X] <s>" + result + "</s>";
                            return result;
                        };
                        Run.onRunSetRuleBookGlobal += (run, rulebook) =>
                        {
                            if (run.availableItems.Contains(itemDef.itemIndex))
                                run.availableItems.Remove(itemDef.itemIndex);
                        };
                    }
                }

                foreach (var equipmentDef in equipmentDefs)
                {
                    var enabledByConfig = MysticsItemsPlugin.configContentToggle.Bind<bool>(
                        "Equipment",
                        GetStr(equipmentDef.nameToken),
                        true,
                        GetStr(equipmentDef.descriptionToken)
                    );

                    if (!enabledByConfig.Value && enabled.Value)
                    {
                        equipmentDef.canDrop = false;
                        equipmentDef.enigmaCompatible = false;
                        equipmentDef.appearsInMultiPlayer = false;
                        equipmentDef.appearsInSinglePlayer = false;

                        On.RoR2.Language.GetLocalizedStringByToken += (orig, self, token) =>
                        {
                            var result = orig(self, token);
                            if (string.Equals(token, equipmentDef.nameToken, StringComparison.InvariantCulture))
                                result = "[X] <s>" + result + "</s>";
                            return result;
                        };
                        Run.onRunSetRuleBookGlobal += (run, rulebook) =>
                        {
                            if (run.availableEquipment.Contains(equipmentDef.equipmentIndex))
                                run.availableEquipment.Remove(equipmentDef.equipmentIndex);
                        };
                    }
                }
            };
        }
    }
}