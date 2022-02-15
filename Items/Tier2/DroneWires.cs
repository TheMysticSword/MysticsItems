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
using RoR2.Projectile;

namespace MysticsItems.Items
{
    public class DroneWires : BaseItem
    {
        public static ConfigurableValue<float> damage = new ConfigurableValue<float>(
            "Item: Spare Wiring",
            "Damage",
            200f,
            "Base damage of the sparks (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_DRONEWIRES_DESC"
            }
        );
        public static ConfigurableValue<float> damagePerStack = new ConfigurableValue<float>(
            "Item: Spare Wiring",
            "DamagePerStack",
            160f,
            "Base damage of the sparks for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_DRONEWIRES_DESC"
            }
        );
        public static ConfigurableValue<float> procCoefficient = new ConfigurableValue<float>(
            "Item: Spare Wiring",
            "ProcCoefficient",
            1f,
            "Spark proc coefficient"
        );
        public static ConfigurableValue<float> droneFireInterval = new ConfigurableValue<float>(
            "Item: Spare Wiring",
            "DroneFireInterval",
            0.3f,
            "How much time should pass between each time a drone drops sparks (in seconds)"
        );
        public static ConfigurableValue<int> droneFireCount = new ConfigurableValue<int>(
            "Item: Spare Wiring",
            "DroneFireCount",
            1,
            "How many sparks should a drone drop in each fire cycle"
        );
        public static ConfigurableValue<float> playerFireInterval = new ConfigurableValue<float>(
            "Item: Spare Wiring",
            "DroneFireInterval",
            3f,
            "How much time should pass between each time a player drops sparks (in seconds)"
        );
        public static ConfigurableValue<int> playerFireCount = new ConfigurableValue<int>(
            "Item: Spare Wiring",
            "DroneFireCount",
            5,
            "How many sparks should a player drop in each fire cycle"
        );

        public static GameObject sparkProjectilePrefab;

        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            sparkProjectilePrefab = Utils.CreateBlankPrefab("MysticsItems_SparkProjectile", true);
            sparkProjectilePrefab.GetComponent<NetworkIdentity>().localPlayerAuthority = true;
        }

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_DroneWires";
            itemDef.tier = ItemTier.Tier2;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage,
                ItemTag.CannotCopy
            };
            
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Wires/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Wires/Icon.png");
            itemDisplayPrefab = PrepareItemDisplayModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Wires/DisplayModel.prefab"));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Chest", new Vector3(-0.0882F, 0.29957F, 0.10621F), new Vector3(24.65009F, 16.77799F, 135.6651F), new Vector3(0.07763F, 0.07608F, 0.09464F));
                AddDisplayRule("HuntressBody", "Chest", new Vector3(0.11016F, 0.19166F, 0.06043F), new Vector3(3.66903F, 357.0302F, 46.24356F), new Vector3(0.06181F, 0.06181F, 0.07285F));
                AddDisplayRule("Bandit2Body", "Chest", new Vector3(0.02937F, 0.2926F, -0.00376F), new Vector3(0F, 0F, 49.27628F), new Vector3(0.06534F, 0.07626F, 0.07902F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(-0.44676F, 0.83563F, -0.35079F), new Vector3(0F, 255.7838F, 12.94845F), new Vector3(0.29993F, 0.29993F, 0.29993F));
                AddDisplayRule("EngiBody", "Chest", new Vector3(0.12126F, 0.34647F, 0.02248F), new Vector3(9.43959F, 353.3998F, 54.79713F), new Vector3(0.08793F, 0.10983F, 0.12737F));
                //AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.035F, 0.89075F, -1.47928F), new Vector3(0F, 90F, 303.695F), new Vector3(0.07847F, 0.07847F, 0.07847F));
                //AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.03562F, 1.40676F, -1.39837F), new Vector3(0F, 90F, 303.1705F), new Vector3(0.08093F, 0.09844F, 0.07912F));
                AddDisplayRule("MageBody", "Chest", new Vector3(0.04831F, 0.18725F, -0.00428F), new Vector3(0F, 0F, 60.53666F), new Vector3(0.07311F, 0.07311F, 0.06881F));
                AddDisplayRule("MercBody", "Chest", new Vector3(-0.05362F, 0.21034F, -0.00002F), new Vector3(0F, 0F, 117.922F), new Vector3(0.0773F, 0.0773F, 0.10484F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(0.17759F, 1.01678F, -0.22692F), new Vector3(343.6147F, 91.47517F, 0F), new Vector3(0.12892F, 0.22446F, 0.19628F));
                AddDisplayRule("LoaderBody", "Chest", new Vector3(0.2286F, 0.17001F, -0.14346F), new Vector3(0F, 0F, 101.2838F), new Vector3(0.07407F, 0.07407F, 0.07407F));
                AddDisplayRule("CrocoBody", "Neck", new Vector3(-0.44025F, 2.13047F, -1.0941F), new Vector3(11.69976F, 267.5576F, 69.92433F), new Vector3(0.78301F, 0.82242F, 0.76275F));
                AddDisplayRule("CaptainBody", "Chest", new Vector3(0.11944F, 0.24645F, 0.02558F), new Vector3(0F, 0F, 56.89931F), new Vector3(0.11927F, 0.11927F, 0.09405F));
                AddDisplayRule("BrotherBody", "chest", BrotherInfection.green, new Vector3(-0.02217F, 0.1268F, 0.00908F), new Vector3(38.70592F, 101.3755F, 273.0802F), new Vector3(0.29321F, 0.2044F, 0.2044F));
                AddDisplayRule("ScavBody", "Chest", new Vector3(-0.60161F, 5.1633F, -0.70479F), new Vector3(8.51487F, 288.6277F, 143.041F), new Vector3(1.72129F, 2.93378F, 2.41413F));
            };

            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;

            MysticsRisky2Utils.Utils.CopyChildren(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Wires/SparkProjectile.prefab"), sparkProjectilePrefab);
            
            float sparkDuration = 4f;

            ProjectileController projectileController = sparkProjectilePrefab.AddComponent<ProjectileController>();
            projectileController.ghostPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Wires/SparkProjectileGhost.prefab");
            projectileController.ghostPrefab.AddComponent<ProjectileGhostController>();
            ObjectScaleCurve objectScaleCurve = projectileController.ghostPrefab.AddComponent<ObjectScaleCurve>();
            objectScaleCurve.overallCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
            objectScaleCurve.useOverallCurveOnly = true;
            objectScaleCurve.timeMax = sparkDuration;
            projectileController.allowPrediction = true;
            sparkProjectilePrefab.AddComponent<ProjectileNetworkTransform>();
            sparkProjectilePrefab.AddComponent<TeamFilter>();
            ProjectileDamage projectileDamage = sparkProjectilePrefab.AddComponent<ProjectileDamage>();
            HitBoxGroup hitBoxGroup = sparkProjectilePrefab.AddComponent<HitBoxGroup>();
            hitBoxGroup.groupName = "MysticsItems_DroneWiresSpark";
            hitBoxGroup.hitBoxes = new HitBox[]
            {
                sparkProjectilePrefab.transform.Find("HitBox").gameObject.AddComponent<HitBox>()
            };
            ProjectileSimple projectileSimple = sparkProjectilePrefab.AddComponent<ProjectileSimple>();
            projectileSimple.desiredForwardSpeed = 14f;
            projectileSimple.lifetime = sparkDuration;
            ProjectileOverlapAttack projectileOverlapAttack = sparkProjectilePrefab.AddComponent<ProjectileOverlapAttack>();
            projectileOverlapAttack.damageCoefficient = 1f;
            projectileOverlapAttack.overlapProcCoefficient = procCoefficient;
            projectileOverlapAttack.resetInterval = 0.5f;
            sparkProjectilePrefab.layer = LayerIndex.projectile.intVal;
            
            MysticsItemsContent.Resources.projectilePrefabs.Add(sparkProjectilePrefab);
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (NetworkServer.active)
            {
                var component = self.AddItemBehavior<MysticsItemsDroneWiresBehaviour>(self.inventory.GetItemCount(itemDef));
                if (component && component.firstInit) component.UpdateDroneInventories();
            }
        }

        public class MysticsItemsDroneWiresBehaviour : CharacterBody.ItemBehavior
        {
            public int oldStack = 0;
            public bool firstInit = false;
            public float timer = 0f;
            public float interval = droneFireInterval;
            public int sparksToFire = droneFireCount;
            public float accurateShotChance = 20f;
            public float minSpeed = 7f;
            public float maxSpeed = 25f;
            public float coneAngle = 45f;
            
            public void Start()
            {
                firstInit = true;
                var isMinion = body.master && body.master.minionOwnership && body.master.minionOwnership.group != null && body.master.minionOwnership.group.isMinion;
                if (!isMinion)
                {
                    interval = playerFireInterval;
                    sparksToFire = playerFireCount;
                }
                timer = UnityEngine.Random.value * interval;
                UpdateDroneInventories();
                MasterSummon.onServerMasterSummonGlobal += MasterSummon_onServerMasterSummonGlobal;
            }

            private void MasterSummon_onServerMasterSummonGlobal(MasterSummon.MasterSummonReport summonReport)
            {
                if (body.master && body.master == summonReport.leaderMasterInstance)
                {
                    CharacterMaster summonMasterInstance = summonReport.summonMasterInstance;
                    if (summonMasterInstance)
                    {
                        CharacterBody body = summonMasterInstance.GetBody();
                        if (body && body.bodyFlags.HasFlag(CharacterBody.BodyFlags.Mechanical))
                        {
                            summonMasterInstance.inventory.GiveItem(MysticsItemsContent.Items.MysticsItems_DroneWires, stack);
                        }
                    }
                }
            }

            public void UpdateDroneInventories()
            {
                UpdateDroneInventories(stack - oldStack);
                oldStack = stack;
            }

            public void UpdateDroneInventories(int difference)
            {
                var master = body.master;
                if (master)
                {
                    foreach (var otherBody in TeamComponent.GetTeamMembers(master.teamIndex).Select(x => x.body).Where(x => x != null))
                    {
                        if (otherBody.bodyFlags.HasFlag(CharacterBody.BodyFlags.Mechanical) && otherBody.master && otherBody.inventory)
                        {
                            var minionOwnership = otherBody.master.minionOwnership;
                            if (minionOwnership && minionOwnership.ownerMaster == master)
                            {
                                if (difference > 0) otherBody.inventory.GiveItem(MysticsItemsContent.Items.MysticsItems_DroneWires, difference);
                                else if (difference < 0) otherBody.inventory.RemoveItem(MysticsItemsContent.Items.MysticsItems_DroneWires, -difference);
                            }
                        }
                    }
                }
            }

            public void OnDestroy()
            {
                UpdateDroneInventories(-stack);
                MasterSummon.onServerMasterSummonGlobal -= MasterSummon_onServerMasterSummonGlobal;
            }

            public void FixedUpdate()
            {
                if (!body.outOfCombat)
                {
                    timer += Time.fixedDeltaTime;
                    if (timer >= interval)
                    {
                        timer = 0;

                        var crit = body.RollCrit();
                        float initialAngle = UnityEngine.Random.value * 360f;

                        bool accurateShot = false;
                        RaycastHit accurateShotRaycast = default(RaycastHit);
                        if (accurateShotChance > 0f && Util.CheckRoll(accurateShotChance) && body.inputBank && body.inputBank.GetAimRaycast(300f, out accurateShotRaycast))
                        {
                            accurateShot = true;
                        }

                        for (var i = 0; i < sparksToFire; i++)
                        {
                            Vector3 fireOrigin = body.corePosition;
                            Vector3 direction = Vector3.up;
                            direction = Quaternion.AngleAxis(UnityEngine.Random.value * coneAngle, Vector3.forward) * direction;
                            direction = Quaternion.AngleAxis(initialAngle + 360f / sparksToFire * i, Vector3.up) * direction;
                            float speed = UnityEngine.Random.Range(minSpeed, maxSpeed);

                            if (accurateShot && i == 0)
                            {
                                float accurateShotTime = UnityEngine.Random.Range(0.2f, 0.5f);

                                var distance = accurateShotRaycast.point - fireOrigin;
                                direction = new Vector3(
                                    distance.x / accurateShotTime,
                                    Trajectory.CalculateInitialYSpeed(accurateShotTime, distance.y),
                                    distance.z / accurateShotTime
                                );
                                speed = direction.magnitude;
                            }

                            ProjectileManager.instance.FireProjectile(
                                sparkProjectilePrefab,
                                fireOrigin,
                                Util.QuaternionSafeLookRotation(direction),
                                body.gameObject,
                                body.damage * (damage / 100f + damagePerStack / 100f * (float)(stack - 1)),
                                0f,
                                crit,
                                DamageColorIndex.Default,
                                null,
                                speed
                            );
                        }
                    }
                }
            }
        }
    }
}
