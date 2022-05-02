using MysticsRisky2Utils;
using RoR2;
using RoR2.Achievements;
using RoR2.Projectile;
using UnityEngine;

namespace MysticsItems.Achievements
{
    public class DiscDeath
    {
        [RegisterAchievement("MysticsItems_DiscDeath", "Items.MysticsItems_DasherDisc", null, typeof(Server))]
        public class Tracker : BaseAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                SetServerTracked(true);
            }

            public override void OnUninstall()
            {
                SetServerTracked(false);
                base.OnUninstall();
            }

            public class Server : RoR2.Achievements.BaseServerAchievement
            {
                public int projectileIndex;
                public BodyIndex bodyIndex;

                public override void OnInstall()
                {
                    base.OnInstall();
                    GenericGameEvents.OnTakeDamage += OnTakeDamage;

                    projectileIndex = ProjectileCatalog.FindProjectileIndex("Sawmerang");
                    bodyIndex = BodyCatalog.FindBodyIndex("EquipmentDroneBody");
                }

                private void OnTakeDamage(DamageReport damageReport)
                {
                    if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.FriendlyFire))
                    {
                        if (damageReport.damageInfo != null && damageReport.victimBody && serverAchievementTracker.networkUser.GetCurrentBody() == damageReport.victimBody && damageReport.damageInfo.inflictor)
                        {
                            ProjectileController projectileController = damageReport.damageInfo.inflictor.GetComponent<ProjectileController>();
                            if (projectileController && projectileController.catalogIndex == projectileIndex && damageReport.attackerBodyIndex == bodyIndex)
                            {
                                Grant();
                            }
                        }
                    }
                }

                public override void OnUninstall()
                {
                    base.OnUninstall();
                    GenericGameEvents.OnTakeDamage -= OnTakeDamage;
                }
            }
        }
    }
}
