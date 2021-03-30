using RoR2;
using RoR2.Networking;
using RoR2.Audio;
using EntityStates;
using R2API;
using R2API.Utils;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Collections.Generic;

namespace MysticsItems.Items
{
    public class Spotter : BaseItem
    {
        public static BuffDef buffDef;
        public static GameObject enemyFollowerPrefab;
        public static GameObject highlightPrefab;
        public static float interval = 20f;
        public static float duration = 10f;
        public static GameObject unlockInteractablePrefab;
        public static NetworkSoundEventDef repairSoundEventDef;

        public override void OnLoad()
        {
            itemDef.name = "Spotter";
            itemDef.tier = ItemTier.Tier2;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage
            };
            SetUnlockable();
            SetAssets("Spotter");
            Material mat = model.transform.Find("mdlSpotterBroken").gameObject.GetComponent<MeshRenderer>().sharedMaterial;
            Main.HopooShaderToMaterial.Standard.Apply(mat);
            Main.HopooShaderToMaterial.Standard.Gloss(mat, 0.2f, 1f);
            Main.HopooShaderToMaterial.Standard.Emission(mat, 1f);
            CopyModelToFollower();
            unlockInteractablePrefab = PrefabAPI.InstantiateClone(model, model.name + "UnlockInteractable", false);

            followerModel.transform.localScale = Vector3.one * 0.2f;
            followerModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
            enemyFollowerPrefab = PrefabAPI.InstantiateClone(new GameObject(), "SpotterController", false);
            enemyFollowerPrefab.AddComponent<NetworkIdentity>();
            enemyFollowerPrefab.AddComponent<CharacterNetworkTransform>();
            Rigidbody rigidbody = enemyFollowerPrefab.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            enemyFollowerPrefab.AddComponent<GenericOwnership>();
            SpotterController component = enemyFollowerPrefab.AddComponent<SpotterController>();
            component.follower = PrefabAPI.InstantiateClone(followerModel, "SpotterFollower", false);
            component.follower.transform.SetParent(enemyFollowerPrefab.transform);
            SimpleLeash leash = component.leash = enemyFollowerPrefab.AddComponent<SimpleLeash>();
            leash.minLeashRadius = 0f;
            leash.maxLeashRadius = Mathf.Infinity;
            leash.smoothTime = 0.2f;
            SimpleRotateToDirection rotateToDirection = component.rotateToDirection = enemyFollowerPrefab.AddComponent<SimpleRotateToDirection>();
            rotateToDirection.maxRotationSpeed = 720f;
            rotateToDirection.smoothTime = 0.1f;
            PrefabAPI.RegisterNetworkPrefab(enemyFollowerPrefab);

            highlightPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/UI/HighlightTier1Item"), Main.TokenPrefix + "HighlightSpotterTarget", false);
            RoR2.UI.HighlightRect highlightRect = highlightPrefab.GetComponent<RoR2.UI.HighlightRect>();
            highlightRect.highlightColor = new Color32(214, 58, 58, 255);
            highlightRect.maxLifeTime = 3f;

            NetworkingAPI.RegisterMessageType<SpotterController.SyncClearTarget>();
            NetworkingAPI.RegisterMessageType<SpotterController.SyncSetTarget>();

            unlockInteractablePrefab.AddComponent<NetworkIdentity>();
            unlockInteractablePrefab.transform.localScale *= 0.25f;
            unlockInteractablePrefab.AddComponent<MysticsItemsSpotterUnlockInteraction>();

            GameObject sparks = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/NetworkedObjects/RadarTower").transform.Find("mdlRadar").Find("Sparks").gameObject, "Sparks", false);
            sparks.transform.localPosition = new Vector3(0f, 1f, 0f);
            ParticleSystem.MainModule particleSystem = sparks.GetComponentInChildren<ParticleSystem>().main;
            particleSystem.scalingMode = ParticleSystemScalingMode.Hierarchy;
            sparks.transform.SetParent(unlockInteractablePrefab.transform);

            Highlight highlight = unlockInteractablePrefab.AddComponent<Highlight>();
            highlight.targetRenderer = unlockInteractablePrefab.GetComponentInChildren<Renderer>();
            highlight.highlightColor = Highlight.HighlightColor.interactive;

            PurchaseInteraction purchaseInteraction = unlockInteractablePrefab.AddComponent<PurchaseInteraction>();
            purchaseInteraction.displayNameToken = Main.TokenPrefix.ToUpper() + "BROKENSPOTTER_NAME";
            purchaseInteraction.contextToken = Main.TokenPrefix.ToUpper() + "BROKENSPOTTER_CONTEXT";
            purchaseInteraction.costType = CostTypeIndex.VolatileBattery;
            purchaseInteraction.available = true;
            purchaseInteraction.cost = 1;
            purchaseInteraction.automaticallyScaleCostWithDifficulty = false;
            purchaseInteraction.requiredUnlockable = "";
            purchaseInteraction.setUnavailableOnTeleporterActivated = false;

            GameObject entityLocatorHolder = PrefabAPI.InstantiateClone(new GameObject(), "EntityLocatorHolder", false);
            entityLocatorHolder.layer = LayerIndex.pickups.intVal;
            SphereCollider sphereCollider = entityLocatorHolder.AddComponent<SphereCollider>();
            sphereCollider.radius = 12f;
            sphereCollider.isTrigger = true;
            entityLocatorHolder.AddComponent<EntityLocator>().entity = unlockInteractablePrefab;
            entityLocatorHolder.transform.SetParent(unlockInteractablePrefab.transform);

            PrefabAPI.RegisterNetworkPrefab(unlockInteractablePrefab);

            On.RoR2.SceneDirector.PopulateScene += (orig, self) =>
            {
                orig(self);
                if (SceneCatalog.GetSceneDefForCurrentScene().baseSceneName == "rootjungle")
                {
                    GameObject obj = Object.Instantiate(unlockInteractablePrefab, new Vector3(-95.4724f, -45.03653f, -48.84156f), Quaternion.Euler(new Vector3(29f, 25f, 348f)));
                    NetworkServer.Spawn(obj);
                }
            };

            On.RoR2.CharacterBody.Awake += (orig, self) =>
            {
                orig(self);
                self.onInventoryChanged += delegate ()
                {
                    if (NetworkServer.active) self.AddItemBehavior<SpotterBehaviour>(self.inventory.GetItemCount(MysticsItemsContent.Items.Spotter));
                };
            };
            Overlays.CreateOverlay(Main.AssetBundle.LoadAsset<Material>("Assets/Misc/Materials/matSpotterMarked.mat"), delegate (CharacterModel model)
            {
                return model.body.HasBuff(buffDef);
            });

            repairSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            repairSoundEventDef.eventName = "Play_drone_repair";
            MysticsItemsContent.Resources.networkSoundEventDefs.Add(repairSoundEventDef);
        }

        public class MysticsItemsSpotterUnlockInteraction : MonoBehaviour
        {
            public void Awake()
            {
                PurchaseInteraction purchaseInteraction = GetComponent<PurchaseInteraction>();
                purchaseInteraction.onPurchase = new PurchaseEvent();
                purchaseInteraction.onPurchase.AddListener((interactor) =>
                {
                    if (OnUnlock != null) OnUnlock(interactor);
                    CharacterBody body = interactor.GetComponent<CharacterBody>();
                    if (body)
                    {
                        Inventory inventory = body.inventory;
                        if (inventory)
                        {
                            inventory.GiveItem(MysticsItemsContent.Items.Spotter);
                        }
                    }
                    PointSoundManager.EmitSoundServer(repairSoundEventDef.index, transform.position);
                    Object.Destroy(this.gameObject);
                });
            }

            public static event System.Action<Interactor> OnUnlock;
        }

        public class SpotterController : NetworkBehaviour
        {
            public GenericOwnership genericOwnership;
            public CharacterBody body;
            public CharacterBody target;
            public GameObject follower;
            public SimpleLeash leash;
            public SimpleRotateToDirection rotateToDirection;
            public Vector3 offset;
            public float stopwatch;
            public float waveY;
            public float waveAmplitude = 0.3f;
            public float waveOffset = 0f;
            public float waveFrequency = 1.5f;

            public void Awake()
            {
                genericOwnership = GetComponent<GenericOwnership>();
                genericOwnership.onOwnerChanged += delegate (GameObject newOwner)
                {
                    body = newOwner ? newOwner.GetComponent<CharacterBody>() : null;

                    if (body)
                    {
                        Vector2 circle = Random.insideUnitCircle * (5f * body.radius);
                        float height = Random.Range(-0.4f, 3f);
                        offset = new Vector3(circle.x, height, circle.y);
                    }
                };

                waveOffset = Random.value;
            }

            public class SyncClearTarget : INetMessage
            {
                NetworkInstanceId objID;

                public SyncClearTarget()
                {
                }

                public SyncClearTarget(NetworkInstanceId objID)
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
                        SpotterController controller = obj.GetComponent<SpotterController>();
                        if (controller)
                        {
                            controller.ClearTarget();
                        }
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                }
            }

            public class SyncSetTarget : INetMessage
            {
                NetworkInstanceId objID;
                NetworkInstanceId objID2;

                public SyncSetTarget()
                {
                }

                public SyncSetTarget(NetworkInstanceId objID, NetworkInstanceId objID2)
                {
                    this.objID = objID;
                    this.objID2 = objID2;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                    objID2 = reader.ReadNetworkId();
                }

                public void OnReceived()
                {
                    if (NetworkServer.active) return;
                    GameObject obj = Util.FindNetworkObject(objID);
                    GameObject obj2 = Util.FindNetworkObject(objID2);
                    if (obj && obj2)
                    {
                        SpotterController controller = obj.GetComponent<SpotterController>();
                        if (controller)
                        {
                            controller.SetTarget(obj2);
                        }
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                    writer.Write(objID2);
                }
            }

            public void Update()
            {
                if (!body || !body.GetFieldValue<Transform>("transform"))
                {
                    return;
                }

                if (NetworkServer.active)
                {
                    if (target && !target.HasBuff(buffDef))
                    {
                        ClearTarget();
                    }
                }

                stopwatch += Time.deltaTime;
                waveY = Mathf.Sin(stopwatch * Mathf.PI * waveFrequency + waveOffset * Mathf.PI * 2) * waveAmplitude;

                if (target)
                {
                    leash.leashOrigin = target.corePosition + offset + Vector3.up * waveY;
                    rotateToDirection.targetRotation = Util.QuaternionSafeLookRotation(target.corePosition - transform.position);
                }
                else
                {
                    leash.leashOrigin = body.corePosition + offset + Vector3.up * waveY;
                    rotateToDirection.targetRotation = Quaternion.LookRotation(body.inputBank.aimDirection);
                }

                ItemDisplay itemDisplay = follower.GetComponent<ItemDisplay>();
                ModelLocator modelLocator = body.modelLocator;
                if (modelLocator)
                {
                    Transform modelTransform = modelLocator.modelTransform;
                    if (modelTransform)
                    {
                        CharacterModel characterModel = modelTransform.GetComponent<CharacterModel>();
                        if (characterModel)
                        {
                            itemDisplay.SetVisibilityLevel(characterModel.visibility);
                            foreach (CharacterModel.RendererInfo rendererInfo in itemDisplay.rendererInfos)
                            {
                                Renderer renderer = rendererInfo.renderer;
                                MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                                renderer.GetPropertyBlock(propertyBlock);
                                propertyBlock.SetFloat("_Fade", target ? 1f : 1f / body.inventory.GetItemCount(MysticsItemsContent.Items.Spotter));
                                renderer.SetPropertyBlock(propertyBlock);
                            }
                        }
                    }
                }
            }

            public void ClearTarget()
            {
                if (NetworkServer.active)
                {
                    if (target && target.HasBuff(buffDef))
                    {
                        target.RemoveBuff(buffDef);
                    }
                    new SyncClearTarget(gameObject.GetComponent<NetworkIdentity>().netId).Send(NetworkDestination.Clients);
                }
                target = null;
            }

            public void SetTarget(GameObject newTarget)
            {
                ClearTarget();
                if (NetworkServer.active)
                {
                    new SyncSetTarget(gameObject.GetComponent<NetworkIdentity>().netId, newTarget.GetComponent<NetworkIdentity>().netId).Send(NetworkDestination.Clients);
                }
                CharacterBody newTargetBody = newTarget.GetComponent<CharacterBody>();
                if (newTargetBody)
                {
                    target = newTargetBody;
                    if (NetworkServer.active) target.AddTimedBuff(buffDef, duration);
                    Util.PlaySound("Play_item_proc_spotter", gameObject);
                    ModelLocator modelLocator = newTargetBody.modelLocator;
                    if (modelLocator)
                    {
                        Transform modelTransform = modelLocator.modelTransform;
                        if (modelTransform)
                        {
                            CharacterModel characterModel = modelTransform.GetComponentInChildren<CharacterModel>();
                            if (characterModel && characterModel.baseRendererInfos.Length >= 1)
                            {
                                RoR2.UI.HighlightRect.CreateHighlight(body.gameObject, characterModel.baseRendererInfos[0].renderer, highlightPrefab);
                            }
                        }
                    }
                }
            }
        }

        public class SpotterBehaviour : CharacterBody.ItemBehavior
        {
            public List<SpotterController> enemyFollowers = new List<SpotterController>();
            public float cooldown = 0f;
            public float cooldownMax = interval;
            public float cooldownIfNoEnemiesFound = 3f;
            public BullseyeSearch bullseyeSearch = new BullseyeSearch();

            public void FixedUpdate()
            {
                if (NetworkServer.active)
                {
                    while (enemyFollowers.Count < stack)
                    {
                        GameObject enemyFollower = Object.Instantiate(enemyFollowerPrefab, body.corePosition, Quaternion.identity);
                        enemyFollower.GetComponent<GenericOwnership>().ownerObject = gameObject;
                        NetworkServer.Spawn(enemyFollower);
                        enemyFollowers.Add(enemyFollower.GetComponent<SpotterController>());
                    }
                    while (enemyFollowers.Count > stack)
                    {
                        SpotterController enemyFollower = enemyFollowers.Last();
                        Object.Destroy(enemyFollower.gameObject);
                        enemyFollowers.Remove(enemyFollower);
                    }
                }
                cooldown -= Time.fixedDeltaTime;
                if (cooldown <= 0f)
                {
                    cooldown = cooldownMax;

                    if (NetworkServer.active)
                    {
                        bullseyeSearch.teamMaskFilter = TeamMask.GetUnprotectedTeams(body.teamComponent.teamIndex);
                        bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
                        bullseyeSearch.filterByLoS = true;
                        Ray ray = CameraRigController.ModifyAimRayIfApplicable(new Ray
                        {
                            origin = body.inputBank.aimOrigin,
                            direction = body.inputBank.aimDirection
                        }, body.gameObject, out _);
                        bullseyeSearch.searchOrigin = ray.origin;
                        bullseyeSearch.searchDirection = ray.direction;
                        bullseyeSearch.maxAngleFilter = 50f;
                        bullseyeSearch.viewer = body;
                        bullseyeSearch.RefreshCandidates();
                        bullseyeSearch.FilterOutGameObject(body.gameObject);
                        List<HurtBox> enemies = bullseyeSearch.GetResults().ToList();

                        foreach (SpotterController enemyFollower in enemyFollowers)
                        {
                            enemyFollower.ClearTarget();
                        }

                        if (enemies.Count > 0)
                        {
                            foreach (SpotterController enemyFollower in enemyFollowers)
                            {
                                GameObject newTarget = null;
                                while (newTarget == null && enemies.Count > 0)
                                {
                                    int index = Mathf.FloorToInt(enemies.Count * (Random.value * 0.99f));
                                    HurtBox newTargetHurtBox = enemies.ElementAt(index);
                                    enemies.RemoveAt(index);
                                    if (newTargetHurtBox.healthComponent && newTargetHurtBox.healthComponent.body)
                                    {
                                        CharacterBody newTargetBody = newTargetHurtBox.healthComponent.body;
                                        if (!newTargetBody.HasBuff(buffDef)) newTarget = newTargetBody.gameObject;
                                    }
                                }
                                if (newTarget)
                                {
                                    enemyFollower.SetTarget(newTarget);
                                }
                                else
                                {
                                    enemyFollower.ClearTarget();
                                }
                            }
                        }
                        else
                        {
                            cooldown = cooldownIfNoEnemiesFound;
                        }
                    }
                }
            }

            public void OnDestroy()
            {
                for (int i = 0; i < enemyFollowers.Count; i++)
                {
                    if (enemyFollowers[i])
                    {
                        enemyFollowers[i].ClearTarget();
                        Object.Destroy(enemyFollowers[i].gameObject);
                    }
                }
            }
        }
    }
}
