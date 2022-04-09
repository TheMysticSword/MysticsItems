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
        public static GameObject hackOverlayPrefab;

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
            equipmentDef.canBeRandomlyTriggered = new ConfigurableCanBeRandomlyTriggeredBool("Equipment: From Omar With Love", false).Value;
            equipmentDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/From Omar With Love/Model.prefab"));
            equipmentDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Equipment/From Omar With Love/Icon.png");

            ModelPanelParameters modelPanelParameters = equipmentDef.pickupModelPrefab.GetComponent<ModelPanelParameters>();
            modelPanelParameters.minDistance = 3f;
            modelPanelParameters.maxDistance = 15f;

            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(equipmentDef.pickupModelPrefab, equipmentDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Stomach", new Vector3(0.07132F, 0.06612F, 0.16335F), new Vector3(356.4405F, 183.4027F, 175.2333F), new Vector3(0.05273F, 0.05273F, 0.05273F));
                AddDisplayRule("HuntressBody", "Pelvis", new Vector3(0.16212F, -0.06929F, -0.04928F), new Vector3(346.9094F, 305.4124F, 344.1372F), new Vector3(0.05129F, 0.05129F, 0.05129F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(-0.08026F, 0.03366F, -0.14135F), new Vector3(25.90035F, 4.31119F, 171.9758F), new Vector3(0.05568F, 0.05568F, 0.05568F));
                AddDisplayRule("ToolbotBody", "Head", new Vector3(2.44491F, 1.16734F, 0.31814F), new Vector3(327.4072F, -0.00001F, 88.11515F), new Vector3(0.88852F, 0.88852F, 0.88852F));
                AddDisplayRule("EngiBody", "LowerArmL", new Vector3(0.00352F, 0.24612F, -0.05998F), new Vector3(0.32021F, 358.9018F, 358.6807F), new Vector3(0.05067F, 0.05067F, 0.05067F));
                AddDisplayRule("MageBody", "Pelvis", new Vector3(-0.17887F, -0.07933F, -0.17931F), new Vector3(357.9544F, 233.4202F, 346.881F), new Vector3(0.04982F, 0.04982F, 0.04982F));
                AddDisplayRule("MercBody", "Pelvis", new Vector3(0.1129F, 0.0254F, -0.17644F), new Vector3(13.57408F, 151.9048F, 358.9593F), new Vector3(0.05206F, 0.05206F, 0.05206F));
                AddDisplayRule("TreebotBody", "WeaponPlatformEnd", new Vector3(0.00002F, 0.09097F, 0.24047F), new Vector3(0F, 0F, 0F), new Vector3(0.11139F, 0.11139F, 0.11139F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(0.23928F, -0.07178F, 0.33048F), new Vector3(356.9285F, 91.33677F, 183.2036F), new Vector3(0.06152F, 0.06152F, 0.06152F));
                AddDisplayRule("CrocoBody", "LowerArmR", new Vector3(0.59287F, 4.87558F, 0.47809F), new Vector3(359.8608F, 58.79917F, 8.98919F), new Vector3(0.55777F, 0.55777F, 0.55777F));
                AddDisplayRule("CaptainBody", "Stomach", new Vector3(0.15397F, 0.13966F, 0.16991F), new Vector3(356.0198F, 17.26032F, 189.3092F), new Vector3(0.05578F, 0.05578F, 0.05578F));
                AddDisplayRule("ScavBody", "MuzzleEnergyCannon", new Vector3(4.83208F, -2.97995F, -10.98303F), new Vector3(0F, 0F, 56.95893F), new Vector3(1.363F, 1.363F, 1.363F));
                AddDisplayRule("EquipmentDroneBody", "HeadCenter", new Vector3(0.92365F, 0F, -0.60534F), new Vector3(270F, 270F, 0F), new Vector3(0.32166F, 0.32166F, 0.32166F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Pelvis", new Vector3(0.15563F, 0.20335F, 0.15945F), new Vector3(358.1388F, 35.73481F, 190.192F), new Vector3(0.053F, 0.053F, 0.053F));
            };

            crosshairPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/WoodSpriteIndicator"), "MysticsItems_OmarHackToolIndicator", false);
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
                var purchaseInteraction = self.GetComponent<PurchaseInteraction>();
                if (purchaseInteraction && purchaseInteraction.costType == CostTypeIndex.Money && (self.displayNameToken.Contains("CHEST") || self.displayNameToken.Contains("SHRINE")))
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

            On.RoR2.EquipmentSlot.Start += EquipmentSlot_Start;
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
            effectComponent.soundName = "MysticsItems_Play_env_OmarHackTool";
            VFXAttributes vfxAttributes = hackVFXPrefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            hackVFXPrefab.AddComponent<DestroyOnTimer>().duration = 10f;
            MysticsItemsContent.Resources.effectPrefabs.Add(hackVFXPrefab);

            On.RoR2.ChestBehavior.ItemDrop += ChestBehavior_ItemDrop;

            hackOverlayPrefab = PrefabAPI.InstantiateClone(new GameObject(), "MysticsItems_OmarHackToolHackOverlay", false);
            EntityStateMachine entityStateMachine = hackOverlayPrefab.AddComponent<EntityStateMachine>();
            entityStateMachine.initialStateType = entityStateMachine.mainStateType = new EntityStates.SerializableEntityStateType(typeof(MysticsItemsOmarHackToolOverlay));
            effectComponent = hackOverlayPrefab.AddComponent<EffectComponent>();
            effectComponent.parentToReferencedTransform = true;
            vfxAttributes = hackOverlayPrefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Low;
            MysticsItemsContent.Resources.effectPrefabs.Add(hackOverlayPrefab);
        }

        private void ChestBehavior_ItemDrop(On.RoR2.ChestBehavior.orig_ItemDrop orig, ChestBehavior self)
        {
            if (NetworkServer.active && self.dropPickup != PickupIndex.none)
            {
                var component = self.GetComponent<MysticsItemsOmarHackToolHackingManager>();
                if (component)
                {
                    component.PlayFX(3f);
                }
            }
            orig(self);
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

        private void EquipmentSlot_Start(On.RoR2.EquipmentSlot.orig_Start orig, EquipmentSlot self)
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
                    var hackingManager = targetInfo.obj.AddComponent<MysticsItemsOmarHackToolHackingManager>();
                    hackingManager.purchaseInteraction = targetInfo.obj.GetComponent<MysticsItemsChestLocator>().purchaseInteraction;
                    if (equipmentSlot.characterBody)
                    {
                        hackingManager.interactor = equipmentSlot.characterBody.GetComponent<Interactor>();
                    }
                    if (hackingManager.GetComponent<ChestBehavior>())
                    {
                        hackingManager.delay = 0.1f;
                    }
                    else
                    {
                        hackingManager.purchaseInteraction.SetAvailable(false);
                    }

                    EffectManager.SpawnEffect(hackOverlayPrefab, new EffectData
                    {
                        rootObject = hackingManager.gameObject
                    }, true);

                    var component2 = equipmentSlot.GetComponent<MysticsItemsOmarHackToolBehaviour>();
                    if (component2 && component2.usesLeft > 0) component2.usesLeft--;

                    targetInfo.Invalidate();

                    equipmentSlot.subcooldownTimer = 0.5f;
                    return true;
                }
            }
            return false;
        }

        public override void OnUseClient(EquipmentSlot equipmentSlot)
        {
            base.OnUseClient(equipmentSlot);
            Util.PlaySound("MysticsItems_Play_item_use_OmarHackTool", equipmentSlot.gameObject);
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
                return source.purchaseInteraction.available && source.purchaseInteraction.cost > 0;
            }
        }

        public class MysticsItemsChestLocator : MonoBehaviour
        {
            public PurchaseInteraction purchaseInteraction;
            public Transform childTransform;
            public RoR2.Hologram.HologramProjector hologramProjector;

            public void Awake()
            {
                purchaseInteraction = gameObject.GetComponent<PurchaseInteraction>();
                hologramProjector = gameObject.GetComponentInChildren<RoR2.Hologram.HologramProjector>();
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
                EquipmentIndex equipmentIndex = characterBody && characterBody.inventory ? characterBody.inventory.currentEquipmentIndex : EquipmentIndex.None;

                var ei = MysticsItemsContent.Equipment.MysticsItems_OmarHackTool.equipmentIndex;
                var shouldDisplay = equipmentSlot ? (equipmentSlot.equipmentIndex == ei || equipmentIndex == ei) : false;

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
                        Destroy(targetIndicatorInstance.gameObject);
                    }
                }

                if (shouldDisplay && targetIndicatorInstance)
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

        public class MysticsItemsOmarHackToolHackingManager : MonoBehaviour
        {
            public bool fxPlayed = false;
            public float delay = 0.8f;
            public PurchaseInteraction purchaseInteraction;
            public Interactor interactor;

            public void PlayFX(float fxScale)
            {
                if (!fxPlayed)
                {
                    fxPlayed = true;
                    EffectManager.SpawnEffect(hackVFXPrefab, new EffectData
                    {
                        origin = transform.position + Vector3.up * 0.5f,
                        scale = fxScale
                    }, true);
                }
            }

            public void FixedUpdate()
            {
                if (delay > 0f)
                {
                    delay -= Time.fixedDeltaTime;
                    if (delay <= 0f)
                    {
                        purchaseInteraction.Networkcost = 0;
                        purchaseInteraction.SetAvailable(true);
                        if (interactor)
                        {
                            interactor.AttemptInteraction(purchaseInteraction.gameObject);
                        }
                    }
                }
            }
        }

        public class MysticsItemsOmarHackToolOverlay : EntityStates.EntityState
        {
            public bool started = false;
            public bool ended = false;

            public TemporaryOverlay temporaryOverlay;
            public Material materialInstance;
            public new ModelLocator modelLocator;
            public Renderer renderer;
            public float duration = 0.8f;

            public override void OnEnter()
            {
                base.OnEnter();
            }

            public override void Update()
            {
                base.Update();

                if (!started)
                {
                    var parent = transform.parent;
                    if (parent)
                    {
                        started = true;

                        modelLocator = parent.GetComponent<ModelLocator>();
                        if (modelLocator && modelLocator.modelTransform)
                        {
                            temporaryOverlay = modelLocator.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                            temporaryOverlay.duration = duration;
                            temporaryOverlay.originalMaterial = Main.AssetBundle.LoadAsset<Material>("Assets/Equipment/From Omar With Love/matOmarHackToolVFXOverlay.mat");
                            //temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 0f, 0.2f, 1f);
                            //temporaryOverlay.animateShaderAlpha = true;
                            temporaryOverlay.destroyComponentOnEnd = false;
                            temporaryOverlay.destroyObjectOnEnd = false;
                            temporaryOverlay.SetupMaterial();
                            
                            renderer = modelLocator.modelTransform.GetComponentInChildren<Renderer>();
                            if (renderer)
                            {
                                var materials = renderer.materials;
                                HG.ArrayUtils.ArrayAppend(ref materials, temporaryOverlay.materialInstance);
                                renderer.materials = materials;
                                materialInstance = renderer.materials.Last();
                            }
                        }
                    }
                }

                if (age >= duration && !ended)
                {
                    ended = true;

                    if (renderer && materialInstance)
                    {
                        var materials = renderer.materials;
                        var index = System.Array.IndexOf(materials, materialInstance);
                        if (index != -1)
                        {
                            HG.ArrayUtils.ArrayRemoveAtAndResize(ref materials, index);
                        }
                        renderer.materials = materials;
                    }
                }
            }
        }
    }
}
