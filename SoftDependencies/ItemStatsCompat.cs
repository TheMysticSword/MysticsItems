using BepInEx.Configuration;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace MysticsItems.SoftDependencies
{
    internal static class ItemStatsCompat
    {
        internal static void Init()
        {
            RoR2Application.onLoad += RegisterItemStatDefs;
        }

        private static void RegisterItemStatDefs()
        {
            // Lunar
            // Moonglasses
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_Moonglasses.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.Moonglasses.critDamageIncrease.Value + Items.Moonglasses.critDamageIncreasePerStack.Value * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_CRITDAMAGE", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, scale: 1f, signed: true))
                    ),
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => -(1f - 1f / Mathf.Pow(2f, itemCount)),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_CRIT", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, signed: true, color: "\"red\""))
                    )
                }
            });
            // Puzzle of Chronos
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_RegenAndDifficultySpeed.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.RegenAndDifficultySpeed.baseRegenIncrease + Items.RegenAndDifficultySpeed.baseRegenIncreasePerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_REGEN", ItemStats.ValueFormatters.Extensions.FormatInt(value: value, postfix: Language.GetString("ITEMSTATS_MYSTICSITEMS_POSTFIX_HEALTHPERSECOND"), signed: true))
                    ),
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.RegenAndDifficultySpeed.timerSpeedIncrease + Items.RegenAndDifficultySpeed.timerSpeedIncreasePerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_DIFFICULTYSPEED", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, scale: 1f, signed: true, color: "\"red\""))
                    )
                }
            });
            // Rift Lens
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_RiftLens.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.RiftLens.baseRifts + Items.RiftLens.riftsPerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_UNSTABLERIFTS", ItemStats.ValueFormatters.Extensions.FormatInt(value: value))
                    )
                }
            });

            // Tier1
            // Spine Implant
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_BackArmor.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.BackArmor.armorAdd + Items.BackArmor.armorAddPerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_ARMOR", ItemStats.ValueFormatters.Extensions.FormatInt(value: value, signed: true))
                    )
                }
            });
            // Donut
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_HealOrbOnBarrel.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.HealOrbOnBarrel.fractionalHealing + Items.HealOrbOnBarrel.fractionalHealingPerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_HEALING", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, scale: 1f))
                    )
                }
            });
            // Manuscript
            {
                var stats = new List<ItemStats.Stat.ItemStat>();
                foreach (var buffType in Items.Manuscript.MysticsItemsManuscript.buffTypes)
                {
                    stats.Add(new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) =>
                        {
                            if (ctx.Inventory)
                            {
                                var component = ctx.Inventory.GetComponent<Items.Manuscript.MysticsItemsManuscript>();
                                if (component) return component.buffStacks[buffType] * Items.Manuscript.statBonus;
                            }
                            return 0;
                        },
                        (value, ctx) => Language.GetStringFormatted(
                            "ITEMSTATS_MYSTICSITEMS_" + buffType.ToString().ToUpperInvariant(),
                            ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, signed: true, scale: 1f)
                        )
                    ));
                }
                ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_Manuscript.itemIndex, new ItemStats.ItemStatDef
                {
                    Stats = stats
                });
            }
            // Scratch Ticket
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_ScratchTicket.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.ScratchTicket.chanceBonus + Items.ScratchTicket.chanceBonusPerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_CHANCE", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, signed: true, scale: 1f))
                    )
                }
            });
            // Cutesy Bow
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_LimitedArmor.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => ctx.Master && ctx.Master.GetComponent<Items.LimitedArmor.MysticsItemsLimitedArmorBehavior>() ? ctx.Master.GetComponent<Items.LimitedArmor.MysticsItemsLimitedArmorBehavior>().remainingHits : 0,
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_REMAINING", ItemStats.ValueFormatters.Extensions.FormatInt(value: value))
                    )
                }
            });
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_LimitedArmorBroken.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.LimitedArmorBroken.brokenArmor * itemCount,
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_ARMOR", ItemStats.ValueFormatters.Extensions.FormatInt(value: value, signed: true))
                    )
                }
            });
            // Choc Chip
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_Cookie.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.Cookie.buffDuration + Items.Cookie.buffDurationPerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_DURATION", ItemStats.ValueFormatters.Extensions.FormatInt(value: value, postfix: Language.GetString("ITEMSTATS_MYSTICSITEMS_POSTFIX_SECONDS"), signed: true))
                    ),
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.Cookie.debuffDuration + Items.Cookie.debuffDurationPerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_DURATION", ItemStats.ValueFormatters.Extensions.FormatInt(value: value, postfix: Language.GetString("ITEMSTATS_MYSTICSITEMS_POSTFIX_SECONDS"), signed: true))
                    )
                }
            });
            // Marwan's Ash
            {
                var damageStat = new ItemStats.Stat.ItemStat(
                    (itemCount, ctx) => Items.MarwanAsh1.damage + Items.MarwanAsh1.damagePerLevel * (ctx.Master && ctx.Master.hasBody ? ctx.Master.GetBody().level : 0f) * itemCount,
                    (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_DAMAGE", ItemStats.ValueFormatters.Extensions.FormatInt(value: value))
                );
                var dotStat = new ItemStats.Stat.ItemStat(
                    (itemCount, ctx) => Items.MarwanAsh1.dotPercent + Items.MarwanAsh1.dotPercentPerLevel * (ctx.Master && ctx.Master.hasBody ? ctx.Master.GetBody().level - Items.MarwanAsh1.upgradeLevel12 : 0f) * itemCount,
                    (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_MAXHEALTH", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, scale: 1f))
                );
                var radiusStat = new ItemStats.Stat.ItemStat(
                    (itemCount, ctx) => Items.MarwanAsh1.radius + Items.MarwanAsh1.radiusPerLevel * (ctx.Master && ctx.Master.hasBody ? ctx.Master.GetBody().level - Items.MarwanAsh1.upgradeLevel23 : 0f) * itemCount,
                    (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_RADIUS", ItemStats.ValueFormatters.Extensions.FormatInt(value: value, postfix: Language.GetString("ITEMSTATS_MYSTICSITEMS_POSTFIX_METERS")))
                );
                ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_MarwanAsh1.itemIndex, new ItemStats.ItemStatDef
                {
                    Stats = new List<ItemStats.Stat.ItemStat>()
                    {
                        damageStat
                    }
                });
                ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_MarwanAsh2.itemIndex, new ItemStats.ItemStatDef
                {
                    Stats = new List<ItemStats.Stat.ItemStat>()
                    {
                        damageStat,
                        dotStat
                    }
                });
                ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_MarwanAsh3.itemIndex, new ItemStats.ItemStatDef
                {
                    Stats = new List<ItemStats.Stat.ItemStat>()
                    {
                        damageStat,
                        dotStat,
                        radiusStat
                    }
                });
            }

            // Tier2
            // Vendetta
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_AllyDeathRevenge.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.AllyDeathRevenge.duration + Items.AllyDeathRevenge.durationPerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_DURATION", ItemStats.ValueFormatters.Extensions.FormatInt(value: value, postfix: Language.GetString("ITEMSTATS_MYSTICSITEMS_POSTFIX_SECONDS")))
                    )
                }
            });
            // Cup of Expresso
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_CoffeeBoostOnItemPickup.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.CoffeeBoostOnItemPickup.maxBuffs + Items.CoffeeBoostOnItemPickup.maxBuffsPerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_MAXBUFFSTACKS", ItemStats.ValueFormatters.Extensions.FormatInt(value: value))
                    ),
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Buffs.CoffeeBoost.boostPower * (Items.CoffeeBoostOnItemPickup.maxBuffs + Items.CoffeeBoostOnItemPickup.maxBuffsPerStack * (itemCount - 1)),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_MAXATTACKSPEEDBONUS", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, scale: 1f))
                    ),
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Buffs.CoffeeBoost.boostPower * (Items.CoffeeBoostOnItemPickup.maxBuffs + Items.CoffeeBoostOnItemPickup.maxBuffsPerStack * (itemCount - 1)),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_MAXMOVESPEEDBONUS", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, scale: 1f))
                    )
                }
            });
            // Contraband Gunpowder
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_ExplosivePickups.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.ExplosivePickups.damage + Items.ExplosivePickups.damagePerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_DAMAGE", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, scale: 1f))
                    ),
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.ExplosivePickups.radius + Items.ExplosivePickups.radiusPerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_RADIUS", ItemStats.ValueFormatters.Extensions.FormatInt(value: value, postfix: Language.GetString("ITEMSTATS_MYSTICSITEMS_POSTFIX_METERS")))
                    )
                }
            });
            // Black Monolith
            
            // Platinum Card

            // Mystic Sword
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_MysticSword.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => ctx.Master && ctx.Master.hasBody && ctx.Master.GetBody().GetComponent<Items.MysticSword.MysticsItemsMysticSwordBehaviour>() ? ctx.Master.GetBody().GetComponent<Items.MysticSword.MysticsItemsMysticSwordBehaviour>().damageBonus : 0f,
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_DAMAGE", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, signed: true))
                    ),
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.MysticSword.damage + Items.MysticSword.damagePerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_BONUSPERKILL", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, scale: 1f, signed: true))
                    )
                }
            });
            // Metronome
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_Rhythm.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Buffs.RhythmCombo.comboCrit + Buffs.RhythmCombo.comboCritPerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_CRITPERBUFFSTACK", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, scale: 1f, signed: true))
                    )
                }
            });
            // Nuclear Accelerator
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_SpeedGivesDamage.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.SpeedGivesDamage.damage + Items.SpeedGivesDamage.damagePerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_DAMAGEPERSPEEDINCREASE", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, scale: 1f, signed: true))
                    ),
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => ctx.Master && ctx.Master.hasBody ? Items.SpeedGivesDamage.CalculateDamageBonus(ctx.Master.GetBody(), (int)itemCount) : 0f,
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_DAMAGE", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, signed: true))
                    )
                }
            });
            // Faulty Spotter

            // Failed Experiment
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_ElitePotion.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.ElitePotion.radius + Items.ElitePotion.radiusPerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_RADIUS", ItemStats.ValueFormatters.Extensions.FormatInt(value: value, postfix: Language.GetString("ITEMSTATS_MYSTICSITEMS_POSTFIX_METERS")))
                    ),
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.ElitePotion.duration + Items.ElitePotion.durationPerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_DURATION", ItemStats.ValueFormatters.Extensions.FormatInt(value: value, postfix: Language.GetString("ITEMSTATS_MYSTICSITEMS_POSTFIX_SECONDS")))
                    )
                }
            });
            // Spare Wiring
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_DroneWires.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.DroneWires.damage + Items.DroneWires.damagePerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_DAMAGE", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value))
                    )
                }
            });
            // Ceremony of Perdition
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_DeathCeremony.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Util.ConvertAmplificationPercentageIntoReductionPercentage(Items.DeathCeremony.damage + Items.DeathCeremony.damagePerStack * (itemCount - 1)),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_DAMAGE", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value))
                    )
                }
            });
            // Devil's Cry
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_JudgementCut.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.JudgementCut.damagePerSlash * (Items.JudgementCut.slashCount + Items.JudgementCut.slashCountPerStack * (itemCount - 1)),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_DAMAGE", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, scale: 1f))
                    ),
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.JudgementCut.radius + Items.JudgementCut.radiusPerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_RADIUS", ItemStats.ValueFormatters.Extensions.FormatInt(value: value, postfix: Language.GetString("ITEMSTATS_MYSTICSITEMS_POSTFIX_METERS")))
                    )
                }
            });

            // Tier3
            // Crystallized World
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_CrystalWorld.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.CrystalWorld.pulses + Items.CrystalWorld.pulsesPerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_PULSES", ItemStats.ValueFormatters.Extensions.FormatInt(value: value))
                    )
                }
            });
            // Timely Execution
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_DasherDisc.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.DasherDisc.CalculateCooldown((int)itemCount),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_COOLDOWN", ItemStats.ValueFormatters.Extensions.FormatInt(value: value, postfix: Language.GetString("ITEMSTATS_MYSTICSITEMS_POSTFIX_SECONDS")))
                    )
                }
            });
            // Super Idol
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_Idol.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => (int)(400f / itemCount * Stage.instance.entryDifficultyCoefficient),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_GOLDFORMAXBUFF", ItemStats.ValueFormatters.Extensions.FormatInt(value: value))
                    )
                }
            });
            // Thought Processor
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_ThoughtProcessor.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.ThoughtProcessor.attackSpeed + Items.ThoughtProcessor.attackSpeedPerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_ATTACKSPEED", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, scale: 1f, signed: true))
                    ),
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => (Items.ThoughtProcessor.attackSpeed + Items.ThoughtProcessor.attackSpeedPerStack * (itemCount - 1)) * 100f,
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_UPTO", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, scale: 1f, signed: true))
                    )
                }
            });
            // Treasure Map
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_TreasureMap.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.TreasureMap.CalculateChargeTime((int)itemCount),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_TIME", ItemStats.ValueFormatters.Extensions.FormatInt(value: value, postfix: Language.GetString("ITEMSTATS_MYSTICSITEMS_POSTFIX_SECONDS")))
                    )
                }
            });
            // Wireless Voltmeter
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_Voltmeter.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.Voltmeter.damage + Items.Voltmeter.damagePerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_DAMAGE", ItemStats.ValueFormatters.Extensions.FormatPercentage(value: value, scale: 1f))
                    )
                }
            });
            // Hiker's Backpack
            ItemStats.ItemStatsMod.AddCustomItemStatDef(MysticsItemsContent.Items.MysticsItems_Backpack.itemIndex, new ItemStats.ItemStatDef
            {
                Stats = new List<ItemStats.Stat.ItemStat>()
                {
                    new ItemStats.Stat.ItemStat(
                        (itemCount, ctx) => Items.Backpack.charges + Items.Backpack.chargesPerStack * (itemCount - 1),
                        (value, ctx) => Language.GetStringFormatted("ITEMSTATS_MYSTICSITEMS_CHARGES", ItemStats.ValueFormatters.Extensions.FormatInt(value: value, signed: true))
                    )
                }
            });
        }
    }
}
