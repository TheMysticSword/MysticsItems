using RoR2;
using RoR2.DirectionalSearch;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;
using System.Collections.Generic;

namespace MysticsItems.Equipment
{
    public class PrinterHacker : BaseEquipment
    {
        public static GameObject crosshairPrefab;

        public override void PreAdd()
        {
            equipmentDef.name = "PrinterHacker";
            equipmentDef.cooldown = 45f;
            equipmentDef.canDrop = true;
            equipmentDef.enigmaCompatible = true;

            SetAssets("Wirehack Wrench");
            Main.HopooShaderToMaterial.Standard.Gloss(GetModelMaterial(), 1f, 20f);
            CopyModelToFollower();
            model.transform.Find("d4b43750924799f8").Rotate(new Vector3(0f, 0f, -30f), Space.Self);
            SetModelPanelDistance(5f, 10f);
            AddDisplayRule((int)Main.CommonBodyIndices.Commando, "Stomach", new Vector3(-0.163F, 0.092F, -0.036F), new Vector3(356.022F, 118.071F, 26.4F), new Vector3(0.024F, 0.024F, 0.024F));
            AddDisplayRule("mdlHuntress", "Pelvis", new Vector3(-0.088F, -0.085F, 0.059F), new Vector3(0.679F, 36.762F, 196.086F), new Vector3(0.019F, 0.019F, 0.019F));
            AddDisplayRule("mdlToolbot", "Hip", new Vector3(-1.202F, 0.577F, -0.876F), new Vector3(0F, 180F, 180F), new Vector3(0.349F, 0.349F, 0.349F));
            AddDisplayRule("mdlEngi", "Pelvis", new Vector3(-0.178F, 0.078F, 0.157F), new Vector3(11.745F, 186.295F, 185.936F), new Vector3(0.047F, 0.047F, 0.047F));
            AddDisplayRule("mdlMage", "Pelvis", new Vector3(-0.172F, -0.067F, -0.078F), new Vector3(7.421F, 5.596F, 187.29F), new Vector3(0.027F, 0.027F, 0.027F));
            AddDisplayRule("mdlMerc", "Chest", new Vector3(-0.115F, 0.032F, 0.083F), new Vector3(18.292F, 60.198F, 185.734F), new Vector3(0.027F, 0.027F, 0.027F));
            AddDisplayRule("mdlTreebot", "FlowerBase", new Vector3(-0.485F, 0.701F, -0.803F), new Vector3(26.173F, 24.306F, 86.838F), new Vector3(0.061F, 0.061F, 0.061F));
            AddDisplayRule("mdlLoader", "Pelvis", new Vector3(-0.216F, -0.016F, -0.022F), new Vector3(342.363F, 183.205F, 159.555F), new Vector3(0.045F, 0.045F, 0.045F));
            AddDisplayRule("mdlCroco", "SpineStomach1", new Vector3(0.845F, 0.495F, 1.289F), new Vector3(74.633F, 327.618F, 247.859F), new Vector3(0.361F, 0.361F, 0.361F));
            AddDisplayRule("mdlCaptain", "Stomach", new Vector3(-0.195F, 0.128F, 0.126F), new Vector3(336.504F, 156.734F, 358.159F), new Vector3(0.041F, 0.041F, 0.041F));
            AddDisplayRule("mdlScav", "Backpack", new Vector3(-5.969F, 10.94F, 0.665F), new Vector3(338.478F, 350.544F, 54.934F), new Vector3(1.363F, 1.363F, 1.363F));
            AddDisplayRule("mdlEquipmentDrone", "GunBarrelBase", new Vector3(0F, 0F, 1.1F), new Vector3(52.577F, 0F, 0.001F), new Vector3(0.283F, 0.283F, 0.283F));

            crosshairPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/WoodSpriteIndicator"), Main.TokenPrefix + "PrinterHackerIndicator", false);
            Object.Destroy(crosshairPrefab.GetComponentInChildren<Rewired.ComponentControls.Effects.RotateAroundAxis>());
            crosshairPrefab.GetComponentInChildren<SpriteRenderer>().sprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Equipment/Wirehack Wrench/Crosshair.png");
            crosshairPrefab.GetComponentInChildren<SpriteRenderer>().color = new Color32(255, 235, 75, 255);
            crosshairPrefab.GetComponentInChildren<SpriteRenderer>().transform.rotation = Quaternion.identity;
            crosshairPrefab.GetComponentInChildren<TMPro.TextMeshPro>().color = new Color32(255, 235, 75, 255);
            while (crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.length > 0) crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.RemoveKey(0);
            crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(0f, 2f);
            crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(0.5f, 1f);
            crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(1f, 1f);

            SetupDuplicator(Resources.Load<GameObject>("Prefabs/NetworkedObjects/Chest/Duplicator"));
            SetupDuplicator(Resources.Load<GameObject>("Prefabs/NetworkedObjects/Chest/DuplicatorLarge"));
            SetupDuplicator(Resources.Load<GameObject>("Prefabs/NetworkedObjects/Chest/DuplicatorMilitary"));
            SetupDuplicator(Resources.Load<GameObject>("Prefabs/NetworkedObjects/Chest/DuplicatorWild"));
        }

        public static void SetupDuplicator(GameObject gameObject)
        {
            Main.modifiedPrefabs.Add(gameObject);
            gameObject.GetComponentInChildren<EntityLocator>().gameObject.AddComponent<MysticsItemsDuplicatorLocator>();
        }

        public override void OnAdd()
        {
            On.RoR2.EquipmentSlot.Update += (orig, self) =>
            {
                orig(self);
                if (self.equipmentIndex == equipmentIndex)
                {
                    CurrentTarget targetInfo = self.GetComponent<CurrentTarget>();
                    if (targetInfo)
                    {
                        DuplicatorSearch duplicatorSearch = new DuplicatorSearch();
                        float num;
                        Ray aimRay = CameraRigController.ModifyAimRayIfApplicable(GetAimRay(self), self.gameObject, out num);
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
                            targetInfo.indicator.targetTransform = duplicator.transform;
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
            CurrentTarget targetInfo = equipmentSlot.GetComponent<CurrentTarget>();
            if (targetInfo)
            {
                if (targetInfo.obj)
                {
                    PurchaseInteraction purchaseInteraction = targetInfo.obj.GetComponent<MysticsItemsDuplicatorLocator>().purchaseInteraction;
                    purchaseInteraction.SetAvailable(false);
                    purchaseInteraction.lockGameObject = null;
                    ShopTerminalBehavior shopTerminalBehavior = targetInfo.obj.GetComponent<MysticsItemsDuplicatorLocator>().shopTerminalBehavior;
                    EffectManager.SimpleEffect(Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniRecycleEffect"), shopTerminalBehavior.pickupDisplay.transform.position, Quaternion.identity, true);
                    shopTerminalBehavior.DropPickup();
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
                return source.transform;
            }

            public GameObject GetRootObject(MysticsItemsDuplicatorLocator source)
            {
                return source.entity.gameObject;
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
            public GameObject entity;
            public PurchaseInteraction purchaseInteraction;
            public ShopTerminalBehavior shopTerminalBehavior;

            public void Awake()
            {
                entity = GetComponent<EntityLocator>().entity;
                purchaseInteraction = entity.GetComponent<PurchaseInteraction>();
                shopTerminalBehavior = entity.GetComponent<ShopTerminalBehavior>();
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
