using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using RoR2.Audio;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Items
{
    public class CoffeeBoostOnItemPickup : BaseItem
    {
        public static GameObject visualEffect;

        public static ConfigurableValue<int> maxBuffs = new ConfigurableValue<int>(
            "Item: Cup of Expresso",
            "MaxBuffs",
            3,
            "Maximum amount of Express Boost buff stacks",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_COFFEEBOOSTONITEMPICKUP_PICKUP",
                "ITEM_MYSTICSITEMS_COFFEEBOOSTONITEMPICKUP_DESC"
            }
        );
        public static ConfigurableValue<int> maxBuffsPerStack = new ConfigurableValue<int>(
            "Item: Cup of Expresso",
            "MaxBuffsPerStack",
            2,
            "More maximum amount of Express Boost buff stacks for each additional stack of this item",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_COFFEEBOOSTONITEMPICKUP_DESC"
            }
        );

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_CoffeeBoostOnItemPickup";
            SetItemTierWhenAvailable(ItemTier.Tier2);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Coffee/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Coffee/Icon.png");
            HopooShaderToMaterial.Standard.Gloss(itemDef.pickupModelPrefab.transform.Find("Цилиндр").Find("Цилиндр.001").GetComponent<Renderer>().sharedMaterial, 0f);
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            itemDef.pickupModelPrefab.transform.Find("Цилиндр").Rotate(new Vector3(-30f, 0f, 0f), Space.Self);
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "HandR", new Vector3(-0.016F, 0.214F, -0.111F), new Vector3(33.043F, 10.378F, 286.615F), new Vector3(0.072F, 0.072F, 0.072F));
                AddDisplayRule("HuntressBody", "Muzzle", new Vector3(-0.397F, -0.017F, -0.251F), new Vector3(0.509F, 134.442F, 184.268F), new Vector3(0.037F, 0.037F, 0.037F));
                AddDisplayRule("Bandit2Body", "MuzzleShotgun", new Vector3(-0.006F, 0.103F, -0.765F), new Vector3(46.857F, 3.395F, 94.648F), new Vector3(0.065F, 0.065F, 0.065F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(-0.925F, 2.116F, 2.185F), new Vector3(0F, 324.78F, 0F), new Vector3(0.448F, 0.448F, 0.448F));
                AddDisplayRule("EngiBody", "HandR", new Vector3(0.014F, 0.165F, 0.065F), new Vector3(21.189F, 152.963F, 68.785F), new Vector3(0.072F, 0.072F, 0.072F));
                AddDisplayRule("MageBody", "HandR", new Vector3(-0.098F, -0.054F, -0.102F), new Vector3(15.816F, 19.399F, 81.6F), new Vector3(0.059F, 0.059F, 0.059F));
                AddDisplayRule("MercBody", "HandR", new Vector3(0.006F, 0.202F, 0.125F), new Vector3(21.174F, 173.671F, 87.267F), new Vector3(0.08F, 0.08F, 0.08F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(1.119F, -0.997F, -0.433F), new Vector3(304.283F, 287.341F, 136.374F), new Vector3(0.189F, 0.189F, 0.189F));
                AddDisplayRule("LoaderBody", "MechLowerArmR", new Vector3(-0.002F, 0.311F, 0.171F), new Vector3(78.649F, 219.923F, 309.488F), new Vector3(0.055F, 0.054F, 0.055F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(0.919F, 4.774F, -1.755F), new Vector3(25.068F, 0.938F, 231.2F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("CaptainBody", "HandR", new Vector3(-0.09F, 0.124F, 0.046F), new Vector3(352.642F, 118.248F, 97.772F), new Vector3(0.076F, 0.069F, 0.076F));
                AddDisplayRule("BrotherBody", "HandR", BrotherInfection.green, new Vector3(0.002F, 0.109F, 0.031F), new Vector3(72.72F, 119.024F, 264.129F), new Vector3(0.043F, 0.043F, 0.043F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Muzzle", new Vector3(-3.90342F, 0.4645F, -0.00028F), new Vector3(90F, 90F, 0F), new Vector3(0.24699F, 0.41591F, 0.24699F));
                AddDisplayRule("RailgunnerBody", "GunScope", new Vector3(-0.0692F, -0.13973F, 0.22722F), new Vector3(278.7664F, 90F, 90F), new Vector3(0.05291F, 0.10893F, 0.05291F));
                AddDisplayRule("VoidSurvivorBody", "Hand", new Vector3(-0.14745F, 0.20297F, 0.04337F), new Vector3(31.68131F, 74.82335F, 46.28517F), new Vector3(0.09265F, 0.09265F, 0.09265F));
            };

            visualEffect = PrefabAPI.InstantiateClone(new GameObject(), "MysticsItems_CoffeeBoostEffect", false);
            EffectComponent effectComponent = visualEffect.AddComponent<EffectComponent>();
            effectComponent.applyScale = true;
            effectComponent.parentToReferencedTransform = true;
            VFXAttributes vfxAttributes = visualEffect.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
            visualEffect.AddComponent<DestroyOnTimer>().duration = 2f;

            GameObject particles = PrefabAPI.InstantiateClone(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Coffee/CoffeeBoostEffect.prefab"), "Particles", false);
            particles.transform.localScale *= 0.5f;
            particles.transform.SetParent(visualEffect.transform);

            MysticsItemsContent.Resources.effectPrefabs.Add(visualEffect);

            On.RoR2.Inventory.GiveItem_ItemIndex_int += (orig, self, itemIndex, count) =>
            {
                if (NetworkServer.active)
                {
                    var master = self.GetComponent<CharacterMaster>();
                    if (master)
                    {
                        var body = master.GetBody();
                        if (body && self.GetItemCount(itemDef) > 0)
                        {
                            for (int i = 0; i < count * GetBuffCountFromTier(ItemCatalog.GetItemDef(itemIndex).tier); i++)
                            {
                                if (body.GetBuffCount(MysticsItemsContent.Buffs.MysticsItems_CoffeeBoost) < (maxBuffs + maxBuffsPerStack * (self.GetItemCount(itemDef) - 1)))
                                {
                                    body.AddBuff(MysticsItemsContent.Buffs.MysticsItems_CoffeeBoost);
                                }
                                else break;
                            }
                        }
                    }
                }
                orig(self, itemIndex, count);
            };

            On.RoR2.Language.GetLocalizedStringByToken += Language_GetLocalizedStringByToken;
        }

        private string Language_GetLocalizedStringByToken(On.RoR2.Language.orig_GetLocalizedStringByToken orig, Language self, string token)
        {
            var result = orig(self, token);
            if (token == "ITEM_MYSTICSITEMS_COFFEEBOOSTONITEMPICKUP_DESC")
                result = Utils.FormatStringByDict(result, new System.Collections.Generic.Dictionary<string, string>()
                {
                    { "BoostPowerMax", (Buffs.CoffeeBoost.boostPower * maxBuffs).ToString(System.Globalization.CultureInfo.InvariantCulture) },
                    { "BoostPowerMaxPerStack", (Buffs.CoffeeBoost.boostPower * maxBuffsPerStack).ToString(System.Globalization.CultureInfo.InvariantCulture) }
                });
            return result;
        }

        public int GetBuffCountFromTier(ItemTier tier)
        {
            switch (tier)
            {
                case ItemTier.NoTier:
                    return 0;
                case ItemTier.Tier2:
                case ItemTier.Boss:
                    return 2;
                case ItemTier.Tier3:
                    return 3;
                default:
                    return 1;
            }
        }
    }
}
