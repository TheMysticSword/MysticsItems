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

namespace MysticsItems.Items
{
    public class GachaponToken : BaseItem
    {
        public static ConfigurableValue<float> passiveCritBonus = new ConfigurableValue<float>(
            "Item: Gachapon Coin",
            "PassiveCritBonus",
            0.5f,
            "Crit chance (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_GACHAPONTOKEN_DESC"
            }
        );
        public static ConfigurableValue<float> passiveCritBonusPerStack = new ConfigurableValue<float>(
            "Item: Gachapon Coin",
            "PassiveCritBonusPerStack",
            0.5f,
            "Crit chance for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_GACHAPONTOKEN_DESC"
            }
        );
        public static ConfigurableValue<float> passiveAttackSpeedBonus = new ConfigurableValue<float>(
            "Item: Gachapon Coin",
            "PassiveAttackSpeedBonus",
            1f,
            "Attack speed (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_GACHAPONTOKEN_DESC"
            }
        );
        public static ConfigurableValue<float> passiveAttackSpeedBonusPerStack = new ConfigurableValue<float>(
            "Item: Gachapon Coin",
            "PassiveAttackSpeedBonusPerStack",
            1f,
            "Attack speed for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_GACHAPONTOKEN_DESC"
            }
        );

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_GachaponToken";
            SetItemTierWhenAvailable(ItemTier.Tier1);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage,
                ItemTag.InteractableRelated,
                ItemTag.AIBlacklist
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Gachapon Token/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Gachapon Token/Icon.png");
            HopooShaderToMaterial.Standard.Gloss(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial, 0.4f, 5f, new Color32(255, 233, 173, 150));
            // HopooShaderToMaterial.Standard.Gloss(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial, 0.4f, 5f, new Color32(173, 233, 247, 150));
            itemDisplayPrefab = PrepareItemDisplayModel(PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Gachapon Token/FollowerModel.prefab")));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Stomach", new Vector3(-0.08186F, 0.05079F, 0.1678F), new Vector3(324.9999F, 245.9885F, 279.2025F), new Vector3(0.03735F, 0.03735F, 0.03735F));
                AddDisplayRule("HuntressBody", "Stomach", new Vector3(-0.12788F, 0.04712F, 0.14654F), new Vector3(276.4182F, 0.00003F, 325.6896F), new Vector3(0.03684F, 0.03684F, 0.03684F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(-0.13064F, 0.04437F, 0.15777F), new Vector3(83.75449F, 309.2176F, 343.5378F), new Vector3(0.02754F, 0.02754F, 0.02754F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(-2.40704F, 1.7756F, 1.887F), new Vector3(0F, 0F, 90F), new Vector3(0.45775F, 0.45775F, 0.45775F));
                AddDisplayRule("EngiBody", "Stomach", new Vector3(-0.11678F, -0.018F, 0.1798F), new Vector3(82.83242F, 318.2914F, 341.584F), new Vector3(0.04024F, 0.04024F, 0.04024F));
                //AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.8316F, 0.72181F, -0.23478F), new Vector3(323.0401F, 108.2313F, 5.96822F), new Vector3(0.29536F, 0.29536F, 0.29536F));
                //AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.47616F, 1.3804F, -0.4785F), new Vector3(74.07391F, 268.1374F, 180F), new Vector3(0.33715F, 0.41012F, 0.3296F));
                AddDisplayRule("MageBody", "Pelvis", new Vector3(-0.10756F, -0.02973F, -0.1595F), new Vector3(276.6097F, 268.12F, 113.7062F), new Vector3(0.03137F, 0.03137F, 0.03137F));
                AddDisplayRule("MercBody", "ThighL", new Vector3(0.00301F, -0.04705F, 0.19166F), new Vector3(75.6413F, 326.5565F, 301.1101F), new Vector3(0.04367F, 0.04367F, 0.04367F));
                AddDisplayRule("TreebotBody", "HeadCenter", new Vector3(0F, 0.04031F, 0F), new Vector3(0F, 0F, 0F), new Vector3(0.06867F, 0.06867F, 0.06867F));
                AddDisplayRule("LoaderBody", "Pelvis", new Vector3(-0.10359F, 0.07784F, -0.18451F), new Vector3(80.79361F, 0F, 0F), new Vector3(0.03828F, 0.03828F, 0.03828F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(1.51614F, 1.86602F, 0.39052F), new Vector3(7.85445F, 337.2328F, 288.036F), new Vector3(0.17627F, 0.17743F, 0.17627F));
                AddDisplayRule("CaptainBody", "Chest", new Vector3(0.16838F, 0.25327F, 0.15011F), new Vector3(284.663F, 240.0974F, 150.5083F), new Vector3(0.03817F, 0.03817F, 0.03817F));
                AddDisplayRule("BrotherBody", "Stomach", new Vector3(-0.12494F, 0.15612F, 0.07713F), new Vector3(0F, 48.36884F, 0F), new Vector3(0.03742F, 0.03184F, 0.03184F));
                //AddDisplayRule("ScavBody", "MuzzleEnergyCannon", new Vector3(-3.88535F, -0.90743F, -18.53646F), new Vector3(16.92252F, 288.3049F, 72.11835F), new Vector3(2.62999F, 2.70243F, 2.62999F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Pelvis", new Vector3(0.08531F, 0.20379F, 0.19368F), new Vector3(84.48428F, -0.00001F, 174.7452F), new Vector3(0.0399F, 0.0399F, 0.0399F));
                AddDisplayRule("RailgunnerBody", "Pelvis", new Vector3(0.10035F, 0.06732F, -0.09966F), new Vector3(68.92004F, 321.2874F, 354.5732F), new Vector3(0.04588F, 0.04588F, 0.04588F));
                AddDisplayRule("VoidSurvivorBody", "Head", new Vector3(-0.09242F, -0.05035F, -0.11544F), new Vector3(317.6919F, 150.0517F, 240.3664F), new Vector3(0.03197F, 0.03197F, 0.03285F));
            };

            ShrineChanceBehavior.onShrineChancePurchaseGlobal += ShrineChanceBehavior_onShrineChancePurchaseGlobal;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void ShrineChanceBehavior_onShrineChancePurchaseGlobal(bool failed, Interactor interactor)
        {
            var body = interactor.GetComponent<CharacterBody>();
            if (body && body.inventory)
            {
                var itemCount = body.inventory.GetItemCount(itemDef);
                if (itemCount > 0)
                {
                    body.AddBuff(MysticsItemsContent.Buffs.MysticsItems_GachaponBonus);
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
                    args.critAdd += passiveCritBonus + passiveCritBonusPerStack * (float)(itemCount - 1);
                    args.attackSpeedMultAdd += (passiveAttackSpeedBonus + passiveAttackSpeedBonusPerStack * (float)(itemCount - 1)) / 100f;
                }
            }
        }
    }
}
