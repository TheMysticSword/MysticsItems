using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace MysticsItems
{
    internal static class FunEvents
    {
        private const string checkString = "mysticsitems_funevent";

        public static void Init()
        {
            if (ContentToggleConfigManager.funEvents.Value)
            {
                var today = System.DateTime.Now;
                if (today.Month == 4 && today.Day == 1)
                {
                    BazaarPrank.Init();
                }

                On.RoR2.NetworkPlayerName.GetResolvedName += NetworkPlayerName_GetResolvedName;
            }
        }

        private static string NetworkPlayerName_GetResolvedName(On.RoR2.NetworkPlayerName.orig_GetResolvedName orig, ref NetworkPlayerName self)
        {
            if (self.nameOverride == checkString) return "???";
            return orig(ref self);
        }

        public static class BazaarPrank
        {
            public static void Init()
            {
                RoR2Application.onLoad += BazaarPrank_OnGameLoad;
                RoR2Application.onFixedUpdate += RoR2Application_onFixedUpdate;
                On.RoR2.ShopTerminalBehavior.Start += ShopTerminalBehavior_Start;
                On.RoR2.ShopTerminalBehavior.SetPickupIndex += ShopTerminalBehavior_SetPickupIndex;
                PickupDropletController.onDropletHitGroundServer += PickupDropletController_onDropletHitGroundServer;
            }

            public static void Finish()
            {
                finished = true;
                RoR2Application.onLoad -= BazaarPrank_OnGameLoad;
                RoR2Application.onFixedUpdate -= RoR2Application_onFixedUpdate;
                On.RoR2.ShopTerminalBehavior.Start -= ShopTerminalBehavior_Start;
                On.RoR2.ShopTerminalBehavior.SetPickupIndex -= ShopTerminalBehavior_SetPickupIndex;
                PickupDropletController.onDropletHitGroundServer -= PickupDropletController_onDropletHitGroundServer;
            }

            public static PickupIndex gesturePickupIndex;
            public static PickupIndex tonicPickupIndex;
            public static PickupIndex bestItemPickupIndex;
            public static float finishTimer = -1f;
            public static bool finished = false;
            public static int lunarBudIndex = 0;
            
            private static void BazaarPrank_OnGameLoad()
            {
                gesturePickupIndex = PickupCatalog.FindPickupIndex(RoR2Content.Items.AutoCastEquipment.itemIndex);
                tonicPickupIndex = PickupCatalog.FindPickupIndex(RoR2Content.Equipment.Tonic.equipmentIndex);
                bestItemPickupIndex = PickupCatalog.FindPickupIndex(MysticsItemsContent.Equipment.MysticsItems_GateChalice.equipmentIndex);
            }

            private static void RoR2Application_onFixedUpdate()
            {
                if (finishTimer > 0f)
                {
                    finishTimer -= Time.fixedDeltaTime;
                    if (finishTimer <= 0f)
                    {
                        if (NetworkServer.active)
                        {
                            Chat.SendBroadcastChat(new Chat.PlayerChatMessage
                            {
                                networkPlayerName = new NetworkPlayerName
                                {
                                    nameOverride = checkString,
                                    steamId = default(CSteamID)
                                },
                                baseToken = "Happy April Fools' day!"
                            });
                        }
                        Finish();
                    }
                }
            }

            private static void PickupDropletController_onDropletHitGroundServer(ref GenericPickupController.CreatePickupInfo createPickupInfo, ref bool shouldSpawn)
            {
                if ((createPickupInfo.pickupIndex == gesturePickupIndex || createPickupInfo.pickupIndex == tonicPickupIndex) &&
                    Stage.instance.sceneDef && Stage.instance.sceneDef.baseSceneName == "bazaar")
                    createPickupInfo.pickupIndex = bestItemPickupIndex;
            }

            private static void ShopTerminalBehavior_Start(On.RoR2.ShopTerminalBehavior.orig_Start orig, ShopTerminalBehavior self)
            {
                orig(self);
                if (MysticsRisky2Utils.Utils.TrimCloneFromString(self.gameObject.name).Contains("LunarShopTerminal"))
                {
                    self.SetPickupIndex((lunarBudIndex % 5) == 0 ? tonicPickupIndex : gesturePickupIndex);
                    lunarBudIndex++;
                    var purchaseInteraction = self.GetComponent<PurchaseInteraction>();
                    if (purchaseInteraction)
                    {
                        purchaseInteraction.onPurchase.AddListener((interactor) =>
                        {
                            if (!finished)
                            {
                                if (interactor)
                                {
                                    var body = interactor.GetComponent<CharacterBody>();
                                    if (body && body.master && body.master.playerCharacterMasterController)
                                    {
                                        var pcmc = body.master.playerCharacterMasterController;
                                        if (pcmc.networkUser)
                                        {
                                            pcmc.networkUser.AwardLunarCoins((uint)purchaseInteraction.cost);
                                        }
                                    }
                                }
                                finishTimer = 5f;
                            }
                        });
                    }
                }
            }

            private static void ShopTerminalBehavior_SetPickupIndex(On.RoR2.ShopTerminalBehavior.orig_SetPickupIndex orig, ShopTerminalBehavior self, PickupIndex newPickupIndex, bool newHidden)
            {
                if (newPickupIndex != PickupIndex.none && MysticsRisky2Utils.Utils.TrimCloneFromString(self.gameObject.name).Contains("LunarShopTerminal"))
                {
                    if (newPickupIndex != tonicPickupIndex && newPickupIndex != gesturePickupIndex)
                    {
                        newPickupIndex = (lunarBudIndex % 5) == 0 ? tonicPickupIndex : gesturePickupIndex;
                        lunarBudIndex++;
                    }
                }
                orig(self, newPickupIndex, newHidden);
            }
        }
    }
}