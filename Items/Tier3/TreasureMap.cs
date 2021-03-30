using RoR2;
using RoR2.Hologram;
using RoR2.Audio;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Networking;
using R2API.Networking.Interfaces;
using System.Collections.ObjectModel;
using System.Text;
using TMPro;
using System.Collections.Generic;

namespace MysticsItems.Items
{
    public class TreasureMap : BaseItem
    {
        public static GameObject zonePrefab;
        public static SpawnCard zoneSpawnCard;
        public static Material ghostMaterial;
        public static NetworkSoundEventDef soundEventDef;

        public override void OnLoad()
        {
            itemDef.name = "TreasureMap";
            itemDef.tier = ItemTier.Tier3;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist
            };
            BanFromDeployables();
            SetAssets("Treasure Map");
            SetModelPanelDistance(3f, 6f);
            AddDisplayRule("CommandoBody", "LowerArmR", new Vector3(-0.084F, 0.183F, -0.006F), new Vector3(83.186F, 36.557F, 131.348F), new Vector3(0.053F, 0.053F, 0.053F));
            AddDisplayRule("HuntressBody", "Muzzle", new Vector3(-0.527F, -0.032F, -0.396F), new Vector3(0.509F, 134.442F, 184.268F), new Vector3(0.042F, 0.042F, 0.042F));
            AddDisplayRule("ToolbotBody", "Head", new Vector3(0.198F, 3.655F, -0.532F), new Vector3(304.724F, 180F, 180F), new Vector3(0.448F, 0.448F, 0.448F));
            AddDisplayRule("EngiBody", "WristDisplay", new Vector3(0.01F, -0.001F, 0.007F), new Vector3(86.234F, 155.949F, 155.218F), new Vector3(0.065F, 0.065F, 0.065F));
            AddDisplayRule("MageBody", "LowerArmR", new Vector3(0.116F, 0.188F, 0.008F), new Vector3(88.872F, 20.576F, 290.58F), new Vector3(0.074F, 0.074F, 0.074F));
            AddDisplayRule("MercBody", "LowerArmR", new Vector3(-0.01F, 0.144F, -0.116F), new Vector3(277.017F, 64.808F, 295.358F), new Vector3(0.072F, 0.072F, 0.072F));
            AddDisplayRule("TreebotBody", "HeadBase", new Vector3(-0.013F, 0.253F, -0.813F), new Vector3(1.857F, 5.075F, 0.053F), new Vector3(0.13F, 0.143F, 0.294F));
            AddDisplayRule("LoaderBody", "MechLowerArmR", new Vector3(-0.01F, 0.544F, -0.144F), new Vector3(275.35F, 95.995F, 266.284F), new Vector3(0.095F, 0.095F, 0.095F));
            AddDisplayRule("CrocoBody", "UpperArmR", new Vector3(1.735F, -0.575F, 0.196F), new Vector3(281.472F, 180.072F, 89.927F), new Vector3(0.868F, 0.868F, 0.868F));
            AddDisplayRule("CaptainBody", "HandR", new Vector3(-0.066F, 0.087F, 0.011F), new Vector3(76.759F, 135.292F, 224.52F), new Vector3(0.059F, 0.053F, 0.059F));
            AddDisplayRule("BrotherBody", "HandR", BrotherInfection.red, new Vector3(0.051F, -0.072F, 0.004F), new Vector3(44.814F, 122.901F, 267.545F), new Vector3(0.063F, 0.063F, 0.063F));

            NetworkingAPI.RegisterMessageType<MysticsItemsTreasureMapZone.SyncRewardLocked>();
            NetworkingAPI.RegisterMessageType<MysticsItemsTreasureMapZone.SyncActive>();

            zonePrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Treasure Map/TreasureMapZone.prefab");
            zonePrefab.AddComponent<NetworkIdentity>();
            HoldoutZoneController holdoutZone = zonePrefab.AddComponent<HoldoutZoneController>();
            holdoutZone.baseRadius = 15f;
            holdoutZone.baseChargeDuration = 120f;
            holdoutZone.radiusSmoothTime = 1f;
            holdoutZone.radiusIndicator = zonePrefab.transform.Find("Visuals/Sphere").gameObject.GetComponent<Renderer>();
            holdoutZone.inBoundsObjectiveToken = Main.TokenPrefix.ToUpper() + "OBJECTIVE_CHARGE_TREASUREMAPZONE";
            holdoutZone.outOfBoundsObjectiveToken = Main.TokenPrefix.ToUpper() + "OBJECTIVE_CHARGE_TREASUREMAPZONE_OOB";
            holdoutZone.applyHealingNova = true;
            holdoutZone.applyFocusConvergence = true;
            holdoutZone.playerCountScaling = 0f; // Charge by 1 second regardless of how many players are charging the zone
            holdoutZone.dischargeRate = 0f;
            MysticsItemsTreasureMapZone captureZone = zonePrefab.AddComponent<MysticsItemsTreasureMapZone>();
            captureZone.itemDef = itemDef;
            captureZone.rewardSpawnCard = Resources.Load<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard/iscGoldChest");
            HologramProjector hologramProjector = zonePrefab.AddComponent<HologramProjector>();
            hologramProjector.displayDistance = holdoutZone.baseRadius;
            hologramProjector.hologramPivot = zonePrefab.transform.Find("HologramPivot");
            hologramProjector.hologramPivot.transform.localScale *= 2f;
            hologramProjector.disableHologramRotation = false;
            captureZone.hologramProjector = hologramProjector;

            On.RoR2.HoldoutZoneController.ChargeHoldoutZoneObjectiveTracker.ShouldBeFlashing += (orig, self) =>
            {
                if (self.sourceDescriptor.master)
                {
                    HoldoutZoneController holdoutZoneController = (HoldoutZoneController)self.sourceDescriptor.source;
                    if (holdoutZoneController && holdoutZoneController.gameObject.GetComponent<MysticsItemsTreasureMapZone>())
                    {
                        return false;
                    }
                }
                return orig(self);
            };

            PrefabAPI.RegisterNetworkPrefab(zonePrefab);

            zoneSpawnCard = ScriptableObject.CreateInstance<SpawnCard>();
            zoneSpawnCard.name = "isc" + Main.TokenPrefix + "TreasureMapZone";
            zoneSpawnCard.prefab = zonePrefab;
            zoneSpawnCard.directorCreditCost = 0;
            zoneSpawnCard.sendOverNetwork = true;
            zoneSpawnCard.hullSize = HullClassification.Human;
            zoneSpawnCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            zoneSpawnCard.requiredFlags = RoR2.Navigation.NodeFlags.None;
            zoneSpawnCard.forbiddenFlags = RoR2.Navigation.NodeFlags.None;
            zoneSpawnCard.occupyPosition = false;

            GenericGameEvents.OnPopulateScene += (rng) =>
            {
                DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(zoneSpawnCard, new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Random
                }, rng));
            };

            ghostMaterial = Resources.Load<Material>("Materials/matGhostEffect");

            soundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            soundEventDef.eventName = "MysticsItems_Play_env_treasuremap";
            MysticsItemsContent.Resources.networkSoundEventDefs.Add(soundEventDef);
        }

        public class MysticsItemsTreasureMapZone : MonoBehaviour, IHologramContentProvider
        {
            public TeamIndex teamIndex = TeamIndex.Player;
            public InteractableSpawnCard rewardSpawnCard;
            public GameObject reward;
            public float baseCaptureTimeMax = 1f;
            public ItemDef itemDef;
            public HologramProjector hologramProjector;
            public List<Material> rewardOriginalMaterials;
            public HoldoutZoneController holdoutZoneController;

            public void Start()
            {
                holdoutZoneController = GetComponent<HoldoutZoneController>();
                rewardOriginalMaterials = new List<Material>();

                if (NetworkServer.active)
                {
                    reward = Object.Instantiate(rewardSpawnCard.prefab, transform.position, transform.rotation);

                    RaycastHit raycastHit;
                    if (Physics.Raycast(new Ray(reward.transform.position + reward.transform.up * 1f, -reward.transform.up), out raycastHit, 2f, LayerIndex.world.mask))
                    {
                        reward.transform.up = raycastHit.normal;
                    }
                    reward.transform.Rotate(Vector3.up, RoR2Application.rng.RangeFloat(0f, 360f), Space.Self);
                    reward.transform.Translate(Vector3.down * 0.3f, Space.Self);
                    reward.transform.rotation *= Quaternion.Euler(RoR2Application.rng.RangeFloat(-30f, 30f), RoR2Application.rng.RangeFloat(-30f, 30f), RoR2Application.rng.RangeFloat(-30f, 30f));
                    NetworkServer.Spawn(reward);

                    PurchaseInteraction purchaseInteraction = reward.GetComponent<PurchaseInteraction>();
                    if (purchaseInteraction)
                    {
                        purchaseInteraction.Networkcost = Run.instance.GetDifficultyScaledCost(purchaseInteraction.cost);
                        purchaseInteraction.lockGameObject = gameObject; // lockGameObject is a SyncVar, so no need to network it
                    }
                    SetRewardLocked(true);
                }

                holdoutZoneController.onCharged = new HoldoutZoneController.HoldoutZoneControllerChargedUnityEvent();
                holdoutZoneController.onCharged.AddListener((zone) =>
                {
                    if (reward)
                    {
                        PurchaseInteraction purchaseInteraction = reward.GetComponent<PurchaseInteraction>();
                        if (purchaseInteraction)
                        {
                            purchaseInteraction.lockGameObject = null;
                        }
                        SetRewardLocked(false);
                    }

                    EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/CoinEmitter"), new EffectData
                    {
                        origin = transform.position,
                        genericFloat = 100f,
                        scale = 1f
                    }, true);
                    PointSoundManager.EmitSoundServer(soundEventDef.index, transform.position);

                    SetZoneActive(false);
                });
            }

            public void FixedUpdate()
            {
                int itemCount = 0;
                foreach (CharacterMaster characterMaster in CharacterMaster.readOnlyInstancesList)
                    if (characterMaster.teamIndex == TeamIndex.Player)
                    {
                        itemCount += characterMaster.inventory.GetItemCount(itemDef);
                    }
                holdoutZoneController.baseChargeDuration = baseCaptureTimeMax * 1f / (1f + 0.5f * (itemCount - 1));
                bool anyoneHasItem = itemCount > 0;

                hologramProjector.displayDistance = holdoutZoneController.currentRadius + 15f;
                if (!holdoutZoneController.enabled) hologramProjector.displayDistance = 0f;
                hologramProjector.hologramPivot.position = transform.position + Vector3.up * holdoutZoneController.currentRadius * 0.5f;

                if (!holdoutZoneController.wasCharged)
                {
                    if (NetworkServer.active)
                    {
                        if (!anyoneHasItem && reward.activeSelf)
                        {
                            SetRewardActive(false);
                            SetZoneActive(false);
                        }
                        if (anyoneHasItem && !reward.activeSelf)
                        {
                            SetRewardActive(true);
                            SetZoneActive(true);
                        }
                    }
                }
            }

            public bool ShouldDisplayHologram(GameObject viewer)
            {
                return !holdoutZoneController.wasCharged;
            }

            public GameObject GetHologramContentPrefab()
            {
                return PlainHologram.prefab;
            }

            public void UpdateHologramContent(GameObject hologramContentObject)
            {
                PlainHologram.MysticsItemsPlainHologramContent component = hologramContentObject.GetComponent<PlainHologram.MysticsItemsPlainHologramContent>();
                if (component)
                {
                    component.text = Mathf.FloorToInt(holdoutZoneController.charge * 100f).ToString() + "%";
                    component.color = new Color32(248, 235, 39, 255);
                }
            }

            public bool rewardUsesGhostMaterial = false;
            public void SetRewardLocked(bool value)
            {
                if (reward)
                {
                    ModelLocator modelLocator = reward.GetComponent<ModelLocator>();
                    if (modelLocator)
                    {
                        Transform modelTransform = modelLocator.modelTransform;
                        if (modelTransform)
                        {
                            int c = 0;
                            foreach (Renderer renderer in modelTransform.GetComponentsInChildren<Renderer>())
                            {
                                if (!rewardUsesGhostMaterial)
                                {
                                    if (rewardOriginalMaterials.Count > c) rewardOriginalMaterials[c] = renderer.material;
                                    else rewardOriginalMaterials.Add(renderer.material);
                                }
                                renderer.materials = new Material[] { (value ? TreasureMap.ghostMaterial : rewardOriginalMaterials[c]) };
                                c++;
                            }
                            rewardUsesGhostMaterial = value;

                            foreach (Collider collider in modelTransform.GetComponentsInChildren<Collider>())
                            {
                                if (!collider.isTrigger) collider.enabled = !value;
                            }
                        }
                    }
                }
                if (NetworkServer.active)
                {
                    new SyncRewardLocked(gameObject.GetComponent<NetworkIdentity>().netId, value).Send(NetworkDestination.Clients);
                }
            }

            public void SetRewardActive(bool value)
            {
                if (reward)
                {
                    reward.SetActive(value);
                }
                if (NetworkServer.active) CallSyncActive();
            }

            public void SetZoneActive(bool value)
            {
                if (holdoutZoneController) holdoutZoneController.enabled = value;
                if (NetworkServer.active) CallSyncActive();
            }

            public void CallSyncActive()
            {
                if (NetworkServer.active)
                {
                    new SyncActive(gameObject.GetComponent<NetworkIdentity>().netId, reward.activeSelf, holdoutZoneController.enabled).Send(NetworkDestination.Clients);
                }
            }

            public class SyncRewardLocked : INetMessage
            {
                NetworkInstanceId objID;
                bool value;

                public SyncRewardLocked()
                {
                }

                public SyncRewardLocked(NetworkInstanceId objID, bool value)
                {
                    this.objID = objID;
                    this.value = value;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                    value = reader.ReadBoolean();
                }

                public void OnReceived()
                {
                    if (NetworkServer.active) return;
                    GameObject obj = Util.FindNetworkObject(objID);
                    if (obj)
                    {
                        MysticsItemsTreasureMapZone component = obj.GetComponent<MysticsItemsTreasureMapZone>();
                        if (component)
                        {
                            component.SetRewardLocked(value);
                        }
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                    writer.Write(value);
                }
            }

            public class SyncActive : INetMessage
            {
                NetworkInstanceId objID;
                bool reward;
                bool zone;

                public SyncActive()
                {
                }

                public SyncActive(NetworkInstanceId objID, bool reward, bool zone)
                {
                    this.objID = objID;
                    this.reward = reward;
                    this.zone = zone;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                    reward = reader.ReadBoolean();
                    zone = reader.ReadBoolean();
                }

                public void OnReceived()
                {
                    if (NetworkServer.active) return;
                    GameObject obj = Util.FindNetworkObject(objID);
                    if (obj)
                    {
                        MysticsItemsTreasureMapZone component = obj.GetComponent<MysticsItemsTreasureMapZone>();
                        if (component)
                        {
                            component.SetRewardActive(reward);
                            component.SetZoneActive(zone);
                        }
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                    writer.Write(reward);
                    writer.Write(zone);
                }
            }
        }
    }
}
