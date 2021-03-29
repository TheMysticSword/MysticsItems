using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using R2API.Networking.Interfaces;
using R2API.Networking;

namespace MysticsItems.Items
{
    public static class CharacterItems
    {
        public static StringBuilder globalStringBuilder = new StringBuilder();
        public static GameObject chestPrefab;
        public static GameObject chestSpawnerPrefab;
        public static SpawnCard chestSpawnerSpawnCard;
        public static GameObject openEffect;

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
                        MysticsItemsCharacterItem characterItem = modelObject.GetComponent<MysticsItemsCharacterItem>();
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
                        MysticsItemsCharacterItem characterItem = modelObject.GetComponent<MysticsItemsCharacterItem>();
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
                        MysticsItemsCharacterItem characterItem = modelObject.GetComponent<MysticsItemsCharacterItem>();
                        if (characterItem)
                        {
                            CharacterInfo characterInfo = FindCharacterInfo(characterItem.bodyName);
                            if (BodyCatalog.GetBodyName(body.bodyIndex) != characterInfo.bodyName) return;
                        }
                    }
                }
                orig(self, body);
            };

            On.RoR2.Language.GetLocalizedStringByToken += (orig, self, token) =>
            {
                string result = orig(self, token);
                MysticsItemsCharacterItem characterItem = characterItems.Find(x => ItemCatalog.GetItemDef(x.itemIndex).nameToken == token);
                if (characterItem != null)
                {
                    CharacterInfo characterInfo = FindCharacterInfo(characterItem.bodyName);
                    ItemDef itemDef = ItemCatalog.GetItemDef(characterItem.itemIndex);
                    string formatToken = "MYSTICSITEMS_CHARACTERITEM_PICKUP_FORMAT";
                    globalStringBuilder.Clear();
                    globalStringBuilder.Append(" <nobr>");
                    globalStringBuilder.Append("<color=#");
                    globalStringBuilder.AppendColor32RGBHexValues(characterInfo.color);
                    globalStringBuilder.Append(">");
                    globalStringBuilder.Append("(");
                    globalStringBuilder.Append(
                        string.Format(
                            self.TokenIsRegistered(formatToken) ? self.GetLocalizedStringByToken(formatToken) : Language.english.GetLocalizedStringByToken(formatToken),
                            self.TokenIsRegistered(characterInfo.nameToken) ? self.GetLocalizedStringByToken(characterInfo.nameToken) : Language.english.GetLocalizedStringByToken(characterInfo.nameToken)
                        )
                    );
                    globalStringBuilder.Append(")");
                    globalStringBuilder.Append("</color>");
                    globalStringBuilder.Append("</nobr>");
                    result += globalStringBuilder.ToString();
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

            chestPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Interactables/Character Item Chest/Chest.prefab");
            chestPrefab.name = Main.TokenPrefix + "CharacterItemChest";
            chestPrefab.AddComponent<NetworkIdentity>();
            GameObject modelBaseTransform = chestPrefab.transform.Find("Base").gameObject;
            GameObject modelTransform = chestPrefab.transform.Find("Base/Full Model").gameObject;
            ModelLocator modelLocator = chestPrefab.AddComponent<ModelLocator>();
            modelLocator.dontDetatchFromParent = true;
            modelLocator.modelBaseTransform = modelBaseTransform.transform;
            modelLocator.modelTransform = modelTransform.transform;
            modelLocator.normalizeToFloor = false;
            Highlight highlight = chestPrefab.AddComponent<Highlight>();
            highlight.targetRenderer = modelTransform.GetComponent<Renderer>();
            highlight.strength = 1f;
            highlight.highlightColor = Highlight.HighlightColor.interactive;
            PurchaseInteraction purchaseInteraction = chestPrefab.AddComponent<PurchaseInteraction>();
            purchaseInteraction.displayNameToken = Main.TokenPrefix.ToUpper() + "CHARACTERITEMCHEST_NAME";
            purchaseInteraction.contextToken = Main.TokenPrefix.ToUpper() + "CHARACTERITEMCHEST_CONTEXT";
            purchaseInteraction.costType = CostTypeIndex.Money;
            purchaseInteraction.cost = 75;
            purchaseInteraction.automaticallyScaleCostWithDifficulty = true;
            purchaseInteraction.requiredUnlockable = "";
            purchaseInteraction.ignoreSpherecastForInteractability = false;
            purchaseInteraction.purchaseStatNames = new string[] { };
            purchaseInteraction.setUnavailableOnTeleporterActivated = false;
            purchaseInteraction.isShrine = false;
            purchaseInteraction.isGoldShrine = false;
            RoR2.Hologram.HologramProjector hologramProjector = chestPrefab.AddComponent<RoR2.Hologram.HologramProjector>();
            hologramProjector.displayDistance = 15f;
            hologramProjector.hologramPivot = chestPrefab.transform.Find("HologramPivot");
            hologramProjector.disableHologramRotation = false;
            chestPrefab.AddComponent<GenericDisplayNameProvider>().displayToken = Main.TokenPrefix.ToUpper() + "CHARACTERITEMCHEST_NAME";
            DitherModel ditherModel = chestPrefab.AddComponent<DitherModel>();
            ditherModel.bounds = modelTransform.GetComponent<Collider>();
            ditherModel.renderers = new Renderer[]
            {
                modelTransform.GetComponent<Renderer>()
            };
            MysticsItemsCharacterItemChest charItemChest = chestPrefab.AddComponent<MysticsItemsCharacterItemChest>();
            charItemChest.combinedObject = modelTransform.gameObject;
            charItemChest.baseObject = modelBaseTransform.transform.Find("Detached/Base").gameObject;
            charItemChest.lidObject = modelBaseTransform.transform.Find("Detached/Top").gameObject;
            charItemChest.dropTransform = chestPrefab.transform.Find("DropTransform");
            EntityLocator entityLocator = modelTransform.AddComponent<EntityLocator>();
            entityLocator.entity = chestPrefab;
            RandomizeSplatBias randomizeSplatBias = modelBaseTransform.AddComponent<RandomizeSplatBias>();
            randomizeSplatBias.minRedBias = 0f;
            randomizeSplatBias.maxRedBias = 0f;
            randomizeSplatBias.minGreenBias = -0.4f;
            randomizeSplatBias.maxGreenBias = 0.7f;
            randomizeSplatBias.minBlueBias = -2f;
            randomizeSplatBias.maxBlueBias = -2f;
            Material material = modelTransform.GetComponent<Renderer>().sharedMaterial;
            Main.HopooShaderToMaterial.Standard.Apply(material);
            Main.HopooShaderToMaterial.Standard.Gloss(material, 0.3f, 7f);
            Main.HopooShaderToMaterial.Standard.Dither(material);
            Outlines.MysticsItemsOutline outline = chestPrefab.AddComponent<Outlines.MysticsItemsOutline>();
            outline.offset = 100f;
            outline.thickness = 100f;
            outline.color = new Color32(100, 240, 255, 255);
            outline.isOn = true;
            outline.targetRenderer = modelTransform.GetComponent<Renderer>();
            PrefabAPI.RegisterNetworkPrefab(chestPrefab);

            chestSpawnerPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Interactables/Character Item Chest/Spawner.prefab");
            chestSpawnerPrefab.name = Main.TokenPrefix + "CharacterItemChestSpawner";
            chestSpawnerPrefab.AddComponent<NetworkIdentity>();
            chestSpawnerPrefab.AddComponent<MysticsItemsCharacterItemChestSpawner>();
            chestSpawnerPrefab.AddComponent<DestroyOnTimer>().duration = 4f;
            PrefabAPI.RegisterNetworkPrefab(chestSpawnerPrefab);

            chestSpawnerSpawnCard = ScriptableObject.CreateInstance<SpawnCard>();
            chestSpawnerSpawnCard.name = "isc" + Main.TokenPrefix + "CharacterItemChestSpawner";
            chestSpawnerSpawnCard.prefab = chestSpawnerPrefab;
            chestSpawnerSpawnCard.directorCreditCost = 0;
            chestSpawnerSpawnCard.sendOverNetwork = true;
            chestSpawnerSpawnCard.hullSize = HullClassification.Human;
            chestSpawnerSpawnCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            chestSpawnerSpawnCard.requiredFlags = RoR2.Navigation.NodeFlags.None;
            chestSpawnerSpawnCard.forbiddenFlags = RoR2.Navigation.NodeFlags.NoChestSpawn | RoR2.Navigation.NodeFlags.NoShrineSpawn;
            chestSpawnerSpawnCard.occupyPosition = true;

            /*
            On.RoR2.Run.Start += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active) self.gameObject.AddComponent<MysticsItemsCharacterItemChestSpawningController>();
            };
            */

            NetworkingAPI.RegisterMessageType<MysticsItemsCharacterItemChest.SyncOpen>();

            openEffect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), Main.TokenPrefix + "OmiExplosionVFXExplosivePickups", false);
            Object.Destroy(openEffect.transform.Find("ScaledHitsparks 1").gameObject);
            Object.Destroy(openEffect.transform.Find("UnscaledHitsparks 1").gameObject);
            Object.Destroy(openEffect.transform.Find("ScaledSmokeRing, Mesh").gameObject);
            Object.Destroy(openEffect.transform.Find("Physics Sparks").gameObject);
            Object.Destroy(openEffect.transform.Find("Flash, Soft Glow").gameObject);
            Object.Destroy(openEffect.transform.Find("Unscaled Flames").gameObject);
            Object.Destroy(openEffect.transform.Find("Dash, Bright").gameObject);
            MysticsItemsContent.Resources.effectPrefabs.Add(openEffect);
        }

        public static List<CharacterInfo> characterInfo = new List<CharacterInfo>();
        public struct CharacterInfo
        {
            public string nameToken;
            public string bodyName;
            public Color color;
        }
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

        public class MysticsItemsCharacterItem : MonoBehaviour
        {
            public string bodyName;
            public ItemIndex itemIndex;
            public ItemTier itemTier;

            public void Awake()
            {
                CharacterInfo characterInfo = FindCharacterInfo(bodyName);
                Outlines.MysticsItemsOutline outline = gameObject.AddComponent<Outlines.MysticsItemsOutline>();
                outline.targetRenderer = GetComponentInChildren<Renderer>();
                Vector3 lossyScale = outline.targetRenderer.gameObject.transform.lossyScale;
                float scale = 100f / ((lossyScale.x + lossyScale.y + lossyScale.z) / 3f);
                outline.offset = scale;
                outline.thickness = scale;
                outline.color = characterInfo.color;
                outline.isOn = false;
            }

            public void SetOutlineVisibility(bool visible)
            {
                foreach (Outlines.MysticsItemsOutline outline in GetComponentsInChildren<Outlines.MysticsItemsOutline>())
                {
                    outline.isOn = visible;
                }
            }
        }
        public static List<MysticsItemsCharacterItem> characterItems = new List<MysticsItemsCharacterItem>();

        public static void SetCharacterItem(BaseItem baseItem, string bodyName)
        {
            ItemDef itemDef = baseItem.itemDef;
            GameObject model = baseItem.model;
            GameObject followerModel = baseItem.followerModel;

            HG.ArrayUtils.ArrayAppend(ref itemDef.tags, ItemTag.WorldUnique);
            HG.ArrayUtils.ArrayAppend(ref itemDef.tags, ItemTag.AIBlacklist);
            HG.ArrayUtils.ArrayAppend(ref itemDef.tags, ItemTag.BrotherBlacklist);
            baseItem.BanFromDeployables();

            CharacterInfo currentCharacterInfo = FindCharacterInfo(bodyName);

            MysticsItemsCharacterItem characterItem = model.AddComponent<MysticsItemsCharacterItem>();
            characterItem.bodyName = currentCharacterInfo.bodyName;
            characterItem.itemIndex = itemDef.itemIndex;
            characterItem.itemTier = itemDef.tier;
            characterItems.Add(characterItem);

            BaseItem.Reskinner reskinner = model.AddComponent<BaseItem.Reskinner>();
            reskinner.defaultBodyName = currentCharacterInfo.bodyName;
            reskinner = followerModel.AddComponent<BaseItem.Reskinner>();
            reskinner.defaultBodyName = currentCharacterInfo.bodyName;
        }

        public class MysticsItemsCharacterItemChest : MonoBehaviour
        {
            public Xoroshiro128Plus rng;
            public List<PickupIndex> drops = new List<PickupIndex>();
            public float dropAge = 0f;
            public float dropDelay = 0.35f;
            public bool dropping = false;
            public Transform dropTransform;
            public float dropStrengthUpward = 20f;
            public float dropStrengthForward = 4f;
            public float dropTransformStepRotation = 0f;
            public float tier1Chance = 0.6f;
            public float tier2Chance = 0.3f;
            public float tier3Chance = 0.1f;

            public int dropsPerPlayer = 1;

            public GameObject combinedObject;
            public GameObject baseObject;
            public GameObject lidObject;
            public float lidThrowStrength = 300f;
            public float lidThrowSpread = 30f;
            public float lidRotationStrength = 10f;

            public void Awake()
            {
                PurchaseInteraction purchaseInteraction = GetComponent<PurchaseInteraction>();

                purchaseInteraction.onPurchase.RemoveAllListeners();
                purchaseInteraction.onPurchase.AddListener((interactor) =>
                {
                    purchaseInteraction.SetAvailable(false);
                    RollDrops();
                    Open();
                });
            }

            public void Start()
            {
                if (NetworkServer.active)
                {
                    rng = new Xoroshiro128Plus(Run.instance.seed);
                }
                if (!dropTransform) dropTransform = transform;
            }

            public void FixedUpdate()
            {
                if (NetworkServer.active && dropping)
                {
                    dropAge += Time.fixedDeltaTime;
                    if (dropAge >= dropDelay)
                    {
                        dropAge = 0f;
                        if (drops.Count > 0)
                        {
                            PickupIndex drop = drops.First();
                            drops.RemoveAt(0);
                            PickupDropletController.CreatePickupDroplet(
                                drop,
                                dropTransform.position,
                                Vector3.up * dropStrengthUpward + dropTransform.forward * dropStrengthForward
                            );
                            dropTransform.Rotate(new Vector3(0f, dropTransformStepRotation, 0f), Space.Self);
                        }
                        if (drops.Count <= 0) dropping = false;
                    }
                }
            }

            public void RollDrops()
            {
                if (!NetworkServer.active) return;

                drops.Clear();

                // Get drops for current characters
                foreach (PlayerCharacterMasterController playerCharacterMasterController in PlayerCharacterMasterController.instances)
                {
                    CharacterMaster master = playerCharacterMasterController.master;
                    if (master && master.hasBody)
                    {
                        CharacterBody characterBody = master.GetBody();
                        if (characterBody.healthComponent && characterBody.healthComponent.alive)
                        {
                            string bodyName = BodyCatalog.GetBodyName(characterBody.bodyIndex);
                            List<MysticsItemsCharacterItem> itemsForThisChar = characterItems.FindAll(x => x.bodyName == bodyName);
                            if (itemsForThisChar.Count > 0)
                            {
                                WeightedSelection<List<PickupIndex>> dropSelector = new WeightedSelection<List<PickupIndex>>();
                                List<PickupIndex> list;
                                list = itemsForThisChar.FindAll(x => ItemCatalog.GetItemDef(x.itemIndex).tier == ItemTier.Tier1).ConvertAll(x => PickupCatalog.FindPickupIndex(x.itemIndex));
                                if (list.Count > 0) dropSelector.AddChoice(list, tier1Chance);
                                list = itemsForThisChar.FindAll(x => ItemCatalog.GetItemDef(x.itemIndex).tier == ItemTier.Tier2).ConvertAll(x => PickupCatalog.FindPickupIndex(x.itemIndex));
                                if (list.Count > 0) dropSelector.AddChoice(list, tier2Chance);
                                list = itemsForThisChar.FindAll(x => ItemCatalog.GetItemDef(x.itemIndex).tier == ItemTier.Tier3).ConvertAll(x => PickupCatalog.FindPickupIndex(x.itemIndex));
                                if (list.Count > 0) dropSelector.AddChoice(list, tier3Chance);

                                for (var i = 0; i < dropsPerPlayer; i++)
                                {
                                    drops.Add(rng.NextElementUniform(dropSelector.Evaluate(rng.nextNormalizedFloat)));
                                }
                            }
                            else // If there are no character items for this character, then drop scrap
                            {
                                WeightedSelection<PickupIndex> dropSelector = new WeightedSelection<PickupIndex>();
                                dropSelector.AddChoice(PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapWhite.itemIndex), tier1Chance);
                                dropSelector.AddChoice(PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapGreen.itemIndex), tier2Chance);
                                dropSelector.AddChoice(PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapRed.itemIndex), tier3Chance);

                                for (var i = 0; i < dropsPerPlayer; i++)
                                {
                                    drops.Add(dropSelector.Evaluate(rng.nextNormalizedFloat));
                                }
                            }
                        }
                    }
                }
            }

            public void Open()
            {
                Util.PlaySound("Play_UI_chest_unlock", gameObject);
                dropping = true;
                dropTransformStepRotation = 360f / (float)drops.Count;
                foreach (Outlines.MysticsItemsOutline mysticsItemsOutline in GetComponentsInChildren<Outlines.MysticsItemsOutline>())
                {
                    mysticsItemsOutline.isOn = false;
                }
                combinedObject.GetComponent<Renderer>().enabled = false;
                baseObject.SetActive(true);
                lidObject.SetActive(true);
                if (NetworkServer.active)
                {
                    EffectManager.SpawnEffect(openEffect, new EffectData
                    {
                        origin = lidObject.transform.position,
                        scale = 2f
                    }, true);
                }
                Rigidbody rigidbody = lidObject.GetComponent<Rigidbody>();
                Vector3 aimLid = Util.ApplySpread(transform.up, -lidThrowSpread, lidThrowSpread, 1f, 1f);
                rigidbody.AddForce(aimLid * lidThrowStrength);
                rigidbody.angularVelocity += Util.QuaternionSafeLookRotation(aimLid).eulerAngles * lidRotationStrength;
                lidObject.AddComponent<DestroyOnTimer>().duration = 10f;
                if (NetworkServer.active)
                {
                    new SyncOpen(gameObject.GetComponent<NetworkIdentity>().netId).Send(NetworkDestination.Clients);
                }
            }

            public class SyncOpen : INetMessage
            {
                NetworkInstanceId objID;

                public SyncOpen()
                {
                }

                public SyncOpen(NetworkInstanceId objID)
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
                        MysticsItemsCharacterItemChest component = obj.GetComponent<MysticsItemsCharacterItemChest>();
                        if (component)
                        {
                            component.Open();
                        }
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                }
            }
        }

        public class MysticsItemsCharacterItemChestSpawner : MonoBehaviour
        {
            public float soundTimer = 1.5f;
            public bool soundPlayed = false;
            public float spawnTimer = 3f;
            public bool spawned = false;

            public void FixedUpdate()
            {
                if (!soundPlayed)
                {
                    soundTimer -= Time.fixedDeltaTime;
                    if (soundTimer <= 0f)
                    {
                        soundPlayed = true;
                        Util.PlaySound("Play_captain_shift_preImpact", gameObject);
                    }
                }
                if (!spawned)
                {
                    spawnTimer -= Time.fixedDeltaTime;
                    if (spawnTimer <= 0f)
                    {
                        spawned = true;
                        Util.PlaySound("Play_captain_shift_impact", gameObject);
                        if (NetworkServer.active)
                        {
                            EffectManager.SimpleEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CaptainAirstrikeImpact1"), transform.position, Quaternion.identity, true);
                            
                            RaycastHit raycastHit;
                            Vector3 position = transform.position;
                            Vector3 normal = Vector3.up;
                            if (Physics.Raycast(new Ray(transform.position + Vector3.up * 0.1f, Vector3.down), out raycastHit, Mathf.Infinity, LayerIndex.world.mask))
                            {
                                position = raycastHit.point;
                                normal = raycastHit.normal;
                            }

                            GameObject chest = Object.Instantiate(chestPrefab, transform.position, Quaternion.identity);
                            chest.transform.up = normal;
                            NetworkServer.Spawn(chest);

                            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                            {
                                baseToken = "MYSTICSITEMS_CHARACTERITEMCHEST_ANNOUNCE"
                            });
                        }
                    }
                }
            }
        }

        public class MysticsItemsCharacterItemChestSpawningController : MonoBehaviour
        {
            public Run run;
            public float interval = 600f;
            public int previousCycle = 0;
            public int currentCycle
            {
                get
                {
                    return run ? Mathf.FloorToInt(run.GetRunStopwatch() / interval) : int.MinValue;
                }
            }
            public Xoroshiro128Plus rng;

            public void Start()
            {
                if (NetworkServer.active)
                {
                    run = Run.instance;
                    rng = new Xoroshiro128Plus(run.seed);
                }
            }

            public void FixedUpdate()
            {
                int _currentCycle = currentCycle;
                if (_currentCycle > previousCycle)
                {
                    previousCycle = _currentCycle;

                    if (NetworkServer.active)
                    {
                        if (!SceneInfo.instance) return;
                        SceneDef sceneDef = SceneInfo.instance.sceneDef;
                        if (sceneDef.sceneType != SceneType.Stage || sceneDef.isFinalStage) return;
                        bool atLeastOneCharacterEligibleForItems = false;
                        foreach (PlayerCharacterMasterController playerCharacterMasterController in PlayerCharacterMasterController.instances)
                        {
                            CharacterMaster master = playerCharacterMasterController.master;
                            if (master && master.hasBody)
                            {
                                CharacterBody characterBody = master.GetBody();
                                if (characterBody.healthComponent && characterBody.healthComponent.alive)
                                {
                                    string bodyName = BodyCatalog.GetBodyName(characterBody.bodyIndex);
                                    List<MysticsItemsCharacterItem> itemsForThisChar = characterItems.FindAll(x => x.bodyName == bodyName);
                                    if (itemsForThisChar.Count > 0)
                                    {
                                        atLeastOneCharacterEligibleForItems = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!atLeastOneCharacterEligibleForItems) return;
                        DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(chestSpawnerSpawnCard, new DirectorPlacementRule
                        {
                            placementMode = DirectorPlacementRule.PlacementMode.Random
                        }, rng));
                    }
                }
            }
        }
    }
}
