using RoR2;
using UnityEngine;
using Rewired.ComponentControls.Effects;

namespace MysticsItems.Items
{
    public class RiftLensDebuff : BaseItem
    {
        public override void PreLoad()
        {
            itemDef.name = "RiftLensDebuff";
            itemDef.tier = ItemTier.NoTier;
        }

        public override void OnLoad()
        {
            base.OnLoad();
            SetIcon("Rift Lens Debuff");

            CharacterStats.moveSpeedModifiers.Add(new CharacterStats.StatModifier
            {
                multiplier = -0.5f,
                times = (x) => ModifierTimesFunction(x, false)
            });

            GameObject debuffedVFX = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Rift Lens Debuff/RiftLensAfflictionVFX.prefab");
            GameObject vfxOrigin = debuffedVFX.transform.Find("Origin").gameObject;
            CustomTempVFXManagement.MysticsItemsCustomTempVFX tempVFX = debuffedVFX.AddComponent<CustomTempVFXManagement.MysticsItemsCustomTempVFX>();
            ObjectScaleCurve scaleOut = vfxOrigin.AddComponent<ObjectScaleCurve>();
            scaleOut.useOverallCurveOnly = true;
            scaleOut.overallCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 1f, 0f, Mathf.Tan(-45f * Mathf.Deg2Rad)),
                    new Keyframe(1f, 0f, Mathf.Tan(135f * Mathf.Deg2Rad), 0f)
                },
                preWrapMode = WrapMode.Clamp,
                postWrapMode = WrapMode.Clamp
            };
            scaleOut.timeMax = 1f;
            scaleOut.enabled = false;
            tempVFX.exitBehaviours = new MonoBehaviour[]
            {
                scaleOut
            };
            RotateAroundAxis rotateAroundAxis = vfxOrigin.AddComponent<RotateAroundAxis>();
            rotateAroundAxis.relativeTo = Space.Self;
            rotateAroundAxis.rotateAroundAxis = RotateAroundAxis.RotationAxis.X;
            rotateAroundAxis.slowRotationSpeed = 30f;
            rotateAroundAxis.speed = RotateAroundAxis.Speed.Slow;
            CustomTempVFXManagement.allVFX.Add(new CustomTempVFXManagement.VFXInfo
            {
                prefab = debuffedVFX,
                condition = (x) => {
                    Inventory inventory = x.inventory;
                    if (inventory) return inventory.GetItemCount(MysticsItemsContent.Items.RiftLensDebuff) > 0;
                    return false;
                },
                radius = CustomTempVFXManagement.DefaultRadiusCall
            });
        }
    }
}
