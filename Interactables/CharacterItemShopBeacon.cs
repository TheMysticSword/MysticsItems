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

            CustomUtils.CopyChildren(Main.AssetBundle.LoadAsset<GameObject>("Assets/CharacterItems/Shop Beacon/ShopBeacon.prefab"), prefab);
            modelBaseTransform = prefab.transform.Find("ModelBase");
            modelTransform = prefab.transform.Find("ModelBase/mdlShopBeacon");
            meshObject = prefab.transform.Find("ModelBase/mdlShopBeacon/Сетка").gameObject;
            modelBaseTransform.Find("Collision").gameObject.layer = LayerIndex.world.intVal;
            genericDisplayNameToken = Main.TokenPrefix.ToUpper() + "CHARACTERITEMSHOPBEACON_NAME";
            Prepare();
            SetUpChildLocator(new ChildLocator.NameTransformPair[]
            {
                new ChildLocator.NameTransformPair{name = "Mesh", transform = meshObject.transform}
            });
            SetUpPingInfo(Main.AssetBundle.LoadAsset<Sprite>("Assets/CharacterItems/Shop Beacon/Icon.png"));
            Dither();
            meshObject.GetComponent<Renderer>().sharedMaterial.mainTexture = Resources.Load<GameObject>("Prefabs/NetworkedObjects/Chest/Chest1").transform.Find("mdlChest1/Cube.001").gameObject.GetComponent<Renderer>().material.mainTexture;
            meshObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_EmPower", 6f);
            meshObject.GetComponent<Renderer>().sharedMaterial.SetColor("_EmColor", Color.black);
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
                if (component.entityStateMachine) component.entityStateMachine.SetNextState(new OpeningCharacterItemShopBeacon());
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

            MysticsItemsContent.Resources.entityStateTypes.Add(typeof(OpeningCharacterItemShopBeacon));
        }

        public override void Load()
        {
            if (Items.CharacterItems.enabled) base.Load();
        }

        public class OpeningCharacterItemShopBeacon : EntityState
        {
            public MaterialPropertyBlock materialPropertyBlock;
            public Renderer renderer;

            public override void OnEnter()
            {
                base.OnEnter();
                PlayAnimation("Body", "Opening", "Opening.playbackRate", duration);
                if (sfxLocator) Util.PlaySound(sfxLocator.openSound, gameObject);
                materialPropertyBlock = new MaterialPropertyBlock();

                ChildLocator childLocator = GetModelChildLocator();
                if (childLocator) renderer = childLocator.FindChildComponent<Renderer>("Mesh");
            }
            
            public override void FixedUpdate()
            {
                base.FixedUpdate();

                if (renderer)
                {
                    renderer.GetPropertyBlock(materialPropertyBlock);
                    float rgb = fixedAge / duration;
                    materialPropertyBlock.SetColor("_EmColor", new Color(rgb, rgb, rgb, 1f));
                    renderer.SetPropertyBlock(materialPropertyBlock);
                }

                if (fixedAge >= duration)
                {
                    outer.SetNextState(new EntityStates.Barrel.Opened());
                    return;
                }
            }

            public static float duration = 6f;
        }
    }
}
