using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.LegacyBalanceConfigManager;
using UnityEngine.AddressableAssets;

namespace MysticsItems.Buffs
{
    public class GachaponBonus : BaseBuff
    {
        public static ConfigurableValue<float> critBonus = new ConfigurableValue<float>(
            "Item: Gachapon Coin",
            "CritBonus",
            3f,
            "Crit chance increase per Shrine of Chance activation (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_GACHAPONTOKEN_DESC"
            }
        );
        public static ConfigurableValue<float> critBonusPerStack = new ConfigurableValue<float>(
            "Item: Gachapon Coin",
            "CritBonusPerStack",
            3f,
            "Crit chance increase per Shrine of Chance activation for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_GACHAPONTOKEN_DESC"
            }
        );
        public static ConfigurableValue<float> attackSpeedBonus = new ConfigurableValue<float>(
            "Item: Gachapon Coin",
            "AttackSpeedBonus",
            5f,
            "Attack speed increase per Shrine of Chance activation (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_GACHAPONTOKEN_DESC"
            }
        );
        public static ConfigurableValue<float> attackSpeedBonusPerStack = new ConfigurableValue<float>(
            "Item: Gachapon Coin",
            "AttackSpeedBonusPerStack",
            5f,
            "Attack speed increase per Shrine of Chance activation for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_GACHAPONTOKEN_DESC"
            }
        );

        public override void OnLoad() {
            buffDef.name = "MysticsItems_GachaponBonus";
            buffDef.buffColor = new Color32(239, 214, 52, 225);
            // buffDef.buffColor = new Color32(79, 202, 255, 225);
            buffDef.canStack = true;
            buffDef.iconSprite = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/CritOnUse/bdFullCrit.asset").WaitForCompletion().iconSprite;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(buffDef))
            {
                var itemCount = 0;
                if (sender.inventory) itemCount = sender.inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_GachaponToken);
                if (itemCount > 0)
                {
                    args.critAdd += (critBonus + critBonusPerStack * (float)(itemCount - 1)) * sender.GetBuffCount(buffDef);
                    args.attackSpeedMultAdd += (attackSpeedBonus + attackSpeedBonusPerStack * (float)(itemCount - 1)) / 100f * sender.GetBuffCount(buffDef);
                }
            }
        }
    }
}
