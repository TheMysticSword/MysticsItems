using MysticsRisky2Utils.BaseAssetTypes;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace MysticsItems.Achievements
{
    public class MultishopTerminalsOnly : BaseAchievement
    {
        public override void OnLoad()
        {
            name = "MysticsItems_MultishopTerminalsOnly";
            unlockableName = "Items.MysticsItems_KeepShopTerminalOpen";
			iconSprite = MysticsRisky2Utils.Utils.AddItemIconBackgroundToSprite(Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Platinum Card/Icon.png"), MysticsRisky2Utils.Utils.ItemIconBackgroundType.Tier2);
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
				public bool eligible = true;
				public int stageRequirement = 6;
				
				public override void OnInstall()
				{
					base.OnInstall();
                    GlobalEventManager.OnInteractionsGlobal += GlobalEventManager_OnInteractionsGlobal;
                    Stage.onServerStageBegin += Stage_onServerStageBegin;
				}

                private void Stage_onServerStageBegin(Stage obj)
                {
                    if (eligible && !RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.Sacrifice) && (Run.instance.stageClearCount + 1) >= stageRequirement)
                    {
						Grant();
                    }
                }

                private void GlobalEventManager_OnInteractionsGlobal(Interactor interactor, IInteractable interactable, GameObject interactableObject)
				{
					if (!eligible) return;
					if (IsCurrentBody(interactor.gameObject))
					{
						PurchaseInteraction purchaseInteraction = interactableObject.GetComponent<PurchaseInteraction>();
						if (purchaseInteraction && purchaseInteraction.costType == CostTypeIndex.Money && purchaseInteraction.cost > 0)
                        {
							if (!purchaseInteraction.displayNameToken.Contains("MULTISHOP_TERMINAL_"))
								eligible = false;
						}
					}
				}

                public override void OnUninstall()
				{
					base.OnUninstall();
					GlobalEventManager.OnInteractionsGlobal -= GlobalEventManager_OnInteractionsGlobal;
					Stage.onServerStageBegin -= Stage_onServerStageBegin;
				}
			}
		}
    }
}
