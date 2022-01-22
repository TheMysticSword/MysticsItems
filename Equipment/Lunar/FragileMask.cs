using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using R2API.Utils;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;
using System.Collections.ObjectModel;
using RoR2.UI;
using MysticsRisky2Utils;
using System.Linq;
using System.Collections.Generic;
using MysticsRisky2Utils.BaseAssetTypes;
using static MysticsItems.BalanceConfigManager;
using R2API.Networking.Interfaces;
using R2API.Networking;

namespace MysticsItems.Equipment
{
    public class FragileMask : BaseEquipment
    {
        public static ConfigurableValue<float> damageMultiplier = new ConfigurableValue<float>(
            "Equipment: Fragile Mask",
            "DamageMultiplier",
            10f,
            "Base damage multiplier while active",
            new List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_FRAGILEMASK_PICKUP",
                "EQUIPMENT_MYSTICSITEMS_FRAGILEMASK_DESC"
            }
        );

        public static Material overrideMaterial;

        public override void OnLoad()
        {
            base.OnLoad();
            equipmentDef.name = "MysticsItems_FragileMask";
            equipmentDef.cooldown = new ConfigurableCooldown("Equipment: Fragile Mask", 3f).Value;
            equipmentDef.canDrop = true;
            equipmentDef.enigmaCompatible = new ConfigurableEnigmaCompatibleBool("Equipment: Fragile Mask", false).Value;
            equipmentDef.isLunar = true;
            equipmentDef.colorIndex = ColorCatalog.ColorIndex.LunarItem;
            //equipmentDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Gate Chalice/Model.prefab"));
            //equipmentDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Equipment/Gate Chalice/Icon.png");

            //itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(equipmentDef.pickupModelPrefab, equipmentDef.pickupModelPrefab.name + "Display", false));
            /*
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Stomach", new Vector3(-0.09F, 0.1F, -0.102F), new Vector3(5.862F, 140.357F, 1.915F), new Vector3(0.059F, 0.059F, 0.059F));
                AddDisplayRule("HuntressBody", "Pelvis", new Vector3(-0.082F, -0.111F, 0.085F), new Vector3(0.679F, 36.762F, 188.148F), new Vector3(0.047F, 0.047F, 0.048F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(-0.096F, 0.027F, -0.151F), new Vector3(337.162F, 337.663F, 11.532F), new Vector3(0.04F, 0.04F, 0.04F));
                AddDisplayRule("ToolbotBody", "Hip", new Vector3(-1.239F, 0.577F, -1.044F), new Vector3(0F, 180F, 180F), new Vector3(0.349F, 0.349F, 0.349F));
                AddDisplayRule("EngiBody", "Pelvis", new Vector3(-0.178F, 0.078F, 0.157F), new Vector3(11.745F, 186.295F, 185.936F), new Vector3(0.047F, 0.047F, 0.047F));
                AddDisplayRule("MageBody", "Pelvis", new Vector3(-0.128F, -0.131F, 0.024F), new Vector3(6.286F, 3.408F, 167.572F), new Vector3(0.044F, 0.044F, 0.044F));
                AddDisplayRule("MercBody", "Chest", new Vector3(0F, 0.193F, -0.286F), new Vector3(71.925F, 180F, 0F), new Vector3(0.027F, 0.027F, 0.027F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(-0.485F, 0.701F, -0.803F), new Vector3(26.173F, 24.306F, 86.838F), new Vector3(0.061F, 0.061F, 0.061F));
                AddDisplayRule("LoaderBody", "Pelvis", new Vector3(-0.216F, -0.016F, -0.022F), new Vector3(342.363F, 183.205F, 159.555F), new Vector3(0.045F, 0.045F, 0.045F));
                AddDisplayRule("CrocoBody", "SpineStomach1", new Vector3(0.845F, 0.495F, 1.289F), new Vector3(74.633F, 327.618F, 247.859F), new Vector3(0.361F, 0.361F, 0.361F));
                AddDisplayRule("CaptainBody", "Stomach", new Vector3(-0.195F, 0.128F, 0.126F), new Vector3(336.504F, 156.734F, 358.159F), new Vector3(0.041F, 0.041F, 0.041F));
                AddDisplayRule("ScavBody", "MuzzleEnergyCannon", new Vector3(0F, 0F, -1.503F), new Vector3(90F, 0F, 0F), new Vector3(2.281F, 2.281F, 2.281F));
                AddDisplayRule("EquipmentDroneBody", "GunBarrelBase", new Vector3(0F, 0F, 1.069F), new Vector3(0F, 0F, 0F), new Vector3(0.267F, 0.267F, 0.267F));
            };
            */

            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            GenericGameEvents.OnTakeDamage += GenericGameEvents_OnTakeDamage;

            On.RoR2.Language.GetLocalizedStringByToken += Language_GetLocalizedStringByToken;

            var materials = Resources.Load<GameObject>("Prefabs/CharacterBodies/BrotherGlassBody").GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials;
            overrideMaterial = materials[1];
            CharacterModelMaterialOverrides.AddOverride(BrittleMaterialOverride);
            Overlays.CreateOverlay(materials[0], (characterModel) =>
            {
                if (characterModel.body)
                {
                    var component = characterModel.body.GetComponent<MysticsItemsFragileMaskBehaviour>();
                    if (component && component.maskActive) return true;
                }
                return false;
            });
        }

        private void GenericGameEvents_OnTakeDamage(DamageReport damageReport)
        {
            if (damageReport.victimBody)
            {
                var component = damageReport.victimBody.GetComponent<MysticsItemsFragileMaskBehaviour>();
                if (component && component.maskActive)
                {
                    if (component.skipDamageCheck <= 0 && damageReport.victimBody.healthComponent)
                    {
                        component.skipDamageCheck++;
                        damageReport.victimBody.healthComponent.Suicide(damageReport.damageInfo.attacker, damageReport.damageInfo.inflictor, damageReport.damageInfo.damageType);
                    }
                    if (component.skipDamageCheck > 0) component.skipDamageCheck--;
                }
            }
        }

        private string Language_GetLocalizedStringByToken(On.RoR2.Language.orig_GetLocalizedStringByToken orig, Language self, string token)
        {
            var result = orig(self, token);
            if (token == "EQUIPMENT_MYSTICSITEMS_FRAGILEMASK_DESC")
                result = Utils.FormatStringByDict(result, new System.Collections.Generic.Dictionary<string, string>()
                {
                    { "BaseDamageBonus", (damageMultiplier * 100f - 100f).ToString(System.Globalization.CultureInfo.InvariantCulture) }
                });
            return result;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var component = sender.GetComponent<MysticsItemsFragileMaskBehaviour>();
            if (component && component.maskActive)
            {
                args.damageMultAdd += damageMultiplier - 1f;
            }
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            var component = self.GetComponent<MysticsItemsFragileMaskBehaviour>();
            if (component && component.maskActive)
            {
                self.isGlass = true;
            }
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            var component = self.GetComponent<MysticsItemsFragileMaskBehaviour>();
            if (!component) component = self.gameObject.AddComponent<MysticsItemsFragileMaskBehaviour>();
            if (self.previousEquipmentIndex == equipmentDef.equipmentIndex && self.inventory.currentEquipmentIndex != equipmentDef.equipmentIndex)
            {
                if (component.maskActive)
                {
                    component.maskWasActive = true;
                    component.maskActive = false;
                    self.statsDirty = true;
                }
            }
        }

        public class MysticsItemsFragileMaskBehaviour : MonoBehaviour
        {
            public bool maskActive = false;
            public bool maskWasActive = false;
            public int skipDamageCheck = 0;
            public CharacterBody body;

            public void Awake()
            {
                body = GetComponent<CharacterBody>();
            }

            public void SetMaskActive(bool enable)
            {
                maskWasActive = maskActive;
                maskActive = enable;
                if (maskWasActive != maskActive)
                {
                    body.statsDirty = true;
                    if (NetworkServer.active)
                        new SyncMaskSetActive(gameObject.GetComponent<NetworkIdentity>().netId, enable).Send(NetworkDestination.Clients);
                }
            }

            public class SyncMaskSetActive : INetMessage
            {
                NetworkInstanceId objID;
                bool enable;

                public SyncMaskSetActive()
                {
                }

                public SyncMaskSetActive(NetworkInstanceId objID, bool enable)
                {
                    this.objID = objID;
                    this.enable = enable;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                    enable = reader.ReadBoolean();
                }

                public void OnReceived()
                {
                    if (NetworkServer.active) return;
                    GameObject obj = Util.FindNetworkObject(objID);
                    if (obj)
                    {
                        MysticsItemsFragileMaskBehaviour controller = obj.GetComponent<MysticsItemsFragileMaskBehaviour>();
                        if (controller)
                        {
                            controller.SetMaskActive(enable);
                        }
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                    writer.Write(enable);
                }
            }
        }

        public override bool OnUse(EquipmentSlot equipmentSlot)
        {
            if (equipmentSlot.characterBody)
            {
                var component = equipmentSlot.characterBody.GetComponent<MysticsItemsFragileMaskBehaviour>();
                var inventory = equipmentSlot.characterBody.inventory;
                var autoCast = inventory && inventory.GetItemCount(RoR2Content.Items.AutoCastEquipment) > 0;
                if (component)
                {
                    component.SetMaskActive(!component.maskActive || autoCast);
                    if (!autoCast) return true;
                }
            }
            return false;
        }

        public void BrittleMaterialOverride(CharacterModel characterModel, ref Material material, ref bool ignoreOverlays)
        {
            if (characterModel.body && characterModel.visibility >= VisibilityLevel.Visible && !ignoreOverlays)
            {
                var component = characterModel.body.GetComponent<MysticsItemsFragileMaskBehaviour>();
                if (component && component.maskActive)
                {
                    material = overrideMaterial;
                }
            }
        }
    }
}
