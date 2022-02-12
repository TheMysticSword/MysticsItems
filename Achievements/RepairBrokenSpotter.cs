using MysticsRisky2Utils.BaseAssetTypes;
using RoR2;
using UnityEngine;

namespace MysticsItems.Achievements
{
    public class RepairBrokenSpotter : BaseAchievement
    {
        public override void OnLoad()
        {
            name = "MysticsItems_RepairBrokenSpotter";
            unlockableName = "Items.MysticsItems_Spotter";
			iconSprite = MysticsRisky2Utils.Utils.AddItemIconBackgroundToSprite(Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Spotter/Icon.png"), MysticsRisky2Utils.Utils.ItemIconBackgroundType.Tier2);
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
