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
            "Item: Hikers Backpack",
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
            "Item: Hikers Backpack",
            "ChargesPerStack",
            1,
            "Additional charges to all skills for each additional stack of this item",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_BACKPACK_DESC"
            }
        );
        public static ConfigurableValue<float> cdr = new ConfigurableValue<float>(
            "Item: Hikers Backpack",
            "CDR",
            8f,
            "Cooldown reduction to all skills for the first stack of this item",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_BACKPACK_DESC"
            }
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
            //itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Thought Processor/Model.prefab"));
            //itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Thought Processor/Icon.png");
            
            //itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            /*
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Head", new Vector3(0F, 0.5F, 0.023F), new Vector3(0F, 270F, 180F), new Vector3(0.2F, 0.2F, 0.2F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(-0.001F, 0.395F, -0.076F), new Vector3(359.536F, 90.36F, 165.383F), new Vector3(0.187F, 0.174F, 0.187F));
                AddDisplayRule("Bandit2Body", "Hat", new Vector3(0.008F, 0.072F, -0.029F), new Vector3(0F, 254.385F, 200.576F), new Vector3(0.137F, 0.137F, 0.137F));
                AddDisplayRule("ToolbotBody", "HandR", new Vector3(0.338F, 1.246F, -0.155F), new Vector3(356.736F, 85.148F, 3.991F), new Vector3(1.5F, 1.613F, 1.5F));
                AddDisplayRule("EngiBody", "HeadCenter", new Vector3(0.001F, 0.3F, 0.022F), new Vector3(359.991F, 272.116F, 173.922F), new Vector3(0.216F, 0.216F, 0.216F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(-0.007F, 0.574F, -0.307F), new Vector3(356.042F, 91.028F, 94.927F), new Vector3(0.133F, 0.133F, 0.133F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(-0.024F, 0.774F, -0.23F), new Vector3(0.234F, 271.606F, 270.571F), new Vector3(0.407F, 0.495F, 0.397F));
                AddDisplayRule("MageBody", "Head", new Vector3(0.003F, 0.232F, -0.181F), new Vector3(1.434F, 264.707F, 230.524F), new Vector3(0.149F, 0.149F, 0.149F));
                AddDisplayRule("MercBody", "Head", new Vector3(0F, 0.315F, 0.116F), new Vector3(1.129F, 273.826F, 158.027F), new Vector3(0.16F, 0.16F, 0.16F));
                AddDisplayRule("TreebotBody", "PlatformBase", new Vector3(0.466F, 0.259F, 0.204F), new Vector3(0F, 143.502F, 0F), new Vector3(0.083F, 0.083F, 0.083F));
                AddDisplayRule("LoaderBody", "Head", new Vector3(0F, 0.291F, 0.019F), new Vector3(0.389F, 270F, 180F), new Vector3(0.171F, 0.171F, 0.171F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(-0.125F, 0.241F, 2.181F), new Vector3(0F, 90F, 290F), new Vector3(1.271F, 0.895F, 1.408F));
                AddDisplayRule("CaptainBody", "Stomach", new Vector3(0.001F, 0.276F, 0.016F), new Vector3(0.318F, 257.881F, 182.808F), new Vector3(0.158F, 0.158F, 0.158F));
                AddDisplayRule("BrotherBody", "Head", BrotherInfection.red, new Vector3(0.011F, 0.129F, 0.071F), new Vector3(28.594F, 22.166F, 285.147F), new Vector3(0.125F, 0.125F, 0.125F));
                AddDisplayRule("ScavBody", "MuzzleEnergyCannon", new Vector3(0F, 0.001F, -22.578F), new Vector3(0F, 270F, 90F), new Vector3(1.733F, 1.733F, 1.733F));
            };
            */

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;

            On.RoR2.Skills.SkillCatalog.SetSkillDefs += SkillCatalog_SetSkillDefs;
            On.EntityStates.GenericCharacterMain.PerformInputs += GenericCharacterMain_PerformInputs;
        }

        private void GenericCharacterMain_PerformInputs(On.EntityStates.GenericCharacterMain.orig_PerformInputs orig, EntityStates.GenericCharacterMain self)
        {
            var hasThisItem = self.characterBody && self.characterBody.inventory && self.characterBody.inventory.GetItemCount(itemDef) > 0;
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

        public static ConfigEntry<bool> enableSkillFixes = Main.configGeneral.Bind<bool>(
            "Vanilla changes",
            "BackpackEnableSkillFixes",
            true,
            "Make certain skills require pressing a key instead of holding it down while carrying the Hikers Backpack item to fix these skills consuming all charges at once."
        );
        public static Dictionary<RoR2.Skills.SkillDef, bool> skillDefsToFix = new Dictionary<RoR2.Skills.SkillDef, bool>();

        private void SkillCatalog_SetSkillDefs(On.RoR2.Skills.SkillCatalog.orig_SetSkillDefs orig, RoR2.Skills.SkillDef[] newSkillDefs)
        {
            orig(newSkillDefs);
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
                    skillDefsToFix.Add(skillDef, false);
            }
        }

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
