using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Buffs
{
    public class SpotterMarked : BaseBuff
    {
        public override void OnLoad() {
            buffDef.name = "SpotterMarked";
            buffDef.buffColor = new Color32(214, 58, 58, 255);
            buffDef.isDebuff = true;

            Items.Spotter.buffDef = buffDef;

            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                // always take damage as crits
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(1),
                    x => x.MatchLdfld<DamageInfo>("damage"),
                    x => x.MatchStloc(5)
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldarg_1);
                    c.EmitDelegate<System.Action<HealthComponent, DamageInfo>>((healthComponent, damageInfo) =>
                    {
                        CharacterBody body = healthComponent.body;
                        if (body && body.HasBuff(buffDef))
                        {
                            damageInfo.crit = true;
                        }
                    });
                }
            };
        }
    }
}
