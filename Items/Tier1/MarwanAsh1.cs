using RoR2;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.BalanceConfigManager;
using System.Collections.Generic;
using System.Linq;

namespace MysticsItems.Items
{
    public class MarwanAsh1 : BaseItem
    {
        public static ConfigurableValue<float> damage = new ConfigurableValue<float>(
            "Item: Marwan s Ash/Light/Weapon",
            "Damage",
            2f,
            "Base damage",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_MARWANASH1_DESC",
                "ITEM_MYSTICSITEMS_MARWANASH2_DESC",
                "ITEM_MYSTICSITEMS_MARWANASH3_DESC"
            }
        );
        public static ConfigurableValue<float> damagePerLevel = new ConfigurableValue<float>(
            "Item: Marwan s Ash/Light/Weapon",
            "DamagePerLevel",
            0.2f,
            "Base damage for each additional level of the owner",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_MARWANASH1_DESC",
                "ITEM_MYSTICSITEMS_MARWANASH2_DESC",
                "ITEM_MYSTICSITEMS_MARWANASH3_DESC"
            }
        );
        public static ConfigurableValue<float> procCoefficient = new ConfigurableValue<float>(
            "Item: Marwan s Ash/Light/Weapon",
            "ProcCoefficient",
            0f,
            "Proc coefficient of the extra hit"
        );
        public static ConfigurableValue<float> dotPercent = new ConfigurableValue<float>(
            "Item: Marwan s Ash/Light/Weapon",
            "DoTPercent",
            0.2f,
            "How much health should the afflicted enemies lose every second on item level 2 (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_MARWANASH2_DESC",
                "ITEM_MYSTICSITEMS_MARWANASH3_DESC"
            }
        );
        public static ConfigurableValue<float> dotPercentPerLevel = new ConfigurableValue<float>(
            "Item: Marwan s Ash/Light/Weapon",
            "DoTPercentPerLevel",
            0.02f,
            "How much health should the afflicted enemies lose every second on item level 2 for each additional level of the owner (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_MARWANASH2_DESC",
                "ITEM_MYSTICSITEMS_MARWANASH3_DESC"
            }
        );
        public static ConfigurableValue<float> dotDuration = new ConfigurableValue<float>(
            "Item: Marwan s Ash/Light/Weapon",
            "DoTDuration",
            2f,
            "How long should the damage over time effect last (in seconds)"
        );
        public static ConfigurableValue<float> radius = new ConfigurableValue<float>(
            "Item: Marwan s Ash/Light/Weapon",
            "Radius",
            7f,
            "Radius of the AoE extra hit on item level 3 (in m)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_MARWANASH3_DESC"
            }
        );
        public static ConfigurableValue<float> radiusPerLevel = new ConfigurableValue<float>(
            "Item: Marwan s Ash/Light/Weapon",
            "RadiusPerLevel",
            1.4f,
            "Radius of the AoE extra hit on item level 3 for each additional level of the owner (in m)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_MARWANASH3_DESC"
            }
        );
        public static ConfigurableValue<float> upgradeLevel12 = new ConfigurableValue<float>(
            "Item: Marwan s Ash/Light/Weapon",
            "UpgradeLevel12",
            17f,
            "Level required for upgrading from level 1 to level 2",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_MARWANASH1_PICKUP",
                "ITEM_MYSTICSITEMS_MARWANASH1_DESC",
                "ITEM_MYSTICSITEMS_MARWANASH2_DESC",
                "ITEM_MYSTICSITEMS_MARWANASH3_DESC"
            }
        );
        public static ConfigurableValue<float> upgradeLevel23 = new ConfigurableValue<float>(
            "Item: Marwan s Ash/Light/Weapon",
            "UpgradeLevel23",
            23f,
            "Level required for upgrading from level 2 to level 3",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_MARWANASH2_PICKUP",
                "ITEM_MYSTICSITEMS_MARWANASH2_DESC",
                "ITEM_MYSTICSITEMS_MARWANASH3_DESC"
            }
        );

        public static DamageAPI.ModdedDamageType ashDamageType;
        public static DamageColorIndex ashDamageColor = DamageColorAPI.RegisterDamageColor(new Color32(96, 245, 250, 255));
        
        public static GameObject ashHitVFX;

        public static float enemyExtraDamageCap;
        public static float enemyBurnDamageCap;
        public static float enemySpreadRadiusCap;

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_MarwanAsh1";
            itemDef.tier = ItemTier.Tier1;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Marwan's Ash/Level 1/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Marwan's Ash/Level 1/Icon.png");
            
            HopooShaderToMaterial.Standard.Gloss(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial, 0f, 0f);
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Chest", new Vector3(-0.14107F, 0.41961F, -0.09729F), new Vector3(344.2867F, 352.4422F, 26.10075F), new Vector3(0.08115F, 0.08115F, 0.08115F));
                AddDisplayRule("HuntressBody", "Chest", new Vector3(0.1801F, 0.27333F, 0.03185F), new Vector3(59.40007F, 154.4985F, 40.1367F), new Vector3(0.06552F, 0.06552F, 0.06552F));
                AddDisplayRule("Bandit2Body", "Chest", new Vector3(-0.1721F, 0.35699F, -0.10153F), new Vector3(313.5005F, 16.6744F, 2.4313F), new Vector3(0.06943F, 0.06943F, 0.06943F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(1.75971F, 2.50135F, -0.23376F), new Vector3(0.68769F, 167.9512F, 0.30914F), new Vector3(0.70768F, 0.70768F, 0.70768F));
                AddDisplayRule("EngiBody", "Chest", new Vector3(0.32528F, 0.37407F, 0.02454F), new Vector3(12.57164F, 341.853F, 318.3862F), new Vector3(0.08925F, 0.08925F, 0.08925F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(-0.47253F, 0.72168F, 0.45966F), new Vector3(0F, 0F, 0F), new Vector3(0.29536F, 0.29536F, 0.29536F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(-0.40498F, 1.38227F, -0.21354F), new Vector3(344.6207F, 123.1186F, 356.0743F), new Vector3(0.33715F, 0.33715F, 0.33715F));
                AddDisplayRule("MageBody", "Chest", new Vector3(0.11005F, 0.334F, -0.06138F), new Vector3(0.00199F, 164.9891F, 11.62114F), new Vector3(0.086F, 0.086F, 0.086F));
                AddDisplayRule("MercBody", "Chest", new Vector3(0.18738F, 0.27546F, -0.05652F), new Vector3(342.8367F, 235.8891F, 41.57959F), new Vector3(0.093F, 0.093F, 0.093F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(0F, 0.42869F, 0F), new Vector3(0F, 0F, 0F), new Vector3(0.46827F, 0.46827F, 0.46827F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(0.09611F, 0.46037F, -0.07509F), new Vector3(358.2845F, 247.987F, 3.11198F), new Vector3(0.09269F, 0.09269F, 0.09269F));
                AddDisplayRule("CrocoBody", "SpineChest1", new Vector3(1.41028F, -0.26767F, -0.19958F), new Vector3(308.9953F, 7.03499F, 320.0229F), new Vector3(1.0009F, 1.0009F, 1.0009F));
                AddDisplayRule("CaptainBody", "Chest", new Vector3(-0.38388F, 0.39949F, -0.01551F), new Vector3(15.849F, 346.474F, 26.10147F), new Vector3(0.10616F, 0.10616F, 0.10616F));
                AddDisplayRule("BrotherBody", "chest", BrotherInfection.white, new Vector3(-0.22101F, 0.42643F, -0.064F), new Vector3(0F, 0F, 281.5435F), new Vector3(0.04683F, 0.09274F, 0.10516F));
                AddDisplayRule("ScavBody", "Chest", new Vector3(9.15645F, 0.11521F, -0.43519F), new Vector3(305.6901F, 285.3795F, 356.474F), new Vector3(2.5856F, 2.65682F, 2.5856F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Chest", new Vector3(0.11749F, 0.41517F, -0.24199F), new Vector3(0.37534F, 284.0059F, 0.99266F), new Vector3(0.07834F, 0.07834F, 0.07834F));
                AddDisplayRule("RailgunnerBody", "Chest", new Vector3(0.17061F, 0.14475F, -0.02294F), new Vector3(356.5987F, 19.29597F, 326.4724F), new Vector3(0.06946F, 0.06946F, 0.06946F));
                AddDisplayRule("VoidSurvivorBody", "UpperArmL", new Vector3(0.07068F, 0.07111F, -0.00293F), new Vector3(356.8744F, 349.4176F, 253.7303F), new Vector3(0.10579F, 0.10579F, 0.10579F));
            };

            GenericGameEvents.OnHitEnemy += GenericGameEvents_OnHitEnemy;
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            GlobalEventManager.onCharacterLevelUp += GlobalEventManager_onCharacterLevelUp;

            ashDamageType = DamageAPI.ReserveDamageType();

            ashHitVFX = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Marwan's Ash/Level 1/AshHitVFX.prefab");
            ashHitVFX.AddComponent<EffectComponent>();
            VFXAttributes vfxAttributes = ashHitVFX.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Low;
            ashHitVFX.AddComponent<DestroyOnTimer>().duration = 2f;
            ashHitVFX.transform.localScale *= 2f;
            MysticsItemsContent.Resources.effectPrefabs.Add(ashHitVFX);

            RoR2Application.onLoad += () =>
            {
                MysticsItemsMarwanAshHelper.level2PickupIndex = PickupCatalog.FindPickupIndex(MysticsItemsContent.Items.MysticsItems_MarwanAsh2.itemIndex);
                MysticsItemsMarwanAshHelper.level3PickupIndex = PickupCatalog.FindPickupIndex(MysticsItemsContent.Items.MysticsItems_MarwanAsh3.itemIndex);
            };

            enemyExtraDamageCap = damage + damagePerLevel * 98f;
            enemyBurnDamageCap = dotPercent / 100f + dotPercentPerLevel / 100f * (99f - (float)upgradeLevel12);
            enemySpreadRadiusCap = radius + radiusPerLevel * (99f - (float)upgradeLevel23);
        }

        private void GenericGameEvents_OnHitEnemy(DamageInfo damageInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo attackerInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo victimInfo)
        {
            if (attackerInfo.body && victimInfo.body && attackerInfo.inventory && !damageInfo.rejected)
            {
                var isAshDamage = DamageAPI.HasModdedDamageType(damageInfo, ashDamageType);

                var itemCount = 0;
                var itemLevel = 1;
                var ashHelper = attackerInfo.body.GetComponent<MysticsItemsMarwanAshHelper>();
                if (ashHelper)
                {
                    itemCount = ashHelper.itemCount;
                    itemLevel = ashHelper.itemLevel;
                }

                if (!isAshDamage && victimInfo.healthComponent && attackerInfo.gameObject && damageInfo.procCoefficient > 0f)
                {
                    if (itemCount > 0)
                    {
                        var _damage = damage + damagePerLevel * (attackerInfo.body.level - 1f) * itemCount;
                        if (attackerInfo.teamIndex != TeamIndex.Player) _damage = Mathf.Min(_damage, enemyExtraDamageCap);
                        var _crit = attackerInfo.body.RollCrit();
                        if (itemLevel < 3)
                        {
                            DamageInfo extraDamageInfo = new DamageInfo();
                            extraDamageInfo.damage = _damage;
                            extraDamageInfo.attacker = attackerInfo.gameObject;
                            extraDamageInfo.procCoefficient = procCoefficient;
                            extraDamageInfo.position = damageInfo.position;
                            extraDamageInfo.crit = _crit;
                            extraDamageInfo.damageColorIndex = ashDamageColor;
                            extraDamageInfo.procChainMask = damageInfo.procChainMask;
                            extraDamageInfo.damageType = DamageType.Silent;
                            DamageAPI.AddModdedDamageType(extraDamageInfo, ashDamageType);
                            victimInfo.healthComponent.TakeDamage(extraDamageInfo);
                            GlobalEventManager.instance.OnHitEnemy(extraDamageInfo, victimInfo.healthComponent.gameObject);
                        }
                        else
                        {
                            var _radius = radius + radiusPerLevel * (attackerInfo.body.level - (float)upgradeLevel23) * itemCount;
                            if (attackerInfo.teamIndex != TeamIndex.Player) _radius = Mathf.Min(_radius, enemySpreadRadiusCap);
                            var blastAttack = new BlastAttack
                            {
                                radius = _radius,
                                baseDamage = _damage,
                                procCoefficient = procCoefficient,
                                crit = _crit,
                                damageColorIndex = ashDamageColor,
                                attackerFiltering = AttackerFiltering.Default,
                                falloffModel = BlastAttack.FalloffModel.None,
                                attacker = attackerInfo.gameObject,
                                teamIndex = attackerInfo.teamIndex,
                                position = damageInfo.position,
                            };
                            DamageAPI.AddModdedDamageType(blastAttack, ashDamageType);
                            blastAttack.Fire();
                        }
                    }
                }
                if (isAshDamage)
                {
                    EffectManager.SimpleImpactEffect(ashHitVFX, damageInfo.position, Vector3.Normalize(Random.onUnitSphere), true);
                    
                    if (itemLevel >= 2)
                    {
                        var dotInfo = new InflictDotInfo
                        {
                            victimObject = victimInfo.gameObject,
                            attackerObject = attackerInfo.gameObject,
                            dotIndex = Buffs.MarwanAshBurn.ashDotIndex,
                            duration = dotDuration,
                            damageMultiplier = 1f,
                            totalDamage = null
                        };
                        
                        var strengthenBurnCount = attackerInfo.inventory.GetItemCount(DLC1Content.Items.StrengthenBurn);
                        if (strengthenBurnCount > 0)
                        {
                            dotInfo.dotIndex = Buffs.MarwanAshBurnStrong.ashDotIndex;
                            var multiplier = 1f + 3f * (float)strengthenBurnCount;
                            dotInfo.damageMultiplier *= multiplier;
                            dotInfo.duration *= multiplier;
                        }
                        
                        DotController.InflictDot(ref dotInfo);
                    }
                }
            }
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            MysticsItemsMarwanAshHelper component = body.gameObject.AddComponent<MysticsItemsMarwanAshHelper>();
            component.dirty = true;
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            var component = self.GetComponent<MysticsItemsMarwanAshHelper>();
            if (component) component.dirty = true;
        }

        private void GlobalEventManager_onCharacterLevelUp(CharacterBody body)
        {
            var component = body.GetComponent<MysticsItemsMarwanAshHelper>();
            if (component) component.dirty = true;
        }

        public class MysticsItemsMarwanAshHelper : MonoBehaviour
        {
            public CharacterBody body;
            
            public bool dirty = false;
            public int itemLevel = 1;
            public int itemCount = 0;
            public BurnEffectController burnEffectController;
            public BurnEffectController burnEffectControllerStrong;

            public void Awake()
            {
                body = GetComponent<CharacterBody>();
            }

            internal static PickupIndex level2PickupIndex;
            internal static PickupIndex level3PickupIndex;

            public void FixedUpdate()
            {
                if (dirty && body.inventory)
                {
                    dirty = false;

                    itemLevel = 1;
                    if (body.level >= upgradeLevel12) itemLevel = 2;
                    if (body.level >= upgradeLevel23) itemLevel = 3;

                    var itemCount1 = body.inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_MarwanAsh1);
                    var itemCount2 = body.inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_MarwanAsh2);
                    var itemCount3 = body.inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_MarwanAsh3);

                    if (itemLevel == 2 && NetworkServer.active)
                    {
                        body.inventory.RemoveItem(MysticsItemsContent.Items.MysticsItems_MarwanAsh1, itemCount1);
                        body.inventory.GiveItem(MysticsItemsContent.Items.MysticsItems_MarwanAsh2, itemCount1);

                        if (itemCount1 > 0)
                        {
                            CharacterMasterNotificationQueue.PushItemTransformNotification(
                                body.master,
                                MysticsItemsContent.Items.MysticsItems_MarwanAsh1.itemIndex,
                                MysticsItemsContent.Items.MysticsItems_MarwanAsh2.itemIndex,
                                CharacterMasterNotificationQueue.TransformationType.Default
                            );
                            NetworkPickupDiscovery.DiscoverPickup(body.master, level2PickupIndex);
                        }

                        itemCount2 += itemCount1;
                        itemCount1 = 0;
                    }
                    if (itemLevel == 3 && NetworkServer.active)
                    {
                        body.inventory.RemoveItem(MysticsItemsContent.Items.MysticsItems_MarwanAsh1, itemCount1);
                        body.inventory.RemoveItem(MysticsItemsContent.Items.MysticsItems_MarwanAsh2, itemCount2);
                        body.inventory.GiveItem(MysticsItemsContent.Items.MysticsItems_MarwanAsh3, itemCount1 + itemCount2);

                        if (itemCount1 > 0)
                        {
                            CharacterMasterNotificationQueue.PushItemTransformNotification(
                                body.master,
                                MysticsItemsContent.Items.MysticsItems_MarwanAsh1.itemIndex,
                                MysticsItemsContent.Items.MysticsItems_MarwanAsh3.itemIndex,
                                CharacterMasterNotificationQueue.TransformationType.Default
                            );
                            NetworkPickupDiscovery.DiscoverPickup(body.master, level2PickupIndex);
                        }
                        if (itemCount2 > 0)
                        {
                            CharacterMasterNotificationQueue.PushItemTransformNotification(
                                body.master,
                                MysticsItemsContent.Items.MysticsItems_MarwanAsh2.itemIndex,
                                MysticsItemsContent.Items.MysticsItems_MarwanAsh3.itemIndex,
                                CharacterMasterNotificationQueue.TransformationType.Default
                            );
                            NetworkPickupDiscovery.DiscoverPickup(body.master, level3PickupIndex);
                        }

                        itemCount3 += itemCount1 + itemCount2;
                        itemCount1 = 0;
                        itemCount2 = 0;
                    }
                    itemCount = itemCount1 + itemCount2 + itemCount3;
                }
            }
        }
    }
}
