using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace MysticsItems.Achievements
{
	[RegisterAchievement("MysticsItems_HackLegendaryChest", "Items.MysticsItems_TreasureMap", null, typeof(Server))]
	public class HackLegendaryChest : BaseAchievement
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
			public override void OnInstall()
			{
				base.OnInstall();
				GlobalEventManager.OnInteractionsGlobal += GlobalEventManager_OnInteractionsGlobal;
			}

			private void GlobalEventManager_OnInteractionsGlobal(Interactor interactor, IInteractable interactable, GameObject interactableObject)
			{
				if (IsCurrentBody(interactor.gameObject))
				{
					PurchaseInteraction purchaseInteraction = interactableObject.GetComponent<PurchaseInteraction>();
					if (purchaseInteraction && purchaseInteraction.costType == CostTypeIndex.Money && purchaseInteraction.cost <= 0)
					{
						if (purchaseInteraction.displayNameToken == "GOLDCHEST_NAME")
							Grant();
					}
				}
			}

			public override void OnUninstall()
			{
				base.OnUninstall();
				GlobalEventManager.OnInteractionsGlobal -= GlobalEventManager_OnInteractionsGlobal;
			}
		}
	}
}
