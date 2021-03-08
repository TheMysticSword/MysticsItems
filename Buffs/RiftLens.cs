using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Buffs
{
    public class RiftLens : BaseBuff
    {
        public override void PreAdd() {
            buffDef.name = "RiftLens";
            buffDef.buffColor = new Color(97f / 255f, 163f / 255f, 239f / 255f);
            buffDef.canStack = true;
            buffDef.isDebuff = true;
        }

        public override void OnAdd() {
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                // movement speed reduction
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdcR4(1),
                    x => x.MatchStloc(55)
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<System.Func<CharacterBody, float>>((characterBody) =>
                    {
                        if (characterBody.HasBuff(buffIndex))
                        {
                            return 0.5f;
                        }
                        return 0;
                    });
                    c.Emit(OpCodes.Ldloc, 55);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 55);
                }
            };
        }
    }
}
