using RoR2;
using System.Collections.Generic;
using System.Linq;
using HG;
using UnityEngine;

namespace MysticsItems
{
    public static class GenericCostTypes
    {
        public static void Init()
        {
            CostTypeDef costType_ItemFraction = new CostTypeDef();
            costType_ItemFraction.costStringFormatToken = "COST_" + Main.TokenPrefix.ToUpper() + "PERCENTAGEITEMS_FORMAT";
            costType_ItemFraction.isAffordable = delegate (CostTypeDef costTypeDef2, CostTypeDef.IsAffordableContext context)
            {
                CharacterBody body = context.activator.gameObject.GetComponent<CharacterBody>();
                if (body)
                {
                    Inventory inventory = body.inventory;
                    if (inventory)
                    {
                        int totalItemCount = 0;
                        ItemIndex itemIndex = 0;
                        ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;
                        while (itemIndex < itemCount)
                        {
                            int thisItemCount = inventory.GetItemCount(itemIndex);
                            if (thisItemCount > 0)
                            {
                                ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
                                if (itemDef.canRemove)
                                {
                                    totalItemCount += inventory.GetItemCount(itemIndex);
                                }
                            }
                            itemIndex++;

                            int itemsToTake = Mathf.FloorToInt(totalItemCount * Mathf.Clamp01(context.cost / 100f));
                            if (itemsToTake > 0) return true;
                        }
                    }
                }
                return false;
            };
            costType_ItemFraction.payCost = delegate (CostTypeDef costTypeDef2, CostTypeDef.PayCostContext context)
            {
                if (context.activatorBody)
                {
                    Inventory inventory = context.activatorBody.inventory;
                    if (inventory)
                    {
                        List<ItemIndex> itemsToTake = CollectionPool<ItemIndex, List<ItemIndex>>.RentCollection();
                        WeightedSelection<ItemIndex> weightedSelection = new WeightedSelection<ItemIndex>(8);
                        WeightedSelection<ItemIndex> weightedSelectionScrap = new WeightedSelection<ItemIndex>(8);
                        int totalItemCount = 0;
                        // Populate weighted selections with items from the inventory (weight is equal to the item count)
                        foreach (ItemIndex itemIndex in ItemCatalog.allItems)
                        {
                            if (itemIndex != context.avoidedItemIndex)
                            {
                                int itemCount = inventory.GetItemCount(itemIndex);
                                if (itemCount > 0)
                                {
                                    ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
                                    if (itemDef.canRemove)
                                    {
                                        (itemDef.ContainsTag(ItemTag.Scrap) ? weightedSelectionScrap : weightedSelection).AddChoice(itemIndex, (float)itemCount);
                                        totalItemCount += itemCount;
                                    }
                                }
                            }
                        }
                        int halfTotalItemCount = Mathf.FloorToInt(totalItemCount * Mathf.Clamp01(context.cost / 100f));
                        // Take choices from the weighted selections and put them into the final list, until we take enough items
                        TakeItemsFromWeightedSelection(weightedSelectionScrap, ref context, ref itemsToTake, halfTotalItemCount);
                        TakeItemsFromWeightedSelection(weightedSelection, ref context, ref itemsToTake, halfTotalItemCount);
                        // Unused shop terminal behaviour? If we didn't take enough items, and the purchasable is a shop terminal choice, add the choice's item index as remaining items to take
                        for (int i = itemsToTake.Count; i < halfTotalItemCount; i++)
                        {
                            itemsToTake.Add(context.avoidedItemIndex);
                        }
                        // Remove the items from the interactor's inventory and add them to the pay context results
                        for (int j = 0; j < itemsToTake.Count; j++)
                        {
                            ItemIndex itemIndex2 = itemsToTake[j];
                            context.results.itemsTaken.Add(itemIndex2);
                            inventory.RemoveItem(itemIndex2, 1);
                        }
                        CollectionPool<ItemIndex, List<ItemIndex>>.ReturnCollection(itemsToTake);
                    }
                }
            };
            costType_ItemFraction.colorIndex = ColorCatalog.ColorIndex.Tier1Item;
            CostTypeCreation.CreateCostType(new CostTypeCreation.CustomCostTypeInfo
            {
                costTypeDef = costType_ItemFraction,
                onRegister = (costTypeIndex) => { OnItemFractionCostTypeRegister(costTypeIndex); }
            });
            On.RoR2.Language.GetLocalizedFormattedStringByToken += (orig, self, token, args) =>
            {
                if (token == costType_ItemFraction.costStringFormatToken)
                {
                    return orig(self, "COST_ITEM_FORMAT", new object[] { args[0].ToString() + "%" });
                }
                return orig(self, token, args);
            };
        }

        public static System.Action<CostTypeIndex> OnItemFractionCostTypeRegister;

        private static void TakeItemsFromWeightedSelection(WeightedSelection<ItemIndex> weightedSelection, ref CostTypeDef.PayCostContext context, ref List<ItemIndex> itemsToTake, int halfTotalItemCount)
        {
            while (weightedSelection.Count > 0 && itemsToTake.Count < halfTotalItemCount)
            {
                int choiceIndex = weightedSelection.EvaluteToChoiceIndex(context.rng.nextNormalizedFloat);
                WeightedSelection<ItemIndex>.ChoiceInfo choice = weightedSelection.GetChoice(choiceIndex);
                ItemIndex value = choice.value;
                int num = (int)choice.weight;
                num--;
                if (num <= 0) weightedSelection.RemoveChoice(choiceIndex);
                else weightedSelection.ModifyChoiceWeight(choiceIndex, (float)num);
                itemsToTake.Add(value);
            }
        }
    }
}
