using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Buffs
{
    public class CoffeeBoost : BaseBuff
    {
        public override void PreAdd() {
            buffDef.name = "CoffeeBoost";
            buffDef.buffColor = new Color(130f / 255f, 130f / 255f, 130f / 255f);
            buffDef.canStack = true;
        }

        public override void OnAdd() {
            Items.CoffeeBoostOnItemPickup.buffIndex = buffIndex;

            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                // movement speed
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdcR4(1),
                    x => x.MatchStloc(54)
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<System.Func<CharacterBody, float>>((characterBody) =>
                    {
                        if (characterBody.HasBuff(buffIndex))
                        {
                            return 0.07f * characterBody.GetBuffCount(buffIndex);
                        }
                        return 0;
                    });
                    c.Emit(OpCodes.Ldloc, 54);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 54);
                }
                // attack speed
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdcR4(1),
                    x => x.MatchStloc(61)
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<System.Func<CharacterBody, float>>((characterBody) =>
                    {
                        if (characterBody.HasBuff(buffIndex))
                        {
                            return 0.07f * characterBody.GetBuffCount(buffIndex);
                        }
                        return 0;
                    });
                    c.Emit(OpCodes.Ldloc, 61);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 61);
                }
            };
        }
    }
}
