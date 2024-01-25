using System.Linq;
using TMPro;
using UnityEngine;

namespace MysticsItems
{
    public static class TMProEffects
    {
        public static void Init()
        {
            On.RoR2.UI.ChatBox.Start += ChatBox_Start;
            On.RoR2.UI.HGTextMeshProUGUI.Awake += HGTextMeshProUGUI_Awake;
        }

        private static void ChatBox_Start(On.RoR2.UI.ChatBox.orig_Start orig, RoR2.UI.ChatBox self)
        {
            orig(self);
            var component = self.messagesText.textComponent.GetComponent<MysticsItemsTextEffects>();
            if (!component)
            {
                component = self.messagesText.textComponent.gameObject.AddComponent<MysticsItemsTextEffects>();
            }
        }

        private static void HGTextMeshProUGUI_Awake(On.RoR2.UI.HGTextMeshProUGUI.orig_Awake orig, RoR2.UI.HGTextMeshProUGUI self)
        {
            orig(self);
            var component = self.GetComponent<MysticsItemsTextEffects>();
            if (!component)
            {
                component = self.gameObject.AddComponent<MysticsItemsTextEffects>();
                component.textComponent = self;
            }
        }

        public class MysticsItemsTextEffects : MonoBehaviour
        {
            public TMP_Text textComponent;
            public bool textChanged;
            public TMP_MeshInfo[] cachedMeshInfo;

            public float updateTimer = 0f;
            public float updateFrequency = 0.016f;

            public float currentTime = 0f;
            
            public void Awake()
            {
                textComponent = GetComponent<TMP_Text>();
                textChanged = true;
            }

            public void Start()
            {
                if (textComponent && textComponent.isActiveAndEnabled)
                {
                    textComponent.ForceMeshUpdate();
                }
            }

            public void OnEnable()
            {
                TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
            }

            public void OnDisable()
            {
                TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
            }

            public void ON_TEXT_CHANGED(Object obj)
            {
                if (obj == textComponent)
                    textChanged = true;
            }

            public void Update()
            {
                currentTime += Time.deltaTime;

                updateTimer -= Time.deltaTime;
                while (updateTimer <= 0f)
                {
                    updateTimer += updateFrequency;
                    if (textComponent && textComponent.isActiveAndEnabled)
                    {
                        var textInfo = textComponent.textInfo;

                        if (textInfo == null || textInfo.meshInfo == null || textInfo.meshInfo.Length <= 0 || textInfo.meshInfo[0].vertices == null) return;

                        if (textChanged)
                        {
                            textChanged = false;
                            cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
                            currentTime = 0f;
                        }

                        var anythingChanged = false;

                        var rng = new Xoroshiro128Plus(4923495);

                        float easeInOutBack(float x)
                        {
                            var c1 = 1.70158f;
                            var c2 = c1 * 1.525f;

                            return x < 0.5f
                              ? (Mathf.Pow(2f * x, 2f) * ((c2 + 1f) * 2f * x - c2)) / 2f
                              : (Mathf.Pow(2f * x - 2f, 2f) * ((c2 + 1f) * (x * 2f - 2f) + c2) + 2f) / 2f;
                        }
                        var aprilFoolsOffset = easeInOutBack(Mathf.Clamp01(currentTime));

                        for (var linkIndex = 0; linkIndex < textInfo.linkCount; linkIndex++)
                        {
                            var link = textInfo.linkInfo[linkIndex];
                            if (link.GetLinkID() == "MysticsItemsAprilFools")
                            {
                                for (int i = link.linkTextfirstCharacterIndex; i < link.linkTextfirstCharacterIndex + link.linkTextLength; i++)
                                {
                                    var charInfo = textInfo.characterInfo[i];
                                    if (!charInfo.isVisible) continue;

                                    anythingChanged = true;

                                    var origVerts = cachedMeshInfo[charInfo.materialReferenceIndex].vertices;
                                    var origColors = cachedMeshInfo[charInfo.materialReferenceIndex].colors32;
                                    var destVerts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                                    var destColors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
                                    var charOffset = new Vector3(rng.RangeFloat(-8f, 8f), rng.RangeFloat(-8f, 8f), 0f) * aprilFoolsOffset * charInfo.scale;
                                    for (var j = 0; j <= 3; j++)
                                    {
                                        destVerts[charInfo.vertexIndex + j] = origVerts[charInfo.vertexIndex + j] + charOffset;
                                        destColors[charInfo.vertexIndex + j] = Color.Lerp(origColors[charInfo.vertexIndex + j], Color.HSVToRGB((Time.time * 0.15f + 0.06f * i) % 1f, 1f, 1f), 0.2f * aprilFoolsOffset);
                                    }
                                }
                            }
                        }

                        if (anythingChanged)
                        {
                            for (var i = 0; i < textInfo.meshInfo.Length; i++)
                            {
                                var meshInfo = textInfo.meshInfo[i];
                                meshInfo.mesh.vertices = meshInfo.vertices;
                                meshInfo.mesh.colors32 = meshInfo.colors32;
                                textComponent.UpdateGeometry(meshInfo.mesh, i);
                                textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                            }
                        }
                    }
                }
            }
        }
    }
}