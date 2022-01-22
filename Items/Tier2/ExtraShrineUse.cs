using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using static MysticsItems.BalanceConfigManager;

namespace MysticsItems.Items
{
    public class ExtraShrineUse : BaseItem
    {
        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_ExtraShrineUse";
            itemDef.tier = ItemTier.Tier2;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Hexahedral Monolith/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Hexahedral Monolith/Icon.png");
            HopooShaderToMaterial.Standard.Apply(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial);
            HopooShaderToMaterial.Standard.Gloss(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial);
            HopooShaderToMaterial.Standard.Emission(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial, -3f);
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "GunMeshR", new Vector3(-0.13951F, -0.01505F, 0.13151F), new Vector3(0F, 0F, 90F), new Vector3(0.00856F, 0.00856F, 0.00856F));
                AddDisplayRule("HuntressBody", "UpperArmL", new Vector3(0.06909F, 0.10681F, -0.00977F), new Vector3(3.66903F, 357.0302F, 178.0301F), new Vector3(0.01358F, 0.01358F, 0.01358F));
                AddDisplayRule("Bandit2Body", "MainWeapon", new Vector3(-0.05477F, 0.2274F, -0.04443F), new Vector3(359.4865F, 89.48757F, 206.7464F), new Vector3(0.0135F, 0.00485F, 0.00485F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(-1.77361F, 2.53066F, 1.76556F), new Vector3(0F, 90F, 90F), new Vector3(0.10065F, 0.10065F, 0.10065F));
                AddDisplayRule("EngiBody", "LowerArmR", new Vector3(0.0113F, 0.13437F, -0.05836F), new Vector3(1.34564F, 72.93568F, 188.458F), new Vector3(0.01476F, 0.01476F, 0.01476F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.035F, 0.89075F, -1.47928F), new Vector3(0F, 90F, 303.695F), new Vector3(0.07847F, 0.07847F, 0.07847F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.03562F, 1.40676F, -1.39837F), new Vector3(0F, 90F, 303.1705F), new Vector3(0.08093F, 0.09844F, 0.07912F));
                AddDisplayRule("MageBody", "Chest", new Vector3(-0.10398F, 0.07562F, -0.31389F), new Vector3(359.7522F, 90.11677F, 8.18118F), new Vector3(0.01236F, 0.01035F, 0.00964F));
                AddDisplayRule("MageBody", "Chest", new Vector3(0.11942F, 0.07423F, -0.30928F), new Vector3(359.136F, 95.88205F, 8.14244F), new Vector3(0.01236F, 0.01035F, 0.00787F));
                AddDisplayRule("MercBody", "HandL", new Vector3(0.01326F, 0.1146F, 0.04565F), new Vector3(88.10731F, 183.3846F, 89.99922F), new Vector3(0.00961F, 0.00961F, 0.00965F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(0.69564F, -0.5422F, -0.29426F), new Vector3(46.13942F, 241.7613F, 12.79626F), new Vector3(0.03647F, 0.03647F, 0.03647F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(0.01517F, -0.06288F, -0.17121F), new Vector3(90F, 90F, 0F), new Vector3(0.0207F, 0.0207F, 0.0207F));
                AddDisplayRule("CrocoBody", "SpineChest1", new Vector3(1.39693F, -0.10569F, -0.18201F), new Vector3(55.10429F, 175.6143F, 292.3791F), new Vector3(0.1379F, 0.1379F, 0.1379F));
                AddDisplayRule("CaptainBody", "MuzzleGun", new Vector3(0.00467F, 0.05642F, -0.1194F), new Vector3(357.9892F, 90.52832F, 89.76476F), new Vector3(0.05388F, 0.01322F, 0.0146F));
                AddDisplayRule("BrotherBody", "UpperArmL", BrotherInfection.green, new Vector3(0.06646F, 0.22781F, -0.00154F), new Vector3(77.05167F, 128.9087F, 289.6219F), new Vector3(0.04861F, 0.10534F, 0.10724F));
                AddDisplayRule("ScavBody", "Stomach", new Vector3(-0.92389F, 11.6509F, -5.90638F), new Vector3(20.93637F, 118.4181F, 332.9505F), new Vector3(0.24839F, 0.25523F, 0.24839F));
            };

            MysticsItemsExtraShrineUseBehaviour.displayPrefab = PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, "MysticsItems_MysteriousMonolithDisplay", false);
            MysticsItemsExtraShrineUseBehaviour.displayPrefab.transform.localScale *= 0.1f;

            On.RoR2.CharacterBody.OnInventoryChanged += (orig, self) =>
            {
                orig(self);
                if (self.teamComponent && self.teamComponent.teamIndex == TeamIndex.Player) UpdateAllShrines();
            };

            On.RoR2.CharacterBody.OnDeathStart += (orig, self) =>
            {
                orig(self);
                if (self.teamComponent && self.teamComponent.teamIndex == TeamIndex.Player) UpdateAllShrines();
            };

            On.RoR2.CharacterBody.Start += (orig, self) =>
            {
                orig(self);
                if (self.teamComponent && self.teamComponent.teamIndex == TeamIndex.Player) UpdateAllShrines();
            };

            On.RoR2.PurchaseInteraction.Awake += (orig, self) =>
            {
                orig(self);
                GenericDisplayNameProvider genericDisplayNameProvider = self.GetComponent<GenericDisplayNameProvider>();
                if (genericDisplayNameProvider && genericDisplayNameProvider.displayToken.Contains("SHRINE"))
                    self.gameObject.AddComponent<MysticsItemsExtraShrineUseBehaviour>();
            };
        }

        public static void UpdateAllShrines()
        {
            int itemCount = Util.GetItemCountForTeam(TeamIndex.Player, MysticsItemsContent.Items.MysticsItems_ExtraShrineUse.itemIndex, true);
            foreach (MysticsItemsExtraShrineUseBehaviour extraShrineUseBehaviour in MysticsItemsExtraShrineUseBehaviour.activeShrines)
            {
                UpdateShrine(extraShrineUseBehaviour, itemCount);
            }
        }

        public static void UpdateShrine(MysticsItemsExtraShrineUseBehaviour self, int itemCount)
        {
            int increaseBy = itemCount;

            increaseBy -= self.increasedPurchaseCount;
            self.increasedPurchaseCount += increaseBy;
            foreach (MonoBehaviour monoBehaviour in self.GetComponents<MonoBehaviour>())
            {
                FieldInfo maxPurchaseCountField = monoBehaviour.GetType().GetField("maxPurchaseCount", Main.bindingFlagAll);
                if (maxPurchaseCountField != null)
                {
                    maxPurchaseCountField.SetValue(monoBehaviour, (int)maxPurchaseCountField.GetValue(monoBehaviour) + increaseBy);
                }
            }
            if (self.display) self.display.SetActive(increaseBy > 0);
        }

        public static void UpdateShrine(MysticsItemsExtraShrineUseBehaviour self)
        {
            int itemCount = Util.GetItemCountForTeam(TeamIndex.Player, MysticsItemsContent.Items.MysticsItems_ExtraShrineUse.itemIndex, true);
            UpdateShrine(self, itemCount);
        }

        public class MysticsItemsExtraShrineUseBehaviour : MonoBehaviour
        {
            public int increasedPurchaseCount = 0;
            public GameObject display;
            public static GameObject displayPrefab;

            public void Start()
            {
                float radius = 1f;
                ModelLocator modelLocator = GetComponent<ModelLocator>();
                if (modelLocator)
                {
                    Transform modelTransform = modelLocator.modelTransform;
                    if (modelTransform)
                    {
                        CapsuleCollider capsuleCollider = modelTransform.gameObject.GetComponent<CapsuleCollider>();
                        if (capsuleCollider) radius = capsuleCollider.radius;
                        SphereCollider sphereCollider = modelTransform.gameObject.GetComponent<SphereCollider>();
                        if (sphereCollider) radius = sphereCollider.radius;
                        BoxCollider boxCollider = modelTransform.gameObject.GetComponent<BoxCollider>();
                        if (boxCollider) radius = (boxCollider.size.x + boxCollider.size.y + boxCollider.size.z) / 3f;
                    }
                }

                display = Instantiate(displayPrefab, transform.position, Quaternion.identity);
                // move it to somewhere around the shrine
                display.transform.up = transform.up;
                float randomAngle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                Vector2 randomCirclePoint = new Vector2(Mathf.Sin(randomAngle), Mathf.Cos(randomAngle)).normalized * (radius * 1.5f);
                display.transform.Translate(new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y), Space.Self);
                // align it to the ground
                RaycastHit raycastHit;
                if (Physics.Raycast(new Ray(display.transform.position + display.transform.up * 1f, -display.transform.up), out raycastHit, 100f, LayerIndex.world.mask))
                {
                    display.transform.position = raycastHit.point + raycastHit.normal * 0.65f;
                    display.transform.up = raycastHit.normal;
                }
                // add slight tilt
                float tiltRange = 30f;
                display.transform.rotation *= Quaternion.Euler(UnityEngine.Random.Range(-tiltRange, tiltRange), UnityEngine.Random.Range(-tiltRange, tiltRange), UnityEngine.Random.Range(-tiltRange, tiltRange));

                UpdateShrine(this);
            }

            public void OnEnable()
            {
                activeShrines.Add(this);
            }

            public void OnDisable()
            {
                activeShrines.Remove(this);
            }

            public void OnDestroy()
            {
                if (display) Destroy(display);
            }

            public static List<MysticsItemsExtraShrineUseBehaviour> activeShrines = new List<MysticsItemsExtraShrineUseBehaviour>();
        }
    }
}
