using RoR2;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
using System.Collections.Generic;
using HG;

namespace MysticsItems
{
    public class Outlines
    {
        public static void Init()
        {
            On.RoR2.SceneCamera.Awake += (orig, self) =>
            {
                orig(self);
                self.gameObject.AddComponent<MysticsItemsOutlineRenderer>();
            };
        }

        public class MysticsItemsOutlineRenderer : MonoBehaviour
        {
            public CommandBuffer commandBuffer;
            public Material outlineMaterial;

            public void Awake()
            {
                commandBuffer = new CommandBuffer();
                outlineMaterial = new Material(Main.AssetBundle.LoadAsset<Shader>("Assets/Misc/Shaders/Unlit/Outline.shader"));
            }

            public void OnRenderImage(RenderTexture source, RenderTexture destination)
            {
                RenderTexture renderTexture = RenderTexture.active = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
                GL.Clear(true, true, Color.clear);
                RenderTexture.active = null;
                commandBuffer.Clear();

                RenderTargetIdentifier renderTarget = new RenderTargetIdentifier(renderTexture);
                commandBuffer.SetRenderTarget(renderTarget);
                outlineMaterial.SetTexture("_SceneTex", source);
                outlineMaterial.SetTexture("_OutlineTex", renderTexture);
                Dictionary<Renderer, float> rendererDictionary = new Dictionary<Renderer, float>();
                foreach (MysticsItemsOutline outline in MysticsItemsOutline.list)
                {
                    if (outline.isOn && outline.targetRenderer)
                    {
                        float offset = outline.offset;
                        if (!rendererDictionary.ContainsKey(outline.targetRenderer))
                        {
                            rendererDictionary.Add(outline.targetRenderer, 0f);
                        }
                        offset += rendererDictionary[outline.targetRenderer];
                        rendererDictionary[outline.targetRenderer] += offset + outline.thickness;
                        outlineMaterial.SetColor("_Color", outline.color);
                        outlineMaterial.SetFloat("_Offset", offset);
                        outlineMaterial.SetFloat("_Thickness", outline.thickness);
                        commandBuffer.DrawRenderer(outline.targetRenderer, outlineMaterial, 0, 0);
                        commandBuffer.DrawRenderer(outline.targetRenderer, outlineMaterial, 0, 1);
                        RenderTexture.active = renderTexture;
                        Graphics.ExecuteCommandBuffer(commandBuffer);
                        RenderTexture.active = null;
                        commandBuffer.Clear();
                    }
                }

                Graphics.Blit(source, destination, outlineMaterial, 2);
                RenderTexture.ReleaseTemporary(renderTexture);
            }
        }

        public class MysticsItemsOutline : MonoBehaviour
        {
            public static List<MysticsItemsOutline> list = new List<MysticsItemsOutline>();

            public bool isOn = true;
            public float thickness = 1f;
            public float offset = 0f;
            public Color color = Color.white;
            public Renderer targetRenderer;

            public void Awake()
            {
                targetRenderer = gameObject.GetComponentInChildren<Renderer>();
            }

            public void OnEnable()
            {
                list.Add(this);
            }

            public void OnDisable()
            {
                list.Remove(this);
            }
        }
    }
}