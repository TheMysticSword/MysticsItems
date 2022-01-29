using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.BalanceConfigManager;

namespace MysticsItems.Buffs
{
    public class RhythmCombo : BaseBuff
    {
        public static ConfigurableValue<float> comboCrit = new ConfigurableValue<float>(
            "Item: Metronome",
            "ComboCrit",
            10f,
            "Critical Strike chance per combo (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_RHYTHM_DESC"
            }
        );
        public static ConfigurableValue<float> comboCritPerStack = new ConfigurableValue<float>(
            "Item: Metronome",
            "ComboCritPerStack",
            5f,
            "Critical Strike chance per combo for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_RHYTHM_DESC"
            }
        );

        public override void OnLoad() {
            buffDef.name = "MysticsItems_RhythmCombo";
            buffDef.buffColor = new Color32(255, 72, 56, 225);
            buffDef.canStack = true;
            buffDef.iconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Buffs/RhythmCombo.png");
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(buffDef))
            {
                var itemCount = sender.inventory ? sender.inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_Rhythm) : 0;
                args.critAdd += (comboCrit + comboCritPerStack * (itemCount - 1)) * sender.GetBuffCount(buffDef);
            }
        }
    }
}
