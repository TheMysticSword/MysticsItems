using RoR2;
using RoR2.Achievements;
using RoR2.Stats;

namespace MysticsItems.Achievements
{
    [RegisterAchievement("MysticsItems_BlockDamageWithArmor", "Items.MysticsItems_Nanomachines", null, null)]
	public class BlockDamageWithArmor : BaseStatMilestoneAchievement
	{
		public override StatDef statDef
		{
			get
			{
				return CustomStats.totalDamageBlockedWithArmor;
			}
		}

		public override ulong statRequirement
		{
			get
			{
				return 5000UL;
			}
		}
	}
}
