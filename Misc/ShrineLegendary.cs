using RoR2;
using RoR2.Hologram;
using RoR2.Networking;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

namespace MysticsItems
{
    public static class ShrineLegendary
    {
        public static GameObject prefab;
        public static InteractableSpawnCard spawnCard;

        public static void Init()
        {
            prefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Interactables/Shrine of the Legend/ShrineLegendary.prefab");
            prefab.AddComponent<NetworkIdentity>();
            prefab.AddComponent<NetworkTransform>();

            GameObject prefabMesh = prefab.transform.Find("Base").Find("mdlShrineLegendary").gameObject;
            prefabMesh.AddComponent<EntityLocator>().entity = prefab;
            Material prefabMaterial = prefabMesh.GetComponent<MeshRenderer>().sharedMaterial;
            prefabMaterial.SetFloat("_Glossiness", 0.5f);
            prefabMaterial.SetFloat("_GlossyReflections", 1f);
            Main.HopooShaderToMaterial.Standard.Apply(prefabMaterial);
            Main.HopooShaderToMaterial.Standard.Dither(prefabMaterial);
            Main.HopooShaderToMaterial.Standard.Gloss(prefabMaterial, 0.07f, 1.25f, new Color32(96, 86, 48, 255));
            ChildLocator childLocator = prefab.AddComponent<ChildLocator>();
            prefab.transform.Find("Base").Find("mdlShrineLegendary").Find("Collision").gameObject.layer = LayerIndex.world.intVal;

            GameObject shrineChanceSymbol = Resources.Load<GameObject>("Prefabs/NetworkedObjects/Shrines/ShrineGoldshoresAccess").transform.Find("Symbol").gameObject;
            GameObject symbol = prefab.transform.Find("Symbol").gameObject;
            symbol.GetComponent<MeshFilter>().mesh = Object.Instantiate(shrineChanceSymbol.GetComponent<MeshFilter>().mesh);
            Material symbolMaterial = Object.Instantiate(shrineChanceSymbol.GetComponent<MeshRenderer>().material);
            symbol.GetComponent<MeshRenderer>().material = symbolMaterial;
            symbolMaterial.SetTexture("_MainTex", Main.AssetBundle.LoadAsset<Texture>("Assets/Interactables/Shrine of the Legend/Symbol.png"));
            symbolMaterial.SetTextureScale("_MainTex", new Vector2(1f, 1f));
            symbolMaterial.SetTexture("_RemapTex", Main.AssetBundle.LoadAsset<Texture>("Assets/Interactables/Shrine of the Legend/Symbol Ramp.png"));
            symbol.AddComponent<Billboard>();

            ModelLocator modelLocator = prefab.AddComponent<ModelLocator>();
            modelLocator.modelBaseTransform = prefab.transform.Find("Base");
            modelLocator.modelTransform = prefabMesh.transform;

            Highlight highlight = prefab.AddComponent<Highlight>();
            highlight.targetRenderer = prefabMesh.GetComponent<MeshRenderer>();
            highlight.strength = 1f;
            highlight.highlightColor = Highlight.HighlightColor.interactive;

            PurchaseInteraction purchaseInteraction = prefab.AddComponent<PurchaseInteraction>();
            purchaseInteraction.displayNameToken = Main.TokenPrefix.ToUpper() + "SHRINE_LEGENDARY_NAME";
            purchaseInteraction.contextToken = Main.TokenPrefix.ToUpper() + "SHRINE_LEGENDARY_CONTEXT";
            purchaseInteraction.costType = CostTypeIndex.WhiteItem;
            purchaseInteraction.available = true;
            purchaseInteraction.cost = 5;
            purchaseInteraction.automaticallyScaleCostWithDifficulty = false;
            purchaseInteraction.ignoreSpherecastForInteractability = false;
            purchaseInteraction.setUnavailableOnTeleporterActivated = true;
            purchaseInteraction.isShrine = true;

            DelayedEvent delayedEvent = prefab.AddComponent<DelayedEvent>();

            HologramProjector hologramProjector = prefab.AddComponent<HologramProjector>();
            hologramProjector.displayDistance = 15f;
            hologramProjector.hologramPivot = prefab.transform.Find("HologramPivot");
            hologramProjector.disableHologramRotation = false;

            GenericDisplayNameProvider displayNameProvider = prefab.AddComponent<GenericDisplayNameProvider>();
            displayNameProvider.displayToken = Main.TokenPrefix.ToUpper() + "SHRINE_LEGENDARY_NAME";

            ShrineLegendaryBehaviour behaviour = prefab.AddComponent<ShrineLegendaryBehaviour>();
            behaviour.maxPurchaseCount = 1;
            behaviour.costMultiplierPerPurchase = 2f;
            behaviour.symbolTransform = symbol.transform;

            PurchaseAvailabilityIndicator purchaseAvailabilityIndicator = prefab.AddComponent<PurchaseAvailabilityIndicator>();
            purchaseAvailabilityIndicator.indicatorObject = symbol.gameObject;

            DitherModel ditherModel = prefab.AddComponent<DitherModel>();
            ditherModel.bounds = prefabMesh.GetComponent<BoxCollider>();
            ditherModel.renderers = new Renderer[]
            {
                prefabMesh.GetComponent<MeshRenderer>()
            };

            PrefabAPI.RegisterNetworkPrefab(prefab);

            spawnCard = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            spawnCard.name = Main.TokenPrefix + "iscShrineLegendary";
            spawnCard.prefab = prefab;
            spawnCard.sendOverNetwork = true;
            spawnCard.hullSize = HullClassification.Golem;
            spawnCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            spawnCard.requiredFlags = RoR2.Navigation.NodeFlags.None;
            spawnCard.forbiddenFlags = RoR2.Navigation.NodeFlags.NoShrineSpawn;
            spawnCard.directorCreditCost = 30;
            spawnCard.occupyPosition = true;
            spawnCard.orientToFloor = false;
            spawnCard.slightlyRandomizeOrientation = false;
            spawnCard.skipSpawnWhenSacrificeArtifactEnabled = false;

            SceneDirector.onGenerateInteractableCardSelection += (sceneDirector, dccs) =>
            {
                if (SceneInfo.instance.sceneDef.baseSceneName == "wispgraveyard")
                {
                    dccs.AddCard(dccs.categories.ToList().FindIndex(x => x.name == "Shrines"), new DirectorCard
                    {
                        spawnCard = spawnCard,
                        selectionWeight = 1,
                        spawnDistance = 0f,
                        allowAmbushSpawn = true,
                        preventOverhead = false,
                        minimumStageCompletions = 1,
                        requiredUnlockable = "",
                        forbiddenUnlockable = ""
                    });
                }
            };

            On.RoR2.SceneDirector.PopulateScene += (orig, self) =>
            {
                orig(self);
                if (SceneCatalog.GetSceneDefForCurrentScene().baseSceneName == "golemplains" && BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.addOns.DebuggingPlains"))
                {
                    Vector3 position = new Vector3(346.2198f, -51.98051f - 0.7f, -209.0303f - 6f);
                    Vector3 rotation = new Vector3(0f, 180f, 0f);
                    SpawnCard.SpawnResult result = spawnCard.DoSpawn(position, Quaternion.Euler(rotation), new DirectorSpawnRequest(
                        spawnCard,
                        new DirectorPlacementRule
                        {
                            placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                            maxDistance = 100f,
                            minDistance = 20f,
                            position = position,
                            preventOverhead = true
                        },
                        RoR2Application.rng)
                    );
                    if (result.success)
                    {
                        result.spawnedInstance.transform.rotation = Quaternion.Euler(rotation);
                    }
                }
            };
        }

        public class ShrineLegendaryBehaviour : NetworkBehaviour
        {
            public int maxPurchaseCount;
            public float costMultiplierPerPurchase;
            public Transform symbolTransform;
            public PurchaseInteraction purchaseInteraction;
            public int purchaseCount;
            public float refreshTimer;
            public const float refreshDuration = 0.5f;
            public bool waitingForRefresh;
            public Xoroshiro128Plus rng;
            public List<ItemIndex> availableItems;

            public void Start()
            {
                DelayedEvent delayedEvent = GetComponent<DelayedEvent>();
                delayedEvent.action = new UnityEvent();
                delayedEvent.action.AddListener(() =>
                {
                    AddShrineStack(purchaseInteraction.lastActivator);
                });
                delayedEvent.timeStepType = DelayedEvent.TimeStepType.FixedTime;

                purchaseInteraction = GetComponent<PurchaseInteraction>();
                purchaseInteraction.onPurchase.AddListener((interactor) =>
                {
                    purchaseInteraction.SetAvailable(false);
                    delayedEvent.CallDelayed(1.5f);
                });

                availableItems = new List<ItemIndex>();
                if (NetworkServer.active)
                {
                    rng = new Xoroshiro128Plus(Run.instance.stageRng.nextUlong);
                    foreach (PickupIndex pickupIndex in Run.instance.availableTier3DropList)
                    {
                        availableItems.Add(PickupCatalog.GetPickupDef(pickupIndex).itemIndex);
                    }
                }
            }

            public void FixedUpdate()
            {
                if (waitingForRefresh)
                {
                    refreshTimer -= Time.fixedDeltaTime;
                    if (refreshTimer <= 0f && purchaseCount < maxPurchaseCount)
                    {
                        purchaseInteraction.SetAvailable(true);
                        purchaseInteraction.Networkcost = (int)((float)purchaseInteraction.cost * costMultiplierPerPurchase);
                        waitingForRefresh = false;
                    }
                }
            }

            [Server]
            public void AddShrineStack(Interactor interactor)
            {
                waitingForRefresh = true;
                CharacterBody component = interactor.GetComponent<CharacterBody>();

                int addReds = 0;
                int[] itemStacks = component.inventory.GetFieldValue<int[]>("itemStacks");
                for (int i = 0; i < itemStacks.Length; i++)
                {
                    ItemIndex itemIndex = (ItemIndex)i;
                    if (itemStacks[i] > 0)
                    {
                        switch (ItemCatalog.GetItemDef(itemIndex).tier)
                        {
                            case ItemTier.Tier1:
                            case ItemTier.Tier2:
                            case ItemTier.Tier3:
                            case ItemTier.Lunar:
                            case ItemTier.Boss:
                                addReds += itemStacks[i];
                                break;
                        }
                        component.inventory.itemAcquisitionOrder.Remove(itemIndex);
                        component.inventory.ResetItem(itemIndex);
                    }
                }
                for (int i = 0; i < addReds; i++)
                {
                    component.inventory.GiveItem(rng.NextElementUniform<ItemIndex>(availableItems));
                }
                component.inventory.SetDirtyBit(8U);

                Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
                {
                    subjectAsCharacterBody = component,
                    baseToken = Main.TokenPrefix.ToUpper() + "SHRINE_LEGENDARY_USE_MESSAGE"
                });
                EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
                {
                    origin = transform.position,
                    rotation = Quaternion.identity,
                    scale = 1f,
                    color = new Color32(255, 97, 84, 255)
                }, true);
                purchaseCount++;
                refreshTimer = refreshDuration;
                if (purchaseCount >= maxPurchaseCount)
                {
                    symbolTransform.gameObject.SetActive(false);
                }
            }

            public override int GetNetworkChannel()
            {
                return QosChannelIndex.defaultReliable.intVal;
            }
        }
    }
}