using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Collections.Generic;
using RoR2.Audio;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.LegacyBalanceConfigManager;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;

namespace MysticsItems.Items
{
    public class StarBook : BaseItem
    {
        public static ConfigurableValue<float> chance = new ConfigurableValue<float>(
            "Item: Stargazer s Records",
            "Chance",
            12f,
            "Chance to drop a star on hit (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_STARBOOK_DESC"
            }
        );
        public static ConfigurableValue<float> damage = new ConfigurableValue<float>(
            "Item: Stargazer s Records",
            "Damage",
            250f,
            "Star damage (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_STARBOOK_DESC"
            }
        );
        public static ConfigurableValue<float> damagePerStack = new ConfigurableValue<float>(
            "Item: Stargazer s Records",
            "DamagePerStack",
            200f,
            "Star damage for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_STARBOOK_DESC"
            }
        );
        public static ConfigurableValue<float> radius = new ConfigurableValue<float>(
            "Item: Stargazer s Records",
            "Radius",
            3f,
            "Star explosion radius (in meters)",
            onChanged: (x) => UpdateProjectileConfigValues()
        );
        public static ConfigurableValue<float> delay = new ConfigurableValue<float>(
            "Item: Stargazer s Records",
            "Delay",
            1f,
            "Delay between the hit proc and the star drop (in seconds)",
            onChanged: (x) => UpdateProjectileConfigValues()
        );
        public static ConfigurableValue<float> procCoefficient = new ConfigurableValue<float>(
            "Item: Stargazer s Records",
            "ProcCoefficient",
            1f,
            "Proc coefficient of the falling star projectile",
            onChanged: (x) => UpdateProjectileConfigValues()
        );
        public static ConfigurableValue<float> duration = new ConfigurableValue<float>(
            "Item: Stargazer s Records",
            "Duration",
            10f,
            "Duration of the buff on pickup (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_STARBOOK_DESC"
            }
        );

        public static GameObject starPrefab;
        public static GameObject starProjectilePrefab;
        public static GameObject starProjectileGhostPrefab;

        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            starPrefab = Utils.CreateBlankPrefab("MysticsItems_FallenStar", true);
            starProjectilePrefab = Utils.CreateBlankPrefab("MysticsItems_StarProjectile", true);
            starProjectilePrefab.GetComponent<NetworkIdentity>().localPlayerAuthority = true;
        }

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_StarBook";
            SetItemTierWhenAvailable(ItemTier.Tier2);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Star Book/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Star Book/Icon.png");
            itemDisplayPrefab = PrepareItemDisplayModel(PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Star Book/FollowerModel.prefab")));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Pelvis", new Vector3(0.07214F, -0.02544F, -0.15227F), new Vector3(0F, 180.374F, 194.0942F), new Vector3(0.02608F, 0.02608F, 0.02608F));
                AddDisplayRule("HuntressBody", "Stomach", new Vector3(-0.13024F, -0.01324F, 0.09361F), new Vector3(354.6184F, 328.7896F, 342.3868F), new Vector3(0.02443F, 0.02443F, 0.02443F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(0.05325F, -0.02883F, 0.20731F), new Vector3(0F, 0F, 21.74835F), new Vector3(0.03697F, 0.03697F, 0.03697F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(2.05349F, 0.47913F, 1.24299F), new Vector3(359.5632F, 92.9772F, 91.61461F), new Vector3(0.22716F, 0.22716F, 0.22716F));
                AddDisplayRule("EngiBody", "Stomach", new Vector3(-0.22077F, -0.08811F, 0.08411F), new Vector3(0F, 300.6727F, 182.2371F), new Vector3(0.04205F, 0.04205F, 0.04205F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(-0.10323F, 0.90644F, -0.00808F), new Vector3(270F, 91.60656F, 0F), new Vector3(0.15251F, 0.15251F, 0.15251F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(-0.93491F, 0.69581F, -0.37497F), new Vector3(0F, 271.2291F, 0F), new Vector3(0.21012F, 0.21012F, 0.21012F));
                AddDisplayRule("MageBody", "ThighL", new Vector3(0.16022F, 0.14565F, -0.00145F), new Vector3(7.16198F, 98.86031F, 181.1707F), new Vector3(0.04215F, 0.04215F, 0.04215F));
                AddDisplayRule("MercBody", "Stomach", new Vector3(0.1413F, -0.10351F, 0.14899F), new Vector3(343.5251F, 46.02943F, 0F), new Vector3(0.03358F, 0.03358F, 0.03358F));
                AddDisplayRule("TreebotBody", "Base", new Vector3(-0.08926F, -0.26521F, -0.11175F), new Vector3(62.67739F, 180F, 207.9309F), new Vector3(0.08878F, 0.08878F, 0.08878F));
                AddDisplayRule("LoaderBody", "Stomach", new Vector3(-0.11321F, -0.15681F, 0.19948F), new Vector3(353.8455F, 341.7138F, 358.7739F), new Vector3(0.04343F, 0.04343F, 0.04343F));
                AddDisplayRule("CrocoBody", "HandL", new Vector3(1.7725F, 1.62665F, 0.27285F), new Vector3(88.3203F, 0.00047F, 204.5181F), new Vector3(0.39841F, 0.39841F, 0.39841F));
                AddDisplayRule("CaptainBody", "Stomach", new Vector3(0.17248F, 0.04995F, 0.19214F), new Vector3(14.39783F, 11.03513F, 0F), new Vector3(0.0472F, 0.0472F, 0.0472F));
                AddDisplayRule("BrotherBody", "ThighR", new Vector3(0.30045F, -0.16571F, -0.02125F), new Vector3(3.68902F, 339.9096F, 134.1321F), new Vector3(0.11827F, 0.03684F, 0.11827F));
                AddDisplayRule("ScavBody", "Head", new Vector3(-3.15235F, 1.98803F, -4.28275F), new Vector3(304.0492F, 230.9892F, 190.2168F), new Vector3(0.70357F, 0.70357F, 0.70357F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Pelvis", new Vector3(0.27172F, 0.08232F, -0.06273F), new Vector3(358.4292F, 121.2072F, 339.9804F), new Vector3(0.03388F, 0.03388F, 0.03388F));
                AddDisplayRule("RailgunnerBody", "Pelvis", new Vector3(0.17538F, 0.15436F, -0.01521F), new Vector3(5.45999F, 119.7464F, 17.75949F), new Vector3(0.0379F, 0.0379F, 0.0379F));
                AddDisplayRule("VoidSurvivorBody", "Stomach", new Vector3(0.10394F, -0.04523F, 0.19263F), new Vector3(5.30274F, 0F, 0F), new Vector3(0.03269F, 0.03269F, 0.03269F));
            };

            HopooShaderToMaterial.Standard.Apply(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial);
            HopooShaderToMaterial.Standard.Emission(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial, 1.5f, new Color32(25, 180, 171, 255));

            Utils.CopyChildren(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Star Book/Star.prefab"), starPrefab);
            starPrefab.AddComponent<NetworkTransform>();
            HopooShaderToMaterial.Standard.Apply(starPrefab.GetComponentInChildren<Renderer>().sharedMaterial);
            HopooShaderToMaterial.Standard.Emission(starPrefab.GetComponentInChildren<Renderer>().sharedMaterial, 1f, new Color32(25, 180, 171, 255));

            var velocityOnStart = starPrefab.AddComponent<VelocityRandomOnStart>();
            velocityOnStart.baseDirection = Vector3.down;
            velocityOnStart.coneAngle = 1f;
            velocityOnStart.directionMode = VelocityRandomOnStart.DirectionMode.Cone;
            velocityOnStart.minSpeed = 15f;
            velocityOnStart.maxSpeed = 15f;
            velocityOnStart.minAngularSpeed = 0f;
            velocityOnStart.maxAngularSpeed = 0f;

            var setRandomRotation = starPrefab.transform.Find("mdlStar").gameObject.AddComponent<SetRandomRotation>();
            setRandomRotation.setRandomXRotation = true;
            setRandomRotation.setRandomYRotation = true;
            setRandomRotation.setRandomZRotation = true;

            var destroyOnTimer = starPrefab.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 8f;
            destroyOnTimer.resetAgeOnDisable = false;
            var blink = starPrefab.AddComponent<BeginRapidlyActivatingAndDeactivating>();
            blink.blinkFrequency = 20f;
            blink.delayBeforeBeginningBlinking = destroyOnTimer.duration - 1f;
            blink.blinkingRootObject = starPrefab.transform.Find("mdlStar").gameObject;

            var rigidbody = starPrefab.GetComponent<Rigidbody>();
            starPrefab.AddComponent<RoR2.Projectile.ProjectileNetworkTransform>();
            var teamFilter = starPrefab.AddComponent<TeamFilter>();

            var pickupTrigger = starPrefab.transform.Find("PickupTrigger").gameObject;
            pickupTrigger.AddComponent<TeamFilter>();
            var buffPickup = pickupTrigger.AddComponent<MysticsItemsBuffPickupStar>();
            buffPickup.baseObject = starPrefab;
            buffPickup.teamFilter = teamFilter;
            buffPickup.buffDuration = duration;

            buffPickup.pickupEffect = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Star Book/CollectPickupEffect.prefab");
            var effectComponent = buffPickup.pickupEffect.AddComponent<EffectComponent>();
            effectComponent.soundName = "MysticsItems_Play_fallenStar_collect";
            var vfxAttributes = buffPickup.pickupEffect.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Low;
            buffPickup.pickupEffect.AddComponent<DestroyOnTimer>().duration = 2f;
            MysticsItemsContent.Resources.effectPrefabs.Add(buffPickup.pickupEffect);

            var gravitationController = starPrefab.transform.Find("GravitationController").gameObject;
            var gravitatePickup = gravitationController.AddComponent<GravitatePickup>();
            gravitatePickup.rigidbody = rigidbody;
            gravitatePickup.teamFilter = teamFilter;
            gravitatePickup.acceleration = 5f;
            gravitatePickup.maxSpeed = 40f;

            GenericGameEvents.OnHitEnemy += GenericGameEvents_OnHitEnemy;

            starProjectileGhostPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Star Book/StarProjectileGhost.prefab");
            var ghostController = starProjectileGhostPrefab.AddComponent<ProjectileGhostController>();

            Utils.CopyChildren(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Star Book/StarProjectile.prefab"), starProjectilePrefab);
            starProjectilePrefab.AddComponent<TeamFilter>();
            var projectileController = starProjectilePrefab.AddComponent<ProjectileController>();
            projectileController.ghostPrefab = starProjectileGhostPrefab;
            projectileController.allowPrediction = true;
            projectileController.flightSoundLoop = Addressables.LoadAssetAsync<LoopSoundDef>("RoR2/Base/Brother/lsdBrotherFirePillar.asset").WaitForCompletion();
            starProjectilePrefab.AddComponent<ProjectileNetworkTransform>();
            var projectileDamage = starProjectilePrefab.AddComponent<ProjectileDamage>();
            var projectileImpactExplosion = starProjectilePrefab.AddComponent<MysticsItemsProjectileImpactExplosionStar>();
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.destroyOnWorld = false;
            projectileImpactExplosion.timerAfterImpact = false;
            projectileImpactExplosion.transformSpace = ProjectileImpactExplosion.TransformSpace.World;
            projectileImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.bonusBlastForce = new Vector3(0f, -500f, 0f);

            projectileImpactExplosion.normalEffect = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Star Book/StarProjectileImpactEffect.prefab");
            effectComponent = projectileImpactExplosion.normalEffect.AddComponent<EffectComponent>();
            effectComponent.soundName = "Play_grandParent_attack1_boulderSmall_impact";
            vfxAttributes = projectileImpactExplosion.normalEffect.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            projectileImpactExplosion.normalEffect.AddComponent<DestroyOnTimer>().duration = 2f;
            MysticsItemsContent.Resources.effectPrefabs.Add(projectileImpactExplosion.normalEffect);

            var shakeEmitter = projectileImpactExplosion.normalEffect.AddComponent<ShakeEmitter>();
            shakeEmitter.radius = 5f;
            shakeEmitter.shakeOnStart = true;
            shakeEmitter.shakeOnEnable = false;
            shakeEmitter.duration = 0.5f;
            shakeEmitter.wave = new Wave
            {
                amplitude = 0.2f,
                frequency = 200f
            };
            shakeEmitter.amplitudeTimeDecay = true;
            shakeEmitter.scaleShakeRadiusWithLocalScale = false;

            projectileImpactExplosion.spawnObjectPrefab = starPrefab;
            projectileImpactExplosion.spawnOffset = Vector3.up * 0.5f;

            UpdateProjectileConfigValues();
            MysticsItemsContent.Resources.projectilePrefabs.Add(starProjectilePrefab);
        }

        public override void AfterContentPackLoaded()
        {
            base.AfterContentPackLoaded();
            starPrefab.GetComponentInChildren<MysticsItemsBuffPickupStar>().buffDef = MysticsItemsContent.Buffs.MysticsItems_StarPickup;
        }

        public static void UpdateProjectileConfigValues()
        {
            if (!starProjectilePrefab) return;

            var projectileImpactExplosion = starProjectilePrefab.GetComponent<MysticsItemsProjectileImpactExplosionStar>();
            projectileImpactExplosion.lifetime = delay;
            projectileImpactExplosion.blastRadius = radius;
            projectileImpactExplosion.blastProcCoefficient = procCoefficient;

            var lightRays = starProjectileGhostPrefab.transform.Find("Light Rays").GetComponent<ParticleSystem>();
            lightRays.startLifetime = delay;

            projectileImpactExplosion.normalEffect.transform.Find("Flame Pillar").localScale = new Vector3(radius, 1f, radius);
        }

        public class MysticsItemsBuffPickupStar : MonoBehaviour
        {
            public GameObject baseObject;
            public TeamFilter teamFilter;
            public GameObject pickupEffect;
            public BuffDef buffDef;
            public float buffDuration;
            private bool alive = true;

            public void OnTriggerStay(Collider other)
            {
                if (NetworkServer.active && alive && TeamComponent.GetObjectTeam(other.gameObject) == teamFilter.teamIndex)
                {
                    var body = other.GetComponent<CharacterBody>();
                    if (body)
                    {
                        body.AddTimedBuff(buffDef.buffIndex, buffDuration);
                        EffectManager.SpawnEffect(pickupEffect, new EffectData
                        {
                            origin = other.transform.position
                        }, true);
                        MysticsItems.OtherModCompat.ExplosivePickups_TryExplode(body);
                        Destroy(baseObject);
                    }
                }
            }
        }

        public class MysticsItemsProjectileImpactExplosionStar : ProjectileImpactExplosion
        {
            public GameObject spawnObjectPrefab;
            public Vector3 spawnOffset;

            public GameObject normalEffect;

            public override void OnBlastAttackResult(BlastAttack blastAttack, BlastAttack.Result result)
            {
                base.OnBlastAttackResult(blastAttack, result);
                if (NetworkServer.active)
                {
                    if (spawnObjectPrefab)
                    {
                        var spawnedObject = Instantiate(spawnObjectPrefab, transform.position + spawnOffset, transform.rotation);
                        var teamFilter = spawnedObject.GetComponent<TeamFilter>();
                        if (teamFilter)
                        {
                            teamFilter.teamIndex = GetComponent<ProjectileController>().teamFilter.teamIndex;
                        }
                        NetworkServer.Spawn(spawnedObject);
                    }
                    if (normalEffect)
                    {
                        EffectManager.SpawnEffect(normalEffect, new EffectData
                        {
                            origin = transform.position,
                            rotation = transform.rotation
                        }, true);
                    }
                }
            }
        }

        private void GenericGameEvents_OnHitEnemy(DamageInfo damageInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo attackerInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo victimInfo)
        {
            if (!damageInfo.rejected && damageInfo.procCoefficient > 0f && !damageInfo.procChainMask.HasProc(ProcType.LoaderLightning) && attackerInfo.inventory)
            {
                var itemCount = attackerInfo.inventory.GetItemCount(itemDef);
                if (itemCount > 0 && Util.CheckRoll(chance * damageInfo.procCoefficient, attackerInfo.master))
                {
                    var fireProjectileInfo = new FireProjectileInfo
                    {
                        projectilePrefab = starProjectilePrefab,
                        position = damageInfo.position,
                        rotation = Util.QuaternionSafeLookRotation(Util.ApplySpread(Vector3.forward, -20f, 20f, 1f, 1f, 0f, 0f)) * Quaternion.Euler(-90f, 0f, 0f),
                        owner = attackerInfo.gameObject,
                        damage = damageInfo.damage * (damage + damagePerStack * (float)(itemCount - 1)) / 100f,
                        force = 0f,
                        crit = attackerInfo.body.RollCrit(),
                        damageColorIndex = DamageColorIndex.Item,
                        procChainMask = damageInfo.procChainMask
                    };
                    fireProjectileInfo.procChainMask.AddProc(ProcType.LoaderLightning);
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                }
            }
        }
    }
}
