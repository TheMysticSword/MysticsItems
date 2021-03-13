using RoR2;
using R2API;
using R2API.Utils;
using MonoMod.RuntimeDetour;
using MysticsItems.Items;
using MysticsItems.Equipment;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using MonoMod.Cil;
using Mono.Cecil.Cil;

// ItemDropAPI adds WorldUnique and achievement-locked items to the available drops list
// This class fixes these issues by removing this mod's items from the drop pools right after ItemDropAPI adds them
namespace MysticsItems
{
    public static class ItemDropFixes
    {
        public static Dictionary<string, bool> preGameUnlockables = new Dictionary<string, bool>();

        public static void Init()
        {
            On.RoR2.PreGameController.RecalculateModifierAvailability += (orig, self) =>
            {
                orig(self);
                preGameUnlockables.Clear();
                foreach (KeyValuePair<string, UnlockableDef> keyValuePair in Unlockables.registeredUnlockables)
                {
                    preGameUnlockables.Add(keyValuePair.Value.name, typeof(PreGameController).InvokeMethod<bool>("AnyUserHasUnlockable", keyValuePair.Value));
                }
            };

            new Hook(typeof(ItemDropAPI).GetMethod("GetDefaultDropList", Main.bindingFlagAll, null, new System.Type[] { typeof(ItemTier) }, null), typeof(ItemDropFixes).GetMethod("GetDefaultDropList", Main.bindingFlagAll));
            new Hook(typeof(ItemDropAPI).GetMethod("GetDefaultLunarDropList", Main.bindingFlagAll, null, new System.Type[] { }, null), typeof(ItemDropFixes).GetMethod("GetDefaultLunarDropList", Main.bindingFlagAll));
            new Hook(typeof(ItemDropAPI).GetMethod("GetDefaultEquipmentDropList", Main.bindingFlagAll, null, new System.Type[] { }, null), typeof(ItemDropFixes).GetMethod("GetDefaultEquipmentDropList", Main.bindingFlagAll));
        }

        public static List<ItemIndex> GetDefaultDropList(System.Func<ItemTier, List<ItemIndex>> orig, ItemTier itemTier)
        {
            List<ItemIndex> result = orig(itemTier);
            foreach (BaseItem item in BaseItem.registeredItems.Values)
            {
                bool cannotDrop = item.itemDef.ContainsTag(ItemTag.WorldUnique);
                bool isLocked = item.itemDef.unlockableName != "" && preGameUnlockables.ContainsKey(item.itemDef.unlockableName) && !preGameUnlockables[item.itemDef.unlockableName];
                if (cannotDrop || isLocked)
                {
                    if (item.itemDef.tier == itemTier && result.Contains(item.itemIndex))
                    {
                        result.Remove(item.itemIndex);
                    }
                }
            }
            return result;
        }

        public static List<PickupIndex> GetDefaultLunarDropList(System.Func<List<PickupIndex>> orig)
        {
            List<PickupIndex> result = orig();
            foreach (BaseItem item in BaseItem.registeredItems.Values)
            {
                bool cannotDrop = item.itemDef.ContainsTag(ItemTag.WorldUnique);
                bool isLocked = item.itemDef.unlockableName != "" && preGameUnlockables.ContainsKey(item.itemDef.unlockableName) && !preGameUnlockables[item.itemDef.unlockableName];
                if (cannotDrop || isLocked)
                {
                    if (result.Contains(item.GetPickupIndex()))
                    {
                        result.Remove(item.GetPickupIndex());
                    }
                }
            }
            foreach (BaseEquipment equipment in BaseEquipment.registeredEquipment.Values)
            {
                bool cannotDrop = false;
                bool isLocked = equipment.equipmentDef.unlockableName != "" && preGameUnlockables.ContainsKey(equipment.equipmentDef.unlockableName) && !preGameUnlockables[equipment.equipmentDef.unlockableName];
                if (cannotDrop || isLocked)
                {
                    if (result.Contains(equipment.GetPickupIndex()))
                    {
                        result.Remove(equipment.GetPickupIndex());
                    }
                }
            }
            return result;
        }

        public static List<EquipmentIndex> GetDefaultEquipmentDropList(System.Func<List<EquipmentIndex>> orig)
        {
            List<EquipmentIndex> result = orig();
            foreach (BaseEquipment equipment in BaseEquipment.registeredEquipment.Values)
            {
                bool cannotDrop = false;
                bool isLocked = equipment.equipmentDef.unlockableName != "" && preGameUnlockables.ContainsKey(equipment.equipmentDef.unlockableName) && !preGameUnlockables[equipment.equipmentDef.unlockableName];
                if (cannotDrop || isLocked)
                {
                    if (result.Contains(equipment.equipmentIndex))
                    {
                        result.Remove(equipment.equipmentIndex);
                    }
                }
            }
            return result;
        }
    }
}
