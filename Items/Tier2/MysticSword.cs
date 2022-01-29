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
            1000f,
            "How many HP should the killed enemy have to trigger this item's effect",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_MYSTICSWORD_DESC"
            }
        );
        public static ConfigurableValue<float> damage = new ConfigurableValue<float>(
            "Item: Mystic Sword",
            "Damage",
            5f,
            "Damage bonus for each strong enemy killed (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_MYSTICSWORD_DESC"
            }
        );
        public static ConfigurableValue<float> damagePerStack = new ConfigurableValue<float>(
            "Item: Mystic Sword",
            "DamagePerStack",
            5f,
            "Damage bonus for each strong enemy killed for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_MYSTICSWORD_DESC"
            }
        );

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
            /*
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Hexahedral Monolith/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Hexahedral Monolith/Icon.png");
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "GunMeshR", new Vector3(-0.13951F, -0.01505F, 0.13151F), new Vector3(0F, 0F, 90F), new Vector3(0.00856F, 0.00856F, 0.00856F));
                AddDisplayRule("HuntressBody", "UpperArmL", new Vector3(0.06909F, 0.10681F, -0.00977F), new Vector3(3.66903F, 357.0302F, 178.0301F), new Vector3(0.01358F, 0.01358F, 0.01358F));
                AddDisplayRule("Bandit2Body", "MainWeapon", new Vector3(-0.05477F, 0.2274F, -0.04443F), new Vector3(359.4865F, 89.48757F, 206.7464F), new Vector3(0.0135F, 0.00485F, 0.00485F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(-1.77361F, 2.53066F, 1.76556F), new Vector3(0F, 90F, 90F), new Vector3(0.10065F, 0.10065F, 0.10065F));
                AddDisplayRule("EngiBody", "LowerArmR", new Vector3(0.0113F, 0.13437F, -0.05836F), new Vector3(1.34564F, 72.93568F, 188.458F), new Vector3(0.01476F, 0.01476F, 0.01476F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.035F, 0.89075F, -1.47928F), new Vector3(0F, 90F, 303.695F), new Vector3(0.07847F, 0.07847F, 0.07847F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.03562F, 1.40676F, -1.39837F), new Vector3(0F, 90F, 303.1705F), new Vector3(0.08093F, 0.09844F, 0.07912F));
                AddDisplayRule("MageBody", "Chest", new Vector3(-0.10398F, 0.07562F, -0.31389F), new Vector3(359.7522F, 90.11677F, 8.18118F), new Vector3(0.01236F, 0.01035F, 0.00964F));
                AddDisplayRule("MageBody", "Chest", new Vector3(0.11942F, 0.07423F, -0.30928F), new Vector3(359.136F, 95.88205F, 8.14244F), new Vector3(0.01236F, 0.01035F, 0.00787F));
                AddDisplayRule("MercBody", "HandL", new Vector3(0.01326F, 0.1146F, 0.04565F), new Vector3(88.10731F, 183.3846F, 89.99922F), new Vector3(0.00961F, 0.00961F, 0.00965F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(0.69564F, -0.5422F, -0.29426F), new Vector3(46.13942F, 241.7613F, 12.79626F), new Vector3(0.03647F, 0.03647F, 0.03647F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(0.01517F, -0.06288F, -0.17121F), new Vector3(90F, 90F, 0F), new Vector3(0.0207F, 0.0207F, 0.0207F));
                AddDisplayRule("CrocoBody", "SpineChest1", new Vector3(1.39693F, -0.10569F, -0.18201F), new Vector3(55.10429F, 175.6143F, 292.3791F), new Vector3(0.1379F, 0.1379F, 0.1379F));
                AddDisplayRule("CaptainBody", "MuzzleGun", new Vector3(0.00467F, 0.05642F, -0.1194F), new Vector3(357.9892F, 90.52832F, 89.76476F), new Vector3(0.05388F, 0.01322F, 0.0146F));
                AddDisplayRule("BrotherBody", "UpperArmL", BrotherInfection.green, new Vector3(0.06646F, 0.22781F, -0.00154F), new Vector3(77.05167F, 128.9087F, 289.6219F), new Vector3(0.04861F, 0.10534F, 0.10724F));
                AddDisplayRule("ScavBody", "Stomach", new Vector3(-0.92389F, 11.6509F, -5.90638F), new Vector3(20.93637F, 118.4181F, 332.9505F), new Vector3(0.24839F, 0.25523F, 0.24839F));
            };
            */

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

        private void CharacterMaster_onStartGlobal(CharacterMaster obj)
        {
            if (obj.inventory) obj.inventory.gameObject.AddComponent<MysticsItemsMysticSwordBehaviour>();
        }

        private void GenericGameEvents_BeforeTakeDamage(DamageInfo damageInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo attackerInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo victimInfo)
        {
            if (attackerInfo.inventory && attackerInfo.inventory.GetItemCount(itemDef) > 0)
            {
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
                                    "+" + ((int)(swordBehaviour.damageBonus * 100f)).ToString(System.Globalization.CultureInfo.InvariantCulture)
                                )
                            );
                            self.tooltipProvider.overrideBodyText = globalStringBuilder.ToString();
                            globalStringBuilder.Clear();
                        }
                    }
                }
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
                if ((damageReport.victimBody.baseMaxHealth * healthMultiplier) >= 1000f)
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
