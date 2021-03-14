using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace MysticsItems.Equipment
{
    public abstract class BaseEquipment : Items.BaseItem
    {
        public EquipmentDef equipmentDef;
        public EquipmentIndex equipmentIndex;
        public static Dictionary<System.Type, BaseEquipment> registeredEquipment = new Dictionary<System.Type, BaseEquipment>();

        public new static BaseEquipment GetFromType(System.Type type)
        {
            if (registeredEquipment.ContainsKey(type))
            {
                return registeredEquipment[type];
            }
            return null;
        }

        public override void Add()
        {
            equipmentDef = new EquipmentDef();
            PreAdd();
            equipmentDef.name = Main.TokenPrefix + equipmentDef.name;
            equipmentIndex = ItemAPI.Add(new CustomEquipment(equipmentDef, itemDisplayRuleDict));
            registeredEquipment.Add(GetType(), this);
            OnAdd();
            On.RoR2.EquipmentSlot.PerformEquipmentAction += (orig, self, equipmentIndex2) =>
            {
                if (NetworkServer.active)
                {
                    if (equipmentIndex2 == equipmentIndex)
                    {
                        return OnUse(self);
                    }
                }
                return orig(self, equipmentIndex2);
            };
        }

        public override void SetAssets(string assetName)
        {
            model = Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/" + assetName + "/Model.prefab");
            model.name = "mdl" + equipmentDef.name;

            bool followerModelSeparate = Main.AssetBundle.Contains("Assets/Equipment/" + assetName + "/FollowerModel.prefab");
            if (followerModelSeparate)
            {
                followerModel = Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/" + assetName + "/FollowerModel.prefab");
                followerModel.name = "mdl" + equipmentDef.name + "Follower";
            }

            PrepareModel(model);
            if (followerModelSeparate) PrepareModel(followerModel);

            // Separate the follower model from the pickup model for adding different visual effects to followers
            if (!followerModelSeparate) CopyModelToFollower();

            equipmentDef.pickupModelPath = Main.AssetPrefix + ":Assets/Equipment/" + assetName + "/Model.prefab";
            equipmentDef.pickupIconPath = Main.AssetPrefix + ":Assets/Equipment/" + assetName + "/Icon.png";
        }

        public override PickupIndex GetPickupIndex()
        {
            return PickupCatalog.FindPickupIndex(equipmentIndex);
        }

        public override void SetDisplayRuleGroup(ItemDisplayRuleSet idrs, DisplayRuleGroup displayRuleGroup)
        {
            idrs.SetEquipmentDisplayRuleGroup(equipmentDef.name, displayRuleGroup);
        }

        public virtual bool OnUse(EquipmentSlot equipmentSlot) { return false; }

        public static new void Init()
        {
            On.RoR2.EquipmentSlot.Awake += (orig, self) =>
            {
                orig(self);
                self.gameObject.AddComponent<CurrentTarget>();
            };
        }

        public override void SetUnlockable()
        {
            equipmentDef.unlockableName = Main.TokenPrefix + "Equipment." + equipmentDef.name;
            Unlockables.Register(equipmentDef.unlockableName, new UnlockableDef
            {
                nameToken = !string.IsNullOrEmpty(equipmentDef.nameToken) ? equipmentDef.nameToken : ("EQUIPMENT_" + Main.TokenPrefix + equipmentDef.name + "_NAME").ToUpper()
            });
        }

        public static Ray GetAimRay(EquipmentSlot equipmentSlot)
        {
            return equipmentSlot.InvokeMethod<Ray>("GetAimRay");
        }

        public class CurrentTarget : MonoBehaviour
        {
            public GameObject obj;
            public Indicator indicator;

            public void Awake()
            {
                indicator = new Indicator(gameObject, null);
            }

            public void Invalidate()
            {
                obj = null;
                indicator.targetTransform = null;
            }
        }
    }
}
