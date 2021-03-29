using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Buffs
{
    public class RiftLens : BaseBuff
    {
        public override void OnLoad() {
            buffDef.name = "RiftLens";
            buffDef.buffColor = new Color(97f / 255f, 163f / 255f, 239f / 255f);
            buffDef.canStack = true;
            buffDef.isDebuff = true;
            AddMoveSpeedModifier(-0.5f, 0f, false);
        }
    }
}
