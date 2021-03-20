using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace MysticsItems.Items
{
    public class ExtraShrineUse : BaseItem
    {
        public override void PreAdd()
        {
            itemDef.name = "ExtraShrineUse";
            itemDef.tier = ItemTier.Tier2;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist
            };
            BanFromDeployables();
            SetAssets("Hexahedral Monolith");
            Main.HopooShaderToMaterial.Standard.Gloss(GetModelMaterial());
            GetModelMaterial().SetFloat("_Smoothness", 0.5f);
        }

        public static void UpdateShrine(MysticsItemsExtraShrineUseBehaviour self)
        {
            ItemIndex itemIndex = registeredItems[typeof(ExtraShrineUse)].itemIndex;
            int increaseBy = 0;
            foreach (CharacterMaster master in CharacterMaster.readOnlyInstancesList) if (master.teamIndex == TeamIndex.Player)
            {
                Inventory inventory = master.inventory;
                if (inventory && inventory.GetItemCount(itemIndex) > 0) increaseBy += 1 + (inventory.GetItemCount(itemIndex) - 1);
            }
            increaseBy -= self.increasedPurchaseCount;
            self.increasedPurchaseCount += increaseBy;
            foreach (MonoBehaviour monoBehaviour in self.GetComponents<MonoBehaviour>())
            {
                FieldInfo maxPurchaseCountField = monoBehaviour.GetType().GetField("maxPurchaseCount", Main.bindingFlagAll);
                if (maxPurchaseCountField != null)
                {
                    maxPurchaseCountField.SetValue(monoBehaviour, (int)maxPurchaseCountField.GetValue(monoBehaviour) + increaseBy);
                }
            }
        }

        public override void OnAdd()
        {
            On.RoR2.Inventory.GiveItem += (orig, self, itemIndex2, count) =>
            {
                orig(self, itemIndex2, count);
                if (itemIndex2 == itemIndex)
                {
                    foreach (MysticsItemsExtraShrineUseBehaviour extraShrineUseBehaviour in MysticsItemsExtraShrineUseBehaviour.activeShrines)
                    {
                        UpdateShrine(extraShrineUseBehaviour);
                    }
                }
            };

            On.RoR2.PurchaseInteraction.Awake += (orig, self) =>
            {
                orig(self);
                GenericDisplayNameProvider genericDisplayNameProvider = self.GetComponent<GenericDisplayNameProvider>();
                if (genericDisplayNameProvider && genericDisplayNameProvider.displayToken.Contains("SHRINE"))
                    self.gameObject.AddComponent<MysticsItemsExtraShrineUseBehaviour>();
            };
        }

        public class MysticsItemsExtraShrineUseBehaviour : MonoBehaviour
        {
            public int increasedPurchaseCount = 0;

            public void Awake()
            {
                UpdateShrine(this);
            }

            public void OnEnable()
            {
                activeShrines.Add(this);
            }

            public void OnDisable()
            {
                activeShrines.Remove(this);
            }

            public static List<MysticsItemsExtraShrineUseBehaviour> activeShrines = new List<MysticsItemsExtraShrineUseBehaviour>();
        }
    }
}
