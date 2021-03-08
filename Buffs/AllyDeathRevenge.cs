using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Buffs
{
    public class AllyDeathRevenge : BaseBuff
    {
        public override void PreAdd() {
            buffDef.name = "AllyDeathRevenge";
            buffDef.buffColor = new Color(211f / 255f, 50f / 255f, 25f / 255f);
        }

        public override void OnAdd() {
            Items.AllyDeathRevenge.buffIndex = buffIndex;

            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
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
                            return 1f;
                        }
                        return 0;
                    });
                    c.Emit(OpCodes.Ldloc, 61);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 61);
                }
                // damage
                if (c.TryGotoPrev(
                    MoveType.After,
                    x => x.MatchLdcR4(1),
                    x => x.MatchStloc(58)
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<System.Func<CharacterBody, float>>((characterBody) =>
                    {
                        if (characterBody.HasBuff(buffIndex))
                        {
                            return 1f;
                        }
                        return 0;
                    });
                    c.Emit(OpCodes.Ldloc, 58);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 58);
                }
            };
        }
    }
}
