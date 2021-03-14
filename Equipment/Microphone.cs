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

namespace MysticsItems.Equipment
{
    public class Microphone : BaseEquipment
    {
        public static GameObject wavePrefab;
        public static GameObject waveProjectile;
        public static BuffIndex buffIndex;

        public override void PreAdd()
        {
            equipmentDef.name = "Microphone";
            equipmentDef.cooldown = 60f;
            equipmentDef.canDrop = true;
            equipmentDef.enigmaCompatible = true;

            SetAssets("Microphone");
            SetModelPanelDistance(5f, 15f);
            Main.HopooShaderToMaterial.Standard.Gloss(GetModelMaterial(), 1f, 15f);
            CopyModelToFollower();

            AddDisplayRule((int)Main.CommonBodyIndices.Commando, "Stomach", new Vector3(-0.131F, 0.101F, -0.106F), new Vector3(353.789F, 220.459F, 176.094F), new Vector3(0.024F, 0.024F, 0.024F));
            AddDisplayRule("mdlHuntress", "Head", new Vector3(-0.065F, 0.139F, 0.087F), new Vector3(57.427F, 169.25F, 159.351F), new Vector3(0.011F, 0.011F, 0.012F));
            AddDisplayRule("mdlToolbot", "HandR", new Vector3(-0.01F, 0.981F, -0.167F), new Vector3(58.56F, 268.579F, 264.86F), new Vector3(0.378F, 0.378F, 0.378F));
            AddDisplayRule("mdlEngi", "HandL", new Vector3(0.015F, 0.18F, -0.04F), new Vector3(70.126F, 294.778F, 9.124F), new Vector3(0.047F, 0.047F, 0.047F));
            AddDisplayRule("mdlMage", "Head", new Vector3(-0.095F, 0.108F, -0.047F), new Vector3(358.993F, 354.776F, 177.852F), new Vector3(0.016F, 0.016F, 0.016F));
            AddDisplayRule("mdlMerc", "HandL", new Vector3(0.021F, 0.127F, 0.013F), new Vector3(356.714F, 344.677F, 266.091F), new Vector3(0.027F, 0.027F, 0.027F));
            AddDisplayRule("mdlTreebot", "WeaponPlatformEnd", new Vector3(-0.044F, -0.068F, 0.154F), new Vector3(339.636F, 327.135F, 61.693F), new Vector3(0.05F, 0.05F, 0.05F));
            AddDisplayRule("mdlLoader", "MechHandL", new Vector3(0.052F, 0.271F, 0.005F), new Vector3(0.351F, 159.123F, 281.328F), new Vector3(0.052F, 0.05F, 0.052F));
            AddDisplayRule("mdlCroco", "Head", new Vector3(-0.543F, 4.565F, -0.104F), new Vector3(284.494F, 3.244F, 267.429F), new Vector3(0.333F, 0.333F, 0.333F));
            AddDisplayRule("mdlCaptain", "HandR", new Vector3(0.002F, 0.155F, -0.014F), new Vector3(53.771F, 271.897F, 272.032F), new Vector3(0.041F, 0.041F, 0.041F));
            AddDisplayRule("mdlScav", "HandL", new Vector3(0.469F, 2.35F, -0.273F), new Vector3(305.162F, 137.483F, 278.565F), new Vector3(0.883F, 0.883F, 0.883F));
            AddDisplayRule("mdlEquipmentDrone", "GunBarrelBase", new Vector3(0F, 0F, 1.453F), new Vector3(0F, 90F, 0F), new Vector3(0.265F, 0.265F, 0.265F));

            wavePrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Microphone/MicrophoneSoundwaveGhost.prefab");
            wavePrefab.AddComponent<ProjectileGhostController>();

            waveProjectile = Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Microphone/MicrophoneSoundwave.prefab");
            NetworkIdentity networkIdentity = waveProjectile.AddComponent<NetworkIdentity>();
            networkIdentity.localPlayerAuthority = true;
            MicrophoneSoundwaveProjectile msp = waveProjectile.AddComponent<MicrophoneSoundwaveProjectile>();
            msp.sizeCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 8f),
                    new Keyframe(1f, 30f)
                }
            };
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
            ProjectileInflictTimedBuff projectileInflictTimedBuff = waveProjectile.AddComponent<ProjectileInflictTimedBuff>();
            projectileInflictTimedBuff.duration = 15f;

            PrefabAPI.RegisterNetworkPrefab(waveProjectile);
            AssetManager.RegisterProjectile(waveProjectile);
        }

        public class MicrophoneSoundwaveProjectile : NetworkBehaviour
        {
            public Rigidbody rigidbody;
            public ProjectileController controller;
            public ProjectileInflictTimedBuff timedBuff;
            public float stopwatch = 0f;
            public float lifetime = 1f;
            public float initialSpeed = 60f;
            public AnimationCurve sizeCurve;
            public AnimationCurve[] colorCurve;

            public void Start()
            {
                rigidbody = GetComponent<Rigidbody>();
                controller = GetComponent<ProjectileController>();
                timedBuff = GetComponent<ProjectileInflictTimedBuff>();

                rigidbody.velocity = initialSpeed * transform.forward;
                timedBuff.buffIndex = buffIndex;
            }

            public void FixedUpdate()
            {
                stopwatch += Time.fixedDeltaTime;
                float t = stopwatch / lifetime;

                Vector3 scale = Vector3.one * sizeCurve.Evaluate(t);
                transform.localScale = scale;
                if (controller.ghost)
                {
                    controller.ghost.transform.localScale = scale;
                    controller.ghost.GetComponentInChildren<Renderer>().material.color = new Color32(
                        (byte)colorCurve[0].Evaluate(t),
                        (byte)colorCurve[1].Evaluate(t),
                        (byte)colorCurve[2].Evaluate(t),
                        (byte)colorCurve[3].Evaluate(t)
                    );
                }

                if (t >= 1f)
                {
                    Object.Destroy(gameObject);
                }
            }
        }

        public override bool OnUse(EquipmentSlot equipmentSlot)
        {
            MicrophoneSoundwaveLauncher component = equipmentSlot.GetComponent<MicrophoneSoundwaveLauncher>();
            if (!component) component = equipmentSlot.gameObject.AddComponent<MicrophoneSoundwaveLauncher>();
            component.ammo += 3;
            component.aimRay = GetAimRay(equipmentSlot);
            return true;
        }

        public override void OnUseClient(EquipmentSlot equipmentSlot)
        {
            Util.PlaySound("MysticsItems_Play_item_use_microphone", equipmentSlot.characterBody.gameObject);
        }

        public class MicrophoneSoundwaveLauncher : MonoBehaviour
        {
            public int ammo = 0;
            public float interval = 0f;
            public float intervalMax = 0.12f;
            public Ray aimRay = new Ray();
            public EquipmentSlot equipmentSlot;

            public void Awake()
            {
                equipmentSlot = GetComponent<EquipmentSlot>();
            }

            public void FixedUpdate()
            {
                interval -= Time.fixedDeltaTime;
                if (interval <= 0f && ammo > 0)
                {
                    interval = intervalMax;
                    ammo--;

                    Vector3 position = transform.position;
                    ProjectileManager.instance.FireProjectile(waveProjectile, position, Util.QuaternionSafeLookRotation(aimRay.direction), equipmentSlot.gameObject, 0f, 0f, false, DamageColorIndex.Default, null, -1f);
                }
            }
        }
    }
}
