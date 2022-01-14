using MysticsRisky2Utils.BaseAssetTypes;
using RoR2;
using UnityEngine;

namespace MysticsItems.Achievements
{
    public class HellSpeedrun : BaseAchievement
    {
        public override void OnLoad()
        {
            name = "MysticsItems_HellSpeedrun";
            unlockableName = "Items.MysticsItems_RiftLens";
			iconSprite = MysticsRisky2Utils.Utils.AddItemIconBackgroundToSprite(Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Rift Lens/Icon.png"), MysticsRisky2Utils.Utils.ItemIconBackgroundType.Lunar);
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
				public float timestamp = 0f;
				public float timeRequirement = 180f;
				public bool eligible = false;

				public override void OnInstall()
				{
					base.OnInstall();
                    Stage.onServerStageBegin += Stage_onServerStageBegin;
                    Stage.onServerStageComplete += Stage_onServerStageComplete;
				}

				public override void OnUninstall()
				{
					base.OnUninstall();
					Stage.onServerStageBegin -= Stage_onServerStageBegin;
					Stage.onServerStageComplete -= Stage_onServerStageComplete;
				}

				public void Stage_onServerStageBegin(Stage obj)
				{
					if (obj.sceneDef && obj.sceneDef.baseSceneName == "dampcavesimple")
					{
						eligible = true;
						timestamp = Run.instance.GetRunStopwatch();
					}
					else
					{
						eligible = false;
					}
				}

				public void Stage_onServerStageComplete(Stage obj)
				{
					if (eligible && (Run.instance.GetRunStopwatch() - timestamp) < timeRequirement)
                    {
						Grant();
                    }
				}
			}
		}
    }
}
