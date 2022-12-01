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
                AddDisplayRule("CommandoBody", "LowerArmR", new Vector3(0.001F, 0.274F, -0.078F), new Vector3(7.29F, 186.203F, 0.157F), new Vector3(0.277F, 0.389F, 0.277F));
                AddDisplayRule("HuntressBody", "HandL", new Vector3(-0.014F, 0.004F, 0.035F), new Vector3(6.909F, 1.748F, 74.816F), new Vector3(0.187F, 0.174F, 0.187F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(-0.069F, -0.12F, -0.197F), new Vector3(18.152F, 14.491F, 196.624F), new Vector3(0.348F, 0.348F, 0.348F));
                AddDisplayRule("ToolbotBody", "HandR", new Vector3(-0.059F, 0.587F, 1.939F), new Vector3(356.736F, 85.148F, 90.496F), new Vector3(3.014F, 3.241F, 3.014F));
                AddDisplayRule("EngiBody", "HandL", new Vector3(0F, 0.104F, 0.042F), new Vector3(3.001F, 0F, 0F), new Vector3(0.259F, 0.259F, 0.259F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.026F, 0.602F, -1.541F), new Vector3(22.044F, 48.281F, 206.737F), new Vector3(0.74F, 0.74F, 0.74F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(-0.248F, 1.434F, -0.84F), new Vector3(300.601F, 223.502F, 297.144F), new Vector3(0.659F, 0.801F, 0.643F));
                AddDisplayRule("MageBody", "HandL", new Vector3(-0.011F, 0.074F, 0.104F), new Vector3(0F, 0F, 355.462F), new Vector3(0.22F, 0.22F, 0.22F));
                AddDisplayRule("MercBody", "HandR", new Vector3(0F, 0.112F, 0.103F), new Vector3(14.285F, 0F, 0F), new Vector3(0.427F, 0.427F, 0.427F));
                AddDisplayRule("TreebotBody", "WeaponPlatform", new Vector3(0F, 0.889F, 0.308F), new Vector3(0F, 0F, 0F), new Vector3(0.846F, 0.846F, 0.846F));
                AddDisplayRule("LoaderBody", "MechHandL", new Vector3(-0.073F, 0.379F, 0.15F), new Vector3(5.558F, 330.424F, 0F), new Vector3(0.36F, 0.36F, 0.36F));
                AddDisplayRule("CrocoBody", "HandL", new Vector3(-1.286F, 0.394F, 0.102F), new Vector3(56.075F, 280.047F, 0F), new Vector3(3.614F, 2.545F, 4.003F));
                AddDisplayRule("CaptainBody", "HandR", new Vector3(-0.086F, 0.125F, 0.016F), new Vector3(14.676F, 274.88F, 359.215F), new Vector3(0.248F, 0.248F, 0.248F));
                AddDisplayRule("BrotherBody", "HandL", BrotherInfection.green, new Vector3(0.019F, -0.013F, 0.017F), new Vector3(348.105F, 324.594F, 242.165F), new Vector3(0.061F, 0.019F, 0.061F));
                AddDisplayRule("ScavBody", "HandL", new Vector3(-3.491F, 2.547F, -2.4F), new Vector3(354.216F, 329.486F, 87.688F), new Vector3(7.501F, 7.7F, 7.501F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "LowerArmR", new Vector3(-0.00116F, 0.35526F, -0.02252F), new Vector3(12.00797F, 359.9336F, 355.1882F), new Vector3(0.2754F, 0.2754F, 0.2754F));
                AddDisplayRule("RailgunnerBody", "Pelvis", new Vector3(0.1939F, 0.23069F, 0.00249F), new Vector3(5.45999F, 119.7464F, 17.75949F), new Vector3(0.33522F, 0.33522F, 0.33522F));
                AddDisplayRule("VoidSurvivorBody", "Head", new Vector3(-0.06693F, -1.12064F, -0.77318F), new Vector3(90F, 0F, 0F), new Vector3(0.5795F, 0.5795F, 0.5795F));
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
