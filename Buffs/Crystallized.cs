using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils;
using RoR2.Skills;
using System.Collections.Generic;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;

namespace MysticsItems.Buffs
{
    public class Crystallized : BaseBuff
    {
        public override void OnLoad() {
            buffDef.name = "MysticsItems_Crystallized";
            buffDef.buffColor = new Color(130f / 255f, 130f / 255f, 130f / 255f);
            buffDef.canStack = false;
            buffDef.iconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Buffs/Crystallized.png");

            SkillDef skillDef = ScriptableObject.CreateInstance<SkillDef>();
            skillDef.skillName = "MysticsItems_Crystallized";
            skillDef.skillNameToken = "SKILL_MYSTICSITEMS_CRYSTALLIZED_NAME";
            skillDef.skillDescriptionToken = "SKILL_MYSTICSITEMS_CRYSTALLIZED_DESCRIPTION";
            skillDef.icon = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Crystal World/texSkillIconCrystallized.png");
            skillDef.activationStateMachineName = "Weapon";
            skillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            skillDef.interruptPriority = EntityStates.InterruptPriority.Any;
            skillDef.baseRechargeInterval = 0f;
            skillDef.baseMaxStock = 0;
            skillDef.rechargeStock = 0;
            skillDef.requiredStock = 0;
            skillDef.stockToConsume = 0;
            skillDef.resetCooldownTimerOnUse = false;
            skillDef.fullRestockOnAssign = false;
            skillDef.dontAllowPastMaxStocks = false;
            skillDef.beginSkillCooldownOnSkillEnd = false;
            skillDef.cancelSprintingOnActivation = false;
            skillDef.forceSprintDuringState = false;
            skillDef.canceledFromSprinting = false;
            skillDef.isCombatSkill = false;
            skillDef.mustKeyPress = true;

            MysticsItemsContent.Resources.skillDefs.Add(skillDef);

            On.RoR2.CharacterBody.OnClientBuffsChanged += (orig, self) =>
            {
                orig(self);
                if (self.skillLocator)
                {
                    if (self.HasBuff(buffDef))
                    {
                        if (self.skillLocator.primary) self.skillLocator.primary.SetSkillOverride(self, skillDef, GenericSkill.SkillOverridePriority.Replacement);
                        if (self.skillLocator.secondary) self.skillLocator.secondary.SetSkillOverride(self, skillDef, GenericSkill.SkillOverridePriority.Replacement);
                        if (self.skillLocator.utility) self.skillLocator.utility.SetSkillOverride(self, skillDef, GenericSkill.SkillOverridePriority.Replacement);
                        if (self.skillLocator.special) self.skillLocator.special.SetSkillOverride(self, skillDef, GenericSkill.SkillOverridePriority.Replacement);
                    }
                    else
                    {
                        if (self.skillLocator.primary) self.skillLocator.primary.UnsetSkillOverride(self, skillDef, GenericSkill.SkillOverridePriority.Replacement);
                        if (self.skillLocator.secondary) self.skillLocator.secondary.UnsetSkillOverride(self, skillDef, GenericSkill.SkillOverridePriority.Replacement);
                        if (self.skillLocator.utility) self.skillLocator.utility.UnsetSkillOverride(self, skillDef, GenericSkill.SkillOverridePriority.Replacement);
                        if (self.skillLocator.special) self.skillLocator.special.UnsetSkillOverride(self, skillDef, GenericSkill.SkillOverridePriority.Replacement);
                    }
                }
            };

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;

            Overlays.CreateOverlay(Main.AssetBundle.LoadAsset<Material>("Assets/Items/Crystal World/matCrystallized.mat"), (model) =>
            {
                return model.body ? model.body.HasBuff(buffDef) : false;
            });

            GenericGameEvents.BeforeTakeDamage += (damageInfo, attackerInfo, victimInfo) =>
            {
                if (!damageInfo.rejected && attackerInfo.body && attackerInfo.body.HasBuff(buffDef)) damageInfo.rejected = true;
            };
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(buffDef))
            {
                args.moveSpeedRootCount++;
            }
        }
    }
}
