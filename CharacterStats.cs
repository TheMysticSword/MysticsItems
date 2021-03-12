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
            public System.Func<Main.GenericCharacterInfo, float> times;
        }

        public static List<StatModifier> levelModifiers = new List<StatModifier>(); // only flat
        public static List<StatModifier> healthModifiers = new List<StatModifier>();
        public static List<StatModifier> shieldModifiers = new List<StatModifier>();
        public static List<StatModifier> regenModifiers = new List<StatModifier>(); // only flat
        public static List<StatModifier> moveSpeedModifiers = new List<StatModifier>();
        public static List<StatModifier> damageModifiers = new List<StatModifier>();
        public static List<StatModifier> attackSpeedModifiers = new List<StatModifier>();

        public static void ErrorHookFailed(string name)
        {
            Main.logger.LogError(MysticsItemsPlugin.PluginName + ": \"" + name + "\" hook failed");
        }
        public static void Init()
        {
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);

                Main.GenericCharacterInfo genericCharacterInfo = default(Main.GenericCharacterInfo);

                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<System.Action<CharacterBody>>((characterBody) => {
                    genericCharacterInfo = new Main.GenericCharacterInfo(characterBody);
                });

                // level
                if (c.TryGotoNext(
                    MoveType.Before,
                    x => x.MatchLdcI4(0),
                    x => x.MatchStloc(0)
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<System.Action<CharacterBody>>((characterBody) =>
                    {
                        int num = 0;
                        foreach (StatModifier statModifier in levelModifiers)
                        {
                            int times = (int)statModifier.times(genericCharacterInfo);
                            if (times != 0f && statModifier.flat != 0f)
                            {
                                num += (int)statModifier.flat * times;
                            }
                        }
                        if (num != 0) characterBody.SetPropertyValue("level", characterBody.level + num);
                    });
                }
                else ErrorHookFailed("level");

                // max health
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdcR4(1),
                    x => x.MatchStloc(42)
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
                    c.Emit(OpCodes.Ldloc, 42);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 42);

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
                    c.Emit(OpCodes.Ldloc, 41);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 41);
                }
                else ErrorHookFailed("max health");

                // max shield
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("baseMaxShield"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("levelMaxShield"),
                    x => x.MatchLdloc(35),
                    x => x.MatchMul(),
                    x => x.MatchAdd(),
                    x => x.MatchStloc(43)
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<System.Func<CharacterBody, float>>((characterBody) =>
                    {
                        float num = 0;
                        foreach (StatModifier statModifier in shieldModifiers) {
                            float times = statModifier.times(genericCharacterInfo);
                            if (times != 0f)
                            {
                                num += characterBody.maxHealth * statModifier.multiplier * times + statModifier.flat * times;
                            }
                        }
                        return num;
                    });
                    c.Emit(OpCodes.Ldloc, 43);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 43);
                }
                else ErrorHookFailed("max shield");

                // regen
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdloc(52),
                    x => x.MatchLdloc(49),
                    x => x.MatchAdd(),
                    x => x.MatchStloc(52)
                ))
                {
                    c.Emit(OpCodes.Ldloc, 45);
                    c.Emit(OpCodes.Ldloc, 51);
                    c.EmitDelegate<System.Func<float, float, float>>((levelMultiplier, regenMultiplier) =>
                    {
                        float num = 0;
                        foreach (StatModifier statModifier in regenModifiers)
                        {
                            float times = statModifier.times(genericCharacterInfo);
                            if (times != 0f && statModifier.flat != 0f)
                            {
                                num += statModifier.flat * levelMultiplier * regenMultiplier * times;
                            }
                        }
                        return num;
                    });
                    c.Emit(OpCodes.Ldloc, 43);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 43);
                }
                else ErrorHookFailed("regen");

                // movement speed
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdcR4(1),
                    x => x.MatchStloc(55)
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
                    c.Emit(OpCodes.Ldloc, 53);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 53);

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
                    c.Emit(OpCodes.Ldloc, 54);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 54);

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
                    c.Emit(OpCodes.Ldloc, 55);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 55);
                }
                else ErrorHookFailed("movement speed");

                // damage
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdcR4(1),
                    x => x.MatchStloc(58)
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
                    c.Emit(OpCodes.Ldloc, 58);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 58);

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
                    c.Emit(OpCodes.Ldloc, 57);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 57);
                }
                else ErrorHookFailed("damage");

                // attack speed
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdcR4(1),
                    x => x.MatchStloc(61)
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
                    c.Emit(OpCodes.Ldloc, 61);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 61);

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
                    c.Emit(OpCodes.Ldloc, 60);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 60);
                }
                else ErrorHookFailed("attack speed");
            };
        }
    }
}
