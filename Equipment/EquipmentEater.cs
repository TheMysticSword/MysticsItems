using RoR2;
using RoR2.DirectionalSearch;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Equipment
{
    public class EquipmentEater : BaseEquipment
    {
        public static GameObject crosshairPrefab;

        public static ConfigurableValue<int> amount = new ConfigurableValue<int>(
            "Equipment: Regurgitator",
            "Amount",
            3,
            "Amount of items to drop from decomposed Equipment",
            new System.Collections.Generic.List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_EQUIPMENTEATER_DESC"
            }
        );
        public static ConfigurableValue<float> whiteChance = new ConfigurableValue<float>(
            "Equipment: Regurgitator",
            "WhiteChance",
            80f,
            "Chance of dropping Item Scrap, White",
            new System.Collections.Generic.List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_EQUIPMENTEATER_DESC"
            }
        );
        public static ConfigurableValue<float> greenChance = new ConfigurableValue<float>(
            "Equipment: Regurgitator",
            "GreenChance",
            16f,
            "Chance of dropping Item Scrap, Green",
            new System.Collections.Generic.List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_EQUIPMENTEATER_DESC"
            }
        );
        public static ConfigurableValue<float> redChance = new ConfigurableValue<float>(
            "Equipment: Regurgitator",
            "RedChance",
            1f,
            "Chance of dropping Item Scrap, Red",
            new System.Collections.Generic.List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_EQUIPMENTEATER_DESC"
            }
        );
        public static ConfigurableValue<float> yellowChance = new ConfigurableValue<float>(
            "Equipment: Regurgitator",
            "YellowChance",
            3f,
            "Chance of dropping Item Scrap, Yellow",
            new System.Collections.Generic.List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_EQUIPMENTEATER_DESC"
            }
        );

        public override void OnLoad()
        {
            equipmentDef.name = "MysticsItems_EquipmentEater";
            ConfigManager.Balance.CreateEquipmentCooldownOption(equipmentDef, "Equipment: Regurgitator", 80f);
            equipmentDef.canDrop = true;
            ConfigManager.Balance.CreateEquipmentEnigmaCompatibleOption(equipmentDef, "Equipment: Regurgitator", false);
            ConfigManager.Balance.CreateEquipmentCanBeRandomlyTriggeredOption(equipmentDef, "Equipment: Regurgitator", false);
            
            equipmentDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Robot/Model.prefab"));
            equipmentDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Equipment/Robot/Icon.png");

            var mat = equipmentDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial;
            HopooShaderToMaterial.Standard.Apply(mat);
            HopooShaderToMaterial.Standard.Gloss(mat, 1f, 20f);
            HopooShaderToMaterial.Standard.Emission(mat, 1f, new Color32(255, 32, 0, 255));
            itemDisplayPrefab = PrepareItemDisplayModel(PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Robot/DisplayModel.prefab")));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Chest", new Vector3(-0.236F, 0.4412F, 0.06494F), new Vector3(37.74326F, 266.3575F, 349.6305F), new Vector3(0.024F, 0.024F, 0.024F));
                AddDisplayRule("HuntressBody", "Chest", new Vector3(0.19911F, 0.30563F, 0.04295F), new Vector3(331.8916F, 236.5876F, 36.71829F), new Vector3(0.019F, 0.019F, 0.019F));
                AddDisplayRule("Bandit2Body", "Chest", new Vector3(-0.15464F, 0.42593F, -0.03754F), new Vector3(359.2669F, 228.9457F, 6.24784F), new Vector3(0.02154F, 0.02154F, 0.02154F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(-1.96334F, 2.89667F, 1.82012F), new Vector3(0F, 266.8001F, 0F), new Vector3(0.16063F, 0.16063F, 0.16063F));
                AddDisplayRule("EngiBody", "HeadCenter", new Vector3(0.0005F, 0.21845F, 0.01729F), new Vector3(0F, 271.6655F, 0F), new Vector3(0.03415F, 0.03415F, 0.03415F));
                AddDisplayRule("MageBody", "Chest", new Vector3(-0.1345F, 0.34131F, 0.00048F), new Vector3(19.95378F, 272.8035F, 0F), new Vector3(0.027F, 0.027F, 0.027F));
                AddDisplayRule("MercBody", "Chest", new Vector3(-0.18292F, 0.34106F, -0.00102F), new Vector3(34.09464F, 265.9956F, 0.16347F), new Vector3(0.027F, 0.027F, 0.027F));
                AddDisplayRule("TreebotBody", "WeaponPlatform", new Vector3(0.00053F, -0.07592F, 0.65437F), new Vector3(4.81685F, 87.86542F, 67.22788F), new Vector3(0.061F, 0.061F, 0.061F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(-0.19993F, 0.57446F, 0.20191F), new Vector3(0F, 270F, 0F), new Vector3(0.03092F, 0.03092F, 0.03092F));
                AddDisplayRule("CrocoBody", "SpineChest1", new Vector3(3.37623F, -1.66042F, -0.13404F), new Vector3(57.23269F, 74.32678F, 316.6777F), new Vector3(0.27546F, 0.27546F, 0.27546F));
                AddDisplayRule("CaptainBody", "Chest", new Vector3(-0.39794F, 0.45265F, 0.05594F), new Vector3(13.92463F, 286.205F, 9.46295F), new Vector3(0.03199F, 0.03199F, 0.03199F));
                AddDisplayRule("ScavBody", "Chest", new Vector3(5.20165F, 6.11034F, 1.88309F), new Vector3(20.00704F, 100.8372F, 29.11542F), new Vector3(0.77147F, 0.77147F, 0.77147F));
                AddDisplayRule("EquipmentDroneBody", "GunBarrelBase", new Vector3(-0.02997F, -0.03078F, 2.96198F), new Vector3(344.4445F, 243.6596F, 331.2252F), new Vector3(0.283F, 0.283F, 0.283F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Chest", new Vector3(-0.28479F, 0.44956F, 0.00002F), new Vector3(44.44641F, 270.068F, 0F), new Vector3(0.03309F, 0.03309F, 0.03309F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysDeputy) AddDisplayRule("DeputyBody", "ShoulderL", new Vector3(-0.02565F, 0.13986F, 0.08349F), new Vector3(285.0317F, 130.252F, 38.14699F), new Vector3(0.01814F, 0.01814F, 0.01814F));
                AddDisplayRule("RailgunnerBody", "Backpack", new Vector3(-0.12978F, 0.47855F, 0.02589F), new Vector3(0F, 270F, 0F), new Vector3(0.03017F, 0.03017F, 0.03017F));
                AddDisplayRule("VoidSurvivorBody", "Chest", new Vector3(0.25125F, 0.51668F, -0.02526F), new Vector3(32.56487F, 274.8344F, 3.04495F), new Vector3(0.03269F, 0.03269F, 0.03269F));
            };

            crosshairPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/WoodSpriteIndicator"), "MysticsItems_EquipmentEaterIndicator", false);
            Object.Destroy(crosshairPrefab.GetComponentInChildren<Rewired.ComponentControls.Effects.RotateAroundAxis>());
            crosshairPrefab.GetComponentInChildren<SpriteRenderer>().sprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Equipment/Robot/Crosshair.png");
            crosshairPrefab.GetComponentInChildren<SpriteRenderer>().color = new Color32(255, 235, 75, 255);
            crosshairPrefab.GetComponentInChildren<SpriteRenderer>().transform.rotation = Quaternion.identity;
            crosshairPrefab.GetComponentInChildren<TMPro.TextMeshPro>().color = new Color32(255, 235, 75, 255);
            while (crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.length > 0) crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.RemoveKey(0);
            crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(0f, 2f);
            crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(0.5f, 1f);
            crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(1f, 1f);

            UseTargetFinder(TargetFinderType.Custom);

            On.RoR2.EquipmentSlot.Update += (orig, self) =>
            {
                orig(self);
                if (self.equipmentIndex == equipmentDef.equipmentIndex)
                {
                    MysticsRisky2UtilsEquipmentTarget targetInfo = self.GetComponent<MysticsRisky2UtilsEquipmentTarget>();
                    if (targetInfo)
                    {
                        PickupSearch pickupSearch = targetInfo.GetCustomTargetFinder<PickupSearch>();
                        float num;
                        Ray aimRay = CameraRigController.ModifyAimRayIfApplicable(self.GetAimRay(), self.gameObject, out num);
                        pickupSearch.searchOrigin = aimRay.origin;
                        pickupSearch.searchDirection = aimRay.direction;
                        pickupSearch.minAngleFilter = 0f;
                        pickupSearch.maxAngleFilter = 10f;
                        pickupSearch.minDistanceFilter = 0f;
                        pickupSearch.maxDistanceFilter = 30f + num;
                        pickupSearch.filterByDistinctEntity = false;
                        pickupSearch.filterByLoS = true;
                        pickupSearch.sortMode = SortMode.DistanceAndAngle;
                        GenericPickupController equipmentPickup = pickupSearch.SearchCandidatesForSingleTarget(InstanceTracker.GetInstancesList<GenericPickupController>().Where(x =>
                        {
                            var pickupDef = PickupCatalog.GetPickupDef(x.pickupIndex);
                            if (pickupDef != null)
                            {
                                EquipmentIndex equipmentIndex = pickupDef.equipmentIndex;
                                if (equipmentIndex != EquipmentIndex.None)
                                {
                                    if (EquipmentCatalog.GetEquipmentDef(equipmentIndex).isLunar) return false;
                                    return true;
                                }
                            }
                            return false;
                        }));
                        if (equipmentPickup)
                        {
                            targetInfo.obj = equipmentPickup.gameObject;
                            targetInfo.indicator.visualizerPrefab = crosshairPrefab;
                            targetInfo.indicator.targetTransform = equipmentPickup.pickupDisplay.transform;
                        }
                        else
                        {
                            targetInfo.Invalidate();
                        }
                        targetInfo.indicator.active = equipmentPickup;
                    }
                }
            };
        }

        public override bool OnUse(EquipmentSlot equipmentSlot)
        {
            MysticsRisky2UtilsEquipmentTarget targetInfo = equipmentSlot.GetComponent<MysticsRisky2UtilsEquipmentTarget>();
            if (targetInfo)
            {
                if (targetInfo.obj)
                {
                    var equipmentPickup = targetInfo.obj.GetComponent<GenericPickupController>();

                    var angleAdd = 360f / amount;
                    var weightedSelection = new WeightedSelection<PickupIndex>(4);
                    weightedSelection.AddChoice(PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapWhite.itemIndex), whiteChance);
                    weightedSelection.AddChoice(PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapGreen.itemIndex), greenChance);
                    weightedSelection.AddChoice(PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapRed.itemIndex), redChance);
                    weightedSelection.AddChoice(PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapYellow.itemIndex), yellowChance);
                    for (var i = 0; i < amount; i++)
                    {
                        PickupDropletController.CreatePickupDroplet(
                            weightedSelection.Evaluate(Random.value),
                            equipmentPickup.transform.position + Vector3.up * 2f,
                            Quaternion.AngleAxis(angleAdd * i, Vector3.up) * (Quaternion.AngleAxis(20f, Vector3.forward) * Vector3.up) * 7f
                        );
                    }

                    equipmentPickup.consumed = true;
                    Object.Destroy(targetInfo.obj);

                    targetInfo.Invalidate();
                    return true;
                }
            }
            return false;
        }
    }
}
