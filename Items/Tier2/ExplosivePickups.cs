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

        public override void PreAdd()
        {
            itemDef.name = "ExplosivePickups";
            itemDef.tier = ItemTier.Tier2;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist
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
            destroyOnTimer.duration = 10f;
            destroyOnTimer.resetAgeOnDisable = false;
            BeginRapidlyActivatingAndDeactivating blink = gunpowderPickup.AddComponent<BeginRapidlyActivatingAndDeactivating>();
            blink.blinkFrequency = 20f;
            blink.delayBeforeBeginningBlinking = 9f;
            blink.blinkingRootObject = gunpowderPickup.transform.Find("мешок").gameObject;
            Rigidbody rigidbody = gunpowderPickup.AddComponent<Rigidbody>();
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
        }

        public static void Explode(CharacterBody body)
        {
            if (body.inventory) {
                int itemCount = body.inventory.GetItemCount(GetFromType(typeof(ExplosivePickups)).itemIndex);
                Explode(body.gameObject, body.corePosition, body.damage * 2f + 1.5f * (float)(itemCount - 1), 13f + 3f * (float)(itemCount - 1), body.RollCrit());
            }
        }

        public static void Explode(GameObject attacker, Vector3 position, float damage, float radius, bool crit)
        {
            if (NetworkServer.active)
            {
                EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
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
