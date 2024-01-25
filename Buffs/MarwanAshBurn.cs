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
                fireEffectPrefab = null
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
                var damageMultiplier = 1f;
                var attackerDamage = 1f;
                if (dotStack.attackerObject)
                {
                    var attackerBody = dotStack.attackerObject.GetComponent<CharacterBody>();
                    if (attackerBody)
                    {
                        attackerDamage = attackerBody.damage;
                        if (attackerDamage != 0f) damageMultiplier = dotStack.damage / attackerDamage;
                    }
                }
                if (self.victimHealthComponent)
                    dotStack.damage = Mathf.Min(self.victimHealthComponent.fullCombinedHealth * damageMultiplier, attackerDamage * Items.MarwanAsh1.burnDamageMultiplierCap / 100f);
                else
                    dotStack.damage = 0;
                dotStack.damage *= ashDotDef.interval;
            });
        }
    }
}
