using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils.BaseAssetTypes;
using RoR2;
using UnityEngine;

namespace MysticsItems.Buffs
{
    public class DasherDiscActive : BaseBuff
    {
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
            IL.RoR2.CharacterModel.UpdateRendererMaterials += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                    MoveType.AfterLabel,
                    x => x.MatchLdarg(3),
                    x => x.MatchBrtrue(out _),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterModel>("activeOverlayCount")
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldloc_0);
                    c.Emit(OpCodes.Ldarg_3);
                    c.EmitDelegate<System.Func<CharacterModel, Material, bool, Material>>((characterModel, material, ignoreOverlays) =>
                    {
                        if (characterModel.body && characterModel.visibility >= VisibilityLevel.Visible && !ignoreOverlays)
                        {
                            if (characterModel.body.HasBuff(buffDef))
                            {
                                return CharacterModel.ghostMaterial;
                            }
                        }
                        return material;
                    });
                    c.Emit(OpCodes.Stloc_0);
                }
            };
        }
    }
}
