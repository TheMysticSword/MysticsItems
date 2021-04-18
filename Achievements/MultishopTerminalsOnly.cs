using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace MysticsItems.Achievements
{
    public class MultishopTerminalsOnly : BaseAchievement
    {
        public override void OnLoad()
        {
            name = "MultishopTerminalsOnly";
            unlockableName = Main.TokenPrefix + "Items.KeepShopTerminalOpen";
			trackerType = typeof(Tracker);
			serverTrackerType = typeof(Tracker.Server);
			allowLoad = false;
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
				public int requirement = 5;
				public int progress = 0;

				public override void OnInstall()
				{
					base.OnInstall();
                    On.RoR2.PurchaseInteraction.Awake += PurchaseInteraction_Awake;
                    SceneExitController.onBeginExit += SceneExitController_onBeginExit;
				}

                public override void OnUninstall()
				{
					base.OnUninstall();
					On.RoR2.PurchaseInteraction.Awake -= PurchaseInteraction_Awake;
					SceneExitController.onBeginExit -= SceneExitController_onBeginExit;
				}

                private void PurchaseInteraction_Awake(On.RoR2.PurchaseInteraction.orig_Awake orig, PurchaseInteraction self)
                {
					orig(self);
					self.onPurchase.AddListener((interactor) =>
					{
						if (interactor.GetComponent<CharacterBody>() == GetCurrentBody() && !self.GetComponent<ShopTerminalBehavior>()) progress = 0;
					});
                }

				private void SceneExitController_onBeginExit(SceneExitController obj)
				{
					if (Run.instance && networkUser != null)
					{
						if (SceneInfo.instance.countsAsStage) progress++;
						if (progress >= requirement)
						{
							CharacterBody currentBody = serverAchievementTracker.networkUser.GetCurrentBody();
							if (currentBody)
							{
								Grant();
							}
						}
					}
				}
			}
		}
    }
}
