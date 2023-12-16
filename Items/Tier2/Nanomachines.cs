using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Collections.Generic;
using RoR2.Audio;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Items
{
    public class Nanomachines : BaseItem
    {
        public static ConfigurableValue<float> minHealth = new ConfigurableValue<float>(
            "Item: Inoperative Nanomachines",
            "MinHealth",
            30f,
            "How much health should the user have at least to trigger the item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_NANOMACHINES_PICKUP",
                "ITEM_MYSTICSITEMS_NANOMACHINES_DESC"
            }
        );
        public static ConfigurableValue<float> maxHealth = new ConfigurableValue<float>(
            "Item: Inoperative Nanomachines",
            "MaxHealth",
            70f,
            "How much health should the user have at most to trigger the item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_NANOMACHINES_PICKUP",
                "ITEM_MYSTICSITEMS_NANOMACHINES_DESC"
            }
        );

        public static GameObject activeEffectPrefab;

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_Nanomachines";
            SetItemTierWhenAvailable(ItemTier.Tier2);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Proximity Nanobots/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Proximity Nanobots/Icon.png");
            MysticsItemsContent.Resources.unlockableDefs.Add(GetUnlockableDef());
            itemDisplayPrefab = PrepareItemDisplayModel(PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Proximity Nanobots/FollowerModel.prefab")));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "CalfR", new Vector3(-0.02165F, 0.14397F, -0.0533F), new Vector3(283.8492F, 276.3043F, 98.7495F), new Vector3(0.02579F, 0.02579F, 0.02579F));
                AddDisplayRule("HuntressBody", "CalfR", new Vector3(0.06795F, 0.2092F, -0.04452F), new Vector3(278.7839F, 1.2873F, 327.3282F), new Vector3(0.0243F, 0.0243F, 0.0243F));
                AddDisplayRule("Bandit2Body", "CalfR", new Vector3(-0.00552F, 0.18029F, -0.06738F), new Vector3(313.3941F, 92.65309F, 275.5553F), new Vector3(0.02547F, 0.02547F, 0.02547F));
                AddDisplayRule("ToolbotBody", "CalfR", new Vector3(0.62627F, 2.21605F, 0.05282F), new Vector3(28.88263F, 6.02953F, 270.3149F), new Vector3(0.21708F, 0.21708F, 0.21708F));
                AddDisplayRule("EngiBody", "CalfR", new Vector3(0.0164F, 0.12436F, -0.08442F), new Vector3(277.2202F, 180F, 176.1767F), new Vector3(0.02989F, 0.02989F, 0.02989F));
                AddDisplayRule("EngiTurretBody", "LegBar3", new Vector3(0.02396F, 0.35298F, 0.18667F), new Vector3(6.86833F, 93.31924F, 90.39735F), new Vector3(0.08437F, 0.08437F, 0.08437F));
                AddDisplayRule("EngiWalkerTurretBody", "LegBar3", new Vector3(0.00002F, 0.17597F, 0.2548F), new Vector3(62.81832F, 90.86576F, 90.97321F), new Vector3(0.102F, 0.102F, 0.102F));
                AddDisplayRule("MageBody", "CalfR", new Vector3(0.0241F, 0.02332F, -0.03891F), new Vector3(270.3604F, 180.0011F, 148.1224F), new Vector3(0.02423F, 0.02423F, 0.02423F));
                AddDisplayRule("MercBody", "CalfR", new Vector3(0.00001F, 0.20225F, -0.03167F), new Vector3(273.8359F, 180F, 180F), new Vector3(0.0241F, 0.0241F, 0.0241F));
                AddDisplayRule("TreebotBody", "Base", new Vector3(0.5353F, 0.5163F, 0.3298F), new Vector3(3.67085F, 335.3158F, 286.064F), new Vector3(0.05916F, 0.05916F, 0.05916F));
                AddDisplayRule("LoaderBody", "CalfR", new Vector3(0.0098F, 0.11872F, -0.09638F), new Vector3(355.0381F, 82.86241F, 270.4088F), new Vector3(0.02775F, 0.02775F, 0.02775F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(0.03485F, 3.12425F, 1.55491F), new Vector3(357.2307F, 270.6913F, 255.9768F), new Vector3(0.38331F, 0.38331F, 0.38331F));
                AddDisplayRule("CaptainBody", "CalfR", new Vector3(0F, 0.26157F, -0.03875F), new Vector3(359.5825F, 90.05307F, 262.7561F), new Vector3(0.02424F, 0.02424F, 0.02424F));
                AddDisplayRule("BrotherBody", "CalfR", BrotherInfection.green, new Vector3(-0.02875F, 0.09234F, -0.01974F), new Vector3(0F, 297.8474F, 0F), new Vector3(0.05206F, 0.05206F, 0.05206F));
                AddDisplayRule("ScavBody", "ThighR", new Vector3(-0.65041F, 1.77553F, 3.10771F), new Vector3(338.4114F, 221.8671F, 314.6543F), new Vector3(0.61603F, 0.61603F, 0.61603F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "CalfR", new Vector3(0F, 0.11285F, 0.09567F), new Vector3(65.87368F, 90.73203F, 90.80209F), new Vector3(0.02974F, 0.02974F, 0.02974F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysDeputy) AddDisplayRule("DeputyBody", "ThighR", new Vector3(-0.01202F, 0.1805F, -0.11587F), new Vector3(12.74939F, 278.4629F, 73.65078F), new Vector3(0.02295F, 0.02295F, 0.02295F));
                AddDisplayRule("RailgunnerBody", "CalfR", new Vector3(-0.08793F, 0.21366F, -0.00001F), new Vector3(0F, 0F, 67.67302F), new Vector3(0.02516F, 0.02516F, 0.02516F));
                AddDisplayRule("VoidSurvivorBody", "CalfR", new Vector3(-0.08759F, 0.07466F, -0.00465F), new Vector3(326.4437F, 324.9552F, 104.0954F), new Vector3(0.02294F, 0.02294F, 0.02294F));
            };

            var mat = itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial;
            HopooShaderToMaterial.Standard.Apply(mat);
            HopooShaderToMaterial.Standard.Emission(mat, 1f, new Color32(255, 246, 0, 255));

            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;

            activeEffectPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Proximity Nanobots/ActiveEffect.prefab");
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            self.AddItemBehavior<MysticsItemsNanomachinesBehaviour>(self.inventory.GetItemCount(itemDef));
        }

        public class MysticsItemsNanomachinesBehaviour : CharacterBody.ItemBehavior
        {
            public HealthComponent healthComponent;
            private bool _isInHealthRange = false;
            public bool isInHealthRange
            {
                get { return _isInHealthRange; }
                set
                {
                    if (_isInHealthRange != value)
                    {
                        _isInHealthRange = value;
                        if (_isInHealthRange)
                        {
                            if (NetworkServer.active)
                                body.AddBuff(MysticsItemsContent.Buffs.MysticsItems_NanomachineArmor);

                            if (!attachedEffect)
                            {
                                attachedEffect = Instantiate(activeEffectPrefab, body.coreTransform);
                                attachedEffect.transform.localScale = Vector3.one * 2f * body.radius;
                            }
                            foreach (var ps in attachedEffect.GetComponentsInChildren<ParticleSystem>())
                            {
                                ps.Play();
                            }
                        }
                        else
                        {
                            if (NetworkServer.active)
                                body.RemoveBuff(MysticsItemsContent.Buffs.MysticsItems_NanomachineArmor);

                            if (attachedEffect)
                            {
                                foreach (var ps in attachedEffect.GetComponentsInChildren<ParticleSystem>())
                                {
                                    ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                                }
                            }
                        }
                    }
                }
            }

            public GameObject attachedEffect;

            public void Start()
            {
                healthComponent = GetComponent<HealthComponent>();
            }

            public void FixedUpdate()
            {
                if (healthComponent)
                {
                    isInHealthRange = healthComponent.combinedHealthFraction >= (minHealth / 100f) && healthComponent.combinedHealthFraction <= (maxHealth / 100f);
                }
            }

            public void OnDestroy()
            {
                isInHealthRange = false;
            }
        }
    }
}
