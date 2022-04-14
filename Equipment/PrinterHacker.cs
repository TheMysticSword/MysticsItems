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
using static MysticsItems.BalanceConfigManager;

namespace MysticsItems.Equipment
{
    public class PrinterHacker : BaseEquipment
    {
        public static GameObject crosshairPrefab;

        public static ConfigurableValue<int> amount = new ConfigurableValue<int>(
            "Equipment: Wirehack Wrench",
            "Amount",
            2,
            "Amount of items to drop from the hacked printer",
            new System.Collections.Generic.List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_PRINTERHACKER_PICKUP",
                "EQUIPMENT_MYSTICSITEMS_PRINTERHACKER_DESC"
            }
        );

        public override void OnLoad()
        {
            equipmentDef.name = "MysticsItems_PrinterHacker";
            equipmentDef.cooldown = new ConfigurableCooldown("Equipment: Wirehack Wrench", 90f).Value;
            equipmentDef.canDrop = true;
            equipmentDef.enigmaCompatible = new ConfigurableEnigmaCompatibleBool("Equipment: Wirehack Wrench", false).Value;
            equipmentDef.canBeRandomlyTriggered = new ConfigurableCanBeRandomlyTriggeredBool("Equipment: Wirehack Wrench", false).Value;
            equipmentDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Wirehack Wrench/Model.prefab"));
            equipmentDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Equipment/Wirehack Wrench/Icon.png");

            HopooShaderToMaterial.Standard.Gloss(equipmentDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial, 1f, 20f);
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(equipmentDef.pickupModelPrefab, equipmentDef.pickupModelPrefab.name + "Display", false));
            equipmentDef.pickupModelPrefab.transform.Find("d4b43750924799f8").Rotate(new Vector3(0f, 0f, -30f), Space.Self);
            var modelPanelParams = equipmentDef.pickupModelPrefab.GetComponent<ModelPanelParameters>();
            modelPanelParams.minDistance = 5;
            modelPanelParams.maxDistance = 10;
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Stomach", new Vector3(-0.163F, 0.092F, -0.036F), new Vector3(356.022F, 118.071F, 26.4F), new Vector3(0.024F, 0.024F, 0.024F));
                AddDisplayRule("HuntressBody", "Pelvis", new Vector3(-0.088F, -0.085F, 0.059F), new Vector3(0.679F, 36.762F, 196.086F), new Vector3(0.019F, 0.019F, 0.019F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(0.129F, 0.025F, -0.111F), new Vector3(316.052F, 250.792F, 338.145F), new Vector3(0.038F, 0.038F, 0.038F));
                AddDisplayRule("ToolbotBody", "Hip", new Vector3(-1.202F, 0.577F, -0.876F), new Vector3(0F, 180F, 180F), new Vector3(0.349F, 0.349F, 0.349F));
                AddDisplayRule("EngiBody", "Pelvis", new Vector3(-0.178F, 0.078F, 0.157F), new Vector3(11.745F, 186.295F, 185.936F), new Vector3(0.047F, 0.047F, 0.047F));
                AddDisplayRule("MageBody", "Pelvis", new Vector3(-0.172F, -0.067F, -0.078F), new Vector3(7.421F, 5.596F, 187.29F), new Vector3(0.027F, 0.027F, 0.027F));
                AddDisplayRule("MercBody", "Chest", new Vector3(-0.115F, 0.032F, 0.083F), new Vector3(18.292F, 60.198F, 185.734F), new Vector3(0.027F, 0.027F, 0.027F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(-0.485F, 0.701F, -0.803F), new Vector3(26.173F, 24.306F, 86.838F), new Vector3(0.061F, 0.061F, 0.061F));
                AddDisplayRule("LoaderBody", "Pelvis", new Vector3(-0.216F, -0.016F, -0.022F), new Vector3(342.363F, 183.205F, 159.555F), new Vector3(0.045F, 0.045F, 0.045F));
                AddDisplayRule("CrocoBody", "SpineStomach1", new Vector3(0.845F, 0.495F, 1.289F), new Vector3(74.633F, 327.618F, 247.859F), new Vector3(0.361F, 0.361F, 0.361F));
                AddDisplayRule("CaptainBody", "Stomach", new Vector3(-0.195F, 0.128F, 0.126F), new Vector3(336.504F, 156.734F, 358.159F), new Vector3(0.041F, 0.041F, 0.041F));
                AddDisplayRule("ScavBody", "Backpack", new Vector3(-5.969F, 10.94F, 0.665F), new Vector3(338.478F, 350.544F, 54.934F), new Vector3(1.363F, 1.363F, 1.363F));
                AddDisplayRule("EquipmentDroneBody", "GunBarrelBase", new Vector3(0F, 0F, 1.1F), new Vector3(52.577F, 0F, 0.001F), new Vector3(0.283F, 0.283F, 0.283F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Pelvis", new Vector3(0.24189F, 0.23523F, -0.05035F), new Vector3(309.5458F, 359.9093F, 0.10958F), new Vector3(0.04303F, 0.04303F, 0.04303F));
                AddDisplayRule("RailgunnerBody", "Stomach", new Vector3(0.10961F, -0.09564F, 0.08252F), new Vector3(0F, 0F, 189.4295F), new Vector3(0.04997F, 0.04997F, 0.04997F));
                AddDisplayRule("VoidSurvivorBody", "Head", new Vector3(-0.17006F, 0.12587F, 0.08001F), new Vector3(324.1714F, 170.6519F, 135.1764F), new Vector3(0.04652F, 0.04652F, 0.04652F));
            };

            crosshairPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/WoodSpriteIndicator"), "MysticsItems_PrinterHackerIndicator", false);
            Object.Destroy(crosshairPrefab.GetComponentInChildren<Rewired.ComponentControls.Effects.RotateAroundAxis>());
            crosshairPrefab.GetComponentInChildren<SpriteRenderer>().sprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Equipment/Wirehack Wrench/Crosshair.png");
            crosshairPrefab.GetComponentInChildren<SpriteRenderer>().color = new Color32(255, 235, 75, 255);
            crosshairPrefab.GetComponentInChildren<SpriteRenderer>().transform.rotation = Quaternion.identity;
            crosshairPrefab.GetComponentInChildren<TMPro.TextMeshPro>().color = new Color32(255, 235, 75, 255);
            while (crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.length > 0) crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.RemoveKey(0);
            crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(0f, 2f);
            crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(0.5f, 1f);
            crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(1f, 1f);

            UseTargetFinder(TargetFinderType.Custom);

            On.RoR2.PurchaseInteraction.Awake += (orig, self) =>
            {
                orig(self);
                string properName = MysticsRisky2Utils.Utils.TrimCloneFromString(self.name);
                if (properName == "Duplicator"
                || properName == "DuplicatorLarge"
                || properName == "DuplicatorWild"
                || properName == "DuplicatorMilitary")
                {
                    Transform transform = self.transform.Find("mdlDuplicator/DuplicatorMesh");
                    if (transform)
                    {
                        MysticsItemsDuplicatorLocator component = self.gameObject.AddComponent<MysticsItemsDuplicatorLocator>();
                        component.childTransform = transform;
                    }
                }
            };

            On.RoR2.EquipmentSlot.Update += (orig, self) =>
            {
                orig(self);
                if (self.equipmentIndex == equipmentDef.equipmentIndex)
                {
                    MysticsRisky2UtilsEquipmentTarget targetInfo = self.GetComponent<MysticsRisky2UtilsEquipmentTarget>();
                    if (targetInfo)
                    {
                        DuplicatorSearch duplicatorSearch = targetInfo.GetCustomTargetFinder<DuplicatorSearch>();
                        float num;
                        Ray aimRay = CameraRigController.ModifyAimRayIfApplicable(self.GetAimRay(), self.gameObject, out num);
                        duplicatorSearch.searchOrigin = aimRay.origin;
                        duplicatorSearch.searchDirection = aimRay.direction;
                        duplicatorSearch.minAngleFilter = 0f;
                        duplicatorSearch.maxAngleFilter = 10f;
                        duplicatorSearch.minDistanceFilter = 0f;
                        duplicatorSearch.maxDistanceFilter = 30f + num;
                        duplicatorSearch.filterByDistinctEntity = false;
                        duplicatorSearch.filterByLoS = true;
                        duplicatorSearch.sortMode = SortMode.DistanceAndAngle;
                        MysticsItemsDuplicatorLocator duplicator = duplicatorSearch.SearchCandidatesForSingleTarget(InstanceTracker.GetInstancesList<MysticsItemsDuplicatorLocator>());
                        if (duplicator)
                        {
                            targetInfo.obj = duplicator.gameObject;
                            targetInfo.indicator.visualizerPrefab = crosshairPrefab;
                            targetInfo.indicator.targetTransform = duplicator.childTransform;
                        }
                        else
                        {
                            targetInfo.Invalidate();
                        }
                        targetInfo.indicator.active = duplicator;
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
                    PurchaseInteraction purchaseInteraction = targetInfo.obj.GetComponent<MysticsItemsDuplicatorLocator>().purchaseInteraction;
                    purchaseInteraction.SetAvailable(false);
                    purchaseInteraction.lockGameObject = null;
                    ShopTerminalBehavior shopTerminalBehavior = targetInfo.obj.GetComponent<MysticsItemsDuplicatorLocator>().shopTerminalBehavior;
                    EffectManager.SimpleEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniRecycleEffect"), shopTerminalBehavior.pickupDisplay.transform.position, Quaternion.identity, true);
                    shopTerminalBehavior.SetHasBeenPurchased(true);
                    for (var i = 0; i < amount; i++)
                        PickupDropletController.CreatePickupDroplet(
                            shopTerminalBehavior.pickupIndex,
                            (shopTerminalBehavior.dropTransform ? shopTerminalBehavior.dropTransform : shopTerminalBehavior.transform).position,
                            shopTerminalBehavior.transform.TransformVector(shopTerminalBehavior.dropVelocity)
                        );
                    shopTerminalBehavior.SetNoPickup();
                    targetInfo.Invalidate();
                    return true;
                }
            }
            return false;
        }

        public class DuplicatorSearch : BaseDirectionalSearch<MysticsItemsDuplicatorLocator, DuplicatorSearchSelector, DuplicatorSearchFilter>
        {
            public DuplicatorSearch() : base(default(DuplicatorSearchSelector), default(DuplicatorSearchFilter))
            {
            }

            public DuplicatorSearch(DuplicatorSearchSelector selector, DuplicatorSearchFilter candidateFilter) : base(selector, candidateFilter)
            {
            }
        }

        public struct DuplicatorSearchSelector : IGenericWorldSearchSelector<MysticsItemsDuplicatorLocator>
        {
            public Transform GetTransform(MysticsItemsDuplicatorLocator source)
            {
                if (source.shopTerminalBehavior && source.shopTerminalBehavior.pickupDisplay)
                {
                    return source.shopTerminalBehavior.pickupDisplay.transform;
                }
                return source.childTransform;
            }

            public GameObject GetRootObject(MysticsItemsDuplicatorLocator source)
            {
                return source.gameObject;
            }
        }

        public struct DuplicatorSearchFilter : IGenericDirectionalSearchFilter<MysticsItemsDuplicatorLocator>
        {
            public bool PassesFilter(MysticsItemsDuplicatorLocator source)
            {
                return source.purchaseInteraction.available && source.shopTerminalBehavior.CurrentPickupIndex() != PickupIndex.none;
            }
        }

        public class MysticsItemsDuplicatorLocator : MonoBehaviour
        {
            public PurchaseInteraction purchaseInteraction;
            public ShopTerminalBehavior shopTerminalBehavior;
            public Transform childTransform;

            public void Awake()
            {
                purchaseInteraction = gameObject.GetComponent<PurchaseInteraction>();
                shopTerminalBehavior = gameObject.GetComponent<ShopTerminalBehavior>();
            }

            public void OnEnable()
            {
                InstanceTracker.Add<MysticsItemsDuplicatorLocator>(this);
            }

            public void OnDisable()
            {
                InstanceTracker.Remove<MysticsItemsDuplicatorLocator>(this);
            }
        }
    }
}
