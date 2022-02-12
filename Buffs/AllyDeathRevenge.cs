using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.BalanceConfigManager;

namespace MysticsItems.Buffs
{
    public class AllyDeathRevenge : BaseBuff
    {
        public static ConfigurableValue<float> attackSpeed = new ConfigurableValue<float>(
            "Item: Vendetta",
            "AttackSpeed",
            100f,
            "Additional attack speed when buffed (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_ALLYDEATHREVENGE_DESC"
            }
        );
        public static ConfigurableValue<float> damage = new ConfigurableValue<float>(
            "Item: Vendetta",
            "Damage",
            100f,
            "Additional damage when buffed (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_ALLYDEATHREVENGE_DESC"
            }
        );

        public override void OnLoad() {
            buffDef.name = "MysticsItems_AllyDeathRevenge";
            buffDef.buffColor = new Color(211f / 255f, 50f / 255f, 25f / 255f);
            buffDef.iconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Buffs/AllyDeathRevenge.png");
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(buffDef))
            {
                args.attackSpeedMultAdd += attackSpeed / 100f;
                args.damageMultAdd += damage / 100f;
            }
        }
    }
}
