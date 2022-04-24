using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using static MysticsItems.BalanceConfigManager;

namespace MysticsItems.Items
{
    public class ExplosivePickups : BaseItem
    {
        public static GameObject gunpowderPickup;
        public static GameObject explosionPrefab;

        public static ConfigurableValue<float> damage = new ConfigurableValue<float>(
            "Item: Contraband Gunpowder",
            "Damage",
            500f,
            "Explosion damage (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_EXPLOSIVEPICKUPS_DESC"
            }
        );
        public static ConfigurableValue<float> damagePerStack = new ConfigurableValue<float>(
            "Item: Contraband Gunpowder",
            "DamagePerStack",
            400f,
            "Explosion damage for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_EXPLOSIVEPICKUPS_DESC"
            }
        );
        public static ConfigurableValue<float> radius = new ConfigurableValue<float>(
            "Item: Contraband Gunpowder",
            "Radius",
            15f,
            "Explosion radius (in meters)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_EXPLOSIVEPICKUPS_DESC"
            }
        );
        public static ConfigurableValue<float> radiusPerStack = new ConfigurableValue<float>(
            "Item: Contraband Gunpowder",
            "RadiusPerStack",
            3f,
            "Explosion radius for each additional stack of this item (in meters)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_EXPLOSIVEPICKUPS_DESC"
            }
        );
        public static ConfigurableValue<float> procCoefficient = new ConfigurableValue<float>(
            "Item: Contraband Gunpowder",
            "ProcCoefficient",
            1f,
            "Explosion proc coefficient (in %)"
        );
        public static ConfigurableValue<float> flaskDropChance = new ConfigurableValue<float>(
            "Item: Contraband Gunpowder",
            "FlaskDropChance",
            25f,
            "Chance on kill to drop a powder flask pickup (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_EXPLOSIVEPICKUPS_DESC"
            }
        );

        public override void OnPluginAwake()
        {
            gunpowderPickup = MysticsRisky2Utils.Utils.CreateBlankPrefab("MysticsItems_ExplosivePack", true);
        }

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_ExplosivePickups";
            SetItemTierWhenAvailable(ItemTier.Tier2);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.OnKillEffect
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Contraband Gunpowder/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Contraband Gunpowder/Icon.png");
            HopooShaderToMaterial.Standard.Apply(itemDef.pickupModelPrefab.transform.Find("мешок").Find("порох").GetComponent<MeshRenderer>().sharedMaterial);
            HopooShaderToMaterial.Standard.Apply(itemDef.pickupModelPrefab.transform.Find("мешок").GetComponent<MeshRenderer>().sharedMaterial);
            HopooShaderToMaterial.Standard.Apply(itemDef.pickupModelPrefab.transform.Find("мешок").Find("верёвка").GetComponent<MeshRenderer>().sharedMaterial);
            HopooShaderToMaterial.Standard.Gloss(itemDef.pickupModelPrefab.transform.Find("мешок").Find("порох").GetComponent<MeshRenderer>().sharedMaterial, 0f);
            HopooShaderToMaterial.Standard.Gloss(itemDef.pickupModelPrefab.transform.Find("мешок").GetComponent<MeshRenderer>().sharedMaterial, 0f);
            HopooShaderToMaterial.Standard.Gloss(itemDef.pickupModelPrefab.transform.Find("мешок").Find("верёвка").GetComponent<MeshRenderer>().sharedMaterial, 0.4f);
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Stomach", new Vector3(-0.175F, 0.066F, 0.045F), new Vector3(16.687F, 66.665F, 36.228F), new Vector3(0.042F, 0.042F, 0.042F));
                AddDisplayRule("HuntressBody", "Pelvis", new Vector3(-0.12F, -0.064F, -0.052F), new Vector3(355.162F, 32.177F, 180.96F), new Vector3(0.042F, 0.042F, 0.042F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(0.156F, 0.031F, 0.127F), new Vector3(350.191F, 244.703F, 340.178F), new Vector3(0.037F, 0.037F, 0.037F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(-0.837F, 1.169F, 3.112F), new Vector3(29.795F, 9.384F, 2.716F), new Vector3(0.489F, 0.489F, 0.489F));
                AddDisplayRule("EngiBody", "Pelvis", new Vector3(-0.206F, 0.04F, -0.104F), new Vector3(4.991F, 46.464F, 181.437F), new Vector3(0.065F, 0.065F, 0.065F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.834F, 0.462F, 0.717F), new Vector3(33.04F, 48.09F, 359.072F), new Vector3(0.168F, 0.168F, 0.168F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.715F, 0.235F, 0.228F), new Vector3(22.677F, 152.024F, 24.393F), new Vector3(0.134F, 0.163F, 0.131F));
                AddDisplayRule("MageBody", "Pelvis", new Vector3(-0.058F, 0F, -0.164F), new Vector3(0.366F, 347.899F, 165.881F), new Vector3(0.044F, 0.044F, 0.044F));
                AddDisplayRule("MercBody", "ThighR", new Vector3(-0.077F, 0.008F, 0.041F), new Vector3(15.315F, 124.284F, 220.104F), new Vector3(0.034F, 0.034F, 0.034F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(-0.062F, -0.523F, -1.156F), new Vector3(41.662F, 244.258F, 1.504F), new Vector3(0.107F, 0.107F, 0.107F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(0.07F, 0.023F, 0.444F), new Vector3(7.628F, 218.893F, 342.184F), new Vector3(0.054F, 0.054F, 0.054F));
                AddDisplayRule("CrocoBody", "SpineChest2", new Vector3(0.779F, 1.753F, -0.514F), new Vector3(337.83F, 226.76F, 273.311F), new Vector3(0.411F, 0.411F, 0.411F));
                AddDisplayRule("CaptainBody", "Stomach", new Vector3(-0.102F, 0.12F, 0.147F), new Vector3(11.46F, 212.011F, 335.706F), new Vector3(0.053F, 0.048F, 0.053F));
                AddDisplayRule("BrotherBody", "Stomach", BrotherInfection.green, new Vector3(-0.18F, 0.131F, 0.075F), new Vector3(303.36F, 82.78F, 283.641F), new Vector3(0.063F, 0.063F, 0.063F));
                AddDisplayRule("ScavBody", "MuzzleEnergyCannon", new Vector3(0.586F, 3.872F, 0.073F), new Vector3(54.107F, 148.5F, 149.008F), new Vector3(0.835F, 0.858F, 0.835F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Pelvis", new Vector3(-0.19573F, 0.1786F, -0.09573F), new Vector3(4.5648F, 55.41101F, 9.99794F), new Vector3(0.053F, 0.053F, 0.053F));
                AddDisplayRule("RailgunnerBody", "Pelvis", new Vector3(-0.15453F, 0.10747F, -0.16311F), new Vector3(359.8358F, 321.1621F, 178.6592F), new Vector3(0.05825F, 0.05825F, 0.05825F));
                AddDisplayRule("VoidSurvivorBody", "Neck", new Vector3(0.08624F, 0.00251F, 0.20226F), new Vector3(23.68506F, 25.82453F, 8.87328F), new Vector3(0.062F, 0.062F, 0.062F));
            };

            MysticsRisky2Utils.Utils.CopyChildren(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Contraband Gunpowder/ExplosivePack.prefab"), gunpowderPickup);
            gunpowderPickup.transform.localScale *= 0.33f;
            
            gunpowderPickup.layer = LayerIndex.debris.intVal;

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

            explosionPrefab = PrefabAPI.InstantiateClone(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Contraband Gunpowder/Explosion.prefab"), "MysticsItems_OmniExplosionVFXExplosivePickups", false);
            if (GeneralConfigManager.gunpowderReduceVFX.Value)
            {
                Object.Destroy(explosionPrefab.transform.Find("Light Flash").gameObject);
                Object.Destroy(explosionPrefab.transform.Find("Sparks").gameObject);
                Object.Destroy(explosionPrefab.transform.Find("Swirls").gameObject);
            }
            VFXAttributes vfxAttributes = explosionPrefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.High;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            EffectComponent effectComponent = explosionPrefab.AddComponent<EffectComponent>();
            effectComponent.applyScale = true;
            effectComponent.soundName = GeneralConfigManager.gunpowderDisableSound.Value ? "" : "MysticsItems_Play_item_proc_gunpowder";
            explosionPrefab.AddComponent<DestroyOnTimer>().duration = 2f;
            ShakeEmitter shakeEmitter = explosionPrefab.AddComponent<ShakeEmitter>();
            shakeEmitter.duration = 0.1f;
            shakeEmitter.scaleShakeRadiusWithLocalScale = true;
            shakeEmitter.amplitudeTimeDecay = true;
            shakeEmitter.radius = 1.5f;
            shakeEmitter.shakeOnStart = true;
            shakeEmitter.wave = new Wave
            {
                amplitude = 9f * GeneralConfigManager.gunpowderScreenshakeScale.Value,
                frequency = 4f * GeneralConfigManager.gunpowderScreenshakeScale.Value
            };
            MysticsItemsContent.Resources.effectPrefabs.Add(explosionPrefab);

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
                orig(self, damageReport);
                if (self.inventory && self.inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_ExplosivePickups) > 0 && self.master && Util.CheckRoll(flaskDropChance, self.master))
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
                            if (inventory && inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_ExplosivePickups) > 0)
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
                            if (inventory && inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_ExplosivePickups) > 0)
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
                            if (inventory && inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_ExplosivePickups) > 0)
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
                            if (inventory && inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_ExplosivePickups) > 0)
                            {
                                Explode(body);
                            }
                        }
                    });
                }
            };
        }

        public static void Explode(CharacterBody body)
        {
            if (body.inventory) {
                int itemCount = body.inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_ExplosivePickups);
                if (itemCount > 0) Explode(body.gameObject, body.corePosition, body.damage * damage / 100f + damagePerStack / 100f * (float)(itemCount - 1), radius + radiusPerStack * (float)(itemCount - 1), body.RollCrit());
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
                    procCoefficient = procCoefficient,
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
                itemIndex = MysticsItemsContent.Items.MysticsItems_ExplosivePickups.itemIndex;
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
                itemIndex = MysticsItemsContent.Items.MysticsItems_ExplosivePickups.itemIndex;
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
    }
}
