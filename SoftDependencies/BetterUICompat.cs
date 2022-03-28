using RoR2;
using BepInEx.Configuration;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MysticsItems.SoftDependencies
{
    internal static class BetterUICompat
    {
        internal static void Init()
        {
            RoR2Application.onLoad += RegisterBuffInfos;

            if (GeneralConfigManager.betterUICompatEnableOverrides.Value)
            {
                System.Func<CharacterBody, string> func;

                func = (statBody) =>
                {
                    var crit = statBody.crit;
                    if (statBody.inventory)
                    {
                        var itemCount = statBody.inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_ScratchTicket);
                        if (itemCount > 0) crit = Items.ScratchTicket.ApplyPercentBonus(itemCount, crit);
                    }
                    return crit.ToString("0.##");
                };
                BetterUI.StatsDisplay.AddStatsDisplay("$crit", func);

                func = (statBody) =>
                {
                    var crit = statBody.crit;
                    if (crit >= 1f && statBody.inventory)
                    {
                        var itemCount = statBody.inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_ScratchTicket);
                        if (itemCount > 0) crit = Items.ScratchTicket.ApplyPercentBonus(itemCount, crit);
                    }
                    float LuckCalc(float chance, float luck)
                    {
                        if (luck == 0) return chance;
                        else if (luck < 0) return (float)((int)chance + Math.Pow(chance % 1, Math.Abs(luck) + 1));
                        else return (float)((int)chance + (1 - Math.Pow(1 - (chance % 1), Math.Abs(luck) + 1)));
                    }
                    crit = 100 * ((int)crit / 100) + 100 * LuckCalc(crit % 100 * 0.01f, statBody.master.luck);
                    return crit.ToString("0.##");
                };
                BetterUI.StatsDisplay.AddStatsDisplay("$luckcrit", func);
            }
        }

        private static void RegisterBuffInfos()
        {
            foreach (var buffDef in typeof(MysticsItemsContent.Buffs).GetFields().Select(x => x.GetValue(null) as BuffDef))
            {
                if (buffDef != null)
                {
                    var tokenStart = "BUFF_" + buffDef.name.ToUpperInvariant() + "_";
                    BetterUI.Buffs.RegisterBuffInfo(buffDef, tokenStart + "NAME", tokenStart + "DESC");
                }
            }
        }
    }
}
