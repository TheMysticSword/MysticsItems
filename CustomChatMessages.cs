using R2API.Networking.Interfaces;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using R2API.Networking;
using System.Collections.Generic;

namespace MysticsItems
{
    public static class CustomChatMessages
    {
        internal static void Init()
        {
            NetworkingAPI.RegisterMessageType<SyncUpgradeMessage>();
        }

        public static void SendUpgradeMessage(CharacterMaster master, PickupIndex originalPickupIndex, PickupIndex upgradePickupIndex)
        {
            if (NetworkServer.active)
            {
                uint originalPickupQuantity = 1U;
                uint upgradePickupQuantity = 1U;
                if (master.inventory)
                {
                    var pickupDef = PickupCatalog.GetPickupDef(originalPickupIndex);
                    var itemIndex = (pickupDef != null) ? pickupDef.itemIndex : ItemIndex.None;
                    if (itemIndex != ItemIndex.None)
                        originalPickupQuantity = (uint)master.inventory.GetItemCount(itemIndex);

                    pickupDef = PickupCatalog.GetPickupDef(upgradePickupIndex);
                    itemIndex = (pickupDef != null) ? pickupDef.itemIndex : ItemIndex.None;
                    if (itemIndex != ItemIndex.None)
                        upgradePickupQuantity = (uint)master.inventory.GetItemCount(itemIndex);
                }
                if (originalPickupQuantity > 0U && upgradePickupQuantity > 0U)
                    new SyncUpgradeMessage(
                        master.GetComponent<NetworkIdentity>().netId,
                        originalPickupIndex.value,
                        originalPickupQuantity,
                        upgradePickupIndex.value,
                        upgradePickupQuantity
                    ).Send(NetworkDestination.Clients);
            }
        }

        private class SyncUpgradeMessage : INetMessage
        {
            NetworkInstanceId objID;
            int originalPickupIndex;
            uint originalPickupQuantity;
            int upgradePickupIndex;
            uint upgradePickupQuantity;

            public SyncUpgradeMessage()
            {
            }

            public SyncUpgradeMessage(NetworkInstanceId objID, int originalPickupIndex, uint originalPickupQuantity, int upgradePickupIndex, uint upgradePickupQuantity)
            {
                this.objID = objID;
                this.originalPickupIndex = originalPickupIndex;
                this.originalPickupQuantity = originalPickupQuantity;
                this.upgradePickupIndex = upgradePickupIndex;
                this.upgradePickupQuantity = upgradePickupQuantity;
            }

            public void Deserialize(NetworkReader reader)
            {
                objID = reader.ReadNetworkId();
                originalPickupIndex = reader.ReadInt32();
                originalPickupQuantity = reader.ReadUInt32();
                upgradePickupIndex = reader.ReadInt32();
                upgradePickupQuantity = reader.ReadUInt32();
            }

            public void OnReceived()
            {
                RoR2Application.fixedTimeTimers.CreateTimer(0.1f, () =>
                {
                    GameObject obj = Util.FindNetworkObject(objID);
                    if (obj)
                    {
                        var master = obj.GetComponent<CharacterMaster>();
                        if (!master) return;
                        var body = master.GetBody();

                        var originalPickupDef = PickupCatalog.GetPickupDef(new PickupIndex(originalPickupIndex));
                        var originalItemDef = ItemCatalog.GetItemDef((originalPickupDef != null) ? originalPickupDef.itemIndex : ItemIndex.None);
                        if (originalItemDef == null || originalItemDef.hidden) return;

                        var upgradePickupDef = PickupCatalog.GetPickupDef(new PickupIndex(upgradePickupIndex));
                        var upgradeItemDef = ItemCatalog.GetItemDef((upgradePickupDef != null) ? upgradePickupDef.itemIndex : ItemIndex.None);
                        if (upgradeItemDef == null || upgradeItemDef.hidden) return;

                        PlayerCharacterMasterController pcmc = master.GetComponent<PlayerCharacterMasterController>();
                        if (pcmc)
                        {
                            NetworkUser networkUser = pcmc.networkUser;
                            if (networkUser)
                            {
                                LocalUser localUser = networkUser.localUser;
                                if (localUser != null)
                                {
                                    localUser.userProfile.DiscoverPickup(new PickupIndex(upgradePickupIndex));
                                }
                            }
                        }

                        foreach (var notificationQueueHandler in NotificationQueue.readOnlyInstancesList)
                        {
                            if (originalItemDef && !originalItemDef.hidden)
                            {
                                if (notificationQueueHandler.hud.targetMaster == master)
                                {
                                    notificationQueueHandler.notificationQueue = new Queue<NotificationQueue.NotificationInfo>(notificationQueueHandler.notificationQueue.Where(x => x.data != (object)originalItemDef));
                                    if (notificationQueueHandler.currentNotification.titleText.token == originalItemDef.nameToken)
                                        notificationQueueHandler.currentNotification.age = notificationQueueHandler.currentNotification.duration;

                                    notificationQueueHandler.notificationQueue.Enqueue(new NotificationQueue.NotificationInfo
                                    {
                                        data = ItemCatalog.GetItemDef(upgradeItemDef.itemIndex)
                                    });
                                }
                            }
                        }

                        AddUpgradeMessage(
                            body,
                            ((originalPickupDef != null) ? originalPickupDef.nameToken : null) ?? PickupCatalog.invalidPickupToken,
                            (originalPickupDef != null) ? originalPickupDef.baseColor : Color.black,
                            originalPickupQuantity,
                            ((upgradePickupDef != null) ? upgradePickupDef.nameToken : null) ?? PickupCatalog.invalidPickupToken,
                            (upgradePickupDef != null) ? upgradePickupDef.baseColor : Color.black,
                            upgradePickupQuantity
                        );
                    }
                });
            }

            public void Serialize(NetworkWriter writer)
            {
                writer.Write(objID);
                writer.Write(originalPickupIndex);
                writer.Write(originalPickupQuantity);
                writer.Write(upgradePickupIndex);
                writer.Write(upgradePickupQuantity);
            }
        }

        public static void AddUpgradeMessage(CharacterBody body, string originalToken, Color32 originalColor, uint originalQuantity, string upgradeToken, Color32 upgradeColor, uint upgradeQuantity)
        {
            Chat.AddMessage(new PlayerUpgradeChatMessage
            {
                subjectAsCharacterBody = body,
                baseToken = "MYSTICSITEMS_PLAYER_UPGRADE",
                originalToken = originalToken,
                originalColor = originalColor,
                originalQuantity = originalQuantity,
                upgradeToken = upgradeToken,
                upgradeColor = upgradeColor,
                upgradeQuantity = upgradeQuantity
            });
        }

        public class PlayerUpgradeChatMessage : SubjectChatMessage
        {
            public override string ConstructChatString()
            {
                string subjectName = GetSubjectName();
                string constructedChatString = Language.GetString(GetResolvedToken());

                string originalName = Language.GetString(originalToken) ?? "???";
                originalName = Util.GenerateColoredString(originalName, originalColor);
                string originalQuantityString = "";
                if (originalQuantity != 1U)
                {
                    originalQuantityString = "(" + originalQuantity + ")";
                }

                string upgradeName = Language.GetString(upgradeToken) ?? "???";
                upgradeName = Util.GenerateColoredString(upgradeName, upgradeColor);
                string upgradeQuantityString = "";
                if (upgradeQuantity != 1U)
                {
                    upgradeQuantityString = "(" + upgradeQuantity + ")";
                }

                return string.Format(constructedChatString, subjectName, originalName, originalQuantityString, upgradeName, upgradeQuantityString);
            }

            public string originalToken;
            public Color32 originalColor;
            public uint originalQuantity;

            public string upgradeToken;
            public Color32 upgradeColor;
            public uint upgradeQuantity;
        }
    }
}