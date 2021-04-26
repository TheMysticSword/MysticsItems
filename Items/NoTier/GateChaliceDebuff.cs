using RoR2;

namespace MysticsItems.Items
{
    public class GateChaliceDebuff : BaseItem
    {
        public override void PreLoad()
        {
            itemDef.name = "GateChaliceDebuff";
            itemDef.tier = ItemTier.NoTier;
        }

        public override void AfterTokensPopulated()
        {
            itemDef.descriptionToken = itemDef.pickupToken;
        }

        public override void OnLoad()
        {
            SetIcon("Gate Chalice Debuff");

            CharacterStats.moveSpeedModifiers.Add(new CharacterStats.StatModifier
            {
                multiplier = -0.33f,
                times = (x) => ModifierTimesFunction(x)
            });
            CharacterStats.armorModifiers.Add(new CharacterStats.FlatStatModifier
            {
                amount = -10f,
                times = (x) => ModifierTimesFunction(x)
            });
        }
    }
}
