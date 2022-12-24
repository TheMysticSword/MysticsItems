using MysticsRisky2Utils;
using RoR2;
using RoR2.Stats;

namespace MysticsItems
{
    public static class CustomStats
    {
        public static readonly StatDef totalDamageBlockedWithArmor = StatDef.Register("MysticsItems_totalDamageBlockedWithArmor", StatRecordType.Sum, StatDataType.ULong, 0.0, null);
        
        internal static void Init()
		{
            GenericGameEvents.OnTakeDamage += GenericGameEvents_OnTakeDamage;
        }

        private static void GenericGameEvents_OnTakeDamage(DamageReport damageReport)
        {
            if (damageReport.damageInfo == null || damageReport.damageInfo.rejected) return;
            if (damageReport.victimBody && damageReport.victimBody.healthComponent && damageReport.victimBody.healthComponent.ospTimer > 0f) return;

            var damageDifference = damageReport.damageInfo.damage - damageReport.damageDealt;
            if (damageDifference > 0f)
            {
                CharacterMaster master = damageReport.victimMaster;
                if (master != null)
                {
                    PlayerStatsComponent playerStatsComponent = master.playerStatsComponent;
                    if (playerStatsComponent != null)
                    {
                        playerStatsComponent.currentStats.PushStatValue(totalDamageBlockedWithArmor, (ulong)damageDifference);
                    }
                }
            }
        }
    }
}