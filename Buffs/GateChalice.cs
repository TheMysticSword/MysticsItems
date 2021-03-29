using RoR2;
using R2API.Utils;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Buffs
{
    public class GateChalice : BaseBuff
    {
        public override void OnLoad() {
            buffDef.name = "GateChalice";
            buffDef.buffColor = new Color(97f / 255f, 163f / 255f, 239f / 255f);
            buffDef.canStack = true;
            buffDef.isDebuff = true;
            AddMoveSpeedModifier(-0.33f);
            AddArmorModifier(-10f);
        }
    }
}
