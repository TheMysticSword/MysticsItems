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
            NetworkingAPI.RegisterMessageType<SyncConversionMessage>();
        }

        public static void SendConversionMessage(CharacterMaster master, PickupIndex originalPickupIndex, uint originalPickupQuantity, PickupIndex convertedPickupIndex, uint convertedPickupQuantity)
        {
            if (NetworkServer.active)
            {
                if (originalPickupQuantity > 0U && convertedPickupQuantity > 0U)
                    new SyncConversionMessage(
                        master.GetComponent<NetworkIdentity>().netId,
                        originalPickupIndex.value,
                        originalPickupQuantity,
                        convertedPickupIndex.value,
                        convertedPickupQuantity
                    ).Send(NetworkDestination.Clients);
            }
        }

        private class SyncConversionMessage : INetMessage
        {
            NetworkInstanceId objID;
            int originalPickupIndex;
            uint originalPickupQuantity;
            int convertedPickupIndex;
            uint convertedPickupQuantity;

            public SyncConversionMessage()
            {
            }

            public SyncConversionMessage(NetworkInstanceId objID, int originalPickupIndex, uint originalPickupQuantity, int convertedPickupIndex, uint convertedPickupQuantity)
            {
                this.objID = objID;
                this.originalPickupIndex = originalPickupIndex;
                this.originalPickupQuantity = originalPickupQuantity;
                this.convertedPickupIndex = convertedPickupIndex;
                this.convertedPickupQuantity = convertedPickupQuantity;
            }

            public void Deserialize(NetworkReader reader)
            {
                objID = reader.ReadNetworkId();
                originalPickupIndex = reader.ReadInt32();
                originalPickupQuantity = reader.ReadUInt32();
                convertedPickupIndex = reader.ReadInt32();
                convertedPickupQuantity = reader.ReadUInt32();
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

                        var convertedPickupDef = PickupCatalog.GetPickupDef(new PickupIndex(convertedPickupIndex));
                        var convertedItemDef = ItemCatalog.GetItemDef((convertedPickupDef != null) ? convertedPickupDef.itemIndex : ItemIndex.None);
                        if (convertedItemDef == null || convertedItemDef.hidden) return;

                        PlayerCharacterMasterController pcmc = master.GetComponent<PlayerCharacterMasterController>();
                        if (pcmc)
                        {
                            NetworkUser networkUser = pcmc.networkUser;
                            if (networkUser)
                            {
                                LocalUser localUser = networkUser.localUser;
                                if (localUser != null)
                                {
                                    localUser.userProfile.DiscoverPickup(new PickupIndex(convertedPickupIndex));
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
                                        data = ItemCatalog.GetItemDef(convertedItemDef.itemIndex)
                                    });
                                }
                            }
                        }

                        AddConversionMessage(
                            body,
                            ((originalPickupDef != null) ? originalPickupDef.nameToken : null) ?? PickupCatalog.invalidPickupToken,
                            (originalPickupDef != null) ? originalPickupDef.baseColor : Color.black,
                            originalPickupQuantity,
                            ((convertedPickupDef != null) ? convertedPickupDef.nameToken : null) ?? PickupCatalog.invalidPickupToken,
                            (convertedPickupDef != null) ? convertedPickupDef.baseColor : Color.black,
                            convertedPickupQuantity
                        );
                    }
                });
            }

            public void Serialize(NetworkWriter writer)
            {
                writer.Write(objID);
                writer.Write(originalPickupIndex);
                writer.Write(originalPickupQuantity);
                writer.Write(convertedPickupIndex);
                writer.Write(convertedPickupQuantity);
            }
        }

        public static void AddConversionMessage(CharacterBody body, string originalToken, Color32 originalColor, uint originalQuantity, string convertedToken, Color32 convertedColor, uint convertedQuantity)
        {
            Chat.AddMessage(new PlayerConversionChatMessage
            {
                subjectAsCharacterBody = body,
                baseToken = "MYSTICSITEMS_PLAYER_CONVERT",
                originalToken = originalToken,
                originalColor = originalColor,
                originalQuantity = originalQuantity,
                convertedToken = convertedToken,
                convertedColor = convertedColor,
                convertedQuantity = convertedQuantity
            });
        }

        public class PlayerConversionChatMessage : SubjectChatMessage
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

                string convertedName = Language.GetString(convertedToken) ?? "???";
                convertedName = Util.GenerateColoredString(convertedName, convertedColor);
                string convertedQuantityString = "";
                if (convertedQuantity != 1U)
                {
                    convertedQuantityString = "(" + convertedQuantity + ")";
                }

                return string.Format(constructedChatString, subjectName, originalName, originalQuantityString, convertedName, convertedQuantityString);
            }

            public string originalToken;
            public Color32 originalColor;
            public uint originalQuantity;

            public string convertedToken;
            public Color32 convertedColor;
            public uint convertedQuantity;
        }
    }
}