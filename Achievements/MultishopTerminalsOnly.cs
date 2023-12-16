using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace MysticsItems.Achievements
{
    public class MultishopTerminalsOnly
    {
		[RegisterAchievement("MysticsItems_MultishopTerminalsOnly", "Items.MysticsItems_KeepShopTerminalOpen", null, typeof(Server))]
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
				public bool eligible = true;
				public int stageRequirement = 6;
				
				public override void OnInstall()
				{
					base.OnInstall();
                    Run.onRunStartGlobal += Run_onRunStartGlobal;
                    GlobalEventManager.OnInteractionsGlobal += GlobalEventManager_OnInteractionsGlobal;
                    Stage.onServerStageBegin += Stage_onServerStageBegin;
				}

                private void Run_onRunStartGlobal(Run obj)
                {
					eligible = true;
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
					Stage.onServerStageBegin -= Stage_onServerStageBegin;
				}
			}
		}
    }
}
