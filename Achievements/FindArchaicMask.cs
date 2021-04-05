using RoR2;

namespace MysticsItems.Achievements
{
    public class FindArchaicMask : BaseAchievement
    {
        public override void OnLoad()
        {
            name = "FindArchaicMask";
            unlockableName = Main.TokenPrefix + "Equipment.ArchaicMask";
			trackerType = typeof(Tracker);
			serverTrackerType = typeof(Tracker.Server);
        }

        public class Tracker : RoR2.Achievements.BaseAchievement
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

			public class Server : RoR2.Achievements.BaseServerAchievement
			{
				public override void OnInstall()
				{
					base.OnInstall();
					Equipment.ArchaicMask.MysticsItemsArchaicMaskUnlockInteraction.OnUnlock += OnActivated;
				}

				public override void OnUninstall()
				{
					base.OnUninstall();
					Equipment.ArchaicMask.MysticsItemsArchaicMaskUnlockInteraction.OnUnlock -= OnActivated;
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
