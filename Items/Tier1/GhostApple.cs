using RoR2;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.LegacyBalanceConfigManager;
using System.Collections.Generic;

namespace MysticsItems.Items
{
    public class GhostApple : BaseItem
    {
        public static ConfigurableValue<float> regen = new ConfigurableValue<float>(
            "Item: Ghost Apple",
            "Regen",
            2f,
            "Regeneration increase from this item (in HP/s)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_GHOSTAPPLE_DESC"
            }
        );
        public static ConfigurableValue<float> minutes = new ConfigurableValue<float>(
            "Item: Ghost Apple",
            "Minutes",
            20f,
            "How long should the item last until it turns into the weaker version (in minutes)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_GHOSTAPPLE_PICKUP",
                "ITEM_MYSTICSITEMS_GHOSTAPPLE_DESC"
            }
        );

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_GhostApple";
            SetItemTierWhenAvailable(ItemTier.Tier1);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Healing
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Ghost Apple/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Ghost Apple/Icon.png");

            itemDisplayPrefab = PrepareItemDisplayModel(PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Ghost Apple/FollowerModel.prefab")));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Stomach", new Vector3(-0.16905F, 0.08873F, -0.05698F), new Vector3(6.13585F, 341.4908F, 17.70715F), new Vector3(0.02301F, 0.02301F, 0.02301F));
                AddDisplayRule("HuntressBody", "Pelvis", new Vector3(-0.08822F, -0.10225F, 0.08362F), new Vector3(345.8347F, 176.5909F, 180.8352F), new Vector3(0.02242F, 0.02242F, 0.02242F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(-0.12375F, 0.03791F, -0.12189F), new Vector3(340.7072F, 24.74142F, 351.343F), new Vector3(0.02805F, 0.02805F, 0.02805F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(0.53646F, 2.04978F, -0.23195F), new Vector3(25.30116F, 73.93198F, 31.19371F), new Vector3(0.31308F, 0.31308F, 0.31308F));
                AddDisplayRule("EngiBody", "Pelvis", new Vector3(-0.13157F, 0.03205F, 0.229F), new Vector3(18.35512F, 146.7131F, 180F), new Vector3(0.03243F, 0.03243F, 0.03243F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.31903F, 0.80205F, -0.69238F), new Vector3(359.9257F, 100.9982F, 25.69395F), new Vector3(0.11999F, 0.11999F, 0.11999F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(-0.06036F, 1.36447F, -1.14807F), new Vector3(324.2756F, 119.1527F, 340.6565F), new Vector3(0.10744F, 0.1307F, 0.10504F));
                AddDisplayRule("MageBody", "Stomach", new Vector3(-0.17845F, 0.13463F, -0.05802F), new Vector3(33.31532F, 244.8169F, 355.2454F), new Vector3(0.02721F, 0.02721F, 0.02721F));
                AddDisplayRule("MercBody", "ThighL", new Vector3(0.10701F, -0.08252F, 0.00921F), new Vector3(337.715F, 152.3524F, 160.8945F), new Vector3(0.02243F, 0.02243F, 0.02243F));
                AddDisplayRule("TreebotBody", "MR2UAntennae4", new Vector3(-0.02388F, -0.48116F, -0.0098F), new Vector3(0F, 0F, 0F), new Vector3(0.10546F, 0.10546F, 0.10546F));
                AddDisplayRule("LoaderBody", "Stomach", new Vector3(-0.21253F, 0.03006F, 0.00002F), new Vector3(0.00577F, 10.91915F, 13.23574F), new Vector3(0.03144F, 0.03144F, 0.03144F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(0.01012F, 4.63169F, -0.13406F), new Vector3(0F, 0F, 0F), new Vector3(0.28688F, 0.28688F, 0.28688F));
                AddDisplayRule("CaptainBody", "Chest", new Vector3(0.00136F, 0.37981F, 0.1799F), new Vector3(350.816F, 280.0829F, 276.5578F), new Vector3(0.02509F, 0.02509F, 0.02509F));
                AddDisplayRule("BrotherBody", "ThighL", BrotherInfection.white, new Vector3(-0.01699F, -0.02623F, -0.04174F), new Vector3(0F, 245.7886F, 340.9104F), new Vector3(0.09872F, 0.09872F, 0.09872F));
                AddDisplayRule("ScavBody", "Backpack", new Vector3(-14.77598F, 11.45091F, 1.75593F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Pelvis", new Vector3(-0.21374F, 0.20372F, -0.13957F), new Vector3(347.6528F, 356.2771F, 16.92452F), new Vector3(0.03765F, 0.03765F, 0.03765F));
                AddDisplayRule("RailgunnerBody", "GunScope", new Vector3(-0.00001F, 0.28852F, 0.30081F), new Vector3(0F, 0F, 180F), new Vector3(0.0276F, 0.0276F, 0.0276F));
                AddDisplayRule("VoidSurvivorBody", "Head", new Vector3(-0.1118F, 0.12395F, 0.07527F), new Vector3(36.57103F, 270F, 358.9005F), new Vector3(0.0327F, 0.0327F, 0.0327F));
            };

            On.RoR2.CharacterMaster.OnInventoryChanged += CharacterMaster_OnInventoryChanged;

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            Inventory inventory = sender.inventory;
            if (inventory)
            {
                int itemCount = inventory.GetItemCount(itemDef);
                if (itemCount > 0)
                {
                    args.baseRegenAdd += regen * itemCount * (1f + 0.2f * (sender.level - 1f));
                }
            }
        }

        private void CharacterMaster_OnInventoryChanged(On.RoR2.CharacterMaster.orig_OnInventoryChanged orig, CharacterMaster self)
        {
            orig(self);

            MysticsItemsGhostAppleBehavior component = MysticsItemsGhostAppleBehavior.GetForMaster(self);
            if (!component.skipItemCheck)
            {
                int itemCount = self.inventory.GetItemCount(itemDef);
                var difference = itemCount - component.oldItemCount;
                for (var i = 0; i < System.Math.Abs(difference); i++)
                {
                    if (difference > 0) component.timers.Add(60f * minutes);
                    else if (component.timers.Count > 0) component.timers.RemoveAt(component.timers.Count - 1);
                }
                component.oldItemCount = itemCount;
            }
        }

        public class MysticsItemsGhostAppleBehavior : MonoBehaviour
        {
            public List<float> timers = new List<float>();
            public CharacterMaster master;
            public bool skipItemCheck = false;
            public int oldItemCount = 0;

            public static Dictionary<CharacterMaster, MysticsItemsGhostAppleBehavior> masterToBehaviourDict = new Dictionary<CharacterMaster, MysticsItemsGhostAppleBehavior>();
            public static MysticsItemsGhostAppleBehavior GetForMaster(CharacterMaster master)
            {
                if (!masterToBehaviourDict.ContainsKey(master))
                    master.gameObject.AddComponent<MysticsItemsGhostAppleBehavior>();
                return masterToBehaviourDict[master];
            }

            public void Awake()
            {
                master = GetComponent<CharacterMaster>();
                masterToBehaviourDict[master] = this;
            }

            public void FixedUpdate()
            {
                for (var i = 0; i < timers.Count; i++)
                {
                    timers[i] -= Time.fixedDeltaTime;
                    if (timers[i] <= 0)
                    {
                        if (master.hasBody)
                        {
                            var body = master.GetBody();
                            skipItemCheck = true;
                            body.inventory.RemoveItem(MysticsItemsContent.Items.MysticsItems_GhostApple);
                            body.inventory.GiveItem(MysticsItemsContent.Items.MysticsItems_GhostAppleWeak);
                            skipItemCheck = false;

                            if (NetworkServer.active)
                            {
                                CharacterMasterNotificationQueue.PushItemTransformNotification(
                                    master,
                                    MysticsItemsContent.Items.MysticsItems_GhostApple.itemIndex,
                                    MysticsItemsContent.Items.MysticsItems_GhostAppleWeak.itemIndex,
                                    CharacterMasterNotificationQueue.TransformationType.Default
                                );
                                NetworkPickupDiscovery.DiscoverPickup(master, LimitedArmorBroken.pickupIndex);
                            }
                        }

                        timers.RemoveAt(i);
                        i--;
                    }
                    if (i >= timers.Count) break;
                }
            }

            public void OnDestroy()
            {
                masterToBehaviourDict.Remove(master);
            }
        }
    }
}
