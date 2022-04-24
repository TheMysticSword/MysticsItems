using MysticsRisky2Utils;
using RoR2;
using RoR2.Achievements;
using RoR2.Projectile;
using UnityEngine;

namespace MysticsItems.Achievements
{
    public class DiscDeath
    {
        public class MysticsItemsDiscDeath : MonoBehaviour
        {
            public float eligibleTime = 0f;
            public float eligibleTimeMax = 0.5f;
            public bool eligible
            {
                get
                {
                    return eligibleTime >= 0f;
                }
            }
            public void FixedUpdate()
            {
                eligibleTime -= Time.fixedDeltaTime;
            }
        }

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
                public override void OnInstall()
                {
                    base.OnInstall();
                    GenericGameEvents.OnTakeDamage += OnTakeDamage;
                    GlobalEventManager.onCharacterDeathGlobal += OnCharacterDeathGlobal;
                }

                private void OnTakeDamage(DamageReport damageReport)
                {
                    if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.FriendlyFire))
                    {
                        if (damageReport.damageInfo != null && damageReport.victimBody && damageReport.damageInfo.inflictor)
                        {
                            ProjectileController projectileController = damageReport.damageInfo.inflictor.GetComponent<ProjectileController>();
                            if (projectileController && projectileController.catalogIndex == ProjectileCatalog.FindProjectileIndex("Sawmerang") && damageReport.attackerBodyIndex == BodyCatalog.FindBodyIndex("EquipmentDroneBody"))
                            {
                                MysticsItemsDiscDeath component = damageReport.victimBody.GetComponent<MysticsItemsDiscDeath>();
                                if (!component) component = damageReport.victimBody.gameObject.AddComponent<MysticsItemsDiscDeath>();
                                component.eligibleTime = component.eligibleTimeMax;
                            }
                        }
                    }
                }

                public override void OnUninstall()
                {
                    base.OnUninstall();
                    GenericGameEvents.OnTakeDamage -= OnTakeDamage;
                    GlobalEventManager.onCharacterDeathGlobal -= OnCharacterDeathGlobal;
                }

                public void OnCharacterDeathGlobal(DamageReport damageReport)
                {
                    CharacterBody currentBody = serverAchievementTracker.networkUser.GetCurrentBody();
                    if (currentBody && currentBody == damageReport.victimBody)
                    {
                        MysticsItemsDiscDeath component = currentBody.GetComponent<MysticsItemsDiscDeath>();
                        if (component && component.eligible)
                        {
                            Grant();
                        }
                    }
                }
            }
        }
    }
}
