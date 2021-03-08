using RoR2;
using R2API.Utils;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using HG;

namespace MysticsItems
{
    public class AssetManager
    {
        public static void Init()
        {
            On.RoR2.ProjectileCatalog.Init += (orig) =>
            {
                orig();
                RegisterProjectilesToCatalog();
            };

            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();
                RegisterBodiesToCatalog();
            };
            On.RoR2.BodyCatalog.CCBodyReloadAll += (orig, self) =>
            {
                orig(self);
                RegisterBodiesToCatalog();
            };

            On.RoR2.MasterCatalog.Init += (orig) =>
            {
                orig();
                RegisterMastersToCatalog(registeredMasters.ToArray());
            };

            On.RoR2.EffectCatalog.Init += (orig) =>
            {
                orig();
                RegisterEffectsToCatalog();
            };
            On.RoR2.EffectCatalog.CCEffectsReload += (orig, self) =>
            {
                orig(self);
                RegisterEffectsToCatalog();
            };
        }

        private static List<GameObject> registeredProjectiles = new List<GameObject>();
        internal static void RegisterProjectile(GameObject prefab)
        {
            registeredProjectiles.Add(prefab);
        }
        private static void RegisterProjectilesToCatalog()
        {
            GameObject[] entries = ArrayUtils.Clone(typeof(ProjectileCatalog).GetFieldValue<GameObject[]>("projectilePrefabs"));
            entries = entries.Concat(registeredProjectiles).ToArray();
            typeof(ProjectileCatalog).InvokeMethod("SetProjectilePrefabs", new object[] { entries });
        }

        private static List<GameObject> registeredBodies = new List<GameObject>();
        internal static void RegisterBody(GameObject prefab)
        {
            registeredBodies.Add(prefab);
        }
        private static void RegisterBodiesToCatalog()
        {
            GameObject[] entries = ArrayUtils.Clone(typeof(BodyCatalog).GetFieldValue<GameObject[]>("bodyPrefabs"));
            entries = entries.Concat(registeredBodies).ToArray();
            typeof(BodyCatalog).InvokeMethod("SetBodyPrefabs", new object[] { entries });
        }

        private static List<GameObject> registeredMasters = new List<GameObject>();
        internal static void RegisterMaster(GameObject prefab)
        {
            registeredMasters.Add(prefab);
        }
        private static void RegisterMastersToCatalog(GameObject[] prefabs)
        {
            GameObject[] entries = ArrayUtils.Clone(typeof(MasterCatalog).GetFieldValue<GameObject[]>("masterPrefabs"));
            entries = entries.Concat(prefabs).ToArray();
            typeof(MasterCatalog).SetFieldValue("nameToIndexMap", new Dictionary<string, MasterCatalog.MasterIndex>());
            typeof(MasterCatalog).InvokeMethod("SetEntries", new object[] { entries });
        }

        private static List<EffectDef> effectDefs = new List<EffectDef>();
        internal static void RegisterEffect(GameObject prefab)
        {
            effectDefs.Add(new EffectDef
            {
                prefab = prefab,
                prefabEffectComponent = prefab.GetComponent<EffectComponent>(),
                prefabVfxAttributes = prefab.GetComponent<VFXAttributes>(),
                prefabName = prefab.name,
                spawnSoundEventName = prefab.GetComponent<EffectComponent>().soundName
            });
        }
        private static void RegisterEffectsToCatalog()
        {
            EffectDef[] entries = ArrayUtils.Clone(typeof(EffectCatalog).GetFieldValue<EffectDef[]>("entries"));
            entries = entries.Concat(effectDefs).ToArray();
            typeof(EffectCatalog).InvokeMethod("SetEntries", new object[] { entries });
        }
    }
}