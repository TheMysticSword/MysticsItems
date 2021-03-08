using RoR2;
using RoR2.Projectile;

namespace MysticsItems.Achievements
{
    public class DiscDeath : BaseAchievement
    {
        public override void PreAdd()
        {
            name = "DiscDeath";
            unlockableName = Main.TokenPrefix + "Items.DasherDisc";
			trackerType = typeof(Tracker);
			serverTrackerType = typeof(Tracker.Server);
        }

        public class Tracker : RoR2.Achievements.BaseAchievement
		{
			public override void OnInstall()
			{
				base.OnInstall();
				SetServerTracked(true);
			}

			public class Server : RoR2.Achievements.BaseServerAchievement
			{
				public override void OnInstall()
				{
					base.OnInstall();
                    GlobalEventManager.onCharacterDeathGlobal += OnCharacterDeathGlobal;
				}

                public override void OnUninstall()
				{
					base.OnUninstall();
					GlobalEventManager.onCharacterDeathGlobal -= OnCharacterDeathGlobal;
				}

				public void OnCharacterDeathGlobal(DamageReport damageReport)
				{
					CharacterBody currentBody = serverAchievementTracker.networkUser.GetCurrentBody();
					if (currentBody && currentBody == damageReport.victimBody)
					{
						if (damageReport.damageInfo.inflictor)
						{
							ProjectileController projectileController = damageReport.damageInfo.inflictor.GetComponent<ProjectileController>();
							if (projectileController && projectileController.catalogIndex == ProjectileCatalog.FindProjectileIndex("Sawmerang"))
							{
								Grant();
							}
						}
					}
				}
			}
		}
    }
}
