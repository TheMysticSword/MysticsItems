using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Audio;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Equipment
{
    public class MechanicalArm : BaseEquipment
    {
        public static GameObject mechanicalArmControllerPrefab;

        public static ConfigurableValue<float> baseCrit = new ConfigurableValue<float>(
            "Equipment: Mechanical Arm",
            "BaseCrit",
            5f,
            "Critical Strike chance bonus when carrying this item",
            new List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_MECHANICALARM_DESC"
            }
        );
        public static ConfigurableValue<float> damage = new ConfigurableValue<float>(
            "Equipment: Mechanical Arm",
            "Damage",
            1000f,
            "Swing damage (in %)",
            new List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_MECHANICALARM_DESC"
            }
        );
        public static ConfigurableValue<float> damageBonusPerCharge = new ConfigurableValue<float>(
            "Equipment: Mechanical Arm",
            "DamageBonusPerCharge",
            200f,
            "How much more damage (in %) should the swing do for each Critical Strike dealt before activation",
            new List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_MECHANICALARM_DESC"
            }
        );
        public static ConfigurableValue<float> procCoefficient = new ConfigurableValue<float>(
            "Equipment: Mechanical Arm",
            "ProcCoefficient",
            1f,
            "Swing proc coefficient",
            new List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_MECHANICALARM_DESC"
            }
        );

        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            mechanicalArmControllerPrefab = MysticsRisky2Utils.Utils.CreateBlankPrefab("MysticsItems_MechanicalArmController", true);
            mechanicalArmControllerPrefab.GetComponent<NetworkIdentity>().localPlayerAuthority = false;
        }

        public override void OnLoad()
        {
            equipmentDef.name = "MysticsItems_MechanicalArm";
            ConfigManager.Balance.CreateEquipmentCooldownOption(equipmentDef, "Equipment: Mechanical Arm", 20f);
            equipmentDef.canDrop = true;
            ConfigManager.Balance.CreateEquipmentEnigmaCompatibleOption(equipmentDef, "Equipment: Mechanical Arm", true);
            ConfigManager.Balance.CreateEquipmentCanBeRandomlyTriggeredOption(equipmentDef, "Equipment: Mechanical Arm", false);
            equipmentDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Mechanical Arm/Model.prefab"));
            equipmentDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Equipment/Mechanical Arm/Icon.png");

            void ApplyToModels(GameObject model)
            {
                var mat = model.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial;
                HopooShaderToMaterial.Standard.Apply(mat);
                HopooShaderToMaterial.Standard.Gloss(mat);
                HopooShaderToMaterial.Standard.Emission(mat, 1.5f, new Color32(191, 15, 3, 255));
            }
            ApplyToModels(equipmentDef.pickupModelPrefab);

            ModelPanelParameters modelPanelParameters = equipmentDef.pickupModelPrefab.GetComponent<ModelPanelParameters>();
            modelPanelParameters.minDistance = 6f;
            modelPanelParameters.maxDistance = 12f;

            itemDisplayPrefab = PrepareItemDisplayModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Mechanical Arm/MovingModel.prefab"));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Chest", new Vector3(0.07816F, 0.25502F, -0.15061F), new Vector3(0F, 180F, 0F), new Vector3(0.07862F, 0.07862F, 0.07862F));
                AddDisplayRule("HuntressBody", "Chest", new Vector3(0.10676F, 0.14576F, -0.04849F), new Vector3(0F, 180F, 0F), new Vector3(0.03776F, 0.03776F, 0.03776F));
                AddDisplayRule("Bandit2Body", "Chest", new Vector3(0.00326F, 0.18527F, -0.12067F), new Vector3(0F, 185.5073F, 0F), new Vector3(0.05941F, 0.05941F, 0.05941F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(2.63852F, 1.90061F, 0.02656F), new Vector3(0F, 180F, 0F), new Vector3(0.56946F, 0.56946F, 0.56946F));
                AddDisplayRule("EngiBody", "Chest", new Vector3(0.01994F, 0.13958F, -0.23417F), new Vector3(0F, 180F, 0F), new Vector3(0.06142F, 0.06142F, 0.06142F));
                AddDisplayRule("MageBody", "Chest", new Vector3(0.09002F, 0.03363F, -0.2332F), new Vector3(0F, 175.5407F, 0F), new Vector3(0.05358F, 0.05358F, 0.05358F));
                AddDisplayRule("MercBody", "Chest", new Vector3(0.00246F, 0.14756F, -0.16344F), new Vector3(0F, 202.2791F, 0F), new Vector3(0.08846F, 0.08846F, 0.08846F));
                AddDisplayRule("TreebotBody", "PlatformBase", new Vector3(0.53248F, 0.66126F, 0F), new Vector3(0F, 180F, 0F), new Vector3(0.19873F, 0.19873F, 0.19873F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(-0.03249F, 0.00003F, -0.06065F), new Vector3(0F, 206.8929F, 0F), new Vector3(0.09891F, 0.09891F, 0.09796F));
                AddDisplayRule("CrocoBody", "SpineChest2", new Vector3(0F, -0.86934F, 0.00013F), new Vector3(0F, 0F, 0F), new Vector3(0.83816F, 0.83816F, 0.83816F));
                AddDisplayRule("CaptainBody", "Chest", new Vector3(-0.00812F, 0.17595F, -0.14316F), new Vector3(0F, 218.1953F, 0F), new Vector3(0.09604F, 0.09604F, 0.09604F));
                AddDisplayRule("ScavBody", "Backpack", new Vector3(6.49963F, 6.38849F, 0.00031F), new Vector3(0F, 180F, 0F), new Vector3(2.02387F, 2.02387F, 2.02387F));
                AddDisplayRule("EquipmentDroneBody", "HeadCenter", new Vector3(0.4758F, 0F, -0.48188F), new Vector3(270F, 180F, 0F), new Vector3(0.42601F, 0.42601F, 0.42601F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Chest", new Vector3(0.1706F, 0.13246F, -0.20744F), new Vector3(0F, 180F, 0F), new Vector3(0.053F, 0.053F, 0.053F));
                AddDisplayRule("RailgunnerBody", "Backpack", new Vector3(0.08517F, 0.21948F, 0.00002F), new Vector3(0F, 180F, 0F), new Vector3(0.09344F, 0.09344F, 0.09344F));
                AddDisplayRule("VoidSurvivorBody", "Center", new Vector3(-0.0074F, 0.3145F, -0.08637F), new Vector3(359.7796F, 219.7314F, 0.26983F), new Vector3(0.07043F, 0.07043F, 0.07043F));
            };

            ChildLocator childLocator = itemDisplayPrefab.AddComponent<ChildLocator>();

            NetworkedBodyAttachment networkedBodyAttachment = mechanicalArmControllerPrefab.AddComponent<NetworkedBodyAttachment>();
            networkedBodyAttachment.shouldParentToAttachedBody = true;
            networkedBodyAttachment.forceHostAuthority = true;

            EntityStateMachine entityStateMachine = mechanicalArmControllerPrefab.AddComponent<EntityStateMachine>();
            entityStateMachine.initialStateType = entityStateMachine.mainStateType = new EntityStates.SerializableEntityStateType(typeof(MysticsItemsMechanicalArmState.Idle));

            NetworkStateMachine networkStateMachine = mechanicalArmControllerPrefab.AddComponent<NetworkStateMachine>();
            networkStateMachine.SetFieldValue("stateMachines", new EntityStateMachine[] {
                entityStateMachine
            });

            MysticsItemsContent.Resources.entityStateTypes.Add(typeof(MysticsItemsMechanicalArmState));
            MysticsItemsContent.Resources.entityStateTypes.Add(typeof(MysticsItemsMechanicalArmState.Idle));
            MysticsItemsContent.Resources.entityStateTypes.Add(typeof(MysticsItemsMechanicalArmState.Swing));

            ModelLocator modelLocator = mechanicalArmControllerPrefab.AddComponent<ModelLocator>();
            modelLocator.dontReleaseModelOnDeath = false;
            modelLocator.autoUpdateModelTransform = false;
            modelLocator.dontDetatchFromParent = true;
            modelLocator.preserveModel = true;

            PrefabAPI.InstantiateClone(
                Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Mechanical Arm/MovingModel.prefab")
                    .transform.Find("HitboxGroup").gameObject,
                "HitboxGroup",
                false
            ).transform.SetParent(mechanicalArmControllerPrefab.transform);
            Object.Destroy(itemDisplayPrefab.transform.Find("HitboxGroup").gameObject);
            HitBoxGroup hitBoxGroup = mechanicalArmControllerPrefab.transform.Find("HitboxGroup").gameObject.AddComponent<HitBoxGroup>();
            hitBoxGroup.groupName = "MysticsItems_MechanicalArmSwing";
            hitBoxGroup.hitBoxes = new HitBox[]
            {
                mechanicalArmControllerPrefab.transform.Find("HitboxGroup/Hitbox").gameObject.AddComponent<HitBox>()
            };
            hitBoxGroup.gameObject.SetActive(true);

            AnimationCurve MakeGenericCurve()
            {
                return new AnimationCurve { keys = new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 1) } };
            }

            Transform armRoot = null;
            foreach (var child in itemDisplayPrefab.GetComponentsInChildren<Transform>())
            {
                var childName = child.name;
                if (childName == "arm.1")
                {
                    armRoot = child;
                    var ntp = new ChildLocator.NameTransformPair
                    {
                        name = "Arm1",
                        transform = child
                    };
                    HGArrayUtilities.ArrayAppend(ref childLocator.transformPairs, ref ntp);
                }
                else
                {
                    if (childName.StartsWith("arm.", false, System.Globalization.CultureInfo.InvariantCulture))
                    {
                        var ntp = new ChildLocator.NameTransformPair
                        {
                            name = "Arm" + childName.Remove(0, "arm.".Length),
                            transform = child
                        };
                        HGArrayUtilities.ArrayAppend(ref childLocator.transformPairs, ref ntp);

                        DynamicBone dynamicBone = child.gameObject.AddComponent<DynamicBone>();
                        dynamicBone.m_Root = child;
                        dynamicBone.m_UpdateRate = 60;
                        dynamicBone.m_UpdateMode = DynamicBone.UpdateMode.Normal;
                        dynamicBone.m_Damping = 0.8f;
                        dynamicBone.m_DampingDistrib = MakeGenericCurve();
                        dynamicBone.m_Elasticity = 0.1f;
                        dynamicBone.m_ElasticityDistrib = MakeGenericCurve();
                        dynamicBone.m_Stiffness = 0.9f;
                        dynamicBone.m_StiffnessDistrib = MakeGenericCurve();
                        dynamicBone.m_Inert = 0f;
                        dynamicBone.m_InertDistrib = MakeGenericCurve();
                        dynamicBone.m_Radius = 0f;
                        dynamicBone.m_RadiusDistrib = MakeGenericCurve();
                    }
                }
            }

            On.RoR2.EquipmentSlot.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
                {
                    MysticsItemsMechanicalArmState armController = MysticsItemsMechanicalArmState.FindMechanicalArmController(self.gameObject);

                    bool carryingThisEquipment = self.equipmentIndex == equipmentDef.equipmentIndex;
                    if (carryingThisEquipment != (armController != null))
                    {
                        if (carryingThisEquipment)
                        {
                            var armControllerInstance = Object.Instantiate<GameObject>(mechanicalArmControllerPrefab);
                            armControllerInstance.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(self.gameObject);
                            return;
                        }
                        Object.Destroy(armController.gameObject);
                    }
                }
            };

            var swingEffectPrefab = MysticsItemsMechanicalArmState.Swing.swingEffectPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Mechanical Arm/SwingEffect.prefab");
            ScaleParticleSystemDuration scaleParticleSystemDuration = swingEffectPrefab.AddComponent<ScaleParticleSystemDuration>();
            scaleParticleSystemDuration.particleSystems = swingEffectPrefab.GetComponentsInChildren<ParticleSystem>();
            scaleParticleSystemDuration.initialDuration = 1f;
            ShakeEmitter shakeEmitter = swingEffectPrefab.AddComponent<ShakeEmitter>();
            shakeEmitter.duration = 0.2f;
            shakeEmitter.radius = 20f;
            shakeEmitter.wave = new Wave
            {
                amplitude = 4f,
                frequency = 4f
            };
            shakeEmitter.amplitudeTimeDecay = true;
            shakeEmitter.shakeOnStart = true;
            shakeEmitter.shakeOnEnable = false;

            MysticsItemsMechanicalArmState.Swing.hitEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/SawmerangImpact");
            var snd = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            snd.eventName = "MysticsItems_Play_mechanicalArm_impact";
            MysticsItemsContent.Resources.networkSoundEventDefs.Add(snd);
            MysticsItemsMechanicalArmState.Swing.impactSound = snd;

            GenericGameEvents.OnHitEnemy += GenericGameEvents_OnHitEnemy;

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.equipmentSlot && sender.equipmentSlot.equipmentIndex == equipmentDef.equipmentIndex)
            {
                args.critAdd += baseCrit.Value;
            }
        }

        private void GenericGameEvents_OnHitEnemy(DamageInfo damageInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo attackerInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo victimInfo)
        {
            if (!damageInfo.rejected && damageInfo.procCoefficient > 0f && damageInfo.crit && attackerInfo.body && attackerInfo.body.equipmentSlot && attackerInfo.body.equipmentSlot.equipmentIndex == equipmentDef.equipmentIndex && attackerInfo.body.equipmentSlot.cooldownTimer > 0f && !float.IsInfinity(attackerInfo.body.equipmentSlot.cooldownTimer))
            {
                attackerInfo.body.AddBuff(MysticsItemsContent.Buffs.MysticsItems_MechanicalArmCharge);
            }
        }

        public override bool OnUse(EquipmentSlot equipmentSlot)
        {
            MysticsItemsMechanicalArmState armController = MysticsItemsMechanicalArmState.FindMechanicalArmController(equipmentSlot.gameObject);
            if (armController != null && armController.outer.state is MysticsItemsMechanicalArmState.Idle)
            {
                armController.outer.SetNextState(new MysticsItemsMechanicalArmState.Swing());
                return true;
            }
            return false;
        }

        public class MysticsItemsMechanicalArmState : EntityStates.EntityState
        {
            public NetworkedBodyAttachment networkedBodyAttachment;
            public CharacterBody body;
            public GameObject bodyObject;
            public CharacterMaster bodyMaster;
            public InputBankTest bodyInputBank;
            public EquipmentSlot bodyEquipmentSlot;

            public Transform armTransform;
            public ChildLocator armChildLocator;
            public Animator armAnimator;

            public bool linkedToDisplay = false;

            private static readonly List<MysticsItemsMechanicalArmState> instancesList = new List<MysticsItemsMechanicalArmState>();

            public static MysticsItemsMechanicalArmState FindMechanicalArmController(GameObject targetObject)
            {
                if (!targetObject) return null;
                foreach (var instance in instancesList)
                {
                    if (instance.networkedBodyAttachment.attachedBodyObject == targetObject) return instance;
                }
                return null;
            }

            public override void OnEnter()
            {
                base.OnEnter();
                networkedBodyAttachment = GetComponent<NetworkedBodyAttachment>();
                if (networkedBodyAttachment)
                {
                    body = networkedBodyAttachment.attachedBody;
                    bodyObject = networkedBodyAttachment.attachedBodyObject;
                    if (bodyObject)
                    {
                        bodyMaster = body.master;
                        bodyInputBank = bodyObject.GetComponent<InputBankTest>();
                        bodyEquipmentSlot = body.equipmentSlot;
                        LinkToDisplay();
                    }
                }
                instancesList.Add(this);
            }

            public void LinkToDisplay()
            {
                if (linkedToDisplay) return;
                if (bodyEquipmentSlot)
                {
                    armTransform = bodyEquipmentSlot.FindActiveEquipmentDisplay();
                    if (armTransform)
                    {
                        armChildLocator = armTransform.GetComponentInChildren<ChildLocator>();
                        if (armChildLocator && modelLocator)
                        {
                            modelLocator.modelTransform = armChildLocator.transform;
                            armAnimator = GetModelAnimator();
                            linkedToDisplay = true;
                        }
                    }
                }
            }

            public override void FixedUpdate()
            {
                base.FixedUpdate();
                LinkToDisplay();
            }

            public override void OnExit()
            {
                base.OnExit();
                instancesList.Remove(this);
            }

            public class Idle : MysticsItemsMechanicalArmState
            {

            }

            public class Swing : MysticsItemsMechanicalArmState
            {
                public float attackSpeedStat;
                public float duration;
                public GameObject swingEffect;

                public float meleeAttackDelay;
                public float hitPauseTime;
                public float attackFireDuration;

                public int totalHitTicks = 0;
                public Vector3 storedHitPauseVelocity = Vector3.zero;

                public bool crit;
                public HitBoxGroup hitBoxGroup;
                public OverlapAttack overlapAttack;
                public uint soundID;

                public override void OnEnter()
                {
                    base.OnEnter();

                    attackSpeedStat = (body ? body.attackSpeed : 1f) * baseAttackSpeedMultiplier;

                    duration = baseDuration / attackSpeedStat;
                    if (armAnimator) PlayAnimationOnAnimator(armAnimator, "Additive", "Swing", "Swing.playbackRate", duration);
                    soundID = Util.PlayAttackSpeedSound(initialSoundString, gameObject, attackSpeedStat);

                    meleeAttackDelay = baseMeleeAttackDelay / attackSpeedStat;
                    attackFireDuration = baseAttackFireDuration / attackSpeedStat;

                    UpdateTransform();

                    if (isAuthority)
                    {
                        //crit = body && bodyMaster && Util.CheckRoll(body.crit, bodyMaster);

                        hitBoxGroup = gameObject.GetComponentInChildren<HitBoxGroup>();

                        if (hitBoxGroup)
                        {
                            OverlapAttack overlapAttack = new OverlapAttack();
                            overlapAttack.attacker = body ? body.gameObject : gameObject;
                            overlapAttack.damage = (body ? body.damage : 1f) * (damageCoefficient + damageCoefficientPerCharge * (body ? (float)body.GetBuffCount(MysticsItemsContent.Buffs.MysticsItems_MechanicalArmCharge) : 0f));
                            overlapAttack.damageColorIndex = DamageColorIndex.Default;
                            overlapAttack.damageType = DamageType.Generic;
                            overlapAttack.forceVector = forceVector;
                            overlapAttack.hitBoxGroup = hitBoxGroup;
                            overlapAttack.hitEffectPrefab = hitEffectPrefab;
                            NetworkSoundEventDef networkSoundEventDef = impactSound;
                            overlapAttack.impactSound = (networkSoundEventDef != null) ? networkSoundEventDef.index : NetworkSoundEventIndex.Invalid;
                            overlapAttack.inflictor = gameObject;
                            overlapAttack.isCrit = crit;
                            overlapAttack.procChainMask = default(ProcChainMask);
                            overlapAttack.pushAwayForce = pushAwayForce;
                            overlapAttack.procCoefficient = procCoefficient;
                            overlapAttack.teamIndex = TeamComponent.GetObjectTeam(body ? body.gameObject : gameObject);
                            this.overlapAttack = overlapAttack;

                            if (NetworkServer.active)
                            {
                                if (body)
                                {
                                    var whiletries = 1000;
                                    while (body.HasBuff(MysticsItemsContent.Buffs.MysticsItems_MechanicalArmCharge) && whiletries-- > 0)
                                        body.RemoveBuff(MysticsItemsContent.Buffs.MysticsItems_MechanicalArmCharge);
                                }
                                /*
                                while (body && body.HasBuff(MysticsItemsContent.Buffs.MysticsItems_MechanicalArmCharge))
                                    body.RemoveBuff(MysticsItemsContent.Buffs.MysticsItems_MechanicalArmCharge);
                                */
                            }
                        }
                    }

                    if (armChildLocator)
                    {
                        for (var i = 1; i <= 6; i++)
                        {
                            var armBone = armChildLocator.FindChild("Arm" + i);
                            if (armBone)
                            {
                                DynamicBone dynamicBone = armBone.GetComponent<DynamicBone>();
                                if (dynamicBone)
                                {
                                    dynamicBone.enabled = false;
                                }
                            }
                        }
                    }
                }

                public override void FixedUpdate()
                {
                    base.FixedUpdate();
                    duration -= Time.fixedDeltaTime;
                    if (meleeAttackDelay > 0)
                    {
                        meleeAttackDelay -= Time.fixedDeltaTime;
                        if (meleeAttackDelay <= 0f)
                        {
                            UpdateTransform();
                            CreateSwingEffect();
                        }
                    }
                    else
                    {
                        if (hitPauseTime > 0)
                        {
                            hitPauseTime -= Time.fixedDeltaTime;

                            if (hitPauseTime <= 0)
                            {
                                ExitHitPause();
                            }
                        }

                        if (isAuthority)
                        {
                            if (attackFireDuration > 0)
                            {
                                FireAttack();
                                attackFireDuration -= Time.fixedDeltaTime;
                            }
                        }
                    }
                    if (duration <= 0) outer.SetNextStateToMain();
                }

                public void UpdateTransform()
                {
                    if (bodyInputBank)
                    {
                        var vector = bodyInputBank.aimDirection;
                        vector.Normalize();
                        transform.localRotation = Util.QuaternionSafeLookRotation(vector, Vector3.up);
                    }
                    if (body)
                    {
                        transform.localScale = Vector3.one * body.radius;
                    }
                }

                public void CreateSwingEffect()
                {
                    swingEffect = Object.Instantiate<GameObject>(swingEffectPrefab, transform);
                    ScaleParticleSystemDuration component = swingEffect.GetComponent<ScaleParticleSystemDuration>();
                    if (component)
                    {
                        component.newDuration = component.initialDuration / particleSystemSimulationSpeed;
                    }
                    Util.PlayAttackSpeedSound("MysticsItems_Play_mechanicalArm_swing", gameObject, attackSpeedStat);
                }

                public void FireAttack()
                {
                    if (overlapAttack != null)
                    {
                        var hitThisFixedUpdate = overlapAttack.Fire();
                        totalHitTicks++;
                        if (hitThisFixedUpdate)
                        {
                            if (body.characterMotor)
                            {
                                storedHitPauseVelocity += body.characterMotor.velocity;
                                body.characterMotor.velocity = Vector3.zero;
                            }
                            if (armAnimator)
                            {
                                armAnimator.speed = 0f;
                            }
                            if (swingEffect)
                            {
                                ScaleParticleSystemDuration component = swingEffect.GetComponent<ScaleParticleSystemDuration>();
                                if (component)
                                {
                                    component.newDuration = 20f;
                                }
                            }
                            hitPauseTime = hitPauseDuration / attackSpeedStat;
                        }
                    }
                }

                public void ExitHitPause()
                {
                    hitPauseTime = 0f;

                    if (body.characterMotor) body.characterMotor.velocity = storedHitPauseVelocity;
                    storedHitPauseVelocity = Vector3.zero;

                    if (armAnimator) armAnimator.speed = 1f;

                    if (swingEffect)
                    {
                        ScaleParticleSystemDuration component = swingEffect.GetComponent<ScaleParticleSystemDuration>();
                        if (component)
                        {
                            component.newDuration = component.initialDuration / particleSystemSimulationSpeed;
                        }
                    }
                }

                public override void OnExit()
                {
                    base.OnExit();
                    
                    if (isAuthority)
                    {
                        if (totalHitTicks <= 0)
                        {
                            CreateSwingEffect();
                            FireAttack();
                        }
                        if (hitPauseTime > 0) ExitHitPause();
                    }

                    if (swingEffect) Object.Destroy(swingEffect);
                    if (armAnimator) armAnimator.speed = 1f;

                    if (armChildLocator)
                    {
                        for (var i = 1; i <= 6; i++)
                        {
                            var armBone = armChildLocator.FindChild("Arm" + i);
                            if (armBone)
                            {
                                DynamicBone dynamicBone = armBone.GetComponent<DynamicBone>();
                                if (dynamicBone)
                                {
                                    dynamicBone.enabled = true;
                                }
                            }
                        }
                    }
                }

                public static float baseDuration = 100f / 24f;
                public static float baseAttackSpeedMultiplier = 2f;
                public static float baseMeleeAttackDelay = 30f / 24f;
                public static float baseAttackFireDuration = 0.1f;
                public static string initialSoundString = "MysticsItems_Play_mechanicalArm_swing_prepare";
                public static float damageCoefficient = MechanicalArm.damage.Value / 100f;
                public static float damageCoefficientPerCharge = MechanicalArm.damageBonusPerCharge.Value / 100f;
                public static Vector3 forceVector = Vector3.zero;
                public static float pushAwayForce = 6000f;
                public static float procCoefficient = MechanicalArm.procCoefficient.Value;
                public static GameObject hitEffectPrefab;
                public static GameObject swingEffectPrefab;
                public static NetworkSoundEventDef impactSound;
                public static float hitPauseDuration = 0.2f;
                public static float particleSystemSimulationSpeed = 1.3f;
            }
        }
    }
}
