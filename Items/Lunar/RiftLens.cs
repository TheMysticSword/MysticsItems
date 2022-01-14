using RoR2;
using RoR2.Navigation;
using R2API;
using R2API.Utils;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using RoR2.UI;
using UnityEngine.Rendering.PostProcessing;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using UnityEngine.UI;
using static MysticsItems.BalanceConfigManager;

namespace MysticsItems.Items
{
    public class RiftLens : BaseItem
    {
        public static GameObject riftChest;
        public static InteractableSpawnCard riftChestSpawnCard;
        public static CostTypeIndex riftLensDebuffCostType;

        public static GameObject hudPanelPrefab;
        public static GameObject riftPositionIndicator;

        public static ConfigurableValue<int> baseRifts = new ConfigurableValue<int>(
            "Item: Rift Lens",
            "BaseRifts",
            3,
            "How many rifts should spawn",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_RIFTLENS_DESC"
            }
        );
        public static ConfigurableValue<int> riftsPerStack = new ConfigurableValue<int>(
            "Item: Rift Lens",
            "RiftsPerStack",
            3,
            "How many rifts should spawn for each additional stack of this item",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_RIFTLENS_DESC"
            }
        );

        public override void OnPluginAwake()
        {
            riftChest = PrefabAPI.InstantiateClone(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Rift Lens/UnstableRift.prefab"), "MysticsItems_UnstableRift", false);
            riftChest.AddComponent<NetworkIdentity>();
            PrefabAPI.RegisterNetworkPrefab(riftChest);

            riftPositionIndicator = PrefabAPI.InstantiateClone(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Rift Lens/UnstableRiftPositionIndicator.prefab"), "MysticsItems_UnstableRiftPositionIndicator", false);

            OnRiftLensCostTypeRegister += (costTypeIndex) =>
            {
                riftChest.GetComponent<PurchaseInteraction>().costType = costTypeIndex;
                riftChest.GetComponent<PurchaseInteraction>().cost = 1;
            };

            //add a custom purchase cost type - we will require the interactor pay with the debuff so that players
            //without the debuff can't help them open chests faster
            CostTypeDef costTypeDef = new CostTypeDef();
            costTypeDef.costStringFormatToken = "COST_MYSTICSITEMS_RIFTLENSDEBUFF_FORMAT";
            costTypeDef.isAffordable = delegate (CostTypeDef costTypeDef2, CostTypeDef.IsAffordableContext context)
            {
                CharacterBody body = context.activator.gameObject.GetComponent<CharacterBody>();
                if (body)
                {
                    Inventory inventory = body.inventory;
                    return inventory ? inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_RiftLensDebuff) > 0 : false;
                }
                return false;
            };
            costTypeDef.payCost = delegate (CostTypeDef costTypeDef2, CostTypeDef.PayCostContext context)
            {
                CharacterBody body = context.activator.gameObject.GetComponent<CharacterBody>();
                if (body)
                {
                    Inventory inventory = body.inventory;
                    if (inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_RiftLensDebuff) > 0) inventory.RemoveItem(MysticsItemsContent.Items.MysticsItems_RiftLensDebuff);
                }
            };
            costTypeDef.colorIndex = ColorCatalog.ColorIndex.LunarItem;
            CostTypeCreation.CreateCostType(new CostTypeCreation.CustomCostTypeInfo
            {
                costTypeDef = costTypeDef,
                onRegister = OnRiftLensCostTypeRegister
            });

            NetworkingAPI.RegisterMessageType<MysticsItemsRiftChest.SyncDestroyThingsOnOpen>();
            NetworkingAPI.RegisterMessageType<MysticsItemsRiftLensBehaviour.SyncMaxCountdown>();
        }

        public static System.Action<CostTypeIndex> OnRiftLensCostTypeRegister;

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_RiftLens";
            itemDef.tier = ItemTier.Lunar;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy
            };
            MysticsItemsContent.Resources.unlockableDefs.Add(GetUnlockableDef());
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Rift Lens/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Rift Lens/Icon.png");
            ModelPanelParameters modelPanelParams = itemDef.pickupModelPrefab.GetComponentInChildren<ModelPanelParameters>();
            modelPanelParams.minDistance = 2;
            modelPanelParams.maxDistance = 6;
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Head", new Vector3(0.1f, 0.25f, 0.15f), new Vector3(20f, 210f, 0f), new Vector3(0.06f, 0.06f, 0.06f));
                AddDisplayRule("HuntressBody", "Head", new Vector3(-0.0009F, 0.2635F, 0.1117F), new Vector3(0F, 180F, 0F), new Vector3(0.03F, 0.03F, 0.03F));
                AddDisplayRule("Bandit2Body", "Head", new Vector3(0F, 0.057F, 0.135F), new Vector3(0F, 180F, 180F), new Vector3(0.028F, 0.028F, 0.028F));
                AddDisplayRule("ToolbotBody", "Head", new Vector3(0.409F, 3.049F, -1.067F), new Vector3(60F, 0F, 180F), new Vector3(0.3F, 0.3F, 0.3F));
                AddDisplayRule("EngiBody", "HeadCenter", new Vector3(0.098F, 0.019F, 0.127F), new Vector3(1.506F, 213.327F, 354.045F), new Vector3(0.029F, 0.029F, 0.029F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.005F, 0.525F, 2.043F), new Vector3(0F, 180F, 0F), new Vector3(0.108F, 0.083F, 0.083F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.006F, 0.774F, 0.853F), new Vector3(0F, 177.859F, 0F), new Vector3(0.306F, 0.306F, 0.306F));
                AddDisplayRule("MageBody", "Head", new Vector3(0.048F, 0.06F, 0.117F), new Vector3(13.941F, 189.822F, 2.364F), new Vector3(0.026F, 0.026F, 0.026F));
                AddDisplayRule("MercBody", "Head", new Vector3(0.05F, 0.156F, 0.151F), new Vector3(10.716F, 202.078F, 355.897F), new Vector3(0.053F, 0.053F, 0.053F));
                AddDisplayRule("TreebotBody", "HeadCenter", new Vector3(-0.005F, 0.058F, -0.002F), new Vector3(85.226F, 270F, 270F), new Vector3(0.098F, 0.098F, 0.098F));
                AddDisplayRule("LoaderBody", "Head", new Vector3(0.051F, 0.125F, 0.134F), new Vector3(10.267F, 205.465F, 354.736F), new Vector3(0.047F, 0.04F, 0.048F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(-1.531F, 1.934F, 0.459F), new Vector3(14.526F, 104.513F, 346.531F), new Vector3(0.236F, 0.236F, 0.236F));
                AddDisplayRule("CaptainBody", "HandR", new Vector3(-0.085F, 0.108F, 0.013F), new Vector3(69.075F, 70.114F, 350.542F), new Vector3(0.026F, 0.03F, 0.042F));
                AddDisplayRule("BrotherBody", "Head", BrotherInfection.blue, new Vector3(0.003F, -0.01F, 0.061F), new Vector3(349.888F, 70.121F, 339.729F), new Vector3(0.133F, 0.133F, 0.133F));
                AddDisplayRule("ScavBody", "Head", new Vector3(5.068F, 4.15F, -0.55F), new Vector3(46.576F, 301.45F, 310.155F), new Vector3(1.363F, 1.363F, 1.363F));
            };

            MysticsItemsRiftChest component = riftChest.AddComponent<MysticsItemsRiftChest>();
            
            SfxLocator sfxLocator = riftChest.AddComponent<SfxLocator>();
            sfxLocator.openSound = "Play_env_riftchest_open";
            
            riftChest.AddComponent<GenericDisplayNameProvider>().displayToken = "MYSTICSITEMS_RIFTCHEST_NAME";

            PurchaseInteraction purchaseInteraction = riftChest.AddComponent<PurchaseInteraction>();
            purchaseInteraction.displayNameToken = "MYSTICSITEMS_RIFTCHEST_NAME";
            purchaseInteraction.contextToken = "MYSTICSITEMS_RIFTCHEST_CONTEXT";

            ChestBehavior chestBehavior = riftChest.AddComponent<ChestBehavior>();
            chestBehavior.tier1Chance = 80f;
            chestBehavior.tier2Chance = 20f;
            chestBehavior.tier3Chance = 1f;

            riftChest.transform.Find("InteractionCollider").gameObject.AddComponent<EntityLocator>().entity = riftChest;

            riftChest.transform.Find("RiftOrigin/Sprite").gameObject.AddComponent<Billboard>();

            ObjectScaleCurve objectScaleCurve = riftChest.transform.Find("RiftOrigin").gameObject.AddComponent<ObjectScaleCurve>();
            objectScaleCurve.useOverallCurveOnly = true;
            objectScaleCurve.overallCurve = new AnimationCurve()
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(1f, 0f)
                }
            };
            objectScaleCurve.timeMax = 0.5f;
            objectScaleCurve.enabled = false;

            //post processing
            GameObject ppHolder = Object.Instantiate(PrefabAPI.InstantiateClone(new GameObject("RiftLensPostProcessing"), "RiftLensPostProcessing", false), riftChest.transform);
            ppHolder.layer = LayerIndex.postProcess.intVal;
            PostProcessVolume pp = ppHolder.AddComponent<PostProcessVolume>();
            pp.isGlobal = false;
            pp.weight = 1f;
            pp.priority = 50;
            pp.blendDistance = 10f;
            SphereCollider sphereCollider = ppHolder.AddComponent<SphereCollider>();
            sphereCollider.radius = 5f;
            sphereCollider.isTrigger = true;
            PostProcessProfile ppProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
            ppProfile.name = "ppRiftLens";
            LensDistortion lensDistortion = ppProfile.AddSettings<LensDistortion>();
            lensDistortion.SetAllOverridesTo(true);
            lensDistortion.intensity.value = -30f;
            lensDistortion.scale.value = 1f;
            ColorGrading colorGrading = ppProfile.AddSettings<ColorGrading>();
            colorGrading.colorFilter.value = new Color32(178, 242, 255, 255);
            colorGrading.colorFilter.overrideState = true;
            pp.sharedProfile = ppProfile;
            PostProcessDuration ppDuration = pp.gameObject.AddComponent<PostProcessDuration>();
            ppDuration.ppVolume = pp;
            ppDuration.ppWeightCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 1f, 0f, Mathf.Tan(-45f * Mathf.Deg2Rad)),
                    new Keyframe(1f, 0f, Mathf.Tan(135f * Mathf.Deg2Rad), 0f)
                },
                preWrapMode = WrapMode.Clamp,
                postWrapMode = WrapMode.Clamp
            };
            ppDuration.maxDuration = 1;
            ppDuration.destroyOnEnd = true;
            ppDuration.enabled = false;
            component.ppDuration = ppDuration;

            riftChestSpawnCard = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            riftChestSpawnCard.name = "iscMysticsItems_UnstableRift";
            riftChestSpawnCard.directorCreditCost = 0;
            riftChestSpawnCard.forbiddenFlags = NodeFlags.NoChestSpawn;
            riftChestSpawnCard.hullSize = HullClassification.Human;
            riftChestSpawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
            riftChestSpawnCard.occupyPosition = true;
            riftChestSpawnCard.orientToFloor = false;
            riftChestSpawnCard.sendOverNetwork = true;
            riftChestSpawnCard.prefab = riftChest;

            GenericGameEvents.OnPopulateScene += (rng) =>
            {
                int riftsToSpawn = 0;
                foreach (CharacterMaster characterMaster in CharacterMaster.readOnlyInstancesList)
                    if (characterMaster.teamIndex == TeamIndex.Player)
                    {
                        int thisItemCount = characterMaster.inventory.GetItemCount(itemDef);
                        if (thisItemCount > 0)
                        {
                            characterMaster.inventory.RemoveItem(MysticsItemsContent.Items.MysticsItems_RiftLensDebuff, characterMaster.inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_RiftLensDebuff));
                            characterMaster.inventory.GiveItem(MysticsItemsContent.Items.MysticsItems_RiftLensDebuff, thisItemCount * riftsPerStack);
                            riftsToSpawn += baseRifts.Value + thisItemCount * (riftsPerStack.Value - 1);
                        }
                    }
                if (riftsToSpawn > 0)
                {
                    for (int i = 0; i < riftsToSpawn; i++)
                    {
                        GameObject riftChest = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(riftChestSpawnCard, new DirectorPlacementRule
                        {
                            placementMode = DirectorPlacementRule.PlacementMode.Random
                        }, rng));
                    }
                }
                MysticsItemsRiftLensBehaviour.RecalculateMaxCountdown();
            };

            On.RoR2.CharacterBody.OnInventoryChanged += (orig, self) =>
            {
                orig(self);
                self.AddItemBehavior<MysticsItemsRiftLensBehaviour>(self.inventory.GetItemCount(itemDef));
            };

            ObjectivePanelController.collectObjectiveSources += ObjectivePanelController_collectObjectiveSources;

            hudPanelPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/UI/HudModules/HudCountdownPanel"), "RiftLensHUDPanel");
            hudPanelPrefab.transform.Find("Juice/Container/CountdownTitleLabel").GetComponent<LanguageTextMeshController>().token = "OBJECTIVE_MYSTICSITEMS_RIFTLENS_FLAVOUR";
            var col = new Color32(0, 157, 255, 255);
            hudPanelPrefab.transform.Find("Juice/Container/Border").GetComponent<Image>().color = col;
            hudPanelPrefab.transform.Find("Juice/Container/CountdownLabel").GetComponent<HGTextMeshProUGUI>().color = col;

            PositionIndicator positionIndicator = riftPositionIndicator.AddComponent<PositionIndicator>();
            positionIndicator.insideViewObject = riftPositionIndicator.transform.Find("InsideFrame").gameObject;
            positionIndicator.outsideViewObject = riftPositionIndicator.transform.Find("OutsideFrame").gameObject;
            positionIndicator.alwaysVisibleObject = riftPositionIndicator.transform.Find("Sprite").gameObject;
            positionIndicator.shouldRotateOutsideViewObject = true;
            positionIndicator.outsideViewRotationOffset = 90f;

            GenericGameEvents.OnPlayerCharacterDeath += GenericGameEvents_OnPlayerCharacterDeath;
        }

        public static string[] riftDeathQuoteTokens = (from i in Enumerable.Range(0, 5) select "PLAYER_DEATH_QUOTE_MYSTICSITEMS_RIFTLENS_" + TextSerialization.ToStringInvariant(i)).ToArray();
        
        public void GenericGameEvents_OnPlayerCharacterDeath(DamageReport damageReport, ref string deathQuote)
        {
            if (damageReport.victimBody)
            {
                MysticsItemsRiftLensBehaviour component = damageReport.victimBody.GetComponent<MysticsItemsRiftLensBehaviour>();
                if (component && component.diedFromTimer)
                {
                    deathQuote = riftDeathQuoteTokens[Random.Range(0, riftDeathQuoteTokens.Length)];
                }
            }
        }

        public static void ObjectivePanelController_collectObjectiveSources(CharacterMaster master, List<ObjectivePanelController.ObjectiveSourceDescriptor> objectiveSourcesList)
        {
            foreach (MysticsItemsRiftLensBehaviour component in InstanceTracker.GetInstancesList<MysticsItemsRiftLensBehaviour>())
            {
                if (component.stack > 0 && component.body && component.riftsLeft > 0 && LocalUserManager.readOnlyLocalUsersList.Any(x => x.cachedBody == component.body))
                {
                    objectiveSourcesList.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
                    {
                        master = master,
                        objectiveType = typeof(RiftLensObjectiveTracker),
                        source = component
                    });
                }
            }
        }

        public class RiftLensObjectiveTracker : ObjectivePanelController.ObjectiveTracker
        {
            public override string GenerateString()
            {
                MysticsItemsRiftLensBehaviour component = (MysticsItemsRiftLensBehaviour)sourceDescriptor.source;
                riftsLeft = component.riftsLeft;
                return string.Format(Language.GetString("OBJECTIVE_MYSTICSITEMS_RIFTLENS_CLOSE_RIFTS"), riftsLeft, component.riftsTotal);
            }

            public override bool IsDirty()
            {
                return ((MysticsItemsRiftLensBehaviour)sourceDescriptor.source).riftsLeft != riftsLeft;
            }

            public int riftsLeft = -1;
        }

        public class MysticsItemsRiftLensBehaviour : CharacterBody.ItemBehavior
        {
            public int riftsLeft = 0;
            public int riftsTotal = 0;
            public float countdownTimer = 0;
            public bool diedFromTimer = false;

            public bool nearNodes = false;
            public float nearNodeCheckTimer = 0;
            public float nearNodeCheckDuration = 1f;

            public Dictionary<HUD, GameObject> hudPanels = new Dictionary<HUD, GameObject>();

            public bool countdown10Played = false;
            public uint countdown10ID;

            public void Start()
            {
                body.onInventoryChanged += Body_onInventoryChanged;
                UpdateItemBasedInfo();
                countdownTimer = maxCountdown;
                diedFromTimer = false;
                nearNodes = true;
            }

            public static float maxCountdown = 150f;

            public static void RecalculateMaxCountdown()
            {
                if (!NetworkServer.active) return;

                // calculate the shortest distance between all rifts
                var distanceBetweenAllRifts = 0f;
                var rifts = InstanceTracker.GetInstancesList<MysticsItemsRiftChest>();
                while (rifts.Count > 1) // we don't do "while (rifts.Count > 0)" because we can't measure the distance to the next rift if there's only one left
                {
                    var rift = rifts[0];
                    rifts.RemoveAt(0);
                    var pos = rift.transform.position;
                    var dist = Mathf.Infinity;

                    // find the distance to the closest rift
                    foreach (var rift2 in rifts)
                    {
                        var dist2 = Vector3.Distance(pos, rift2.transform.position);
                        if (dist2 < dist)
                        {
                            dist = dist2;
                        }
                    }

                    distanceBetweenAllRifts += dist;
                }

                // calculate the shortest distance to the nearest rift for each player...
                var distanceToNearestRiftForEachPlayer = new List<float>();
                foreach (var instance in InstanceTracker.GetInstancesList<MysticsItemsRiftLensBehaviour>())
                {
                    var pos = instance.body ? instance.body.corePosition : instance.transform.position;
                    var dist = Mathf.Infinity;

                    foreach (var rift in InstanceTracker.GetInstancesList<MysticsItemsRiftChest>())
                    {
                        var dist2 = Vector3.Distance(pos, rift.transform.position);
                        if (dist2 < dist)
                        {
                            dist = dist2;
                        }
                    }

                    distanceToNearestRiftForEachPlayer.Add(dist);
                }

                // and pick the longest one out of them to compensate for the unluckiest player
                var distanceToNearestRift = distanceToNearestRiftForEachPlayer.Count > 0 ? distanceToNearestRiftForEachPlayer.Max() : 1f;

                var totalDistance = distanceToNearestRift + distanceBetweenAllRifts;
                var averageWalkSpeed = 7f;
                // players can't always go through the shortest path due to terrain and gravity, so we'll add bonus time
                var timeBonusMultiplier = 3f;
                var timeBonusFlat = 10f;
                var calculatedCountdownTime = (totalDistance / averageWalkSpeed) * timeBonusMultiplier + timeBonusFlat;

                maxCountdown = calculatedCountdownTime;
                new SyncMaxCountdown(calculatedCountdownTime).Send(NetworkDestination.Clients);
                foreach (var instance in InstanceTracker.GetInstancesList<MysticsItemsRiftLensBehaviour>())
                {
                    instance.countdownTimer = maxCountdown;
                }
            }

            public class SyncMaxCountdown : INetMessage
            {
                float maxCountdown;

                public SyncMaxCountdown()
                {
                }

                public SyncMaxCountdown(float maxCountdown)
                {
                    this.maxCountdown = maxCountdown;
                }

                public void Deserialize(NetworkReader reader)
                {
                    maxCountdown = reader.ReadSingle();
                }

                public void OnReceived()
                {
                    var difference = maxCountdown - MysticsItemsRiftLensBehaviour.maxCountdown;
                    MysticsItemsRiftLensBehaviour.maxCountdown = maxCountdown;
                    foreach (var instance in InstanceTracker.GetInstancesList<MysticsItemsRiftLensBehaviour>())
                    {
                        instance.countdownTimer += difference;
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(maxCountdown);
                }
            }

            public void Update()
            {
                if (!nearNodes)
                {
                    nearNodeCheckTimer -= Time.deltaTime;
                    if (nearNodeCheckTimer <= 0)
                    {
                        nearNodeCheckTimer = nearNodeCheckDuration;
                        if (SceneInfo.instance.GetNodeGraph(MapNodeGroup.GraphType.Ground).FindNodesInRangeWithFlagConditions(body.corePosition, 0f, 20f, HullMask.Human, NodeFlags.None, NodeFlags.NoCharacterSpawn, true).Count > 0)
                        {
                            nearNodes = true;
                        }
                    }
                }
                else {
                    if (riftsLeft > 0)
                    {
                        countdownTimer -= Time.deltaTime;

                        if (!diedFromTimer)
                        {
                            foreach (HUD hud in HUD.readOnlyInstanceList)
                            {
                                SetHudCountdownEnabled(hud, hud.targetBodyObject);
                            }
                            SetCountdownTime(countdownTimer);
                        }
                        else
                        {
                            foreach (HUD hud in HUD.readOnlyInstanceList)
                            {
                                SetHudCountdownEnabled(hud, false);
                            }
                        }

                        if (countdownTimer <= 10f && !countdown10Played)
                        {
                            countdown10Played = true;
                            countdown10ID = Util.PlaySound("MysticsItems_Play_riftLens_countdown_10", body.gameObject);
                        }

                        if (countdownTimer <= 0 && !diedFromTimer)
                        {
                            diedFromTimer = true;
                            if (NetworkServer.active) body.healthComponent.Suicide();
                        }
                    }
                    else
                    {
                        foreach (HUD hud in HUD.readOnlyInstanceList)
                        {
                            SetHudCountdownEnabled(hud, false);
                        }
                        if (countdown10Played)
                        {
                            countdown10Played = false;
                            AkSoundEngine.StopPlayingID(countdown10ID);
                        }
                    }
                }
            }

            public void OnDestroy()
            {
                if (body) body.onInventoryChanged -= Body_onInventoryChanged;
                AkSoundEngine.StopPlayingID(countdown10ID);
            }

            public void Body_onInventoryChanged()
            {
                UpdateItemBasedInfo();
            }

            public void UpdateItemBasedInfo()
            {
                if (!body) return;
                riftsLeft = body.inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_RiftLensDebuff);
                riftsTotal = Mathf.Max(stack * riftsPerStack, riftsTotal); // the Max call is here to avoid riftsTotal becoming less than riftsLeft in case the player removes a lens before opening all chests
            }

            public void SetHudCountdownEnabled(HUD hud, bool shouldEnableCountdownPanel)
            {
                hudPanels.TryGetValue(hud, out GameObject hudPanel);
                if (hudPanel != shouldEnableCountdownPanel)
                {
                    if (shouldEnableCountdownPanel)
                    {
                        RectTransform rectTransform = hud.GetComponent<ChildLocator>().FindChild("TopCenterCluster") as RectTransform;
                        if (rectTransform)
                        {
                            GameObject value = Object.Instantiate<GameObject>(hudPanelPrefab, rectTransform);
                            hudPanels[hud] = value;
                        }
                    }
                    else
                    {
                        Object.Destroy(hudPanel);
                        hudPanels.Remove(hud);
                    }
                }
            }

            public void SetCountdownTime(double secondsRemaining)
            {
                foreach (KeyValuePair<HUD, GameObject> keyValuePair in hudPanels)
                {
                    keyValuePair.Value.GetComponent<TimerText>().seconds = secondsRemaining;
                }
            }

            public void OnEnable()
            {
                InstanceTracker.Add(this);
            }

            public void OnDisable()
            {
                InstanceTracker.Remove(this);
                foreach (HUD hud in HUD.readOnlyInstanceList)
                {
                    SetHudCountdownEnabled(hud, false);
                }
            }
        }

        public class MysticsItemsRiftChest : MonoBehaviour
        {
            public PostProcessDuration ppDuration;
            public PositionIndicator positionIndicator;

            public void Awake()
            {
                if (NetworkServer.active) GetComponent<ChestBehavior>().RollItem();
            }

            public void Start()
            {
                PurchaseInteraction purchaseInteraction = GetComponent<PurchaseInteraction>();
                purchaseInteraction.onPurchase = new PurchaseEvent();
                purchaseInteraction.onPurchase.AddListener((interactor) =>
                {
                    purchaseInteraction.SetAvailable(false);
                    GetComponent<ChestBehavior>().ItemDrop();
                    DestroyThingsOnOpen();
                });

                positionIndicator = Object.Instantiate<GameObject>(riftPositionIndicator, transform.position, Quaternion.identity).GetComponent<PositionIndicator>();
                positionIndicator.targetTransform = transform;
            }

            public void DestroyThingsOnOpen()
            {
                SfxLocator sfxLocator = GetComponent<SfxLocator>();
                if (sfxLocator) Util.PlaySound(sfxLocator.openSound, gameObject);

                if (ppDuration) ppDuration.enabled = true;
                if (positionIndicator) positionIndicator.gameObject.SetActive(false);

                ObjectScaleCurve objectScaleCurve = GetComponentInChildren<ObjectScaleCurve>();
                if (objectScaleCurve)
                {
                    objectScaleCurve.enabled = true;
                }

                if (NetworkServer.active)
                {
                    new SyncDestroyThingsOnOpen(gameObject.GetComponent<NetworkIdentity>().netId).Send(NetworkDestination.Clients);
                }
            }

            public class SyncDestroyThingsOnOpen : INetMessage
            {
                NetworkInstanceId objID;

                public SyncDestroyThingsOnOpen()
                {
                }

                public SyncDestroyThingsOnOpen(NetworkInstanceId objID)
                {
                    this.objID = objID;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                }

                public void OnReceived()
                {
                    if (NetworkServer.active) return;
                    GameObject obj = Util.FindNetworkObject(objID);
                    if (obj)
                    {
                        MysticsItemsRiftChest component = obj.GetComponent<MysticsItemsRiftChest>();
                        if (component)
                        {
                            component.DestroyThingsOnOpen();
                        }
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                }
            }

            public void OnEnable()
            {
                InstanceTracker.Add<MysticsItemsRiftChest>(this);
            }

            public void OnDisable()
            {
                InstanceTracker.Remove<MysticsItemsRiftChest>(this);
            }
        }
    }
}
