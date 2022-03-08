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
using System.Collections.Generic;

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
            0.2f,
            "Reduce debuff duration by this amount (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_COOKIE_DESC"
            }
        );
        public static ConfigurableValue<float> debuffDurationPerStack = new ConfigurableValue<float>(
            "Item: Choc Chip",
            "DebuffDurationPerStack",
            0.2f,
            "Reduce debuff duration by this amount for each additional stack of this item (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_COOKIE_DESC"
            }
        );

        public static List<BuffDef> ignoredBuffDefs = new List<BuffDef>();

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_Cookie";
            itemDef.tier = ItemTier.Tier1;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Cookie/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Cookie/Icon.png");

            var mat = itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial;
            HopooShaderToMaterial.Standard.Emission(mat, 0f);
            HopooShaderToMaterial.Standard.Gloss(mat, 0f);
            mat.color = new Color32(180, 180, 180, 255);
            itemDisplayPrefab = PrepareItemDisplayModel(PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Cookie/DisplayModel.prefab")));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Head", new Vector3(0F, 0.16388F, 0.18652F), new Vector3(0F, 0F, 0F), new Vector3(0.102F, 0.102F, 0.102F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(0F, 0.0967F, 0.11243F), new Vector3(43.64907F, 0F, 0F), new Vector3(0.084F, 0.09993F, 0.084F));
                AddDisplayRule("Bandit2Body", "Head", new Vector3(-0.00001F, -0.01385F, 0.12776F), new Vector3(0F, 0F, 0F), new Vector3(0.06952F, 0.06952F, 0.06952F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(-0.00001F, 1.12616F, 3.23301F), new Vector3(0F, 0F, 0F), new Vector3(1.043F, 1.043F, 1.043F));
                AddDisplayRule("EngiBody", "HeadCenter", new Vector3(0F, -0.09658F, 0.15593F), new Vector3(28.31221F, 0F, 0F), new Vector3(0.11432F, 0.11432F, 0.11432F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0F, 0.51898F, 1.40674F), new Vector3(6.00462F, 0F, 0F), new Vector3(0.24555F, 0.24555F, 0.24555F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.03191F, 0.67419F, -1.02958F), new Vector3(347.364F, 0F, 0F), new Vector3(0.58633F, 0.58633F, 0.5732F));
                AddDisplayRule("MageBody", "Head", new Vector3(0F, -0.01334F, 0.13699F), new Vector3(20.87633F, 0F, 0F), new Vector3(0.07433F, 0.07433F, 0.07433F));
                AddDisplayRule("MercBody", "Head", new Vector3(0F, 0.00938F, 0.16908F), new Vector3(23.24298F, 0F, 0F), new Vector3(0.093F, 0.093F, 0.093F));
                AddDisplayRule("TreebotBody", "PlatformBase", new Vector3(-0.5384F, -0.33493F, 0F), new Vector3(0F, 0F, 37.80375F), new Vector3(0.26476F, 0.26476F, 0.26476F));
                AddDisplayRule("LoaderBody", "Head", new Vector3(0F, 0.0199F, 0.16741F), new Vector3(13F, 0F, 0F), new Vector3(0.10986F, 0.10986F, 0.10986F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(0F, 3.92894F, -0.2927F), new Vector3(80.82946F, 180F, 180F), new Vector3(2.0196F, 2.0196F, 2.0196F));
                AddDisplayRule("CaptainBody", "Head", new Vector3(0F, -0.0366F, 0.16365F), new Vector3(25.47889F, 0F, 0F), new Vector3(0.086F, 0.086F, 0.086F));
                AddDisplayRule("BrotherBody", "Head", BrotherInfection.white, new Vector3(-0.00094F, -0.05567F, 0.03761F), new Vector3(0F, 88.56445F, 15.20469F), new Vector3(0.09773F, 0.063F, 0.063F));
                AddDisplayRule("ScavBody", "Head", new Vector3(0F, 4.3108F, 0.00021F), new Vector3(270.1856F, 0F, 0F), new Vector3(2.62999F, 2.70243F, 2.62999F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Head", new Vector3(0.00003F, 0.19399F, 0.0348F), new Vector3(287.3143F, 0F, 0F), new Vector3(0.06746F, 0.06746F, 0.06746F));
            };

            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += CharacterBody_AddTimedBuff_BuffDef_float;
            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float_int += CharacterBody_AddTimedBuff_BuffDef_float_int;
            On.RoR2.DotController.OnDotStackAddedServer += DotController_OnDotStackAddedServer;

            RoR2Application.onLoad += () =>
            {
                ignoredBuffDefs.Add(RoR2Content.Buffs.MedkitHeal);
                ignoredBuffDefs.Add(RoR2Content.Buffs.HiddenInvincibility);
            };
        }

        private void DotController_OnDotStackAddedServer(On.RoR2.DotController.orig_OnDotStackAddedServer orig, DotController self, object dotStack)
        {
            orig(self, dotStack);
            var _dotStack = (dotStack as DotController.DotStack);
            if (self.victimBody && _dotStack.dotDef.associatedBuff && !ignoredBuffDefs.Contains(_dotStack.dotDef.associatedBuff))
            {
                _dotStack.timer = GetModifiedDuration(self.victimBody, _dotStack.dotDef.associatedBuff.isDebuff, _dotStack.timer);
            }
        }

        private void CharacterBody_AddTimedBuff_BuffDef_float(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float orig, CharacterBody self, BuffDef buffDef, float duration)
        {
            if (!ignoredBuffDefs.Contains(buffDef))
                duration = GetModifiedDuration(self, buffDef.isDebuff, duration);
            orig(self, buffDef, duration);
        }

        private void CharacterBody_AddTimedBuff_BuffDef_float_int(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float_int orig, CharacterBody self, BuffDef buffDef, float duration, int maxStacks)
        {
            orig(self, buffDef, duration, maxStacks);
            if (!ignoredBuffDefs.Contains(buffDef))
            {
                var modifiedDuration = GetModifiedDuration(self, buffDef.isDebuff, duration);
                foreach (var timedBuff in self.timedBuffs.Where(x => x.buffIndex == buffDef.buffIndex && x.timer == duration))
                {
                    timedBuff.timer = modifiedDuration;
                }
            }
        }

        public static float GetModifiedDuration(CharacterBody body, bool isDebuff, float duration)
        {
            Inventory inventory = body.inventory;
            if (inventory)
            {
                int itemCount = inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_Cookie);
                if (itemCount > 0)
                {
                    if (isDebuff)
                        duration = Mathf.Min(Mathf.Max(duration - debuffDuration - debuffDurationPerStack * (itemCount - 1), 1f), duration);
                    else
                        duration += buffDuration + buffDurationPerStack * (itemCount - 1);
                }
            }
            return duration;
        }
    }
}
