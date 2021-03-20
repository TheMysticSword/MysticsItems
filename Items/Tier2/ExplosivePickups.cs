using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Items
{
    public class ExplosivePickups : BaseItem
    {
        public static GameObject gunpowderPickup;
        public static GameObject explosionPrefab;

        public override void PreAdd()
        {
            itemDef.name = "ExplosivePickups";
            itemDef.tier = ItemTier.Tier2;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.OnKillEffect
            };
            SetAssets("Contraband Gunpowder");
            Main.HopooShaderToMaterial.Standard.Apply(
                model.transform.Find("мешок").Find("порох").GetComponent<MeshRenderer>().sharedMaterial,
                model.transform.Find("мешок").GetComponent<MeshRenderer>().sharedMaterial,
                model.transform.Find("мешок").Find("верёвка").GetComponent<MeshRenderer>().sharedMaterial
            );
            Main.HopooShaderToMaterial.Standard.Gloss(model.transform.Find("мешок").Find("порох").GetComponent<MeshRenderer>().sharedMaterial, 0f);
            Main.HopooShaderToMaterial.Standard.Gloss(model.transform.Find("мешок").GetComponent<MeshRenderer>().sharedMaterial, 0f);
            Main.HopooShaderToMaterial.Standard.Gloss(model.transform.Find("мешок").Find("верёвка").GetComponent<MeshRenderer>().sharedMaterial, 0.4f);

            gunpowderPickup = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Contraband Gunpowder/ExplosivePack.prefab");
            gunpowderPickup.transform.localScale *= 0.33f;
            
            gunpowderPickup.layer = LayerIndex.debris.intVal;
            gunpowderPickup.AddComponent<NetworkIdentity>();

            DestroyOnTimer destroyOnTimer = gunpowderPickup.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 60f;
            destroyOnTimer.resetAgeOnDisable = false;
            BeginRapidlyActivatingAndDeactivating blink = gunpowderPickup.AddComponent<BeginRapidlyActivatingAndDeactivating>();
            blink.blinkFrequency = 20f;
            blink.delayBeforeBeginningBlinking = destroyOnTimer.duration - 1f;
            blink.blinkingRootObject = gunpowderPickup.transform.Find("мешок").gameObject;

            Rigidbody rigidbody = gunpowderPickup.GetComponent<Rigidbody>();
            VelocityRandomOnStart velocity = gunpowderPickup.AddComponent<VelocityRandomOnStart>();
            velocity.minSpeed = 10f;
            velocity.maxSpeed = 20f;
            velocity.baseDirection = Vector3.up;
            velocity.localDirection = false;
            velocity.directionMode = VelocityRandomOnStart.DirectionMode.Cone;
            velocity.coneAngle = 15f;
            velocity.maxAngularSpeed = velocity.minAngularSpeed = 0f;
            gunpowderPickup.AddComponent<RoR2.Projectile.ProjectileNetworkTransform>();
            TeamFilter teamFilter = gunpowderPickup.AddComponent<TeamFilter>();

            GameObject pickupTrigger = gunpowderPickup.transform.Find("PickupTrigger").gameObject;
            pickupTrigger.layer = LayerIndex.pickups.intVal;
            pickupTrigger.AddComponent<TeamFilter>();
            ExplosivePack explosivePack = pickupTrigger.AddComponent<ExplosivePack>();
            explosivePack.baseObject = gunpowderPickup;
            explosivePack.teamFilter = teamFilter;

            GameObject gravitationController = gunpowderPickup.transform.Find("GravitationController").gameObject;
            ExplosivePackGravitate gravitatePickup = gravitationController.AddComponent<ExplosivePackGravitate>();
            gravitatePickup.rigidbody = rigidbody;
            gravitatePickup.teamFilter = teamFilter;
            gravitatePickup.acceleration = 5f;
            gravitatePickup.maxSpeed = 40f;

            PrefabAPI.RegisterNetworkPrefab(gunpowderPickup);

            /*
            explosionPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Contraband Gunpowder/Explosion.prefab");
            EffectComponent effectComponent = explosionPrefab.AddComponent<EffectComponent>();
            effectComponent.applyScale = true;
            effectComponent.soundName = "Play_mage_m1_impact";
            VFXAttributes vfxAttributes = explosionPrefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
            vfxAttributes.optionalLights = new Light[]
            {
                explosionPrefab.transform.Find("Point Light").gameObject.GetComponent<Light>()
            };
            vfxAttributes.secondaryParticleSystem = new ParticleSystem[]
            {
                explosionPrefab.transform.Find("Big Circle").gameObject.GetComponent<ParticleSystem>(),
                explosionPrefab.transform.Find("Small Circles").gameObject.GetComponent<ParticleSystem>()
            };
            DestroyOnTimer destroyOnTimer1 = explosionPrefab.AddComponent<DestroyOnTimer>();
            destroyOnTimer1.duration = 2f;
            ShakeEmitter shakeEmitter = explosionPrefab.AddComponent<ShakeEmitter>();
            shakeEmitter.shakeOnStart = true;
            shakeEmitter.wave = new Wave
            {
                amplitude = 1f,
                frequency = 180f,
                cycleOffset = 0f
            };
            shakeEmitter.duration = 0.15f;
            shakeEmitter.radius = 10f;
            shakeEmitter.scaleShakeRadiusWithLocalScale = true;
            shakeEmitter.amplitudeTimeDecay = true;
            explosionPrefab.transform.Find("Point Light").gameObject.GetComponent<Light>().gameObject.AddComponent<MysticsItemsScaleLight>();

            AssetManager.RegisterEffect(explosionPrefab);
            */

            explosionPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), Main.TokenPrefix + "OmiExplosionVFXExplosivePickups", false);
            Object.Destroy(explosionPrefab.transform.Find("ScaledHitsparks 1").gameObject);
            Object.Destroy(explosionPrefab.transform.Find("UnscaledHitsparks 1").gameObject);
            Object.Destroy(explosionPrefab.transform.Find("ScaledSmoke, Billboard").gameObject);
            Object.Destroy(explosionPrefab.transform.Find("ScaledSmokeRing, Mesh").gameObject);
            Object.Destroy(explosionPrefab.transform.Find("Unscaled Smoke, Billboard").gameObject);
            //Object.Destroy(explosionPrefab.transform.Find("AreaIndicatorRing, Billboard").gameObject);
            Object.Destroy(explosionPrefab.transform.Find("AreaIndicatorRing, Random Billboard").gameObject);
            Object.Destroy(explosionPrefab.transform.Find("Physics Sparks").gameObject);
            Object.Destroy(explosionPrefab.transform.Find("Flash, Soft Glow").gameObject);
            Object.Destroy(explosionPrefab.transform.Find("Unscaled Flames").gameObject);
            Object.Destroy(explosionPrefab.transform.Find("Dash, Bright").gameObject);
            Object.Destroy(explosionPrefab.transform.Find("Point Light").gameObject);
            AssetManager.RegisterEffect(explosionPrefab);
        }

        public static void Explode(CharacterBody body)
        {
            if (body.inventory) {
                int itemCount = body.inventory.GetItemCount(GetFromType(typeof(ExplosivePickups)).itemIndex);
                Explode(body.gameObject, body.corePosition, body.damage * 2.5f + 2f * (float)(itemCount - 1), 8f + 1.6f * (float)(itemCount - 1), body.RollCrit());
            }
        }

        public static void Explode(GameObject attacker, Vector3 position, float damage, float radius, bool crit)
        {
            if (NetworkServer.active)
            {
                EffectManager.SpawnEffect(explosionPrefab, new EffectData
                {
                    origin = position,
                    scale = radius,
                    rotation = Random.rotation
                }, true);
                BlastAttack blastAttack = new BlastAttack
                {
                    position = position,
                    baseDamage = damage,
                    baseForce = 0f,
                    radius = radius,
                    attacker = attacker,
                    inflictor = null,
                    teamIndex = TeamComponent.GetObjectTeam(attacker),
                    crit = crit,
                    procChainMask = default,
                    procCoefficient = 0f, // Keep this at 0% to prevent elemental band proccing
                    damageColorIndex = DamageColorIndex.Item,
                    falloffModel = BlastAttack.FalloffModel.None,
                    damageType = DamageType.AOE
                };
                blastAttack.Fire();
            }
        }

        public class ExplosivePack : MonoBehaviour
        {
            public bool picked = false;
            public GameObject baseObject;
            public TeamFilter teamFilter;
            public ItemIndex itemIndex;

            public void Awake()
            {
                itemIndex = GetFromType(typeof(ExplosivePickups)).itemIndex;
            }

            public void OnTriggerStay(Collider collider)
            {
                if (NetworkServer.active && !picked && TeamComponent.GetObjectTeam(collider.gameObject) == teamFilter.teamIndex)
                {
                    CharacterBody body = collider.GetComponent<CharacterBody>();
                    if (body)
                    {
                        Inventory inventory = body.inventory;
                        if (inventory && inventory.GetItemCount(itemIndex) > 0)
                        {
                            Explode(body);
                            picked = true;
                            Object.Destroy(baseObject);
                        }
                    }
                }
            }
        }

        public class ExplosivePackGravitate : MonoBehaviour
        {
            private Transform gravitateTarget;
            public Rigidbody rigidbody;
            public TeamFilter teamFilter;
            public float acceleration;
            public float maxSpeed;
            public ItemIndex itemIndex;

            public void Awake()
            {
                itemIndex = GetFromType(typeof(ExplosivePickups)).itemIndex;
            }

            public void FixedUpdate()
            {
                if (gravitateTarget)
                {
                    rigidbody.velocity = Vector3.MoveTowards(rigidbody.velocity, (gravitateTarget.transform.position - transform.position).normalized * maxSpeed, acceleration);
                }
            }

            public void OnTriggerEnter(Collider other)
            {
                if (NetworkServer.active && !gravitateTarget && teamFilter.teamIndex != TeamIndex.None && TeamComponent.GetObjectTeam(other.gameObject) == teamFilter.teamIndex)
                {
                    CharacterBody body = other.GetComponent<CharacterBody>();
                    if (body)
                    {
                        Inventory inventory = body.inventory;
                        if (inventory && inventory.GetItemCount(itemIndex) > 0)
                        {
                            gravitateTarget = other.gameObject.transform;
                        }
                    }
                }
            }
        }

        public override void OnAdd()
        {
            /*
            Main.OnHitEnemy += delegate (DamageInfo damageInfo, Main.GenericCharacterInfo attackerInfo, Main.GenericCharacterInfo victimInfo)
            {
                if (NetworkServer.active)
                {
                    if (attackerInfo.inventory && attackerInfo.inventory.GetItemCount(itemIndex) > 0)
                    {
                        if (Util.CheckRoll(7f, attackerInfo.master))
                        {
                            GameObject gameObject = Object.Instantiate(gunpowderPickup, damageInfo.position, Quaternion.identity);
                            gameObject.GetComponent<TeamFilter>().teamIndex = attackerInfo.teamIndex;
                            NetworkServer.Spawn(gameObject);
                        }
                    }
                }
            };
            */
            On.RoR2.CharacterBody.HandleOnKillEffectsServer += (orig, self, damageReport) =>
            {
                if (self.inventory && self.inventory.GetItemCount(itemIndex) > 0)
                {
                    GameObject gameObject = Object.Instantiate(gunpowderPickup, Util.GetCorePosition(damageReport.victim.gameObject), Quaternion.Euler(Random.onUnitSphere.normalized));
                    gameObject.GetComponent<TeamFilter>().teamIndex = TeamComponent.GetObjectTeam(self.gameObject);
                    NetworkServer.Spawn(gameObject);
                }
            };

            IL.RoR2.HealthPickup.OnTriggerStay += (il) =>
            {
                ILCursor c = new ILCursor(il);
                
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<HealthPickup>("baseObject"),
                    x => x.MatchCallOrCallvirt<Object>("Destroy")
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldarg_1);
                    c.EmitDelegate<System.Action<MonoBehaviour, Collider>>((pickup, collider) =>
                    {
                        CharacterBody body = collider.GetComponent<CharacterBody>();
                        if (body)
                        {
                            Inventory inventory = body.inventory;
                            if (inventory && inventory.GetItemCount(itemIndex) > 0)
                            {
                                Explode(body);
                            }
                        }
                    });
                }
            };

            IL.RoR2.MoneyPickup.OnTriggerStay += (il) =>
            {
                ILCursor c = new ILCursor(il);

                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<MoneyPickup>("baseObject"),
                    x => x.MatchCallOrCallvirt<Object>("Destroy")
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldarg_1);
                    c.EmitDelegate<System.Action<MonoBehaviour, Collider>>((pickup, collider) =>
                    {
                        CharacterBody body = collider.GetComponent<CharacterBody>();
                        if (body)
                        {
                            Inventory inventory = body.inventory;
                            if (inventory && inventory.GetItemCount(itemIndex) > 0)
                            {
                                Explode(body);
                            }
                        }
                    });
                }
            };

            IL.RoR2.BuffPickup.OnTriggerStay += (il) =>
            {
                ILCursor c = new ILCursor(il);

                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<BuffPickup>("baseObject"),
                    x => x.MatchCallOrCallvirt<Object>("Destroy")
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldarg_1);
                    c.EmitDelegate<System.Action<MonoBehaviour, Collider>>((pickup, collider) =>
                    {
                        CharacterBody body = collider.GetComponent<CharacterBody>();
                        if (body)
                        {
                            Inventory inventory = body.inventory;
                            if (inventory && inventory.GetItemCount(itemIndex) > 0)
                            {
                                Explode(body);
                            }
                        }
                    });
                }
            };

            IL.RoR2.AmmoPickup.OnTriggerStay += (il) =>
            {
                ILCursor c = new ILCursor(il);

                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<AmmoPickup>("baseObject"),
                    x => x.MatchCallOrCallvirt<Object>("Destroy")
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldarg_1);
                    c.EmitDelegate<System.Action<MonoBehaviour, Collider>>((pickup, collider) =>
                    {
                        CharacterBody body = collider.GetComponent<CharacterBody>();
                        if (body)
                        {
                            Inventory inventory = body.inventory;
                            if (inventory && inventory.GetItemCount(itemIndex) > 0)
                            {
                                Explode(body);
                            }
                        }
                    });
                }
            };
        }
    }
}
