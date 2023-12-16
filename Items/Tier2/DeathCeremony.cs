using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using Rewired.ComponentControls.Effects;
using RoR2;
using RoR2.Orbs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static MysticsItems.LegacyBalanceConfigManager;
using System.Linq;

namespace MysticsItems.Items
{
    public class DeathCeremony : BaseItem
    {
        public static ConfigurableValue<float> baseCrit = new ConfigurableValue<float>(
            "Item: Ceremony of Perdition",
            "BaseCrit",
            5f,
            "Crit chance bonus for the first stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_DEATHCEREMONY_DESC"
            }
        );
        public static ConfigurableValue<float> damage = new ConfigurableValue<float>(
            "Item: Ceremony of Perdition",
            "Damage",
            10f,
            "Fractional damage on mark trigger (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_DEATHCEREMONY_DESC"
            }
        );
        public static ConfigurableValue<float> damagePerStack = new ConfigurableValue<float>(
            "Item: Ceremony of Perdition",
            "DamagePerStack",
            10f,
            "Fractional damage on mark trigger for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_DEATHCEREMONY_DESC"
            }
        );
        public static ConfigurableValue<float> duration = new ConfigurableValue<float>(
            "Item: Ceremony of Perdition",
            "Duration",
            10f,
            "How long should the crit mark last (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_DEATHCEREMONY_DESC"
            }
        );
        public static ConfigurableValue<bool> canProc = new ConfigurableValue<bool>(
            "Item: Ceremony of Perdition",
            "CanProc",
            false,
            "Hits shared to other enemies can proc item effects"
        );

        public static GameObject damageShareOrbEffect;
        
        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            NetworkingAPI.RegisterMessageType<MysticsItemsDeathCeremonyMark.SyncMarked>();
        }

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_DeathCeremony";
            SetItemTierWhenAvailable(ItemTier.Tier2);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage
            };

            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Death Ceremony/Model.prefab"));
            HopooShaderToMaterial.Standard.Apply(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial);
            HopooShaderToMaterial.Standard.Emission(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial, 1f);
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Death Ceremony/Icon.png");
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "CalfL", new Vector3(0.07748F, 0.19394F, -0.03508F), new Vector3(351.2407F, 38.31051F, 271.2653F), new Vector3(0.08418F, 0.08418F, 0.08418F));
                AddDisplayRule("HuntressBody", "CalfL", new Vector3(0.01081F, 0.33039F, -0.04676F), new Vector3(271.8222F, -0.00021F, 329.7724F), new Vector3(0.07837F, 0.07837F, 0.07837F));
                AddDisplayRule("Bandit2Body", "CalfL", new Vector3(0.03402F, 0.33989F, -0.05447F), new Vector3(275.6507F, 198.2068F, 136.1487F), new Vector3(0.07333F, 0.07333F, 0.07333F));
                AddDisplayRule("ToolbotBody", "CalfL", new Vector3(-0.76507F, 2.13588F, -0.16809F), new Vector3(0F, 0F, 92.59356F), new Vector3(0.6665F, 0.6665F, 0.6665F));
                AddDisplayRule("EngiBody", "CalfL", new Vector3(0.06642F, 0.07521F, -0.07491F), new Vector3(2.18099F, 54.26896F, 272.251F), new Vector3(0.07373F, 0.07373F, 0.07373F));
                AddDisplayRule("EngiTurretBody", "LegBar2", new Vector3(-0.00001F, 0.25339F, 0.2332F), new Vector3(87.51187F, 180F, 180F), new Vector3(0.31299F, 0.31299F, 0.31299F));
                AddDisplayRule("EngiWalkerTurretBody", "LegBar2", new Vector3(-0.00001F, 0.32614F, 0.3331F), new Vector3(80.43612F, 180F, 180F), new Vector3(0.35353F, 0.35353F, 0.35353F));
                AddDisplayRule("MageBody", "CalfL", new Vector3(-0.05753F, 0.055F, -0.00398F), new Vector3(283.2863F, 250.2234F, 182.1714F), new Vector3(0.08759F, 0.08759F, 0.08759F));
                AddDisplayRule("MercBody", "CalfL", new Vector3(0F, 0.06378F, -0.0637F), new Vector3(275.3986F, 0F, 0F), new Vector3(0.0835F, 0.0835F, 0.0835F));
                AddDisplayRule("TreebotBody", "FootFrontL", new Vector3(-0.00003F, 0.2441F, -0.14866F), new Vector3(270.8983F, 0F, 0F), new Vector3(0.20205F, 0.20205F, 0.20205F));
                AddDisplayRule("LoaderBody", "CalfL", new Vector3(0.10101F, 0.10896F, -0.05009F), new Vector3(286.7617F, 305.9382F, 3.19435F), new Vector3(0.10456F, 0.10456F, 0.10456F));
                AddDisplayRule("CrocoBody", "CalfL", new Vector3(-0.72054F, 1.36348F, -0.3729F), new Vector3(355.4709F, 323.8942F, 83.82096F), new Vector3(0.78694F, 0.78694F, 0.78694F));
                AddDisplayRule("CaptainBody", "CalfL", new Vector3(0F, 0.05716F, -0.09369F), new Vector3(287.3015F, 0F, 0F), new Vector3(0.10502F, 0.10502F, 0.10502F));
                AddDisplayRule("BrotherBody", "CalfL", BrotherInfection.green, new Vector3(0.00384F, 0.00536F, -0.03235F), new Vector3(14.82572F, 260.7474F, 350.3363F), new Vector3(0.04861F, 0.10534F, 0.10724F));
                AddDisplayRule("ScavBody", "CalfL", new Vector3(0.33272F, -0.17368F, -1.09799F), new Vector3(277.8127F, 180F, 165.8016F), new Vector3(1.08614F, 1.08614F, 1.08614F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "CalfL", new Vector3(-0.04687F, 0.03461F, 0.08856F), new Vector3(80.03226F, 0F, 32.95736F), new Vector3(0.07694F, 0.07694F, 0.07694F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysDeputy) AddDisplayRule("DeputyBody", "Pelvis", new Vector3(0.14346F, -0.0212F, -0.0735F), new Vector3(344.0173F, 180F, 163.1369F), new Vector3(0.07305F, 0.07305F, 0.07305F));
                AddDisplayRule("RailgunnerBody", "CalfL", new Vector3(0.07292F, 0.00507F, 0.09758F), new Vector3(65.51882F, 247.9167F, 209.6704F), new Vector3(0.10736F, 0.10736F, 0.10736F));
                AddDisplayRule("VoidSurvivorBody", "ThighL", new Vector3(0.08283F, 0.31418F, 0.06681F), new Vector3(11.83692F, 314.669F, 281.4625F), new Vector3(0.09226F, 0.09226F, 0.09226F));
            };

            GameObject debuffedVFX = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Death Ceremony/MarkedVFX.prefab");
            GameObject vfxOrigin = debuffedVFX.transform.Find("Origin").gameObject;
            vfxOrigin.transform.localScale *= 3f;
            CustomTempVFXManagement.MysticsRisky2UtilsTempVFX tempVFX = debuffedVFX.AddComponent<CustomTempVFXManagement.MysticsRisky2UtilsTempVFX>();
            RotateAroundAxis rotateAroundAxis = vfxOrigin.gameObject.AddComponent<RotateAroundAxis>();
            rotateAroundAxis.relativeTo = Space.Self;
            rotateAroundAxis.rotateAroundAxis = RotateAroundAxis.RotationAxis.X;
            rotateAroundAxis.fastRotationSpeed = 17f;
            rotateAroundAxis.speed = RotateAroundAxis.Speed.Fast;
            rotateAroundAxis = vfxOrigin.gameObject.AddComponent<RotateAroundAxis>();
            rotateAroundAxis.relativeTo = Space.Self;
            rotateAroundAxis.rotateAroundAxis = RotateAroundAxis.RotationAxis.Z;
            rotateAroundAxis.fastRotationSpeed = 34f;
            rotateAroundAxis.speed = RotateAroundAxis.Speed.Fast;
            ObjectScaleCurve fadeOut = vfxOrigin.AddComponent<ObjectScaleCurve>();
            fadeOut.overallCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 1f, Mathf.Tan(180f * Mathf.Deg2Rad), Mathf.Tan(-20f * Mathf.Deg2Rad)),
                    new Keyframe(1f, 0f, Mathf.Tan(160f * Mathf.Deg2Rad), 0f)
                }
            };
            fadeOut.useOverallCurveOnly = true;
            fadeOut.enabled = false;
            fadeOut.timeMax = 0.2f;
            tempVFX.exitBehaviours = new MonoBehaviour[]
            {
                fadeOut
            };
            ObjectScaleCurve fadeIn = vfxOrigin.AddComponent<ObjectScaleCurve>();
            fadeIn.overallCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 0f, Mathf.Tan(180f * Mathf.Deg2Rad), Mathf.Tan(70f * Mathf.Deg2Rad)),
                    new Keyframe(1f, 1f, Mathf.Tan(-160f * Mathf.Deg2Rad), 0f)
                }
            };
            fadeIn.useOverallCurveOnly = true;
            fadeIn.enabled = false;
            fadeIn.timeMax = 0.2f;
            tempVFX.enterBehaviours = new MonoBehaviour[]
            {
                fadeIn
            };
            CustomTempVFXManagement.allVFX.Add(new CustomTempVFXManagement.VFXInfo
            {
                prefab = debuffedVFX,
                condition = (x) =>
                {
                    var component = x.GetComponent<MysticsItemsDeathCeremonyMark>();
                    if (component) return component.markTimer > 0f;
                    return false;
                },
                radius = CustomTempVFXManagement.DefaultRadiusCall
            });

            damageShareOrbEffect = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Death Ceremony/DeathCeremonyOrbEffect.prefab");
            EffectComponent effectComponent = damageShareOrbEffect.AddComponent<EffectComponent>();
            effectComponent.positionAtReferencedTransform = false;
            effectComponent.parentToReferencedTransform = false;
            effectComponent.applyScale = true;
            VFXAttributes vfxAttributes = damageShareOrbEffect.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Low;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            OrbEffect orbEffect = damageShareOrbEffect.AddComponent<OrbEffect>();
            orbEffect.startVelocity1 = new Vector3(-10f, 0f, -10f);
            orbEffect.startVelocity2 = new Vector3(-10f, 10f, 10f);
            orbEffect.endVelocity1 = new Vector3(-10f, 0f, -10f);
            orbEffect.endVelocity2 = new Vector3(10f, 10f, 10f);
            orbEffect.movementCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 0f),
                    new Keyframe(1f, 1f)
                },
                preWrapMode = WrapMode.Clamp,
                postWrapMode = WrapMode.Clamp
            };
            orbEffect.faceMovement = true;
            orbEffect.callArrivalIfTargetIsGone = false;
            DestroyOnTimer destroyOnTimer = damageShareOrbEffect.transform.Find("Trail").gameObject.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 0.5f;
            destroyOnTimer.enabled = false;
            MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsOrbEffectOnArrivalDefaults onArrivalDefaults = damageShareOrbEffect.AddComponent<MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsOrbEffectOnArrivalDefaults>();
            onArrivalDefaults.orbEffect = orbEffect;
            onArrivalDefaults.transformsToUnparentChildren = new Transform[] {
                damageShareOrbEffect.transform
            };
            onArrivalDefaults.componentsToEnable = new MonoBehaviour[]
            {
                destroyOnTimer
            };
            MysticsItemsContent.Resources.effectPrefabs.Add(damageShareOrbEffect);

            GenericGameEvents.OnHitEnemy += GenericGameEvents_OnHitEnemy;

            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;

            Run.onRunStartGlobal += (x) =>
            {
                MysticsItemsDeathCeremonyMark.ownerToComponentDict.Clear();
            };
            TeamComponent.onJoinTeamGlobal += TeamComponent_onJoinTeamGlobal;
            TeamComponent.onLeaveTeamGlobal += TeamComponent_onLeaveTeamGlobal;
        }

        private void TeamComponent_onJoinTeamGlobal(TeamComponent teamComponent, TeamIndex newTeamIndex)
        {
            if (MysticsItemsDeathCeremonyMark.ExistsForBody(teamComponent.body))
            {
                if (!MysticsItemsDeathCeremonyMark.marksPerTeam.ContainsKey(newTeamIndex))
                    MysticsItemsDeathCeremonyMark.marksPerTeam[newTeamIndex] = new List<MysticsItemsDeathCeremonyMark>();
                MysticsItemsDeathCeremonyMark.marksPerTeam[newTeamIndex].Add(MysticsItemsDeathCeremonyMark.GetForBody(teamComponent.body));
            }
        }

        private void TeamComponent_onLeaveTeamGlobal(TeamComponent teamComponent, TeamIndex newTeamIndex)
        {
            if (MysticsItemsDeathCeremonyMark.ExistsForBody(teamComponent.body) && MysticsItemsDeathCeremonyMark.marksPerTeam.ContainsKey(newTeamIndex))
            {
                MysticsItemsDeathCeremonyMark.marksPerTeam[newTeamIndex].Remove(MysticsItemsDeathCeremonyMark.GetForBody(teamComponent.body));
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var inventory = sender.inventory;
            if (inventory)
            {
                var itemCount = inventory.GetItemCount(itemDef);
                if (itemCount > 0)
                {
                    args.critAdd += baseCrit * (ConfigManager.General.initialCritStacking ? itemCount : 1f);
                }
            }
        }

        private void GenericGameEvents_OnHitEnemy(DamageInfo damageInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo attackerInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo victimInfo)
        {
            if (!damageInfo.procChainMask.HasProc(ProcType.ChainLightning) && !damageInfo.rejected && attackerInfo.body && attackerInfo.inventory && victimInfo.body)
            {
                var itemCount = attackerInfo.inventory.GetItemCount(itemDef);
                if (itemCount > 0 && damageInfo.procCoefficient > 0f && damageInfo.crit)
                {
                    var mark = MysticsItemsDeathCeremonyMark.GetForBody(victimInfo.body);
                    mark.markTimer = duration;
                }
                if (MysticsItemsDeathCeremonyMark.marksPerTeam.ContainsKey(victimInfo.teamIndex))
                {
                    foreach (var body in MysticsItemsDeathCeremonyMark.marksPerTeam[victimInfo.teamIndex].Select(x => x.body))
                    {
                        if (body != victimInfo.body && body.healthComponent && body.healthComponent.alive)
                        {
                            EffectData effectData = new EffectData
                            {
                                origin = victimInfo.body.corePosition,
                                genericFloat = 0.2f
                            };
                            effectData.SetHurtBoxReference(body.gameObject);
                            EffectManager.SpawnEffect(damageShareOrbEffect, effectData, true);

                            DamageInfo markDamageInfo = new DamageInfo();
                            markDamageInfo.damage = damageInfo.damage * Util.ConvertAmplificationPercentageIntoReductionPercentage(damage + damagePerStack * (itemCount - 1)) / 100f;
                            markDamageInfo.attacker = attackerInfo.gameObject;
                            markDamageInfo.procCoefficient = canProc ? damageInfo.procCoefficient : 0f;
                            markDamageInfo.position = body.corePosition;
                            markDamageInfo.crit = damageInfo.crit;
                            markDamageInfo.damageType = DamageType.Generic;
                            markDamageInfo.procChainMask = damageInfo.procChainMask;
                            markDamageInfo.procChainMask.AddProc(ProcType.ChainLightning);
                            markDamageInfo.damageColorIndex = damageInfo.damageColorIndex;
                            body.healthComponent.TakeDamage(markDamageInfo);
                            GlobalEventManager.instance.OnHitEnemy(markDamageInfo, body.healthComponent.gameObject);
                        }
                    }
                }
            }
        }

        public class MysticsItemsDeathCeremonyMark : MonoBehaviour
        {
            public static Dictionary<CharacterBody, MysticsItemsDeathCeremonyMark> ownerToComponentDict = new Dictionary<CharacterBody, MysticsItemsDeathCeremonyMark>();

            public static MysticsItemsDeathCeremonyMark GetForBody(CharacterBody owner)
            {
                if (!ownerToComponentDict.ContainsKey(owner))
                    ownerToComponentDict[owner] = owner.gameObject.AddComponent<MysticsItemsDeathCeremonyMark>();
                return ownerToComponentDict[owner];
            }
            public static bool ExistsForBody(CharacterBody owner)
            {
                return ownerToComponentDict.ContainsKey(owner);
            }

            private float _markTimer = 0f;
            public float markTimer
            {
                get { return _markTimer; }
                set
                {
                    if (value > _markTimer && NetworkServer.active)
                        new SyncMarked(gameObject.GetComponent<NetworkIdentity>().netId, value).Send(NetworkDestination.Clients);
                    _markTimer = value;
                }
            }

            public class SyncMarked : INetMessage
            {
                NetworkInstanceId objID;
                float markTimer;

                public SyncMarked()
                {
                }

                public SyncMarked(NetworkInstanceId objID, float markTimer)
                {
                    this.objID = objID;
                    this.markTimer = markTimer;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                    markTimer = reader.ReadSingle();
                }

                public void OnReceived()
                {
                    if (NetworkServer.active) return;
                    GameObject obj = Util.FindNetworkObject(objID);
                    if (obj)
                    {
                        MysticsItemsDeathCeremonyMark controller = obj.GetComponent<MysticsItemsDeathCeremonyMark>();
                        if (controller)
                        {
                            controller.markTimer = markTimer;
                        }
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                    writer.Write(markTimer);
                }
            }

            public CharacterBody body;
            public TeamComponent teamComponent;

            public void Awake()
            {
                body = GetComponent<CharacterBody>();
                teamComponent = GetComponent<TeamComponent>();

                if (!marksPerTeam.ContainsKey(teamComponent.teamIndex))
                    marksPerTeam[teamComponent.teamIndex] = new List<MysticsItemsDeathCeremonyMark>();
                marksPerTeam[teamComponent.teamIndex].Add(this);
            }

            public void FixedUpdate()
            {
                markTimer -= Time.fixedDeltaTime;
            }

            public void OnDestroy()
            {
                var teamIndex = teamComponent.teamIndex;
                if (marksPerTeam.ContainsKey(teamIndex))
                    marksPerTeam[teamIndex].Remove(this);
            }

            public static Dictionary<TeamIndex, List<MysticsItemsDeathCeremonyMark>> marksPerTeam = new Dictionary<TeamIndex, List<MysticsItemsDeathCeremonyMark>>();
        }
    }
}
