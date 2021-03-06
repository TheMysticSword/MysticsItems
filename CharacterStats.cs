using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems
{
    public static class CharacterStats
    {
        public struct StatModifier
        {
            public float multiplier;
            public float flat;
            public System.Func<GenericGameEvents.GenericCharacterInfo, float> times;
        }

        public struct FlatStatModifier
        {
            public float amount;
            public System.Func<GenericGameEvents.GenericCharacterInfo, float> times;
        }

        public static List<FlatStatModifier> levelModifiers = new List<FlatStatModifier>();
        public static List<StatModifier> healthModifiers = new List<StatModifier>();
        public static List<StatModifier> shieldModifiers = new List<StatModifier>();
        public static List<FlatStatModifier> regenModifiers = new List<FlatStatModifier>();
        public static List<StatModifier> moveSpeedModifiers = new List<StatModifier>();
        public static List<StatModifier> damageModifiers = new List<StatModifier>();
        public static List<StatModifier> attackSpeedModifiers = new List<StatModifier>();
        public static List<FlatStatModifier> critModifiers = new List<FlatStatModifier>();
        public static List<FlatStatModifier> armorModifiers = new List<FlatStatModifier>();
        public static List<StatModifier> cooldownModifiers = new List<StatModifier>();

        public static void ErrorHookFailed(string name)
        {
            Main.logger.LogError(name + " stat hook failed");
        }
        public static void Init()
        {
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);

                GenericGameEvents.GenericCharacterInfo genericCharacterInfo = default(GenericGameEvents.GenericCharacterInfo);

                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<System.Action<CharacterBody>>((characterBody) => {
                    genericCharacterInfo = new GenericGameEvents.GenericCharacterInfo(characterBody);
                });

                int levelStatMultiplierPosition = 41;

                // level
                if (c.TryGotoNext(
                    MoveType.Before,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdloc(1),
                    x => x.MatchCallOrCallvirt<CharacterBody>("set_level")
                ))
                {
                    c.EmitDelegate<System.Action>(() =>
                    {
                        int num = 0;
                        foreach (FlatStatModifier statModifier in levelModifiers)
                        {
                            int times = (int)statModifier.times(genericCharacterInfo);
                            if (times != 0f && statModifier.amount != 0f)
                            {
                                num += (int)statModifier.amount * times;
                            }
                        }
                        if (num != 0) genericCharacterInfo.body.SetPropertyValue("level", genericCharacterInfo.body.level + num);
                    });
                }
                else ErrorHookFailed("level");

                // max health
                int maxHealthFlatPosition = 50;
                int maxHealthMultiplierPosition = 51;
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdfld<CharacterBody>("baseMaxHealth"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("levelMaxHealth")
                ) && c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchStloc(maxHealthMultiplierPosition)
                ))
                {
                    c.EmitDelegate<System.Func<float>>(() =>
                    {
                        float num = 0;
                        foreach (StatModifier statModifier in healthModifiers)
                        {
                            float times = statModifier.times(genericCharacterInfo);
                            if (times != 0f && statModifier.multiplier != 0f)
                            {
                                num += statModifier.multiplier * times;
                            }
                        }
                        return num;
                    });
                    c.Emit(OpCodes.Ldloc, maxHealthMultiplierPosition);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, maxHealthMultiplierPosition);

                    c.EmitDelegate<System.Func<float>>(() =>
                    {
                        float num = 0;
                        foreach (StatModifier statModifier in healthModifiers)
                        {
                            float times = statModifier.times(genericCharacterInfo);
                            if (times != 0f && statModifier.flat != 0f)
                            {
                                num += statModifier.flat * times;
                            }
                        }
                        return num;
                    });
                    c.Emit(OpCodes.Ldloc, maxHealthFlatPosition);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, maxHealthFlatPosition);
                }
                else ErrorHookFailed("max health");

                // max shield
                int maxShieldFlatPosition = 52;
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("baseMaxShield"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("levelMaxShield")
                ) && c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchStloc(maxShieldFlatPosition)
                ))
                {
                    c.EmitDelegate<System.Func<float>>(() =>
                    {
                        float num = 0;
                        foreach (StatModifier statModifier in shieldModifiers) {
                            float times = statModifier.times(genericCharacterInfo);
                            if (times != 0f)
                            {
                                num += genericCharacterInfo.body.maxHealth * statModifier.multiplier * times + statModifier.flat * times;
                            }
                        }
                        return num;
                    });
                    c.Emit(OpCodes.Ldloc, maxShieldFlatPosition);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, maxShieldFlatPosition);
                }
                else ErrorHookFailed("max shield");

                // regen
                int regenLevelMultiplierPosition = 54;
                int regenMultiplierPosition = 60;
                int regenFlatPosition = 61;
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("baseRegen"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("levelRegen")
                ) && c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchStloc(regenFlatPosition)
                ))
                {
                    c.Emit(OpCodes.Ldloc, regenLevelMultiplierPosition);
                    c.Emit(OpCodes.Ldloc, regenMultiplierPosition);
                    c.EmitDelegate<System.Func<float, float, float>>((levelMultiplier, regenMultiplier) =>
                    {
                        float num = 0;
                        foreach (FlatStatModifier statModifier in regenModifiers)
                        {
                            float times = statModifier.times(genericCharacterInfo);
                            if (times != 0f && statModifier.amount != 0f)
                            {
                                num += statModifier.amount * levelMultiplier * regenMultiplier * times;
                            }
                        }
                        return num;
                    });
                    c.Emit(OpCodes.Ldloc, regenFlatPosition);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, regenFlatPosition);
                }
                else ErrorHookFailed("regen");

                // movement speed
                int moveSpeedFlatPosition = 62;
                int moveSpeedMultiplierPosition = 63;
                int moveSpeedDivisorPosition = 64;
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdcR4(1),
                    x => x.MatchStloc(moveSpeedDivisorPosition)
                ))
                {
                    c.EmitDelegate<System.Func<float>>(() =>
                    {
                        float num = 0;
                        foreach (StatModifier statModifier in moveSpeedModifiers)
                        {
                            float times = statModifier.times(genericCharacterInfo);
                            if (times != 0f && statModifier.flat != 0f)
                            {
                                num += statModifier.flat * times;
                            }
                        }
                        return num;
                    });
                    c.Emit(OpCodes.Ldloc, moveSpeedFlatPosition);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, moveSpeedFlatPosition);

                    c.EmitDelegate<System.Func<float>>(() =>
                    {
                        float num = 0;
                        foreach (StatModifier statModifier in moveSpeedModifiers)
                        {
                            float times = statModifier.times(genericCharacterInfo);
                            if (times != 0f && statModifier.multiplier > 0f)
                            {
                                num += statModifier.multiplier * times;
                            }
                        }
                        return num;
                    });
                    c.Emit(OpCodes.Ldloc, moveSpeedMultiplierPosition);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, moveSpeedMultiplierPosition);

                    c.EmitDelegate<System.Func<float>>(() =>
                    {
                        float num = 0;
                        foreach (StatModifier statModifier in moveSpeedModifiers)
                        {
                            float times = statModifier.times(genericCharacterInfo);
                            if (times != 0f && statModifier.multiplier < 0f)
                            {
                                num -= statModifier.multiplier * times;
                            }
                        }
                        return num;
                    });
                    c.Emit(OpCodes.Ldloc, moveSpeedDivisorPosition);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, moveSpeedDivisorPosition);
                }
                else ErrorHookFailed("movement speed");

                // damage
                int damageFlatPosition = 66;
                int damageMultiplierPosition = 67;
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("baseDamage"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("levelDamage")
                ) && c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchStloc(damageMultiplierPosition)
                ))
                {
                    c.EmitDelegate<System.Func<float>>(() =>
                    {
                        float num = 0;
                        foreach (StatModifier statModifier in damageModifiers)
                        {
                            float times = statModifier.times(genericCharacterInfo);
                            if (times != 0f && statModifier.multiplier != 0f)
                            {
                                num += statModifier.multiplier * times;
                            }
                        }
                        return num;
                    });
                    c.Emit(OpCodes.Ldloc, damageMultiplierPosition);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, damageMultiplierPosition);

                    c.EmitDelegate<System.Func<float>>(() =>
                    {
                        float num = 0;
                        foreach (StatModifier statModifier in damageModifiers)
                        {
                            float times = statModifier.times(genericCharacterInfo);
                            if (times != 0f && statModifier.flat != 0f)
                            {
                                num += statModifier.flat * times;
                            }
                        }
                        return num;
                    });
                    c.Emit(OpCodes.Ldloc, damageFlatPosition);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, damageFlatPosition);
                }
                else ErrorHookFailed("damage");

                // attack speed
                int attackSpeedFlatPosition = 70;
                int attackSpeedMultiplierPosition = 71;
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("baseAttackSpeed"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("levelAttackSpeed")
                ) && c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchStloc(attackSpeedMultiplierPosition)
                ))
                {
                    c.EmitDelegate<System.Func<float>>(() =>
                    {
                        float num = 0;
                        foreach (StatModifier statModifier in attackSpeedModifiers)
                        {
                            float times = statModifier.times(genericCharacterInfo);
                            if (times != 0f && statModifier.multiplier != 0f)
                            {
                                num += statModifier.multiplier * times;
                            }
                        }
                        return num;
                    });
                    c.Emit(OpCodes.Ldloc, attackSpeedMultiplierPosition);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, attackSpeedMultiplierPosition);

                    c.EmitDelegate<System.Func<float>>(() =>
                    {
                        float num = 0;
                        foreach (StatModifier statModifier in attackSpeedModifiers)
                        {
                            float times = statModifier.times(genericCharacterInfo);
                            if (times != 0f && statModifier.flat != 0f)
                            {
                                num += statModifier.flat * times;
                            }
                        }
                        return num;
                    });
                    c.Emit(OpCodes.Ldloc, attackSpeedFlatPosition);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, attackSpeedFlatPosition);
                }
                else ErrorHookFailed("attack speed");

                // crit
                int critFlatPosition = 72;
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("baseCrit"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("levelCrit"),
                    x => x.MatchLdloc(levelStatMultiplierPosition),
                    x => x.MatchMul(),
                    x => x.MatchAdd(),
                    x => x.MatchStloc(critFlatPosition)
                ))
                {
                    c.EmitDelegate<System.Func<float>>(() =>
                    {
                        float num = 0;
                        foreach (FlatStatModifier statModifier in critModifiers)
                        {
                            float times = statModifier.times(genericCharacterInfo);
                            if (times != 0f && statModifier.amount != 0f)
                            {
                                num += statModifier.amount * times;
                            }
                        }
                        return num;
                    });
                    c.Emit(OpCodes.Ldloc, critFlatPosition);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, critFlatPosition);
                }
                else ErrorHookFailed("crit");

                // armor
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("baseArmor"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("levelArmor")
                ) && c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchCallOrCallvirt<CharacterBody>("set_armor")
                ))
                {
                    c.EmitDelegate<System.Action>(() =>
                    {
                        float num = 0;
                        foreach (FlatStatModifier statModifier in armorModifiers)
                        {
                            float times = statModifier.times(genericCharacterInfo);
                            if (times != 0f && statModifier.amount != 0f)
                            {
                                num += statModifier.amount * times;
                            }
                        }
                        if (num != 0) genericCharacterInfo.body.SetPropertyValue("armor", genericCharacterInfo.body.armor + num);
                    });
                }
                else ErrorHookFailed("armor");

                // cooldown
                int cooldownFlatPosition = 73;
                int cooldownMultiplierPosition = 74;
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdcR4(1),
                    x => x.MatchStloc(cooldownMultiplierPosition)
                ))
                {
                    c.EmitDelegate<System.Func<float>>(() =>
                    {
                        float num = 0;
                        foreach (StatModifier statModifier in cooldownModifiers)
                        {
                            float times = statModifier.times(genericCharacterInfo);
                            if (times != 0f && statModifier.multiplier != 0f)
                            {
                                num += statModifier.multiplier * times;
                            }
                        }
                        return num;
                    });
                    c.Emit(OpCodes.Ldloc, cooldownMultiplierPosition);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, cooldownMultiplierPosition);

                    c.EmitDelegate<System.Func<float>>(() =>
                    {
                        float num = 0;
                        foreach (StatModifier statModifier in cooldownModifiers)
                        {
                            float times = statModifier.times(genericCharacterInfo);
                            if (times != 0f && statModifier.flat != 0f)
                            {
                                num += statModifier.flat * times;
                            }
                        }
                        return num;
                    });
                    c.Emit(OpCodes.Ldloc, cooldownFlatPosition);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, cooldownFlatPosition);
                }
                else ErrorHookFailed("cooldown");
            };
        }
    }
}
