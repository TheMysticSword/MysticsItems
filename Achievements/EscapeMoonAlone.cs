using MysticsRisky2Utils.BaseAssetTypes;
using RoR2;
using UnityEngine;

namespace MysticsItems.Achievements
{
    public class EscapeMoonAlone : BaseAchievement
    {
        public override void OnLoad()
        {
            name = "MysticsItems_EscapeMoonAlone";
            unlockableName = "Items.MysticsItems_AllyDeathRevenge";
			iconSprite = MysticsRisky2Utils.Utils.AddItemIconBackgroundToSprite(Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Ally Death Revenge/Icon.png"), MysticsRisky2Utils.Utils.ItemIconBackgroundType.Tier2);
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
				public BodyIndex requiredBodyIndex;

				public override void OnInstall()
				{
					base.OnInstall();
                    GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
					requiredBodyIndex = BodyCatalog.FindBodyIndex("BrotherHurtBody");
				}


				public override void OnUninstall()
				{
					base.OnUninstall();
					GlobalEventManager.onCharacterDeathGlobal -= GlobalEventManager_onCharacterDeathGlobal;
				}

				private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
				{
					if (damageReport.victimBodyIndex != requiredBodyIndex) return;
					foreach (var teamMember in TeamComponent.GetTeamMembers(serverAchievementTracker.networkUser.master.teamIndex))
					{
						if (teamMember.body != serverAchievementTracker.networkUser.GetCurrentBody() && teamMember.body.healthComponent && teamMember.body.healthComponent.alive)
						{
							return;
						}
					}
					Grant();
				}
			}
		}
    }
}
