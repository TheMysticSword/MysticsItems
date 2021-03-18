using EntityStates;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Items
{
    public class DasherDisc : BaseItem
    {
        public static GameObject controllerPrefab;
        public static BuffIndex buffActive;
        public static BuffIndex buffCooldown;

        public override void PreAdd()
        {
            itemDef.name = "DasherDisc";
            itemDef.tier = ItemTier.Tier3;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility
            };
            SetUnlockable();
            SetAssets("Dasher Disc");
            Material mat = model.transform.Find("mdlDasherDisc").GetComponent<MeshRenderer>().sharedMaterial;
            Main.HopooShaderToMaterial.Standard.Apply(mat);
            Main.HopooShaderToMaterial.Standard.Emission(mat, 4f);
            Main.HopooShaderToMaterial.Standard.Gloss(mat);
            Spinner spinner = model.transform.Find("mdlDasherDisc").gameObject.AddComponent<Spinner>();
            spinner.trail = model.transform.Find("mdlDasherDisc").Find("Particle System").gameObject;
            CopyModelToFollower();
            followerModel.transform.Find("mdlDasherDisc").localScale = Vector3.one * 5f;

            controllerPrefab = PrefabAPI.InstantiateClone(new GameObject(), "DasherDiscController", false);
            controllerPrefab.AddComponent<NetworkIdentity>();
            controllerPrefab.AddComponent<GenericOwnership>();
            controllerPrefab.AddComponent<NetworkedBodyAttachment>();
            EntityStateMachine stateMachine = controllerPrefab.AddComponent<EntityStateMachine>();
            stateMachine.mainStateType = stateMachine.initialStateType = new SerializableEntityStateType(typeof(DiscBaseState.Ready));
            controllerPrefab.AddComponent<NetworkStateMachine>();
            DiscController component = controllerPrefab.AddComponent<DiscController>();
            GameObject follower = PrefabAPI.InstantiateClone(followerModel, "DasherDiscFollower", false);
            follower.transform.SetParent(controllerPrefab.transform);
            component.follower = follower;
            component.disc = follower.transform.Find("mdlDasherDisc").gameObject;
            component.discSpinner = component.disc.GetComponent<Spinner>();
            PrefabAPI.RegisterNetworkPrefab(controllerPrefab);

            LoadoutAPI.AddSkill(typeof(DiscBaseState));
            LoadoutAPI.AddSkill(typeof(DiscBaseState.Ready));
            LoadoutAPI.AddSkill(typeof(DiscBaseState.Trigger));
            LoadoutAPI.AddSkill(typeof(DiscBaseState.Invincible));
        }

        public override void OnAdd()
        {
            On.RoR2.CharacterBody.Awake += (orig, self) =>
            {
                orig(self);
                self.onInventoryChanged += delegate ()
                {
                    if (NetworkServer.active) self.AddItemBehavior<DiscBehaviour>(self.inventory.GetItemCount(itemIndex));
                };
            };
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                    MoveType.AfterLabel,
                    x => x.MatchLdarg(1),
                    x => x.MatchLdfld<DamageInfo>("rejected"),
                    x => x.MatchBrfalse(out _)
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldarg_1);
                    c.EmitDelegate<System.Action<HealthComponent, DamageInfo>>((healthComponent, damageInfo) =>
                    {
                        if (healthComponent.body.HasBuff(buffActive)) damageInfo.rejected = true;
                    });
                }
            };
            IL.RoR2.CharacterModel.UpdateRendererMaterials += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                    MoveType.AfterLabel,
                    x => x.MatchLdarg(3),
                    x => x.MatchBrtrue(out _),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterModel>("activeOverlayCount")
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldloc_0);
                    c.Emit(OpCodes.Ldarg_3);
                    c.EmitDelegate<System.Func<CharacterModel, Material, bool, Material>>((characterModel, material, ignoreOverlays) =>
                    {
                        if (characterModel.body && characterModel.visibility >= VisibilityLevel.Visible && !ignoreOverlays)
                        {
                            if (characterModel.body.HasBuff(buffActive)) {
                                return CharacterModel.ghostMaterial;
                            }
                        }
                        return material;
                    });
                    c.Emit(OpCodes.Stloc_0);
                }
            };
            On.RoR2.BuffCatalog.Init += (orig) =>
            {
                orig();
                buffActive = Buffs.BaseBuff.GetFromType(typeof(Buffs.DasherDiscActive));
                buffCooldown = Buffs.BaseBuff.GetFromType(typeof(Buffs.DasherDiscCooldown));
            };
        }

        public class Spinner : MonoBehaviour
        {
            public float baseSpeed = -90f;
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

        public class DiscBehaviour : CharacterBody.ItemBehavior
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
                        while (body.HasBuff(buffActive)) body.RemoveBuff(buffActive);
                        while (body.HasBuff(buffCooldown)) body.ClearTimedBuffs(buffCooldown);
                    }
                }
            }
        }

        public class DiscController : NetworkBehaviour
        {
            public GameObject follower;
            public GameObject disc;
            public Spinner discSpinner;
            public GenericOwnership genericOwnership;
            public CharacterBody body;
            public float distance = 1.5f;
            public float tilt = 30f;
            public float rotation = 0f;
            public float rotationSpeed = 45f;
            public bool rotate = true;

            public void Awake()
            {
                genericOwnership = GetComponent<GenericOwnership>();
                genericOwnership.onOwnerChanged += delegate (GameObject newOwner)
                {
                    body = newOwner ? newOwner.GetComponent<CharacterBody>() : null;

                    if (body)
                    {
                        distance = 1f + body.radius;
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
                childTransform.position = controller.body.corePosition;
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
                    return !controller.body.HasBuff(buffCooldown);
                }
                return false;
            }

            public class Ready : DiscBaseState
            {
                public override void FixedUpdate()
                {
                    base.FixedUpdate();
                    if (IsReady() && controller.body && NetworkServer.active)
                    {
                        HealthComponent healthComponent = controller.body.healthComponent;
                        if (healthComponent && healthComponent.combinedHealthFraction <= healthThreshold)
                        {
                            outer.SetNextState(new Trigger());
                        }
                    }
                }

                public static float healthThreshold = 0.25f;
            }

            public class Trigger : DiscBaseState
            {
                public override void OnEnter()
                {
                    base.OnEnter();
                    baseDistance = controller.distance;
                    Util.PlaySound("Play_item_proc_dasherdisc", gameObject);
                }

                public override void FixedUpdate()
                {
                    base.FixedUpdate();
                    controller.distance = Mathf.Lerp(baseDistance, -baseDistance, Mathf.Clamp01(fixedAge / duration));
                    if (fixedAge >= duration)
                    {
                        outer.SetNextState(new Invincible());
                    }
                }

                public float baseDistance;

                public static float duration = 0.2f;
                public override float DiscSpinBoost => 2f;
                public override bool DiscLeavesTrail => true;
                public override bool DiscRotatesAroundCharacter => false;
            }

            public class Invincible : DiscBaseState
            {
                public override void OnEnter()
                {
                    base.OnEnter();
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

                        if (NetworkServer.active) controller.body.AddBuff(buffActive);
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
                            float cooldown = 60f;
                            Inventory inventory = controller.body.inventory;
                            if (inventory)
                            {
                                cooldown /= 1f + 0.2f * (inventory.GetItemCount(GetFromType(typeof(DasherDisc)).itemIndex) - 1);
                            }
                            int cooldownSeconds = Mathf.CeilToInt(cooldown);
                            for (int i = 0; i < cooldownSeconds; i++)
                            {
                                controller.body.AddTimedBuff(buffCooldown, cooldown / (float)cooldownSeconds * (i + 1));
                            }

                            if (controller.body.HasBuff(buffActive)) controller.body.RemoveBuff(buffActive);
                        }
                    }
                }

                public override void FixedUpdate()
                {
                    base.FixedUpdate();
                    if (fixedAge >= duration)
                    {
                        outer.SetNextState(new Ready());
                    }
                }

                public static float duration = 6f;
                public override float DiscSpinBoost => 2f;
                public override bool DiscRotatesAroundCharacter => false;
            }
        }
    }
}
