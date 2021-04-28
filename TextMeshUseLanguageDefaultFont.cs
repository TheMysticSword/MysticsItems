using TMPro;
using UnityEngine;
using RoR2.UI;

namespace MysticsItems
{
    public class TextMeshUseLanguageDefaultFont : MonoBehaviour
    {
        public void Awake()
        {
            TextMeshProUGUI component = GetComponent<TextMeshProUGUI>();
            if (component)
            {
                component.font = HGTextMeshProUGUI.defaultLanguageFont;
                component.UpdateFontAsset();
            }
        }
    }
}
