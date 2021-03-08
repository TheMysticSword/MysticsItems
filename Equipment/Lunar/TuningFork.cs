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

            public void Start()
            {
                meshRenderer = GetComponent<MeshRenderer>();
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

                    meshRenderer.material.SetFloat("_Boost", (1f - stopwatch / time) * 0.34f);
                }
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
