using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Buffs
{
    public class CoffeeBoost : BaseBuff
    {
        public override void OnLoad() {
            buffDef.name = "CoffeeBoost";
            buffDef.buffColor = new Color(130f / 255f, 130f / 255f, 130f / 255f);
            buffDef.canStack = true;
            AddMoveSpeedModifier(0.07f);
            AddAttackSpeedModifier(0.07f);
        }
    }
}
