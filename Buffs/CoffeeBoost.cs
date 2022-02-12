using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.BalanceConfigManager;

namespace MysticsItems.Buffs
{
    public class CoffeeBoost : BaseBuff
    {
        public static ConfigurableValue<float> boostPower = new ConfigurableValue<float>(
            "Item: Cup of Expresso",
            "BoostPower",
            7f,
            "Movement speed and attack speed increase per Express Boost stack (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_COFFEEBOOSTONITEMPICKUP_DESC"
            }
        );
        
        public override void OnLoad() {
            buffDef.name = "MysticsItems_CoffeeBoost";
            buffDef.buffColor = new Color32(96, 215, 229, 225);
            buffDef.canStack = true;
            buffDef.iconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Buffs/CoffeeBoost.png");
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(buffDef))
            {
                args.moveSpeedMultAdd += boostPower / 100f * sender.GetBuffCount(buffDef);
                args.attackSpeedMultAdd += boostPower / 100f * sender.GetBuffCount(buffDef);
            }
        }
    }
}
