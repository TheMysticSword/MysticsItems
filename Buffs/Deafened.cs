using RoR2;
using R2API.Utils;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Rewired.ComponentControls.Effects;

namespace MysticsItems.Buffs
{
    public class Deafened : BaseBuff
    {
        public static float multiplier = 1.5f;
        
        public override void OnLoad() {
            buffDef.name = "Deafened";
            buffDef.buffColor = new Color32(255, 195, 112, 255);
            buffDef.isDebuff = true;

            Equipment.Microphone.buffDef = buffDef;
            AddMoveSpeedModifier(-0.5f);
            AddArmorModifier(-20f);

            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);

                // force skill value recalculation
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<System.Action<CharacterBody>>((characterBody) =>
                {
                    if (characterBody.HasBuff(buffDef))
                    {
                        if (characterBody.skillLocator.primary) characterBody.skillLocator.primary.RecalculateValues();
                        if (characterBody.skillLocator.secondary) characterBody.skillLocator.secondary.RecalculateValues();
                        if (characterBody.skillLocator.utility) characterBody.skillLocator.utility.RecalculateValues();
                        if (characterBody.skillLocator.special) characterBody.skillLocator.special.RecalculateValues();
                    }
                });
            };
            // cooldown increase (can't do this in RecalculateStats because this function makes it so the modified cooldown can't be higher than the base cooldown)
            On.RoR2.GenericSkill.CalculateFinalRechargeInterval += (orig, self) =>
            {
                return orig(self) * (self.characterBody.HasBuff(buffDef) ? multiplier : 1f);
            };

            // when the debuff is first received, add a few seconds to current skill cooldowns
            On.RoR2.CharacterBody.OnBuffFirstStackGained += (orig, self, buffDef) =>
            {
                if (buffDef == this.buffDef)
                {
                    GenericSkill[] skills =
                    {
                        self.skillLocator.primary,
                        self.skillLocator.secondary,
                        self.skillLocator.utility,
                        self.skillLocator.special
                    };
                    foreach (GenericSkill skill in skills)
                    {
                        if (skill)
                        {
                            if (skill.stock > 0) skill.DeductStock(1);
                            skill.rechargeStopwatch = Mathf.Min(skill.CalculateFinalRechargeInterval() / multiplier, skill.rechargeStopwatch);
                        }
                    }
                }
                orig(self, buffDef);
            };

            // force cd recalculation when final debuff stack lost
            On.RoR2.CharacterBody.OnBuffFinalStackLost += (orig, self, buffDef) =>
            {
                orig(self, buffDef);
                if (buffDef == this.buffDef)
                {
                    GenericSkill[] skills =
                    {
                        self.skillLocator.primary,
                        self.skillLocator.secondary,
                        self.skillLocator.utility,
                        self.skillLocator.special
                    };
                    foreach (GenericSkill skill in skills)
                    {
                        if (skill)
                        {
                            skill.RecalculateValues();
                        }
                    }
                }
            };

            GameObject debuffedVFX = Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Microphone/DeafenedVFX.prefab");
            GameObject vfxOrigin = debuffedVFX.transform.Find("Origin").gameObject;
            CustomTempVFXManagement.MysticsItemsCustomTempVFX tempVFX = debuffedVFX.AddComponent<CustomTempVFXManagement.MysticsItemsCustomTempVFX>();
            RotateAroundAxis rotateAroundAxis = vfxOrigin.transform.Find("Ring").gameObject.AddComponent<RotateAroundAxis>();
            rotateAroundAxis.relativeTo = Space.Self;
            rotateAroundAxis.rotateAroundAxis = RotateAroundAxis.RotationAxis.X;
            rotateAroundAxis.fastRotationSpeed = 100f;
            rotateAroundAxis.speed = RotateAroundAxis.Speed.Fast;
            rotateAroundAxis = vfxOrigin.transform.Find("Ring (1)").gameObject.AddComponent<RotateAroundAxis>();
            rotateAroundAxis.relativeTo = Space.Self;
            rotateAroundAxis.rotateAroundAxis = RotateAroundAxis.RotationAxis.Z;
            rotateAroundAxis.fastRotationSpeed = 50f;
            rotateAroundAxis.speed = RotateAroundAxis.Speed.Fast;
            ObjectScaleCurve fadeOut = vfxOrigin.AddComponent<ObjectScaleCurve>();
            fadeOut.overallCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 1f, Mathf.Tan(180f * Mathf.Deg2Rad), Mathf.Tan(-20f * Mathf.Deg2Rad)),
                    new Keyframe(1f, 0f, Mathf.Tan(160f * Mathf.Deg2Rad), 0f)
                }
            };
            fadeOut.useOverallCurveOnly = true;
            fadeOut.enabled = false;
            fadeOut.timeMax = 0.6f;
            tempVFX.exitBehaviours = new MonoBehaviour[]
            {
                fadeOut
            };
            ObjectScaleCurve fadeIn = vfxOrigin.AddComponent<ObjectScaleCurve>();
            fadeIn.overallCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 0f, Mathf.Tan(180f * Mathf.Deg2Rad), Mathf.Tan(70f * Mathf.Deg2Rad)),
                    new Keyframe(1f, 1f, Mathf.Tan(-160f * Mathf.Deg2Rad), 0f)
                }
            };
            fadeIn.useOverallCurveOnly = true;
            fadeIn.enabled = false;
            fadeIn.timeMax = 0.6f;
            tempVFX.enterBehaviours = new MonoBehaviour[]
            {
                fadeIn
            };
            CustomTempVFXManagement.allVFX.Add(new CustomTempVFXManagement.VFXInfo
            {
                prefab = debuffedVFX,
                condition = (x) => x.HasBuff(buffDef),
                radius = CustomTempVFXManagement.DefaultRadiusCall
            });
        }
    }
}
