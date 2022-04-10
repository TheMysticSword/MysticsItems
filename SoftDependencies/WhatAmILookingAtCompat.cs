using BepInEx;
using System.Linq;

namespace MysticsItems.SoftDependencies
{
    internal static class WhatAmILookingAtCompat
    {
        internal static void Init()
        {
            if (GeneralConfigManager.whatAmILookingAtCompatEnabledByConfig.Value)
            {
                var map = WhatAmILookingAt.WhatAmILookingAtPlugin.ContentPackToBepinPluginMap;
                if (map != null && !map.ContainsKey(MysticsItemsPlugin.PluginName))
                {
                    map.Add(MysticsItemsPlugin.PluginName, typeof(MysticsItemsPlugin).GetCustomAttributes(false).FirstOrDefault(x => x is BepInPlugin) as BepInPlugin);
                }
            }
        }
    }
}
