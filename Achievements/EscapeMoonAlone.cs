using RoR2;
using UnityEngine;

namespace MysticsItems.Achievements
{
    public class EscapeMoonAlone : BaseAchievement
    {
        public override void PreAdd()
        {
            name = "EscapeMoonAlone";
            unlockableName = Main.TokenPrefix + "Items.AllyDeathRevenge";
			trackerType = typeof(Tracker);
        }

        public class Tracker : RoR2.Achievements.BaseAchievement
		{
			public override void OnInstall()
			{
				base.OnInstall();
				Run.onClientGameOverGlobal += OnClientGameOverGlobal;
			}

			public override void OnUninstall()
			{
				base.OnUninstall();
				Run.onClientGameOverGlobal -= OnClientGameOverGlobal;
			}

			public void OnClientGameOverGlobal(Run run, RunReport runReport)
			{
				if (runReport.gameEnding && runReport.gameEnding == RoR2Content.GameEndings.mainEnding && run.livingPlayerCount == 1)
				{
					Grant();
				}
			}
		}
    }
}
