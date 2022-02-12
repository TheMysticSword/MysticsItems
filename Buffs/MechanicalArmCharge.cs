using MysticsRisky2Utils.BaseAssetTypes;
using UnityEngine;

namespace MysticsItems.Buffs
{
    public class MechanicalArmCharge : BaseBuff
    {
        public override void OnLoad() {
            buffDef.name = "MysticsItems_MechanicalArmCharge";
            buffDef.buffColor = new Color32(255, 92, 63, 255);
            buffDef.canStack = true;
            buffDef.iconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Buffs/MechanicalArmCharge.png");
        }
    }
}
