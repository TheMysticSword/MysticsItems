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

            GenericGameEvents.BeforeTakeDamage += (damageInfo, victimInfo) =>
            {
                if (victimInfo.body.HasBuff(buffDef)) damageInfo.crit = true;
            };

            Overlays.CreateOverlay(Main.AssetBundle.LoadAsset<Material>("Assets/Items/Spotter/matSpotterMarked.mat"), delegate (CharacterModel model)
            {
                return model.body.HasBuff(buffDef);
            });
        }
    }
}
