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
		public static float gracePeriod = 0.1f;
		public static Dictionary<CharacterBody, FreezelockInfo> freezelockInfoByBody = new Dictionary<CharacterBody, FreezelockInfo>();

		public class FreezelockInfo
        {
			public float freezeDuration = 0f;
			public float unfrozenTimestamp = 0f;

			public FreezelockInfo()
            {
				if (Run.instance)
					unfrozenTimestamp = Run.instance.GetRunStopwatch();
            }
        }

		public override void OnInstall()
		{
			base.OnInstall();
            On.EntityStates.FrozenState.OnEnter += FrozenState_OnEnter;
            On.EntityStates.FrozenState.FixedUpdate += FrozenState_FixedUpdate;
            On.EntityStates.FrozenState.OnExit += FrozenState_OnExit;
            Stage.onStageStartGlobal += Stage_onStageStartGlobal;
		}

        private void FrozenState_OnEnter(On.EntityStates.FrozenState.orig_OnEnter orig, EntityStates.FrozenState self)
        {
			orig(self);
			if (freezelockInfoByBody.ContainsKey(self.characterBody) && Run.instance)
			{
				if ((Run.instance.GetRunStopwatch() - freezelockInfoByBody[self.characterBody].unfrozenTimestamp) > gracePeriod)
                {
					freezelockInfoByBody[self.characterBody].freezeDuration = 0f;
				}
			}
		}

        private void FrozenState_FixedUpdate(On.EntityStates.FrozenState.orig_FixedUpdate orig, EntityStates.FrozenState self)
        {
			orig(self);
			if (!freezelockInfoByBody.ContainsKey(self.characterBody)) freezelockInfoByBody[self.characterBody] = new FreezelockInfo();
			freezelockInfoByBody[self.characterBody].freezeDuration += Time.fixedDeltaTime;
			if (freezelockInfoByBody[self.characterBody].freezeDuration >= requirement)
            {
				Grant();
            }
		}

		private void FrozenState_OnExit(On.EntityStates.FrozenState.orig_OnExit orig, EntityStates.FrozenState self)
		{
			orig(self);
			if (freezelockInfoByBody.ContainsKey(self.characterBody) && Run.instance)
				freezelockInfoByBody[self.characterBody].unfrozenTimestamp = Run.instance.GetRunStopwatch();
		}

		private void Stage_onStageStartGlobal(Stage obj)
		{
			freezelockInfoByBody.Clear();
		}

		public override void OnUninstall()
		{
			On.EntityStates.FrozenState.OnEnter -= FrozenState_OnEnter;
			On.EntityStates.FrozenState.FixedUpdate -= FrozenState_FixedUpdate;
			On.EntityStates.FrozenState.OnExit -= FrozenState_OnExit;
			Stage.onStageStartGlobal -= Stage_onStageStartGlobal;
			base.OnUninstall();
		}
	}
}
