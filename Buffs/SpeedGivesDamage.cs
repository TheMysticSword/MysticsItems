using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Buffs
{
    public class SpeedGivesDamage : BaseBuff
    {
        public override void PreAdd() {
            buffDef.name = "SpeedGivesDamage";
            buffDef.buffColor = new Color32(200, 255, 140, 255);
            buffDef.canStack = true;
        }

        public override void OnAdd() {
            Items.SpeedGivesDamage.buffIndex = buffIndex;

            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                // damage
                if (c.TryGotoNext(
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
                            return (Items.SpeedGivesDamage.percentPerBuffStack / 100f) * characterBody.GetBuffCount(buffIndex);
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
