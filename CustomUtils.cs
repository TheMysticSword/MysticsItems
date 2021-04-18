using UnityEngine;
using UnityEngine.Networking;
using System.Reflection;
using R2API;

namespace MysticsItems
{
    public static class CustomUtils
    {
        public static GameObject inactivePrefabParent;

        public static GameObject CreateBlankPrefab(string name = "GameObject")
        {
            if (!inactivePrefabParent)
            {
                inactivePrefabParent = new GameObject("InactivePrefabParent");
                Object.DontDestroyOnLoad(inactivePrefabParent);
                inactivePrefabParent.SetActive(false);
            }
            return Object.Instantiate(new GameObject(name), inactivePrefabParent.transform);
        }

        public static int netIDCount = int.MinValue;
        public static NetworkIdentity GrabNetID(string name = "MysticsItemsNetworkIdentity")
        {
            GameObject obj = CreateBlankPrefab(name + netIDCount.ToString());
            NetworkIdentity networkIdentity = obj.AddComponent<NetworkIdentity>();
            PrefabAPI.RegisterNetworkPrefab(obj);
            netIDCount++;
            return networkIdentity;
        }

        public static void ReleaseNetID(GameObject gameObject, NetworkIdentity netID)
        {
            NetworkIdentity networkIdentity = gameObject.GetComponent<NetworkIdentity>();
            if (!networkIdentity) networkIdentity = gameObject.AddComponent<NetworkIdentity>();
            foreach (PropertyInfo propertyInfo in typeof(NetworkIdentity).GetProperties(Main.bindingFlagAll))
            {
                if (propertyInfo.CanRead && propertyInfo.CanWrite)
                {
                    try
                    {
                        propertyInfo.SetValue(networkIdentity, propertyInfo.GetValue(netID));
                    }
                    catch { }
                }
            }
            foreach (FieldInfo fieldInfo in typeof(NetworkIdentity).GetFields(Main.bindingFlagAll))
            {
                try
                {
                    fieldInfo.SetValue(networkIdentity, fieldInfo.GetValue(netID));
                }
                catch { }
            }
            Object.Destroy(netID);
        }
    }
}