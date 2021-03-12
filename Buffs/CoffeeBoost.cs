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
            AddMoveSpeedModifier(0.07f);
            AddAttackSpeedModifier(0.07f);
        }
    }
}
