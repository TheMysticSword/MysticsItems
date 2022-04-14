using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace MysticsItems.Achievements
{
    public class DiscDeath : BaseAchievement
    {
        public override void OnLoad()
        {
            name = "MysticsItems_DiscDeath";
            unlockableName = "Items.MysticsItems_DasherDisc";
            iconSprite = MysticsRisky2Utils.Utils.AddItemIconBackgroundToSprite(Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Dasher Disc/Icon.png"), MysticsRisky2Utils.Utils.ItemIconBackgroundType.Tier3);
            trackerType = typeof(Tracker);
            serverTrackerType = typeof(Tracker.Server);
        }

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

        public class Tracker : RoR2.Achievements.BaseAchievement
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
