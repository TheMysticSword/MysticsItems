using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

namespace MysticsItems.Equipment
{
    public abstract class BaseEquipment : Items.BaseItem
    {
        public EquipmentDef equipmentDef;
        public static List<BaseEquipment> loadedEquipment = new List<BaseEquipment>();
        public static List<BaseEquipment> equipmentThatUsesTargetFinder = new List<BaseEquipment>();
        public BullseyeSearch targetFinder;
        public TargetFinderType targetFinderType = TargetFinderType.None;
        public GameObject targetFinderVisualizerPrefab;

        public enum TargetFinderType
        {
            None,
            Enemies,
            Friendlies
        }

        public new EquipmentDef Load()
        {
            equipmentDef = ScriptableObject.CreateInstance<EquipmentDef>();
            OnLoad();
            equipmentDef.name = Main.TokenPrefix + equipmentDef.name;
            equipmentDef.AutoPopulateTokens();
            loadedEquipment.Add(this);
            return equipmentDef;
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

            equipmentDef.pickupModelPrefab = model;
            equipmentDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Equipment/" + assetName + "/Icon.png");
        }

        public override PickupIndex GetPickupIndex()
        {
            return PickupCatalog.FindPickupIndex(equipmentDef.equipmentIndex);
        }

        public virtual bool OnUse(EquipmentSlot equipmentSlot) { return false; }

        public virtual void OnUseClient(EquipmentSlot equipmentSlot) { }

        public void UseTargetFinder(TargetFinderType type, GameObject visualizerPrefab)
        {
            targetFinderType = type;
            targetFinderVisualizerPrefab = visualizerPrefab;
            equipmentThatUsesTargetFinder.Add(this);
        }

        public void ConfigureTargetFinderBase(EquipmentSlot self)
        {
            if (targetFinder == null) targetFinder = new BullseyeSearch();
            targetFinder.teamMaskFilter = TeamMask.allButNeutral;
            targetFinder.teamMaskFilter.RemoveTeam(self.characterBody.teamComponent.teamIndex);
            targetFinder.sortMode = BullseyeSearch.SortMode.Angle;
            targetFinder.filterByLoS = true;
            float num;
            Ray ray = CameraRigController.ModifyAimRayIfApplicable(GetAimRay(self), self.gameObject, out num);
            targetFinder.searchOrigin = ray.origin;
            targetFinder.searchDirection = ray.direction;
            targetFinder.maxAngleFilter = 10f;
            targetFinder.viewer = self.characterBody;
        }

        public void ConfigureTargetFinderForEnemies(EquipmentSlot self)
        {
            ConfigureTargetFinderBase(self);
            targetFinder.teamMaskFilter = TeamMask.GetUnprotectedTeams(self.characterBody.teamComponent.teamIndex);
            targetFinder.RefreshCandidates();
            targetFinder.FilterOutGameObject(self.gameObject);
        }

        public void ConfigureTargetFinderForFriendlies(EquipmentSlot self)
        {
            ConfigureTargetFinderBase(self);
            targetFinder.teamMaskFilter = TeamMask.none;
            targetFinder.teamMaskFilter.AddTeam(self.characterBody.teamComponent.teamIndex);
            targetFinder.RefreshCandidates();
            targetFinder.FilterOutGameObject(self.gameObject);
        }

        public static new void Init()
        {
            On.RoR2.EquipmentSlot.PerformEquipmentAction += (orig, self, equipmentDef2) =>
            {
                if (NetworkServer.active)
                {
                    foreach (BaseEquipment equipment in loadedEquipment)
                    {
                        if (equipmentDef2 == equipment.equipmentDef)
                        {
                            return equipment.OnUse(self);
                        }
                    }
                }
                return orig(self, equipmentDef2);
            };

            On.RoR2.EquipmentSlot.RpcOnClientEquipmentActivationRecieved += (orig, self) =>
            {
                orig(self);
                EquipmentIndex equipmentIndex2 = self.equipmentIndex;
                foreach (BaseEquipment equipment in loadedEquipment)
                {
                    if (equipmentIndex2 == equipment.equipmentDef.equipmentIndex)
                    {
                        equipment.OnUseClient(self);
                    }
                }
            };

            On.RoR2.EquipmentSlot.FixedUpdate += (orig, self) =>
            {
                orig(self);
                CurrentTarget currentTarget = self.GetComponent<CurrentTarget>();
                if (!currentTarget) self.gameObject.AddComponent<CurrentTarget>();
            };

            On.RoR2.EquipmentSlot.Update += (orig, self) =>
            {
                orig(self);
                foreach (BaseEquipment equipment in equipmentThatUsesTargetFinder)
                {
                    CurrentTarget targetInfo = self.GetComponent<CurrentTarget>();
                    if (targetInfo)
                    {
                        if (self.stock > 0)
                        {
                            switch (equipment.targetFinderType)
                            {
                                case TargetFinderType.Enemies:
                                    equipment.ConfigureTargetFinderForEnemies(self);
                                    break;
                                case TargetFinderType.Friendlies:
                                    equipment.ConfigureTargetFinderForFriendlies(self);
                                    break;
                            }
                            HurtBox hurtBox = equipment.targetFinder.GetResults().FirstOrDefault();
                            if (hurtBox)
                            {
                                targetInfo.obj = hurtBox.healthComponent.gameObject;
                                targetInfo.indicator.visualizerPrefab = equipment.targetFinderVisualizerPrefab;
                                targetInfo.indicator.targetTransform = hurtBox.transform;
                            }
                            else
                            {
                                targetInfo.Invalidate();
                            }
                            targetInfo.indicator.active = hurtBox;
                        }
                    }
                }
            };
        }

        public override UnlockableDef GetUnlockableDef()
        {
            if (!equipmentDef.unlockableDef)
            {
                equipmentDef.unlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
                equipmentDef.unlockableDef.cachedName = Main.TokenPrefix + "Equipment." + equipmentDef.name;
                equipmentDef.unlockableDef.nameToken = ("EQUIPMENT_" + Main.TokenPrefix + equipmentDef.name + "_NAME").ToUpper();
            }
            return equipmentDef.unlockableDef;
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
