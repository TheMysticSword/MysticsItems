using RoR2;
using RoR2.CharacterAI;
using MysticsRisky2Utils;
using UnityEngine;
using MysticsRisky2Utils.BaseAssetTypes;

namespace MysticsItems.CharacterMasters
{
    public class UnstableRiftShooter : BaseCharacterMaster
    {
        public static CharacterSpawnCard characterSpawnCard;

        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            prefab = Utils.CreateBlankPrefab("MysticsItems_UnstableRiftShooterMaster", true);
        }

        public override void OnLoad()
        {
            base.OnLoad();

            masterName = "MysticsItems_UnstableRiftShooter";
            Prepare();

            RoR2Application.onLoad += () =>
            {
                prefab.GetComponent<CharacterMaster>().bodyPrefab = BodyCatalog.FindBodyPrefab("MysticsItems_UnstableRiftShooterBody");
            };

            characterSpawnCard = spawnCard;
        }
    }
}
