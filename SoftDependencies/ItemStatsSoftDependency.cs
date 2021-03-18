using RoR2;
using System.Collections.Generic;
using MysticsItems.Items;
using ItemStats;
using ItemStats.Stat;
using ItemStats.ValueFormatters;
using ItemStats.StatModification;
using MonoMod.RuntimeDetour;
using System;

namespace MysticsItems.SoftDependencies
{
    public static class ItemStatsSoftDependency
    {
        public const string PluginGUID = "dev.ontrigger.itemstats";

        public static void AddItemStatDef(Type itemType, ItemStatDef itemStatDef, List<Type> statModifiers = null)
        {
            BaseItem item = BaseItem.GetFromType(itemType);
            if (item != null)
            {
                ItemStatsMod.AddCustomItemStatDef(item.itemIndex, itemStatDef);
            }
        }

        public static void Init()
        {
            // tier1
            AddItemStatDef(typeof(HealOrbOnBarrel), new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 0.1f + (0.1f * itemCount - 1),
                        (value, ctx) => $"Heal Amount: {value.FormatPercentage()}"
                    )
                }
            }, new List<Type> { typeof(HealingIncreaseModifier) });
            AddItemStatDef(typeof(ScratchTicket), new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => (30f + 20f * (itemCount - 1)) * Run.instance.difficultyCoefficient,
                        (value, ctx) => $"Gold Reward: {value.FormatInt("$")}"
                    ),
                    new ItemStat(
                        (itemCount, ctx) => (30f + 20f * (itemCount - 1) - 40f) * Run.instance.difficultyCoefficient,
                        (value, ctx) => $"Profit On Perfect Shrine: {value.FormatInt("$")}"
                    ),
                    new ItemStat(
                        (itemCount, ctx) => ((30f + 20f * (itemCount - 1) - 40f) * Run.instance.difficultyCoefficient) * 0.547085202f,
                        (value, ctx) => $"Average Profit: {value.FormatInt("$")}"
                    )
                }
            });
            AddItemStatDef(typeof(CommandoScope), new ItemStatDef
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
            AddItemStatDef(typeof(BackArmor), new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 10f + 10f * (itemCount - 1),
                        (value, ctx) => $"Bonus Armor: {value.FormatInt()}"
                    )
                }
            });
            AddItemStatDef(typeof(ArtificerNanobots), new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => (1f - 1f / (1f + 0.2f * (itemCount - 1))) * (1f - 0.2f),
                        (value, ctx) => $"Damage: {value.FormatPercentage()}"
                    )
                }
            });

            // tier2
            AddItemStatDef(typeof(CoffeeBoostOnItemPickup), new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 0.07f + 0.07f * (itemCount - 1),
                        (value, ctx) => $"Max Attack Speed: {value.FormatPercentage()}"
                    ),
                    new ItemStat(
                        (itemCount, ctx) => 0.07f + 0.07f * (itemCount - 1),
                        (value, ctx) => $"Max Movement Speed: {value.FormatPercentage()}"
                    )
                }
            });
            AddItemStatDef(typeof(ExplosivePickups), new ItemStatDef
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
            AddItemStatDef(typeof(AllyDeathRevenge), new ItemStatDef
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
            AddItemStatDef(typeof(Spotter), new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => itemCount,
                        (value, ctx) => $"Max Marked Enemies: {value.FormatInt()}"
                    )
                }
            });
            AddItemStatDef(typeof(SpeedGivesDamage), new ItemStatDef
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
            AddItemStatDef(typeof(ExtraShrineUse), new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => itemCount,
                        (value, ctx) => $"Extra Shrine Uses: {value.FormatInt()}"
                    )
                }
            });
            AddItemStatDef(typeof(CommandoRevolverDrum), new ItemStatDef
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

            // tier3
            AddItemStatDef(typeof(Voltmeter), new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 8f + 8f * (itemCount - 1),
                        (value, ctx) => $"Reflected Damage: {value.FormatPercentage()}"
                    )
                }
            });
            AddItemStatDef(typeof(ThoughtProcessor), new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => -0.1f - 0.05f * (itemCount - 1),
                        (value, ctx) => $"Cooldown Per Skill Use: {value.FormatPercentage()}"
                    )
                }
            });
            AddItemStatDef(typeof(CrystalWorld), new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 2 + (itemCount - 1),
                        (value, ctx) => $"Max Occurrences: {value.FormatInt()}"
                    )
                }
            });
            AddItemStatDef(typeof(DasherDisc), new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => 60f / (1f + 0.2f * (itemCount - 1)),
                        (value, ctx) => $"Cooldown: {value.FormatInt("s")}"
                    )
                }
            });
            AddItemStatDef(typeof(TreasureMap), new ItemStatDef
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
            AddItemStatDef(typeof(RiftLens), new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat(
                        (itemCount, ctx) => itemCount,
                        (value, ctx) => $"Rift Chests: {value.FormatInt()}"
                    )
                }
            });
        }
    }
}
