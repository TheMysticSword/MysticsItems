using RoR2;
using UnityEngine;

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
            SetIcon("Rift Lens Debuff");

            CharacterStats.moveSpeedModifiers.Add(new CharacterStats.StatModifier
            {
                multiplier = -0.5f,
                times = (x) => ModifierTimesFunction(x, false)
            });

            GameObject debuffedVFX = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Rift Lens Debuff/RiftLensAfflictionVFX.prefab");
            CustomTempVFXManagement.MysticsItemsCustomTempVFX tempVFX = debuffedVFX.AddComponent<CustomTempVFXManagement.MysticsItemsCustomTempVFX>();
            tempVFX.enterObjects = new GameObject[]
            {
                debuffedVFX.transform.Find("Origin").gameObject
            };
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
