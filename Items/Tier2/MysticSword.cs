using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API.Networking.Interfaces;
using R2API.Networking;
using static MysticsItems.BalanceConfigManager;
using RoR2.Orbs;

namespace MysticsItems.Items
{
    public class MysticSword : BaseItem
    {
        public static ConfigurableValue<float> healthThreshold = new ConfigurableValue<float>(
            "Item: Mystic Sword",
            "HealthThreshold",
            1900f,
            "How many HP should the killed enemy have to trigger this item's effect",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_MYSTICSWORD_DESC"
            }
        );
        public static ConfigurableValue<float> damage = new ConfigurableValue<float>(
            "Item: Mystic Sword",
            "Damage",
            3f,
            "Damage bonus for each strong enemy killed (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_MYSTICSWORD_DESC"
            }
        );
        public static ConfigurableValue<float> damagePerStack = new ConfigurableValue<float>(
            "Item: Mystic Sword",
            "DamagePerStack",
            3f,
            "Damage bonus for each strong enemy killed for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_MYSTICSWORD_DESC"
            }
        );

        private static string tooltipString = "<color=#BE2BE1></color><color=#EAEEDD></color><color=#AAAAA5></color>";

        public static DamageColorIndex damageColorIndex = DamageColorAPI.RegisterDamageColor(new Color32(247, 245, 197, 255));
        public static GameObject onKillOrbEffect;
        public static GameObject onKillVFX;
        public static NetworkSoundEventDef onKillSFX;

        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            NetworkingAPI.RegisterMessageType<MysticsItemsMysticSwordBehaviour.SyncDamageBonus>();
        }

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_MysticSword";
            itemDef.tier = ItemTier.Tier2;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage,
                ItemTag.OnKillEffect,
                ItemTag.AIBlacklist
            };
            
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Mystic Sword/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Mystic Sword/Icon.png");
            var mat = itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial;
            HopooShaderToMaterial.Standard.Apply(mat);
            HopooShaderToMaterial.Standard.Emission(mat, 1f, new Color32(0, 250, 255, 255));
            itemDef.pickupModelPrefab.transform.Find("GameObject").localScale *= 0.1f;

            var swordFollowerPrefab = PrefabAPI.InstantiateClone(PrepareItemDisplayModel(PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Mystic Sword/DisplayModel.prefab"))), "MysticsItems_MysticSwordItemFollowerPrefab", false);
            swordFollowerPrefab.transform.Find("TranslatePivot").transform.localScale *= 0.02f;
            ObjectTransformCurve objectTransformCurve = swordFollowerPrefab.transform.Find("TranslatePivot").gameObject.AddComponent<ObjectTransformCurve>();
            objectTransformCurve.translationCurveX = AnimationCurve.Constant(0f, 1f, 0f);
            var floatY = 0.1f;
            objectTransformCurve.translationCurveY = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0.25f, floatY),
                    new Keyframe(0.75f, -floatY)
                },
                preWrapMode = WrapMode.PingPong,
                postWrapMode = WrapMode.PingPong
            };
            objectTransformCurve.translationCurveZ = AnimationCurve.Constant(0f, 1f, 0f);
            objectTransformCurve.useTranslationCurves = true;
            objectTransformCurve.timeMax = 10f;
            objectTransformCurve.rotationCurveX = AnimationCurve.Constant(0f, 1f, 0f);
            objectTransformCurve.rotationCurveY = AnimationCurve.Linear(0f, 0f, 1f, 360f);
            objectTransformCurve.rotationCurveY.preWrapMode = WrapMode.Loop;
            objectTransformCurve.rotationCurveY.postWrapMode = WrapMode.Loop;
            objectTransformCurve.rotationCurveZ = AnimationCurve.Constant(0f, 1f, 0f);
            objectTransformCurve.useRotationCurves = true;
            objectTransformCurve.gameObject.AddComponent<MysticSwordAnimationReset>();

            itemDisplayPrefab = PrefabAPI.InstantiateClone(new GameObject("MysticsItems_MysticSwordFollower"), "MysticsItems_MysticSwordFollower", false);
            itemDisplayPrefab.AddComponent<ItemDisplay>();
            ItemFollower itemFollower = itemDisplayPrefab.AddComponent<ItemFollower>();
            itemFollower.followerPrefab = swordFollowerPrefab;
            itemFollower.distanceDampTime = 0.1f;
            itemFollower.distanceMaxSpeed = 20f;
            itemFollower.targetObject = itemDisplayPrefab;

            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Base", new Vector3(0.17794F, -0.28733F, -0.73752F), new Vector3(3.15473F, 89.99998F, 270.0002F), Vector3.one);
                AddDisplayRule("HuntressBody", "Base", new Vector3(0.17816F, -0.23663F, -0.52846F), new Vector3(2.42504F, 269.9999F, 90.0001F), Vector3.one);
                AddDisplayRule("Bandit2Body", "Base", new Vector3(0.4537F, 0.29041F, -0.57258F), new Vector3(270F, 0F, 0F), Vector3.one);
                AddDisplayRule("ToolbotBody", "Base", new Vector3(-1.04879F, -4.19278F, 5.42458F), new Vector3(0F, 90F, 90F), Vector3.one);
                AddDisplayRule("EngiBody", "Base", new Vector3(0.0113F, -0.52335F, -0.69199F), new Vector3(270F, 0F, 0F), Vector3.one);
                AddDisplayRule("EngiTurretBody", "Base", new Vector3(1.03266F, 3.98892F, -2.18302F), new Vector3(0F, 90F, 0F), Vector3.one);
                AddDisplayRule("EngiWalkerTurretBody", "Base", new Vector3(1.53037F, 3.79942F, -2.10391F), new Vector3(0F, 90F, 0F), Vector3.one);
                AddDisplayRule("MageBody", "Base", new Vector3(0.38669F, -0.43447F, -0.48611F), new Vector3(270F, 0F, 0F), Vector3.one);
                AddDisplayRule("MercBody", "Base", new Vector3(0.38005F, -0.35752F, -0.53391F), new Vector3(270F, 0F, 0F), Vector3.one);
                AddDisplayRule("TreebotBody", "Base", new Vector3(0.69145F, -1.39195F, -1.94014F), new Vector3(270F, 0F, 0F), Vector3.one * 1f);
                AddDisplayRule("LoaderBody", "Base", new Vector3(0.26563F, -0.57799F, -0.60309F), new Vector3(270F, 0F, 0F), Vector3.one);
                AddDisplayRule("CrocoBody", "Base", new Vector3(2.43278F, 4.85691F, 4.92643F), new Vector3(90F, 0F, 0F), Vector3.one * 1f);
                AddDisplayRule("CaptainBody", "Base", new Vector3(0.52281F, -0.26508F, -0.8575F), new Vector3(270F, 0F, 0F), Vector3.one);
                AddDisplayRule("BrotherBody", "HandR", BrotherInfection.green, new Vector3(-0.00915F, 0.08592F, 0.02786F), new Vector3(77.05167F, 128.9087F, 289.6218F), new Vector3(0.06672F, 0.02927F, 0.06676F));
                AddDisplayRule("ScavBody", "Base", new Vector3(4.53188F, 14.35975F, 10.88982F), new Vector3(90F, 0F, 0F), Vector3.one * 2f);
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Chest", new Vector3(0.20249F, 0.6232F, -0.59153F), new Vector3(10.681F, 0.007F, 0.071F), new Vector3(1F, 1F, 1F));
            };

            {
                onKillVFX = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Mystic Sword/SwordPowerUpKillEffect.prefab");
                EffectComponent effectComponent = onKillVFX.AddComponent<EffectComponent>();
                effectComponent.applyScale = true;
                VFXAttributes vfxAttributes = onKillVFX.AddComponent<VFXAttributes>();
                vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
                vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
                onKillVFX.AddComponent<DestroyOnTimer>().duration = 1f;
                MysticsItemsContent.Resources.effectPrefabs.Add(onKillVFX);
            }

            {
                onKillOrbEffect = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Mystic Sword/SwordPowerUpOrbEffect.prefab");
                EffectComponent effectComponent = onKillOrbEffect.AddComponent<EffectComponent>();
                effectComponent.positionAtReferencedTransform = false;
                effectComponent.parentToReferencedTransform = false;
                effectComponent.applyScale = true;
                VFXAttributes vfxAttributes = onKillOrbEffect.AddComponent<VFXAttributes>();
                vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
                vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
                OrbEffect orbEffect = onKillOrbEffect.AddComponent<OrbEffect>();
                orbEffect.startVelocity1 = new Vector3(-25f, 5f, -25f);
                orbEffect.startVelocity2 = new Vector3(25f, 50f, 25f);
                orbEffect.endVelocity1 = new Vector3(0f, 0f, 0f);
                orbEffect.endVelocity2 = new Vector3(0f, 0f, 0f);
                var curveHolder = onKillVFX.transform.Find("Origin/Particle System").GetComponent<ParticleSystem>().sizeOverLifetime;
                orbEffect.movementCurve = curveHolder.size.curve;
                orbEffect.faceMovement = true;
                orbEffect.callArrivalIfTargetIsGone = false;
                DestroyOnTimer destroyOnTimer = onKillOrbEffect.transform.Find("Origin/Unparent").gameObject.AddComponent<DestroyOnTimer>();
                destroyOnTimer.duration = 0.5f;
                destroyOnTimer.enabled = false;
                MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsOrbEffectOnArrivalDefaults onArrivalDefaults = onKillOrbEffect.AddComponent<MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsOrbEffectOnArrivalDefaults>();
                onArrivalDefaults.orbEffect = orbEffect;
                onArrivalDefaults.transformsToUnparentChildren = new Transform[] {
                    onKillOrbEffect.transform.Find("Origin/Unparent")
                };
                onArrivalDefaults.componentsToEnable = new MonoBehaviour[]
                {
                    destroyOnTimer
                };
                MysticsItemsContent.Resources.effectPrefabs.Add(onKillOrbEffect);
            }

            onKillSFX = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            onKillSFX.eventName = "MysticsItems_Play_item_proc_MysticSword";
            MysticsItemsContent.Resources.networkSoundEventDefs.Add(onKillSFX);

            CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;

            if (!SoftDependencies.SoftDependenciesCore.itemStatsCompatEnabled) On.RoR2.UI.ItemIcon.SetItemIndex += ItemIcon_SetItemIndex;

            GenericGameEvents.BeforeTakeDamage += GenericGameEvents_BeforeTakeDamage;
        }

        private class MysticSwordAnimationReset : MonoBehaviour
        {
            public ObjectTransformCurve objectTransformCurve;

            public void Awake()
            {
                objectTransformCurve = GetComponent<ObjectTransformCurve>();
            }

            public void LateUpdate()
            {
                if (objectTransformCurve.time >= objectTransformCurve.timeMax)
                {
                    objectTransformCurve.time -= objectTransformCurve.timeMax;
                }
            }
        }

        private void CharacterMaster_onStartGlobal(CharacterMaster obj)
        {
            if (obj.inventory) obj.inventory.gameObject.AddComponent<MysticsItemsMysticSwordBehaviour>();
        }

        private void GenericGameEvents_BeforeTakeDamage(DamageInfo damageInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo attackerInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo victimInfo)
        {
            if (attackerInfo.inventory && attackerInfo.inventory.GetItemCount(itemDef) > 0)
            {
                if (damageInfo.damageColorIndex == DamageColorIndex.Default)
                    damageInfo.damageColorIndex = damageColorIndex;
            }
        }

        private void ItemIcon_SetItemIndex(On.RoR2.UI.ItemIcon.orig_SetItemIndex orig, RoR2.UI.ItemIcon self, ItemIndex newItemIndex, int newItemCount)
        {
            orig(self, newItemIndex, newItemCount);

            if (newItemIndex == itemDef.itemIndex)
            {
                Transform parent = self.transform.parent;
                if (parent)
                {
                    RoR2.UI.ItemInventoryDisplay itemInventoryDisplay = parent.GetComponent<RoR2.UI.ItemInventoryDisplay>();
                    if (itemInventoryDisplay && itemInventoryDisplay.inventory)
                    {
                        MysticsItemsMysticSwordBehaviour swordBehaviour = itemInventoryDisplay.inventory.GetComponent<MysticsItemsMysticSwordBehaviour>();
                        if (swordBehaviour)
                        {
                            globalStringBuilder.Clear();
                            globalStringBuilder.Append(Language.GetString(self.tooltipProvider.bodyToken) + "\r\n");
                            globalStringBuilder.Append("\r\n");
                            globalStringBuilder.Append(Language.GetString("MYSTICSITEMS_STATCHANGE_LIST_HEADER"));
                            globalStringBuilder.Append("\r\n");
                            globalStringBuilder.Append(
                                Language.GetStringFormatted(
                                    "MYSTICSITEMS_STATCHANGE_LIST_DAMAGE",
                                    "+" + (Mathf.RoundToInt(swordBehaviour.damageBonus * 100f)).ToString(System.Globalization.CultureInfo.InvariantCulture)
                                )
                            );
                            globalStringBuilder.Append(tooltipString);
                            self.tooltipProvider.overrideBodyText = globalStringBuilder.ToString();
                            globalStringBuilder.Clear();
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(self.tooltipProvider.overrideBodyText) && self.tooltipProvider.overrideBodyText.Contains(tooltipString))
                    self.tooltipProvider.overrideBodyText = "";
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var itemCount = sender.inventory.GetItemCount(itemDef);
                if (itemCount > 0)
                {
                    var component = sender.inventory.GetComponent<MysticsItemsMysticSwordBehaviour>();
                    if (component) args.damageMultAdd += component.damageBonus;
                }
            }
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (!NetworkServer.active) return;

            if (damageReport.victimBody)
            {
                var healthMultiplier = 1f;
                if (damageReport.victimBody.inventory)
                    healthMultiplier += damageReport.victimBody.inventory.GetItemCount(RoR2Content.Items.BoostHp) * 0.1f;
                if ((damageReport.victimBody.baseMaxHealth * healthMultiplier) >= healthThreshold)
                {
                    var onKillOrbTargets = new List<GameObject>();

                    foreach (var teamMember in TeamComponent.GetTeamMembers(damageReport.attackerTeamIndex))
                    {
                        var teamMemberBody = teamMember.body;
                        if (teamMemberBody)
                        {
                            var inventory = teamMemberBody.inventory;
                            if (inventory)
                            {
                                int itemCount = inventory.GetItemCount(itemDef);
                                if (itemCount > 0)
                                {
                                    var component = inventory.GetComponent<MysticsItemsMysticSwordBehaviour>();
                                    if (component)
                                    {
                                        component.damageBonus += damage / 100f + damagePerStack / 100f * (float)(itemCount - 1);
                                        onKillOrbTargets.Add(teamMemberBody.gameObject);
                                        //RoR2.Audio.EntitySoundManager.EmitSoundServer(onKillSFX.index, teamMember.body.gameObject);
                                    }
                                }
                            }
                        }
                    }

                    if (onKillOrbTargets.Count > 0)
                    {
                        for (var i = 0; i < 5; i++)
                        {
                            EffectData effectData = new EffectData
                            {
                                origin = damageReport.victimBody.corePosition,
                                genericFloat = UnityEngine.Random.Range(1.35f, 1.7f),
                                scale = UnityEngine.Random.Range(0.02f, 0.2f)
                            };
                            effectData.SetHurtBoxReference(RoR2Application.rng.NextElementUniform(onKillOrbTargets));
                            EffectManager.SpawnEffect(onKillOrbEffect, effectData, true);
                        }

                        EffectManager.SpawnEffect(onKillVFX, new EffectData
                        {
                            origin = damageReport.victimBody.corePosition,
                            scale = damageReport.victimBody.radius
                        }, true);
                        RoR2.Audio.PointSoundManager.EmitSoundServer(onKillSFX.index, damageReport.victimBody.corePosition);
                    }
                }
            }
        }

        public class MysticsItemsMysticSwordBehaviour : MonoBehaviour
        {
            private float _damageBonus;
            public float damageBonus
            {
                get { return _damageBonus; }
                set
                {
                    _damageBonus = value;
                    if (NetworkServer.active)
                        new SyncDamageBonus(gameObject.GetComponent<NetworkIdentity>().netId, value).Send(NetworkDestination.Clients);
                }
            }

            public class SyncDamageBonus : INetMessage
            {
                NetworkInstanceId objID;
                float damageBonus;

                public SyncDamageBonus()
                {
                }

                public SyncDamageBonus(NetworkInstanceId objID, float damageBonus)
                {
                    this.objID = objID;
                    this.damageBonus = damageBonus;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                    damageBonus = reader.ReadSingle();
                }

                public void OnReceived()
                {
                    if (NetworkServer.active) return;
                    GameObject obj = Util.FindNetworkObject(objID);
                    if (obj)
                    {
                        MysticsItemsMysticSwordBehaviour component = obj.GetComponent<MysticsItemsMysticSwordBehaviour>();
                        if (component) component.damageBonus = damageBonus;
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                    writer.Write(damageBonus);
                }
            }
        }
    }
}
