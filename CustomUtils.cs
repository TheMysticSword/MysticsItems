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

        public static void CopyChildren(GameObject from, GameObject to, bool cloneFromThenDestroy = true)
        {
            string trueName = to.name;
            if (cloneFromThenDestroy) from = PrefabAPI.InstantiateClone(from, from.name + "Copy", false);

            Transform parent = to.transform.parent;

            int childCount = from.transform.childCount;
            for (var i = 0; i < childCount; i++)
            {
                from.transform.GetChild(0).SetParent(to.transform);
            }
            foreach (Component fromComponent in from.GetComponents<Component>())
            {
                System.Type componentType = fromComponent.GetType();

                Component toComponent = to.GetComponent(componentType);
                if (!toComponent) toComponent = to.AddComponent(componentType);

                bool isAnimator = typeof(Animator).IsAssignableFrom(fromComponent.GetType());
                bool animatorLogWarnings = false;

                if (isAnimator)
                {
                    Animator fromAnimator = (Animator)fromComponent;
                    Animator toAnimator = (Animator)toComponent;
                    animatorLogWarnings = fromAnimator.logWarnings;
                    fromAnimator.logWarnings = false;
                    toAnimator.logWarnings = false;
                }

                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default;
                foreach (PropertyInfo propertyInfo in componentType.GetProperties(flags))
                {
                    if (propertyInfo.CanWrite)
                    {
                        try
                        {
                            propertyInfo.SetValue(toComponent, propertyInfo.GetValue(fromComponent));
                        }
                        catch { }
                    }
                }
                foreach (FieldInfo fieldInfo in componentType.GetFields(flags))
                {
                    fieldInfo.SetValue(toComponent, fieldInfo.GetValue(fromComponent));
                }

                if (isAnimator)
                {
                    Animator fromAnimator = (Animator)fromComponent;
                    Animator toAnimator = (Animator)toComponent;
                    fromAnimator.logWarnings = animatorLogWarnings;
                    toAnimator.logWarnings = animatorLogWarnings;
                }
            }

            to.transform.SetParent(parent);
            to.name = trueName;

            if (cloneFromThenDestroy) Object.Destroy(from);
        }

        public static string TrimCloneFromString(string originalString)
        {
            if (originalString.EndsWith("(Clone)")) originalString = originalString.Remove(originalString.Length - "(Clone)".Length);
            return originalString;
        }
    }
}