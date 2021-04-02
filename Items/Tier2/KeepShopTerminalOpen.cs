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
        public override void OnLoad()
        {;
            itemDef.name = "KeepShopTerminalOpen";
            itemDef.tier = ItemTier.Tier2;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist
            };
            //SetUnlockable();

            On.RoR2.MultiShopController.Awake += (orig, self) =>
            {
                orig(self);
                self.gameObject.AddComponent<MysticsItemsKeepShopTerminalOpenBehaviour>();
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
                            CharacterMaster characterMaster = characterBody.master;
                            if (inventory && characterMaster)
                            {
                                int itemCount = inventory.GetItemCount(MysticsItemsContent.Items.KeepShopTerminalOpen);
                                if (itemCount > 0 && Util.CheckRoll((1f - 1f / (1f + 0.25f * (itemCount - 1))) * 100f, characterMaster))
                                {
                                    Reopen();
                                }
                            }
                        }    
                    });
                }
            }
        }

        public void Reopen()
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
                }
            }
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
