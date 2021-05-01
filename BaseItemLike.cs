using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MysticsItems.ContentManagement;

namespace MysticsItems
{
    public abstract class BaseItemLike : BaseLoadableAsset
    {
        public GameObject model;
        public GameObject followerModel;
        public ItemDisplayRuleDict itemDisplayRuleDict = new ItemDisplayRuleDict();
        public static event System.Action onSetupIDRS;
        
        public abstract void PreLoad(); // Always executed before loading

        public virtual void AfterTokensPopulated() { }

        public abstract void SetAssets(string assetName);

        public abstract void SetIcon(string assetName);

        public void PrepareModel(GameObject model)
        {
            model.AddComponent<MysticsItemsItemFollowerVisualScaling>();

            // Automatically set up a ModelPanelParameters component if camera points are present
            Transform focusPoint = model.transform.Find("FocusPoint");
            if (focusPoint)
            {
                Transform cameraPosition = focusPoint.Find("CameraPosition");
                if (cameraPosition)
                {
                    ModelPanelParameters component = model.GetComponent<ModelPanelParameters>();
                    if (!component) component = model.AddComponent<ModelPanelParameters>();
                    component.focusPointTransform = focusPoint;
                    component.cameraPositionTransform = cameraPosition;
                }
            }

            // Apply HG shader
            foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
            {
                foreach (Material material in renderer.sharedMaterials)
                {
                    if (material != null && material.shader.name == "Standard" && material.shader != Main.HopooShaderToMaterial.Standard.shader)
                    {
                        Main.HopooShaderToMaterial.Standard.Apply(material);
                        Main.HopooShaderToMaterial.Standard.Gloss(material, 0.2f, 5f);
                        Main.HopooShaderToMaterial.Standard.Emission(material, 0f);
                        Main.HopooShaderToMaterial.Standard.Dither(material);
                        material.SetTexture("_EmTex", material.GetTexture("_MainTex"));
                    }
                }
            }
        }

        public void CopyModelToFollower()
        {
            if (followerModel) Object.Destroy(followerModel);
            followerModel = PrefabAPI.InstantiateClone(model, model.name + "Follower", false);

            // Add ItemDisplay component for dither, flash and other HG effect support
            ItemDisplay itemDisplay = followerModel.AddComponent<ItemDisplay>();
            List<CharacterModel.RendererInfo> rendererInfos = new List<CharacterModel.RendererInfo>();
            foreach (Renderer renderer in followerModel.GetComponentsInChildren<Renderer>())
            {
                CharacterModel.RendererInfo rendererInfo = new CharacterModel.RendererInfo
                {
                    renderer = renderer,
                    defaultMaterial = renderer.material
                };
                rendererInfos.Add(rendererInfo);
            }
            itemDisplay.rendererInfos = rendererInfos.ToArray();
        }

        public void SetModelPanelDistance(float min = 1f, float max = 10f)
        {
            ModelPanelParameters component = model.GetComponent<ModelPanelParameters>();
            if (component)
            {
                component.minDistance = min;
                component.maxDistance = max;
            }
        }

        public Material GetModelMaterial()
        {
            return model.GetComponentInChildren<MeshRenderer>().material;
        }

        public Material GetFollowerModelMaterial()
        {
            return followerModel.GetComponentInChildren<MeshRenderer>().material;
        }

        public struct MysticsItemsDisplayRules
        {
            public BaseItemLike baseItem;
            public List<ItemDisplayRule> displayRules;
        }

        public static Dictionary<string, List<MysticsItemsDisplayRules>> displayRules = new Dictionary<string, List<MysticsItemsDisplayRules>>();

        public virtual void AddDisplayRule(string bodyName, string childName, Vector3 localPos, Vector3 localAngles, Vector3 localScale)
        {
            AddDisplayRule(bodyName, childName, followerModel, localPos, localAngles, localScale);
        }

        public virtual void AddDisplayRule(string bodyName, string childName, GameObject followerPrefab, Vector3 localPos, Vector3 localAngles, Vector3 localScale)
        {
            displayRules.TryGetValue(bodyName, out List<MysticsItemsDisplayRules> displayRulesList);
            if (displayRulesList == null)
            {
                displayRulesList = new List<MysticsItemsDisplayRules>()
                {
                    new MysticsItemsDisplayRules
                    {
                        baseItem = this,
                        displayRules = new List<ItemDisplayRule>()
                    }
                };
                displayRules.Add(bodyName, displayRulesList);
            }
            MysticsItemsDisplayRules displayRulesForThisItem = default;
            if (displayRulesList.Any(x => x.baseItem == this))
            {
                displayRulesForThisItem = displayRulesList.Find(x => x.baseItem == this);
            }
            else
            {
                displayRulesForThisItem = new MysticsItemsDisplayRules
                {
                    baseItem = this,
                    displayRules = new List<ItemDisplayRule>()
                };
                displayRulesList.Add(displayRulesForThisItem);
            }
            displayRulesForThisItem.displayRules.Add(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = followerPrefab,
                childName = childName,
                localPos = localPos,
                localAngles = localAngles,
                localScale = localScale
            });
        }

        public void SetUnlockable()
        {
            MysticsItemsContent.Resources.unlockableDefs.Add(GetUnlockableDef());
        }

        public abstract UnlockableDef GetUnlockableDef();

        public static void Init()
        {
            On.RoR2.CharacterBody.Awake += (orig, self) =>
            {
                orig(self);
                GetItemLikeComponent(self);
            };
        }

        public static void PostGameLoad()
        {
            if (onSetupIDRS != null) onSetupIDRS();
            foreach (KeyValuePair<string, List<MysticsItemsDisplayRules>> displayRulesList in displayRules)
            {
                string bodyName = displayRulesList.Key;
                BodyIndex bodyIndex = BodyCatalog.FindBodyIndex(bodyName);
                if (bodyIndex != BodyIndex.None)
                {
                    GameObject bodyPrefab = BodyCatalog.GetBodyPrefab(bodyIndex);
                    CharacterModel characterModel = bodyPrefab.GetComponentInChildren<CharacterModel>();
                    ItemDisplayRuleSet idrs = characterModel.itemDisplayRuleSet;
                    foreach (MysticsItemsDisplayRules displayRules in displayRulesList.Value)
                    {
                        BaseItemLike item = displayRules.baseItem;
                        Object keyAsset = (Object)item.asset;
                        idrs.SetDisplayRuleGroup(keyAsset, new DisplayRuleGroup { rules = displayRules.displayRules.ToArray() });
                    }
                    idrs.InvokeMethod("GenerateRuntimeValues");
                }
                else
                {
                    Main.logger.LogError("Body " + bodyName + " not found");
                }
            }
        }

        public abstract PickupIndex GetPickupIndex();

        public static StringBuilder globalStringBuilder = new StringBuilder();

        public class Reskinner : MonoBehaviour
        {
            public string defaultBodyName;

            public void Start()
            {
                MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
                if (meshRenderer)
                {
                    string bodyName = defaultBodyName;
                    int skinIndex = 0;
                    CharacterModel characterModel = GetComponentInParent<CharacterModel>();
                    if (characterModel)
                    {
                        CharacterBody body = characterModel.body;
                        if (body)
                        {
                            bodyName = body.name;
                            skinIndex = (int)body.skinIndex;
                        }
                    }
                    GameObject bodyPrefab = BodyCatalog.FindBodyPrefab(bodyName);
                    if (bodyPrefab)
                    {
                        CharacterBody body = bodyPrefab.GetComponent<CharacterBody>();
                        if (body)
                        {
                            ModelSkinController modelSkinController = body.GetComponentInChildren<ModelSkinController>();
                            if (modelSkinController)
                            {
                                Material mat = GetBestMaterial(modelSkinController.skins[skinIndex].rendererInfos.ToList());
                                meshRenderer.material.shader = mat.shader;
                                meshRenderer.material.CopyPropertiesFromMaterial(mat);

                                ItemDisplay itemDisplay = GetComponent<ItemDisplay>();
                                if (itemDisplay)
                                {
                                    itemDisplay.rendererInfos[0].defaultMaterial = meshRenderer.material;
                                }
                            }
                        }
                    }
                }
            }

            public static Material GetBestMaterial(List<CharacterModel.RendererInfo> rendererInfos)
            {
                rendererInfos.Sort((x, y) => {
                    Shader shaderX = x.defaultMaterial.shader;
                    Shader shaderY = y.defaultMaterial.shader;
                    if (shaderX == Main.HopooShaderToMaterial.Standard.shader && shaderY != Main.HopooShaderToMaterial.Standard.shader) return -1;
                    if (shaderX.name.StartsWith("Hopoo Games/FX/") && !shaderY.name.StartsWith("Hopoo Games/FX/")) return 1;
                    return 0;
                });
                return rendererInfos.First().defaultMaterial;
            }
        }

        public class MysticsItemsItemFollowerVisualScaling : MonoBehaviour
        {
            public List<GameObject> effectObjects = new List<GameObject>();

            public void Start()
            {
                foreach (GameObject effectObject in effectObjects)
                {
                    if (effectObject)
                    {
                        float scale = (effectObject.transform.lossyScale.x + effectObject.transform.lossyScale.y + effectObject.transform.lossyScale.z) / 3f;
                        float localScale = (effectObject.transform.localScale.x + effectObject.transform.localScale.y + effectObject.transform.localScale.z) / 3f;

                        foreach (ParticleSystem particleSystem in effectObject.GetComponents<ParticleSystem>())
                        {
                            ParticleSystem.MainModule main = particleSystem.main;
                            ParticleSystem.MinMaxCurve gravityModifier = main.gravityModifier;
                            gravityModifier.constant *= scale;
                            main.gravityModifier = gravityModifier;
                        }

                        foreach (Light light in effectObject.GetComponents<Light>())
                        {
                            light.range *= scale / localScale;
                        }
                    }
                }
            }
        }

        public void SetScalableChildEffect(string childName)
        {
            GameObject child = model.transform.Find(childName).gameObject;
            if (child)
            {
                SetScalableChildEffect(child);
            }
            else
            {
                Main.logger.LogError("Couldn't find child effect " + childName);
            }
        }

        public void SetScalableChildEffect(GameObject child)
        {
            List<GameObject> effectObjects = model.GetComponent<MysticsItemsItemFollowerVisualScaling>().effectObjects;
            if (!effectObjects.Contains(child)) effectObjects.Add(child);
        }

        public static MysticsItemsItemLikeComponent GetItemLikeComponent(CharacterBody characterBody)
        {
            MysticsItemsItemLikeComponent component = characterBody.GetComponent<MysticsItemsItemLikeComponent>();
            if (!component) component = characterBody.gameObject.AddComponent<MysticsItemsItemLikeComponent>();
            return component;
        }

        public static T GetComponentFieldValue<T>(CharacterBody characterBody, string name) where T : System.Type
        {
            MysticsItemsItemLikeComponent component = GetItemLikeComponent(characterBody);
            if (component.fields.ContainsKey(name)) return (T)component.fields[name];
            return default;
        }

        public static void SetComponentFieldValue(CharacterBody characterBody, string name, object value)
        {
            MysticsItemsItemLikeComponent component = GetItemLikeComponent(characterBody);
            if (component.fields.ContainsKey(name)) component.fields[name] = value;
            else component.fields.Add(name, value);
        }

        public class MysticsItemsItemLikeComponent : MonoBehaviour
        {
            public ItemDef itemDef;
            public Dictionary<string, object> fields = new Dictionary<string, object>();
        }
    }
}
