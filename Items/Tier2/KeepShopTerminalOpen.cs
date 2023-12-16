using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Collections.Generic;
using R2API.Networking.Interfaces;
using R2API.Networking;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Items
{
    public class KeepShopTerminalOpen : BaseItem
    {
        public static NetworkSoundEventDef sfx;

        public static ConfigurableValue<float> discount = new ConfigurableValue<float>(
            "Item: Platinum Card",
            "Discount",
            75f,
            "How much should the price of the terminal decrease on trigger (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_KEEPSHOPTERMINALOPEN_PICKUP",
                "ITEM_MYSTICSITEMS_KEEPSHOPTERMINALOPEN_DESC"
            }
        );

        public override void OnPluginAwake()
        {
            //NetworkingAPI.RegisterMessageType<MysticsItemsKeepShopTerminalOpenBehaviour.SyncSetTerminalState>();
        }

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_KeepShopTerminalOpen";
            SetItemTierWhenAvailable(ItemTier.Tier2);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Platinum Card/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Platinum Card/Icon.png");
            MysticsItemsContent.Resources.unlockableDefs.Add(GetUnlockableDef());
            HopooShaderToMaterial.Standard.Apply(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial);
            HopooShaderToMaterial.Standard.DisableEverything(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial);
            HopooShaderToMaterial.Standard.Gloss(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial, 0.1f, 3f);
            itemDisplayPrefab = PrepareItemDisplayModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Platinum Card/FollowerModel.prefab"));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "GunMeshL", new Vector3(-0.14803F, 0.02213F, 0.12401F), new Vector3(71.07552F, 0F, 0F), new Vector3(0.042F, 0.042F, 0.042F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(-0.08864F, 0.2563F, -0.11958F), new Vector3(343.8997F, 75.59196F, 43.60138F), new Vector3(0.042F, 0.042F, 0.042F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(-0.16663F, 0.07171F, -0.03448F), new Vector3(340.1071F, 88.24358F, 257.9142F), new Vector3(0.037F, 0.037F, 0.037F));
                AddDisplayRule("ToolbotBody", "Head", new Vector3(-1.20046F, 2.3477F, 0.38287F), new Vector3(7.40534F, 171.1227F, 354.4013F), new Vector3(0.489F, 0.489F, 0.489F));
                AddDisplayRule("EngiBody", "Chest", new Vector3(-0.1787F, 0.14487F, 0.22069F), new Vector3(328.6561F, 145.9245F, 8.96297F), new Vector3(0.04921F, 0.04921F, 0.04921F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.4724F, 0.67251F, 0.05926F), new Vector3(31.60266F, 67.34644F, 23.17469F), new Vector3(0.168F, 0.168F, 0.168F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0F, 1.40432F, -0.53826F), new Vector3(72.01626F, 0F, 0F), new Vector3(0.134F, 0.163F, 0.131F));
                AddDisplayRule("MageBody", "Head", new Vector3(-0.08953F, 0.11065F, -0.10433F), new Vector3(357.7894F, 103.1625F, 271.0524F), new Vector3(0.044F, 0.044F, 0.044F));
                AddDisplayRule("MercBody", "Pelvis", new Vector3(-0.10721F, 0.03029F, 0.08815F), new Vector3(13.93437F, 164.9596F, 180F), new Vector3(0.034F, 0.034F, 0.034F));
                AddDisplayRule("TreebotBody", "WeaponPlatform", new Vector3(0.12857F, 0.44352F, 0.29435F), new Vector3(339.8401F, 258.922F, 32.09783F), new Vector3(0.107F, 0.107F, 0.107F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(0.12546F, 0.07357F, -0.12968F), new Vector3(7.50558F, 26.88992F, 245.9819F), new Vector3(0.05961F, 0.05961F, 0.05961F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(1.227F, 2.85624F, -0.59417F), new Vector3(341.2246F, 0F, 0F), new Vector3(0.50349F, 0.50349F, 0.50349F));
                AddDisplayRule("CaptainBody", "Stomach", new Vector3(0.10938F, 0.16565F, 0.15251F), new Vector3(351.8886F, 224.6353F, 241.3711F), new Vector3(0.053F, 0.048F, 0.053F));
                AddDisplayRule("BrotherBody", "Stomach", BrotherInfection.green, new Vector3(0.1446F, 0.10245F, 0.12114F), new Vector3(296.6585F, 215.6237F, 260.6332F), new Vector3(0.063F, 0.063F, 0.06164F));
                AddDisplayRule("ScavBody", "Backpack", new Vector3(-5.27366F, 11.30539F, 2.34812F), new Vector3(26.35906F, 351.5907F, 98.18745F), new Vector3(1.31515F, 1.35137F, 1.31515F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Chest", new Vector3(0.10082F, 0.38579F, -0.25451F), new Vector3(0F, 90F, 270F), new Vector3(0.05257F, 0.05257F, 0.05257F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysDeputy) AddDisplayRule("DeputyBody", "Chest", new Vector3(0.06402F, 0.09178F, 0.1434F), new Vector3(276.4962F, 262.4514F, 304.5559F), new Vector3(0.02664F, 0.02664F, 0.02664F));
                AddDisplayRule("RailgunnerBody", "Backpack", new Vector3(0.11811F, 0.41013F, -0.08114F), new Vector3(0F, 90F, 270F), new Vector3(0.04824F, 0.04824F, 0.04824F));
                AddDisplayRule("VoidSurvivorBody", "Chest", new Vector3(0.03282F, -0.13304F, -0.12525F), new Vector3(354.9498F, 87.16938F, 256.3283F), new Vector3(0.04222F, 0.04222F, 0.04222F));
            };

            /*
            On.RoR2.MultiShopController.Start += (orig, self) =>
            {
                orig(self);
                self.gameObject.AddComponent<MysticsItemsKeepShopTerminalOpenBehaviour>();
            };
            */

            On.RoR2.Stage.BeginServer += (orig, self) =>
            {
                orig(self);
                foreach (CharacterMaster characterMaster in CharacterMaster.readOnlyInstancesList)
                {
                    Inventory inventory = characterMaster.inventory;
                    if (inventory)
                    {
                        int count = inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_KeepShopTerminalOpenConsumed);
                        inventory.RemoveItem(MysticsItemsContent.Items.MysticsItems_KeepShopTerminalOpenConsumed, count);
                        inventory.GiveItem(MysticsItemsContent.Items.MysticsItems_KeepShopTerminalOpen, count);
                    }
                }
            };

            sfx = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            sfx.eventName = "MysticsItems_Play_item_proc_creditcard";
            MysticsItemsContent.Resources.networkSoundEventDefs.Add(sfx);

            On.RoR2.Items.MultiShopCardUtils.OnPurchase += MultiShopCardUtils_OnPurchase;
        }

        private void MultiShopCardUtils_OnPurchase(On.RoR2.Items.MultiShopCardUtils.orig_OnPurchase orig, CostTypeDef.PayCostContext context, int moneyCost)
        {
            orig(context, moneyCost);
            CharacterMaster activatorMaster = context.activatorMaster;
            if (activatorMaster && activatorMaster.hasBody && activatorMaster.inventory && activatorMaster.inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_KeepShopTerminalOpen) > 0 && context.purchasedObject)
            {
                ShopTerminalBehavior shopTerminalBehavior = context.purchasedObject.GetComponent<ShopTerminalBehavior>();
                if (shopTerminalBehavior && shopTerminalBehavior.serverMultiShopController)
                {
                    var remainingTerminals = shopTerminalBehavior.serverMultiShopController.terminalGameObjects
                        .Where(x => x)
                        .Select(x => x.GetComponent<PurchaseInteraction>())
                        .Where(x => x.Networkavailable)
                        .Count();

                    if (remainingTerminals > 1)
                    {
                        shopTerminalBehavior.serverMultiShopController.SetCloseOnTerminalPurchase(context.purchasedObject.GetComponent<PurchaseInteraction>(), false);
                        activatorMaster.inventory.RemoveItem(MysticsItemsContent.Items.MysticsItems_KeepShopTerminalOpen);
                        activatorMaster.inventory.GiveItem(MysticsItemsContent.Items.MysticsItems_KeepShopTerminalOpenConsumed);

                        RoR2.Audio.PointSoundManager.EmitSoundServer(KeepShopTerminalOpen.sfx.index, shopTerminalBehavior.transform.position);

                        shopTerminalBehavior.serverMultiShopController.Networkcost = (int)(shopTerminalBehavior.serverMultiShopController.Networkcost * (1f - discount / 100f));
                        foreach (var terminal in shopTerminalBehavior.serverMultiShopController.terminalGameObjects)
                        {
                            if (terminal)
                            {
                                var purchaseInteraction = terminal.GetComponent<PurchaseInteraction>();
                                if (purchaseInteraction)
                                    purchaseInteraction.Networkcost = (int)(purchaseInteraction.Networkcost * (1f - discount / 100f));
                            }
                        }
                    }
                }
            }
        }
    }

    /*
    public class MysticsItemsKeepShopTerminalOpenBehaviour : MonoBehaviour
    {
        public List<PickupIndex> terminalPickups;
        public List<bool> terminalPickupsHideMode;
        public List<GameObject> terminals;
        public MultiShopController multiShopController;

        public void Start()
        {
            multiShopController = GetComponent<MultiShopController>();
            terminals = new List<GameObject>();
            terminalPickups = new List<PickupIndex>();
            terminalPickupsHideMode = new List<bool>();

            if (NetworkServer.active)
            {
                for (var i = 0; i < multiShopController.terminalGameObjects.Length; i++)
                {
                    GameObject terminal = multiShopController.terminalGameObjects[i];
                    terminals.Add(terminal);
                    terminalPickups.Add(terminal.GetComponent<ShopTerminalBehavior>().pickupIndex);
                    terminalPickupsHideMode.Add(terminal.GetComponent<ShopTerminalBehavior>().hidden);
                    SetTerminalState(terminal, false);
                    terminal.GetComponent<PurchaseInteraction>().onPurchase.AddListener((interactor) => {
                        TerminalOnPurchase(interactor, terminal);
                    });
                }
            }
        }

        public void TerminalOnPurchase(Interactor interactor, GameObject terminal)
        {
            if (terminals.Contains(terminal) && terminalPickups[terminals.IndexOf(terminal)] != PickupIndex.none)
            {
                terminalPickups[terminals.IndexOf(terminal)] = PickupIndex.none;
                CharacterBody characterBody = interactor.GetComponent<CharacterBody>();
                if (characterBody)
                {
                    Inventory inventory = characterBody.inventory;
                    if (inventory)
                    {
                        int itemCount = inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_KeepShopTerminalOpen);
                        if (itemCount > 0 && Reopen())
                        {
                            inventory.RemoveItem(MysticsItemsContent.Items.MysticsItems_KeepShopTerminalOpen);
                            inventory.GiveItem(MysticsItemsContent.Items.MysticsItems_KeepShopTerminalOpenConsumed);

                            RoR2.Audio.EntitySoundManager.EmitSoundServer(KeepShopTerminalOpen.sfx.index, terminal);
                        }
                    }
                }
            }
        }

        public bool Reopen()
        {
            if (NetworkServer.active)
            {
                multiShopController.Networkcost = (int)(multiShopController.cost * (1f - KeepShopTerminalOpen.discount / 100f));

                List<PickupIndex> availablePickups = terminalPickups.FindAll(x => x != PickupIndex.none);
                if (availablePickups.Count > 0)
                {
                    multiShopController.Networkavailable = true;
                    for (var i = 0; i < multiShopController.terminalGameObjects.Length; i++)
                    {
                        GameObject gameObject = multiShopController.terminalGameObjects[i];
                        if (terminalPickups[i] != PickupIndex.none)
                        {
                            //multiShopController.doCloseOnTerminalPurchase[i] = false;
                            gameObject.GetComponent<PurchaseInteraction>().Networkavailable = true;
                            gameObject.GetComponent<PurchaseInteraction>().Networkcost = (int)(gameObject.GetComponent<PurchaseInteraction>().cost * 0.9f);
                            gameObject.GetComponent<ShopTerminalBehavior>().SetPickupIndex(terminalPickups[i], terminalPickupsHideMode[i]);
                            gameObject.GetComponent<ShopTerminalBehavior>().SetHasBeenPurchased(false);
                            SetTerminalState(gameObject, true);
                        }
                        else
                        {
                            SetTerminalState(gameObject, false);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public static void SetTerminalState(GameObject gameObject, bool open)
        {
            if (NetworkServer.active)
            {
                new SyncSetTerminalState(gameObject.GetComponent<NetworkIdentity>().netId, open).Send(NetworkDestination.Clients);
            }
            if (open)
            {
                int layerIndex = gameObject.GetComponent<ShopTerminalBehavior>().animator.GetLayerIndex("Body");
                gameObject.GetComponent<ShopTerminalBehavior>().animator.PlayInFixedTime("Open", layerIndex);
                gameObject.GetComponent<ShopTerminalBehavior>().animator.speed = 0f;
            }
            else
            {
                gameObject.GetComponent<ShopTerminalBehavior>().animator.speed = 1f;
            }
        }

        public class SyncSetTerminalState : INetMessage
        {
            NetworkInstanceId objID;
            bool open;

            public SyncSetTerminalState()
            {
            }

            public SyncSetTerminalState(NetworkInstanceId objID, bool open)
            {
                this.objID = objID;
                this.open = open;
            }

            public void Deserialize(NetworkReader reader)
            {
                objID = reader.ReadNetworkId();
                open = reader.ReadBoolean();
            }

            public void OnReceived()
            {
                if (NetworkServer.active) return;
                GameObject obj = Util.FindNetworkObject(objID);
                if (obj)
                {
                    SetTerminalState(obj, open);
                }
            }

            public void Serialize(NetworkWriter writer)
            {
                writer.Write(objID);
                writer.Write(open);
            }
        }
    }
    */
}
