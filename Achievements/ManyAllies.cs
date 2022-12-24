using RoR2;
using RoR2.Achievements;
using UnityEngine;
using System.Linq;

namespace MysticsItems.Achievements
{
	[RegisterAchievement("MysticsItems_ManyAllies", "Items.MysticsItems_BuffInTPRange", null, typeof(Server))]
	public class ManyAllies : BaseAchievement
	{
		public override void OnInstall()
		{
			base.OnInstall();
			SetServerTracked(true);
		}

		public override void OnUninstall()
		{
			SetServerTracked(false);
			base.OnUninstall();
		}

		public class Server : BaseServerAchievement
		{
			public int allyRequirement = 10;

			public override void OnInstall()
			{
				base.OnInstall();
                MasterSummon.onServerMasterSummonGlobal += MasterSummon_onServerMasterSummonGlobal;
			}

            private void MasterSummon_onServerMasterSummonGlobal(MasterSummon.MasterSummonReport masterSummonReport)
            {
				var allyCount = TeamComponent.GetTeamMembers(TeamIndex.Player).Where(x => x.body && !x.body.isPlayerControlled && !x.body.bodyFlags.HasFlag(CharacterBody.BodyFlags.Mechanical)).Count();
				if (allyCount >= allyRequirement)
                {
					Grant();
                }
            }

			public override void OnUninstall()
			{
				base.OnUninstall();
				MasterSummon.onServerMasterSummonGlobal -= MasterSummon_onServerMasterSummonGlobal;
			}
		}
	}
}
