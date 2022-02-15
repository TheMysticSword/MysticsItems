using RoR2;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils;
using System.Collections.Generic;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using System.Linq;
using static MysticsItems.BalanceConfigManager;
using R2API.Networking.Interfaces;
using R2API.Networking;

namespace MysticsItems.Items
{
    public class Manuscript : BaseItem
    {
        public static ConfigurableValue<float> statBonus = new ConfigurableValue<float>(
            "Item: Manuscript",
            "StatBonus",
            10f,
            "Stat increase amount for each stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_MANUSCRIPT_PICKUP",
                "ITEM_MYSTICSITEMS_MANUSCRIPT_DESC"
            }
        );

        private static string tooltipString = "<color=#E6BC4D></color><color=#D4CB6E></color><color=#ABEFA2></color>";

        public override void OnPluginAwake()
        {
            NetworkingAPI.RegisterMessageType<MysticsItemsManuscript.SyncAddBuff>();
            NetworkingAPI.RegisterMessageType<MysticsItemsManuscript.SyncRemoveBuff>();
        }

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_Manuscript";
            itemDef.tier = ItemTier.Tier1;
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Manuscript/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Manuscript/Icon.png");
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Stomach", new Vector3(0.1184F, 0.03107F, 0.13488F), new Vector3(7.10407F, 29.41645F, 4.27448F), new Vector3(0.06273F, 0.06273F, 0.06273F));
                AddDisplayRule("HuntressBody", "Pelvis", new Vector3(-0.07245F, -0.02231F, -0.12606F), new Vector3(359.4893F, 192.2952F, 180.6779F), new Vector3(0.07464F, 0.07464F, 0.07464F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(0.19866F, 0.00068F, -0.07429F), new Vector3(342.5111F, 116.3451F, 2.75113F), new Vector3(0.08171F, 0.08171F, 0.08171F));
                AddDisplayRule("ToolbotBody", "Head", new Vector3(1.96105F, 2.25306F, 0.13524F), new Vector3(358.9201F, 86.74506F, 48.77309F), new Vector3(0.52277F, 0.52277F, 0.52277F));
                AddDisplayRule("EngiBody", "Pelvis", new Vector3(-0.24636F, 0.11208F, -0.00536F), new Vector3(0F, 260.4841F, 185.2644F), new Vector3(0.11046F, 0.11046F, 0.11012F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.83716F, 0.32407F, 0.42391F), new Vector3(9.1868F, 65.17504F, 4.82785F), new Vector3(0.29536F, 0.29536F, 0.29536F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.68571F, 0.97994F, -1.40877F), new Vector3(5.88779F, 88.10306F, 2.28121F), new Vector3(0.21312F, 0.25924F, 0.20835F));
                AddDisplayRule("MageBody", "Stomach", new Vector3(-0.17208F, 0.01912F, 0.08921F), new Vector3(351.3029F, 299.5006F, 345.3905F), new Vector3(0.07512F, 0.07512F, 0.07512F));
                AddDisplayRule("MercBody", "ThighL", new Vector3(0.04919F, 0.02955F, 0.16279F), new Vector3(355.7952F, 39.64868F, 166.2498F), new Vector3(0.07548F, 0.07548F, 0.07548F));
                AddDisplayRule("TreebotBody", "PlatformBase", new Vector3(-0.5374F, 1.17061F, -0.71397F), new Vector3(353.4398F, 216.8F, 0F), new Vector3(0.24613F, 0.24613F, 0.24613F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(0.23444F, -0.02274F, 0.23167F), new Vector3(0F, 89.14168F, 0F), new Vector3(0.08265F, 0.08265F, 0.08265F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(0F, 3.83218F, -0.3716F), new Vector3(0F, 0F, 0F), new Vector3(1.162F, 1.162F, 1.162F));
                AddDisplayRule("CaptainBody", "HandR", new Vector3(0.07594F, 0.19539F, -0.01458F), new Vector3(302.5499F, 277.2516F, 189.9369F), new Vector3(0.086F, 0.086F, 0.086F));
                AddDisplayRule("BrotherBody", "ThighL", BrotherInfection.white, new Vector3(0.00936F, -0.12183F, 0.03127F), new Vector3(7.67382F, 142.5023F, 346.2559F), new Vector3(0.115F, 0.063F, 0.063F));
                AddDisplayRule("ScavBody", "MuzzleEnergyCannon", new Vector3(2.28289F, -3.68348F, -17.40374F), new Vector3(58.55879F, 87.78275F, 267.3442F), new Vector3(1.94718F, 2.00081F, 1.94718F));
            };

            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
            On.RoR2.Inventory.RemoveItem_ItemIndex_int += Inventory_RemoveItem_ItemIndex_int;

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;

            if (!SoftDependencies.SoftDependenciesCore.itemStatsCompatEnabled) On.RoR2.UI.ItemIcon.SetItemIndex += ItemIcon_SetItemIndex;

            MysticsItemsManuscript.Init();
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (!self.inventory.GetComponent<MysticsItemsManuscript>())
            {
                self.inventory.gameObject.AddComponent<MysticsItemsManuscript>();
            }
        }

        private void ItemIcon_SetItemIndex(On.RoR2.UI.ItemIcon.orig_SetItemIndex orig, RoR2.UI.ItemIcon self, ItemIndex newItemIndex, int newItemCount)
        {
            orig(self, newItemIndex, newItemCount);

            if (newItemIndex == itemDef.itemIndex)
            {
                Transform parent = self.transform.parent;
                if (parent)
                {
                    RoR2.UI.ItemInventoryDisplay itemInventoryDisplay = parent.GetComponent<RoR2.UI.ItemInventoryDisplay>();
                    if (itemInventoryDisplay && itemInventoryDisplay.inventory)
                    {
                        MysticsItemsManuscript manuscript = itemInventoryDisplay.inventory.GetComponent<MysticsItemsManuscript>();
                        if (manuscript)
                        {
                            globalStringBuilder.Clear();
                            globalStringBuilder.Append(Language.GetString(self.tooltipProvider.bodyToken) + "\r\n");
                            globalStringBuilder.Append("\r\n");
                            globalStringBuilder.Append(Language.GetString("MYSTICSITEMS_STATCHANGE_LIST_HEADER"));
                            foreach (var buffType in manuscript.buffOrder)
                            {
                                globalStringBuilder.Append("\r\n");
                                globalStringBuilder.Append(
                                    Language.GetStringFormatted(
                                        "MYSTICSITEMS_STATCHANGE_LIST_" + buffType.ToString().ToUpperInvariant(),
                                        "+" + (manuscript.buffStacks[buffType] * statBonus).ToString(System.Globalization.CultureInfo.InvariantCulture)
                                    )
                                );
                            }
                            globalStringBuilder.Append(tooltipString);
                            self.tooltipProvider.overrideBodyText = globalStringBuilder.ToString();
                            globalStringBuilder.Clear();
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(self.tooltipProvider.overrideBodyText) && self.tooltipProvider.overrideBodyText.Contains(tooltipString))
                    self.tooltipProvider.overrideBodyText = "";
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            Inventory inventory = sender.inventory;
            if (inventory)
            {
                MysticsItemsManuscript component = inventory.GetComponent<MysticsItemsManuscript>();
                if (component)
                {
                    args.healthMultAdd += statBonus / 100f * component.buffStacks[MysticsItemsManuscript.BuffType.MaxHealth];
                    args.regenMultAdd += statBonus / 100f * component.buffStacks[MysticsItemsManuscript.BuffType.Regen];
                    args.damageMultAdd += statBonus / 100f * component.buffStacks[MysticsItemsManuscript.BuffType.Damage];
                    args.moveSpeedMultAdd += statBonus / 100f * component.buffStacks[MysticsItemsManuscript.BuffType.MoveSpeed];
                    args.attackSpeedMultAdd += statBonus / 100f * component.buffStacks[MysticsItemsManuscript.BuffType.AttackSpeed];
                    args.critAdd += statBonus * component.buffStacks[MysticsItemsManuscript.BuffType.Crit];
                    args.armorAdd += statBonus * component.buffStacks[MysticsItemsManuscript.BuffType.Armor];
                }
            }
        }

        public void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            MysticsItemsManuscript component = self.GetComponent<MysticsItemsManuscript>();
            if (!component) component = self.gameObject.AddComponent<MysticsItemsManuscript>();
            orig(self, itemIndex, count);
            if (NetworkServer.active && itemIndex == itemDef.itemIndex) for (var i = 0; i < count; i++) component.AddBuff();
        }

        public void Inventory_RemoveItem_ItemIndex_int(On.RoR2.Inventory.orig_RemoveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            MysticsItemsManuscript component = self.GetComponent<MysticsItemsManuscript>();
            if (!component) component = self.gameObject.AddComponent<MysticsItemsManuscript>();
            orig(self, itemIndex, count);
            if (NetworkServer.active && itemIndex == itemDef.itemIndex) for (var i = 0; i < count; i++) component.RemoveBuff();
        }

        public class MysticsItemsManuscript : MonoBehaviour
        {
            public enum BuffType
            {
                MaxHealth,
                Regen,
                Damage,
                MoveSpeed,
                AttackSpeed,
                Crit,
                Armor
            }
            public static List<BuffType> buffTypes;
            public List<BuffType> buffOrder;
            public Dictionary<BuffType, int> buffStacks;

            internal static void Init()
            {
                buffTypes = ((BuffType[])System.Enum.GetValues(typeof(BuffType))).ToList();
            }

            public void Awake()
            {
                buffStacks = new Dictionary<BuffType, int>();
                for (var i = 0; i < buffTypes.Count; i++) buffStacks.Add(buffTypes[i], 0);
                buffOrder = new List<BuffType>();
            }

            public void AddBuff()
            {
                AddBuff(RoR2Application.rng.NextElementUniform(buffTypes));
            }

            public void AddBuff(BuffType chosenBuffType)
            {
                if (NetworkServer.active)
                    new SyncAddBuff(gameObject.GetComponent<NetworkIdentity>().netId, (int)chosenBuffType).Send(NetworkDestination.Clients);
                if (!buffOrder.Contains(chosenBuffType)) buffOrder.Add(chosenBuffType);
                buffStacks[chosenBuffType]++;
            }

            public void RemoveBuff()
            {
                if (NetworkServer.active)
                    new SyncRemoveBuff(gameObject.GetComponent<NetworkIdentity>().netId).Send(NetworkDestination.Clients);
                if (buffOrder.Count > 0)
                {
                    buffStacks[buffOrder[buffOrder.Count - 1]]--;
                    if (buffStacks[buffOrder[buffOrder.Count - 1]] <= 0) buffOrder.RemoveAt(buffOrder.Count - 1);
                }
            }

            public class SyncAddBuff : INetMessage
            {
                NetworkInstanceId objID;
                int chosenBuffType;

                public SyncAddBuff()
                {
                }

                public SyncAddBuff(NetworkInstanceId objID, int chosenBuffType)
                {
                    this.objID = objID;
                    this.chosenBuffType = chosenBuffType;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                    chosenBuffType = reader.ReadInt32();
                }

                public void OnReceived()
                {
                    if (NetworkServer.active) return;
                    GameObject obj = Util.FindNetworkObject(objID);
                    if (obj)
                    {
                        MysticsItemsManuscript controller = obj.GetComponent<MysticsItemsManuscript>();
                        if (controller)
                        {
                            controller.AddBuff((BuffType)chosenBuffType);
                        }
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                    writer.Write(chosenBuffType);
                }
            }

            public class SyncRemoveBuff : INetMessage
            {
                NetworkInstanceId objID;

                public SyncRemoveBuff()
                {
                }

                public SyncRemoveBuff(NetworkInstanceId objID)
                {
                    this.objID = objID;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                }

                public void OnReceived()
                {
                    if (NetworkServer.active) return;
                    GameObject obj = Util.FindNetworkObject(objID);
                    if (obj)
                    {
                        MysticsItemsManuscript controller = obj.GetComponent<MysticsItemsManuscript>();
                        if (controller)
                        {
                            controller.RemoveBuff();
                        }
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                }
            }
        }
    }
}
