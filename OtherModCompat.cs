using RoR2;
using UnityEngine;

namespace MysticsItems
{
    public static class OtherModCompat
    {
        public static void ExplosivePickups_TryExplode(CharacterBody body)
        {
            Items.ExplosivePickups.Explode(body);
        }

        public static void ElitePotion_AddSpreadEffect(BuffDef eliteBuffDef, GameObject vfx = null, BuffDef debuff = null, DotController.DotIndex dot = DotController.DotIndex.None, float damage = 0f, float procCoefficient = 0f)
        {
            Items.ElitePotion.spreadEffectInfos.Add(new Items.ElitePotion.SpreadEffectInfo
            {
                eliteBuffDef = eliteBuffDef,
                vfx = vfx,
                debuff = debuff,
                dot = dot,
                damage = damage,
                procCoefficient = procCoefficient
            });
        }
    }
}
