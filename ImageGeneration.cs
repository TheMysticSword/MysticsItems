using R2API.Utils;
using RoR2;
using RoR2.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using R2API;
using UnityEngine.AddressableAssets;

namespace MysticsItems
{
    internal static class ImageGeneration
    {
        public static string path = System.IO.Path.Combine(Application.persistentDataPath, "MysticsItems", "ImageGeneration");
        public static HGTextMeshProUGUI notifTitleTmp;
        public static HGTextMeshProUGUI notifDescriptionTmp;

        internal static void Init()
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var notifPanel = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/NotificationPanel2.prefab").WaitForCompletion(), "MysticsRisky2Utils_NotificationPanel", false);
            notifTitleTmp = notifPanel.transform.Find("CanvasGroup/TextArea/Title").GetComponent<HGTextMeshProUGUI>();
            notifDescriptionTmp = notifPanel.transform.Find("CanvasGroup/TextArea/Description").GetComponent<HGTextMeshProUGUI>();
        }

        public static void GenerateItemTable(float padding, float columnWidth, float rowHeight, int columns, Language language, List<ItemTableSection> sections)
        {
            var relativeScaleX = columnWidth / 512f;
            var relativeScaleY = rowHeight / 107f;

            var currentColumn = 1;
            var currentRow = 1;

            var rows = 0;
            
            var nextSectionsRowOffset = 0;
            
            var visualSlots = new List<ItemTableVisualSlot>();
            
            foreach (var section in sections)
            {
                var sectionRows = (section.itemDefs != null ? section.itemDefs.Count : 0) + (section.equipmentDefs != null ? section.equipmentDefs.Count : 0);

                var currentRowDelta = 0;

                if (section.itemDefs != null)
                    foreach (var itemDef in section.itemDefs)
                    {
                        visualSlots.Add(new ItemTableVisualSlot
                        {
                            x = (currentColumn - 1) * columnWidth + padding,
                            row = currentRow,
                            icon = (Texture2D)itemDef.pickupIconTexture,
                            name = language.GetLocalizedStringByToken(itemDef.nameToken),
                            description = language.GetLocalizedStringByToken(itemDef.pickupToken),
                            color = section.color
                        });

                        currentRow++;
                        currentRowDelta++;
                    }
                if (section.equipmentDefs != null)
                    foreach (var equipmentDef in section.equipmentDefs)
                    {
                        visualSlots.Add(new ItemTableVisualSlot
                        {
                            x = (currentColumn - 1) * columnWidth + padding,
                            row = currentRow,
                            icon = (Texture2D)equipmentDef.pickupIconTexture,
                            name = language.GetLocalizedStringByToken(equipmentDef.nameToken),
                            description = language.GetLocalizedStringByToken(equipmentDef.pickupToken),
                            color = section.color
                        });

                        currentRow++;
                        currentRowDelta++;
                    }
                
                currentColumn++;
                currentRow -= currentRowDelta;
                nextSectionsRowOffset = Mathf.Max(nextSectionsRowOffset, sectionRows);
                
                if (currentColumn == columns + 1)
                {
                    currentColumn = 1;
                    currentRow += nextSectionsRowOffset;
                    rows += nextSectionsRowOffset;
                    nextSectionsRowOffset = 0;
                }
            }
            rows += nextSectionsRowOffset;

            Texture2D texture = new Texture2D(
                (int)(columns * columnWidth + padding * 2f),
                (int)(rows * rowHeight + padding * 2f),
                TextureFormat.ARGB32,
                false
            );

            for (var x = 0; x < texture.width; x++)
                for (var y = 0; y < texture.height; y++)
                    texture.SetPixel(x, y, new Color32(0, 0, 0, 30));

            for (var x = (int)padding; x < texture.width - padding; x++)
                for (var y = (int)padding; y < texture.height - padding; y++)
                    texture.SetPixel(x, y, new Color32(0, 0, 0, 80));

            foreach (var visualSlot in visualSlots)
            {
                visualSlot.y = -visualSlot.row * rowHeight - padding;
                
                RenderTexture renderTexture;

                // icon drawing
                Texture2D resizedIcon = new Texture2D((int)rowHeight, (int)rowHeight, TextureFormat.ARGB32, false);
                Graphics.ConvertTexture(visualSlot.icon, resizedIcon);
                renderTexture = RenderTexture.GetTemporary(resizedIcon.width, resizedIcon.height, 24, RenderTextureFormat.ARGB32);
                renderTexture.Create();
                RenderTexture.active = renderTexture;
                Graphics.Blit(resizedIcon, renderTexture);
                resizedIcon.ReadPixels(new Rect(0, 0, resizedIcon.width, resizedIcon.height), 0, 0);
                resizedIcon.Apply();
                RenderTexture.active = null;
                RenderTexture.ReleaseTemporary(renderTexture);

                for (var x = 0; x < resizedIcon.width; x++)
                    for (var y = 0; y < resizedIcon.height; y++)
                    {
                        Color backgroundPixel = texture.GetPixel(x + (int)visualSlot.x, y + (int)visualSlot.y);
                        Color layerPixel = resizedIcon.GetPixel(x, y);
                        texture.SetPixel(x + (int)visualSlot.x, y + (int)visualSlot.y, new Color(
                            backgroundPixel.r * (1 - layerPixel.a) + layerPixel.r * layerPixel.a,
                            backgroundPixel.g * (1 - layerPixel.a) + layerPixel.g * layerPixel.a,
                            backgroundPixel.b * (1 - layerPixel.a) + layerPixel.b * layerPixel.a,
                            Mathf.Clamp01(backgroundPixel.a + layerPixel.a)
                        ));
                    }

                // text drawing
                void DrawText(string text, Color color, float tx, float ty, HGTextMeshProUGUI notifTmp)
                {
                    Texture2D textMeshTexture = new Texture2D((int)columnWidth, (int)rowHeight, TextureFormat.ARGB32, false);

                    var tmpObject = new GameObject("TMPObject");
                    var tmp = tmpObject.AddComponent<TextMeshPro>();

                    tmp.rectTransform.pivot = new Vector2(0f, 1f);
                    tmp.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 30f);
                    tmp.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30f);

                    tmp.text = text;
                    tmp.color = color;
                    //tmp.alpha = notifTmp.alpha;
                    tmp.font = HGTextMeshProUGUI.defaultLanguageFont;
                    tmp.fontStyle = notifTmp.fontStyle;
                    tmp.lineSpacing = notifTmp.lineSpacing;
                    tmp.characterSpacing = notifTmp.characterSpacing;
                    
                    tmp.enableKerning = notifTmp.enableKerning;
                    tmp.enableWordWrapping = notifTmp.enableWordWrapping;
                    tmp.overflowMode = notifTmp.overflowMode;
                    tmp.alignment = TextAlignmentOptions.TopLeft;
                    tmp.fontSize = notifTmp.fontSize;
                    tmp.autoSizeTextContainer = notifTmp.autoSizeTextContainer;
                    tmp.enableAutoSizing = notifTmp.enableAutoSizing;
                    
                    tmp.ClearMesh(false);
                    tmp.ForceMeshUpdate();

                    var fontMat = tmp.fontMaterial;

                    var size = 18f;
                    var ratio = (float)textMeshTexture.width / (float)textMeshTexture.height;
                    var orthoSizeHalf = new Vector2(size * ratio * 0.5f, size * 0.5f);
                    var pivot = new Vector2(0.5f, 0.5f);

                    var centerOffset = new Vector2(
                        ((float)textMeshTexture.width / 2f - tx) / (float)textMeshTexture.width * size * ratio,
                        -((float)textMeshTexture.height / 2f - ty) / (float)textMeshTexture.height * size
                    );
                    centerOffset *= 0.5f;
                    
                    var matrix = Matrix4x4.Ortho(
                        orthoSizeHalf.x * (pivot.x - 1f) + centerOffset.x, orthoSizeHalf.x * pivot.x + centerOffset.x,
                        orthoSizeHalf.y * (pivot.y - 1f) + centerOffset.y, orthoSizeHalf.y * pivot.y + centerOffset.y,
                        -1, 1
                    );

                    renderTexture = RenderTexture.GetTemporary(textMeshTexture.width, textMeshTexture.height, 24, RenderTextureFormat.ARGB32);
                    renderTexture.Create();

                    Graphics.SetRenderTarget(renderTexture);
                    GL.PushMatrix();
                    GL.LoadProjectionMatrix(matrix);
                    GL.LoadIdentity();
                    GL.Clear(false, true, new Color(0, 0, 0, 0));

                    tmp.renderer.material.SetPass(0);
                    Graphics.DrawMeshNow(tmp.mesh, Matrix4x4.identity);

                    RenderTexture.active = renderTexture;
                    textMeshTexture.ReadPixels(new Rect(0, 0, textMeshTexture.width, textMeshTexture.height), 0, 0, false);
                    textMeshTexture.Apply(false);

                    GL.PopMatrix();

                    RenderTexture.active = null;
                    RenderTexture.ReleaseTemporary(renderTexture);
                    Object.Destroy(tmpObject);

                    for (var x = 0; x < textMeshTexture.width; x++)
                        for (var y = 0; y < textMeshTexture.height; y++)
                        {
                            Color backgroundPixel = texture.GetPixel(x + (int)visualSlot.x, y + (int)visualSlot.y);
                            Color layerPixel = textMeshTexture.GetPixel(x, y);
                            texture.SetPixel(x + (int)visualSlot.x, y + (int)visualSlot.y, new Color(
                                backgroundPixel.r * (1 - layerPixel.a) + layerPixel.r * layerPixel.a,
                                backgroundPixel.g * (1 - layerPixel.a) + layerPixel.g * layerPixel.a,
                                backgroundPixel.b * (1 - layerPixel.a) + layerPixel.b * layerPixel.a,
                                Mathf.Clamp01(backgroundPixel.a + layerPixel.a)
                            ));
                        }
                }

                DrawText(visualSlot.name, visualSlot.color, (float)resizedIcon.width + 10f * relativeScaleX, 10f * relativeScaleY, notifTitleTmp);
                DrawText(visualSlot.description, notifDescriptionTmp.color, (float)resizedIcon.width + 10f * relativeScaleX, 42f * relativeScaleY, notifDescriptionTmp);
            }
            texture.Apply();

            File.WriteAllBytes(System.IO.Path.Combine(path, "ItemTable.png"), texture.EncodeToPNG());
        }

        internal class ItemTableSection
        {
            public List<ItemDef> itemDefs = default;
            public List<EquipmentDef> equipmentDefs = default;
            public Color color = default;
        }

        private class ItemTableVisualSlot
        {
            public float x;
            public float y;
            internal int row;
            public Texture2D icon;
            public string name;
            public string description;
            public Color color;
        }
    }
}
