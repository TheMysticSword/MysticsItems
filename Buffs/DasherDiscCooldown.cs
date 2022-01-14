using MysticsRisky2Utils.BaseAssetTypes;
using UnityEngine;

namespace MysticsItems.Buffs
{
    public class DasherDiscCooldown : BaseBuff
    {
        public override void OnLoad() {
            buffDef.name = "MysticsItems_DasherDiscCooldown";
            buffDef.buffColor = UnityEngine.Color.white;
            buffDef.canStack = true;
            buffDef.isDebuff = true;
            buffDef.iconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Buffs/DasherDiscCooldown.png");
        }
    }
}
