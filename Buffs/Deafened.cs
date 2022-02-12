using RoR2;
using R2API.Utils;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Rewired.ComponentControls.Effects;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using static MysticsItems.BalanceConfigManager;
using System.Collections.Generic;

namespace MysticsItems.Buffs
{
    public class Deafened : BaseBuff
    {
        public static ConfigurableValue<float> moveSpeedReduction = new ConfigurableValue<float>(
            "Equipment: Vintage Microphone",
            "MoveSpeedReduction",
            50f,
            "Movement speed reduction to Deafened enemies (in %)",
            new List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_MICROPHONE_DESC"
            }
        );
        public static ConfigurableValue<float> armorReduction = new ConfigurableValue<float>(
            "Equipment: Vintage Microphone",
            "ArmorReduction",
            20f,
            "Armor reduction to Deafened enemies",
            new List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_MICROPHONE_DESC"
            }
        );
        public static ConfigurableValue<float> attackSpeedReduction = new ConfigurableValue<float>(
            "Equipment: Vintage Microphone",
            "AttackSpeedReduction",
            50f,
            "Attack speed reduction to Deafened enemies (in %)",
            new List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_MICROPHONE_DESC"
            }
        );
        public static ConfigurableValue<float> damageReduction = new ConfigurableValue<float>(
            "Equipment: Vintage Microphone",
            "DamageReduction",
            30f,
            "Damage reduction to Deafened enemies (in %)",
            new List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_MICROPHONE_DESC"
            }
        );

        public override void OnLoad() {
            buffDef.name = "MysticsItems_Deafened";
            buffDef.buffColor = new Color32(255, 195, 112, 255);
            buffDef.isDebuff = true;
            buffDef.iconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Buffs/Deafened.png");

            Equipment.Microphone.buffDef = buffDef;

            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            
            GameObject debuffedVFX = Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Microphone/DeafenedVFX.prefab");
            GameObject vfxOrigin = debuffedVFX.transform.Find("Origin").gameObject;
            CustomTempVFXManagement.MysticsRisky2UtilsTempVFX tempVFX = debuffedVFX.AddComponent<CustomTempVFXManagement.MysticsRisky2UtilsTempVFX>();
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

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(buffDef))
            {
                args.moveSpeedReductionMultAdd += moveSpeedReduction.Value / 100f;
                args.armorAdd -= armorReduction.Value;
                args.attackSpeedMultAdd -= attackSpeedReduction.Value / 100f;
                args.damageMultAdd -= damageReduction.Value / 100f;
            }
        }
    }
}
