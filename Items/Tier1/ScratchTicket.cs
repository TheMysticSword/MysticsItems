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

        public override void PreLoad()
        {
            itemDef.name = "ScratchTicket";
            itemDef.tier = ItemTier.Tier1;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy
            };
        }

        public override void OnLoad()
        {
            SetAssets("Scratch Ticket");
            SetModelPanelDistance(1f, 2f);
            Main.HopooShaderToMaterial.Standard.Gloss(GetModelMaterial(), 0.05f, 20f);

            AddDisplayRule("CommandoBody", "Head", new Vector3(0.109F, 0.159F, -0.123F), new Vector3(25.857F, 140.857F, 4.186F), new Vector3(0.102F, 0.102F, 0.102F));
            AddDisplayRule("HuntressBody", "Head", new Vector3(0.077F, 0.1F, -0.13F), new Vector3(85.128F, 22.234F, 233.333F), new Vector3(0.084F, 0.084F, 0.084F));
            AddDisplayRule("Bandit2Body", "Stomach", new Vector3(0.088F, 0.093F, 0.172F), new Vector3(72.061F, 15.327F, 356.324F), new Vector3(0.15F, 0.15F, 0.15F));
            AddDisplayRule("ToolbotBody", "Chest", new Vector3(1.804F, 1.034F, -1.656F), new Vector3(20.492F, 214.386F, 67.265F), new Vector3(1.043F, 1.043F, 1.043F));
            AddDisplayRule("EngiBody", "HeadCenter", new Vector3(0.064F, 0.027F, -0.169F), new Vector3(42.86F, 152.127F, 353.333F), new Vector3(0.175F, 0.175F, 0.175F));
            AddDisplayRule("MageBody", "Head", new Vector3(0.044F, 0.113F, -0.177F), new Vector3(13.198F, 167.753F, 11.941F), new Vector3(0.086F, 0.086F, 0.086F));
            AddDisplayRule("MercBody", "Head", new Vector3(0.066F, 0.195F, -0.092F), new Vector3(358.901F, 249.659F, 64.015F), new Vector3(0.093F, 0.093F, 0.093F));
            AddDisplayRule("TreebotBody", "PlatformBase", new Vector3(-0.073F, 0.359F, -1.062F), new Vector3(13.168F, 154.79F, 328.62F), new Vector3(0.315F, 0.315F, 0.315F));
            AddDisplayRule("LoaderBody", "MechBase", new Vector3(0.162F, 0.046F, -0.123F), new Vector3(13.044F, 208.575F, 57.569F), new Vector3(0.15F, 0.15F, 0.15F));
            AddDisplayRule("CrocoBody", "HandR", new Vector3(-1.129F, -1.383F, 0.659F), new Vector3(18.756F, 105.807F, 169.38F), new Vector3(1.162F, 1.162F, 1.162F));
            AddDisplayRule("CaptainBody", "Chest", new Vector3(-0.1F, 0.226F, 0.174F), new Vector3(15.849F, 346.474F, 358.999F), new Vector3(0.086F, 0.086F, 0.086F));
            AddDisplayRule("BrotherBody", "UpperArmL", BrotherInfection.white, new Vector3(-0.018F, 0.215F, -0.064F), new Vector3(0F, 0F, 131.256F), new Vector3(0.115F, 0.063F, 0.063F));

            coinEffect = Resources.Load<GameObject>("Prefabs/Effects/CoinEmitter");

            On.RoR2.ShrineChanceBehavior.AddShrineStack += (orig, self, activator) =>
            {
                orig(self, activator);
                if (self.GetFieldValue<int>("successfulPurchaseCount") == 2)
                {
                    CharacterBody body = activator.GetComponent<CharacterBody>();
                    if (body)
                    {
                        Inventory inventory = body.inventory;
                        if (inventory && inventory.GetItemCount(MysticsItemsContent.Items.ScratchTicket) > 0)
                        {
                            MysticsItemsScratchTicketCheck component = self.GetComponent<MysticsItemsScratchTicketCheck>();
                            if (!component) component = self.gameObject.AddComponent<MysticsItemsScratchTicketCheck>();
                            if (!component.check)
                            {
                                component.check = true;

                                uint goldReward = (uint)(self.GetComponent<PurchaseInteraction>().cost * (1f + 0.5f * (inventory.GetItemCount(MysticsItemsContent.Items.ScratchTicket) - 1)));
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
                }
            };
        }

        public class MysticsItemsScratchTicketCheck : MonoBehaviour
        {
            public bool check = false;
        }
    }
}
