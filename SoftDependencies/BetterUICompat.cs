using RoR2;
using BepInEx.Configuration;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace MysticsItems.SoftDependencies
{
    internal static class BetterUICompat
    {
        internal static void Init()
        {
            RoR2Application.onLoad += RegisterBuffInfos;
            RoR2Application.onLoad += BetterUIItemStats.RegisterItemStats;

            Func<CharacterBody, string> func = (statBody) =>
            {
                var crit = statBody.crit;
                if (crit >= 1f && statBody.inventory)
                {
                    var itemCount = statBody.inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_ScratchTicket);
                    if (itemCount > 0) crit = Items.ScratchTicket.ApplyPercentBonus(itemCount, crit);
                }
                float LuckCalc(float chance, float luck)
                {
                    if (luck == 0) return chance;
                    else if (luck < 0) return (float)((int)chance + Math.Pow(chance % 1, Math.Abs(luck) + 1));
                    else return (float)((int)chance + (1 - Math.Pow(1 - (chance % 1), Math.Abs(luck) + 1)));
                }
                crit = 100 * ((int)crit / 100) + 100 * LuckCalc(crit % 100 * 0.01f, statBody.master.luck);
                return crit.ToString("0.##");
            };
            BetterUI.StatsDisplay.AddStatsDisplay("$mysticsitemscrit", func);
        }

        private static void RegisterBuffInfos()
        {
            foreach (var buffDef in typeof(MysticsItemsContent.Buffs).GetFields().Select(x => x.GetValue(null) as BuffDef))
            {
                if (buffDef != null)
                {
                    var tokenStart = "BUFF_" + buffDef.name.ToUpperInvariant() + "_";
                    BetterUI.Buffs.RegisterBuffInfo(buffDef, tokenStart + "NAME", tokenStart + "DESC");
                }
            }
        }

        public static class BetterUIItemStats
        {
            public static void RegisterItemStats()
            {
                // Lunar
                // Moonglasses
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_Moonglasses,
                    "ITEMSTATS_MYSTICSITEMS_CRITDAMAGE",
                    Items.Moonglasses.critDamageIncrease.Value / 100f,
                    Items.Moonglasses.critDamageIncreasePerStack.Value / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_Moonglasses,
                    "ITEMSTATS_MYSTICSITEMS_CRIT",
                    1f,
                    2f,
                    stackingFormula: DivideOnEachStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                // Puzzle of Chronos
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_RegenAndDifficultySpeed,
                    "ITEMSTATS_MYSTICSITEMS_REGEN",
                    Items.RegenAndDifficultySpeed.baseRegenIncrease,
                    Items.RegenAndDifficultySpeed.baseRegenIncreasePerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Regen,
                    itemTag: BetterUI.ItemStats.ItemTag.Healing
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_RegenAndDifficultySpeed,
                    "ITEMSTATS_MYSTICSITEMS_DIFFICULTYSPEED",
                    Items.RegenAndDifficultySpeed.timerSpeedIncrease / 100f,
                    Items.RegenAndDifficultySpeed.timerSpeedIncreasePerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                // Rift Lens
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_RiftLens,
                    "ITEMSTATS_MYSTICSITEMS_UNSTABLERIFTS",
                    Items.RiftLens.baseRifts,
                    Items.RiftLens.riftsPerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Charges
                );

                // Tier 1
                // Spine Implant
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_BackArmor,
                    "ITEMSTATS_MYSTICSITEMS_ARMOR",
                    Items.BackArmor.armorAdd,
                    Items.BackArmor.armorAddPerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Armor
                );
                // Donut
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_HealOrbOnBarrel,
                    "ITEMSTATS_MYSTICSITEMS_HEALING",
                    Items.HealOrbOnBarrel.fractionalHealing / 100f,
                    Items.HealOrbOnBarrel.fractionalHealingPerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent,
                    itemTag: BetterUI.ItemStats.ItemTag.Healing
                );
                // Manuscript
                foreach (var buffType in Items.Manuscript.MysticsItemsManuscript.buffTypes)
                {
                    BetterUI.ItemStats.RegisterStat(
                        MysticsItemsContent.Items.MysticsItems_Manuscript,
                        "ITEMSTATS_MYSTICSITEMS_" + buffType.ToString().ToUpperInvariant(),
                        0f,
                        0f,
                        statFormatter: new BetterUI.ItemStats.StatFormatter()
                        {
                            suffix = "%",
                            style = BetterUI.ItemStats.Styles.Damage,
                            statFormatter = (sb, value, master) =>
                            {
                                if (master.inventory)
                                {
                                    var component = master.inventory.GetComponent<Items.Manuscript.MysticsItemsManuscript>();
                                    if (component) sb.AppendFormat("{0:0.##}", component.buffStacks[buffType] * Items.Manuscript.statBonus);
                                    else sb.Append("0");
                                }
                                else sb.Append("0");
                            }
                        },
                        itemTag: BetterUI.ItemStats.ItemTag.Healing
                    );
                }
                // Scratch Ticket
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_ScratchTicket,
                    "ITEMSTATS_MYSTICSITEMS_CHANCE",
                    Items.ScratchTicket.chanceBonus / 100f,
                    Items.ScratchTicket.chanceBonusPerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                BetterUI.ItemStats.RegisterModifier(
                    BetterUI.ItemStats.ItemTag.Luck,
                    MysticsItemsContent.Items.MysticsItems_ScratchTicket,
                    Items.ScratchTicket.alternateBonus ? BetterUI.ItemStats.ItemModifier.PercentBonus : FlatBonusProper,
                    Items.ScratchTicket.chanceBonus / (Items.ScratchTicket.alternateBonus ? 1f : 100f),
                    stackModifier: Items.ScratchTicket.chanceBonusPerStack / (Items.ScratchTicket.alternateBonus ? 1f : 100f)
                );
                // Cutesy Bow
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_LimitedArmor,
                    "ITEMSTATS_MYSTICSITEMS_REMAINING",
                    0f,
                    0f,
                    statFormatter: LimitedArmorRemainingHits
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_LimitedArmorBroken,
                    "ITEMSTATS_MYSTICSITEMS_ARMOR",
                    Items.LimitedArmorBroken.brokenArmor,
                    Items.LimitedArmorBroken.brokenArmor,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Armor
                );
                // Choc Chip
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_Cookie,
                    "ITEMSTATS_MYSTICSITEMS_DURATION",
                    Items.Cookie.buffDuration,
                    Items.Cookie.buffDurationPerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Seconds
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_Cookie,
                    "ITEMSTATS_MYSTICSITEMS_DURATION",
                    -Items.Cookie.debuffDuration,
                    -Items.Cookie.debuffDurationPerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Seconds
                );
                // Marwan's Ash
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_MarwanAsh1,
                    "ITEMSTATS_MYSTICSITEMS_DAMAGE",
                    1f,
                    1f,
                    statFormatter: MarwanAshDamage
                );

                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_MarwanAsh2,
                    "ITEMSTATS_MYSTICSITEMS_DAMAGE",
                    1f,
                    1f,
                    statFormatter: MarwanAshDamage
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_MarwanAsh2,
                    "ITEMSTATS_MYSTICSITEMS_MAXHEALTH",
                    1f,
                    1f,
                    statFormatter: MarwanAshBurnDamage
                );

                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_MarwanAsh3,
                    "ITEMSTATS_MYSTICSITEMS_DAMAGE",
                    1f,
                    1f,
                    statFormatter: MarwanAshDamage
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_MarwanAsh3,
                    "ITEMSTATS_MYSTICSITEMS_MAXHEALTH",
                    1f,
                    1f,
                    statFormatter: MarwanAshBurnDamage
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_MarwanAsh3,
                    "ITEMSTATS_MYSTICSITEMS_RADIUS",
                    1f,
                    1f,
                    statFormatter: MarwanAshRadius
                );
                // Ghost Apple
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_GhostApple,
                    "ITEMSTATS_MYSTICSITEMS_REGEN",
                    Items.GhostApple.regen,
                    Items.GhostApple.regen,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Regen
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_GhostAppleWeak,
                    "ITEMSTATS_MYSTICSITEMS_REGEN",
                    Items.GhostAppleWeak.regenWeak,
                    Items.GhostAppleWeak.regenWeak,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Regen
                );
                // Constant Flow
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_Flow,
                    "ITEMSTATS_MYSTICSITEMS_MOVESPEED",
                    Items.Flow.moveSpeed / 100f,
                    Items.Flow.moveSpeedPerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent,
                    itemTag: BetterUI.ItemStats.ItemTag.MovementSpeed
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_Flow,
                    "ITEMSTATS_MYSTICSITEMS_REDUCTION",
                    -Items.Flow.slowReduction / 100f,
                    -Items.Flow.slowReductionPerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_Flow,
                    "ITEMSTATS_MYSTICSITEMS_MOVESPEED",
                    1f,
                    1f,
                    statFormatter: new BetterUI.ItemStats.StatFormatter()
                    {
                        suffix = "%",
                        style = BetterUI.ItemStats.Styles.Health,
                        statFormatter = (sb, value, master) =>
                        {
                            var currentRootSlow = -Items.Flow.initialRootSlow.Value;
                            if (value > 1f)
                            {
                                currentRootSlow /= Items.Flow.rootSlowReductionPerStack * (value - 1f);
                            }
                            sb.AppendFormat("{0:0.##}", currentRootSlow);
                        }
                    }
                );
                // Gachapon Coin
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_GachaponToken,
                    "ITEMSTATS_MYSTICSITEMS_CRIT",
                    Buffs.GachaponBonus.critBonus / 100f,
                    Buffs.GachaponBonus.critBonusPerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_GachaponToken,
                    "ITEMSTATS_MYSTICSITEMS_ATTACKSPEED",
                    Buffs.GachaponBonus.attackSpeedBonus / 100f,
                    Buffs.GachaponBonus.attackSpeedBonusPerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_GachaponToken,
                    "ITEMSTATS_MYSTICSITEMS_CRIT",
                    Items.GachaponToken.passiveCritBonus / 100f,
                    Items.GachaponToken.passiveCritBonusPerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_GachaponToken,
                    "ITEMSTATS_MYSTICSITEMS_ATTACKSPEED",
                    Items.GachaponToken.passiveAttackSpeedBonus / 100f,
                    Items.GachaponToken.passiveAttackSpeedBonusPerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );

                // Tier 2
                // Vendetta
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_AllyDeathRevenge,
                    "ITEMSTATS_MYSTICSITEMS_DURATION",
                    Items.AllyDeathRevenge.duration,
                    Items.AllyDeathRevenge.durationPerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Seconds
                );
                // Cup of Expresso
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_AllyDeathRevenge,
                    "ITEMSTATS_MYSTICSITEMS_MAXBUFFSTACKS",
                    Items.CoffeeBoostOnItemPickup.maxBuffs,
                    Items.CoffeeBoostOnItemPickup.maxBuffsPerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Charges
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_AllyDeathRevenge,
                    "ITEMSTATS_MYSTICSITEMS_MAXATTACKSPEEDBONUS",
                    Buffs.CoffeeBoost.boostPower / 100f * Items.CoffeeBoostOnItemPickup.maxBuffs,
                    Buffs.CoffeeBoost.boostPower / 100f * Items.CoffeeBoostOnItemPickup.maxBuffsPerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_AllyDeathRevenge,
                    "ITEMSTATS_MYSTICSITEMS_MAXMOVESPEEDBONUS",
                    Buffs.CoffeeBoost.boostPower / 100f * Items.CoffeeBoostOnItemPickup.maxBuffs,
                    Buffs.CoffeeBoost.boostPower / 100f * Items.CoffeeBoostOnItemPickup.maxBuffsPerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                // Contraband Gunpowder
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_ExplosivePickups,
                    "ITEMSTATS_MYSTICSITEMS_DAMAGE",
                    Items.ExplosivePickups.damage / 100f,
                    Items.ExplosivePickups.damagePerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent,
                    itemTag: BetterUI.ItemStats.ItemTag.Damage
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_ExplosivePickups,
                    "ITEMSTATS_MYSTICSITEMS_RADIUS",
                    Items.ExplosivePickups.radius,
                    Items.ExplosivePickups.radiusPerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Range
                );
                BetterUI.ItemStats.RegisterProc(
                    MysticsItemsContent.Items.MysticsItems_ExplosivePickups,
                    Items.ExplosivePickups.flaskDropChance / 100f,
                    stackingFormula: BetterUI.ItemStats.NoStacking
                );
                // Black Monolith

                // Platinum Card

                // Mystic Sword
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_MysticSword,
                    "ITEMSTATS_MYSTICSITEMS_DAMAGE",
                    0f,
                    0f,
                    statFormatter: MysticSwordDamageBonusFormat
                );
                BetterUI.ItemStats.RegisterModifier(
                    BetterUI.ItemStats.ItemTag.Damage,
                    MysticsItemsContent.Items.MysticsItems_MysticSword,
                    BetterUI.ItemStats.ItemModifier.PercentBonus,
                    100f,
                    stackModifier: 0f,
                    modificationCounter: MysticSwordDamageBonusModifier
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_MysticSword,
                    "ITEMSTATS_MYSTICSITEMS_UPTO",
                    Items.MysticSword.maxDamage / 100f,
                    Items.MysticSword.maxDamagePerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_MysticSword,
                    "ITEMSTATS_MYSTICSITEMS_BONUSPERKILL",
                    Items.MysticSword.damage / 100f,
                    Items.MysticSword.damagePerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                // Metronome
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_Rhythm,
                    "ITEMSTATS_MYSTICSITEMS_CRIT",
                    Items.Rhythm.critBonus / 100f,
                    Items.Rhythm.critBonusPerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                // Nuclear Accelerator
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_SpeedGivesDamage,
                    "ITEMSTATS_MYSTICSITEMS_DAMAGEPERSPEEDINCREASE",
                    Items.SpeedGivesDamage.damage / 100f,
                    Items.SpeedGivesDamage.damagePerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                BetterUI.ItemStats.RegisterModifier(
                    BetterUI.ItemStats.ItemTag.Damage,
                    MysticsItemsContent.Items.MysticsItems_SpeedGivesDamage,
                    BetterUI.ItemStats.ItemModifier.PercentBonus,
                    100f,
                    stackModifier: 0f,
                    modificationCounter: SpeedGivesDamageBonusModifier
                );
                // Faulty Spotter

                // Failed Experiment
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_ElitePotion,
                    "ITEMSTATS_MYSTICSITEMS_RADIUS",
                    Items.ElitePotion.radius,
                    Items.ElitePotion.radiusPerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Range
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_ElitePotion,
                    "ITEMSTATS_MYSTICSITEMS_DURATION",
                    Items.ElitePotion.duration,
                    Items.ElitePotion.durationPerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Seconds
                );
                // Spare Wiring
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_DroneWires,
                    "ITEMSTATS_MYSTICSITEMS_DAMAGE",
                    Items.DroneWires.damage / 100f,
                    Items.DroneWires.damagePerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent,
                    itemTag: BetterUI.ItemStats.ItemTag.Damage
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_DroneWires,
                    "ITEMSTATS_MYSTICSITEMS_DAMAGE",
                    Items.DroneWires.playerDamage / 100f,
                    Items.DroneWires.playerDamagePerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent,
                    itemTag: BetterUI.ItemStats.ItemTag.Damage
                );
                // Ceremony of Perdition
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_DeathCeremony,
                    "ITEMSTATS_MYSTICSITEMS_DAMAGE",
                    Items.DeathCeremony.damage / 100f,
                    Items.DeathCeremony.damagePerStack / 100f,
                    stackingFormula: BetterUI.ItemStats.HyperbolicStacking,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                // Purrfect Headphones
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_BuffInTPRange,
                    "ITEMSTATS_MYSTICSITEMS_MOVESPEED",
                    Buffs.BuffInTPRange.moveSpeed / 100f,
                    Buffs.BuffInTPRange.moveSpeedPerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent,
                    itemTag: BetterUI.ItemStats.ItemTag.MovementSpeed
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_BuffInTPRange,
                    "ITEMSTATS_MYSTICSITEMS_ARMOR",
                    Buffs.BuffInTPRange.armor,
                    Buffs.BuffInTPRange.armorPerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Armor
                );
                // Inoperative Nanomachines
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_BuffInTPRange,
                    "ITEMSTATS_MYSTICSITEMS_ARMOR",
                    Buffs.NanomachineArmor.armor,
                    Buffs.NanomachineArmor.armorPerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                // Snow Ring
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_BuffInTPRange,
                    "ITEMSTATS_MYSTICSITEMS_RADIUS",
                    Items.SnowRing.radius,
                    Items.SnowRing.radiusPerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Range
                );
                // Stargazer's Records
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_StarBook,
                    "ITEMSTATS_MYSTICSITEMS_CHANCE",
                    Items.StarBook.chance,
                    0f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Chance,
                    itemTag: BetterUI.ItemStats.ItemTag.Luck
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_StarBook,
                    "ITEMSTATS_MYSTICSITEMS_DAMAGE",
                    Items.StarBook.damage,
                    Items.StarBook.damagePerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent,
                    itemTag: BetterUI.ItemStats.ItemTag.Damage
                );
                // Time Dilator
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_TimePiece,
                    "ITEMSTATS_MYSTICSITEMS_MOVESPEED",
                    Buffs.TimePieceSlow.slow / 100f,
                    Buffs.TimePieceSlow.slowPerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );

                // Tier 3
                // Crystallized World
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_CrystalWorld,
                    "ITEMSTATS_MYSTICSITEMS_PULSES",
                    Items.CrystalWorld.pulses,
                    Items.CrystalWorld.pulsesPerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Charges
                );
                // Timely Execution
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_DasherDisc,
                    "ITEMSTATS_MYSTICSITEMS_COOLDOWN",
                    1f,
                    1f,
                    stackingFormula: DasherDiscCooldown,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Seconds
                );
                // Super Idol
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_Idol,
                    "ITEMSTATS_MYSTICSITEMS_GOLDFORMAXBUFF",
                    1f,
                    1f,
                    stackingFormula: SuperIdolGoldForMaxBuff,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Gold
                );
                // Thought Processor
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_ThoughtProcessor,
                    "ITEMSTATS_MYSTICSITEMS_COOLDOWN",
                    Items.ThoughtProcessor.cdr / 100f,
                    Items.ThoughtProcessor.cdrPerStack / 100f,
                    stackingFormula: BetterUI.ItemStats.HyperbolicStacking,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                // Treasure Map
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_TreasureMap,
                    "ITEMSTATS_MYSTICSITEMS_TIME",
                    1f,
                    1f,
                    stackingFormula: TreasureMapChargeTime,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Seconds
                );
                // Wireless Voltmeter
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_Voltmeter,
                    "ITEMSTATS_MYSTICSITEMS_DAMAGE",
                    Items.Voltmeter.damage / 100f,
                    Items.Voltmeter.damagePerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent,
                    itemTag: BetterUI.ItemStats.ItemTag.Damage
                );
                // Hiker's Backpack
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_Backpack,
                    "ITEMSTATS_MYSTICSITEMS_CHARGES",
                    Items.Backpack.charges,
                    Items.Backpack.chargesPerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Charges
                );
                // Devil's Cry
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_JudgementCut,
                    "ITEMSTATS_MYSTICSITEMS_DAMAGE",
                    Items.JudgementCut.damagePerSlash / 100f * Items.JudgementCut.slashCount,
                    Items.JudgementCut.damagePerSlash / 100f * Items.JudgementCut.slashCountPerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent,
                    itemTag: BetterUI.ItemStats.ItemTag.Damage
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_JudgementCut,
                    "ITEMSTATS_MYSTICSITEMS_RADIUS",
                    Items.JudgementCut.radius,
                    Items.JudgementCut.radiusPerStack,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Range
                );
                // Ten Commandments of Vyrael

                // Charger Upgrade Module
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_ShieldUpgrade,
                    "ITEMSTATS_MYSTICSITEMS_SHIELD",
                    Items.ShieldUpgrade.shieldBonus / 100f,
                    Items.ShieldUpgrade.shieldBonusPerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
                BetterUI.ItemStats.RegisterStat(
                    MysticsItemsContent.Items.MysticsItems_ShieldUpgrade,
                    "ITEMSTATS_MYSTICSITEMS_REDUCTION",
                    Items.ShieldUpgrade.rechargeBoost / 100f,
                    Items.ShieldUpgrade.rechargeBoostPerStack / 100f,
                    statFormatter: BetterUI.ItemStats.StatFormatter.Percent
                );
            }

            // stacking formulas
            public static float DivideOnEachStack(float value, float extraStackValue, int stacks)
            {
                return -(value - value / Mathf.Pow(extraStackValue, stacks));
            }
            public static float DasherDiscCooldown(float value, float extraStackValue, int stacks)
            {
                return Items.DasherDisc.CalculateCooldown(stacks);
            }
            public static float SuperIdolGoldForMaxBuff(float value, float extraStackValue, int stacks)
            {
                return Items.Idol.CalculateIdolCap(stacks);
            }
            public static float TreasureMapChargeTime(float value, float extraStackValue, int stacks)
            {
                return Items.TreasureMap.CalculateChargeTime(stacks);
            }

            // stat formatters
            public static BetterUI.ItemStats.StatFormatter LimitedArmorRemainingHits = new BetterUI.ItemStats.StatFormatter()
            {
                suffix = "",
                style = BetterUI.ItemStats.Styles.Stack,
                statFormatter = (sb, value, master) =>
                {
                    if (master.inventory)
                    {
                        var component = master.inventory.GetComponent<Items.LimitedArmor.MysticsItemsLimitedArmorBehavior>();
                        if (component) sb.Append(component.GetTotalStock());
                        else sb.Append("0");
                    }
                    else sb.Append("0");
                }
            };
            public static BetterUI.ItemStats.StatFormatter MarwanAshDamage = new BetterUI.ItemStats.StatFormatter()
            {
                suffix = "%",
                style = BetterUI.ItemStats.Styles.Damage,
                statFormatter = (sb, value, master) =>
                {
                    var level = master.hasBody ? master.GetBody().level : 1f;
                    sb.AppendFormat("{0:0.##}", Items.MarwanAsh1.damage + Items.MarwanAsh1.damagePerLevel * level * value);
                }
            };
            public static BetterUI.ItemStats.StatFormatter MarwanAshBurnDamage = new BetterUI.ItemStats.StatFormatter()
            {
                suffix = "%",
                style = BetterUI.ItemStats.Styles.Damage,
                statFormatter = (sb, value, master) =>
                {
                    var level = master.hasBody ? master.GetBody().level : 1f;
                    sb.AppendFormat("{0:0.##}", Items.MarwanAsh1.dotPercent + Items.MarwanAsh1.dotPercentPerLevel * (level - Items.MarwanAsh1.upgradeLevel12) * value);
                }
            };
            public static BetterUI.ItemStats.StatFormatter MarwanAshRadius = new BetterUI.ItemStats.StatFormatter()
            {
                suffix = "",
                style = BetterUI.ItemStats.Styles.Damage,
                statFormatter = (sb, value, master) =>
                {
                    var level = master.hasBody ? master.GetBody().level : 1f;
                    sb.AppendFormat("{0:0.##}", Items.MarwanAsh1.radius + Items.MarwanAsh1.radiusPerLevel * (level - Items.MarwanAsh1.upgradeLevel23) * value);
                    sb.Append(" " + Language.GetString("ITEMSTATS_MYSTICSITEMS_POSTFIX_METERS"));
                }
            };
            public static BetterUI.ItemStats.StatFormatter MysticSwordDamageBonusFormat = new BetterUI.ItemStats.StatFormatter()
            {
                suffix = "%",
                style = BetterUI.ItemStats.Styles.Damage,
                statFormatter = (sb, value, master) =>
                {
                    if (master.inventory)
                    {
                        var component = master.inventory.GetComponent<Items.MysticSword.MysticsItemsMysticSwordBehaviour>();
                        if (component) sb.AppendFormat("{0:0.##}", component.damageBonus * 100f);
                        else sb.Append("0");
                    }
                    else sb.Append("0");
                }
            };
            public static BetterUI.ItemStats.StatFormatter SpeedGivesDamageBonusFormat = new BetterUI.ItemStats.StatFormatter()
            {
                suffix = "%",
                style = BetterUI.ItemStats.Styles.Damage,
                statFormatter = (sb, value, master) =>
                {
                    if (master.hasBody)
                        sb.AppendFormat("{0:0.##}", Items.SpeedGivesDamage.CalculateDamageBonus(master.GetBody(), (int)value));
                    else sb.Append("0");
                }
            };

            // item modification formulas
            public static float FlatBonusProper(float value, float modifier, float stackModifier, float stacks)
            {
                return modifier + stackModifier * (stacks - 1);
            }

            // item modification counters
            public static float MysticSwordDamageBonusModifier(CharacterMaster master, BetterUI.ItemStats.ItemModifier itemModifier)
            {
                if (master.inventory)
                {
                    var component = master.inventory.GetComponent<Items.MysticSword.MysticsItemsMysticSwordBehaviour>();
                    if (component) return component.damageBonus * 100f;
                }
                return 0f;
            }
            public static float SpeedGivesDamageBonusModifier(CharacterMaster master, BetterUI.ItemStats.ItemModifier itemModifier)
            {
                if (master.hasBody)
                    return Items.SpeedGivesDamage.CalculateDamageBonus(master.GetBody(), master.inventory.GetItemCount(itemModifier.itemDef)) * 100f;
                return 0f;
            }
        }
    }
}
