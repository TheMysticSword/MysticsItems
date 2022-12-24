using RoR2;
using RoR2.Achievements;
using UnityEngine;
using System.Linq;

namespace MysticsItems.Achievements
{
	[RegisterAchievement("MysticsItems_Woolierush", "Items.MysticsItems_Rhythm", null, typeof(Server))]
	public class Woolierush : BaseAchievement
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
			public float timeMax = 300f;
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
				if (eligible && obj.sceneDef && obj.sceneDef.isFinalStage)
				{
					Grant();
					eligible = true;
                }
                else
                {
					timestamp = Run.instance.GetRunStopwatch();
				}
			}

			public void Stage_onServerStageComplete(Stage obj)
			{
				if ((Run.instance.GetRunStopwatch() - timestamp) > timeMax)
					eligible = false;
			}
		}
	}
}
