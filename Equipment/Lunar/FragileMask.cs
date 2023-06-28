using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static MysticsItems.LegacyBalanceConfigManager;

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
        public static ConfigurableValue<bool> lingeringEffect = new ConfigurableValue<bool>(
            "Equipment: Fragile Mask",
            "LingeringEffect",
            false,
            "If true, the item's effect will be active for an extra second after turning it off"
        );

        public static Material overrideMaterial;
        public static NetworkSoundEventDef sfxEnable;
        public static NetworkSoundEventDef sfxDisable;

        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            NetworkingAPI.RegisterMessageType<MysticsItemsFragileMaskBehaviour.SyncMaskSetActive>();
        }

        public override void OnLoad()
        {
            base.OnLoad();
            equipmentDef.name = "MysticsItems_FragileMask";
            ConfigManager.Balance.CreateEquipmentCooldownOption(equipmentDef, "Equipment: Fragile Mask", 3f);
            equipmentDef.canDrop = true;
            ConfigManager.Balance.CreateEquipmentEnigmaCompatibleOption(equipmentDef, "Equipment: Fragile Mask", false);
            ConfigManager.Balance.CreateEquipmentCanBeRandomlyTriggeredOption(equipmentDef, "Equipment: Fragile Mask", false);
            equipmentDef.isLunar = true;
            equipmentDef.colorIndex = ColorCatalog.ColorIndex.LunarItem;
            equipmentDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Fragile Mask/Model.prefab"));
            var mat = equipmentDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial;
            HopooShaderToMaterial.Standard.Apply(mat);
            HopooShaderToMaterial.Standard.Emission(mat, 3f, new Color32(43, 255, 251, 255));
            equipmentDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Equipment/Fragile Mask/Icon2.png");

            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(equipmentDef.pickupModelPrefab, equipmentDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Head", new Vector3(-0.00016F, 0.16862F, 0.22667F), new Vector3(0F, 0F, 0F), new Vector3(0.147F, 0.147F, 0.147F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(-0.01269F, 0.19186F, 0.14171F), new Vector3(342.3076F, 0F, 0F), new Vector3(0.119F, 0.119F, 0.121F));
                AddDisplayRule("Bandit2Body", "Head", new Vector3(-0.00647F, 0.00004F, 0.15421F), new Vector3(0F, 0F, 0F), new Vector3(0.097F, 0.097F, 0.097F));
                AddDisplayRule("ToolbotBody", "Head", new Vector3(0.17721F, 3.27152F, -1.81851F), new Vector3(306.0066F, 180F, 0F), new Vector3(1.24747F, 1.24747F, 1.24747F));
                AddDisplayRule("EngiBody", "HeadCenter", new Vector3(-0.00145F, -0.04144F, 0.22149F), new Vector3(0F, 0F, 0F), new Vector3(0.15596F, 0.15596F, 0.15596F));
                AddDisplayRule("MageBody", "Head", new Vector3(0F, 0.03035F, 0.14915F), new Vector3(0F, 0F, 0F), new Vector3(0.10014F, 0.10014F, 0.10014F));
                AddDisplayRule("MercBody", "Head", new Vector3(-0.01324F, 0.07837F, 0.20433F), new Vector3(0F, 0F, 0F), new Vector3(0.123F, 0.123F, 0.123F));
                AddDisplayRule("TreebotBody", "PlatformBase", new Vector3(0.01191F, 0.29394F, 0.88825F), new Vector3(21.4706F, 359.7744F, 16.32179F), new Vector3(0.3626F, 0.3626F, 0.3626F));
                AddDisplayRule("LoaderBody", "Head", new Vector3(-0.01458F, 0.04875F, 0.16764F), new Vector3(0F, 0F, 0F), new Vector3(0.14481F, 0.14481F, 0.14481F));
                AddDisplayRule("CrocoBody", "MouthMuzzle", new Vector3(-0.16546F, -0.03058F, 3.43643F), new Vector3(67.59733F, 0.04097F, 359.0291F), new Vector3(1.49662F, 1.49662F, 1.49662F));
                AddDisplayRule("CaptainBody", "Head", new Vector3(-0.01887F, 0.03281F, 0.24355F), new Vector3(1.79143F, 0F, 0F), new Vector3(0.1343F, 0.13818F, 0.15238F));
                AddDisplayRule("ScavBody", "Chest", new Vector3(-1.90286F, 4.41959F, -7.60945F), new Vector3(319.543F, 180F, 18.09413F), new Vector3(2.08147F, 2.08147F, 2.08147F));
                AddDisplayRule("EquipmentDroneBody", "HeadCenter", new Vector3(-0.06693F, -1.12064F, -0.77318F), new Vector3(90F, 0F, 0F), new Vector3(0.5795F, 0.5795F, 0.5795F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Head", new Vector3(-0.01493F, 0.22875F, -0.04979F), new Vector3(270F, 0F, 0F), new Vector3(0.12327F, 0.11932F, 0.11932F));
                AddDisplayRule("RailgunnerBody", "Head", new Vector3(0F, -0.00018F, 0.13138F), new Vector3(8.64072F, 0F, 0F), new Vector3(0.11091F, 0.11091F, 0.11091F));
                AddDisplayRule("VoidSurvivorBody", "Head", new Vector3(0.00002F, 0.0516F, 0.20447F), new Vector3(337.5584F, 0F, 0F), new Vector3(0.17494F, 0.13959F, 0.13131F));
            };

            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            GenericGameEvents.OnTakeDamage += GenericGameEvents_OnTakeDamage;

            On.RoR2.Language.GetLocalizedStringByToken += Language_GetLocalizedStringByToken;

            var materials = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/BrotherGlassBody").GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials;
            overrideMaterial = materials[1];
            CharacterModelMaterialOverrides.AddOverride("FragileMask", BrittleMaterialOverride);
            Overlays.CreateOverlay(materials[0], (characterModel) =>
            {
                if (characterModel.body)
                {
                    var component = characterModel.body.GetComponent<MysticsItemsFragileMaskBehaviour>();
                    if (component && component.maskActive) return true;
                }
                return false;
            });

            sfxEnable = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            sfxEnable.eventName = "MysticsItems_Play_item_use_fragileMask_on";
            MysticsItemsContent.Resources.networkSoundEventDefs.Add(sfxEnable);

            sfxDisable = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            sfxDisable.eventName = "MysticsItems_Play_item_use_fragileMask_off";
            MysticsItemsContent.Resources.networkSoundEventDefs.Add(sfxDisable);
        }

        private void GenericGameEvents_OnTakeDamage(DamageReport damageReport)
        {
            if (damageReport.victimBody && damageReport.damageDealt > 0)
            {
                var component = MysticsItemsFragileMaskBehaviour.GetForBody(damageReport.victimBody);
                if (component && component.maskActive)
                {
                    if (damageReport.victimBody.healthComponent)
                    {
                        damageReport.victimBody.healthComponent.Networkhealth = 0;
                    }
                }
            }
        }

        private string Language_GetLocalizedStringByToken(On.RoR2.Language.orig_GetLocalizedStringByToken orig, Language self, string token)
        {
            var result = orig(self, token);
            if (token == "EQUIPMENT_MYSTICSITEMS_FRAGILEMASK_DESC")
                result = Utils.FormatStringByDict(result, new Dictionary<string, string>()
                {
                    { "BaseDamageBonus", (damageMultiplier * 100f - 100f).ToString(System.Globalization.CultureInfo.InvariantCulture) }
                });
            return result;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var component = MysticsItemsFragileMaskBehaviour.GetForBody(sender);
            if (component && component.maskActive)
            {
                args.damageMultAdd += damageMultiplier - 1f;
            }
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            var component = MysticsItemsFragileMaskBehaviour.GetForBody(self);
            if (component && component.maskActive)
            {
                self.isGlass = true;
            }
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            var component = MysticsItemsFragileMaskBehaviour.GetForBody(self);
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
            public float maskDisableDelay = 0f;
            public CharacterBody body;

            public static Dictionary<CharacterBody, MysticsItemsFragileMaskBehaviour> bodyToBehaviourDict = new Dictionary<CharacterBody, MysticsItemsFragileMaskBehaviour>();
            public static MysticsItemsFragileMaskBehaviour GetForBody(CharacterBody body)
            {
                if (!bodyToBehaviourDict.ContainsKey(body))
                    body.gameObject.AddComponent<MysticsItemsFragileMaskBehaviour>();
                return bodyToBehaviourDict[body];
            }

            public void Awake()
            {
                body = GetComponent<CharacterBody>();
                bodyToBehaviourDict[body] = this;
            }

            public void SetMaskActive(bool enable)
            {
                maskWasActive = maskActive;
                maskActive = enable;
                if (maskWasActive != maskActive)
                {
                    body.statsDirty = true;
                    var modelLocator = body.modelLocator;
                    if (modelLocator)
                    {
                        var modelTransform = modelLocator.modelTransform;
                        if (modelTransform)
                        {
                            var model = modelTransform.GetComponent<CharacterModel>();
                            if (model)
                            {
                                CharacterModelMaterialOverrides.SetOverrideActive(model, "FragileMask", maskActive);
                            }
                        }
                    }

                    if (NetworkServer.active)
                        new SyncMaskSetActive(gameObject.GetComponent<NetworkIdentity>().netId, enable).Send(NetworkDestination.Clients);
                }
            }

            public void FixedUpdate()
            {
                if (NetworkServer.active && maskActive && maskDisableDelay > 0f)
                {
                    maskDisableDelay -= Time.fixedDeltaTime;
                    if (maskDisableDelay <= 0f)
                    {
                        SetMaskActive(false);
                    }
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
            
            public void OnDestroy()
            {
                bodyToBehaviourDict.Remove(body);
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
                    if (component.maskActive && component.maskDisableDelay <= 0f && !autoCast)
                    {
                        component.maskDisableDelay = lingeringEffect ? 1f : 0.01f;
                        RoR2.Audio.EntitySoundManager.EmitSoundServer(sfxDisable.index, equipmentSlot.characterBody.gameObject);
                        return true;
                    }
                    if (!component.maskActive)
                    {
                        component.SetMaskActive(true);
                        if (!autoCast)
                        {
                            RoR2.Audio.EntitySoundManager.EmitSoundServer(sfxEnable.index, equipmentSlot.characterBody.gameObject);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void BrittleMaterialOverride(CharacterModel characterModel, ref Material material, ref bool ignoreOverlays)
        {
            if (characterModel.body && characterModel.visibility >= VisibilityLevel.Visible && !ignoreOverlays)
            {
                material = overrideMaterial;
            }
        }
    }
}
