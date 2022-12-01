using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace MysticsItems
{
    internal static class DamageNumberTint
    {
        internal static void Init()
        {
            IL.RoR2.HealthComponent.HandleDamageDealt += HealthComponent_HandleDamageDealt;
            IL.RoR2.DamageNumberManager.SpawnDamageNumber += DamageNumberManager_SpawnDamageNumber;
        }

        public static Color nuclearTintColor = new Color32(179, 239, 0, 255);
        public static Color mysticSwordTintColor = new Color32(244, 236, 80, 255);

        private static DamageDealtMessage damageDealtMessage;
        private static void HealthComponent_HandleDamageDealt(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (c.TryGotoNext(
                MoveType.Before,
                x => x.MatchStloc(0)
            ))
            {
                c.EmitDelegate<System.Func<DamageDealtMessage, DamageDealtMessage>>((newDamageDealtMessage) => {
                    damageDealtMessage = newDamageDealtMessage;
                    return newDamageDealtMessage;
                });
            }

            if (c.TryGotoNext(
                MoveType.Before,
                x => x.MatchRet()
            ))
            {
                c.EmitDelegate<System.Action>(() => {
                    damageDealtMessage = null;
                });
            }
        }
        private static void DamageNumberManager_SpawnDamageNumber(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (c.TryGotoNext(
                MoveType.Before,
                x => x.MatchStloc(0)
            ))
            {
                c.EmitDelegate<System.Func<Color, Color>>((particleColor) => {
                    if (damageDealtMessage != null && damageDealtMessage.attacker != null)
                    {
                        var body = damageDealtMessage.attacker.GetComponent<CharacterBody>();
                        if (body && body.inventory)
                        {
                            var inventory = body.inventory;

                            var itemCount = inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_SpeedGivesDamage);
                            if (itemCount > 0)
                            {
                                particleColor = Color.Lerp(particleColor, nuclearTintColor, 0.3f);
                            }

                            itemCount = inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_MysticSword);
                            if (itemCount > 0)
                            {
                                particleColor = Color.Lerp(particleColor, mysticSwordTintColor, 0.3f);
                            }
                        }
                    }
                    return particleColor;
                });
            }
        }

    }
}