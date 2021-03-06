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

namespace MysticsItems.Equipment
{
    public class GateChalice : BaseEquipment
    {
        public static GameObject visualEffectOnUse;
        public static GameObject visualEffectTeleportOut;
        public static GameObject sceneExitControllerObject;

        public override void OnPluginAwake()
        {
            sceneExitControllerObject = CustomUtils.CreateBlankPrefab(Main.TokenPrefix + "GateChaliceSceneExitControllerObject", true);
        }

        public override void PreLoad()
        {
            equipmentDef.name = "GateChalice";
            equipmentDef.cooldown = 140f;
            equipmentDef.canDrop = true;
            equipmentDef.enigmaCompatible = true;
            equipmentDef.isLunar = true;
            equipmentDef.colorIndex = ColorCatalog.ColorIndex.LunarItem;
        }

        public override void OnLoad()
        {
            base.OnLoad();
            SetAssets("Gate Chalice");
            Material mat = model.transform.Find("mdlGateChalice").gameObject.GetComponent<MeshRenderer>().sharedMaterial;
            Main.HopooShaderToMaterial.Standard.Apply(mat);
            Main.HopooShaderToMaterial.Standard.Gloss(mat, 0.5f);
            Main.HopooShaderToMaterial.Standard.Emission(mat, 0.02f, new Color(48f / 255f, 127f / 255f, 255f / 255f));
            foreach (Transform lightTransform in model.transform.Find("mdlGateChalice").Find("Lights"))
            {
                SetScalableChildEffect(lightTransform.gameObject);
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
            CopyModelToFollower();
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
            };
            
            visualEffectOnUse = PrefabAPI.InstantiateClone(new GameObject(), Main.TokenPrefix + "GateChaliceOnUseEffect", false);
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
            massSparks.GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/ActivateRadarTowerEffect").transform.Find("MassSparks").gameObject.GetComponent<ParticleSystemRenderer>().material);
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

            visualEffectTeleportOut = PrefabAPI.InstantiateClone(new GameObject(), Main.TokenPrefix + "GateChaliceTeleportOutEffect", false);
            effectComponent = visualEffectTeleportOut.AddComponent<EffectComponent>();
            vfxAttributes = visualEffectTeleportOut.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.High;
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

            TeleporterInteraction.onTeleporterChargedGlobal += (teleporterInteraction) =>
            {
                foreach (CharacterMaster master in CharacterMaster.readOnlyInstancesList)
                {
                    if (master.teamIndex == TeamIndex.Player)
                    {
                        master.inventory.RemoveItem(MysticsItemsContent.Items.GateChaliceDebuff, master.inventory.GetItemCount(MysticsItemsContent.Items.GateChaliceDebuff));
                    }
                }
            };
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
                if (SceneInfo.instance && SceneInfo.instance.sceneDef.isFinalStage)
                {
                    controller.useRunNextStageScene = false;
                    controller.destinationScene = SceneInfo.instance.sceneDef;
                }
                controller.Begin();
            }

            public void FixedUpdate()
            {
                if (attach) transform.position = attach.position;
                if (!effectCreated && controller.GetFieldValue<SceneExitController.ExitState>("exitState") == SceneExitController.ExitState.TeleportOut)
                {
                    effectCreated = true;
                    EffectManager.SimpleEffect(visualEffectTeleportOut, Vector3.zero, Quaternion.identity, true);
                }
            }
        }

        public override bool OnUse(EquipmentSlot equipmentSlot)
        {
            CharacterBody characterBody = equipmentSlot.characterBody;
            if (!characterBody.healthComponent.alive) return false;
            EffectData effectData = new EffectData
            {
                origin = characterBody.corePosition,
                scale = characterBody.radius
            };
            effectData.SetHurtBoxReference(characterBody.gameObject);
            EffectManager.SpawnEffect(visualEffectOnUse, effectData, true);
            GameObject sceneExit = Object.Instantiate(sceneExitControllerObject, characterBody.corePosition, Quaternion.identity);
            sceneExit.GetComponent<MysticsItemsGateChaliceSceneExit>().attach = characterBody.gameObject.transform;

            foreach (CharacterMaster master in CharacterMaster.readOnlyInstancesList)
            {
                if (master.teamIndex == TeamIndex.Player)
                {
                    master.inventory.GiveItem(MysticsItemsContent.Items.GateChaliceDebuff);
                    ReadOnlyCollection<NotificationQueue> readOnlyCollection = NotificationQueue.readOnlyInstancesList;
                    for (int i = 0; i < readOnlyCollection.Count; i++)
                    {
                        readOnlyCollection[i].OnPickup(master, PickupCatalog.FindPickupIndex(MysticsItemsContent.Items.GateChaliceDebuff.itemIndex));
                    }
                }
            }

            return true;
        }
    }
}
