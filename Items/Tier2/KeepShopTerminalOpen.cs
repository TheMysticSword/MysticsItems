using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Collections.Generic;
using R2API.Networking.Interfaces;
using R2API.Networking;

namespace MysticsItems.Items
{
    public class KeepShopTerminalOpen : BaseItem
    {
        public override void PreLoad()
        {
            itemDef.name = "KeepShopTerminalOpen";
            itemDef.tier = ItemTier.Tier2;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist
            };
            SetUnlockable();
        }

        public override void OnLoad()
        {
            On.RoR2.MultiShopController.Awake += (orig, self) =>
            {
                orig(self);
                self.gameObject.AddComponent<MysticsItemsKeepShopTerminalOpenBehaviour>();
            };

            On.RoR2.Stage.BeginServer += (orig, self) =>
            {
                orig(self);
                foreach (CharacterBody characterBody in CharacterBody.readOnlyInstancesList)
                {
                    Inventory inventory = characterBody.inventory;
                    if (inventory)
                    {
                        int count = inventory.GetItemCount(MysticsItemsContent.Items.KeepShopTerminalOpenConsumed);
                        inventory.ResetItem(MysticsItemsContent.Items.KeepShopTerminalOpenConsumed);
                        inventory.GiveItem(MysticsItemsContent.Items.KeepShopTerminalOpen, count);
                    }
                }
            };

            NetworkingAPI.RegisterMessageType<MysticsItemsKeepShopTerminalOpenBehaviour.SyncSetTerminalState>();
        }
    }

    public class MysticsItemsKeepShopTerminalOpenBehaviour : MonoBehaviour
    {
        public List<PickupIndex> terminalPickups = new List<PickupIndex>();
        public List<GameObject> terminals = new List<GameObject>();
        public MultiShopController multiShopController;

        public void Start()
        {
            multiShopController = GetComponent<MultiShopController>();

            if (NetworkServer.active)
            {
                for (var i = 0; i < multiShopController.terminalGameObjects.Length; i++)
                {
                    GameObject terminal = multiShopController.terminalGameObjects[i];
                    terminals.Add(terminal);
                    SetTerminalState(terminal, false);
                    terminal.GetComponent<PurchaseInteraction>().onPurchase.AddListener((interactor) => {
                        terminalPickups[terminals.IndexOf(terminal)] = PickupIndex.none;
                        CharacterBody characterBody = interactor.GetComponent<CharacterBody>();
                        if (characterBody)
                        {
                            Inventory inventory = characterBody.inventory;
                            if (inventory)
                            {
                                int itemCount = inventory.GetItemCount(MysticsItemsContent.Items.KeepShopTerminalOpen);
                                if (itemCount > 0 && Reopen())
                                {
                                    inventory.RemoveItem(MysticsItemsContent.Items.KeepShopTerminalOpen);
                                    inventory.GiveItem(MysticsItemsContent.Items.KeepShopTerminalOpenConsumed);
                                }
                            }
                        }    
                    });
                }
            }
        }

        public bool Reopen()
        {
            if (NetworkServer.active)
            {
                List<PickupIndex> availablePickups = terminalPickups.FindAll(x => x != PickupIndex.none);
                if (availablePickups.Count > 0)
                {
                    multiShopController.Networkavailable = true;
                    for (var i = 0; i < multiShopController.terminalGameObjects.Length; i++)
                    {
                        if (terminalPickups[i] != PickupIndex.none)
                        {
                            GameObject gameObject = multiShopController.terminalGameObjects[i];
                            gameObject.GetComponent<PurchaseInteraction>().Networkavailable = true;
                            gameObject.GetComponent<ShopTerminalBehavior>().SetPickupIndex(terminalPickups[i]);
                            gameObject.GetComponent<ShopTerminalBehavior>().SetHasBeenPurchased(false);
                            SetTerminalState(gameObject, true);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public static void SetTerminalState(GameObject gameObject, bool open)
        {
            if (open)
            {
                int layerIndex = gameObject.GetComponent<ShopTerminalBehavior>().animator.GetLayerIndex("Body");
                gameObject.GetComponent<ShopTerminalBehavior>().animator.PlayInFixedTime("Open", layerIndex);
                gameObject.GetComponent<ShopTerminalBehavior>().animator.speed = 0f;
            }
            else
            {
                gameObject.GetComponent<ShopTerminalBehavior>().animator.speed = 1f;
            }
        }

        public class SyncSetTerminalState : INetMessage
        {
            NetworkInstanceId objID;
            bool open;

            public SyncSetTerminalState()
            {
            }

            public SyncSetTerminalState(NetworkInstanceId objID, bool open)
            {
                this.objID = objID;
                this.open = open;
            }

            public void Deserialize(NetworkReader reader)
            {
                objID = reader.ReadNetworkId();
                open = reader.ReadBoolean();
            }

            public void OnReceived()
            {
                if (NetworkServer.active) return;
                GameObject obj = Util.FindNetworkObject(objID);
                if (obj)
                {
                    SetTerminalState(obj, open);
                }
            }

            public void Serialize(NetworkWriter writer)
            {
                writer.Write(objID);
                writer.Write(open);
            }
        }
    }
}
