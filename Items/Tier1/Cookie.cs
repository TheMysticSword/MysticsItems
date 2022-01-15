using RoR2;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.BalanceConfigManager;
using System.Linq;

namespace MysticsItems.Items
{
    public class Cookie : BaseItem
    {
        public static ConfigurableValue<float> buffDuration = new ConfigurableValue<float>(
            "Item: Choc Chip",
            "BuffDuration",
            1f,
            "Extra buff duration (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_COOKIE_DESC"
            }
        );
        public static ConfigurableValue<float> buffDurationPerStack = new ConfigurableValue<float>(
            "Item: Choc Chip",
            "BuffDurationPerStack",
            1f,
            "Extra buff duration for each additional stack of this item (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_COOKIE_DESC"
            }
        );
        public static ConfigurableValue<float> debuffDuration = new ConfigurableValue<float>(
            "Item: Choc Chip",
            "DebuffDuration",
            1f,
            "Reduce debuff duration by this amount (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_COOKIE_DESC"
            }
        );
        public static ConfigurableValue<float> debuffDurationPerStack = new ConfigurableValue<float>(
            "Item: Choc Chip",
            "DebuffDurationPerStack",
            1f,
            "Reduce debuff duration by this amount for each additional stack of this item (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_COOKIE_DESC"
            }
        );

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_Cookie";
            itemDef.tier = ItemTier.Tier1;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility
            };
            //itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Scratch Ticket/Model.prefab"));
            //itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Scratch Ticket/Icon.png");

            //itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            /*
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Head", new Vector3(0.109F, 0.159F, -0.123F), new Vector3(25.857F, 140.857F, 4.186F), new Vector3(0.102F, 0.102F, 0.102F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(0.077F, 0.1F, -0.13F), new Vector3(85.128F, 22.234F, 233.333F), new Vector3(0.084F, 0.084F, 0.084F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(0.088F, 0.093F, 0.172F), new Vector3(72.061F, 15.327F, 356.324F), new Vector3(0.15F, 0.15F, 0.15F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(1.804F, 1.034F, -1.656F), new Vector3(20.492F, 214.386F, 67.265F), new Vector3(1.043F, 1.043F, 1.043F));
                AddDisplayRule("EngiBody", "HeadCenter", new Vector3(0.064F, 0.027F, -0.169F), new Vector3(42.86F, 152.127F, 353.333F), new Vector3(0.175F, 0.175F, 0.175F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.8316F, 0.72181F, -0.23478F), new Vector3(323.0401F, 108.2313F, 5.96822F), new Vector3(0.29536F, 0.29536F, 0.29536F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.47616F, 1.3804F, -0.4785F), new Vector3(74.07391F, 268.1374F, 180F), new Vector3(0.33715F, 0.41012F, 0.3296F));
                AddDisplayRule("MageBody", "Head", new Vector3(0.044F, 0.113F, -0.177F), new Vector3(13.198F, 167.753F, 11.941F), new Vector3(0.086F, 0.086F, 0.086F));
                AddDisplayRule("MercBody", "Head", new Vector3(0.066F, 0.195F, -0.092F), new Vector3(358.901F, 249.659F, 64.015F), new Vector3(0.093F, 0.093F, 0.093F));
                AddDisplayRule("TreebotBody", "PlatformBase", new Vector3(-0.073F, 0.359F, -1.062F), new Vector3(13.168F, 154.79F, 328.62F), new Vector3(0.315F, 0.315F, 0.315F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(0.162F, 0.046F, -0.123F), new Vector3(13.044F, 208.575F, 57.569F), new Vector3(0.15F, 0.15F, 0.15F));
                AddDisplayRule("CrocoBody", "HandR", new Vector3(-1.129F, -1.383F, 0.659F), new Vector3(18.756F, 105.807F, 169.38F), new Vector3(1.162F, 1.162F, 1.162F));
                AddDisplayRule("CaptainBody", "Chest", new Vector3(-0.1F, 0.226F, 0.174F), new Vector3(15.849F, 346.474F, 358.999F), new Vector3(0.086F, 0.086F, 0.086F));
                AddDisplayRule("BrotherBody", "UpperArmL", BrotherInfection.white, new Vector3(-0.018F, 0.215F, -0.064F), new Vector3(0F, 0F, 131.256F), new Vector3(0.115F, 0.063F, 0.063F));
                AddDisplayRule("ScavBody", "MuzzleEnergyCannon", new Vector3(-3.88535F, -0.90743F, -18.53646F), new Vector3(16.92252F, 288.3049F, 72.11835F), new Vector3(2.62999F, 2.70243F, 2.62999F));
            };
            */

            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += CharacterBody_AddTimedBuff_BuffDef_float;
            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float_int += CharacterBody_AddTimedBuff_BuffDef_float_int;
        }

        private void CharacterBody_AddTimedBuff_BuffDef_float(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float orig, CharacterBody self, BuffDef buffDef, float duration)
        {
            duration = GetModifiedDuration(self, buffDef, duration);
            orig(self, buffDef, duration);
        }

        private void CharacterBody_AddTimedBuff_BuffDef_float_int(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float_int orig, CharacterBody self, BuffDef buffDef, float duration, int maxStacks)
        {
            orig(self, buffDef, duration, maxStacks);
            foreach (var timedBuff in self.timedBuffs.Where(x => x.buffIndex == buffDef.buffIndex && x.timer == duration))
            {
                timedBuff.timer = GetModifiedDuration(self, buffDef, duration);
            }
        }

        public static float GetModifiedDuration(CharacterBody body, BuffDef buffDef, float duration)
        {
            Inventory inventory = body.inventory;
            if (inventory)
            {
                int itemCount = inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_Cookie);
                if (itemCount > 0)
                {
                    if (buffDef.isDebuff)
                        duration = Mathf.Max(duration - debuffDuration - debuffDurationPerStack * (itemCount - 1), 1f);
                    else
                        duration += buffDuration + buffDurationPerStack * (itemCount - 1);
                }
            }
            return duration;
        }
    }
}
