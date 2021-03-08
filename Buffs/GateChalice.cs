using RoR2;
using R2API.Utils;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Buffs
{
    public class GateChalice : BaseBuff
    {
        public override void PreAdd() {
            buffDef.name = "GateChalice";
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
                            return 0.33f * characterBody.GetBuffCount(buffIndex);
                        }
                        return 0;
                    });
                    c.Emit(OpCodes.Ldloc, 55);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 55);
                }
                // armor reduction
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdarg(0),
                    x => x.MatchCallOrCallvirt<CharacterBody>("get_armor"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdcI4(0x2B),
                    x => x.MatchCallOrCallvirt<CharacterBody>("HasBuff"),
                    x => x.MatchBrtrue(out _),
                    x => x.MatchLdcR4(0),
                    x => x.MatchBr(out _),
                    x => x.MatchLdcR4(500),
                    x => x.MatchAdd(),
                    x => x.MatchCallOrCallvirt<CharacterBody>("set_armor")
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<System.Action<CharacterBody>>((characterBody) =>
                    {
                        if (characterBody.HasBuff(buffIndex))
                        {
                            characterBody.SetPropertyValue("armor", characterBody.armor - 10f * characterBody.GetBuffCount(buffIndex));
                        }
                    });
                }
            };
        }
    }
}
