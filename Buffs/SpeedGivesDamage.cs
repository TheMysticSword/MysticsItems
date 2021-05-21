using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Buffs
{
    public class SpeedGivesDamage : BaseBuff
    {
        public static float damagePerStack = 0.1f;

        public override void OnLoad() {
            buffDef.name = "SpeedGivesDamage";
            buffDef.buffColor = new Color32(200, 255, 140, 255);
            buffDef.canStack = true;
            AddDamageModifier(damagePerStack);

            Overlays.CreateOverlay(Main.AssetBundle.LoadAsset<Material>("Assets/Items/Nuclear Accelerator/matNuclearAcceleratorActiveOverlay.mat"), delegate (CharacterModel model)
            {
                return model.body.HasBuff(buffDef);
            });
        }
    }
}
