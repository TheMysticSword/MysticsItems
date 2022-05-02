using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils.BaseAssetTypes;
using RoR2;
using UnityEngine;
using MysticsRisky2Utils;
using static MysticsItems.BalanceConfigManager;

namespace MysticsItems.Buffs
{
    public class DasherDiscActive : BaseBuff
    {
        public static ConfigurableValue<float> moveSpeedBuff = new ConfigurableValue<float>(
            "Item: Timely Execution",
            "MoveSpeedBuff",
            30f,
            "Movement speed increase when buffed (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_DASHERDISC_DESC"
            }
        );

        public override void OnLoad() {
            buffDef.name = "MysticsItems_DasherDiscActive";
            buffDef.buffColor = UnityEngine.Color.white;
            buffDef.iconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Buffs/DasherDiscActive.png");

            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                    MoveType.AfterLabel,
                    x => x.MatchLdarg(1),
                    x => x.MatchLdfld<DamageInfo>("rejected"),
                    x => x.MatchBrfalse(out _)
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldarg_1);
                    c.EmitDelegate<System.Action<HealthComponent, DamageInfo>>((healthComponent, damageInfo) =>
                    {
                        if (healthComponent.body.HasBuff(buffDef)) damageInfo.rejected = true;
                    });
                }
            };
            On.RoR2.CharacterModel.UpdateMaterials += CharacterModel_UpdateMaterials;
            CharacterModelMaterialOverrides.AddOverride("DasherDiscActive", IncorporealMaterialOverride);

            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(buffDef))
            {
                args.moveSpeedMultAdd += moveSpeedBuff / 100f;
            }
        }

        private void CharacterModel_UpdateMaterials(On.RoR2.CharacterModel.orig_UpdateMaterials orig, CharacterModel self)
        {
            CharacterModelMaterialOverrides.SetOverrideActive(self, "DasherDiscActive", self.visibility >= VisibilityLevel.Visible && self.body && self.body.HasBuff(buffDef));
            orig(self);
        }

        public void IncorporealMaterialOverride(CharacterModel characterModel, ref Material material, ref bool ignoreOverlays)
        {
            if (!ignoreOverlays)
            {
                ignoreOverlays = true;
                material = CharacterModel.ghostMaterial;
            }
        }
    }
}
