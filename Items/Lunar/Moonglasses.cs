using RoR2;
using RoR2.Navigation;
using R2API;
using R2API.Utils;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Collections.Generic;
using System.Linq;

namespace MysticsItems.Items
{
    public class Moonglasses : BaseItem
    {
        public override void OnLoad()
        {
            itemDef.name = "Moonglasses";
            itemDef.tier = ItemTier.Lunar;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage
            };

            GenericGameEvents.OnApplyDamageModifiers += MoonglassesCritModifier;

            // Apply the crit multiplier after all stats are calculated
            On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
            {
                orig(self);
                Inventory inventory = self.inventory;
                if (inventory)
                {
                    int itemCount = self.inventory.GetItemCount(itemDef);
                    if (itemCount > 0)
                    {
                        self.crit /= 2 * itemCount;
                    }
                }
            };
        }

        private float MoonglassesCritModifier(DamageInfo damageInfo, GenericGameEvents.GenericCharacterInfo attackerInfo, GenericGameEvents.GenericCharacterInfo victimInfo, float damage)
        {
            if (damageInfo.crit)
            {
                if (attackerInfo.inventory && attackerInfo.inventory.GetItemCount(itemDef) > 0)
                {
                    damage /= 2f; // Undo default crit multiplier
                    damage *= 3f + 1f * (attackerInfo.inventory.GetItemCount(itemDef) - 1);
                }
            }
            return damage;
        }
    }
}
