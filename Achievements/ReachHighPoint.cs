using RoR2;
using RoR2.Achievements;
using System.Collections.Generic;
using System.Linq;

namespace MysticsItems.Achievements
{
    public class ReachHighPoint
    {
        [RegisterAchievement("MysticsItems_ReachHighPoint", "Items.MysticsItems_Backpack", null, null)]
        public class Tracker : BaseAchievement
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
