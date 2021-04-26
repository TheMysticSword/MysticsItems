using RoR2;

namespace MysticsItems.Items
{
    public class RiftLensDebuff : BaseItem
    {
        public override void PreLoad()
        {
            itemDef.name = "RiftLensDebuff";
            itemDef.tier = ItemTier.NoTier;
        }

        public override void AfterTokensPopulated()
        {
            itemDef.descriptionToken = itemDef.pickupToken;
        }

        public override void OnLoad()
        {
            SetIcon("Rift Lens Debuff");

            CharacterStats.moveSpeedModifiers.Add(new CharacterStats.StatModifier
            {
                multiplier = -0.5f,
                times = (x) => ModifierTimesFunction(x, false)
            });
        }
    }
}
