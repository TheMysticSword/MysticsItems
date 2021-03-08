using RoR2;
using UnityEngine;

namespace MysticsItems
{
    public static class LaserTurret
    {
        public static GameObject prefab;

        public static void Init()
        {
            prefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/CharacterBodies/LaserTurret/LaserTurret.prefab");
        }

        public static void Spawn(Vector3 position)
        {
            Object.Instantiate(prefab, position, Quaternion.identity);
        }
    }
}