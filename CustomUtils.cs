using UnityEngine;
using UnityEngine.Networking;
using System.Reflection;
using R2API;

namespace MysticsItems
{
    public static class CustomUtils
    {
        public static GameObject CreateBlankPrefab(string name = "GameObject", bool network = false)
        {
            GameObject gameObject = PrefabAPI.InstantiateClone(new GameObject(name), name, false);
            if (network)
            {
                gameObject.AddComponent<NetworkIdentity>();
                PrefabAPI.RegisterNetworkPrefab(gameObject);
            }
            return gameObject;
        }

        public static void CopyChildren(GameObject from, GameObject to)
        {
            int childCount = from.transform.childCount;
            for (var i = 0; i < childCount; i++)
            {
                from.transform.GetChild(0).SetParent(to.transform);
            }
        }
    }
}