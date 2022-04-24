using RoR2;
using UnityEngine;
using Rewired.ComponentControls.Effects;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;

namespace MysticsItems.Items
{
    public class RiftLensDebuff : BaseItem
    {
        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_RiftLensDebuff";
            SetItemTierWhenAvailable(ItemTier.NoTier);
            itemDef.canRemove = false;
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Rift Lens Debuff/Icon.png");

            GameObject debuffedVFX = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Rift Lens Debuff/RiftLensAfflictionVFX.prefab");
            GameObject vfxOrigin = debuffedVFX.transform.Find("Origin").gameObject;
            CustomTempVFXManagement.MysticsRisky2UtilsTempVFX tempVFX = debuffedVFX.AddComponent<CustomTempVFXManagement.MysticsRisky2UtilsTempVFX>();
            RotateAroundAxis rotateAroundAxis = vfxOrigin.AddComponent<RotateAroundAxis>();
            rotateAroundAxis.relativeTo = Space.Self;
            rotateAroundAxis.rotateAroundAxis = RotateAroundAxis.RotationAxis.X;
            rotateAroundAxis.slowRotationSpeed = 30f;
            rotateAroundAxis.speed = RotateAroundAxis.Speed.Slow;
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
            CustomTempVFXManagement.allVFX.Add(new CustomTempVFXManagement.VFXInfo
            {
                prefab = debuffedVFX,
                condition = (x) => {
                    Inventory inventory = x.inventory;
                    if (inventory) return inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_RiftLensDebuff) > 0;
                    return false;
                },
                radius = CustomTempVFXManagement.DefaultRadiusCall
            });
        }
    }
}
