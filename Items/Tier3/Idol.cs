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
using static MysticsItems.BalanceConfigManager;

namespace MysticsItems.Items
{
    public class Idol : BaseItem
    {
        public static GameObject idolHUDIndicator;

        public static ConfigurableValue<float> goldCap = new ConfigurableValue<float>(
            "Item: Super Idol",
            "GoldCap",
            400f,
            "Gold required for full buff power (at starting difficulty level)\r\nFor comparison, a Small Chest costs $25, a Large Chest - $50, a Legendary Chest - $400.",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_IDOL_DESC"
            }
        );
        public static ConfigurableValue<float> healthBonus = new ConfigurableValue<float>(
            "Item: Super Idol",
            "HealthBonus",
            100f,
            "Extra maximum HP at full buff power (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_IDOL_DESC"
            }
        );
        public static ConfigurableValue<float> armorBonus = new ConfigurableValue<float>(
            "Item: Super Idol",
            "ArmorBonus",
            90f,
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
            itemDef.tier = ItemTier.Tier3;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Healing
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
            };

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;

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

        public static float CalculateIdolBonus(CharacterMaster master, int itemCount)
        {
            return Mathf.Clamp01(master.money / ((goldCap / (float)itemCount) * Stage.instance.entryDifficultyCoefficient));
        }
    }
}
