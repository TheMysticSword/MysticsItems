using RoR2;
using System.Collections.Generic;
using System.Linq;
using HG;
using UnityEngine;

namespace MysticsItems
{
    public static class CostTypeCreation
    {
        public static void Init()
        {
            CostTypeCatalog.modHelper.getAdditionalEntries += (list) =>
            {
                list = list.Concat(customCostTypes.ConvertAll(x => x.costTypeDef)).ToList();
            };
            On.RoR2.CostTypeCatalog.Init += (orig) =>
            {
                orig();
                for (int i = 0; i < customCostTypes.Count; i++)
                {
                    CustomCostTypeInfo customCostType = customCostTypes[i];
                    customCostType.index = (CostTypeIndex)System.Array.IndexOf(CostTypeCatalog.costTypeDefs, customCostType.costTypeDef);
                    if (customCostType.onRegister != null) customCostType.onRegister(customCostType.index);
                }
            };
            On.RoR2.CostTypeCatalog.GetCostTypeDef += (orig2, costTypeIndex) =>
            {
                CustomCostTypeInfo customCostType = customCostTypes.FirstOrDefault(x => x.index == costTypeIndex);
                if (!customCostType.Equals(default)) return customCostType.costTypeDef;
                return orig2(costTypeIndex);
            };
        }

        public struct CustomCostTypeInfo
        {
            public CostTypeDef costTypeDef;
            public System.Action<CostTypeIndex> onRegister;
            public CostTypeIndex index;
        }

        public static List<CustomCostTypeInfo> customCostTypes = new List<CustomCostTypeInfo>();

        public static void CreateCostType(CustomCostTypeInfo customCostType)
        {
            customCostTypes.Add(customCostType);
        }
    }
}
