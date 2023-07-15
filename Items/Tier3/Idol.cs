using RoR2;
using RoR2.UI;
using R2API.Utils;
using UnityEngine;
using System;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using static MysticsItems.LegacyBalanceConfigManager;
using UnityEngine.UI;

namespace MysticsItems.Items
{
    public class Idol : BaseItem
    {
        public static GameObject idolHUDIndicator;
        public static Sprite idolHUDGlowingIcon;
        public static Material idolHUDGlowingIconMaterial;

        public static ConfigurableValue<float> goldCap = new ConfigurableValue<float>(
            "Item: Super Idol",
            "GoldCap",
            800f,
            "Gold required for full buff power (at starting difficulty level)\r\nFor comparison, a Small Chest costs $25, a Large Chest - $50, a Legendary Chest - $400.",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_IDOL_DESC"
            }
        );
        public static ConfigurableValue<float> healthBonus = new ConfigurableValue<float>(
            "Item: Super Idol",
            "HealthBonus",
            60f,
            "Extra maximum HP at full buff power (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_IDOL_DESC"
            }
        );
        public static ConfigurableValue<float> armorBonus = new ConfigurableValue<float>(
            "Item: Super Idol",
            "ArmorBonus",
            60f,
            "Extra armor at full buff power",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_IDOL_DESC"
            }
        );

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_Idol";
            SetItemTierWhenAvailable(ItemTier.Tier3);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Healing,
                ItemTag.Utility
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Idol/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Idol/Icon.png");
            
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Chest", new Vector3(0.18821F, 0.52588F, 0.00182F), new Vector3(358.7003F, 312.6689F, 337.5352F), new Vector3(0.08767F, 0.08767F, 0.08767F));
                AddDisplayRule("HuntressBody", "UpperArmR", new Vector3(-0.02179F, -0.01052F, -0.11917F), new Vector3(357.3364F, 280.1227F, 114.9923F), new Vector3(0.09079F, 0.08448F, 0.09079F));
                AddDisplayRule("Bandit2Body", "UpperArmL", new Vector3(0.10079F, -0.00043F, 0.07363F), new Vector3(298.8222F, 332.7441F, 247.8996F), new Vector3(0.0641F, 0.0641F, 0.0641F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(-1.89821F, 3.01594F, 2.51729F), new Vector3(0F, 346.3047F, 0F), new Vector3(0.42416F, 0.45611F, 0.42416F));
                AddDisplayRule("EngiBody", "Chest", new Vector3(0F, 0.1055F, 0.29581F), new Vector3(9.44163F, 0F, 0F), new Vector3(0.0933F, 0.0933F, 0.0933F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(-0.70342F, 0.94393F, -0.28912F), new Vector3(0F, 244.9429F, 0F), new Vector3(0.20635F, 0.20635F, 0.20635F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(-0.62005F, 1.52319F, -0.7088F), new Vector3(0F, 257.2478F, 0F), new Vector3(0.20675F, 0.20675F, 0.20167F));
                AddDisplayRule("MageBody", "Chest", new Vector3(-0.14155F, 0.35904F, 0.00001F), new Vector3(0F, 0F, 21.02748F), new Vector3(0.06871F, 0.06871F, 0.06871F));
                AddDisplayRule("MercBody", "UpperArmL", new Vector3(0.06158F, -0.16948F, -0.00001F), new Vector3(0F, 0F, 221.5403F), new Vector3(0.09694F, 0.09694F, 0.09694F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(0.35723F, 0.72811F, 0.1532F), new Vector3(7.34207F, 52.03194F, 0F), new Vector3(0.23625F, 0.23625F, 0.23625F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(-0.13887F, 0.54848F, -0.11031F), new Vector3(4.72459F, 7.59919F, 7.84216F), new Vector3(0.08712F, 0.08712F, 0.08712F));
                AddDisplayRule("CrocoBody", "Chest", new Vector3(-3.39114F, 2.52565F, 1.61244F), new Vector3(320.5719F, 162.2655F, 341.2106F), new Vector3(0.895F, 0.895F, 0.895F));
                AddDisplayRule("CaptainBody", "Chest", new Vector3(0.32895F, 0.03454F, 0.05078F), new Vector3(3.45364F, 74.41675F, 19.36633F), new Vector3(0.07825F, 0.07825F, 0.07825F));
                AddDisplayRule("BrotherBody", "UpperArmR", BrotherInfection.red, new Vector3(-0.03472F, -0.0628F, 0.05088F), new Vector3(340.2432F, 34.20839F, 49.33977F), new Vector3(0.10485F, 0.10485F, 0.10485F));
                AddDisplayRule("ScavBody", "Chest", new Vector3(7.69683F, 4.43335F, 0.0701F), new Vector3(344.7279F, 181.2736F, 27.68404F), new Vector3(1.98721F, 1.98721F, 1.98721F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Chest", new Vector3(-0.15735F, 0.48555F, -0.15454F), new Vector3(5.92817F, 15.42453F, 20.44858F), new Vector3(0.08192F, 0.08192F, 0.08192F));
                AddDisplayRule("RailgunnerBody", "Chest", new Vector3(-0.14274F, 0.23206F, -0.10299F), new Vector3(344.693F, 0F, 23.5299F), new Vector3(0.07586F, 0.07586F, 0.07586F));
                AddDisplayRule("VoidSurvivorBody", "Neck", new Vector3(-0.17193F, 0.12368F, -0.12447F), new Vector3(347.7072F, 40.52266F, 3.19146F), new Vector3(0.06408F, 0.06408F, 0.06408F));
            };

            var mainOverlay = Main.AssetBundle.LoadAsset<Material>("Assets/Items/Idol/matIdolOverlay.mat");
            var progressOverlaySteps = 3;
            var progressPerStep = (float)1 / (float)progressOverlaySteps;
            for (var i = 1; i <= progressOverlaySteps; i++)
            {
                var step = progressPerStep * i;

                Material matStep = null;
                if (i < progressOverlaySteps)
                {
                    
                    matStep = Material.Instantiate(mainOverlay);
                    matStep.name += "_Step" + i;
                    matStep.SetColor("_Color", matStep.GetColor("_Color") * step);
                    matStep.SetFloat("_OffsetMult", step);
                }
                else matStep = mainOverlay;

                Overlays.CreateOverlay(matStep, (model) =>
                {
                    if (model.body && model.body.inventory && model.body.master)
                    {
                        int itemCount = model.body.inventory.GetItemCount(itemDef);
                        if (itemCount > 0)
                        {
                            var idolBonus = CalculateIdolBonus(model.body.master, itemCount);
                            return idolBonus >= step && idolBonus < step + progressPerStep;
                        }
                    }
                    return false;
                });
            }

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;

            idolHUDIndicator = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Idol/IdolHUDIndicator.prefab");
            idolHUDIndicator.AddComponent<HudElement>();
            idolHUDIndicator.transform.localScale *= 4f;
            MysticsItemsIdolIndicator indicatorComponent = idolHUDIndicator.AddComponent<MysticsItemsIdolIndicator>();
            indicatorComponent.fillingRenderer = idolHUDIndicator.transform.Find("Filling").GetComponent<Renderer>();
            UnityEngine.UI.LayoutElement layoutElement = idolHUDIndicator.AddComponent<UnityEngine.UI.LayoutElement>();
            layoutElement.ignoreLayout = true;
            idolHUDIndicator.transform.localPosition += Vector3.down * 10f;

            HUD.onHudTargetChangedGlobal += HUD_onHudTargetChangedGlobal;

            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;

            On.RoR2.CharacterMaster.GiveMoney += CharacterMaster_GiveMoney;

            RoR2Application.onNextUpdate += () =>
            {
                if (MysticsItemsIdolIndicator.dirty)
                {
                    MysticsItemsIdolIndicator.dirty = false;
                    MysticsItemsIdolIndicator.RefreshAll();
                }
            };

            idolHUDGlowingIcon = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Idol/IconGlowing.png");
            idolHUDGlowingIconMaterial = Main.AssetBundle.LoadAsset<Material>("Assets/Items/Idol/matIdoHUDGlowingIcon.mat");
            On.RoR2.UI.ItemIcon.SetItemIndex += ItemIcon_SetItemIndex;
        }

        private void ItemIcon_SetItemIndex(On.RoR2.UI.ItemIcon.orig_SetItemIndex orig, ItemIcon self, ItemIndex newItemIndex, int newItemCount)
        {
            orig(self, newItemIndex, newItemCount);
            var shouldDisplay = newItemIndex == itemDef.itemIndex && newItemCount > 0;
            var fillIcon = self.GetComponent<MysticsItemsSuperIdolHUDIcon>();
            if (shouldDisplay != fillIcon)
            {
                if (shouldDisplay)
                {
                    var itemInventoryDisplay = self.GetComponentInParent<ItemInventoryDisplay>();
                    if (itemInventoryDisplay && itemInventoryDisplay.inventory)
                    {
                        var master = itemInventoryDisplay.inventory.GetComponent<CharacterMaster>();
                        if (master)
                        {
                            fillIcon = self.gameObject.AddComponent<MysticsItemsSuperIdolHUDIcon>();
                            fillIcon.master = master;
                            fillIcon.fillAmount = CalculateIdolBonus(master, newItemCount);
                        }
                    }
                }
                else
                {
                    UnityEngine.Object.Destroy(fillIcon);
                    fillIcon = null;
                }
            }
            if (shouldDisplay && fillIcon)
            {
                fillIcon.itemCount = newItemCount;
                fillIcon.UpdateTransform();
            }
        }

        public class MysticsItemsSuperIdolHUDIcon : MonoBehaviour
        {
            public GameObject iconInstance;
            public RawImage image;
            public Material material;
            public CharacterMaster master;
            public int itemCount;
            public float fillAmount;
            private float fillAmountVelocity;
            public RectTransform rectTransform;
            public RectTransform parentRect;

            public void Awake()
            {
                iconInstance = new GameObject("MysticsItemsSuperIdolHUDIcon");
                iconInstance.transform.SetParent(transform);

                image = iconInstance.AddComponent<RawImage>();
                image.texture = idolHUDGlowingIcon.texture;
                image.raycastTarget = true;
                image.maskable = true;
                image.material = material = Instantiate(idolHUDGlowingIconMaterial);

                parentRect = transform as RectTransform;

                rectTransform = image.rectTransform;
                UpdateTransform();
            }

            public void UpdateTransform()
            {
                rectTransform.localPosition = Vector2.zero;
                rectTransform.localRotation = Quaternion.identity;
                rectTransform.localScale = Vector2.one;
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0f, 1f);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentRect.rect.width);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentRect.rect.height);
                rectTransform.offsetMin = -Vector2.one * parentRect.rect.width / 2f;
                rectTransform.offsetMax = Vector2.one * parentRect.rect.height / 2f;
            }

            public void Update()
            {
                if (master)
                {
                    var idolBonus = CalculateIdolBonus(master, itemCount);
                    fillAmount = Mathf.SmoothDamp(fillAmount, idolBonus, ref fillAmountVelocity, 1f);

                    if (material) material.SetVector("_RenderPortion", new Vector4(0f, 1f, 0f, fillAmount));
                }
            }

            public void OnDestroy()
            {
                Destroy(iconInstance);
                Destroy(material);
            }
        }

        private void CharacterMaster_GiveMoney(On.RoR2.CharacterMaster.orig_GiveMoney orig, CharacterMaster self, uint amount)
        {
            orig(self, amount);
            if (NetworkServer.active)
            {
                if (self.inventory && self.inventory.GetItemCount(itemDef) > 0)
                {
                    var body = self.GetBody();
                    if (body)
                    {
                        Utils.ForceRecalculateStats(body);
                    }
                }
            }
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            MysticsItemsIdolIndicator.dirty = true;
        }

        private void HUD_onHudTargetChangedGlobal(HUD hud)
        {
            MysticsItemsIdolIndicator.dirty = true;
        }

        [RequireComponent(typeof(HudElement))]
        public class MysticsItemsIdolIndicator : MonoBehaviour
        {
            public static bool dirty = false;

            public static void RefreshAll()
            {
                foreach (var hudInstance in HUD.readOnlyInstanceList) RefreshForHUDInstance(hudInstance);
            }

            public static void RefreshForHUDInstance(HUD hudInstance)
            {
                CharacterMaster targetMaster = hudInstance.targetMaster;
                Inventory inventory = targetMaster ? targetMaster.inventory : null;
                int itemCount = inventory ? inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_Idol) : 0;

                var shouldDisplay = itemCount > 0;

                MysticsItemsIdolIndicator targetIndicatorInstance = instancesList.FirstOrDefault(x => x.hud == hudInstance);
                
                if (targetIndicatorInstance != shouldDisplay)
                {
                    if (!targetIndicatorInstance)
                    {
                        if (hudInstance.mainUIPanel)
                        {
                            var transform = (RectTransform)hudInstance.mainUIPanel.transform.Find("SpringCanvas/UpperLeftCluster");
                            if (transform)
                            {
                                targetIndicatorInstance = Instantiate(idolHUDIndicator, transform).GetComponent<MysticsItemsIdolIndicator>();
                                targetIndicatorInstance.hud = hudInstance;
                                var layoutGroup = transform.GetComponent<UnityEngine.UI.LayoutGroup>();
                                if (layoutGroup) {
                                    targetIndicatorInstance.transform.localPosition += Vector3.down * layoutGroup.minHeight;
                                }
                            }
                        }
                    }
                    else
                    {
                        Destroy(targetIndicatorInstance.gameObject);
                    }
                }

                if (shouldDisplay && targetIndicatorInstance)
                {
                    targetIndicatorInstance.targetMaster = targetMaster;
                    targetIndicatorInstance.itemCount = itemCount;
                }
            }

            public static List<MysticsItemsIdolIndicator> instancesList = new List<MysticsItemsIdolIndicator>();

            public void Awake()
            {
                materialPropertyBlock = new MaterialPropertyBlock();
            }

            public void Update()
            {
                if (targetMaster)
                {
                    var idolBonus = CalculateIdolBonus(targetMaster, itemCount);

                    fillAmount = Mathf.SmoothDamp(fillAmount, idolBonus, ref fillAmountVelocity, 1f);

                    if (fillingRenderer)
                    {
                        fillingRenderer.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetVector("_RenderPortion", new Vector4(0f, 1f, 0f, fillAmount));
                        fillingRenderer.SetPropertyBlock(materialPropertyBlock);
                    }
                }
            }

            public void OnEnable()
            {
                instancesList.Add(this);
            }

            public void OnDisable()
            {
                instancesList.Remove(this);
            }

            public HUD hud;
            public CharacterMaster targetMaster;
            public int itemCount;
            public Renderer fillingRenderer;
            public MaterialPropertyBlock materialPropertyBlock;
            public float fillAmount = 0f;
            private float fillAmountVelocity;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory && sender.master)
            {
                int itemCount = sender.inventory.GetItemCount(itemDef);
                if (itemCount > 0)
                {
                    var idolBonus = CalculateIdolBonus(sender.master, itemCount);
                    args.healthMultAdd += healthBonus / 100f * idolBonus;
                    args.armorAdd += armorBonus * idolBonus;
                }
            }
        }

        public static float CalculateIdolCap(int itemCount)
        {
            var x = Stage.instance.entryDifficultyCoefficient;
            var scaledCapMultiplier = x + Mathf.Min(Mathf.Pow(x / 6f, 2f), 40f);
            return Mathf.Min((goldCap / (float)itemCount) * scaledCapMultiplier, float.MaxValue);
        }

        public static float CalculateIdolBonus(CharacterMaster master, int itemCount)
        {
            return Mathf.Clamp01(master.money / CalculateIdolCap(itemCount));
        }
    }
}
