using RoR2;
using UnityEngine;

namespace MysticsItems.Items
{
    public class GateChaliceDebuff : BaseItem
    {
        public override void PreLoad()
        {
            itemDef.name = "GateChaliceDebuff";
            itemDef.tier = ItemTier.NoTier;
        }

        public override void OnLoad()
        {
            base.OnLoad();
            SetIcon("Gate Chalice Debuff");

            CharacterStats.moveSpeedModifiers.Add(new CharacterStats.StatModifier
            {
                multiplier = -0.33f,
                times = (x) => ModifierTimesFunction(x)
            });
            CharacterStats.armorModifiers.Add(new CharacterStats.FlatStatModifier
            {
                amount = -10f,
                times = (x) => ModifierTimesFunction(x)
            });

            GameObject debuffedVFX = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Gate Chalice Debuff/GateChaliceAfflictionVFX.prefab");
            CustomTempVFXManagement.MysticsItemsCustomTempVFX tempVFX = debuffedVFX.AddComponent<CustomTempVFXManagement.MysticsItemsCustomTempVFX>();
            tempVFX.enterObjects = new GameObject[]
            {
                debuffedVFX.transform.Find("Origin").gameObject
            };
            Material matDebuffedVFX = debuffedVFX.transform.Find("Origin/Embers").gameObject.GetComponent<Renderer>().sharedMaterial;
            Main.HopooShaderToMaterial.CloudRemap.Apply(
                matDebuffedVFX,
                Main.AssetBundle.LoadAsset<Texture>("Assets/Items/Gate Chalice Debuff/texRampGateChaliceAfflictionVFX.png")
            );
            Main.HopooShaderToMaterial.CloudRemap.Boost(matDebuffedVFX, 1f);
            CustomTempVFXManagement.allVFX.Add(new CustomTempVFXManagement.VFXInfo
            {
                prefab = debuffedVFX,
                condition = (x) => {
                    Inventory inventory = x.inventory;
                    if (inventory) return inventory.GetItemCount(MysticsItemsContent.Items.GateChaliceDebuff) > 0;
                    return false;
                },
                radius = CustomTempVFXManagement.DefaultRadiusCall
            });
        }
    }
}
