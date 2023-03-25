using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using static MysticsItems.LegacyBalanceConfigManager;

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
            3.2f,
            "Base regen bonus for all allies (in HP/s)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_REGENANDDIFFICULTYSPEED_DESC"
            }
        );
        public static ConfigurableValue<float> baseRegenIncreasePerStack = new ConfigurableValue<float>(
            "Item: Puzzle of Chronos",
            "BaseRegenIncreasePerStack",
            3.2f,
            "Base regen bonus for all allies for each additional stack of this item (in HP/s)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_REGENANDDIFFICULTYSPEED_DESC"
            }
        );
        public static ConfigurableValue<float> timerSpeedIncrease = new ConfigurableValue<float>(
            "Item: Puzzle of Chronos",
            "TimerSpeedIncrease",
            30f,
            "How much faster should difficulty scale over time (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_REGENANDDIFFICULTYSPEED_DESC"
            }
        );
        public static ConfigurableValue<float> timerSpeedIncreasePerStack = new ConfigurableValue<float>(
            "Item: Puzzle of Chronos",
            "TimerSpeedIncreasePerStack",
            30f,
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
            SetItemTierWhenAvailable(ItemTier.Lunar);
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

            var followerPrefab = PrefabAPI.InstantiateClone(PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false)), "MysticsItems_MysticSwordItemFollowerPrefab", false);
            followerPrefab.transform.Find("Scaler").localScale *= 0.6f;
            ObjectTransformCurve objectTransformCurve = followerPrefab.transform.Find("Scaler").gameObject.AddComponent<ObjectTransformCurve>();
            objectTransformCurve.useTranslationCurves = false;
            objectTransformCurve.timeMax = 20f;
            objectTransformCurve.rotationCurveX = AnimationCurve.Constant(0f, 1f, 0f);
            objectTransformCurve.rotationCurveY = AnimationCurve.Linear(0f, -360f, 1f, 0f);
            objectTransformCurve.rotationCurveY.preWrapMode = WrapMode.Loop;
            objectTransformCurve.rotationCurveY.postWrapMode = WrapMode.Loop;
            objectTransformCurve.rotationCurveZ = AnimationCurve.Constant(0f, 1f, 0f);
            objectTransformCurve.useRotationCurves = true;
            objectTransformCurve.gameObject.AddComponent<MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsObjectTransformCurveLoop>();

            itemDisplayPrefab = PrefabAPI.InstantiateClone(new GameObject("MysticsItems_LunarCubeFollower"), "MysticsItems_LunarCubeFollower", false);
            itemDisplayPrefab.AddComponent<ItemDisplay>();
            ItemFollower itemFollower = itemDisplayPrefab.AddComponent<ItemFollower>();
            itemFollower.followerPrefab = followerPrefab;
            itemFollower.distanceDampTime = 0.1f;
            itemFollower.distanceMaxSpeed = 20f;
            itemFollower.targetObject = itemDisplayPrefab;

            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Base", new Vector3(-0.53829F, -0.13216F, 0.11265F), new Vector3(3.15473F, 89.99998F, 270.0002F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("HuntressBody", "Base", new Vector3(-0.50487F, -0.23664F, 0.24332F), new Vector3(2.42504F, 269.9999F, 90.0001F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("Bandit2Body", "Base", new Vector3(0.17414F, -0.32998F, -0.00678F), new Vector3(270F, 0F, 0F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("ToolbotBody", "Base", new Vector3(2.4828F, -4.92693F, 1.29549F), new Vector3(0F, 90F, 90F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("EngiBody", "Base", new Vector3(-0.60578F, -0.52338F, -0.14311F), new Vector3(270F, 0F, 0F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("EngiTurretBody", "Base", new Vector3(-1.18285F, 2.28749F, -2.48034F), new Vector3(0F, 90F, 0F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("EngiWalkerTurretBody", "Base", new Vector3(-0.87233F, 2.00669F, -2.59369F), new Vector3(0F, 90F, 0F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("MageBody", "Base", new Vector3(-0.43306F, -0.43867F, 0.26171F), new Vector3(270F, 0F, 0F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("MercBody", "Base", new Vector3(-0.52697F, -0.49787F, 0.45683F), new Vector3(270F, 0F, 0F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("TreebotBody", "Base", new Vector3(-0.75802F, -1.67818F, -0.93951F), new Vector3(270F, 0F, 0F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("LoaderBody", "Base", new Vector3(-0.43154F, -0.69567F, 0.22747F), new Vector3(270F, 0F, 0F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("CrocoBody", "Base", new Vector3(-4.12918F, 3.9816F, 0.70414F), new Vector3(90F, 0F, 0F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("CaptainBody", "Base", new Vector3(-0.35685F, -0.47045F, 0.06453F), new Vector3(270F, 0F, 0F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("BrotherBody", "ThighL", BrotherInfection.blue, new Vector3(0.07921F, -0.00948F, 0.04758F), new Vector3(68.71732F, 228.1228F, 69.86813F), new Vector3(0.09518F, 0.04176F, 0.09524F));
                AddDisplayRule("ScavBody", "Base", new Vector3(-14.92595F, 3.65256F, 0.84163F), new Vector3(90F, 0F, 0F), new Vector3(4F, 4F, 4F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Base", new Vector3(-0.5431F, 0.93526F, 0.49732F), new Vector3(10.681F, 0.007F, 0.071F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("RailgunnerBody", "Base", new Vector3(-0.59444F, -0.29985F, 0.26853F), new Vector3(90F, 0F, 0F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("VoidSurvivorBody", "Base", new Vector3(-0.41662F, 0.33464F, -0.26613F), new Vector3(90F, 0F, 0F), new Vector3(1F, 1F, 1F));
            };

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
            };
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
                args.baseRegenAdd += (baseRegenIncrease.Value + baseRegenIncreasePerStack.Value * (float)(itemCount - 1)) * (1f + 0.2f * (sender.level - 1f));
            }
        }
    }
}
