using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MysticsItems
{
    public static class ContentToggleConfigManager
    {
        internal static ConfigEntry<bool> enabled;
        internal static ConfigEntry<bool> secrets;
        internal static ConfigEntry<bool> funEvents;
        internal static List<ItemIndex> disabledItems = new List<ItemIndex>();
        internal static List<EquipmentIndex> disabledEquipment = new List<EquipmentIndex>();

        internal static void Init()
        {
            enabled = MysticsItemsPlugin.configContentToggle.Bind(
                "! Enable Content Toggle Config",
                "EnableContentToggleConfig",
                true,
                "If enabled, mod content of your choice will not appear in a run.\r\nOtherwise, all mod content will be available."
            );

            secrets = MysticsItemsPlugin.configContentToggle.Bind(
                "Misc",
                "Secrets",
                true,
                "Enable secret content"
            );

            funEvents = MysticsItemsPlugin.configContentToggle.Bind(
                "Misc",
                "FunEvents",
                true,
                "Enable fun events that happen on specific dates"
            );

            Run.onRunSetRuleBookGlobal += (run, rulebook) =>
            {
                foreach (var disabledItem in disabledItems)
                {
                    if (run.availableItems.Contains(disabledItem))
                        run.availableItems.Remove(disabledItem);
                }
                foreach (var equipmentIndex in disabledEquipment)
                {
                    if (run.availableEquipment.Contains(equipmentIndex))
                        run.availableEquipment.Remove(equipmentIndex);
                }
                PickupDropTable.RegenerateAll(run);
            };

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
                        disabledItems.Add(itemDef.itemIndex);

                        On.RoR2.Language.GetLocalizedStringByToken += (orig, self, token) =>
                        {
                            var result = orig(self, token);
                            if (string.Equals(token, itemDef.nameToken, StringComparison.InvariantCulture))
                                result = "[X] <s>" + result + "</s>";
                            return result;
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
                        disabledEquipment.Add(equipmentDef.equipmentIndex);

                        On.RoR2.Language.GetLocalizedStringByToken += (orig, self, token) =>
                        {
                            var result = orig(self, token);
                            if (string.Equals(token, equipmentDef.nameToken, StringComparison.InvariantCulture))
                                result = "[X] <s>" + result + "</s>";
                            return result;
                        };
                    }
                }
            };
        }
    }
}