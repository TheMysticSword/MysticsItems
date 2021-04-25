using RoR2;
using System.Collections.Generic;
using MysticsItems.Items;
using ItemStats;
using ItemStats.Stat;
using ItemStats.ValueFormatters;
using ItemStats.StatModification;
using MonoMod.RuntimeDetour;
using System;
using UnityEngine;

namespace MysticsItems.SoftDependencies
{
    public static class ItemStatsSoftDependency
    {
        public const string PluginGUID = "dev.ontrigger.itemstats";

        public static void AddItemStatDef(ItemDef itemDef, ItemStatDef itemStatDef, List<Type> statModifiers = null)
        {
            if (itemDef != null)
            {
                ItemStatsMod.AddCustomItemStatDef(itemDef.itemIndex, itemStatDef);
            }
        }

        public static void Init()
        {
            // tier1
            AddItemStatDef(MysticsItemsContent.Items.HealOrbOnBarrel, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 0.1f + (0.1f * itemCount - 1),
                        (value, ctx) => $"Heal Amount: {value.FormatPercentage()}"
                    )
                }
            }, new List<Type> { typeof(HealingIncreaseModifier) });
            AddItemStatDef(MysticsItemsContent.Items.ScratchTicket, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 0.01f + 0.01f * (itemCount - 1),
                        (value, ctx) => $"Chance Increase: {value.FormatPercentage()}"
                    )
                }
            });
            /*
            AddItemStatDef(MysticsItemsContent.Items.CommandoScope, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 25f + CommandoScope.CalculateExtraDistance((int)itemCount),
                        (value, ctx) => $"Max Damage Distance: {value.FormatInt("m")}"
                    ),
                    new ItemStat(
                        (itemCount, ctx) => 60f + CommandoScope.CalculateExtraDistance((int)itemCount) * 2f,
                        (value, ctx) => $"Min Damage Distance: {value.FormatInt("m")}"
                    )
                }
            });
            */
            AddItemStatDef(MysticsItemsContent.Items.BackArmor, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 10f + 10f * (itemCount - 1),
                        (value, ctx) => $"Bonus Armor: {value.FormatInt()}"
                    )
                }
            });
            /*
            AddItemStatDef(MysticsItemsContent.Items.ArtificerNanobots, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => (1f - 1f / (1f + 0.2f * (itemCount - 1))) * (1f - 0.2f),
                        (value, ctx) => $"Damage: {value.FormatPercentage()}"
                    )
                }
            });
            */

            // tier2
            AddItemStatDef(MysticsItemsContent.Items.CoffeeBoostOnItemPickup, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 0.21f + 0.07f * (itemCount - 1),
                        (value, ctx) => $"Max Attack Speed: {value.FormatPercentage()}"
                    ),
                    new ItemStat(
                        (itemCount, ctx) => 0.21f + 0.07f * (itemCount - 1),
                        (value, ctx) => $"Max Movement Speed: {value.FormatPercentage()}"
                    )
                }
            });
            AddItemStatDef(MysticsItemsContent.Items.ExplosivePickups, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 2.5f + 2f * (itemCount - 1),
                        (value, ctx) => $"Damage: {value.FormatPercentage()}"
                    ),
                    new ItemStat(
                        (itemCount, ctx) => 8f + 1.6f * (itemCount - 1),
                        (value, ctx) => $"Radius: {value.FormatInt("m")}"
                    )
                }
            });
            AddItemStatDef(MysticsItemsContent.Items.AllyDeathRevenge, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 15f + 5f * (itemCount - 1),
                        (value, ctx) => $"Duration: {value.FormatInt("s")}"
                    ),
                    new ItemStat(
                        (itemCount, ctx) => 2f + 0.5f * (itemCount - 1),
                        (value, ctx) => $"Duration (Same-Stage Death): {value.FormatInt("s")}"
                    )
                }
            });
            AddItemStatDef(MysticsItemsContent.Items.Spotter, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => itemCount,
                        (value, ctx) => $"Max Marked Enemies: {value.FormatInt()}"
                    )
                }
            });
            AddItemStatDef(MysticsItemsContent.Items.SpeedGivesDamage, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 0.01f + 0.005f * (itemCount - 1),
                        (value, ctx) => $"Damage Per 2.5% Speed: {value.FormatPercentage()}"
                    ),
                    new ItemStat(
                        (itemCount, ctx) => ctx.Master ? (ctx.Master.GetBody().moveSpeed / (ctx.Master.GetBody().baseMoveSpeed + ctx.Master.GetBody().levelMoveSpeed * ctx.Master.GetBody().level) - 1f) / 0.025f * (0.01f + 0.005f * (itemCount - 1)) : 0f,
                        (value, ctx) => $"Current Damage Boost: {value.FormatPercentage()}"
                    )
                }
            });
            AddItemStatDef(MysticsItemsContent.Items.ExtraShrineUse, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => itemCount,
                        (value, ctx) => $"Extra Shrine Uses: {value.FormatInt()}"
                    )
                }
            });
            /*
            AddItemStatDef(MysticsItemsContent.Items.CommandoRevolverDrum, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 0.1f,
                        (value, ctx) => $"Proc Chance: {value.FormatPercentage()}"
                    ),
                    new ItemStat(
                        (itemCount, ctx) => (6f + 6f * (itemCount - 1)) * 0.5f,
                        (value, ctx) => $"Damage: {value.FormatPercentage()}"
                    )
                }
            }, new List<Type> { typeof(LuckModifier), null });
            */
            /*
            AddItemStatDef(MysticsItemsContent.Items.KeepShopTerminalOpen, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => itemCount,
                        (value, ctx) => $"Extra Purchases: {value.FormatInt()}"
                    ),
                    new ItemStat(
                        (itemCount, ctx) => itemCount + (ctx.Inventory ? ctx.Inventory.GetItemCount(MysticsItemsContent.Items.KeepShopTerminalOpenConsumed) : 0),
                        (value, ctx) => $"Extra Purchases This Stage: {value.FormatInt()}"
                    )
                }
            });
            */

            // tier3
            AddItemStatDef(MysticsItemsContent.Items.Voltmeter, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 8f + 8f * (itemCount - 1),
                        (value, ctx) => $"Reflected Damage: {value.FormatPercentage()}"
                    )
                }
            });
            AddItemStatDef(MysticsItemsContent.Items.ThoughtProcessor, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => -0.1f - 0.05f * (itemCount - 1),
                        (value, ctx) => $"Cooldown Per Skill Use: {value.FormatPercentage()}"
                    )
                }
            });
            AddItemStatDef(MysticsItemsContent.Items.CrystalWorld, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 2 + (itemCount - 1),
                        (value, ctx) => $"Max Occurrences: {value.FormatInt()}"
                    )
                }
            });
            AddItemStatDef(MysticsItemsContent.Items.DasherDisc, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 60f / (1f + 0.2f * (itemCount - 1)),
                        (value, ctx) => $"Cooldown: {value.FormatInt("s")}"
                    )
                }
            });
            AddItemStatDef(MysticsItemsContent.Items.TreasureMap, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 120f / (1f + 0.5f * (itemCount - 1)),
                        (value, ctx) => $"Excavation Time: {value.FormatInt("s")}"
                    )
                }
            });

            // lunar
            AddItemStatDef(MysticsItemsContent.Items.RiftLens, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => itemCount,
                        (value, ctx) => $"Rift Chests: {value.FormatInt()}"
                    )
                }
            });
            /*
            AddItemStatDef(MysticsItemsContent.Items.Moonglasses, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => itemCount,
                        (value, ctx) => $"Extra Crit Damage: {value.FormatPercentage()}"
                    ),
                    new ItemStat(
                        (itemCount, ctx) => 1f / (itemCount * 2f),
                        (value, ctx) => $"Crit Chance Multiplier: {value.FormatPercentage()}"
                    )
                }
            });
            */
        }
    }
}
