using EntityStates;
using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Networking.Interfaces;
using R2API.Networking;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Items
{
    public class DasherDisc : BaseItem
    {
        public static GameObject controllerPrefab;

        public static ConfigurableValue<float> duration = new ConfigurableValue<float>(
            "Item: Timely Execution",
            "Duration",
            7f,
            "Invincibility duration (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_DASHERDISC_DESC"
            }
        );
        public static ConfigurableValue<float> cooldown = new ConfigurableValue<float>(
            "Item: Timely Execution",
            "Cooldown",
            60f,
            "Invincibility cooldown (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_DASHERDISC_PICKUP",
                "ITEM_MYSTICSITEMS_DASHERDISC_DESC"
            }
        );
        public static ConfigurableValue<float> cooldownReductionPerStack = new ConfigurableValue<float>(
            "Item: Timely Execution",
            "CooldownReductionPerStack",
            50f,
            "Invincibility cooldown reduction for each additional stack of this item (in %, hyperbolic)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_DASHERDISC_DESC"
            }
        );

        public override void OnPluginAwake()
        {
            controllerPrefab = PrefabAPI.InstantiateClone(new GameObject(), "DasherDiscController", false);
            controllerPrefab.AddComponent<NetworkIdentity>().localPlayerAuthority = false;
            PrefabAPI.RegisterNetworkPrefab(controllerPrefab);

            NetworkingAPI.RegisterMessageType<DiscBaseState.Ready.SyncFireTrigger>();
        }

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_DasherDisc";
            SetItemTierWhenAvailable(ItemTier.Tier3);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Dasher Disc/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Dasher Disc/Icon.png");
            MysticsItemsContent.Resources.unlockableDefs.Add(GetUnlockableDef());
            Material mat = itemDef.pickupModelPrefab.transform.Find("mdlDasherDisc").GetComponent<MeshRenderer>().sharedMaterial;
            HopooShaderToMaterial.Standard.Apply(mat);
            HopooShaderToMaterial.Standard.Emission(mat, 4f);
            HopooShaderToMaterial.Standard.Gloss(mat);
            MysticsItemsDasherDiscSpinner spinner = itemDef.pickupModelPrefab.transform.Find("mdlDasherDisc").gameObject.AddComponent<MysticsItemsDasherDiscSpinner>();
            spinner.trail = itemDef.pickupModelPrefab.transform.Find("mdlDasherDisc").Find("Particle System").gameObject;
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            itemDisplayPrefab.transform.Find("mdlDasherDisc").localScale = Vector3.one * 5f;

            controllerPrefab.AddComponent<GenericOwnership>();
            NetworkedBodyAttachment networkedBodyAttachment = controllerPrefab.AddComponent<NetworkedBodyAttachment>();
            networkedBodyAttachment.forceHostAuthority = true;
            EntityStateMachine stateMachine = controllerPrefab.AddComponent<EntityStateMachine>();
            stateMachine.mainStateType = stateMachine.initialStateType = new SerializableEntityStateType(typeof(DiscBaseState.Ready));
            NetworkStateMachine networkStateMachine = controllerPrefab.AddComponent<NetworkStateMachine>();
            networkStateMachine.SetFieldValue("stateMachines", new EntityStateMachine[] {
                stateMachine
            });
            DiscController component = controllerPrefab.AddComponent<DiscController>();
            GameObject follower = PrefabAPI.InstantiateClone(itemDisplayPrefab, "DasherDiscFollower", false);
            follower.transform.SetParent(controllerPrefab.transform);
            component.follower = follower;
            component.disc = follower.transform.Find("mdlDasherDisc").gameObject;
            component.discSpinner = component.disc.GetComponent<MysticsItemsDasherDiscSpinner>();
            
            MysticsItemsContent.Resources.entityStateTypes.Add(typeof(DiscBaseState));
            MysticsItemsContent.Resources.entityStateTypes.Add(typeof(DiscBaseState.Ready));
            MysticsItemsContent.Resources.entityStateTypes.Add(typeof(DiscBaseState.Trigger));

            On.RoR2.CharacterBody.Awake += (orig, self) =>
            {
                orig(self);
                self.onInventoryChanged += delegate ()
                {
                    if (NetworkServer.active) self.AddItemBehavior<MysticsItemsDasherDiscBehaviour>(self.inventory.GetItemCount(itemDef));
                };
            };
        }

        public class MysticsItemsDasherDiscSpinner : MonoBehaviour
        {
            public float baseSpeed = -45f;
            public float speedMultiplier = 1f;
            public GameObject trail;

            public void Update()
            {
                transform.Rotate(new Vector3(0f, 0f, -baseSpeed * speedMultiplier * Time.deltaTime), Space.Self);
            }

            public void SetTrail(bool enable)
            {
                if (trail) trail.SetActive(enable);
            }
        }

        public class MysticsItemsDasherDiscBehaviour : CharacterBody.ItemBehavior
        {
            public GameObject controller;

            public void Start()
            {
                if (NetworkServer.active)
                {
                    controller = Object.Instantiate(controllerPrefab, body.corePosition, Quaternion.identity);
                    controller.GetComponent<GenericOwnership>().ownerObject = gameObject;
                    controller.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(gameObject);
                }
            }

            public void OnDestroy()
            {
                if (NetworkServer.active)
                {
                    if (controller) Object.Destroy(controller);
                    if (body)
                    {
                        while (body.HasBuff(MysticsItemsContent.Buffs.MysticsItems_DasherDiscActive)) body.RemoveBuff(MysticsItemsContent.Buffs.MysticsItems_DasherDiscActive);
                        while (body.HasBuff(MysticsItemsContent.Buffs.MysticsItems_DasherDiscCooldown)) body.ClearTimedBuffs(MysticsItemsContent.Buffs.MysticsItems_DasherDiscCooldown);
                    }
                }
            }
        }

        public static float CalculateCooldown(int itemCount)
        {
            return cooldown / (1f + cooldownReductionPerStack / 100f * (itemCount - 1));
        }

        public class DiscController : NetworkBehaviour
        {
            public GameObject follower;
            public GameObject disc;
            public MysticsItemsDasherDiscSpinner discSpinner;
            public GenericOwnership genericOwnership;
            public CharacterBody body;
            public float distance = 1.5f;
            public float tilt = 30f;
            public float rotation = 0f;
            public float rotationSpeed = -45f;
            public bool rotate = true;
            public bool mustTrigger = false;

            public void Awake()
            {
                genericOwnership = GetComponent<GenericOwnership>();
                genericOwnership.onOwnerChanged += delegate (GameObject newOwner)
                {
                    body = newOwner ? newOwner.GetComponent<CharacterBody>() : null;

                    if (body)
                    {
                        distance = 2f * body.radius;
                    }
                };
            }

            public void Update()
            {
                if (rotate) rotation += rotationSpeed * Time.deltaTime;
            }
        }

        public class DiscBaseState : EntityState
        {
            public GenericOwnership genericOwnership;
            public DiscController controller;
            public virtual float DiscSpinBoost => 0f;
            public virtual bool DiscLeavesTrail => false;
            public virtual bool DiscRotatesAroundCharacter => true;
            public Vector3 velocity;

            public override void OnEnter()
            {
                base.OnEnter();
                genericOwnership = GetComponent<GenericOwnership>();
                controller = GetComponent<DiscController>();
                controller.discSpinner.speedMultiplier += DiscSpinBoost;
                controller.discSpinner.SetTrail(DiscLeavesTrail);
                controller.rotate = DiscRotatesAroundCharacter;
            }

            public override void OnExit()
            {
                base.OnExit();
                controller.discSpinner.speedMultiplier -= DiscSpinBoost;
                controller.discSpinner.SetTrail(false);
                controller.rotate = true;
            }

            public override void Update()
            {
                base.Update();
                Transform childTransform = controller.follower.transform;
                childTransform.position = Vector3.SmoothDamp(childTransform.position, controller.body.corePosition, ref velocity, 0.1f, Mathf.Infinity, Time.deltaTime);
                childTransform.localRotation = Quaternion.Euler(new Vector3(
                    controller.tilt * Mathf.Sin(Mathf.Deg2Rad * -controller.rotation),
                    controller.rotation,
                    controller.tilt * Mathf.Cos(Mathf.Deg2Rad * -controller.rotation)
                ));
                controller.disc.transform.localPosition = new Vector3(controller.distance, 0f, 0f);
            }

            public bool IsReady()
            {
                if (controller.body)
                {
                    return !controller.body.HasBuff(MysticsItemsContent.Buffs.MysticsItems_DasherDiscCooldown);
                }
                return false;
            }

            public class Ready : DiscBaseState
            {
                public override void FixedUpdate()
                {
                    base.FixedUpdate();
                    if (isAuthority)
                    {
                        if (NetworkServer.active && !controller.mustTrigger)
                        {
                            if (IsReady() && controller.body)
                            {
                                HealthComponent healthComponent = controller.body.healthComponent;
                                if (healthComponent && healthComponent.isHealthLow && healthComponent.alive)
                                {
                                    controller.mustTrigger = true;
                                    new SyncFireTrigger(controller.gameObject.GetComponent<NetworkIdentity>().netId).Send(NetworkDestination.Clients);
                                }
                            }

                        }
                        if (controller.mustTrigger)
                        {
                            controller.mustTrigger = false;
                            outer.SetNextState(new Trigger());
                        }
                    }
                }

                public class SyncFireTrigger : INetMessage
                {
                    NetworkInstanceId objID;

                    public SyncFireTrigger()
                    {
                    }

                    public SyncFireTrigger(NetworkInstanceId objID)
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
                            DiscController component = obj.GetComponent<DiscController>();
                            if (component)
                            {
                                component.mustTrigger = true;
                            }
                        }
                    }

                    public void Serialize(NetworkWriter writer)
                    {
                        writer.Write(objID);
                    }
                }
            }

            public class Trigger : DiscBaseState
            {
                public override void OnEnter()
                {
                    base.OnEnter();
                    baseDistance = controller.distance;
                    controller.mustTrigger = false;
                    Util.PlaySound("Play_item_proc_dasherdisc", gameObject);

                    if (controller.body)
                    {
                        ModelLocator modelLocator = controller.body.modelLocator;
                        if (modelLocator)
                        {
                            Transform modelTransform = modelLocator.modelTransform;
                            if (modelTransform)
                            {
                                HurtBoxGroup hurtBoxGroup = modelTransform.GetComponent<HurtBoxGroup>();
                                hurtBoxGroup.hurtBoxesDeactivatorCounter++;
                            }
                        }

                        if (NetworkServer.active) controller.body.AddTimedBuff(MysticsItemsContent.Buffs.MysticsItems_DasherDiscActive, DasherDisc.duration);
                    }
                }

                public override void Update()
                {
                    base.Update();
                    controller.distance = Mathf.Lerp(baseDistance, -baseDistance, Mathf.Clamp01(age / discDashDuration));
                }

                public override void FixedUpdate()
                {
                    base.FixedUpdate();
                    if (isAuthority && fixedAge >= minDuration && !controller.body.HasBuff(MysticsItemsContent.Buffs.MysticsItems_DasherDiscActive))
                    {
                        outer.SetNextState(new Ready());
                    }
                }

                public override void OnExit()
                {
                    base.OnExit();
                    if (controller.body)
                    {
                        ModelLocator modelLocator = controller.body.modelLocator;
                        if (modelLocator)
                        {
                            Transform modelTransform = modelLocator.modelTransform;
                            if (modelTransform)
                            {
                                HurtBoxGroup hurtBoxGroup = modelTransform.GetComponent<HurtBoxGroup>();
                                hurtBoxGroup.hurtBoxesDeactivatorCounter--;
                            }
                        }

                        if (NetworkServer.active)
                        {
                            Inventory inventory = controller.body.inventory;
                            float cooldown = CalculateCooldown(inventory ? inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_DasherDisc) : 1);
                            int cooldownSeconds = Mathf.CeilToInt(cooldown);
                            for (int i = 0; i < cooldownSeconds; i++)
                            {
                                controller.body.AddTimedBuff(MysticsItemsContent.Buffs.MysticsItems_DasherDiscCooldown, cooldown / (float)cooldownSeconds * (i + 1));
                            }

                            controller.body.ClearTimedBuffs(MysticsItemsContent.Buffs.MysticsItems_DasherDiscActive);
                        }
                    }
                }

                public float baseDistance;

                public static float minDuration = 1f;
                public static float discDashDuration = 0.2f;
                public override float DiscSpinBoost => 9f;
                public override bool DiscLeavesTrail => true;
                public override bool DiscRotatesAroundCharacter => false;
            }
        }
    }
}
