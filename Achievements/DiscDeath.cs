using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace MysticsItems.Achievements
{
    public class DiscDeath : BaseAchievement
    {
        public override void OnLoad()
        {
            name = "DiscDeath";
            unlockableName = Main.TokenPrefix + "Items.DasherDisc";
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

                private void OnTakeDamage(DamageInfo damageInfo, GenericGameEvents.GenericCharacterInfo genericCharacterInfo)
                {
                    if (damageInfo.inflictor)
                    {
						ProjectileController projectileController = damageInfo.inflictor.GetComponent<ProjectileController>();
						if (projectileController && projectileController.catalogIndex == ProjectileCatalog.FindProjectileIndex("Sawmerang"))
						{
							MysticsItemsDiscDeath component = genericCharacterInfo.body.GetComponent<MysticsItemsDiscDeath>();
							if (!component) component = genericCharacterInfo.body.gameObject.AddComponent<MysticsItemsDiscDeath>();
							component.eligibleTime = component.eligibleTimeMax;
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
					if (currentBody)
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
