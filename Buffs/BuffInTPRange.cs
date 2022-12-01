using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using RoR2;
using UnityEngine;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Buffs
{
    public class BuffInTPRange : BaseBuff
    {
        public static ConfigurableValue<float> moveSpeed = new ConfigurableValue<float>(
            "Item: Purrfect Headphones",
            "MoveSpeed",
            25f,
            "How much movement speed to give while active (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_BUFFINTPRANGE_DESC"
            }
        );
        public static ConfigurableValue<float> moveSpeedPerStack = new ConfigurableValue<float>(
            "Item: Purrfect Headphones",
            "MoveSpeedPerStack",
            25f,
            "How much movement speed to give while active (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_BUFFINTPRANGE_DESC"
            }
        );

        public static ConfigurableValue<float> armor = new ConfigurableValue<float>(
            "Item: Purrfect Headphones",
            "Armor",
            10f,
            "How much armor to give while active (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_BUFFINTPRANGE_DESC"
            }
        );
        public static ConfigurableValue<float> armorPerStack = new ConfigurableValue<float>(
            "Item: Purrfect Headphones",
            "ArmorPerStack",
            10f,
            "How much armor to give while active (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_BUFFINTPRANGE_DESC"
            }
        );

        public override void OnLoad() {
            buffDef.name = "MysticsItems_BuffInTPRange";
            buffDef.buffColor = new Color32(40, 251, 255, 225);
            buffDef.canStack = false;
            buffDef.iconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Cat Ear Headphones/texCatEarHeadphonesBuffIcon.png");
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(buffDef))
            {
                var itemCount = 0;
                if (sender.inventory) itemCount = sender.inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_BuffInTPRange);
                args.moveSpeedMultAdd += moveSpeed / 100f + moveSpeedPerStack / 100f * (float)(itemCount - 1);
                args.armorAdd += armor + armorPerStack * (float)(itemCount - 1);
            }
        }
    }
}
