using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Buffs
{
    public class NanomachineArmor : BaseBuff
    {
        public static ConfigurableValue<float> armor = new ConfigurableValue<float>(
            "Item: Inoperative Nanomachines",
            "Armor",
            25f,
            "How much armor should the item give when active",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_NANOMACHINES_DESC"
            }
        );
        public static ConfigurableValue<float> armorPerStack = new ConfigurableValue<float>(
            "Item: Inoperative Nanomachines",
            "ArmorPerStack",
            25f,
            "How much armor should the item give when active for each additional stack of this item",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_NANOMACHINES_DESC"
            }
        );

        public override void OnLoad() {
            buffDef.name = "MysticsItems_NanomachineArmor";
            buffDef.buffColor = new Color32(91, 88, 70, 225);
            buffDef.canStack = false;
            buffDef.iconSprite = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/LunarGolem/bdLunarShell.asset").WaitForCompletion().iconSprite;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;

            Overlays.CreateOverlay(Main.AssetBundle.LoadAsset<Material>("Assets/Items/Proximity Nanobots/matNanobotsShield.mat"), delegate (CharacterModel model)
            {
                return model.body ? model.body.HasBuff(buffDef) : false;
            });
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(buffDef))
            {
                var itemCount = 0;
                if (sender.inventory) itemCount = sender.inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_Nanomachines);
                args.armorAdd += armor + armorPerStack * (float)(itemCount - 1);
            }
        }
    }
}
