using MysticsRisky2Utils.BaseAssetTypes;
using RoR2;
using UnityEngine;

namespace MysticsItems.Achievements
{
    public class FindArchaicMask : BaseAchievement
    {
        public override void OnLoad()
        {
            name = "MysticsItems_FindArchaicMask";
            unlockableName = "Equipment.MysticsItems_ArchaicMask";
			iconSprite = MysticsRisky2Utils.Utils.AddItemIconBackgroundToSprite(Main.AssetBundle.LoadAsset<Sprite>("Assets/Equipment/Archaic Mask/Icon.png"), MysticsRisky2Utils.Utils.ItemIconBackgroundType.Equipment);
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
