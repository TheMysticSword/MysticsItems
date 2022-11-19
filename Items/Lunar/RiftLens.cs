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
using static MysticsItems.LegacyBalanceConfigManager;
using UnityEngine.AddressableAssets;

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
            1,
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
        }

        public static System.Action<CostTypeIndex> OnRiftLensCostTypeRegister;

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_RiftLens";
            SetItemTierWhenAvailable(ItemTier.Lunar);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy,
                ItemTag.OnStageBeginEffect
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Rift Lens/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Rift Lens/Icon.png");
            MysticsItemsContent.Resources.unlockableDefs.Add(GetUnlockableDef());
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
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "GunBarrel", new Vector3(0.00002F, 0.07266F, -0.32569F), new Vector3(0F, 180F, 0F), new Vector3(0.02325F, 0.02325F, 0.02325F));
                AddDisplayRule("RailgunnerBody", "GunScope", new Vector3(-0.07466F, -0.14553F, 0.31781F), new Vector3(0F, 180F, 0F), new Vector3(0.02909F, 0.02909F, 0.02909F));
                AddDisplayRule("VoidSurvivorBody", "Head", new Vector3(0.00001F, 0.17385F, 0.09392F), new Vector3(58.31651F, 180F, 180F), new Vector3(0.03831F, 0.03831F, 0.03831F));
            };

            MysticsItemsRiftChest component = riftChest.AddComponent<MysticsItemsRiftChest>();
            
            SfxLocator sfxLocator = riftChest.AddComponent<SfxLocator>();
            sfxLocator.openSound = "Play_env_riftchest_open";
            
            riftChest.AddComponent<GenericDisplayNameProvider>().displayToken = "MYSTICSITEMS_RIFTCHEST_NAME";

            PurchaseInteraction purchaseInteraction = riftChest.AddComponent<PurchaseInteraction>();
            purchaseInteraction.displayNameToken = "MYSTICSITEMS_RIFTCHEST_NAME";
            purchaseInteraction.contextToken = "MYSTICSITEMS_RIFTCHEST_CONTEXT";

            ChestBehavior chestBehavior = riftChest.AddComponent<ChestBehavior>();
            chestBehavior.dropTable = Addressables.LoadAssetAsync<PickupDropTable>("RoR2/Base/Common/dtTier1Item.asset").WaitForCompletion();

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
            ppDuration.ppWeightCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
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

            SceneDirector.onPrePopulateSceneServer += (sceneDirector) =>
            {
                foreach (CharacterMaster characterMaster in CharacterMaster.readOnlyInstancesList)
                    if (characterMaster.teamIndex == TeamIndex.Player)
                    {
                        var debuffs = characterMaster.inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_RiftLensDebuff);
                        if (debuffs > 0)
                            characterMaster.inventory.RemoveItem(MysticsItemsContent.Items.MysticsItems_RiftLensDebuff, debuffs);
                    }
            };

            GenericGameEvents.OnPopulateScene += (rng) =>
            {
                int riftsToSpawn = 0;
                foreach (CharacterMaster characterMaster in CharacterMaster.readOnlyInstancesList)
                    if (characterMaster.teamIndex == TeamIndex.Player)
                    {
                        int thisItemCount = characterMaster.inventory.GetItemCount(itemDef);
                        if (thisItemCount > 0)
                        {
                            characterMaster.inventory.GiveItem(MysticsItemsContent.Items.MysticsItems_RiftLensDebuff, baseRifts + riftsPerStack * (thisItemCount - 1));
                            riftsToSpawn += baseRifts.Value + riftsPerStack.Value * (thisItemCount - 1);
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
            };

            On.RoR2.CharacterBody.OnInventoryChanged += (orig, self) =>
            {
                orig(self);
                self.AddItemBehavior<MysticsItemsRiftLensBehaviour>(self.inventory.GetItemCount(itemDef));
            };

            ObjectivePanelController.collectObjectiveSources += ObjectivePanelController_collectObjectiveSources;

            hudPanelPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/UI/HudModules/HudCountdownPanel"), "RiftLensHUDPanel");
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

        public static float GetMaxCountdown(CharacterBody body)
        {
            var averageWalkSpeed = (body.baseMoveSpeed != 0f ? body.baseMoveSpeed : 7f) * (body.sprintingSpeedMultiplier != 0f ? body.sprintingSpeedMultiplier : 1.45f) * Mathf.Min(1f + 0.05f * Run.instance.stageClearCount, 1.5f);
            var timeBonusFlat = 10f;
            var maxTime = 360f;
            
            // smart time calculation using node path lengths
            if (SceneInfo.instance && SceneInfo.instance.groundNodes)
            {
                var finalTimeMultiplier = 0.85f;
                
                var nodeGraph = SceneInfo.instance.groundNodes;

                var distanceBetweenAllRifts = 0f;
                var rifts = InstanceTracker.GetInstancesList<MysticsItemsRiftChest>();
                while (rifts.Count > 1) // we don't do "while (rifts.Count > 0)" because we can't measure the distance to the next rift if there's only one left
                {
                    var riftStart = rifts[0];
                    rifts.RemoveAt(0);
                    var startPos = riftStart.transform.position;
                    var dist = Mathf.Infinity;
                    MysticsItemsRiftChest riftEnd = null;

                    // find the closest rift
                    foreach (var rift2 in rifts)
                    {
                        var dist2 = Vector3.Distance(startPos, rift2.transform.position);
                        if (dist2 < dist)
                        {
                            dist = dist2;
                            riftEnd = rift2;
                        }
                    }
                    
                    if (riftEnd)
                    {
                        var endPos = riftEnd.transform.position;

                        var path = new Path(nodeGraph);
                        nodeGraph.ComputePath(new NodeGraph.PathRequest
                        {
                            startPos = startPos,
                            endPos = endPos,
                            path = path,
                            hullClassification = HullClassification.Human
                        }).Wait();
                        if (path.status == PathStatus.Valid)
                        {
                            for (int i = 1; i < path.waypointsCount; i++)
                            {
                                var pointA = nodeGraph.nodes[path[i - 1].nodeIndex.nodeIndex].position;
                                var pointB = nodeGraph.nodes[path[i].nodeIndex.nodeIndex].position;
                                var pointDistance = Vector3.Distance(pointA, pointB);
                                distanceBetweenAllRifts += pointDistance;
                            }
                        }
                        else
                        {
                            distanceBetweenAllRifts += Vector3.Distance(startPos, endPos) * 3.5f;
                        }
                    }
                }

                // calculate the shortest distance to the nearest rift for the current player
                var distanceToNearestRift = 1000f;
                {
                    var startPos = body.corePosition;
                    var dist = Mathf.Infinity;
                    MysticsItemsRiftChest riftEnd = null;

                    foreach (var rift in InstanceTracker.GetInstancesList<MysticsItemsRiftChest>())
                    {
                        var dist2 = Vector3.Distance(startPos, rift.transform.position);
                        if (dist2 < dist)
                        {
                            dist = dist2;
                            riftEnd = rift;
                        }
                    }

                    if (riftEnd)
                    {
                        var endPos = riftEnd.transform.position;

                        var path = new Path(nodeGraph);
                        nodeGraph.ComputePath(new NodeGraph.PathRequest
                        {
                            startPos = startPos,
                            endPos = endPos,
                            path = path,
                            hullClassification = HullClassification.Human
                        }).Wait();
                        if (path.status == PathStatus.Valid)
                        {
                            dist = 0f;
                            for (int i = 1; i < path.waypointsCount; i++)
                            {
                                var pointA = nodeGraph.nodes[path[i - 1].nodeIndex.nodeIndex].position;
                                var pointB = nodeGraph.nodes[path[i].nodeIndex.nodeIndex].position;
                                var pointDistance = Vector3.Distance(pointA, pointB);
                                dist += pointDistance;
                            }
                            distanceToNearestRift = dist;
                        }
                        else
                        {
                            distanceToNearestRift = Vector3.Distance(startPos, endPos) * 3.5f;
                        }
                    }
                }

                var totalDistance = distanceToNearestRift + distanceBetweenAllRifts;
                var calculatedCountdownTime = Mathf.Min((totalDistance / averageWalkSpeed) * finalTimeMultiplier + timeBonusFlat, maxTime);
                return calculatedCountdownTime;
            }

            // fallback time calculation
            {
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

                // calculate the shortest distance to the nearest rift for the current player
                var distanceToNearestRift = 1000f;
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
                            distanceToNearestRift = dist;
                        }
                    }
                }

                var totalDistance = distanceToNearestRift + distanceBetweenAllRifts;
                // players can't always go through the shortest path due to terrain and gravity, so we'll add bonus time
                var timeBonusMultiplier = 2.7f;
                var calculatedCountdownTime = Mathf.Min((totalDistance / averageWalkSpeed) * timeBonusMultiplier + timeBonusFlat, maxTime);
                return calculatedCountdownTime;
            }
        }

        public class MysticsItemsRiftLensBehaviour : CharacterBody.ItemBehavior
        {
            public int riftsLeft = 0;
            public int riftsTotal = 0;
            public float maxCountdown = 150f;
            public float countdownTimer = 150f;
            public bool diedFromTimer = false;

            public bool countdownCalculated = false;
            public float countdownCalculationTimer = 0f;
            public float countdownCalculationInterval = 4f;

            public bool nearNodes = true;
            public float nearNodeCheckTimer = 0;
            public float nearNodeCheckDuration = 1f;

            public Dictionary<HUD, GameObject> hudPanels = new Dictionary<HUD, GameObject>();

            public bool countdown10Played = false;
            public uint countdown10ID;

            public bool voidCampLockedBonusAdded = false;

            public void Start()
            {
                body.onInventoryChanged += Body_onInventoryChanged;
                diedFromTimer = false;
            }

            public bool CalculateCountdown()
            {
                if (InstanceTracker.GetInstancesList<MysticsItemsRiftChest>().Count <= 0) return false;
                
                var newCountdownTime = GetMaxCountdown(body);
                countdownTimer += newCountdownTime - maxCountdown;
                maxCountdown = newCountdownTime;
                return true;
            }

            public void Update()
            {
                if (!countdownCalculated)
                {
                    countdownCalculationTimer -= Time.deltaTime;
                    if (countdownCalculationTimer <= 0)
                    {
                        countdownCalculated = CalculateCountdown();
                        if (countdownCalculated)
                        {
                            UpdateItemBasedInfo();
                            countdownCalculationTimer += countdownCalculationInterval;
                        }
                    }
                }

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
                else
                {
                    if (riftsLeft > 0)
                    {
                        countdownTimer -= Time.deltaTime;

                        if (!diedFromTimer)
                        {
                            foreach (HUD hud in HUD.readOnlyInstanceList)
                            {
                                SetHudCountdownEnabled(hud, hud.targetBodyObject == body.gameObject);
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
                riftsTotal = Mathf.Max(baseRifts + riftsPerStack * (stack - 1), riftsTotal); // the Max call is here to avoid riftsTotal becoming less than riftsLeft in case the player removes a lens before opening all chests
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
            public PurchaseInteraction purchaseInteraction;
            public float lockUpdateTimer = 0;
            public float lockUpdateInterval = 2f;
            
            public void Awake()
            {
                purchaseInteraction = GetComponent<PurchaseInteraction>();
                purchaseInteraction.onPurchase = new PurchaseEvent();
                purchaseInteraction.onPurchase.AddListener((interactor) =>
                {
                    purchaseInteraction.SetAvailable(false);
                    GetComponent<ChestBehavior>().ItemDrop();
                    DestroyThingsOnOpen();
                });

                positionIndicator = Object.Instantiate<GameObject>(riftPositionIndicator, transform.position, Quaternion.identity).GetComponent<PositionIndicator>();
                positionIndicator.targetTransform = transform;

                UpdateLock();
            }

            public void FixedUpdate()
            {
                lockUpdateTimer += Time.fixedDeltaTime;
                if (lockUpdateTimer >= lockUpdateInterval)
                {
                    lockUpdateTimer = 0;
                    UpdateLock();
                }
            }

            public void UpdateLock()
            {
                if (purchaseInteraction && purchaseInteraction.lockGameObject)
                {
                    /*
                    var name = MysticsRisky2Utils.Utils.TrimCloneFromString(purchaseInteraction.lockGameObject.name);
                    switch (name)
                    {
                        case "PurchaseLockVoid":
                            foreach (var component in InstanceTracker.GetInstancesList<MysticsItemsRiftLensBehaviour>().Where(x => !x.voidCampLockedBonusAdded))
                            {
                                component.voidCampLockedBonusAdded = true;
                                component.countdownTimer += 100f;
                                component.maxCountdown += 100f;
                            }
                            return;
                    }
                    */

                    purchaseInteraction.lockGameObject = null;
                }
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
