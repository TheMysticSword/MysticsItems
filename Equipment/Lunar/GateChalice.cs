using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using R2API.Utils;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;
using System.Collections.ObjectModel;
using RoR2.UI;
using MysticsRisky2Utils;
using System.Linq;
using System.Collections.Generic;
using MysticsRisky2Utils.BaseAssetTypes;
using static MysticsItems.LegacyBalanceConfigManager;
using RoR2.Navigation;

namespace MysticsItems.Equipment
{
    public class GateChalice : BaseEquipment
    {
        public static GameObject visualEffectOnUse;
        public static GameObject visualEffectTeleportOut;
        public static GameObject sceneExitControllerObject;

        public static GameObject itemDestroyEffectPrefab;

        public static Dictionary<string, List<string>> destinationOverrides = new Dictionary<string, List<string>>()
        {
            { "limbo", new List<string>() { "moon2" } },
            { "arena", new List<string>() { "voidstage" } },
            { "voidstage", new List<string>() { "voidraid" } }
        };

        public static List<string> cannotSkipStages = new List<string>()
        {
            "moon2", "voidraid", "bazaar"
        };

        public struct SpecialStageTeleportationInfo
        {
            public Bounds bounds;
            public List<Vector3> destinations;
        }
        public static Dictionary<string, SpecialStageTeleportationInfo> specialStageTeleportation = new Dictionary<string, SpecialStageTeleportationInfo>()
        {
            { "moon2", new SpecialStageTeleportationInfo
                {
                    bounds = new Bounds
                    {
                        center = new Vector3(-87.11757f, 566.5618f, 5.444388f),
                        size = new Vector3(500f, 250f, 500f)
                    },
                    destinations = new List<Vector3>()
                    {
                        new Vector3(-179.5247f, 497.6573f, -223.9494f),
                        new Vector3(7.340976f, 497.6573f, -225.1836f),
                        new Vector3(138.1888f, 497.6573f, -93.66596f),
                        new Vector3(137.9696f, 497.6573f, 95.98901f),
                        new Vector3(1.488069f, 497.6573f, 227.6736f),
                        new Vector3(-181.7953f, 497.6573f, 224.1614f),
                        new Vector3(-312.913f, 497.6573f, 93.92822f),
                        new Vector3(-312.8959f, 497.6573f, -97.17789f)
                    }
                }
            }
        };
        
        public override void OnPluginAwake()
        {
            sceneExitControllerObject = MysticsRisky2Utils.Utils.CreateBlankPrefab("MysticsItems_GateChaliceSceneExitControllerObject", true);
        }

        public override void OnLoad()
        {
            base.OnLoad();
            equipmentDef.name = "MysticsItems_GateChalice";
            ConfigManager.Balance.CreateEquipmentCooldownOption(equipmentDef, "Equipment: Gate Chalice", 60f);
            equipmentDef.canDrop = true;
            ConfigManager.Balance.CreateEquipmentEnigmaCompatibleOption(equipmentDef, "Equipment: Gate Chalice", false);
            ConfigManager.Balance.CreateEquipmentCanBeRandomlyTriggeredOption(equipmentDef, "Equipment: Gate Chalice", false);
            equipmentDef.isLunar = true;
            equipmentDef.colorIndex = ColorCatalog.ColorIndex.LunarItem;
            equipmentDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Gate Chalice/Model.prefab"));
            equipmentDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Equipment/Gate Chalice/Icon.png");

            Material mat = equipmentDef.pickupModelPrefab.transform.Find("mdlGateChalice").gameObject.GetComponent<MeshRenderer>().sharedMaterial;
            HopooShaderToMaterial.Standard.Apply(mat);
            HopooShaderToMaterial.Standard.Gloss(mat, 0.5f);
            HopooShaderToMaterial.Standard.Emission(mat, 0.02f, new Color(48f / 255f, 127f / 255f, 255f / 255f));
            foreach (Transform lightTransform in equipmentDef.pickupModelPrefab.transform.Find("mdlGateChalice").Find("Lights"))
            {
                SetScalableChildEffect(equipmentDef.pickupModelPrefab, lightTransform.gameObject);
                FlickerLight flickerLight = lightTransform.gameObject.AddComponent<FlickerLight>();
                flickerLight.light = lightTransform.gameObject.GetComponent<Light>();
                flickerLight.sinWaves = new Wave[] {
                    new Wave {
                        amplitude = 0.3f,
                        frequency = 4f
                    },
                    new Wave {
                        amplitude = 0.6f,
                        frequency = 2f
                    },
                    new Wave {
                        amplitude = 0.9f,
                        frequency = 1f
                    }
                };
            }
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(equipmentDef.pickupModelPrefab, equipmentDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Stomach", new Vector3(-0.09F, 0.1F, -0.102F), new Vector3(5.862F, 140.357F, 1.915F), new Vector3(0.059F, 0.059F, 0.059F));
                AddDisplayRule("HuntressBody", "Pelvis", new Vector3(-0.082F, -0.111F, 0.085F), new Vector3(0.679F, 36.762F, 188.148F), new Vector3(0.047F, 0.047F, 0.048F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(-0.096F, 0.027F, -0.151F), new Vector3(337.162F, 337.663F, 11.532F), new Vector3(0.04F, 0.04F, 0.04F));
                AddDisplayRule("ToolbotBody", "Hip", new Vector3(-1.239F, 0.577F, -1.044F), new Vector3(0F, 180F, 180F), new Vector3(0.349F, 0.349F, 0.349F));
                AddDisplayRule("EngiBody", "Pelvis", new Vector3(-0.178F, 0.078F, 0.157F), new Vector3(11.745F, 186.295F, 185.936F), new Vector3(0.047F, 0.047F, 0.047F));
                AddDisplayRule("MageBody", "Pelvis", new Vector3(-0.128F, -0.131F, 0.024F), new Vector3(6.286F, 3.408F, 167.572F), new Vector3(0.044F, 0.044F, 0.044F));
                AddDisplayRule("MercBody", "Chest", new Vector3(0F, 0.193F, -0.286F), new Vector3(71.925F, 180F, 0F), new Vector3(0.027F, 0.027F, 0.027F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(-0.485F, 0.701F, -0.803F), new Vector3(26.173F, 24.306F, 86.838F), new Vector3(0.061F, 0.061F, 0.061F));
                AddDisplayRule("LoaderBody", "Pelvis", new Vector3(-0.216F, -0.016F, -0.022F), new Vector3(342.363F, 183.205F, 159.555F), new Vector3(0.045F, 0.045F, 0.045F));
                AddDisplayRule("CrocoBody", "SpineStomach1", new Vector3(0.845F, 0.495F, 1.289F), new Vector3(74.633F, 327.618F, 247.859F), new Vector3(0.361F, 0.361F, 0.361F));
                AddDisplayRule("CaptainBody", "Stomach", new Vector3(-0.195F, 0.128F, 0.126F), new Vector3(336.504F, 156.734F, 358.159F), new Vector3(0.041F, 0.041F, 0.041F));
                AddDisplayRule("ScavBody", "MuzzleEnergyCannon", new Vector3(0F, 0F, -1.503F), new Vector3(90F, 0F, 0F), new Vector3(2.281F, 2.281F, 2.281F));
                AddDisplayRule("EquipmentDroneBody", "GunBarrelBase", new Vector3(0F, 0F, 1.069F), new Vector3(0F, 0F, 0F), new Vector3(0.267F, 0.267F, 0.267F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Pelvis", new Vector3(0.19432F, 0.18834F, -0.17385F), new Vector3(343.5044F, 339.549F, 357.2065F), new Vector3(0.05181F, 0.05181F, 0.05181F));
                AddDisplayRule("RailgunnerBody", "Pelvis", new Vector3(0.14878F, -0.03065F, 0.10522F), new Vector3(354.5591F, 180F, 180F), new Vector3(0.05639F, 0.05639F, 0.05639F));
                AddDisplayRule("VoidSurvivorBody", "Hand", new Vector3(-0.03581F, 0.28639F, -0.00539F), new Vector3(0F, 0F, 4.98446F), new Vector3(0.09781F, 0.09781F, 0.09781F));
            };
            
            visualEffectOnUse = PrefabAPI.InstantiateClone(new GameObject(), "MysticsItems_GateChaliceOnUseEffect", false);
            EffectComponent effectComponent = visualEffectOnUse.AddComponent<EffectComponent>();
            effectComponent.soundName = "Play_env_teleporter_active_button";
            effectComponent.positionAtReferencedTransform = true;
            effectComponent.applyScale = true;
            effectComponent.disregardZScale = true;
            VFXAttributes vfxAttributes = visualEffectOnUse.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Low;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            visualEffectOnUse.AddComponent<DestroyOnTimer>().duration = 2f;

            GameObject massSparks = PrefabAPI.InstantiateClone(new GameObject(), "MassSparks", false);
            ParticleSystem particleSystem = massSparks.AddComponent<ParticleSystem>();
            massSparks.GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ActivateRadarTowerEffect").transform.Find("MassSparks").gameObject.GetComponent<ParticleSystemRenderer>().material);
            particleSystem.useAutoRandomSeed = true;
            ParticleSystem.MainModule mainModule = particleSystem.main;
            mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
            mainModule.scalingMode = ParticleSystemScalingMode.Local;
            mainModule.startLifetime = 5f;
            mainModule.duration = 1f;
            mainModule.playOnAwake = true;
            ParticleSystem.MinMaxCurve particleSpeed = mainModule.startSpeed;
            particleSpeed.mode = ParticleSystemCurveMode.TwoConstants;
            particleSpeed.constantMin = 10f;
            particleSpeed.constantMax = 1000f;
            mainModule.startSize = 0.1f;
            mainModule.startColor = new Color(48f / 255f, 127f / 255f, 255f / 255f);
            mainModule.gravityModifier = 0.3f;
            mainModule.maxParticles = 20;
            ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
            emissionModule.enabled = true;
            emissionModule.rateOverTime = 10;
            emissionModule.rateOverDistance = 0;
            emissionModule.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst
                {
                    time = 0f,
                    count = 20f,
                    cycleCount = 1,
                    repeatInterval = 0.01f,
                    probability = 1f
                }
            });
            ParticleSystem.ShapeModule shapeModule = particleSystem.shape;
            shapeModule.shapeType = ParticleSystemShapeType.Sphere;
            massSparks.transform.SetParent(visualEffectOnUse.transform);

            MysticsItemsContent.Resources.effectPrefabs.Add(visualEffectOnUse);

            visualEffectTeleportOut = PrefabAPI.InstantiateClone(new GameObject(), "MysticsItems_GateChaliceTeleportOutEffect", false);
            effectComponent = visualEffectTeleportOut.AddComponent<EffectComponent>();
            vfxAttributes = visualEffectTeleportOut.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
            visualEffectTeleportOut.AddComponent<DestroyOnTimer>().duration = 4f;

            GameObject ppHolder = PrefabAPI.InstantiateClone(new GameObject(), "PP", false);
            ppHolder.layer = LayerIndex.postProcess.intVal;
            ppHolder.AddComponent<MysticsItemsGateChalicePPController>().time = 3f;
            PostProcessVolume pp = ppHolder.AddComponent<PostProcessVolume>();
            pp.isGlobal = true;
            pp.weight = 0f;
            pp.priority = 50;
            PostProcessProfile ppProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
            ppProfile.name = "ppGateChalice";
            LensDistortion lensDistortion = ppProfile.AddSettings<LensDistortion>();
            lensDistortion.SetAllOverridesTo(true);
            lensDistortion.intensity.value = -100f;
            lensDistortion.scale.value = 1f;
            ColorGrading colorGrading = ppProfile.AddSettings<ColorGrading>();
            colorGrading.colorFilter.value = new Color(97f / 255f, 163f / 255f, 239f / 255f);
            colorGrading.colorFilter.overrideState = true;
            pp.sharedProfile = ppProfile;
            ppHolder.transform.SetParent(visualEffectTeleportOut.transform);

            MysticsItemsContent.Resources.effectPrefabs.Add(visualEffectTeleportOut);

            SceneExitController sceneExitController = sceneExitControllerObject.AddComponent<SceneExitController>();
            sceneExitController.useRunNextStageScene = true;
            sceneExitControllerObject.AddComponent<MysticsItemsGateChaliceSceneExit>();

            itemDestroyEffectPrefab = PrefabAPI.InstantiateClone(new GameObject(), "MysticsItems_GateChaliceItemDestroyEffect", false);
            EntityStateMachine entityStateMachine = itemDestroyEffectPrefab.AddComponent<EntityStateMachine>();
            entityStateMachine.initialStateType = entityStateMachine.mainStateType = new EntityStates.SerializableEntityStateType(typeof(MysticsItemsGateChaliceItemDestroyEffect));
            PickupDisplay pickupDisplay = itemDestroyEffectPrefab.AddComponent<PickupDisplay>();
            Rigidbody rigidbody = itemDestroyEffectPrefab.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.drag = 2f;
            effectComponent = itemDestroyEffectPrefab.AddComponent<EffectComponent>();
            effectComponent.applyScale = true;
            effectComponent.soundName = "Play_moonBrother_phase4_itemSuck_returnSingle";
            vfxAttributes = itemDestroyEffectPrefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.High;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            Highlight highlight = itemDestroyEffectPrefab.AddComponent<Highlight>();
            pickupDisplay.highlight = highlight;
            highlight.highlightColor = Highlight.HighlightColor.pickup;
            MysticsItemsContent.Resources.effectPrefabs.Add(itemDestroyEffectPrefab);
        }

        public class MysticsItemsGateChaliceItemDestroyEffect : EntityStates.EntityState
        {
            public float shatterTime = 1.5f;
            public bool shatterFlag = false;
            public float duration = 3f;

            public EffectComponent effectComponent;
            public PickupDisplay pickupDisplay;
            
            public override void OnEnter()
            {
                base.OnEnter();
                effectComponent = GetComponent<EffectComponent>();
                pickupDisplay = GetComponent<PickupDisplay>();
                if (effectComponent)
                {
                    if (pickupDisplay)
                    {
                        pickupDisplay.SetPickupIndex(new PickupIndex((int)effectComponent.effectData.genericUInt), false);
                    }
                    if (rigidbody)
                    {
                        var force = Vector3.up;
                        force = Quaternion.AngleAxis(45f, Vector3.forward) * force;
                        force = Quaternion.AngleAxis(effectComponent.effectData.genericFloat, Vector3.up) * force;
                        rigidbody.AddForce(force * 400f);
                    }
                }
            }

            public override void Update()
            {
                base.Update();
                if (age >= shatterTime && !shatterFlag)
                {
                    shatterFlag = true;
                    if (pickupDisplay && pickupDisplay.modelObject)
                    {
                        var childLocator = pickupDisplay.modelObject.GetComponent<ChildLocator>();
                        if (!childLocator) childLocator = pickupDisplay.modelObject.AddComponent<ChildLocator>();
                        if (childLocator.transformPairs == null) childLocator.transformPairs = new ChildLocator.NameTransformPair[] { };
                        var transformPair = new ChildLocator.NameTransformPair
                        {
                            name = "ShatterOrigin",
                            transform = pickupDisplay.modelObject.transform
                        };
                        HGArrayUtilities.ArrayAppend(ref childLocator.transformPairs, ref transformPair);

                        TemporaryOverlay temporaryOverlay = pickupDisplay.modelObject.AddComponent<TemporaryOverlay>();
                        temporaryOverlay.duration = 0.5f;
                        temporaryOverlay.destroyObjectOnEnd = true;
                        temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matShatteredGlass");
                        temporaryOverlay.destroyEffectPrefab = (GameObject)LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BrittleDeath");
                        temporaryOverlay.destroyEffectChildString = "ShatterOrigin";
                        temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                        temporaryOverlay.animateShaderAlpha = true;
                        temporaryOverlay.SetupMaterial();

                        var renderer = pickupDisplay.modelRenderer;
                        if (renderer)
                        {
                            var materials = renderer.materials;
                            HGArrayUtilities.ArrayAppend(ref materials, ref temporaryOverlay.materialInstance);
                            renderer.materials = materials;
                        }
                    }
                }
                if (age >= duration) Object.Destroy(gameObject);
            }
        }

        private class MysticsItemsGateChalicePPController : MonoBehaviour
        {
            public float stopwatch = 0f;
            public float time = 0f;
            public AnimationCurve curve = new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(0.7f, 1f),
                new Keyframe(1f, 0f)
            );

            public PostProcessVolume volume;

            public void Start()
            {
                volume = GetComponent<PostProcessVolume>();
            }

            public void Update()
            {
                stopwatch += Time.deltaTime;
                volume.weight = curve.Evaluate(stopwatch / time);
            }
        }

        private class MysticsItemsGateChaliceSceneExit : MonoBehaviour
        {
            public SceneExitController controller;
            public Transform attach;
            public bool effectCreated = false;

            public void Start()
            {
                controller = GetComponent<SceneExitController>();

                // special next scene overrides based on the current scene
                var mostRecentSceneDef = SceneCatalog.mostRecentSceneDef;
                if (mostRecentSceneDef)
                {
                    if (destinationOverrides.ContainsKey(mostRecentSceneDef.baseSceneName))
                    {
                        var possibleOverrides = RoR2Application.rng.NextElementUniform(destinationOverrides[mostRecentSceneDef.baseSceneName]);
                        var sceneOverride = SceneCatalog.FindSceneDef(possibleOverrides);
                        if (sceneOverride) controller.destinationScene = sceneOverride;
                    }
                }

                // if a teleporter has a next scene override, use it
                if (TeleporterInteraction.instance)
                {
                    var sceneExitController = TeleporterInteraction.instance.sceneExitController;
                    if (sceneExitController && !sceneExitController.useRunNextStageScene)
                    {
                        controller.useRunNextStageScene = false;
                        controller.destinationScene = sceneExitController.destinationScene;
                    }
                }

                controller.Begin();
            }

            public void FixedUpdate()
            {
                if (attach) transform.position = attach.position;
                if (!effectCreated && controller.exitState == SceneExitController.ExitState.TeleportOut)
                {
                    effectCreated = true;
                    EffectManager.SimpleEffect(visualEffectTeleportOut, Vector3.zero, Quaternion.identity, true);
                }
            }
        }

        public override bool OnUse(EquipmentSlot equipmentSlot)
        {
            if (!SceneExitController.isRunning && (!SceneCatalog.mostRecentSceneDef || (!SceneCatalog.mostRecentSceneDef.isFinalStage && !cannotSkipStages.Contains(SceneCatalog.mostRecentSceneDef.baseSceneName))))
            {
                CharacterBody characterBody = equipmentSlot.characterBody;
                if (characterBody.healthComponent && !characterBody.healthComponent.alive) return false;
                EffectData effectData = new EffectData
                {
                    origin = characterBody.corePosition,
                    scale = characterBody.radius
                };
                effectData.SetHurtBoxReference(characterBody.gameObject);
                EffectManager.SpawnEffect(visualEffectOnUse, effectData, true);
                GameObject sceneExit = Object.Instantiate(sceneExitControllerObject, characterBody.corePosition, Quaternion.identity);
                sceneExit.GetComponent<MysticsItemsGateChaliceSceneExit>().attach = characterBody.gameObject.transform;

                /*
                if (equipmentSlot.inventory)
                {
                    List<ItemIndex> list = equipmentSlot.inventory.itemAcquisitionOrder;
                    for (var i = 0; i < itemsToDestroy.Value; i++)
                    {
                        if (list.Count <= 0) break;
                        ItemIndex itemIndex = list[Mathf.FloorToInt((list.Count - 1) * Random.value)];
                        ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
                        if (itemDef.canRemove)
                        {
                            equipmentSlot.inventory.RemoveItem(itemIndex);

                            PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(itemIndex);

                            EffectManager.SpawnEffect(itemDestroyEffectPrefab, new EffectData
                            {
                                origin = equipmentSlot.characterBody.corePosition,
                                scale = equipmentSlot.characterBody.radius * 2f,
                                genericUInt = (uint)pickupIndex.value,
                                genericFloat = 360f / (float)itemsToDestroy.Value * (float)i
                            }, true);
                        }
                    }
                }
                */
                return true;
            }
            if (SceneCatalog.mostRecentSceneDef)
            {
                var kvp = specialStageTeleportation.FirstOrDefault(x => x.Key == SceneCatalog.mostRecentSceneDef.baseSceneName);
                if (!kvp.Equals(default(KeyValuePair<string, SpecialStageTeleportationInfo>)))
                {
                    var specialStageTeleportationInfo = kvp.Value;
                    var body = equipmentSlot.characterBody;
                    if (body)
                    {
                        var randomPoints = new List<Vector3>();
                        foreach (var ally in TeamComponent.GetTeamMembers(TeamComponent.GetObjectTeam(body.gameObject)))
                        {
                            if (ally.body && ally.body.isPlayerControlled && !specialStageTeleportationInfo.bounds.Contains(ally.body.corePosition))
                            {
                                if (randomPoints.Count <= 0) randomPoints.AddRange(specialStageTeleportationInfo.destinations);

                                var randomPoint = RoR2Application.rng.NextElementUniform(randomPoints);

                                TeleportHelper.TeleportBody(ally.body, randomPoint);
                                GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(ally.body.gameObject);
                                if (teleportEffectPrefab)
                                {
                                    EffectManager.SimpleEffect(teleportEffectPrefab, randomPoint, Quaternion.identity, true);
                                }

                                randomPoints.Remove(randomPoint);
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
