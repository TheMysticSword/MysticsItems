using RoR2;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Items
{
    public class ScratchTicket : BaseItem
    {
        public static GameObject coinEffect;

        public override void PreAdd()
        {
            itemDef.name = "ScratchTicket";
            itemDef.tier = ItemTier.Tier1;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist
            };
            BanFromDeployables();
            SetAssets("Scratch Ticket");
            SetModelPanelDistance(1f, 2f);
            Main.HopooShaderToMaterial.Standard.Gloss(GetModelMaterial(), 0.05f, 20f);

            coinEffect = Resources.Load<GameObject>("Prefabs/Effects/CoinEmitter");
        }

        public override void OnAdd()
        {
            On.RoR2.ShrineChanceBehavior.AddShrineStack += (orig, self, activator) =>
            {
                orig(self, activator);
                if (self.GetFieldValue<int>("successfulPurchaseCount") == 2)
                {
                    CharacterBody body = activator.GetComponent<CharacterBody>();
                    if (body)
                    {
                        Inventory inventory = body.inventory;
                        if (inventory && inventory.GetItemCount(itemIndex) > 0)
                        {
                            uint goldReward = (uint)((40u + 10u * (inventory.GetItemCount(itemIndex) - 1)) * Run.instance.difficultyCoefficient);
                            TeamManager.instance.GiveTeamMoney(body.teamComponent.teamIndex, goldReward);
                            EffectManager.SpawnEffect(coinEffect, new EffectData
                            {
                                origin = self.transform.position + Vector3.up * 2f * self.transform.localScale.y,
                                genericFloat = goldReward,
                                scale = 2f
                            }, true);
                        }
                    }
                }
            };
        }
    }
}
