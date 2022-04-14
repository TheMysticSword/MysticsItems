using RoR2;
using R2API.Utils;
using UnityEngine;
using System;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.BalanceConfigManager;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace MysticsItems.Items
{
    public class Backpack : BaseItem
    {
        public static ConfigurableValue<int> charges = new ConfigurableValue<int>(
            "Item: Hiker s Backpack",
            "Charges",
            1,
            "Additional charges to all skills",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_BACKPACK_PICKUP",
                "ITEM_MYSTICSITEMS_BACKPACK_DESC"
            }
        );
        public static ConfigurableValue<int> chargesPerStack = new ConfigurableValue<int>(
            "Item: Hiker s Backpack",
            "ChargesPerStack",
            1,
            "Additional charges to all skills for each additional stack of this item",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_BACKPACK_DESC"
            }
        );
        public static ConfigurableValue<float> cdr = new ConfigurableValue<float>(
            "Item: Hiker s Backpack",
            "CDR",
            8f,
            "Cooldown reduction to all skills for the first stack of this item",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_BACKPACK_DESC"
            }
        );
        public static ConfigurableValue<bool> increaseEngiTurretLimit = new ConfigurableValue<bool>(
            "Item: Hiker s Backpack",
            "IncreaseEngiTurretLimit",
            true,
            "Should the item increase the limit on Engineer turrets that you can place?"
        );

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_Backpack";
            itemDef.tier = ItemTier.Tier3;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility
            };
            MysticsItemsContent.Resources.unlockableDefs.Add(GetUnlockableDef());
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Backpack/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Backpack/Icon.png");

            var mat = itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial;
            HopooShaderToMaterial.Standard.Apply(mat);
            HopooShaderToMaterial.Standard.DisableEverything(mat);
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            //mat.SetFloat("_NormalStrength", 0f);
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Chest", new Vector3(0.00145F, 0.1424F, -0.2211F), new Vector3(0F, 180F, 0F), new Vector3(0.28214F, 0.28214F, 0.28214F));
                AddDisplayRule("HuntressBody", "Chest", new Vector3(0.00002F, 0.06688F, -0.1311F), new Vector3(0F, 180F, 0F), new Vector3(0.23527F, 0.21892F, 0.23527F));
                AddDisplayRule("Bandit2Body", "Chest", new Vector3(0.00001F, 0.1205F, -0.19442F), new Vector3(0F, 180F, 0F), new Vector3(0.26903F, 0.26903F, 0.26903F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(0.00002F, 0.80955F, -3.0003F), new Vector3(0F, 180F, 0F), new Vector3(1.91497F, 2.05923F, 1.91497F));
                AddDisplayRule("EngiBody", "Chest", new Vector3(0F, 0.22328F, -0.30179F), new Vector3(0F, 180F, 0F), new Vector3(0.20968F, 0.20968F, 0.20968F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.27521F, 0.12166F, 1.14743F), new Vector3(85.1495F, 160.0622F, 70.12805F), new Vector3(0.55429F, 0.55429F, 0.55429F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0F, 0.28722F, -1.25043F), new Vector3(43.3701F, 180F, 0F), new Vector3(0.57174F, 0.69535F, 0.55769F));
                AddDisplayRule("MageBody", "Chest", new Vector3(0F, 0.06311F, -0.36474F), new Vector3(350.1945F, 180F, 0F), new Vector3(0.149F, 0.149F, 0.149F));
                AddDisplayRule("MercBody", "Chest", new Vector3(0F, -0.05041F, -0.25515F), new Vector3(0F, 180F, 0F), new Vector3(0.32494F, 0.32494F, 0.32494F));
                AddDisplayRule("TreebotBody", "PlatformBase", new Vector3(-0.3864F, 1.18414F, -0.80142F), new Vector3(38.11974F, 196.571F, 5.81885F), new Vector3(0.23823F, 0.23823F, 0.23823F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(0F, 0.11231F, -0.2115F), new Vector3(0F, 180F, 0F), new Vector3(0.20914F, 0.20914F, 0.20914F));
                AddDisplayRule("CrocoBody", "SpineChest3", new Vector3(0F, 0F, 0F), new Vector3(288.0211F, 180.0001F, 180.0788F), new Vector3(2.34323F, 2.34323F, 2.34323F));
                AddDisplayRule("CaptainBody", "Chest", new Vector3(-0.05223F, 0.02899F, -0.26799F), new Vector3(0F, 185.0811F, 0F), new Vector3(0.40041F, 0.40041F, 0.40041F));
                AddDisplayRule("BrotherBody", "chest", BrotherInfection.red, new Vector3(0F, 0.21823F, -0.11272F), new Vector3(0F, 240.0996F, 0F), new Vector3(0.125F, 0.125F, 0.125F));
                AddDisplayRule("ScavBody", "Backpack", new Vector3(0F, 3.81932F, -0.17169F), new Vector3(0F, 180F, 0F), new Vector3(9.43553F, 9.03975F, 10.77423F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Chest", new Vector3(0.00101F, 0.06171F, -0.23013F), new Vector3(6.80454F, 180F, 0F), new Vector3(0.33888F, 0.33888F, 0.33888F));
                AddDisplayRule("RailgunnerBody", "Backpack", new Vector3(-0.0405F, -0.23093F, -0.02859F), new Vector3(359.9882F, 182.3322F, 0.29078F), new Vector3(0.38543F, 0.444F, 0.3482F));
                AddDisplayRule("VoidSurvivorBody", "Chest", new Vector3(0.00001F, -0.02248F, -0.19335F), new Vector3(24F, 180F, 0F), new Vector3(0.278F, 0.278F, 0.278F));
            };

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;

            /*
            On.RoR2.Skills.SkillCatalog.SetSkillDefs += SkillCatalog_SetSkillDefs;
            On.EntityStates.GenericCharacterMain.PerformInputs += GenericCharacterMain_PerformInputs;
            */
        }

        private int CharacterMaster_GetDeployableSameSlotLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            var result = orig(self, slot);
            if (slot == DeployableSlot.EngiTurret && increaseEngiTurretLimit)
            {
                var itemCount = self.inventory.GetItemCount(itemDef);
                if (itemCount > 0) result += charges + chargesPerStack * (itemCount - 1);
            }
            return result;
        }

        /*
        private void GenericCharacterMain_PerformInputs(On.EntityStates.GenericCharacterMain.orig_PerformInputs orig, EntityStates.GenericCharacterMain self)
        {
            var hasThisItem = GeneralConfigManager.backpackEnableSkillFixes.Value && self.characterBody && self.characterBody.inventory && self.characterBody.inventory.GetItemCount(itemDef) > 0;
            if (hasThisItem)
            {
                for (var i = 0; i < skillDefsToFix.Count; i++)
                {
                    var kvp = skillDefsToFix.ElementAt(i);
                    skillDefsToFix[kvp.Key] = kvp.Key.mustKeyPress;
                    kvp.Key.mustKeyPress = true;
                }
            }
            orig(self);
            if (hasThisItem)
            {
                for (var i = 0; i < skillDefsToFix.Count; i++)
                {
                    var kvp = skillDefsToFix.ElementAt(i);
                    kvp.Key.mustKeyPress = skillDefsToFix[kvp.Key];
                }
            }
        }

        public static Dictionary<RoR2.Skills.SkillDef, bool> skillDefsToFix = new Dictionary<RoR2.Skills.SkillDef, bool>();

        private void SkillCatalog_SetSkillDefs(On.RoR2.Skills.SkillCatalog.orig_SetSkillDefs orig, RoR2.Skills.SkillDef[] newSkillDefs)
        {
            orig(newSkillDefs);
            if (GeneralConfigManager.backpackEnableSkillFixes.Value)
            {
                var skillsToFix = new List<string>()
                {
                    "ResetRevolver", "SkullRevolver",
                    "ThrowGrenade", "ThrowStickyGrenade", "CommandoBodySweepBarrage", "CommandoBodyBarrage",
                    "CrocoDisease",
                    "HuntressBodyArrowRain", "FireArrowSnipe", "AimArrowSnipe",
                    "MageBodyFlyUp", "MageBodyFlamethrower",
                    "MercBodyEvisProjectile", "MercBodyEvis",
                    "ToolbotDualWield", "ToolbotCancelDualWield",
                    "LunarDetonatorSpecialReplacement"
                };
                foreach (var skillDef in newSkillDefs)
                {
                    if (skillsToFix.Contains(RoR2.Skills.SkillCatalog.GetSkillName(skillDef.skillIndex)) && !skillDef.mustKeyPress)
                    {
                        if (!skillDefsToFix.ContainsKey(skillDef))
                            skillDefsToFix.Add(skillDef, false);
                    }
                }
            }
        }
        */

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                int itemCount = sender.inventory.GetItemCount(itemDef);
                if (itemCount > 0)
                {
                    args.cooldownMultAdd -= cdr / 100f;
                }

                var skills = new GenericSkill[]
                {
                    sender.skillLocator.primary,
                    sender.skillLocator.secondary,
                    sender.skillLocator.utility,
                    sender.skillLocator.special
                };
                foreach (var skill in skills)
                {
                    if (skill)
                        skill.SetBonusStockFromBody(0);
                }
            }
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (self.inventory)
            {
                int itemCount = self.inventory.GetItemCount(itemDef);
                var extraCharges = itemCount > 0 ? charges + chargesPerStack * (itemCount - 1) : 0;

                var skills = new GenericSkill[]
                {
                    self.skillLocator.primary,
                    self.skillLocator.secondary,
                    self.skillLocator.utility,
                    self.skillLocator.special
                };
                foreach (var skill in skills)
                {
                    if (skill)
                        skill.SetBonusStockFromBody(skill.bonusStockFromBody + extraCharges);
                }
            }
        }

    }
}
