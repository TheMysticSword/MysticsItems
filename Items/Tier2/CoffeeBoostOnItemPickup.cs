using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

namespace MysticsItems.Items
{
    public class CoffeeBoostOnItemPickup : BaseItem
    {
        public static GameObject visualEffect;

        public override void PreLoad()
        {
            itemDef.name = "CoffeeBoostOnItemPickup";
            itemDef.tier = ItemTier.Tier2;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist
            };
        }

        public override void OnLoad()
        {
            SetAssets("Coffee");
            Main.HopooShaderToMaterial.Standard.Gloss(model.transform.Find("Цилиндр").Find("Цилиндр.001").GetComponent<Renderer>().sharedMaterial, 0f);
            model.transform.Find("Цилиндр").Rotate(new Vector3(-30f, 0f, 0f), Space.Self);

            visualEffect = PrefabAPI.InstantiateClone(new GameObject(), Main.TokenPrefix + "CoffeeBoostEffect", false);
            EffectComponent effectComponent = visualEffect.AddComponent<EffectComponent>();
            effectComponent.applyScale = true;
            effectComponent.disregardZScale = true;
            effectComponent.soundName = "Play_item_proc_coffee";
            VFXAttributes vfxAttributes = visualEffect.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            visualEffect.AddComponent<DestroyOnTimer>().duration = 2f;

            GameObject particles = PrefabAPI.InstantiateClone(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Coffee/CoffeeBoostEffect.prefab"), "Particles", false);
            particles.transform.localScale *= 0.5f;
            particles.transform.SetParent(visualEffect.transform);

            MysticsItemsContent.Resources.effectPrefabs.Add(visualEffect);

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

            On.RoR2.Inventory.GiveItem_ItemIndex_int += (orig, self, itemIndex, count) =>
            {
                if (NetworkServer.active)
                {
                    CharacterBody body = CharacterBody.readOnlyInstancesList.ToList().Find(body2 => body2.inventory == self);
                    if (body && self.GetItemCount(itemDef) > 0)
                    {
                        for (int i = 0; i < count * GetBuffCountFromTier(ItemCatalog.GetItemDef(itemIndex).tier); i++)
                        {
                            if (body.GetBuffCount(MysticsItemsContent.Buffs.CoffeeBoost) < 2 + self.GetItemCount(itemDef))
                            {
                                EffectData effectData = new EffectData
                                {
                                    origin = body.corePosition,
                                    scale = body.radius,
                                    rotation = Util.QuaternionSafeLookRotation(Vector3.forward)
                                };
                                EffectManager.SpawnEffect(visualEffect, effectData, true);
                                body.AddBuff(MysticsItemsContent.Buffs.CoffeeBoost);
                            }
                            else break;
                        }
                    }
                }
                orig(self, itemIndex, count);
            };
            On.RoR2.Inventory.RemoveItem_ItemIndex_int += (orig, self, itemIndex, count) =>
            {
                if (NetworkServer.active)
                {
                    CharacterBody body = CharacterBody.readOnlyInstancesList.ToList().Find(body2 => body2.inventory == self);
                    if (body)
                    {
                        for (int i = 0; i < count * GetBuffCountFromTier(ItemCatalog.GetItemDef(itemIndex).tier); i++)
                        {
                            if (body.HasBuff(MysticsItemsContent.Buffs.CoffeeBoost)) body.RemoveBuff(MysticsItemsContent.Buffs.CoffeeBoost);
                            else break;
                        }
                    }
                }
                orig(self, itemIndex, count);
            };
            On.RoR2.CharacterBody.OnInventoryChanged += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && self.inventory && self.inventory.GetItemCount(itemDef) <= 0)
                {
                    while (self.HasBuff(MysticsItemsContent.Buffs.CoffeeBoost)) self.RemoveBuff(MysticsItemsContent.Buffs.CoffeeBoost);
                }
            };
        }

        public int GetBuffCountFromTier(ItemTier tier)
        {
            switch (tier)
            {
                case ItemTier.NoTier:
                case ItemTier.Lunar:
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
