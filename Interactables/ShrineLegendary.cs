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
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;

namespace MysticsItems.Interactables
{
    public class ShrineLegendary : BaseInteractable
    {
        public static DirectorCard directorCard;

        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            prefab = MysticsRisky2Utils.Utils.CreateBlankPrefab("MysticsItems_ShrineLegendary", true);
            prefab.AddComponent<NetworkTransform>();
        }

        public override void OnLoad()
        {
            base.OnLoad();

            MysticsRisky2Utils.Utils.CopyChildren(Main.AssetBundle.LoadAsset<GameObject>("Assets/Interactables/Shrine of the Legend/ShrineLegendary.prefab"), prefab, true);

            modelBaseTransform = prefab.transform.Find("Base");
            modelTransform = prefab.transform.Find("Base/mdlShrineLegendary");
            meshObject = prefab.transform.Find("Base/mdlShrineLegendary").gameObject;
            prefab.transform.Find("Base/mdlShrineLegendary/Collision").gameObject.layer = LayerIndex.world.intVal;
            genericDisplayNameToken = "MYSTICSITEMS_SHRINE_LEGENDARY_NAME";

            Prepare();
            Dither();

            Material prefabMaterial = meshObject.GetComponent<MeshRenderer>().sharedMaterial;
            prefabMaterial.SetFloat("_Glossiness", 0.5f);
            prefabMaterial.SetFloat("_GlossyReflections", 1f);
            HopooShaderToMaterial.Standard.Gloss(prefabMaterial, 0.07f, 1.25f, new Color32(96, 86, 48, 255));

            GameObject shrineChanceSymbol = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/Shrines/ShrineGoldshoresAccess").transform.Find("Symbol").gameObject;
            GameObject symbol = prefab.transform.Find("Symbol").gameObject;
            symbol.GetComponent<MeshFilter>().mesh = Object.Instantiate(shrineChanceSymbol.GetComponent<MeshFilter>().mesh);
            Material symbolMaterial = Object.Instantiate(shrineChanceSymbol.GetComponent<MeshRenderer>().material);
            symbol.GetComponent<MeshRenderer>().material = symbolMaterial;
            symbolMaterial.SetTexture("_MainTex", Main.AssetBundle.LoadAsset<Texture>("Assets/Interactables/Shrine of the Legend/Symbol.png"));
            symbolMaterial.SetTextureScale("_MainTex", new Vector2(1f, 1f));
            symbolMaterial.SetTexture("_RemapTex", Main.AssetBundle.LoadAsset<Texture>("Assets/Interactables/Shrine of the Legend/Symbol Ramp.png"));
            symbol.AddComponent<Billboard>();

            PurchaseInteraction purchaseInteraction = prefab.AddComponent<PurchaseInteraction>();
            purchaseInteraction.displayNameToken = "MYSTICSITEMS_SHRINE_LEGENDARY_NAME";
            purchaseInteraction.contextToken = "MYSTICSITEMS_SHRINE_LEGENDARY_CONTEXT";
            purchaseInteraction.costType = CostTypeIndex.LunarCoin;
            purchaseInteraction.available = true;
            purchaseInteraction.cost = 1;
            purchaseInteraction.automaticallyScaleCostWithDifficulty = false;
            purchaseInteraction.ignoreSpherecastForInteractability = false;
            purchaseInteraction.setUnavailableOnTeleporterActivated = true;
            purchaseInteraction.isShrine = true;

            PurchaseAvailabilityIndicator purchaseAvailabilityIndicator = prefab.AddComponent<PurchaseAvailabilityIndicator>();
            purchaseAvailabilityIndicator.indicatorObject = symbol.gameObject;

            RoR2.EntityLogic.DelayedEvent delayedEvent = prefab.AddComponent<RoR2.EntityLogic.DelayedEvent>();

            ShrineLegendaryBehaviour behaviour = prefab.AddComponent<ShrineLegendaryBehaviour>();
            behaviour.maxPurchaseCount = 1;
            behaviour.costMultiplierPerPurchase = 2f;
            behaviour.symbolTransform = symbol.transform;

            spawnCard.hullSize = HullClassification.Golem;
            spawnCard.forbiddenFlags = RoR2.Navigation.NodeFlags.NoShrineSpawn;
            spawnCard.directorCreditCost = 30;
            spawnCard.occupyPosition = true;
            spawnCard.orientToFloor = false;
            spawnCard.slightlyRandomizeOrientation = false;
            spawnCard.skipSpawnWhenSacrificeArtifactEnabled = false;

            directorCard = new DirectorCard
            {
                spawnCard = spawnCard,
                selectionWeight = 1,
                spawnDistance = 0f,
                preventOverhead = false,
                minimumStageCompletions = 1,
                requiredUnlockableDef = null,
                forbiddenUnlockableDef = null
            };

            ConfigManager.General.onSecretsToggled += (setEnabled) => enabled = setEnabled;
            enabled = ConfigManager.General.secrets;

            // Custom purchase cost type to take a fraction of the player's items
            /*
            GenericCostTypes.OnItemFractionCostTypeRegister += (costTypeIndex) =>
            {
                purchaseInteraction.costType = costTypeIndex;
                purchaseInteraction.cost = 100;
            };
            */
        }

        private static bool _enabled;
        public static bool enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (_enabled != value && directorCard != null)
                {
                    _enabled = value;
                    if (_enabled)
                    {
                        AddDirectorCardTo("wispgraveyard", "Shrines", directorCard);
                    }
                    else
                    {
                        RemoveDirectorCardFrom("wispgraveyard", "Shrines", directorCard);
                    }
                }
            }
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
                RoR2.EntityLogic.DelayedEvent delayedEvent = GetComponent<RoR2.EntityLogic.DelayedEvent>();
                delayedEvent.action = new UnityEvent();
                delayedEvent.action.AddListener(() =>
                {
                    AddShrineStack(purchaseInteraction.lastActivator);
                });
                delayedEvent.timeStepType = RoR2.EntityLogic.DelayedEvent.TimeStepType.FixedTime;

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
                        purchaseInteraction.Networkcost = (int)(100f * (1f - Mathf.Pow(1f - (float)purchaseInteraction.cost / 100f, costMultiplierPerPurchase)));
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
                int[] itemStacks = component.inventory.itemStacks;
                for (int i = 0; i < itemStacks.Length; i++)
                {
                    ItemIndex itemIndex = (ItemIndex)i;
                    if (itemStacks[i] > 0)
                    {
                        switch (ItemCatalog.GetItemDef(itemIndex).tier)
                        {
                            case ItemTier.Tier1:
                            case ItemTier.Tier2:
                            case ItemTier.Lunar:
                            case ItemTier.Boss:
                                addReds += itemStacks[i];
                                component.inventory.itemAcquisitionOrder.Remove(itemIndex);
                                component.inventory.ResetItem(itemIndex);
                                break;
                        }
                    }
                }
                for (var i = 0; i < addReds; i++)
                {
                    var rolledRed = rng.NextElementUniform<ItemIndex>(availableItems);
                    component.inventory.GiveItem(rolledRed);
                }
                component.inventory.SetDirtyBit(8U);

                Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
                {
                    subjectAsCharacterBody = component,
                    baseToken = "MYSTICSITEMS_SHRINE_LEGENDARY_USE_MESSAGE"
                });
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
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