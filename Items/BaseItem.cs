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
        public CharacterItem characterItemComponent;

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
            itemIndex = ItemAPI.Add(new CustomItem(itemDef, itemDisplayRuleDict));
            registeredItems.Add(GetType(), this);
            OnAdd();
            if (OnAddGlobal != null) OnAddGlobal.Invoke(this);
        }

        public virtual void SetAssets(string assetName)
        {
            model = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/" + assetName + "/Model.prefab");
            model.name = "mdl" + itemDef.name;

            PrepareModel();

            // Separate the follower model from the pickup model for adding different visual effects to followers
            CopyModelToFollower();

            itemDef.pickupModelPath = Main.AssetPrefix + ":Assets/Items/" + assetName + "/Model.prefab";
            itemDef.pickupIconPath = Main.AssetPrefix + ":Assets/Items/" + assetName + "/Icon.png";
        }

        public void PrepareModel()
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

        public virtual void PreAdd() { }

        public virtual void OnAdd() { }

        public static void Init()
        {
            On.RoR2.GenericPickupController.UpdatePickupDisplay += (orig, self) =>
            {
                orig(self);
                if (self.pickupDisplay)
                {
                    GameObject modelObject = self.pickupDisplay.GetFieldValue<GameObject>("modelObject");
                    if (modelObject)
                    {
                        CharacterItem characterItem = modelObject.GetComponent<CharacterItem>();
                        if (characterItem)
                        {
                            characterItem.SetOutlineVisibility(true);
                        }
                    }
                }
            };

            On.RoR2.GenericPickupController.GetInteractability += (orig, self, activator) =>
            {
                CharacterBody characterBody = activator.GetComponent<CharacterBody>();
                if (characterBody && self.pickupDisplay)
                {
                    GameObject modelObject = self.pickupDisplay.GetFieldValue<GameObject>("modelObject");
                    if (modelObject)
                    {
                        CharacterItem characterItem = modelObject.GetComponent<CharacterItem>();
                        if (characterItem)
                        {
                            CharacterInfo characterInfo = FindCharacterInfo(characterItem.bodyName);
                            if (BodyCatalog.GetBodyName(characterBody.bodyIndex) != characterInfo.bodyName)
                            {
                                return Interactability.Disabled;
                            }
                        }
                    }
                }
                return orig(self, activator);
            };

            On.RoR2.GenericPickupController.AttemptGrant += (orig, self, body) =>
            {
                if (self.pickupDisplay)
                {
                    GameObject modelObject = self.pickupDisplay.GetFieldValue<GameObject>("modelObject");
                    if (modelObject)
                    {
                        CharacterItem characterItem = modelObject.GetComponent<CharacterItem>();
                        if (characterItem)
                        {
                            CharacterInfo characterInfo = FindCharacterInfo(characterItem.bodyName);
                            if (BodyCatalog.GetBodyName(body.bodyIndex) != characterInfo.bodyName) return;
                        }
                    }
                }
                orig(self, body);
            };

            On.RoR2.GenericPickupController.GetDisplayName += (orig, self) =>
            {
                string displayName = orig(self);
                if (self.pickupDisplay)
                {
                    GameObject modelObject = self.pickupDisplay.GetFieldValue<GameObject>("modelObject");
                    if (modelObject)
                    {
                        CharacterItem characterItem = modelObject.GetComponent<CharacterItem>();
                        if (characterItem)
                        {
                            CharacterInfo characterInfo = FindCharacterInfo(characterItem.bodyName);
                            globalStringBuilder.Clear();
                            globalStringBuilder.Append(" <nobr>");
                            globalStringBuilder.Append("<color=#");
                            globalStringBuilder.AppendColor32RGBHexValues(characterInfo.color);
                            globalStringBuilder.Append(">");
                            globalStringBuilder.Append("(");
                            globalStringBuilder.Append(string.Format(Language.GetString("MYSTICSITEMS_CHARACTERITEM_PICKUP_FORMAT"), Language.GetString(characterInfo.nameToken)));
                            globalStringBuilder.Append(")");
                            globalStringBuilder.Append("</color>");
                            globalStringBuilder.Append("</nobr>");
                            displayName += globalStringBuilder.ToString();
                        }
                    }
                }
                return displayName;
            };

            On.RoR2.CharacterMaster.Respawn += (orig, self, footPosition, rotation, tryToGroundSafely) =>
            {
                CharacterBody result = orig(self, footPosition, rotation, tryToGroundSafely);
                if (NetworkServer.active)
                {
                    if (MysticsItemsCharacterItemsTracker.instance)
                    {
                        MysticsItemsCharacterItemsTracker.instance.Refresh();
                    }
                }
                return result;
            };

            characterInfo.Add(new CharacterInfo
            {
                nameToken = "UNDEFINED",
                bodyName = "",
                color = new Color32(0, 0, 0, 255)
            });
            characterInfo.Add(new CharacterInfo
            {
                nameToken = "COMMANDO_BODY_NAME",
                bodyName = "CommandoBody",
                color = new Color32(222, 171, 60, 255)
            });
            characterInfo.Add(new CharacterInfo
            {
                nameToken = "HUNTRESS_BODY_NAME",
                bodyName = "HuntressBody",
                color = new Color32(192, 54, 57, 255)
            });
            characterInfo.Add(new CharacterInfo
            {
                nameToken = "TOOLBOT_BODY_NAME",
                bodyName = "ToolbotBody",
                color = new Color32(181, 176, 54, 255)
            });
            characterInfo.Add(new CharacterInfo
            {
                nameToken = "ENGI_BODY_NAME",
                bodyName = "EngiBody",
                color = new Color32(142, 75, 192, 255)
            });
            characterInfo.Add(new CharacterInfo
            {
                nameToken = "MAGE_BODY_NAME",
                bodyName = "MageBody",
                color = new Color32(231, 231, 231, 255)
            });
            characterInfo.Add(new CharacterInfo
            {
                nameToken = "MERC_BODY_NAME",
                bodyName = "MercBody",
                color = new Color32(93, 95, 142, 255)
            });
            characterInfo.Add(new CharacterInfo
            {
                nameToken = "TREEBOT_BODY_NAME",
                bodyName = "TreebotBody",
                color = new Color32(186, 190, 173, 255)
            });
            characterInfo.Add(new CharacterInfo
            {
                nameToken = "LOADER_BODY_NAME",
                bodyName = "LoaderBody",
                color = new Color32(203, 176, 66, 255)
            });
            characterInfo.Add(new CharacterInfo
            {
                nameToken = "CROCO_BODY_NAME",
                bodyName = "CrocoBody",
                color = new Color32(173, 85, 107, 255)
            });
            characterInfo.Add(new CharacterInfo
            {
                nameToken = "CAPTAIN_BODY_NAME",
                bodyName = "CaptainBody",
                color = new Color32(57, 60, 90, 255)
            });

            On.RoR2.Run.Awake += (orig, self) =>
            {
                orig(self);
                self.gameObject.AddComponent<MysticsItemsCharacterItemsTracker>();
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

        public class MysticsItemsCharacterItemsTracker : MonoBehaviour
        {
            public List<string> bodyNames = new List<string>();

            public static MysticsItemsCharacterItemsTracker instance;

            public void OnEnable()
            {
                instance = this;
            }

            public void OnDisable()
            {
                instance = null;
            }

            public void Refresh()
            {
                if (!NetworkServer.active) return;
                bodyNames.Clear();
                foreach (PlayerCharacterMasterController player in PlayerCharacterMasterController.instances)
                {
                    if (player.master.hasBody)
                    {
                        CharacterBody body = player.master.GetBody();
                        bodyNames.Add(BodyCatalog.GetBodyName(body.bodyIndex));
                    }
                }

                foreach (CharacterItem characterItem in characterItems)
                {
                    PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(characterItem.itemIndex);

                    List<PickupIndex> list = null;
                    switch (characterItem.itemTier)
                    {
                        case ItemTier.Tier1:
                            list = Run.instance.availableTier1DropList;
                            break;
                        case ItemTier.Tier2:
                            list = Run.instance.availableTier2DropList;
                            break;
                        case ItemTier.Tier3:
                            list = Run.instance.availableTier3DropList;
                            break;
                        case ItemTier.Lunar:
                            list = Run.instance.availableLunarDropList;
                            break;
                        case ItemTier.Boss:
                            list = Run.instance.availableBossDropList;
                            break;
                    }
                    if (list != null)
                    {
                        while (bodyNames.Contains(characterItem.bodyName) && !list.Contains(pickupIndex)) list.Add(pickupIndex);
                        while (!bodyNames.Contains(characterItem.bodyName) && list.Contains(pickupIndex)) list.Remove(pickupIndex);
                    }
                }
            }
        }

        public bool CharacterItemCanDrop
        {
            get
            {
                if (!characterItemComponent || !MysticsItemsCharacterItemsTracker.instance) return true;
                if (MysticsItemsCharacterItemsTracker.instance.bodyNames.Contains(characterItemComponent.bodyName)) return true;
                return false;
            }
        }

        public virtual PickupIndex GetPickupIndex()
        {
            return PickupCatalog.FindPickupIndex(itemIndex);
        }

        public event System.Action<BaseItem> OnAddGlobal;

        public static StringBuilder globalStringBuilder = new StringBuilder();

        public struct CharacterInfo
        {
            public string nameToken;
            public string bodyName;
            public Color color;
        }
        public static List<CharacterInfo> characterInfo = new List<CharacterInfo>();
        public static CharacterInfo FindCharacterInfo(string bodyName)
        {
            CharacterInfo currentCharacterInfo = characterInfo.FirstOrDefault(x => x.bodyName == bodyName);
            if (currentCharacterInfo.Equals(default(CharacterInfo)))
            {
                currentCharacterInfo = characterInfo.First();
                Main.logger.LogError("Couldn't find CharacterInfo with bodyName " + bodyName);
            }
            return currentCharacterInfo;
        }

        public static List<CharacterItem> characterItems = new List<CharacterItem>();

        public virtual void SetCharacterItem(string bodyName)
        {
            HG.ArrayUtils.ArrayAppend(ref itemDef.tags, ItemTag.AIBlacklist);
            HG.ArrayUtils.ArrayAppend(ref itemDef.tags, ItemTag.BrotherBlacklist);

            CharacterInfo currentCharacterInfo = FindCharacterInfo(bodyName);

            CharacterItem characterItem = model.AddComponent<CharacterItem>();
            characterItem.bodyName = currentCharacterInfo.bodyName;
            characterItem.itemIndex = itemDef.itemIndex;
            characterItem.itemTier = itemDef.tier;
            characterItems.Add(characterItem);
            characterItemComponent = characterItem;

            Reskinner reskinner = model.AddComponent<Reskinner>();
            reskinner.defaultBodyName = currentCharacterInfo.bodyName;
            reskinner = followerModel.AddComponent<Reskinner>();
            reskinner.defaultBodyName = currentCharacterInfo.bodyName;
        }

        public class CharacterItem : MonoBehaviour
        {
            public string bodyName;
            public ItemIndex itemIndex;
            public ItemTier itemTier;

            public void Awake()
            {
                CharacterInfo characterInfo = FindCharacterInfo(bodyName);
                Outlines.Outline outline = gameObject.AddComponent<Outlines.Outline>();
                outline.offset = 1f;
                outline.color = characterInfo.color;
                outline.isOn = false;
            }

            public void SetOutlineVisibility(bool visible)
            {
                foreach (Outlines.Outline outline in GetComponentsInChildren<Outlines.Outline>())
                {
                    outline.isOn = visible;
                }
            }
        }

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
                                Material mat = modelSkinController.skins[skinIndex].rendererInfos[0].defaultMaterial;
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
