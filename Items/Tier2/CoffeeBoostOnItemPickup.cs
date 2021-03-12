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
        public static BuffIndex buffIndex;

        public override void PreAdd()
        {
            itemDef.name = "CoffeeBoostOnItemPickup";
            itemDef.tier = ItemTier.Tier2;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist
            };
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

            AssetManager.RegisterEffect(visualEffect);
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

        public override void OnAdd()
        {
            On.RoR2.Inventory.GiveItem += (orig, self, itemIndex, count) =>
            {
                if (NetworkServer.active)
                {
                    CharacterBody body = CharacterBody.readOnlyInstancesList.ToList().Find(body2 => body2.inventory == self);
                    if (body && self.GetItemCount(this.itemIndex) > 0)
                    {
                        for (int i = 0; i < count * GetBuffCountFromTier(ItemCatalog.GetItemDef(itemIndex).tier); i++)
                        {
                            if (body.GetBuffCount(buffIndex) < 2 + self.GetItemCount(this.itemIndex))
                            {
                                EffectData effectData = new EffectData
                                {
                                    origin = body.corePosition,
                                    scale = body.radius,
                                    rotation = Util.QuaternionSafeLookRotation(Vector3.forward)
                                };
                                EffectManager.SpawnEffect(visualEffect, effectData, true);
                                body.AddBuff(buffIndex);
                            }
                            else break;
                        }
                    }
                }
                orig(self, itemIndex, count);
            };
            On.RoR2.Inventory.RemoveItem += (orig, self, itemIndex, count) =>
            {
                if (NetworkServer.active)
                {
                    CharacterBody body = CharacterBody.readOnlyInstancesList.ToList().Find(body2 => body2.inventory == self);
                    if (body)
                    {
                        for (int i = 0; i < count * GetBuffCountFromTier(ItemCatalog.GetItemDef(itemIndex).tier); i++)
                        {
                            if (body.HasBuff(buffIndex)) body.RemoveBuff(buffIndex);
                            else break;
                        }
                    }
                }
                orig(self, itemIndex, count);
            };
            On.RoR2.CharacterBody.OnInventoryChanged += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && self.inventory && self.inventory.GetItemCount(itemIndex) <= 0)
                {
                    while (self.HasBuff(buffIndex)) self.RemoveBuff(buffIndex);
                }
            };
        }
    }
}
