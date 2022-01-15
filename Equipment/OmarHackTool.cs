using RoR2;
using RoR2.DirectionalSearch;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using static MysticsItems.BalanceConfigManager;
using RoR2.UI;
using TMPro;
using R2API.Networking.Interfaces;
using R2API.Networking;

namespace MysticsItems.Equipment
{
    public class OmarHackTool : BaseEquipment
    {
        public static GameObject crosshairPrefab;
        public static GameObject hudPrefab;
        public static GameObject hackVFXPrefab;

        public override void OnPluginAwake()
        {
            NetworkingAPI.RegisterMessageType<MysticsItemsOmarHackToolBehaviour.SyncUsesLeft>();
        }

        public override void OnLoad()
        {
            equipmentDef.name = "MysticsItems_OmarHackTool";
            equipmentDef.cooldown = new ConfigurableCooldown("Equipment: From Omar With Love", 60f).Value;
            equipmentDef.canDrop = true;
            equipmentDef.enigmaCompatible = new ConfigurableEnigmaCompatibleBool("Equipment: From Omar With Love", false).Value;
            //equipmentDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Wirehack Wrench/Model.prefab"));
            //equipmentDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Equipment/Wirehack Wrench/Icon.png");

            //itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(equipmentDef.pickupModelPrefab, equipmentDef.pickupModelPrefab.name + "Display", false));
            /*
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Stomach", new Vector3(-0.163F, 0.092F, -0.036F), new Vector3(356.022F, 118.071F, 26.4F), new Vector3(0.024F, 0.024F, 0.024F));
                AddDisplayRule("HuntressBody", "Pelvis", new Vector3(-0.088F, -0.085F, 0.059F), new Vector3(0.679F, 36.762F, 196.086F), new Vector3(0.019F, 0.019F, 0.019F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(0.129F, 0.025F, -0.111F), new Vector3(316.052F, 250.792F, 338.145F), new Vector3(0.038F, 0.038F, 0.038F));
                AddDisplayRule("ToolbotBody", "Hip", new Vector3(-1.202F, 0.577F, -0.876F), new Vector3(0F, 180F, 180F), new Vector3(0.349F, 0.349F, 0.349F));
                AddDisplayRule("EngiBody", "Pelvis", new Vector3(-0.178F, 0.078F, 0.157F), new Vector3(11.745F, 186.295F, 185.936F), new Vector3(0.047F, 0.047F, 0.047F));
                AddDisplayRule("MageBody", "Pelvis", new Vector3(-0.172F, -0.067F, -0.078F), new Vector3(7.421F, 5.596F, 187.29F), new Vector3(0.027F, 0.027F, 0.027F));
                AddDisplayRule("MercBody", "Chest", new Vector3(-0.115F, 0.032F, 0.083F), new Vector3(18.292F, 60.198F, 185.734F), new Vector3(0.027F, 0.027F, 0.027F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(-0.485F, 0.701F, -0.803F), new Vector3(26.173F, 24.306F, 86.838F), new Vector3(0.061F, 0.061F, 0.061F));
                AddDisplayRule("LoaderBody", "Pelvis", new Vector3(-0.216F, -0.016F, -0.022F), new Vector3(342.363F, 183.205F, 159.555F), new Vector3(0.045F, 0.045F, 0.045F));
                AddDisplayRule("CrocoBody", "SpineStomach1", new Vector3(0.845F, 0.495F, 1.289F), new Vector3(74.633F, 327.618F, 247.859F), new Vector3(0.361F, 0.361F, 0.361F));
                AddDisplayRule("CaptainBody", "Stomach", new Vector3(-0.195F, 0.128F, 0.126F), new Vector3(336.504F, 156.734F, 358.159F), new Vector3(0.041F, 0.041F, 0.041F));
                AddDisplayRule("ScavBody", "Backpack", new Vector3(-5.969F, 10.94F, 0.665F), new Vector3(338.478F, 350.544F, 54.934F), new Vector3(1.363F, 1.363F, 1.363F));
                AddDisplayRule("EquipmentDroneBody", "GunBarrelBase", new Vector3(0F, 0F, 1.1F), new Vector3(52.577F, 0F, 0.001F), new Vector3(0.283F, 0.283F, 0.283F));
            };
            */

            crosshairPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/WoodSpriteIndicator"), "MysticsItems_OmarHackToolIndicator", false);
            var spriteTransform = crosshairPrefab.GetComponentInChildren<SpriteRenderer>().transform;
            var rotateComponent = crosshairPrefab.GetComponentInChildren<Rewired.ComponentControls.Effects.RotateAroundAxis>();
            var crosshairSprites = Main.AssetBundle.LoadAssetWithSubAssets<Sprite>("Assets/Equipment/From Omar With Love/HoverIndicator.png");
            for (var i = 0; i < crosshairSprites.Length; i++)
            {
                var spriteTransform2 = Object.Instantiate(spriteTransform.gameObject, spriteTransform);
                var spriteRenderer = spriteTransform2.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = crosshairSprites[i];
                spriteRenderer.color = new Color32(0, 255, 127, 255);
                spriteRenderer.transform.rotation = Quaternion.identity;
                var rotateComponent2 = spriteTransform2.GetComponent<Rewired.ComponentControls.Effects.RotateAroundAxis>();
                rotateComponent2.rotateAroundAxis = rotateComponent.rotateAroundAxis;
                rotateComponent2.relativeTo = rotateComponent.relativeTo;
                rotateComponent2.speed = Rewired.ComponentControls.Effects.RotateAroundAxis.Speed.Slow;
                rotateComponent2.slowRotationSpeed = (i + 1) * 25f;
                rotateComponent2.reverse = (i % 2) == 0;
                Object.Destroy(spriteTransform2.GetComponent<ObjectScaleCurve>());
            }
            Object.Destroy(spriteTransform.GetComponent<SpriteRenderer>());
            Object.Destroy(rotateComponent);
            crosshairPrefab.GetComponentInChildren<TMPro.TextMeshPro>().color = new Color32(0, 255, 127, 255);
            while (crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.length > 0) crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.RemoveKey(0);
            crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(0f, 0f);
            crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(0.5f, 1f);
            crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(1f, 1f);

            UseTargetFinder(TargetFinderType.Custom);

            On.RoR2.PurchaseInteraction.Awake += (orig, self) =>
            {
                orig(self);
                if (self.GetComponent<ChestBehavior>() && self.GetComponent<PurchaseInteraction>() && self.displayNameToken.Contains("CHEST"))
                {
                    ModelLocator modelLocator = self.GetComponent<ModelLocator>();
                    if (modelLocator && modelLocator.modelTransform)
                    {
                        MysticsItemsChestLocator component = self.gameObject.AddComponent<MysticsItemsChestLocator>();
                        component.childTransform = modelLocator.modelTransform;
                    }
                }
            };

            On.RoR2.EquipmentSlot.Update += (orig, self) =>
            {
                orig(self);
                if (self.equipmentIndex == equipmentDef.equipmentIndex)
                {
                    MysticsRisky2UtilsEquipmentTarget targetInfo = self.GetComponent<MysticsRisky2UtilsEquipmentTarget>();
                    if (targetInfo)
                    {
                        var component = self.GetComponent<MysticsItemsOmarHackToolBehaviour>();
                        if (component && component.usesLeft > 0)
                        {
                            ChestSearch duplicatorSearch = targetInfo.GetCustomTargetFinder<ChestSearch>();
                            float num;
                            Ray aimRay = CameraRigController.ModifyAimRayIfApplicable(self.GetAimRay(), self.gameObject, out num);
                            duplicatorSearch.searchOrigin = aimRay.origin;
                            duplicatorSearch.searchDirection = aimRay.direction;
                            duplicatorSearch.minAngleFilter = 0f;
                            duplicatorSearch.maxAngleFilter = 10f;
                            duplicatorSearch.minDistanceFilter = 0f;
                            duplicatorSearch.maxDistanceFilter = 30f + num;
                            duplicatorSearch.filterByDistinctEntity = false;
                            duplicatorSearch.filterByLoS = true;
                            duplicatorSearch.sortMode = SortMode.DistanceAndAngle;
                            MysticsItemsChestLocator duplicator = duplicatorSearch.SearchCandidatesForSingleTarget(InstanceTracker.GetInstancesList<MysticsItemsChestLocator>());
                            if (duplicator)
                            {
                                targetInfo.obj = duplicator.gameObject;
                                targetInfo.indicator.visualizerPrefab = crosshairPrefab;
                                targetInfo.indicator.targetTransform = duplicator.childTransform;
                            }
                            else
                            {
                                targetInfo.Invalidate();
                            }
                            targetInfo.indicator.active = duplicator;
                        }
                        else
                        {
                            targetInfo.Invalidate();
                            targetInfo.indicator.active = false;
                        }
                    }
                }
            };

            On.RoR2.EquipmentSlot.Awake += EquipmentSlot_Awake;
            On.RoR2.EquipmentSlot.ExecuteIfReady += EquipmentSlot_ExecuteIfReady;
            On.RoR2.EquipmentSlot.UpdateInventory += EquipmentSlot_UpdateInventory;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            Stage.onStageStartGlobal += Stage_onStageStartGlobal;
            HUD.onHudTargetChangedGlobal += HUD_onHudTargetChangedGlobal;

            On.RoR2.UI.EquipmentIcon.Update += EquipmentIcon_Update;

            hudPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/From Omar With Love/OverEquipmentIcon.prefab");
            var hudComp = hudPrefab.AddComponent<MysticsItemsOmarHackToolHUD>();
            hudComp.usesLeftText = hudPrefab.transform.Find("UsesText").GetComponent<TextMeshProUGUI>();
            hudComp.usesLeftText.font = HGTextMeshProUGUI.defaultLanguageFont;

            hackVFXPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/From Omar With Love/HackVFX.prefab");
            EffectComponent effectComponent = hackVFXPrefab.AddComponent<EffectComponent>();
            effectComponent.applyScale = true;
            effectComponent.soundName = "MysticsItems_Play_item_use_OmarHackTool";
            VFXAttributes vfxAttributes = hackVFXPrefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            hackVFXPrefab.AddComponent<DestroyOnTimer>().duration = 10f;
            MysticsItemsContent.Resources.effectPrefabs.Add(hackVFXPrefab);
        }

        private void HUD_onHudTargetChangedGlobal(HUD hud)
        {
            MysticsItemsOmarHackToolHUD.RefreshAll();
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            MysticsItemsOmarHackToolHUD.RefreshAll();
        }

        private void EquipmentSlot_UpdateInventory(On.RoR2.EquipmentSlot.orig_UpdateInventory orig, EquipmentSlot self)
        {
            orig(self);
            var component = self.GetComponent<MysticsItemsOmarHackToolBehaviour>();
            if (component)
            {
                var maxStock = self.maxStock;
                if (component.maxUses != maxStock)
                {
                    component.maxUses = maxStock;
                    MysticsItemsOmarHackToolHUD.RefreshAll();
                }
            }
        }

        private void EquipmentIcon_Update(On.RoR2.UI.EquipmentIcon.orig_Update orig, EquipmentIcon self)
        {
            orig(self);
            if (self.targetEquipmentSlot && self.targetEquipmentSlot.equipmentIndex == equipmentDef.equipmentIndex)
            {
                var component = self.targetEquipmentSlot.GetComponent<MysticsItemsOmarHackToolBehaviour>();
                if (component && component.usesLeft <= 0)
                {
                    if (self.iconImage)
                    {
                        self.iconImage.color = Color.gray;
                    }
                }
            }
        }

        private void Stage_onStageStartGlobal(Stage stage)
        {
            foreach (var component in InstanceTracker.GetInstancesList<MysticsItemsOmarHackToolBehaviour>())
            {
                component.usesLeft = component.maxUses;
                MysticsItemsOmarHackToolHUD.RefreshAll();
            }
        }

        private bool EquipmentSlot_ExecuteIfReady(On.RoR2.EquipmentSlot.orig_ExecuteIfReady orig, EquipmentSlot self)
        {
            if (self.equipmentIndex == equipmentDef.equipmentIndex)
            {
                var component = self.GetComponent<MysticsItemsOmarHackToolBehaviour>();
                if (component && component.usesLeft <= 0) return false;
            }
            return orig(self);
        }

        private void EquipmentSlot_Awake(On.RoR2.EquipmentSlot.orig_Awake orig, EquipmentSlot self)
        {
            orig(self);
            self.gameObject.AddComponent<MysticsItemsOmarHackToolBehaviour>();
        }

        public class MysticsItemsOmarHackToolBehaviour : MonoBehaviour
        {
            private int _usesLeft = 0;
            public int usesLeft
            {
                get { return _usesLeft; }
                set
                {
                    if (_usesLeft != value)
                    {
                        _usesLeft = value;
                        MysticsItemsOmarHackToolHUD.RefreshAll();
                        if (NetworkServer.active)
                            new SyncUsesLeft(gameObject.GetComponent<NetworkIdentity>().netId, value).Send(NetworkDestination.Clients);
                    }
                }
            }

            public class SyncUsesLeft : INetMessage
            {
                NetworkInstanceId objID;
                int usesLeft;

                public SyncUsesLeft()
                {
                }

                public SyncUsesLeft(NetworkInstanceId objID, int usesLeft)
                {
                    this.objID = objID;
                    this.usesLeft = usesLeft;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                    usesLeft = reader.ReadInt32();
                }

                public void OnReceived()
                {
                    if (NetworkServer.active) return;
                    GameObject obj = Util.FindNetworkObject(objID);
                    if (obj)
                    {
                        var component = obj.GetComponent<MysticsItemsOmarHackToolBehaviour>();
                        if (component) component.usesLeft = usesLeft;
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                    writer.Write(usesLeft);
                }
            }

            private int _maxUses = 0;
            public int maxUses
            {
                get { return _maxUses; }
                set
                {
                    usesLeft += Mathf.Max(value - _maxUses, 0);
                    _maxUses = value;
                }
            }
            
            public void OnEnable()
            {
                InstanceTracker.Add(this);
            }

            public void OnDisable()
            {
                InstanceTracker.Remove(this);
            }
        }

        public override bool OnUse(EquipmentSlot equipmentSlot)
        {
            MysticsRisky2UtilsEquipmentTarget targetInfo = equipmentSlot.GetComponent<MysticsRisky2UtilsEquipmentTarget>();
            if (targetInfo)
            {
                if (targetInfo.obj)
                {
                    PurchaseInteraction purchaseInteraction = targetInfo.obj.GetComponent<MysticsItemsChestLocator>().purchaseInteraction;
                    purchaseInteraction.Networkcost = 0;

                    if (equipmentSlot.characterBody)
                    {
                        Interactor component = equipmentSlot.characterBody.GetComponent<Interactor>();
                        if (component)
                        {
                            component.AttemptInteraction(targetInfo.obj);
                        }
                    }

                    var component2 = equipmentSlot.GetComponent<MysticsItemsOmarHackToolBehaviour>();
                    if (component2 && component2.usesLeft > 0) component2.usesLeft--;

                    EffectManager.SpawnEffect(hackVFXPrefab, new EffectData
                    {
                        origin = targetInfo.obj.transform.position + Vector3.up * 0.5f,
                        scale = 3f
                    }, true);

                    targetInfo.Invalidate();

                    return true;
                }
            }
            return false;
        }

        public class ChestSearch : BaseDirectionalSearch<MysticsItemsChestLocator, ChestSearchSelector, ChestSearchFilter>
        {
            public ChestSearch() : base(default(ChestSearchSelector), default(ChestSearchFilter))
            {
            }

            public ChestSearch(ChestSearchSelector selector, ChestSearchFilter candidateFilter) : base(selector, candidateFilter)
            {
            }
        }

        public struct ChestSearchSelector : IGenericWorldSearchSelector<MysticsItemsChestLocator>
        {
            public Transform GetTransform(MysticsItemsChestLocator source)
            {
                if (source.hologramProjector && source.hologramProjector.hologramPivot)
                {
                    return source.hologramProjector.hologramPivot;
                }
                return source.childTransform;
            }

            public GameObject GetRootObject(MysticsItemsChestLocator source)
            {
                return source.gameObject;
            }
        }

        public struct ChestSearchFilter : IGenericDirectionalSearchFilter<MysticsItemsChestLocator>
        {
            public bool PassesFilter(MysticsItemsChestLocator source)
            {
                return source.purchaseInteraction.available && source.chestBehavior && source.purchaseInteraction.cost > 0;
            }
        }

        public class MysticsItemsChestLocator : MonoBehaviour
        {
            public PurchaseInteraction purchaseInteraction;
            public ChestBehavior chestBehavior;
            public Transform childTransform;
            public RoR2.Hologram.HologramProjector hologramProjector;

            public void Awake()
            {
                purchaseInteraction = gameObject.GetComponent<PurchaseInteraction>();
                chestBehavior = gameObject.GetComponent<ChestBehavior>();
                hologramProjector = gameObject.GetComponent<RoR2.Hologram.HologramProjector>();
            }

            public void OnEnable()
            {
                InstanceTracker.Add<MysticsItemsChestLocator>(this);
            }

            public void OnDisable()
            {
                InstanceTracker.Remove<MysticsItemsChestLocator>(this);
            }
        }

        public class MysticsItemsOmarHackToolHUD : MonoBehaviour
        {
            public static void RefreshAll()
            {
                foreach (var hudInstance in HUD.readOnlyInstanceList) RefreshForHUDInstance(hudInstance);
            }

            public static void RefreshForHUDInstance(HUD hudInstance)
            {
                CharacterMaster targetMaster = hudInstance.targetMaster;
                CharacterBody characterBody = hudInstance.targetBodyObject ? hudInstance.targetBodyObject.GetComponent<CharacterBody>() : null;
                EquipmentSlot equipmentSlot = characterBody ? characterBody.equipmentSlot : null;

                var shouldDisplay = equipmentSlot ? equipmentSlot.equipmentIndex == MysticsItemsContent.Equipment.MysticsItems_OmarHackTool.equipmentIndex : false;

                MysticsItemsOmarHackToolHUD targetIndicatorInstance = instancesList.FirstOrDefault(x => x.hud == hudInstance);

                if (targetIndicatorInstance != shouldDisplay)
                {
                    if (!targetIndicatorInstance)
                    {
                        if (hudInstance.mainUIPanel)
                        {
                            var transform = (RectTransform)hudInstance.mainUIPanel.transform.Find("SpringCanvas/BottomRightCluster/Scaler/EquipmentSlot/DisplayRoot");
                            if (transform)
                            {
                                targetIndicatorInstance = Instantiate(hudPrefab, transform).GetComponent<MysticsItemsOmarHackToolHUD>();
                                targetIndicatorInstance.hud = hudInstance;
                            }
                        }
                    }
                    else
                    {
                        Destroy(targetIndicatorInstance);
                    }
                }

                if (shouldDisplay)
                {
                    targetIndicatorInstance.equipmentSlot = equipmentSlot;
                    targetIndicatorInstance.equipmentBehaviour = equipmentSlot.GetComponent<MysticsItemsOmarHackToolBehaviour>();
                    targetIndicatorInstance.UpdateText();
                }
            }

            public static List<MysticsItemsOmarHackToolHUD> instancesList = new List<MysticsItemsOmarHackToolHUD>();

            public void Update()
            {
                
            }

            public void UpdateText()
            {
                if (equipmentBehaviour)
                {
                    if (usesLeftText)
                    {
                        usesLeftText.text = equipmentBehaviour.usesLeft + "/" + equipmentBehaviour.maxUses;
                    }
                }
            }

            public void OnEnable()
            {
                instancesList.Add(this);
            }

            public void OnDisable()
            {
                instancesList.Remove(this);
            }

            public HUD hud;
            public EquipmentSlot equipmentSlot;
            public MysticsItemsOmarHackToolBehaviour equipmentBehaviour;
            public TextMeshProUGUI usesLeftText;
        }
    }
}
