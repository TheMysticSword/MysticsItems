namespace MysticsItems.Buffs
{
    public class DasherDiscCooldown : BaseBuff
    {
        public override void OnLoad() {
            buffDef.name = "DasherDiscCooldown";
            buffDef.buffColor = UnityEngine.Color.white;
            buffDef.canStack = true;
            buffDef.isDebuff = true;
        }
    }
}
