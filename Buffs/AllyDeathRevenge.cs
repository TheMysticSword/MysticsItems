using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Buffs
{
    public class AllyDeathRevenge : BaseBuff
    {
        public override void OnLoad() {
            buffDef.name = "AllyDeathRevenge";
            buffDef.buffColor = new Color(211f / 255f, 50f / 255f, 25f / 255f);
            AddAttackSpeedModifier(1f);
            AddDamageModifier(1f);
        }
    }
}
