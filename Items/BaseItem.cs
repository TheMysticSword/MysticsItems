using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MysticsItems.ContentManagement;

namespace MysticsItems.Items
{
    public abstract class BaseItem : BaseItemLike
    {
        public ItemDef itemDef;
        public static List<BaseItem> loadedItems = new List<BaseItem>();
        
        public override void Load()
        {
            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            PreLoad();
            bool disabledByConfig = false;
            if (itemDef.inDroppableTier)
            {
                disabledByConfig = MysticsItemsPlugin.config.Bind<bool>(
                    "Disable items",
                    itemDef.name,
                    false,
                    string.Format(
                        "{0} - {1}",
                        LanguageLoader.GetLoadedStringByToken("ITEM_" + (Main.TokenPrefix + itemDef.name).ToUpper() + "_NAME"),
                        LanguageLoader.GetLoadedStringByToken("ITEM_" + (Main.TokenPrefix + itemDef.name).ToUpper() + "_PICKUP")
                    )
                ).Value;
            }
            string name = itemDef.name;
            itemDef.name = Main.TokenPrefix + itemDef.name;
            itemDef.AutoPopulateTokens();
            itemDef.name = name;
            AfterTokensPopulated();
            if (!disabledByConfig) {
                OnLoad();
                loadedItems.Add(this);
            } else
            {
                itemDef.tier = ItemTier.NoTier;
            }
            asset = itemDef;
        }

        public override void SetAssets(string assetName)
        {
            model = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/" + assetName + "/Model.prefab");
            model.name = "mdl" + itemDef.name;

            bool followerModelSeparate = Main.AssetBundle.Contains("Assets/Items/" + assetName + "/FollowerModel.prefab");
            if (followerModelSeparate)
            {
                followerModel = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/" + assetName + "/FollowerModel.prefab");
                followerModel.name = "mdl" + itemDef.name + "Follower";
            }

            PrepareModel(model);
            if (followerModelSeparate) PrepareModel(followerModel);

            // Separate the follower model from the pickup model for adding different visual effects to followers
            if (!followerModelSeparate) CopyModelToFollower();

            itemDef.pickupModelPrefab = model;
            SetIcon(assetName);
        }

        public override void SetIcon(string assetName)
        {
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/" + assetName + "/Icon.png");
        }

        public override UnlockableDef GetUnlockableDef()
        {
            if (!itemDef.unlockableDef)
            {
                itemDef.unlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
                itemDef.unlockableDef.cachedName = Main.TokenPrefix + "Items." + itemDef.name;
                itemDef.unlockableDef.nameToken = ("ITEM_" + Main.TokenPrefix + itemDef.name + "_NAME").ToUpper();
            }
            return itemDef.unlockableDef;
        }

        public override PickupIndex GetPickupIndex()
        {
            return PickupCatalog.FindPickupIndex(itemDef.itemIndex);
        }
    }
}
