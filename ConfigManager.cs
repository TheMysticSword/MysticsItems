using BepInEx;
using BepInEx.Configuration;
using MysticsRisky2Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace MysticsItems
{
    public static class ConfigManager
    {
        internal static void Init()
        {
            if (MysticsRisky2Utils.SoftDependencies.SoftDependencyManager.RiskOfOptionsDependency.enabled)
            {
                var modIcon = Main.AssetBundle.LoadAsset<Sprite>("Assets/Misc/Textures/MysticsItemsIcon.png");
                MysticsRisky2Utils.SoftDependencies.SoftDependencyManager.RiskOfOptionsDependency.RegisterModInfo(General.categoryGUID, General.categoryName, "General settings of Mystic's Items, a large-scale item mod", modIcon);
                MysticsRisky2Utils.SoftDependencies.SoftDependencyManager.RiskOfOptionsDependency.RegisterModInfo(Balance.categoryGUID, Balance.categoryName, "Configure the values of items and equipment from Mystic's Items", modIcon);
            }

            General.Init();
        }

        public static class General
        {
            public static ConfigFile config = new ConfigFile(Paths.ConfigPath + "\\MysticsItems_General.cfg", true);
            public static string categoryName = "Mystic's Items (General)";
            public static string categoryGUID = MysticsItemsPlugin.PluginGUID + "_general";

            public static ConfigOptions.ConfigurableValue<bool> secrets = ConfigOptions.ConfigurableValue.CreateBool(
                categoryGUID,
                categoryName,
                config,
                "Misc",
                "Secrets",
                true,
                "Enable secret optional content",
                onChanged: (newValue) =>
                {
                    if (onSecretsToggled != null) onSecretsToggled(newValue);
                }
            );
            public static Action<bool> onSecretsToggled;

            internal static void Init()
            {
                InitItemAndEquipmentDisabling();
            }

            private static void InitItemAndEquipmentDisabling()
            {
                Run.onRunSetRuleBookGlobal += (run, rulebook) =>
                {
                    foreach (var disabledItem in disabledItems.Keys)
                    {
                        if (run.availableItems.Contains(disabledItem))
                            run.availableItems.Remove(disabledItem);
                    }
                    foreach (var equipmentIndex in disabledEquipment.Keys)
                    {
                        if (run.availableEquipment.Contains(equipmentIndex))
                            run.availableEquipment.Remove(equipmentIndex);
                    }
                    PickupDropTable.RegenerateAll(run);
                };

                On.RoR2.Artifacts.EnigmaArtifactManager.OnRunStartGlobal += (orig, run) =>
                {
                    orig(run);
                    if (NetworkServer.active)
                    {
                        RoR2.Artifacts.EnigmaArtifactManager.validEquipment.RemoveAll(x => disabledEquipment.ContainsKey(x));
                    }
                };

                RoR2Application.onLoad += () =>
                {
                    var itemDefs = typeof(MysticsItemsContent.Items).GetFields().Select(x => x.GetValue(null) as ItemDef)
                        .Where(x => !x.hidden && x.inDroppableTier).ToList();
                    var equipmentDefs = typeof(MysticsItemsContent.Equipment).GetFields().Select(x => x.GetValue(null) as EquipmentDef)
                        .Where(x => x.canDrop).ToList();

                    List<string> richTags = new List<string>()
                    {
                        "align", "color", "alpha", "b", "i", "cspace", "font", "indent", "line-height", "line-indent", "link",
                        "lowercase", "uppercase", "smallcaps", "margin", "mark", "mspace", "noparse", "nobr", "page", "pos", "size",
                        "space", "sprite", "s", "u", "style", "sub", "sup", "voffset", "width"
                    };
                    string GetSanitizedStringFromToken(string token)
                    {
                        var initialString = Language.english.GetLocalizedStringByToken(token);
                        var finalString = "";

                        var inRichTag = false;
                        var wasInRichTag = false;
                        while (initialString.Length > 0)
                        {
                            var chr = initialString[0];
                            switch (chr)
                            {
                                case '<':
                                    foreach (var richTag in richTags)
                                        if ((initialString.IndexOf(richTag, StringComparison.InvariantCulture) == 1 || initialString.IndexOf("/" + richTag, StringComparison.InvariantCulture) == 1) && initialString.Contains(">"))
                                        {
                                            inRichTag = true;
                                            wasInRichTag = true;
                                            break;
                                        }
                                    break;
                                case '>':
                                    inRichTag = false;
                                    break;
                            }
                            if (!inRichTag)
                            {
                                if (!wasInRichTag)
                                {
                                    switch (chr)
                                    {
                                        case '=':
                                        case '\n':
                                        case '\t':
                                        case '\\':
                                        case '"':
                                        case '\'':
                                        case '[':
                                        case ']':
                                            finalString += " ";
                                            break;
                                        default:
                                            finalString += chr;
                                            break;
                                    }
                                }
                                wasInRichTag = false;
                            }
                            initialString = initialString.Remove(0, 1);
                        }

                        return finalString;
                    }

                    foreach (var itemDef in itemDefs)
                    {
                        ConfigOptions.ConfigurableValue.CreateBool(
                            categoryGUID,
                            categoryName,
                            config,
                            "Enabled Items",
                            GetSanitizedStringFromToken(itemDef.nameToken),
                            true,
                            "Should this item be enabled? Changes to this value take effect only at the start of a run. Item description: " + GetSanitizedStringFromToken(itemDef.pickupToken),
                            onChanged: (newValue) =>
                            {
                                if (!newValue)
                                {
                                    disabledItems.Add(itemDef.itemIndex, new DisabledItem
                                    {
                                        previousTier = itemDef.tier,
                                        wasWorldUnique = itemDef.ContainsTag(ItemTag.WorldUnique)
                                    });
                                    itemDef.tier = ItemTier.NoTier;
                                    itemDef.deprecatedTier = ItemTier.NoTier;
                                    HG.ArrayUtils.ArrayAppend(ref itemDef.tags, ItemTag.WorldUnique);
                                }
                                else
                                {
                                    if (disabledItems.ContainsKey(itemDef.itemIndex))
                                    {
                                        var disabledItem = disabledItems[itemDef.itemIndex];
                                        itemDef.tier = disabledItem.previousTier;
                                        itemDef.deprecatedTier = disabledItem.previousTier;
                                        if (!disabledItem.wasWorldUnique)
                                        {
                                            HG.ArrayUtils.ArrayRemoveAtAndResize(ref itemDef.tags, Array.IndexOf(itemDef.tags, ItemTag.WorldUnique));
                                        }
                                        disabledItems.Remove(itemDef.itemIndex);
                                    }
                                }
                            }
                        );
                    }

                    foreach (var equipmentDef in equipmentDefs)
                    {
                        ConfigOptions.ConfigurableValue.CreateBool(
                            categoryGUID,
                            categoryName,
                            config,
                            "Enabled Equipment",
                            GetSanitizedStringFromToken(equipmentDef.nameToken),
                            true,
                            "Should this equipment be enabled? Changes to this value take effect only at the start of a run. Equipment description: " + GetSanitizedStringFromToken(equipmentDef.pickupToken),
                            onChanged: (newValue) =>
                            {
                                if (!newValue)
                                {
                                    disabledEquipment.Add(equipmentDef.equipmentIndex, new DisabledEquipment
                                    {
                                        appearedInSinglePlayer = equipmentDef.appearsInSinglePlayer,
                                        appearedInMultiPlayer = equipmentDef.appearsInMultiPlayer
                                    });

                                    equipmentDef.canDrop = false;
                                    equipmentDef.appearsInMultiPlayer = false;
                                    equipmentDef.appearsInSinglePlayer = false;
                                }
                                else
                                {
                                    if (disabledEquipment.ContainsKey(equipmentDef.equipmentIndex))
                                    {
                                        var _disabledEquipment = disabledEquipment[equipmentDef.equipmentIndex];
                                        equipmentDef.canDrop = true;
                                        equipmentDef.appearsInSinglePlayer = _disabledEquipment.appearedInSinglePlayer;
                                        equipmentDef.appearsInMultiPlayer = _disabledEquipment.appearedInMultiPlayer;
                                        disabledEquipment.Remove(equipmentDef.equipmentIndex);
                                    }
                                }
                            }
                        );
                    }
                };
            }
            
            public struct DisabledItem
            {
                public ItemTier previousTier;
                public bool wasWorldUnique;
            }
            public static Dictionary<ItemIndex, DisabledItem> disabledItems = new Dictionary<ItemIndex, DisabledItem>();

            public struct DisabledEquipment
            {
                public bool appearedInSinglePlayer;
                public bool appearedInMultiPlayer;
            }
            public static Dictionary<EquipmentIndex, DisabledEquipment> disabledEquipment = new Dictionary<EquipmentIndex, DisabledEquipment>();
        }

        public static class Balance
        {
            public static ConfigFile config = new ConfigFile(Paths.ConfigPath + "\\MysticsItems_Balance.cfg", true);
            public static string categoryName = "Mystic's Items (Balance)";
            public static string categoryGUID = MysticsItemsPlugin.PluginGUID + "_balance";

            public static ConfigOptions.ConfigurableValue<bool> enabled = ConfigOptions.ConfigurableValue.CreateBool(
                categoryGUID,
                categoryName,
                config,
                "! Enabled !",
                "Enable Balance Config",
                false,
                "If enabled, items from this mod will use values of your choice. Otherwise, the mod will use default recommended values."
            );

            public static void CreateEquipmentCooldownOption(EquipmentDef equipmentDef, string section, float defaultValue)
            {
                ConfigOptions.ConfigurableValue<float> configurableValue = null;
                configurableValue = ConfigOptions.ConfigurableValue.CreateFloat(
                    categoryGUID,
                    categoryName,
                    config,
                    section,
                    "Cooldown",
                    defaultValue,
                    0f,
                    1000f,
                    onChanged: (newValue) =>
                    {
                        if (equipmentDef) equipmentDef.cooldown = newValue;
                    }
                );
            }

            public static void CreateEquipmentEnigmaCompatibleOption(EquipmentDef equipmentDef, string section, bool defaultValue)
            {
                ConfigOptions.ConfigurableValue.CreateBool(
                    categoryGUID,
                    categoryName,
                    config,
                    section,
                    "Enigma Compatible",
                    defaultValue,
                    "Can be rolled by the Artifact of Enigma",
                    onChanged: (newValue) =>
                    {
                        if (equipmentDef)
                        {
                            equipmentDef.enigmaCompatible = newValue;
                            var list = EquipmentCatalog.enigmaEquipmentList;
                            if (list != null && equipmentDef._equipmentIndex != EquipmentIndex.None)
                            {
                                var contains = list.Contains(equipmentDef.equipmentIndex);
                                if (newValue != contains)
                                {
                                    if (contains) list.Remove(equipmentDef.equipmentIndex);
                                    else list.Add(equipmentDef.equipmentIndex);
                                }
                            }
                        }
                    }
                );
            }

            public static void CreateEquipmentCanBeRandomlyTriggeredOption(EquipmentDef equipmentDef, string section, bool defaultValue)
            {
                ConfigOptions.ConfigurableValue.CreateBool(
                    categoryGUID,
                    categoryName,
                    config,
                    section,
                    "Can Be Randomly Triggered",
                    defaultValue,
                    "Can be rolled by the Bottled Chaos item from the Survivors of the Void DLC",
                    onChanged: (newValue) =>
                    {
                        if (equipmentDef)
                        {
                            equipmentDef.canBeRandomlyTriggered = newValue;
                            var list = EquipmentCatalog.randomTriggerEquipmentList;
                            if (list != null && equipmentDef._equipmentIndex != EquipmentIndex.None)
                            {
                                var contains = list.Contains(equipmentDef.equipmentIndex);
                                if (newValue != contains)
                                {
                                    if (contains) list.Remove(equipmentDef.equipmentIndex);
                                    else list.Add(equipmentDef.equipmentIndex);
                                }
                            }
                        }
                    }
                );
            }
        }
    }
}