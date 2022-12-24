using RoR2;
using RoR2.Achievements;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace MysticsItems.Achievements
{
	[RegisterAchievement("MysticsItems_FreezelockEnemy", "Items.MysticsItems_SnowRing", null, null)]
	public class FreezelockEnemy : BaseAchievement
	{
		public static float requirement = 7f;
		public static Dictionary<CharacterBody, float> freezeDurationByBody = new Dictionary<CharacterBody, float>();

		public override void OnInstall()
		{
			base.OnInstall();
            On.EntityStates.FrozenState.FixedUpdate += FrozenState_FixedUpdate;
            On.EntityStates.FrozenState.OnExit += FrozenState_OnExit;
            Stage.onStageStartGlobal += Stage_onStageStartGlobal;
		}

        private void FrozenState_FixedUpdate(On.EntityStates.FrozenState.orig_FixedUpdate orig, EntityStates.FrozenState self)
        {
			orig(self);
			if (!freezeDurationByBody.ContainsKey(self.characterBody)) freezeDurationByBody[self.characterBody] = 0f;
			freezeDurationByBody[self.characterBody] += Time.fixedDeltaTime;
			if (freezeDurationByBody[self.characterBody] >= requirement)
            {
				Grant();
            }
		}

		private void FrozenState_OnExit(On.EntityStates.FrozenState.orig_OnExit orig, EntityStates.FrozenState self)
		{
			orig(self);
			if (freezeDurationByBody.ContainsKey(self.characterBody)) freezeDurationByBody[self.characterBody] = 0;
		}

		private void Stage_onStageStartGlobal(Stage obj)
		{
			freezeDurationByBody.Clear();
		}

		public override void OnUninstall()
		{
			On.EntityStates.FrozenState.FixedUpdate -= FrozenState_FixedUpdate;
			Stage.onStageStartGlobal -= Stage_onStageStartGlobal;
			base.OnUninstall();
		}
	}
}
