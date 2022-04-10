using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using RoR2;
using System.Linq;
using UnityEngine;

namespace MysticsItems.Buffs
{
    public class MarwanAshBurn : BaseBuff
    {
        public static DamageColorIndex ashDoTDamageColor = DamageColorAPI.RegisterDamageColor(new Color32(1, 167, 172, 255));
        public static DotController.DotDef ashDotDef;
        public static DotController.DotIndex ashDotIndex;
        public static BurnEffectController.EffectParams ashBurnEffectParams;

        public override void OnLoad() {
            buffDef.name = "MysticsItems_MarwanAshBurn";
            buffDef.buffColor = new Color32(96, 245, 250, 255);
            buffDef.canStack = false;
            buffDef.isDebuff = false;
            buffDef.iconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Buffs/MarwanAshBurn.png");

            ashBurnEffectParams = new BurnEffectController.EffectParams
            {
                startSound = "Play_item_proc_igniteOnKill_Loop",
                stopSound = "Stop_item_proc_igniteOnKill_Loop",
                overlayMaterial = Main.AssetBundle.LoadAsset<Material>("Assets/Items/Marwan's Ash/matMarwanAshBurnOverlay.mat"),
                fireEffectPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Marwan's Ash/AshBurnVFX.prefab")
            };

            ashDotDef = new DotController.DotDef
            {
                associatedBuff = buffDef,
                damageCoefficient = 1f,
                damageColorIndex = ashDoTDamageColor,
                interval = 0.333f
            };
            ashDotIndex = DotAPI.RegisterDotDef(ashDotDef, (self, dotStack) =>
            {
                DotController.DotStack oldDotStack = self.dotStackList.FirstOrDefault(x => x.dotIndex == dotStack.dotIndex);
                if (oldDotStack != null)
                {
                    self.RemoveDotStackAtServer(self.dotStackList.IndexOf(oldDotStack));
                }

                var itemCount = 1;
                var attackerLevel = 1f;
                var damageMultiplier = 1f;
                var isPlayerTeam = false;
                if (dotStack.attackerObject)
                {
                    var ashHelper = dotStack.attackerObject.GetComponent<Items.MarwanAsh1.MysticsItemsMarwanAshHelper>();
                    if (ashHelper) itemCount = ashHelper.itemCount;

                    var attackerBody = dotStack.attackerObject.GetComponent<CharacterBody>();
                    if (attackerBody)
                    {
                        attackerLevel = attackerBody.level;
                        if (attackerBody.damage != 0f)
                            damageMultiplier = dotStack.damage / attackerBody.damage / ashDotDef.damageCoefficient;
                        isPlayerTeam = attackerBody.teamComponent.teamIndex == TeamIndex.Player;
                    }
                }
                var hpFractionDamage = (Items.MarwanAsh1.dotPercent / 100f + Items.MarwanAsh1.dotPercentPerLevel / 100f * (attackerLevel - (float)Items.MarwanAsh1.upgradeLevel12) * itemCount) * damageMultiplier;
                if (!isPlayerTeam) hpFractionDamage = Mathf.Min(hpFractionDamage, Items.MarwanAsh1.enemyBurnDamageCap);
                dotStack.damage = (self.victimHealthComponent ? self.victimHealthComponent.fullCombinedHealth * hpFractionDamage : 0) * ashDotDef.interval;
            });

            On.RoR2.DotController.UpdateDotVisuals += DotController_UpdateDotVisuals;
        }

        private void DotController_UpdateDotVisuals(On.RoR2.DotController.orig_UpdateDotVisuals orig, DotController self)
        {
            orig(self);
            Items.MarwanAsh1.MysticsItemsMarwanAshHelper ashHelper = self.victimBody ? self.victimBody.GetComponent<Items.MarwanAsh1.MysticsItemsMarwanAshHelper>() : null;
            if (ashHelper)
            {
                if (self.HasDotActive(ashDotIndex))
                {
                    if (!ashHelper.burnEffectController)
                    {
                        ModelLocator modelLocator = self.victimBody.modelLocator;
                        if (modelLocator && modelLocator.modelTransform)
                        {
                            ashHelper.burnEffectController = self.victimBody.gameObject.AddComponent<BurnEffectController>();
                            ashHelper.burnEffectController.effectType = ashBurnEffectParams;
                            ashHelper.burnEffectController.target = modelLocator.modelTransform.gameObject;
                        }
                    }
                }
                else if (ashHelper.burnEffectController)
                {
                    Object.Destroy(ashHelper.burnEffectController);
                    ashHelper.burnEffectController = null;
                }
            }
        }
    }
}
