using RoR2;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.LegacyBalanceConfigManager;
using System.Collections.Generic;
using System.Linq;

namespace MysticsItems.Items
{
    public class Flow : BaseItem
    {
        public static ConfigurableValue<float> moveSpeed = new ConfigurableValue<float>(
            "Item: Constant Flow",
            "MoveSpeed",
            7f,
            "Movement speed increase (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_FLOW_DESC"
            }
        );
        public static ConfigurableValue<float> moveSpeedPerStack = new ConfigurableValue<float>(
            "Item: Constant Flow",
            "MoveSpeedPerStack",
            7f,
            "Movement speed increase for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_FLOW_DESC"
            }
        );
        public static ConfigurableValue<float> slowReduction = new ConfigurableValue<float>(
            "Item: Constant Flow",
            "SlowReduction",
            35f,
            "Slowing effect reduction (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_FLOW_DESC"
            }
        );
        public static ConfigurableValue<float> slowReductionPerStack = new ConfigurableValue<float>(
            "Item: Constant Flow",
            "SlowReductionPerStack",
            35f,
            "Slowing effect reduction for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_FLOW_DESC"
            }
        );
        public static ConfigurableValue<float> initialRootSlow = new ConfigurableValue<float>(
            "Item: Constant Flow",
            "InitialRootSlow",
            75f,
            "How much should rooting effects slow you down (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_FLOW_DESC"
            }
        );
        public static ConfigurableValue<float> rootSlowReductionPerStack = new ConfigurableValue<float>(
            "Item: Constant Flow",
            "RootSlowReductionPerStack",
            15f,
            "Reduction of the slowing effect from roots for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_FLOW_DESC"
            }
        );

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_Flow";
            SetItemTierWhenAvailable(ItemTier.Tier1);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Flow/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Flow/Icon.png");
            var clothReenabler = itemDef.pickupModelPrefab.AddComponent<MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsClothReenabler>();
            clothReenabler.clothToReenable = itemDef.pickupModelPrefab.GetComponentsInChildren<Cloth>().ToArray();

            var jellyFollowerPrefab = PrefabAPI.InstantiateClone(PrepareItemDisplayModel(PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Flow/FollowerModel.prefab"))), "MysticsItems_JellyFollowerPrefab", false);
            clothReenabler = jellyFollowerPrefab.AddComponent<MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsClothReenabler>();
            clothReenabler.clothToReenable = jellyFollowerPrefab.GetComponentsInChildren<Cloth>().ToArray();
            jellyFollowerPrefab.transform.Find("meduza-duza6").transform.localScale *= 0.05f;
            var objectTransformCurve = jellyFollowerPrefab.transform.Find("meduza-duza6").gameObject.AddComponent<ObjectTransformCurve>();
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
            objectTransformCurve.timeMax = 20f;
            objectTransformCurve.gameObject.AddComponent<MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsObjectTransformCurveLoop>();

            itemDisplayPrefab = PrefabAPI.InstantiateClone(new GameObject("MysticsItems_JellyFollower"), "MysticsItems_JellyFollower", false);
            itemDisplayPrefab.AddComponent<ItemDisplay>();
            ItemFollower itemFollower = itemDisplayPrefab.AddComponent<ItemFollower>();
            itemFollower.followerPrefab = jellyFollowerPrefab;
            itemFollower.distanceDampTime = 0.2f;
            itemFollower.distanceMaxSpeed = 10f;
            itemFollower.targetObject = itemDisplayPrefab;

            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Base", new Vector3(0.50847F, 0.05723F, -0.56729F), new Vector3(271.9725F, 89.99981F, 270.0003F), Vector3.one);
                AddDisplayRule("HuntressBody", "Base", new Vector3(-0.50119F, -0.00672F, -0.56326F), new Vector3(297.3108F, 269.9998F, 90.00016F), Vector3.one);
                AddDisplayRule("Bandit2Body", "Base", new Vector3(-0.26881F, -0.15213F, -0.68005F), new Vector3(337.8547F, 269.9999F, 90.00005F), Vector3.one);
                AddDisplayRule("ToolbotBody", "Base", new Vector3(-3.47283F, 1.51176F, 5.90505F), new Vector3(284.0736F, 269.9998F, 270.0002F), Vector3.one);
                AddDisplayRule("EngiBody", "Base", new Vector3(0.74343F, 0.15695F, -0.69214F), new Vector3(270F, 0F, 0F), Vector3.one);
                AddDisplayRule("EngiTurretBody", "Base", new Vector3(1.74126F, 3.2426F, 0.51312F), new Vector3(0F, 355.2149F, -0.00001F), Vector3.one);
                AddDisplayRule("EngiWalkerTurretBody", "Base", new Vector3(1.39608F, 3.63917F, -0.39318F), new Vector3(0F, 12.29862F, -0.00001F), Vector3.one);
                AddDisplayRule("MageBody", "Base", new Vector3(0.42939F, 0.11568F, -0.58509F), new Vector3(270F, 0F, 0F), Vector3.one);
                AddDisplayRule("MercBody", "Base", new Vector3(0.62629F, 0.00632F, -0.62222F), new Vector3(270F, 0F, 0F), Vector3.one);
                AddDisplayRule("TreebotBody", "Base", new Vector3(1.41955F, 0.20202F, -2.20395F), new Vector3(270F, 0F, 0F), Vector3.one * 1f);
                AddDisplayRule("LoaderBody", "Base", new Vector3(0.69425F, 0.0649F, -0.60318F), new Vector3(270F, 0F, 0F), Vector3.one);
                AddDisplayRule("CrocoBody", "Base", new Vector3(6.1224F, -0.05422F, 4.79566F), new Vector3(90F, 0F, 0F), Vector3.one * 1f);
                AddDisplayRule("CaptainBody", "Base", new Vector3(0.68516F, 0.33544F, -0.82694F), new Vector3(270F, 0F, 0F), Vector3.one);
                AddDisplayRule("BrotherBody", "UpperArmR", BrotherInfection.white, new Vector3(-0.0571F, -0.05567F, -0.0232F), new Vector3(304.7814F, 295.9864F, 82.42023F), new Vector3(0.06672F, 0.02927F, 0.06676F));
                AddDisplayRule("ScavBody", "Base", new Vector3(18.37369F, -1.50871F, 7.59698F), new Vector3(90F, 0F, 0F), Vector3.one * 4f);
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Base", new Vector3(-0.07413F, 1.75623F, -0.58661F), new Vector3(0F, 91.89472F, 0F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("RailgunnerBody", "Base", new Vector3(0.59886F, 0.1738F, -0.39643F), new Vector3(270F, 0F, 0F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("VoidSurvivorBody", "Base", new Vector3(0.92226F, 0.16697F, 0.54787F), new Vector3(68.90422F, 0F, 0F), new Vector3(0.8F, 0.8F, 0.8F));
            };

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            Inventory inventory = sender.inventory;
            if (inventory)
            {
                int itemCount = inventory.GetItemCount(itemDef);
                if (itemCount > 0)
                {
                    args.moveSpeedMultAdd += (moveSpeed + moveSpeedPerStack * (float)(itemCount - 1)) / 100f;
                }
            }
        }


        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            float itemCount = 0;
            bool rootReplacedWithSlow = false;

            c.Emit(OpCodes.Ldarg, 0);
            c.EmitDelegate<System.Action<CharacterBody>>((body) => {
                var inventory = body.inventory;
                if (inventory)
                {
                    itemCount = inventory.GetItemCount(itemDef);
                }
            });

            int locBaseSpeedIndex = -1;
            int locSpeedMultIndex = -1;
            int locSpeedDivIndex = -1;
            bool ILFound = c.TryGotoNext(
                x => x.MatchLdfld<CharacterBody>(nameof(CharacterBody.baseMoveSpeed))
            ) && c.TryGotoNext(
                x => x.MatchLdfld<CharacterBody>(nameof(CharacterBody.levelMoveSpeed))
            ) && c.TryGotoNext(
                x => x.MatchStloc(out locBaseSpeedIndex)
            ) && c.TryGotoNext(
                x => x.MatchLdloc(locBaseSpeedIndex),
                x => x.MatchLdloc(out locSpeedMultIndex),
                x => x.MatchLdloc(out locSpeedDivIndex),
                x => x.MatchDiv(),
                x => x.MatchMul(),
                x => x.MatchStloc(locBaseSpeedIndex)
            ) && c.TryGotoNext(MoveType.After,
                x => x.MatchLdloc(out _),
                x => x.MatchOr(),
                x => x.MatchLdloc(out _),
                x => x.MatchOr()
            );

            if (ILFound)
            {
                c.EmitDelegate<System.Func<bool, bool>>((shouldRoot) => {
                    rootReplacedWithSlow = false;
                    if (itemCount > 0f && shouldRoot)
                    {
                        shouldRoot = false;
                        rootReplacedWithSlow = true;
                    }
                    return shouldRoot;
                });
                c.Emit(OpCodes.Ldarg, 0);
                c.EmitDelegate<System.Action<CharacterBody>>((body) => {
                    if (rootReplacedWithSlow)
                    {
                        var currentRootSlow = initialRootSlow / 100f;
                        if (itemCount > 1f)
                        {
                            currentRootSlow /= rootSlowReductionPerStack / 100f * (itemCount - 1f);
                        }
                        body.moveSpeed /= currentRootSlow;
                    }
                });

                c.GotoPrev(x => x.MatchLdfld<CharacterBody>(nameof(CharacterBody.levelMoveSpeed)));
                c.GotoNext(x => x.MatchStloc(locSpeedDivIndex));
                c.EmitDelegate<System.Func<float, float>>((origSpeedDiv) => {
                    if (itemCount > 0f)
                    {
                        return origSpeedDiv / ((slowReduction + slowReductionPerStack * (itemCount - 1)) / 100f);
                    }
                    return origSpeedDiv;
                });
            }
            else
            {
                Main.logger.LogError("Constant Flow won't work properly");
            }
        }
    }
}
