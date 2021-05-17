using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MysticsItems.ContentManagement;
using EntityStates;

namespace MysticsItems.Interactables
{
    public class CharacterItemShopBeacon : BaseInteractable
    {
        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            prefab = CustomUtils.CreateBlankPrefab(Main.TokenPrefix + "CharacterItemShopBeacon", true);
        }

        public override void OnLoad()
        {
            base.OnLoad();

            CustomUtils.CopyChildren(PrefabAPI.InstantiateClone(Main.AssetBundle.LoadAsset<GameObject>("Assets/Interactables/Shop Beacon/ShopBeacon.prefab"), "ShopBeacon", false), prefab);
            modelBaseTransform = prefab.transform.Find("ModelBase");
            modelTransform = prefab.transform.Find("ModelBase/mdlShopBeacon");
            meshObject = prefab.transform.Find("ModelBase/mdlShopBeacon/Сетка").gameObject;
            modelBaseTransform.Find("Collision").gameObject.layer = LayerIndex.world.intVal;
            genericDisplayNameToken = Main.TokenPrefix.ToUpper() + "CHARACTERITEMSHOPBEACON_NAME";
            Prepare();
            Dither();
            meshObject.GetComponent<Renderer>().material.mainTexture = Resources.Load<GameObject>("Prefabs/NetworkedObjects/Chest/Chest1").transform.Find("mdlChest1/Cube.001").gameObject.GetComponent<Renderer>().material.mainTexture;
            PurchaseInteraction purchaseInteraction = prefab.AddComponent<PurchaseInteraction>();
            purchaseInteraction.displayNameToken = Main.TokenPrefix.ToUpper() + "CHARACTERITEMSHOPBEACON_NAME";
            purchaseInteraction.contextToken = Main.TokenPrefix.ToUpper() + "CHARACTERITEMSHOPBEACON_CONTEXT";
            purchaseInteraction.costType = CostTypeIndex.Money;
            purchaseInteraction.cost = 150;
            purchaseInteraction.automaticallyScaleCostWithDifficulty = true;
            purchaseInteraction.requiredUnlockable = "";
            purchaseInteraction.ignoreSpherecastForInteractability = false;
            purchaseInteraction.purchaseStatNames = new string[] { };
            purchaseInteraction.setUnavailableOnTeleporterActivated = false;
            purchaseInteraction.isShrine = false;
            purchaseInteraction.isGoldShrine = false;
            SetPurchasable();
            SetUpEntityStateMachine();
            onPurchase += (gameObject, interactor, component) =>
            {
                if (component.purchaseInteraction) component.purchaseInteraction.SetAvailable(false);
                if (component.entityStateMachine) component.entityStateMachine.SetNextState(new EntityStates.Barrel.Opening());
                Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
                {
                    baseToken = Main.TokenPrefix.ToUpper() + "CHARACTER_ITEM_SHOP_ENABLED",
                    paramTokens = new string[] {
                        "???"
                    }
                });
            };

            spawnCard.directorCreditCost = 0;
            spawnCard.slightlyRandomizeOrientation = false;
            spawnCard.hullSize = HullClassification.Golem;

            GenericGameEvents.OnPopulateScene += (rng) =>
            {
                if (((Run.instance.stageClearCount + 1) % 2) == 1 && SceneInfo.instance.countsAsStage)
                {
                    DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.Random
                    }, rng));
                }
            };
        }

        public override void Load()
        {
            if (Items.CharacterItems.enabled) base.Load();
        }
    }
}
