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

namespace MysticsItems.Items
{
    public class SnowRing : BaseItem
    {
        public static ConfigurableValue<float> chance = new ConfigurableValue<float>(
            "Item: Snow Ring",
            "Chance",
            33f,
            "Chance to trigger this item's effect (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_SNOWRING_DESC"
            }
        );
        public static ConfigurableValue<int> targets = new ConfigurableValue<int>(
            "Item: Snow Ring",
            "Targets",
            1,
            "How many enemies to freeze",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_SNOWRING_DESC"
            }
        );
        public static ConfigurableValue<int> targetsPerStack = new ConfigurableValue<int>(
            "Item: Snow Ring",
            "TargetsPerStack",
            1,
            "How many enemies to freeze for each additional stack of this item",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_SNOWRING_DESC"
            }
        );
        public static ConfigurableValue<float> radius = new ConfigurableValue<float>(
            "Item: Snow Ring",
            "Radius",
            15f,
            "Range of search for nearby enemies to freeze (in meters)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_SNOWRING_DESC"
            }
        );
        public static ConfigurableValue<float> radiusPerStack = new ConfigurableValue<float>(
            "Item: Snow Ring",
            "RadiusPerStack",
            1f,
            "Range of search for nearby enemies to freeze for each additional stack of this item (in meters)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_SNOWRING_DESC"
            }
        );
        public static ConfigurableValue<float> freezeDuration = new ConfigurableValue<float>(
            "Item: Snow Ring",
            "FreezeDuration",
            2f,
            "Duration of the freeze effect (in seconds)"
        );

        public static GameObject snowRingOrbEffectPrefab;

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_SnowRing";
            SetItemTierWhenAvailable(ItemTier.Tier2);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Snow Ring/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Snow Ring/Icon.png");
            MysticsItemsContent.Resources.unlockableDefs.Add(GetUnlockableDef());

            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlCommandoDualies",
                transformLocation = "CommandoArmature/ROOT/base/stomach/chest/upper_arm.r/lower_arm.r/hand.r/finger1.1.r",
                childName = "Finger1.1.R"
            });
            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlHuntress",
                transformLocation = "HuntressArmature/ROOT/base/stomach/chest/upper_arm.r/lower_arm.r/hand.r/finger1.1.r",
                childName = "Finger1.1.R"
            });
            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlBandit2",
                transformLocation = "BanditArmature/ROOT/base/stomach/chest/clavicle.r/upper_arm.r/lower_arm.r/hand.r/finger1.1.r",
                childName = "Finger1.1.R"
            });
            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlToolbot",
                transformLocation = "ToolbotArmature/ROOT/base/stomach/chest/upper_arm.r/lower_arm.r/hand.r/finger1.1.r",
                childName = "Finger1.1.R"
            });
            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlEngi",
                transformLocation = "EngiArmature/ROOT/base/stomach/chest/upper_arm.r/lower_arm.r/hand.r/finger1.1.r",
                childName = "Finger1.1.R"
            });
            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlMage",
                transformLocation = "MageArmature/ROOT/base/stomach/chest/clavicle.r/upper_arm.r/lower_arm.r/hand.r/finger1.1.r",
                childName = "Finger1.1.R"
            });
            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlMerc",
                transformLocation = "MercArmature/ROOT/base/stomach/chest/clavicle.r/upper_arm.r/lower_arm.r/hand.r/finger1.1.r",
                childName = "Finger1.1.R"
            });
            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlLoader",
                transformLocation = "LoaderArmature/ROOT/base/stomach/chest/clavicle.r/upper_arm.r/lower_arm.r/hand.r/finger1.1.r",
                childName = "Finger1.1.R"
            });
            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlCroco",
                transformLocation = "CrocoArmature/ROOT/base/stomach/chest/clavicle.r/upper_arm.r/lower_arm.r/hand.r/finger1.1.r",
                childName = "Finger1.1.R"
            });
            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlCaptain",
                transformLocation = "CaptainArmature/ROOT/base/stomach/chest/clavicle.r/upper_arm.r/lower_arm.r/hand.r/finger1.1.r",
                childName = "Finger1.1.R"
            });
            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlBrother",
                transformLocation = "BrotherArmature/ROOT/base/stomach/chest/clavicle.r/upper_arm.r/lower_arm.r/hand.r/finger1.1.r",
                childName = "Finger1.1.R"
            });
            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlScav",
                transformLocation = "ScavArmature/ROOT/base/stomach/chest/clavicle.r/upper_arm.r/lower_arm.r/hand.r/finger1.1.r",
                childName = "Finger1.1.R"
            });
            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlRailGunner",
                transformLocation = "RailGunnerArmature/ROOT/base/stomach/chest/clavicle.r/upper_arm.r/lower_arm.r/hand.r/finger1.1.r",
                childName = "Finger1.1.R"
            });
            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlVoidSurvivor",
                transformLocation = "VoidSurvivorArmature/ROOT/base/Stomach/Chest/Shoulder.l/Upperarm.l/Forearm.l/Hand/Index1",
                childName = "IndexFinger1L"
            });
            if (SoftDependencies.SoftDependenciesCore.itemDisplaysDeputy)
            {
                ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
                {
                    modelName = "mdlDeputy",
                    transformLocation = "Rig/ROOT/base/Stomach/Stomach.1/Chest/Clavicle.L/Upper_arm.L/Lower_arm.L/Hand.L/Index.L",
                    childName = "IndexFingerL"
                });
            }
            itemDisplayPrefab = PrepareItemDisplayModel(PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Snow Ring/FollowerModel.prefab")));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Finger1.1.R", new Vector3(0.00278F, 0.02057F, -0.00242F), new Vector3(17.76288F, 206.6698F, 353.4145F), new Vector3(0.04084F, 0.04071F, 0.04084F));
                AddDisplayRule("HuntressBody", "Finger1.1.R", new Vector3(-0.00001F, 0.01474F, 0.0176F), new Vector3(0F, 0F, 0F), new Vector3(0.03081F, 0.03081F, 0.03081F));
                AddDisplayRule("Bandit2Body", "Finger1.1.R", new Vector3(-0.00312F, 0.00505F, 0.00709F), new Vector3(0F, 46.98147F, 0F), new Vector3(0.0259F, 0.0259F, 0.0259F));
                AddDisplayRule("ToolbotBody", "Finger11R", new Vector3(0F, 0.36673F, 0.00008F), new Vector3(0F, 265.9748F, 0F), new Vector3(0.39985F, 0.39985F, 0.39985F));
                AddDisplayRule("EngiBody", "Finger1.1.R", new Vector3(0.00661F, 0.05113F, -0.00281F), new Vector3(0F, 202.9878F, 0F), new Vector3(0.0371F, 0.0371F, 0.0371F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.41921F, 0.72896F, 0.66925F), new Vector3(352.6319F, 62.13663F, 0.00001F), new Vector3(0.17382F, 0.17382F, 0.17382F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(-0.64363F, 0.89352F, -1.50497F), new Vector3(90F, 90.00001F, 0F), new Vector3(0.11598F, 0.11598F, 0.11598F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.64363F, 0.89352F, -1.50497F), new Vector3(90F, 90.00001F, 0F), new Vector3(0.11598F, 0.11598F, 0.11598F));
                AddDisplayRule("MageBody", "Finger1.1.R", new Vector3(0.01268F, 0.03154F, -0.00041F), new Vector3(345.3728F, 212.2031F, 351.5826F), new Vector3(0.03327F, 0.03327F, 0.03327F));
                AddDisplayRule("MercBody", "Finger1.1.R", new Vector3(-0.00809F, 0.10248F, 0.00681F), new Vector3(346.9798F, 0.72021F, 3.60836F), new Vector3(0.04172F, 0.04172F, 0.04172F));
                AddDisplayRule("TreebotBody", "MuzzleSyringe", new Vector3(0.00001F, -0.03513F, -0.00304F), new Vector3(85.07714F, 180F, 180F), new Vector3(0.07588F, 0.07588F, 0.07588F));
                AddDisplayRule("LoaderBody", "Finger1.1.R", new Vector3(0.00044F, 0.01515F, 0.00604F), new Vector3(314.6832F, 19.93609F, 345.5392F), new Vector3(0.03984F, 0.03984F, 0.03984F));
                AddDisplayRule("CrocoBody", "Finger1.1.R", new Vector3(0.02934F, 0.02567F, 0.03547F), new Vector3(15.38371F, 120.6994F, 0F), new Vector3(0.5717F, 0.5717F, 0.5717F));
                AddDisplayRule("CaptainBody", "Finger1.1.R", new Vector3(0.00759F, 0.04344F, -0.00898F), new Vector3(330.2978F, 0F, 0F), new Vector3(0.04114F, 0.04114F, 0.04114F));
                AddDisplayRule("BrotherBody", "Finger1.1.R", BrotherInfection.green, new Vector3(-0.00005F, 0.0355F, -0.00034F), new Vector3(0F, 97.04736F, 326.1438F), new Vector3(0.02588F, 0.02588F, 0.02588F));
                AddDisplayRule("ScavBody", "Finger11L", new Vector3(-0.02125F, 0.00004F, -0.13491F), new Vector3(0F, 278.9842F, 0F), new Vector3(0.60882F, 0.60882F, 0.60882F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "GunBarrel", new Vector3(0F, -0.00006F, 0.35967F), new Vector3(270F, 0F, 0F), new Vector3(0.05727F, 0.05727F, 0.05727F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysDeputy) AddDisplayRule("DeputyBody", "IndexFingerL", new Vector3(0.00147F, 0.02022F, -0.00053F), new Vector3(357.1624F, 0F, 0F), new Vector3(0.02675F, 0.02675F, 0.02675F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysChirr) AddDisplayRule("ChirrBody", "FootL", new Vector3(0.01638F, 0.13605F, 0.00001F), new Vector3(0F, 111.8158F, 0F), new Vector3(0.05733F, 0.05733F, 0.05733F));
                AddDisplayRule("RailgunnerBody", "Finger1.1.R", new Vector3(0.00121F, 0.00881F, 0.00222F), new Vector3(346.2775F, 29.66737F, 359.7971F), new Vector3(0.02708F, 0.02708F, 0.02708F));
                AddDisplayRule("VoidSurvivorBody", "IndexFinger1L", new Vector3(-0.00893F, 0.04544F, -0.00622F), new Vector3(0F, 55.2822F, 0F), new Vector3(0.04782F, 0.04782F, 0.04782F));
            };

            GenericGameEvents.OnTakeDamage += GenericGameEvents_OnTakeDamage;

            {
                snowRingOrbEffectPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Snow Ring/SnowRingOrbEffect.prefab");
                var effectComponent = snowRingOrbEffectPrefab.AddComponent<EffectComponent>();
                effectComponent.positionAtReferencedTransform = false;
                effectComponent.parentToReferencedTransform = false;
                effectComponent.applyScale = true;
                var vfxAttributes = snowRingOrbEffectPrefab.AddComponent<VFXAttributes>();
                vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
                vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
                var orbEffect = snowRingOrbEffectPrefab.AddComponent<RoR2.Orbs.OrbEffect>();
                orbEffect.startVelocity1 = new Vector3(0f, 5f, 0f);
                orbEffect.startVelocity2 = new Vector3(0f, 20f, 0f);
                orbEffect.endVelocity1 = new Vector3(0f, 5f, 0f);
                orbEffect.endVelocity2 = new Vector3(0f, 20f, 0f);
                orbEffect.movementCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
                orbEffect.faceMovement = true;
                orbEffect.callArrivalIfTargetIsGone = false;
                var destroyOnTimer = snowRingOrbEffectPrefab.transform.Find("Origin/Unparent").gameObject.AddComponent<DestroyOnTimer>();
                destroyOnTimer.duration = 2f;
                destroyOnTimer.enabled = false;
                var onArrivalDefaults = snowRingOrbEffectPrefab.AddComponent<MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsOrbEffectOnArrivalDefaults>();
                onArrivalDefaults.orbEffect = orbEffect;
                onArrivalDefaults.transformsToUnparentChildren = new Transform[] {
                    snowRingOrbEffectPrefab.transform.Find("Origin/Unparent")
                };
                onArrivalDefaults.componentsToEnable = new MonoBehaviour[]
                {
                    destroyOnTimer
                };
                MysticsItemsContent.Resources.effectPrefabs.Add(snowRingOrbEffectPrefab);
            }
        }

        private void GenericGameEvents_OnTakeDamage(DamageReport damageReport)
        {
            if (damageReport.damageDealt > 0 && damageReport.attackerBody && damageReport.attackerBody.mainHurtBox && damageReport.victimBody && damageReport.victimBody.inventory)
            {
                var itemCount = damageReport.victimBody.inventory.GetItemCount(itemDef);
                if (itemCount > 0 && damageReport.victimBody.inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) <= 0 && Util.CheckRoll(chance, damageReport.victimMaster))
                {
                    var search = new BullseyeSearch();
                    search.searchOrigin = damageReport.victimBody.corePosition;
                    search.searchDirection = Vector3.zero;
                    search.teamMaskFilter = TeamMask.allButNeutral;
                    search.teamMaskFilter.RemoveTeam(damageReport.victimTeamIndex);
                    search.filterByLoS = false;
                    search.sortMode = BullseyeSearch.SortMode.Distance;
                    search.maxDistanceFilter = radius + radiusPerStack * (float)(itemCount - 1);
                    search.RefreshCandidates();
                    var results = search.GetResults().Where(x =>
                    {
                        if (!x.healthComponent || !x.healthComponent.body) return false;
                        var setStateOnHurt = x.healthComponent.body.GetComponent<SetStateOnHurt>();
                        return setStateOnHurt && setStateOnHurt.canBeFrozen;
                    }).Select(x => x.healthComponent.body).Distinct().ToList();

                    var totalTargets = targets + targetsPerStack * (itemCount - 1);
                    for (var i = 0; i < totalTargets; i++)
                    {
                        if (results.Count <= 0) break;

                        var targetBody = RoR2Application.rng.NextElementUniform(results);
                        results.Remove(targetBody);

                        var orb = new SnowRingOrb();
                        orb.origin = damageReport.victimBody.corePosition;
                        orb.target = targetBody.mainHurtBox;
                        orb.detonationEffectPrefab = EntityStates.FrozenState.frozenEffectPrefab;
                        orb.travelSpeed = 120f;
                        orb.orbEffectPrefab = snowRingOrbEffectPrefab;
                        orb.freezeDuration = freezeDuration;
                        RoR2.Orbs.OrbManager.instance.AddOrb(orb);
                    }
                }
            }
        }

        public class SnowRingOrb : RoR2.Orbs.Orb
        {
            public override void Begin()
            {
                duration = distanceToTarget / travelSpeed;
                EffectData effectData = new EffectData
                {
                    scale = 1f,
                    origin = origin,
                    genericFloat = duration
                };
                effectData.SetHurtBoxReference(target);
                if (orbEffectPrefab)
                {
                    EffectManager.SpawnEffect(orbEffectPrefab, effectData, true);
                }
            }

            public override void OnArrival()
            {
                base.OnArrival();
                
                if (!target || !target.healthComponent || !target.healthComponent.body) return;

                var targetBody = target.healthComponent.body;

                var setStateOnHurt = targetBody.gameObject.GetComponent<SetStateOnHurt>();
                if (setStateOnHurt && setStateOnHurt.canBeFrozen)
                {
                    setStateOnHurt.SetFrozen(freezeDuration);

                    EffectManager.SpawnEffect(detonationEffectPrefab, new EffectData
                    {
                        origin = targetBody.corePosition,
                        rotation = Quaternion.identity,
                        scale = targetBody.radius
                    }, true);
                }
            }

            public float travelSpeed = 60f;
            public GameObject detonationEffectPrefab;
            public GameObject orbEffectPrefab;
            public float freezeDuration;
        }
    }
}
