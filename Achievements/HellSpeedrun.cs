using RoR2;
using RoR2.Achievements;

namespace MysticsItems.Achievements
{
    public class HellSpeedrun
    {
		[RegisterAchievement("MysticsItems_HellSpeedrun", "Items.MysticsItems_RiftLens", null, typeof(Server))]
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
