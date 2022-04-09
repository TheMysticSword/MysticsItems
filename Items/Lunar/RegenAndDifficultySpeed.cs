using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static MysticsItems.BalanceConfigManager;

namespace MysticsItems.Items
{
    public class RegenAndDifficultySpeed : BaseItem
    {
        public static float extraDifficultyTime = 0f;
        public static bool calculatingDifficultyCoefficient = false;
        public static int totalItemCount = 0;
        public static float onlineSyncTimer = 0f;
        public static float onlineSyncDuration = 60f;

        public static ConfigurableValue<float> baseRegenIncrease = new ConfigurableValue<float>(
            "Item: Puzzle of Chronos",
            "BaseRegenIncrease",
            6f,
            "Base regen bonus for all allies (in HP/s)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_REGENANDDIFFICULTYSPEED_DESC"
            }
        );
        public static ConfigurableValue<float> baseRegenIncreasePerStack = new ConfigurableValue<float>(
            "Item: Puzzle of Chronos",
            "BaseRegenIncreasePerStack",
            6f,
            "Base regen bonus for all allies for each additional stack of this item (in HP/s)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_REGENANDDIFFICULTYSPEED_DESC"
            }
        );
        public static ConfigurableValue<float> timerSpeedIncrease = new ConfigurableValue<float>(
            "Item: Puzzle of Chronos",
            "TimerSpeedIncrease",
            20f,
            "How much faster should difficulty scale over time (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_REGENANDDIFFICULTYSPEED_DESC"
            }
        );
        public static ConfigurableValue<float> timerSpeedIncreasePerStack = new ConfigurableValue<float>(
            "Item: Puzzle of Chronos",
            "TimerSpeedIncreasePerStack",
            20f,
            "How much faster should difficulty scale over time for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_REGENANDDIFFICULTYSPEED_DESC"
            }
        );

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_RegenAndDifficultySpeed";
            itemDef.tier = ItemTier.Lunar;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Healing,
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Lunar Cube/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Lunar Cube/Icon.png");
            Material material = itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial;
            HopooShaderToMaterial.Standard.Apply(material);
            ColorUtility.TryParseHtmlString("#9BFFFF", out Color color);
            HopooShaderToMaterial.Standard.Emission(material, 2f, color);
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));

            Rigidbody rigidbody = itemDisplayPrefab.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;

            SimpleLeash leash = itemDisplayPrefab.AddComponent<SimpleLeash>();
            leash.minLeashRadius = 1f;
            leash.maxLeashRadius = 20f;
            leash.maxFollowSpeed = 27f;
            leash.smoothTime = 0.18f;

            itemDisplayPrefab.transform.localScale *= 8f;
            itemDisplayPrefab.transform.localRotation = itemDisplayPrefab.transform.localRotation * Quaternion.AngleAxis(45f, Vector3.up) * Quaternion.AngleAxis(45f, Vector3.forward);

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;

            On.RoR2.Run.Start += (orig, self) =>
            {
                orig(self);
                extraDifficultyTime = 0f;
            };

            On.RoR2.Run.RecalculateDifficultyCoefficentInternal += (orig, self) =>
            {
                calculatingDifficultyCoefficient = true;
                orig(self);
                calculatingDifficultyCoefficient = false;
            };

            On.RoR2.Run.GetRunStopwatch += (orig, self) =>
            {
                return orig(self) + (calculatingDifficultyCoefficient ? extraDifficultyTime : 0);
            };

            On.RoR2.Run.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (totalItemCount > 0)
                {
                    if (!self.isRunStopwatchPaused)
                        extraDifficultyTime += Time.fixedDeltaTime * (timerSpeedIncrease.Value / 100f + timerSpeedIncreasePerStack.Value / 100f * (float)(totalItemCount - 1));

                    if (NetworkServer.active)
                    {
                        onlineSyncTimer -= Time.fixedDeltaTime;
                        if (onlineSyncTimer <= 0f)
                        {
                            onlineSyncTimer = onlineSyncDuration;
                            new SyncTimer(extraDifficultyTime).Send(NetworkDestination.Clients);
                        }
                    }
                }
            };

            On.RoR2.CharacterBody.OnInventoryChanged += (orig, self) =>
            {
                orig(self);

                int newTotalItemCount = 0;
                for (int team = 0; team < (int)TeamIndex.Count; team++)
                {
                    newTotalItemCount += Util.GetItemCountForTeam((TeamIndex)team, itemDef.itemIndex, true);
                }
                if (totalItemCount > 0 && newTotalItemCount == 0 && NetworkServer.active)
                {
                    new SyncTimer(extraDifficultyTime).Send(NetworkDestination.Clients);
                }
                totalItemCount = newTotalItemCount;

                self.AddItemBehavior<MysticsItemsRegenAndDifficultySpeedBehaviour>(self.inventory.GetItemCount(itemDef));
            };
        }

        public class MysticsItemsRegenAndDifficultySpeedBehaviour : CharacterBody.ItemBehavior
        {
            public GameObject follower;
            public SimpleLeash leash;

            public void Start()
            {
                follower = Object.Instantiate<GameObject>(loadedDictionary["MysticsItems_RegenAndDifficultySpeed"].itemDisplayPrefab, this.body.corePosition, Quaternion.identity);
                leash = follower.GetComponent<SimpleLeash>();
            }

            public void Update()
            {
                if (leash) leash.leashOrigin = body.corePosition;
            }

            public void OnDestroy()
            {
                if (follower)
                {
                    Object.Destroy(follower);
                }
            }
        }

        public class SyncTimer : INetMessage
        {
            float time;

            public SyncTimer()
            {
            }

            public SyncTimer(float time)
            {
                this.time = time;
            }

            public void Deserialize(NetworkReader reader)
            {
                time = reader.ReadSingle();
            }

            public void OnReceived()
            {
                if (NetworkServer.active) return;
                extraDifficultyTime = time;
            }

            public void Serialize(NetworkWriter writer)
            {
                writer.Write(time);
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var itemCount = Util.GetItemCountForTeam(sender.teamComponent.teamIndex, itemDef.itemIndex, true);
            if (itemCount > 0) {
                args.baseRegenAdd += baseRegenIncrease.Value + baseRegenIncreasePerStack.Value * (float)(itemCount - 1);
            }
        }
    }
}
