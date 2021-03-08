namespace MysticsItems.Buffs
{
    public class DasherDiscActive : BaseBuff
    {
        public override void PreAdd() {
            buffDef.name = "DasherDiscActive";
            buffDef.buffColor = UnityEngine.Color.white;
        }
    }
}
