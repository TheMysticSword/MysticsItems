using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Buffs
{
    public class TimePieceSlow : BaseBuff
    {
        public static ConfigurableValue<float> slow = new ConfigurableValue<float>(
            "Item: Time Dilator",
            "Slow",
            20f,
            "Enemy slowing effect (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_TIMEPIECE_DESC"
            }
        );
        public static ConfigurableValue<float> slowPerStack = new ConfigurableValue<float>(
            "Item: Time Dilator",
            "SlowPerStack",
            20f,
            "Enemy slowing effect for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_TIMEPIECE_DESC"
            }
        );

        public override void OnLoad() {
            buffDef.name = "MysticsItems_TimePieceSlow";
            buffDef.buffColor = new Color32(91, 88, 70, 225);
            buffDef.canStack = true;
            buffDef.iconSprite = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/SlowOnHit/bdSlow60.asset").WaitForCompletion().iconSprite;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;

            On.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += (orig, self) =>
            {
                orig(self);
                var component = self.GetComponent<MysticsItemsTimePieceSlowComponent>();
                if (!component)
                    component = self.gameObject.AddComponent<MysticsItemsTimePieceSlowComponent>();
                self.UpdateSingleTemporaryVisualEffect(ref component.temporaryVisualEffect, CharacterBody.AssetReferences.slowDownTimeTempEffectPrefab, self.radius, self.HasBuff(buffDef), "");
            };
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(buffDef))
            {
                var buffCount = sender.GetBuffCount(buffDef);
                var currentSlow = (slow + slowPerStack * (float)(buffCount - 1)) / 100f;
                args.moveSpeedReductionMultAdd += currentSlow;
            }
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (c.TryGotoNext(
                x => x.MatchCallOrCallvirt<CharacterBody>("set_attackSpeed")
            ))
            {
                c.Emit(OpCodes.Ldarg, 0);
                c.EmitDelegate<System.Func<float, CharacterBody, float>>((attackSpeed, sender) => {
                    if (sender.HasBuff(buffDef))
                    {
                        var buffCount = sender.GetBuffCount(buffDef);
                        var currentSlow = 1f + (slow + slowPerStack * (float)(buffCount - 1)) / 100f;
                        attackSpeed /= currentSlow;
                    }
                    return attackSpeed;
                });
            }
            else
            {
                Main.logger.LogError("Time Dilator won't slow down enemy attack speed");
            }
        }

        public class MysticsItemsTimePieceSlowComponent : MonoBehaviour
        {
            public TemporaryVisualEffect temporaryVisualEffect;
        }
    }
}
