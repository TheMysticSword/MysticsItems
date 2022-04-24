using RoR2;
using RoR2.Achievements;

namespace MysticsItems.Achievements
{
    public class EscapeMoonAlone
    {
		[RegisterAchievement("MysticsItems_EscapeMoonAlone", "Items.MysticsItems_AllyDeathRevenge", null, typeof(Server))]
		public class Tracker : BaseAchievement
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
				public BodyIndex requiredBodyIndex;

				public override void OnInstall()
				{
					base.OnInstall();
                    GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
					requiredBodyIndex = BodyCatalog.FindBodyIndex("BrotherHurtBody");
				}


				public override void OnUninstall()
				{
					base.OnUninstall();
					GlobalEventManager.onCharacterDeathGlobal -= GlobalEventManager_onCharacterDeathGlobal;
				}

				private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
				{
					if (damageReport.victimBodyIndex != requiredBodyIndex) return;
					foreach (var teamMember in TeamComponent.GetTeamMembers(serverAchievementTracker.networkUser.master.teamIndex))
					{
						if (teamMember.body != serverAchievementTracker.networkUser.GetCurrentBody() && teamMember.body.healthComponent && teamMember.body.healthComponent.alive)
						{
							return;
						}
					}
					Grant();
				}
			}
		}
    }
}
