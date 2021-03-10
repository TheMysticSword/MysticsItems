using UnityEngine;
using TMPro;
using R2API;
using RoR2;

namespace MysticsItems
{
    public static class PlainHologram
    {
        public static GameObject prefab;

        public static void Init()
        {
            prefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CostHologramContent"), Main.TokenPrefix + "TreasureMapHologramContent", false);
            CostHologramContent costHologramContent = prefab.GetComponent<CostHologramContent>();
            MysticsItemsPlainHologramContent plainHologramContent = prefab.AddComponent<MysticsItemsPlainHologramContent>();
            plainHologramContent.targetTextMesh = costHologramContent.targetTextMesh;
            Object.Destroy(costHologramContent);
        }

        public class MysticsItemsPlainHologramContent : MonoBehaviour
        {
            public void FixedUpdate()
            {
                if (targetTextMesh)
                {
                    targetTextMesh.SetText(text);
                    targetTextMesh.color = color;
                }
            }

            public string text;
            public Color color = Color.white;
            public TextMeshPro targetTextMesh;
        }
    }
}