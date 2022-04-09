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
    public static class NetworkPickupDiscovery
    {
        internal static void Init()
        {
            NetworkingAPI.RegisterMessageType<SyncDiscoverPickup>();
        }

        public static void DiscoverPickup(CharacterMaster master, PickupIndex pickupIndex)
        {
            if (NetworkServer.active)
            {
                new SyncDiscoverPickup(
                    master.GetComponent<NetworkIdentity>().netId,
                    pickupIndex.value
                ).Send(NetworkDestination.Clients);
            }
        }

        private class SyncDiscoverPickup : INetMessage
        {
            NetworkInstanceId objID;
            int pickupIndex;

            public SyncDiscoverPickup()
            {
            }

            public SyncDiscoverPickup(NetworkInstanceId objID, int pickupIndex)
            {
                this.objID = objID;
                this.pickupIndex = pickupIndex;
            }

            public void Deserialize(NetworkReader reader)
            {
                objID = reader.ReadNetworkId();
                pickupIndex = reader.ReadInt32();
            }

            public void OnReceived()
            {
                GameObject obj = Util.FindNetworkObject(objID);
                if (obj)
                {
                    var master = obj.GetComponent<CharacterMaster>();
                    if (master)
                    {
                        PlayerCharacterMasterController pcmc = master.GetComponent<PlayerCharacterMasterController>();
                        if (pcmc)
                        {
                            NetworkUser networkUser = pcmc.networkUser;
                            if (networkUser)
                            {
                                LocalUser localUser = networkUser.localUser;
                                if (localUser != null)
                                {
                                    localUser.userProfile.DiscoverPickup(new PickupIndex(pickupIndex));
                                }
                            }
                        }
                    }
                }
            }

            public void Serialize(NetworkWriter writer)
            {
                writer.Write(objID);
                writer.Write(pickupIndex);
            }
        }
    }
}