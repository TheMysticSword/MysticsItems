using RoR2;
using RoR2.Projectile;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;
using System.Collections.Generic;
using RoR2.Audio;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Equipment
{
    public class Microphone : BaseEquipment
    {
        public static GameObject wavePrefab;
        public static GameObject waveProjectile;
        public static List<GameObject> waveProjectilesScaled = new List<GameObject>();
        public static BuffDef buffDef;
        public static int shotsPerCast = 3;
        public static NetworkSoundEventDef sound;

        public static ConfigurableValue<float> duration = new ConfigurableValue<float>(
            "Equipment: Vintage Microphone",
            "Duration",
            15f,
            "Deafen debuff duration (in seconds)",
            new List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_MICROPHONE_DESC"
            }
        );
        public static ConfigurableValue<float> boostDistance = new ConfigurableValue<float>(
            "Equipment: Vintage Microphone",
            "BoostDistance",
            15f,
            "How far should it launch the owner when used mid-air (in meters)"
        );
        
        public override void OnPluginAwake()
        {
            waveProjectile = PrefabAPI.InstantiateClone(new GameObject(), "MysticsItems_MicrophoneSoundwave", false);
            waveProjectile.AddComponent<NetworkIdentity>().localPlayerAuthority = true;
            PrefabAPI.RegisterNetworkPrefab(waveProjectile);
        }

        public override void OnLoad()
        {
            equipmentDef.name = "MysticsItems_Microphone";
            ConfigManager.Balance.CreateEquipmentCooldownOption(equipmentDef, "Equipment: Vintage Microphone", 60f);
            equipmentDef.canDrop = true;
            ConfigManager.Balance.CreateEquipmentEnigmaCompatibleOption(equipmentDef, "Equipment: Vintage Microphone", true);
            ConfigManager.Balance.CreateEquipmentCanBeRandomlyTriggeredOption(equipmentDef, "Equipment: Vintage Microphone", false);
            equipmentDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Microphone/Model.prefab"));
            equipmentDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Equipment/Microphone/Icon.png");

            var modelPanelParams = equipmentDef.pickupModelPrefab.GetComponent<ModelPanelParameters>();
            modelPanelParams.minDistance = 5;
            modelPanelParams.maxDistance = 15;
            HopooShaderToMaterial.Standard.Gloss(equipmentDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial, 1f, 15f);
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(equipmentDef.pickupModelPrefab, equipmentDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Stomach", new Vector3(-0.131F, 0.101F, -0.106F), new Vector3(353.789F, 220.459F, 176.094F), new Vector3(0.024F, 0.024F, 0.024F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(-0.065F, 0.139F, 0.087F), new Vector3(57.427F, 169.25F, 159.351F), new Vector3(0.011F, 0.011F, 0.012F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(-0.097F, 0.048F, -0.129F), new Vector3(4.954F, 282.723F, 159.07F), new Vector3(0.029F, 0.029F, 0.029F));
                AddDisplayRule("ToolbotBody", "HandR", new Vector3(-0.01F, 0.981F, -0.167F), new Vector3(58.56F, 268.579F, 264.86F), new Vector3(0.378F, 0.378F, 0.378F));
                AddDisplayRule("EngiBody", "HandL", new Vector3(0.015F, 0.18F, -0.04F), new Vector3(70.126F, 294.778F, 9.124F), new Vector3(0.047F, 0.047F, 0.047F));
                AddDisplayRule("MageBody", "Head", new Vector3(-0.095F, 0.108F, -0.047F), new Vector3(358.993F, 354.776F, 177.852F), new Vector3(0.016F, 0.016F, 0.016F));
                AddDisplayRule("MercBody", "HandL", new Vector3(0.021F, 0.127F, 0.013F), new Vector3(356.714F, 344.677F, 266.091F), new Vector3(0.027F, 0.027F, 0.027F));
                AddDisplayRule("TreebotBody", "WeaponPlatformEnd", new Vector3(-0.044F, -0.068F, 0.154F), new Vector3(339.636F, 327.135F, 61.693F), new Vector3(0.05F, 0.05F, 0.05F));
                AddDisplayRule("LoaderBody", "MechHandL", new Vector3(0.052F, 0.271F, 0.005F), new Vector3(0.351F, 159.123F, 281.328F), new Vector3(0.052F, 0.05F, 0.052F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(-0.543F, 4.565F, -0.104F), new Vector3(284.494F, 3.244F, 267.429F), new Vector3(0.333F, 0.333F, 0.333F));
                AddDisplayRule("CaptainBody", "HandR", new Vector3(0.002F, 0.155F, -0.014F), new Vector3(53.771F, 271.897F, 272.032F), new Vector3(0.041F, 0.041F, 0.041F));
                AddDisplayRule("ScavBody", "HandL", new Vector3(0.469F, 2.35F, -0.273F), new Vector3(305.162F, 137.483F, 278.565F), new Vector3(0.883F, 0.883F, 0.883F));
                AddDisplayRule("EquipmentDroneBody", "GunBarrelBase", new Vector3(0F, 0F, 1.453F), new Vector3(0F, 90F, 0F), new Vector3(0.265F, 0.265F, 0.265F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Head", new Vector3(-0.12632F, 0.03962F, -0.03585F), new Vector3(339.427F, 188.6309F, 0F), new Vector3(0.03139F, 0.03139F, 0.03139F));
                AddDisplayRule("RailgunnerBody", "GunScope", new Vector3(0F, 0.18265F, 0.3376F), new Vector3(0F, 270F, 243.3805F), new Vector3(0.02478F, 0.02478F, 0.02478F));
                AddDisplayRule("VoidSurvivorBody", "Chest", new Vector3(0.11444F, 0.29135F, 0.09171F), new Vector3(30.82122F, 341.6345F, 328.628F), new Vector3(0.03418F, 0.03418F, 0.03418F));
            };

            wavePrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Microphone/MicrophoneSoundwaveGhost.prefab");
            wavePrefab.AddComponent<ProjectileGhostController>();

            MysticsRisky2Utils.Utils.CopyChildren(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Microphone/MicrophoneSoundwave.prefab"), waveProjectile);
            MicrophoneSoundwaveProjectile msp = waveProjectile.AddComponent<MicrophoneSoundwaveProjectile>();
            msp.colorCurve = new AnimationCurve[]
            {
                new AnimationCurve{keys = new Keyframe[] { new Keyframe(0f, 255f) }},
                new AnimationCurve{keys = new Keyframe[] { new Keyframe(0f, 195f) }},
                new AnimationCurve{keys = new Keyframe[] { new Keyframe(0f, 112f) }},
                new AnimationCurve{keys = new Keyframe[] {
                    new Keyframe(0f, 0f),
                    new Keyframe(0.5f, 255f),
                    new Keyframe(1f, 0f)
                }}
            };
            ProjectileController projectileController = waveProjectile.AddComponent<ProjectileController>();
            projectileController.ghostPrefab = wavePrefab;
            projectileController.allowPrediction = true;
            waveProjectile.AddComponent<ProjectileNetworkTransform>();
            waveProjectile.AddComponent<TeamFilter>();
            ProjectileDamage projectileDamage = waveProjectile.AddComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.Stun1s;
            HitBoxGroup hitBoxGroup = waveProjectile.AddComponent<HitBoxGroup>();
            hitBoxGroup.groupName = "MicrophoneSoundwave";
            hitBoxGroup.hitBoxes = new HitBox[]
            {
                waveProjectile.transform.Find("Hitbox").gameObject.AddComponent<HitBox>()
            };
            ProjectileOverlapAttack projectileOverlapAttack = waveProjectile.AddComponent<ProjectileOverlapAttack>();
            projectileOverlapAttack.damageCoefficient = 0f;
            projectileOverlapAttack.overlapProcCoefficient = 0f;
            ProjectileInflictTimedBuff projectileInflictTimedBuff = waveProjectile.AddComponent<ProjectileInflictTimedBuff>();
            projectileInflictTimedBuff.duration = duration.Value;

            MysticsItemsContent.Resources.projectilePrefabs.Add(waveProjectile);

            for (int i = 0; i < shotsPerCast; i++)
            {
                GameObject waveProjectileScaled = PrefabAPI.InstantiateClone(waveProjectile, waveProjectile.name + "Scaled" + i);
                waveProjectileScaled.transform.localScale = Vector3.one * 30f * ((i + 1f) / (float)shotsPerCast);
                if (i > 0) waveProjectileScaled.GetComponent<MicrophoneSoundwaveProjectile>().colorCurve[3].AddKey(new Keyframe(0.5f * (i / (float)shotsPerCast), 0f));
                waveProjectilesScaled.Add(waveProjectileScaled);
                MysticsItemsContent.Resources.projectilePrefabs.Add(waveProjectileScaled);
            }

            sound = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            sound.eventName = "MysticsItems_Play_item_use_microphone";
            MysticsItemsContent.Resources.networkSoundEventDefs.Add(sound);
        }

        public class MicrophoneSoundwaveProjectile : NetworkBehaviour
        {
            public Rigidbody rigidbody;
            public ProjectileController controller;
            public ProjectileInflictTimedBuff timedBuff;
            public float stopwatch = 0f;
            public float lifetime = 1f;
            public float initialSpeed = 60f;
            public AnimationCurve[] colorCurve;
            public Renderer renderer;
            public MaterialPropertyBlock materialPropertyBlock;

            public void Start()
            {
                rigidbody = GetComponent<Rigidbody>();
                controller = GetComponent<ProjectileController>();
                timedBuff = GetComponent<ProjectileInflictTimedBuff>();

                rigidbody.velocity = initialSpeed * transform.forward;
                timedBuff.buffDef = buffDef;

                materialPropertyBlock = new MaterialPropertyBlock();

                EvaluateCurves();
            }

            public void FixedUpdate()
            {
                stopwatch += Time.fixedDeltaTime;

                if (controller.ghost) controller.ghost.transform.localScale = transform.localScale;
                EvaluateCurves();
                if (stopwatch >= lifetime)
                {
                    Object.Destroy(gameObject);
                }
            }

            public void EvaluateCurves()
            {
                float t = stopwatch / lifetime;
                
                if (controller.ghost)
                {
                    if (!renderer) renderer = controller.ghost.GetComponentInChildren<Renderer>();
                    if (renderer)
                    {
                        renderer.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetColor("_Color", new Color32(
                            (byte)colorCurve[0].Evaluate(t),
                            (byte)colorCurve[1].Evaluate(t),
                            (byte)colorCurve[2].Evaluate(t),
                            (byte)colorCurve[3].Evaluate(t)
                        ));
                        materialPropertyBlock.SetFloat("_Zoom", colorCurve[3].Evaluate(t));
                        materialPropertyBlock.SetFloat("_DistortionPower", colorCurve[3].Evaluate(t));
                        renderer.SetPropertyBlock(materialPropertyBlock);
                    }
                }
            }
        }

        public override bool OnUse(EquipmentSlot equipmentSlot)
        {
            MicrophoneSoundwaveLauncher component = equipmentSlot.GetComponent<MicrophoneSoundwaveLauncher>();
            if (!component) component = equipmentSlot.gameObject.AddComponent<MicrophoneSoundwaveLauncher>();
            component.ammo++;
            component.aimRay = equipmentSlot.GetAimRay();
            return true;
        }

        public class MicrophoneSoundwaveLauncher : MonoBehaviour
        {
            public int ammo = 0;
            public float interval = 0f;
            public float intervalMax = 0.5f;
            public Ray aimRay = new Ray();
            public EquipmentSlot equipmentSlot;

            public void Awake()
            {
                equipmentSlot = GetComponent<EquipmentSlot>();
            }

            public void FixedUpdate()
            {
                if (!NetworkServer.active) return;
                interval -= Time.fixedDeltaTime;
                if (interval <= 0f && ammo > 0)
                {
                    interval = intervalMax;
                    ammo--;

                    var characterBody = equipmentSlot.characterBody;
                    if (characterBody)
                    {
                        if (characterBody.characterMotor && !characterBody.characterMotor.isGrounded)
                        {
                            var height = boostDistance.Value;
                            var mass = characterBody.characterMotor ? characterBody.characterMotor.mass : (characterBody.rigidbody ? characterBody.rigidbody.mass : 1f);
                            var ySpeed = Trajectory.CalculateInitialYSpeedForHeight(height, -characterBody.acceleration);

                            var force = -ySpeed * mass * equipmentSlot.GetAimRay().direction;

                            if (characterBody.characterMotor)
                            {
                                characterBody.characterMotor.ApplyForce(force, false, false);
                            }
                            else if (characterBody.rigidbody)
                            {
                                characterBody.rigidbody.AddForce(force, ForceMode.Impulse);
                            }
                        }
                    }

                    Vector3 position = transform.position;
                    for (int i = 0; i < shotsPerCast; i++)
                    {
                        ProjectileManager.instance.FireProjectile(waveProjectilesScaled[i], position + (10f * (float)i) * aimRay.direction.normalized, Util.QuaternionSafeLookRotation(aimRay.direction), equipmentSlot.gameObject, 0f, 0f, false, DamageColorIndex.Default, null, -1f);
                    }
                    PointSoundManager.EmitSoundServer(sound.index, position);
                }
            }
        }
    }
}
