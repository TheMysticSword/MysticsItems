using RoR2;
using System.Linq;
using UnityEngine;

namespace MysticsItems
{
    internal static class UpdateFirstLaunchManager
    {
        public static void Init()
        {
            LocalUserManager.onUserSignIn += (user) =>
            {
                var prefix = "MysticsItems_LastModVersion_";
                var lastModVersion = "0.0.0";
                var storedAchievement = user.userProfile.achievementsList.FirstOrDefault(x => x.StartsWith(prefix, false, System.Globalization.CultureInfo.InvariantCulture));
                if (!string.IsNullOrEmpty(storedAchievement)) lastModVersion = storedAchievement.Remove(0, prefix.Length);
                var lastModVersionNum = ConvertVersionStringToInt(lastModVersion);
                if (lastModVersionNum != currentVersionNum)
                {
                    void ResetAchievements(params string[] achievementsToReset)
                    {
                        AchievementManager.availability.CallWhenAvailable(() =>
                        {
                            foreach (var achievementToReset in achievementsToReset)
                            {
                                if (AchievementManager.achievementNamesToDefs.TryGetValue(achievementToReset, out var achievementDef))
                                {
                                    if (achievementDef != null)
                                    {
                                        if (user.userProfile.HasAchievement(achievementDef.identifier))
                                            user.userProfile.RevokeAchievement(achievementDef.identifier);
                                        var unlockable = UnlockableCatalog.GetUnlockableDef(achievementDef.unlockableRewardIdentifier);
                                        if (unlockable && user.userProfile.HasUnlockable(unlockable))
                                            user.userProfile.RevokeUnlockable(unlockable);
                                    }
                                }
                            }
                        });
                    }

                    if (lastModVersionNum < 200)
                    {
                        ResetAchievements(
                            "MysticsItems_DiscDeath",
                            "MysticsItems_EscapeMoonAlone",
                            "MysticsItems_RepairBrokenSpotter"
                        );
                    }

                    user.userProfile.achievementsList.Remove(storedAchievement);
                    user.userProfile.achievementsList.Add(prefix + MysticsItemsPlugin.PluginVersion);
                }
            };
        }

        private static int ConvertVersionStringToInt(string versionString)
        {
            var result = 0;
            var versionSplit = versionString.Split('.');
            for (var i = 0; i < versionSplit.Length; i++)
            {
                result += (int)(Mathf.Pow(10, versionSplit.Length - i - 1) * int.Parse(versionSplit[i], System.Globalization.CultureInfo.InvariantCulture));
            }
            return result;
        }

        private static readonly int currentVersionNum = ConvertVersionStringToInt(MysticsItemsPlugin.PluginVersion);
    }
}