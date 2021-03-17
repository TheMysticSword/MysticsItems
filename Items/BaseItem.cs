using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MysticsItems.Items
{
    public abstract class BaseItem
    {
        public GameObject model;
        public GameObject followerModel;
        public ItemDef itemDef;
        public ItemDisplayRuleDict itemDisplayRuleDict = new ItemDisplayRuleDict();
        public ItemIndex itemIndex;
        public static Dictionary<System.Type, BaseItem> registeredItems = new Dictionary<System.Type, BaseItem>();
        public static List<BaseItem> deployableBanned = new List<BaseItem>();
        public static bool dontLoad = false;

        public static BaseItem GetFromType(System.Type type)
        {
            if (registeredItems.ContainsKey(type))
            {
                return registeredItems[type];
            }
            return null;
        }

        public virtual void Add()
        {
            itemDef = new ItemDef();
            PreAdd();
            itemDef.name = Main.TokenPrefix + itemDef.name;
            if (dontLoad) return;
            itemIndex = ItemAPI.Add(new CustomItem(itemDef, itemDisplayRuleDict));
            registeredItems.Add(GetType(), this);
            OnAdd();
            if (OnAddGlobal != null) OnAddGlobal.Invoke(this);
        }

        public virtual void SetAssets(string assetName)
        {
            model = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/" + assetName + "/Model.prefab");
            model.name = "mdl" + itemDef.name;

            bool followerModelSeparate = Main.AssetBundle.Contains("Assets/Items/" + assetName + "/FollowerModel.prefab");
            if (followerModelSeparate)
            {
                followerModel = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/" + assetName + "/FollowerModel.prefab");
                followerModel.name = "mdl" + itemDef.name + "Follower";
            }

            PrepareModel(model);
            if (followerModelSeparate) PrepareModel(followerModel);

            // Separate the follower model from the pickup model for adding different visual effects to followers
            if (!followerModelSeparate) CopyModelToFollower();

            itemDef.pickupModelPath = Main.AssetPrefix + ":Assets/Items/" + assetName + "/Model.prefab";
            itemDef.pickupIconPath = Main.AssetPrefix + ":Assets/Items/" + assetName + "/Icon.png";
        }

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
                    if (material != null && material.shader.name == "Standard")
                    {
                        Main.HopooShaderToMaterial.Standard.Apply(material);
                        Main.HopooShaderToMaterial.Standard.Gloss(material, 0.2f, 5f);
                        Main.HopooShaderToMaterial.Standard.Emission(material, 0f);
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
                foreach (Material material in renderer.materials)
                {
                    if (material != null && material.shader == Main.HopooShaderToMaterial.Standard.shader)
                    {
                        Main.HopooShaderToMaterial.Standard.Dither(material);
                    }
                }
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

        public void DefaultDisplayRule(string childName, Vector3 localPos, Vector3 localAngles, Vector3 localScale)
        {
            ItemDisplayRule[] defaultRules = itemDisplayRuleDict.DefaultRules;
            HG.ArrayUtils.ArrayAppend(ref defaultRules, new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = followerModel,
                childName = childName,
                localPos = localPos,
                localAngles = localAngles,
                localScale = localScale
            });
            typeof(ItemDisplayRuleDict).GetProperty("DefaultRules").SetValue(itemDisplayRuleDict, defaultRules, Main.bindingFlagAll, null, null, null);
        }

        public void AddDisplayRule(string characterModelName, string childName, Vector3 localPos, Vector3 localAngles, Vector3 localScale)
        {
            AddDisplayRule(characterModelName, childName, followerModel, localPos, localAngles, localScale);
        }

        public void AddDisplayRule(string characterModelName, string childName, GameObject followerModel, Vector3 localPos, Vector3 localAngles, Vector3 localScale)
        {
            itemDisplayRuleDict.TryGetRules(characterModelName, out ItemDisplayRule[] itemDisplayRules);
            if (itemDisplayRules.Length == 0) itemDisplayRules = new ItemDisplayRule[] { };
            HG.ArrayUtils.ArrayAppend(ref itemDisplayRules, new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = followerModel,
                childName = childName,
                localPos = localPos,
                localAngles = localAngles,
                localScale = localScale
            });
            itemDisplayRuleDict.Add(characterModelName, itemDisplayRules);
        }

        public struct BodyIndexBasedDisplayRules
        {
            public BaseItem baseItem;
            public List<ItemDisplayRule> displayRules;
        }

        public static Dictionary<int, List<BodyIndexBasedDisplayRules>> bodyIndexBasedDisplayRules = new Dictionary<int, List<BodyIndexBasedDisplayRules>>();

        public virtual void AddDisplayRule(int bodyIndex, string childName, Vector3 localPos, Vector3 localAngles, Vector3 localScale)
        {
            bodyIndexBasedDisplayRules.TryGetValue(bodyIndex, out List<BodyIndexBasedDisplayRules> displayRulesList);
            if (displayRulesList == null)
            {
                displayRulesList = new List<BodyIndexBasedDisplayRules>()
                {
                    new BodyIndexBasedDisplayRules
                    {
                        baseItem = this,
                        displayRules = new List<ItemDisplayRule>()
                    }
                };
                bodyIndexBasedDisplayRules.Add(bodyIndex, displayRulesList);
            }
            BodyIndexBasedDisplayRules displayRules = default;
            if (displayRulesList.Any(x => x.baseItem == this))
            {
                displayRules = displayRulesList.Find(x => x.baseItem == this);
            }
            else
            {
                displayRules = new BodyIndexBasedDisplayRules
                {
                    baseItem = this,
                    displayRules = new List<ItemDisplayRule>()
                };
                displayRulesList.Add(displayRules);
            }
            displayRules.displayRules.Add(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = followerModel,
                childName = childName,
                localPos = localPos,
                localAngles = localAngles,
                localScale = localScale
            });
        }

        public virtual void SetUnlockable()
        {
            itemDef.unlockableName = Main.TokenPrefix + "Items." + itemDef.name;
            Unlockables.Register(itemDef.unlockableName, new UnlockableDef
            {
                nameToken = !string.IsNullOrEmpty(itemDef.nameToken) ? itemDef.nameToken : ("ITEM_" + Main.TokenPrefix + itemDef.name + "_NAME").ToUpper()
            });
        }

        public void BanFromDeployables()
        {
            deployableBanned.Add(this);
        }

        public static void RemoveDeployableBannedItems(Deployable deployable)
        {
            CharacterMaster master = deployable.gameObject.GetComponent<CharacterMaster>();
            if (master && master.hasBody && deployable.ownerMaster)
            {
                foreach (BaseItem item in deployableBanned)
                {
                    master.GetBody().inventory.ResetItem(item.itemIndex);
                }
            }
        }

        public virtual void PreAdd() { }

        public virtual void OnAdd() { }

        public static void Init()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += (orig, self) =>
            {
                orig(self);
                if (self.master)
                {
                    Deployable deployable = self.master.GetComponent<Deployable>();
                    if (deployable) RemoveDeployableBannedItems(deployable);
                }
            };

            On.RoR2.CharacterMaster.AddDeployable += (orig, self, deployable, slot) =>
            {
                orig(self, deployable, slot);
                RemoveDeployableBannedItems(deployable);
            };
        }

        public static void PostGameLoad()
        {
            foreach (KeyValuePair<int, List<BodyIndexBasedDisplayRules>> displayRulesList in bodyIndexBasedDisplayRules)
            {
                GameObject bodyPrefab = BodyCatalog.GetBodyPrefab(displayRulesList.Key);
                CharacterModel characterModel = bodyPrefab.GetComponentInChildren<CharacterModel>();
                ItemDisplayRuleSet idrs = characterModel.itemDisplayRuleSet;
                foreach (BodyIndexBasedDisplayRules displayRules in displayRulesList.Value)
                {
                    BaseItem item = displayRules.baseItem;
                    item.SetDisplayRuleGroup(idrs, new DisplayRuleGroup { rules = displayRules.displayRules.ToArray() });
                }
                idrs.InvokeMethod("GenerateRuntimeValues");
            }
        }

        public virtual void SetDisplayRuleGroup(ItemDisplayRuleSet idrs, DisplayRuleGroup displayRuleGroup)
        {
            idrs.SetItemDisplayRuleGroup(itemDef.name, displayRuleGroup);
        }

        public virtual PickupIndex GetPickupIndex()
        {
            return PickupCatalog.FindPickupIndex(itemIndex);
        }

        public event System.Action<BaseItem> OnAddGlobal;

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
    }
}
