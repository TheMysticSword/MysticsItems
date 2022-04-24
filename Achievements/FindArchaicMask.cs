using RoR2;
using RoR2.Achievements;

namespace MysticsItems.Achievements
{
    public class FindArchaicMask
    {
		[RegisterAchievement("MysticsItems_FindArchaicMask", "Equipment.MysticsItems_ArchaicMask", null, typeof(Server))]
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
