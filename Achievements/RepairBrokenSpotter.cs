using RoR2;
using RoR2.Achievements;

namespace MysticsItems.Achievements
{
    public class RepairBrokenSpotter
    {
		[RegisterAchievement("MysticsItems_RepairBrokenSpotter", "Items.MysticsItems_Spotter", null, typeof(Server))]
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
				public override void OnInstall()
				{
					base.OnInstall();
					Items.Spotter.MysticsItemsSpotterUnlockInteraction.OnUnlock += OnActivated;
				}

				public override void OnUninstall()
				{
					base.OnUninstall();
					Items.Spotter.MysticsItemsSpotterUnlockInteraction.OnUnlock -= OnActivated;
				}

				public void OnActivated(Interactor interactor)
				{
					CharacterBody currentBody = serverAchievementTracker.networkUser.GetCurrentBody();
					if (currentBody && currentBody.GetComponent<Interactor>() == interactor)
					{
						Grant();
					}
				}
			}
		}
    }
}
