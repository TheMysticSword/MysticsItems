using RoR2;
using RoR2.Achievements;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

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
			public bool eligible = true;

			public static List<string> finalStages = new List<string>()
			{
				"moon2", "moon"
			};

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
				if (eligible && obj.sceneDef && finalStages.Contains(obj.sceneDef.baseSceneName))
				{
					Grant();
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
