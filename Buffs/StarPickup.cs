using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Buffs
{
    public class StarPickup : BaseBuff
    {
        public static ConfigurableValue<float> attackSpeed = new ConfigurableValue<float>(
            "Item: Stargazer s Records",
            "AttackSpeed",
            5f,
            "Additional attack speed when buffed (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_STARBOOK_DESC"
            }
        );
        public static ConfigurableValue<float> attackSpeedPerStack = new ConfigurableValue<float>(
            "Item: Stargazer s Records",
            "AttackSpeedPerStack",
            5f,
            "Additional attack speed when buffed for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_STARBOOK_DESC"
            }
        );

        public override void OnLoad() {
            buffDef.name = "MysticsItems_StarPickup";
            buffDef.buffColor = new Color32(25, 180, 171, 255);
            buffDef.iconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Star Book/texStarBuff.png");
            buffDef.canStack = true;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(buffDef))
            {
                var itemCount = 0;
                if (sender.inventory) itemCount = sender.inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_StarBook);
                if (itemCount > 0)
                {
                    args.attackSpeedMultAdd += (attackSpeed + attackSpeedPerStack * (float)(itemCount - 1)) / 100f * sender.GetBuffCount(buffDef);
                }
            }
        }
    }
}
