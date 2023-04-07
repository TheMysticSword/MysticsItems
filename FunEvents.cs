using MysticsRisky2Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace MysticsItems
{
    internal static class FunEvents
    {
        public static void Init()
        {
            ConfigOptions.ConfigurableValue<bool> enabledByConfig = ConfigOptions.ConfigurableValue.CreateBool(
                ConfigManager.General.categoryGUID,
                ConfigManager.General.categoryName,
                ConfigManager.General.config,
                "Misc",
                "Fun Events",
                true,
                "Enable fun events that happen on specific dates",
                restartRequired: true
            );

            if (enabledByConfig)
            {
                var today = System.DateTime.Now;
                if (today.Month == 4 && today.Day == 1)
                {
                    AprilFools.Init();
                }
                if ((today.Month == 12 && today.Day >= 25) || (today.Month == 1 && today.Day <= 5))
                {
                    ChristmasAndNewYear.Init();
                }
            }
        }

        public static class ChristmasAndNewYear
        {
            public static GameObject festiveEffectsPrefab;

            public static void Init()
            {
                SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            }

            private static void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
            {
                if (scene.name == "bazaar")
                {
                    if (festiveEffectsPrefab == null)
                    {
                        festiveEffectsPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Mods/Mystic's Items/Effects/FestiveStage.prefab");
                        foreach (var particleSystem in festiveEffectsPrefab.GetComponentsInChildren<ParticleSystem>())
                        {
                            var shape = particleSystem.shape;
                            shape.radius = 100f;
                        }
                    }
                    Object.Instantiate(festiveEffectsPrefab, Vector3.zero, Quaternion.identity);
                }
            }
        }

        public static class AprilFools
        {
            public static void Init()
            {
                On.RoR2.Language.GetLocalizedStringByToken += Language_GetLocalizedStringByToken;
            }

            private static string Language_GetLocalizedStringByToken(On.RoR2.Language.orig_GetLocalizedStringByToken orig, Language self, string token)
            {
                var result = orig(self, token);
                if (token.Contains("MYSTICSITEMS_"))
                {
                    result = "<link=\"MysticsItemsAprilFools\">" + result + "</link>";
                }
                return result;
            }
        }
    }
}