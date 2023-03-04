using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using RoR2.Audio;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using MonoMod.Cil;
using static MysticsItems.LegacyBalanceConfigManager;
using Mono.Cecil.Cil;

namespace MysticsItems.Items
{
    public class SpeedGivesDamage : BaseItem
    {
        public static ConfigurableValue<float> passiveSpeed = new ConfigurableValue<float>(
            "Item: Nuclear Accelerator",
            "PassiveSpeed",
            10f,
            "Movement speed increase from the first stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_SPEEDGIVESDAMAGE_DESC"
            }
        );
        public static ConfigurableValue<float> damage = new ConfigurableValue<float>(
            "Item: Nuclear Accelerator",
            "Damage",
            0.25f,
            "Damage increase for every 1% of speed increase (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_SPEEDGIVESDAMAGE_DESC"
            }
        );
        public static ConfigurableValue<float> damagePerStack = new ConfigurableValue<float>(
            "Item: Nuclear Accelerator",
            "DamagePerStack",
            0.25f,
            "Damage increase for every 1% of speed increase for every additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_SPEEDGIVESDAMAGE_DESC"
            }
        );
        public static ConfigurableValue<bool> sprintCounts = new ConfigurableValue<bool>(
            "Item: Nuclear Accelerator",
            "Sprint Counts",
            false,
            "If true, sprint speed multiplier (x1.45 by default) also increases damage"
        );
        public static ConfigOptions.ConfigurableValue<bool> alternate = ConfigOptions.ConfigurableValue.CreateBool(
            ConfigManager.General.categoryGUID,
            ConfigManager.General.categoryName,
            ConfigManager.General.config,
            "Gameplay",
            "Nuclear Accelerator Alternate",
            false,
            "Should the Nuclear Accelerator item give the bonus depending on current velocity instead of current movespeed buff?"
        );

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_SpeedGivesDamage";
            SetItemTierWhenAvailable(ItemTier.Tier2);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage,
                ItemTag.Utility
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Nuclear Accelerator/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Nuclear Accelerator/Icon.png");
            itemDisplayPrefab = PrepareItemDisplayModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Nuclear Accelerator/ItemDisplayModel.prefab"));
            void ApplyToModel(GameObject model)
            {
                HopooShaderToMaterial.Standard.Apply(model.GetComponentInChildren<Renderer>().sharedMaterial);
                HopooShaderToMaterial.Standard.Emission(model.GetComponentInChildren<Renderer>().sharedMaterial);
            }
            ApplyToModel(itemDef.pickupModelPrefab);
            ApplyToModel(itemDisplayPrefab);
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "ThighL", new Vector3(0.06317F, 0.35321F, -0.00313F), new Vector3(279.3661F, 188.532F, 65.6066F), new Vector3(0.04149F, 0.04149F, 0.04149F));
                AddDisplayRule("HuntressBody", "ThighR", new Vector3(-0.10573F, 0.20407F, 0.05449F), new Vector3(88.68747F, 59.52037F, 158.7484F), new Vector3(0.04791F, 0.04791F, 0.04791F));
                AddDisplayRule("Bandit2Body", "ThighR", new Vector3(-0.07541F, 0.27521F, 0.00958F), new Vector3(84.7448F, 48.91237F, 137.3522F), new Vector3(0.04852F, 0.04852F, 0.04852F));
                AddDisplayRule("ToolbotBody", "ThighR", new Vector3(0.0691F, 1.59033F, 0.64486F), new Vector3(85.93443F, 273.296F, 271.2461F), new Vector3(0.495F, 0.495F, 0.495F));
                AddDisplayRule("EngiBody", "CannonHeadR", new Vector3(-0.19011F, 0.28565F, -0.04556F), new Vector3(271.6535F, 281.4757F, 170.0434F), new Vector3(0.05734F, 0.20054F, 0.05734F));
                AddDisplayRule("EngiTurretBody", "Neck", new Vector3(0.00631F, 0.09396F, -0.17715F), new Vector3(270.0198F, 3.60967F, 0F), new Vector3(0.15454F, 0.26937F, 0.211F));
                AddDisplayRule("EngiWalkerTurretBody", "Neck", new Vector3(0F, 0.1756F, -0.18462F), new Vector3(270F, 0F, 0F), new Vector3(0.15894F, 0.30337F, 0.211F));
                AddDisplayRule("MageBody", "Chest", new Vector3(0.115F, 0.06328F, -0.32405F), new Vector3(79.85654F, 180F, 0F), new Vector3(0.0335F, 0.047F, 0.047F));
                AddDisplayRule("MageBody", "Chest", new Vector3(-0.11264F, 0.06343F, -0.32408F), new Vector3(80.6522F, 180F, 0F), new Vector3(0.0335F, 0.047F, 0.047F));
                AddDisplayRule("MercBody", "ThighR", new Vector3(-0.03369F, 0.3048F, 0.13015F), new Vector3(81.77126F, 23.8203F, 40.4371F), new Vector3(0.04351F, 0.05609F, 0.04931F));
                AddDisplayRule("TreebotBody", "PlatformBase", new Vector3(-0.48384F, -0.32524F, -0.00001F), new Vector3(0F, 0F, 121.4299F), new Vector3(0.12147F, 0.31448F, 0.12147F));
                AddDisplayRule("LoaderBody", "MechLowerArmR", new Vector3(-0.00948F, 0.50984F, -0.09007F), new Vector3(275.6503F, 103.8318F, 260.0916F), new Vector3(0.04906F, 0.08035F, 0.04684F));
                AddDisplayRule("CrocoBody", "ThighR", new Vector3(-1.12653F, 1.01827F, 0.15804F), new Vector3(278.8466F, 0F, 90F), new Vector3(0.46178F, 0.731F, 0.5194F));
                AddDisplayRule("CaptainBody", "ThighR", new Vector3(0F, 0.32372F, 0.13409F), new Vector3(81.04952F, 0F, 0F), new Vector3(0.045F, 0.05004F, 0.04977F));
                AddDisplayRule("BrotherBody", "CalfR", BrotherInfection.green, new Vector3(0.038F, 0.121F, 0.051F), new Vector3(43.102F, 358.401F, 241.259F), new Vector3(0.078F, 0.078F, 0.078F));
                AddDisplayRule("ScavBody", "ThighR", new Vector3(-1.84683F, 1.59361F, 0.91504F), new Vector3(57.83196F, 338.4957F, 56.00451F), new Vector3(0.99F, 1.01489F, 0.99F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "ThighR", new Vector3(-0.01739F, 0.46343F, 0.11342F), new Vector3(82.73656F, 358.8853F, 0F), new Vector3(0.05528F, 0.10105F, 0.05528F));
                AddDisplayRule("RailgunnerBody", "BottomRail", new Vector3(0.00021F, 0.57204F, -0.03453F), new Vector3(270F, 0F, 0F), new Vector3(0.05084F, 0.05084F, 0.05084F));
                AddDisplayRule("VoidSurvivorBody", "CannonEnd", new Vector3(0.22525F, -0.20491F, -0.06686F), new Vector3(81.33131F, 272.2093F, 154.0057F), new Vector3(0.08941F, 0.15972F, 0.08941F));
            };

            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            int locBaseDamageIndex = -1;
            int locDamageMultIndex = -1;
            bool ILFound = c.TryGotoNext(
                x => x.MatchLdfld<CharacterBody>(nameof(CharacterBody.baseDamage))
            ) && c.TryGotoNext(
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<CharacterBody>(nameof(CharacterBody.levelDamage))
            ) && c.TryGotoNext(
                x => x.MatchStloc(out locBaseDamageIndex)
            ) && c.TryGotoNext(
                x => x.MatchLdloc(locBaseDamageIndex),
                x => x.MatchLdloc(out locDamageMultIndex),
                x => x.MatchMul()
            ) && c.TryGotoNext(
                x => x.MatchStloc(locBaseDamageIndex)
            );

            if (ILFound)
            {
                c.GotoPrev(x => x.MatchLdfld<CharacterBody>(nameof(CharacterBody.baseDamage)));
                c.GotoNext(x => x.MatchStloc(locDamageMultIndex));
                c.Emit(OpCodes.Ldarg, 0);
                c.EmitDelegate<System.Func<float, CharacterBody, float>>((origDamageMult, body) => {
                    var newDamageMult = origDamageMult;
                    if (body.inventory && !alternate)
                    {
                        int itemCount = body.inventory.GetItemCount(itemDef);
                        if (itemCount > 0)
                        {
                            newDamageMult += CalculateDamageBonus(body, itemCount);
                        }
                    }
                    return newDamageMult;
                });
            }
            else
            {
                Main.logger.LogError("Nuclear Accelerator won't work");
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                int itemCount = sender.inventory.GetItemCount(itemDef);
                if (itemCount > 0)
                {
                    args.moveSpeedMultAdd += passiveSpeed / 100f;
                }
            }
        }

        public static float CalculateDamageBonus(CharacterBody body, int itemCount)
        {
            var baseMoveSpeed = body.baseMoveSpeed + body.levelMoveSpeed * (body.level - 1f);
            
            if (alternate)
            {
                var currentVelocity = (body.characterMotor ? body.characterMotor.velocity : (body.rigidbody ? body.rigidbody.velocity : Vector3.zero)).magnitude;
                var velocityDifference = (currentVelocity / baseMoveSpeed / ((!sprintCounts && body.isSprinting) ? body.sprintingSpeedMultiplier : 1f)) - 1f;
                return Mathf.Max(velocityDifference * (damage + damagePerStack * (itemCount - 1)), 0f);
            }

            var moveSpeedIncrease = (body.moveSpeed / baseMoveSpeed / ((!sprintCounts && body.isSprinting) ? body.sprintingSpeedMultiplier : 1f)) - 1f;
            return Mathf.Max(moveSpeedIncrease * (damage + damagePerStack * (itemCount - 1)), 0f);
        }
    }
}
