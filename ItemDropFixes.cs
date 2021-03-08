using RoR2;
using R2API;
using R2API.Utils;
using MonoMod.RuntimeDetour;
using MysticsItems.Items;
using MysticsItems.Equipment;
using System.Collections.Generic;
using System.Linq;

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

            new Hook(typeof(ItemDropAPI).GetMethod("RunOnBuildDropTable", Main.bindingFlagAll), typeof(ItemDropFixes).GetMethod("Run_BuildDropTable", Main.bindingFlagAll));
        }

        public static void Run_BuildDropTable(On.RoR2.Run.orig_BuildDropTable orig, Run self)
        {
            orig(self);
            foreach (BaseItem item in BaseItem.registeredItems.Values)
            {
                List<PickupIndex> list = null;
                switch (item.itemDef.tier)
                {
                    case ItemTier.Tier1:
                        list = self.availableTier1DropList;
                        break;
                    case ItemTier.Tier2:
                        list = self.availableTier2DropList;
                        break;
                    case ItemTier.Tier3:
                        list = self.availableTier3DropList;
                        break;
                    case ItemTier.Lunar:
                        list = self.availableLunarDropList;
                        break;
                    case ItemTier.Boss:
                        list = self.availableBossDropList;
                        break;
                }
                bool cannotDrop = item.itemDef.ContainsTag(ItemTag.WorldUnique);
                bool isLocked = item.itemDef.unlockableName != "" && preGameUnlockables.ContainsKey(item.itemDef.unlockableName) && !preGameUnlockables[item.itemDef.unlockableName];
                if (cannotDrop || isLocked)
                {
                    if (list != null)
                    {
                        PickupIndex pickupIndex = item.GetPickupIndex();
                        while (list.Contains(pickupIndex)) list.Remove(pickupIndex);
                    }
                    while (self.availableItems.Contains(item.itemIndex)) self.availableItems.Remove(item.itemIndex);
                }
            }

            foreach (BaseEquipment equipment in BaseEquipment.registeredEquipment.Values)
            {
                bool cannotDrop = true;
                bool isLocked = equipment.equipmentDef.unlockableName != "" && preGameUnlockables.ContainsKey(equipment.equipmentDef.unlockableName) && !preGameUnlockables[equipment.equipmentDef.unlockableName];
                if (cannotDrop || isLocked)
                {
                    PickupIndex pickupIndex = equipment.GetPickupIndex();
                    if (!equipment.equipmentDef.isLunar)
                    {
                        while (self.availableEquipmentDropList.Contains(pickupIndex)) self.availableEquipmentDropList.Remove(pickupIndex);
                        while (self.availableNormalEquipmentDropList.Contains(pickupIndex)) self.availableNormalEquipmentDropList.Remove(pickupIndex);
                    }
                    else
                    {
                        while (self.availableLunarDropList.Contains(pickupIndex)) self.availableLunarDropList.Remove(pickupIndex);
                        while (self.availableLunarEquipmentDropList.Contains(pickupIndex)) self.availableLunarEquipmentDropList.Remove(pickupIndex);
                    }
                    while (self.availableEquipment.Contains(equipment.equipmentIndex)) self.availableEquipment.Remove(equipment.equipmentIndex);
                }
            }
        }
    }
}
