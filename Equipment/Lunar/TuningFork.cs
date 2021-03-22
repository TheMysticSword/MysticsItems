using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using R2API.Utils;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;

namespace MysticsItems.Equipment
{
    public class TuningFork : BaseEquipment
    {
        public static GameObject visualEffect;
        public static float radius = 60f;

        public override void PreAdd()
        {
            equipmentDef.name = "TuningFork";
            equipmentDef.cooldown = 45f;
            equipmentDef.canDrop = true;
            equipmentDef.enigmaCompatible = true;
            equipmentDef.isLunar = true;
            equipmentDef.colorIndex = ColorCatalog.ColorIndex.LunarItem;

            SetAssets("Tuning Fork");
            model.transform.Find("mdlTuningFork").Rotate(new Vector3(0f, 0f, -45f), Space.Self);

            AddDisplayRule((int)Main.CommonBodyIndices.Commando, "Chest", new Vector3(-0.035F, 0.196F, -0.208F), new Vector3(341.98F, 196.319F, 18.721F), new Vector3(0.042F, 0.042F, 0.042F));
            AddDisplayRule("mdlHuntress", "Head", new Vector3(0.001F, 0.253F, -0.112F), new Vector3(303.972F, 359.427F, 0.457F), new Vector3(0.053F, 0.023F, 0.052F));
            AddDisplayRule("mdlToolbot", "Chest", new Vector3(-2.568F, 2.244F, -0.393F), new Vector3(0F, 90F, 128.784F), new Vector3(0.404F, 0.404F, 0.404F));
            AddDisplayRule("mdlEngi", "Chest", new Vector3(0.074F, 0.278F, -0.256F), new Vector3(10.736F, 0F, 0F), new Vector3(0.047F, 0.047F, 0.047F));
            AddDisplayRule("mdlMage", "HandL", new Vector3(-0.018F, -0.007F, 0.069F), new Vector3(348.97F, 0F, 0F), new Vector3(0.044F, 0.044F, 0.044F));
            AddDisplayRule("mdlMage", "HandR", new Vector3(-0.052F, 0F, -0.059F), new Vector3(2.7F, 32.085F, 0F), new Vector3(0.044F, 0.044F, 0.044F));
            AddDisplayRule("mdlMerc", "HandL", new Vector3(-0.202F, 0.132F, -0.07F), new Vector3(282.883F, 156.523F, 272.905F), new Vector3(0.041F, 0.124F, 0.041F));
            AddDisplayRule("mdlTreebot", "WeaponPlatform", new Vector3(0F, -0.168F, 0.421F), new Vector3(0F, 0F, 0F), new Vector3(0.146F, 0.087F, 0.146F));
            AddDisplayRule("mdlLoader", "MechHandR", new Vector3(0.087F, 0.245F, 0F), new Vector3(0F, 0F, 269.477F), new Vector3(0.076F, 0.076F, 0.076F));
            AddDisplayRule("mdlCroco", "SpineChest3", new Vector3(-0.496F, 0.632F, 1.029F), new Vector3(76.142F, 328.553F, 189.334F), new Vector3(0.361F, 0.361F, 0.361F));
            AddDisplayRule("mdlCaptain", "HandL", new Vector3(0F, 0.197F, -0.042F), new Vector3(0F, 0F, 0F), new Vector3(0.041F, 0.041F, 0.041F));
            AddDisplayRule("mdlScav", "MuzzleEnergyCannon", new Vector3(0F, -4.749F, 0F), new Vector3(90F, 0F, 0F), new Vector3(1.363F, 1.363F, 1.363F));
            AddDisplayRule("mdlScav", "MuzzleEnergyCannon", new Vector3(3.396F, 3.443F, -0.001F), new Vector3(40.988F, 270F, 270F), new Vector3(1.363F, 1.363F, 1.363F));
            AddDisplayRule("mdlScav", "MuzzleEnergyCannon", new Vector3(-3.396F, 3.443F, -0.001F), new Vector3(36.034F, 90F, 90F), new Vector3(1.363F, 1.363F, 1.363F));
            AddDisplayRule("mdlEquipmentDrone", "GunBarrelBase", new Vector3(0F, 0F, 1.069F), new Vector3(52.789F, 0F, 0F), new Vector3(0.267F, 0.267F, 0.267F));

            visualEffect = PrefabAPI.InstantiateClone(new GameObject(), Main.TokenPrefix + "TuningForkEffect", false);
            float time = 1.2f;
            EffectComponent effectComponent = visualEffect.AddComponent<EffectComponent>();
            effectComponent.parentToReferencedTransform = true;
            effectComponent.applyScale = true;
            effectComponent.disregardZScale = true;
            effectComponent.soundName = "Play_item_use_tuningfork";
            VFXAttributes vfxAttributes = visualEffect.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.High;
            visualEffect.AddComponent<DestroyOnTimer>().duration = time;

            ShakeEmitter shakeEmitter = visualEffect.AddComponent<ShakeEmitter>();
            shakeEmitter.shakeOnStart = true;
            shakeEmitter.wave = new Wave
            {
                frequency = 2f,
                amplitude = 0.1f
            };
            shakeEmitter.duration = time;
            shakeEmitter.radius = radius;
            shakeEmitter.amplitudeTimeDecay = true;

            GameObject ppHolder = PrefabAPI.InstantiateClone(new GameObject(), "PP", false);
            ppHolder.AddComponent<PPController>().time = time;
            SphereCollider ppSphere = ppHolder.AddComponent<SphereCollider>();
            ppSphere.radius = radius * 0.3f;
            ppSphere.isTrigger = true;
            ppHolder.layer = LayerIndex.postProcess.intVal;
            PostProcessVolume pp = ppHolder.AddComponent<PostProcessVolume>();
            pp.blendDistance = radius * 0.7f;
            pp.isGlobal = false;
            pp.weight = 0f;
            pp.priority = 4;
            PostProcessProfile ppProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
            ppProfile.name = "ppLocalTuningFork";
            LensDistortion lensDistortion = ppProfile.AddSettings<LensDistortion>();
            lensDistortion.SetAllOverridesTo(true);
            lensDistortion.intensity.value = -50f;
            lensDistortion.scale.value = 1f;
            pp.sharedProfile = ppProfile;
            ppHolder.transform.SetParent(visualEffect.transform);

            GameObject radiusIndicator = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/NetworkedObjects/Teleporters/Teleporter1").transform.Find("TeleporterBaseMesh").Find("BuiltInEffects").Find("ChargingEffect").Find("RadiusScaler").Find("ClearAreaIndicator").gameObject, "RadiusIndicator", false);
            radiusIndicator.AddComponent<RadiusIndicatorController>();
            MeshRenderer meshRenderer = radiusIndicator.GetComponent<MeshRenderer>();
            meshRenderer.material.SetFloat("_RimPower", 1.2f);
            meshRenderer.material.SetTexture("_RemapTex", Main.AssetBundle.LoadAsset<Texture2D>("Assets/Equipment/Tuning Fork/texRampTuningFork.png"));

            for (int i = 0; i < 3; i++)
            {
                GameObject radiusIndicator2 = PrefabAPI.InstantiateClone(radiusIndicator, "RadiusIndicator" + (i + 1).ToString(), false);
                radiusIndicator2.GetComponent<RadiusIndicatorController>().delay = 0.2f * i;
                radiusIndicator2.GetComponent<RadiusIndicatorController>().time = time - 0.2f * i;
                radiusIndicator2.transform.SetParent(visualEffect.transform);
            }

            AssetManager.RegisterEffect(visualEffect);
        }

        public class RadiusIndicatorController : MonoBehaviour
        {
            public float stopwatch = 0f;
            public float time = 0f;
            public float delay = 0f;
            private bool rendererEnabled = false;
            private float radius = 0f;

            public MeshRenderer meshRenderer;
            public Material material;

            public void Start()
            {
                meshRenderer = GetComponent<MeshRenderer>();
                material = Object.Instantiate(meshRenderer.sharedMaterial);
                meshRenderer.material = material;
            }

            public void Update()
            {
                if (delay > 0)
                {
                    delay -= Time.deltaTime;
                    if (delay <= 0)
                    {
                        meshRenderer.enabled = true;
                    }
                }
                else
                {
                    stopwatch += Time.deltaTime;

                    if (!rendererEnabled)
                    {
                        rendererEnabled = true;
                        meshRenderer.enabled = true;
                    }

                    radius = stopwatch / time * TuningFork.radius;
                    transform.localScale = Vector3.one * radius;

                    material.SetFloat("_Boost", (1f - stopwatch / time) * 0.34f);
                }
            }

            public void OnDestroy()
            {
                Object.Destroy(material);
            }
        }

        public class PPController : MonoBehaviour
        {
            public float stopwatch = 0f;
            public float time = 0f;
            public AnimationCurve curve = new AnimationCurve(
                new Keyframe {
                    time = 0f,
                    value = 0f
                },
                new Keyframe
                {
                    time = 0.5f,
                    value = 1f
                },
                new Keyframe
                {
                    time = 1f,
                    value = 0f
                }
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

        public override bool OnUse(EquipmentSlot equipmentSlot)
        {
            EffectData effectData = new EffectData
            {
                origin = equipmentSlot.characterBody.corePosition,
                rotation = Quaternion.identity
            };
            effectData.SetHurtBoxReference(equipmentSlot.characterBody.mainHurtBox);
            EffectManager.SpawnEffect(visualEffect, effectData, true);
            HurtBox[] hurtBoxes = new SphereSearch
            {
                mask = LayerIndex.entityPrecise.mask,
                origin = equipmentSlot.characterBody.corePosition,
                queryTriggerInteraction = QueryTriggerInteraction.Collide,
                radius = radius
            }.RefreshCandidates().FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes();
            float totalHealthFraction = 0f;
            int totalHealthComponents = 0;
            foreach (HurtBox hurtBox in hurtBoxes)
            {
                HealthComponent healthComponent = hurtBox.healthComponent;
                if (healthComponent && healthComponent.alive)
                {
                    totalHealthFraction += healthComponent.combinedHealthFraction;
                    totalHealthComponents++;
                }
            }
            float redistributedHealth = totalHealthFraction / totalHealthComponents;
            foreach (HurtBox hurtBox in hurtBoxes)
            {
                HealthComponent healthComponent = hurtBox.healthComponent;
                if (healthComponent && healthComponent.alive)
                {
                    float hp = Mathf.Min(redistributedHealth, 1f);
                    float barrier = Mathf.Max(redistributedHealth - 1f, 0f);
                    healthComponent.Networkhealth = (healthComponent.fullHealth / healthComponent.fullCombinedHealth) * hp * healthComponent.fullCombinedHealth;
                    healthComponent.Networkshield = (healthComponent.fullShield / healthComponent.fullCombinedHealth) * hp * healthComponent.fullCombinedHealth;
                    healthComponent.Networkbarrier = (healthComponent.fullBarrier / healthComponent.fullCombinedHealth) * barrier * healthComponent.fullCombinedHealth;
                }
            }
            return true;
        }
    }
}
