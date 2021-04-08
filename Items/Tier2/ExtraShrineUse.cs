using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace MysticsItems.Items
{
    public class ExtraShrineUse : BaseItem
    {
        public override void PreLoad()
        {
            itemDef.name = "ExtraShrineUse";
            itemDef.tier = ItemTier.Tier2;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy
            };
        }

        public override void OnLoad()
        {
            SetAssets("Hexahedral Monolith");
            Main.HopooShaderToMaterial.Standard.Gloss(GetModelMaterial());
            GetModelMaterial().SetFloat("_Smoothness", 0.5f);

            MysticsItemsExtraShrineUseBehaviour.displayPrefab = PrefabAPI.InstantiateClone(model, Main.TokenPrefix + "MysteriousMonolithDisplay", false);
            MysticsItemsExtraShrineUseBehaviour.displayPrefab.transform.localScale *= 0.1f;

            On.RoR2.CharacterBody.OnInventoryChanged += (orig, self) =>
            {
                orig(self);
                int itemCount = BaseItem.GetTeamItemCount(MysticsItemsContent.Items.ExtraShrineUse);
                foreach (MysticsItemsExtraShrineUseBehaviour extraShrineUseBehaviour in MysticsItemsExtraShrineUseBehaviour.activeShrines)
                {
                    UpdateShrine(extraShrineUseBehaviour, itemCount);
                }
            };

            On.RoR2.PurchaseInteraction.Awake += (orig, self) =>
            {
                orig(self);
                GenericDisplayNameProvider genericDisplayNameProvider = self.GetComponent<GenericDisplayNameProvider>();
                if (genericDisplayNameProvider && genericDisplayNameProvider.displayToken.Contains("SHRINE"))
                    self.gameObject.AddComponent<MysticsItemsExtraShrineUseBehaviour>();
            };
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
            int itemCount = BaseItem.GetTeamItemCount(MysticsItemsContent.Items.ExtraShrineUse);
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
