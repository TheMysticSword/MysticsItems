using MysticsRisky2Utils.BaseAssetTypes;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MysticsItems.Achievements
{
    public class ReachHighPoint : BaseAchievement
    {
        public override void OnLoad()
        {
            name = "MysticsItems_ReachHighPoint";
            unlockableName = "Items.MysticsItems_Backpack";
			iconSprite = MysticsRisky2Utils.Utils.AddItemIconBackgroundToSprite(Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Backpack/Icon.png"), MysticsRisky2Utils.Utils.ItemIconBackgroundType.Tier3);
			trackerType = typeof(Tracker);
        }

        public class Tracker : RoR2.Achievements.BaseAchievement
		{
            public Dictionary<string, float> stageInfo = new Dictionary<string, float>()
            {
                { "frozenwall", 210f }
            };
            public bool stageRequirementMet = false;
            public float yThreshold = 2000f;

			public override void OnInstall()
			{
				base.OnInstall();
                On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
                SceneCatalog.onMostRecentSceneDefChanged += SceneCatalog_onMostRecentSceneDefChanged;
            }

            public override void OnUninstall()
			{
                On.RoR2.CharacterBody.FixedUpdate -= CharacterBody_FixedUpdate;
                SceneCatalog.onMostRecentSceneDefChanged -= SceneCatalog_onMostRecentSceneDefChanged;
                base.OnUninstall();
            }

            private void CharacterBody_FixedUpdate(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
            {
                orig(self);
                if (stageRequirementMet && self == localUser.cachedBody && self.corePosition.y >= yThreshold && (!self.characterMotor || self.characterMotor.isGrounded))
                {
                    Grant();
                }
            }

            private void SceneCatalog_onMostRecentSceneDefChanged(SceneDef sceneDef)
            {
                stageRequirementMet = stageInfo.Keys.Contains(sceneDef.baseSceneName);
                if (stageRequirementMet)
                {
                    yThreshold = stageInfo[sceneDef.baseSceneName];
                }
            }
        }
    }
}
