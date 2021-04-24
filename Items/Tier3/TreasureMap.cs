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
using ThreeEyedGames;

namespace MysticsItems.Items
{
    public class TreasureMap : BaseItem
    {
        public static GameObject zonePrefab;
        public static NetworkIdentity zoneNetID;
        public static SpawnCard zoneSpawnCard;
        public static Material ghostMaterial;
        public static NetworkSoundEventDef soundEventDef;
        public static GameObject rewardPrefab;
        public static GameObject effectPrefab;

        public override void PreLoad()
        {
            itemDef.name = "TreasureMap";
            itemDef.tier = ItemTier.Tier3;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy
            };
        }

        public override void OnPluginAwake()
        {
            rewardPrefab = PrefabAPI.InstantiateClone(Resources.Load<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard/iscGoldChest").prefab, Main.TokenPrefix + "TreasureMapReward");
            zoneNetID = CustomUtils.GrabNetID();
        }

        public override void OnLoad()
        {
            SetAssets("Treasure Map");
            SetModelPanelDistance(3f, 6f);
            AddDisplayRule("CommandoBody", "LowerArmR", new Vector3(-0.084F, 0.183F, -0.006F), new Vector3(83.186F, 36.557F, 131.348F), new Vector3(0.053F, 0.053F, 0.053F));
            AddDisplayRule("HuntressBody", "Muzzle", new Vector3(-0.527F, -0.032F, -0.396F), new Vector3(0.509F, 134.442F, 184.268F), new Vector3(0.042F, 0.042F, 0.042F));
            AddDisplayRule("Bandit2Body", "MuzzleShotgun", new Vector3(0.014F, -0.07F, -0.668F), new Vector3(0F, 180F, 180F), new Vector3(0.04F, 0.04F, 0.04F));
            AddDisplayRule("ToolbotBody", "Head", new Vector3(0.198F, 3.655F, -0.532F), new Vector3(304.724F, 180F, 180F), new Vector3(0.448F, 0.448F, 0.448F));
            AddDisplayRule("EngiBody", "WristDisplay", new Vector3(0.01F, -0.001F, 0.007F), new Vector3(86.234F, 155.949F, 155.218F), new Vector3(0.065F, 0.065F, 0.065F));
            AddDisplayRule("MageBody", "LowerArmR", new Vector3(0.116F, 0.188F, 0.008F), new Vector3(88.872F, 20.576F, 290.58F), new Vector3(0.074F, 0.074F, 0.074F));
            AddDisplayRule("MercBody", "LowerArmR", new Vector3(-0.01F, 0.144F, -0.116F), new Vector3(277.017F, 64.808F, 295.358F), new Vector3(0.072F, 0.072F, 0.072F));
            AddDisplayRule("TreebotBody", "HeadBase", new Vector3(-0.013F, 0.253F, -0.813F), new Vector3(1.857F, 5.075F, 0.053F), new Vector3(0.13F, 0.143F, 0.294F));
            AddDisplayRule("LoaderBody", "MechLowerArmR", new Vector3(-0.01F, 0.544F, -0.144F), new Vector3(275.35F, 95.995F, 266.284F), new Vector3(0.095F, 0.095F, 0.095F));
            AddDisplayRule("CrocoBody", "UpperArmR", new Vector3(1.735F, -0.575F, 0.196F), new Vector3(281.472F, 180.072F, 89.927F), new Vector3(0.868F, 0.868F, 0.868F));
            AddDisplayRule("CaptainBody", "HandR", new Vector3(-0.066F, 0.087F, 0.011F), new Vector3(76.759F, 135.292F, 224.52F), new Vector3(0.059F, 0.053F, 0.059F));
            AddDisplayRule("BrotherBody", "HandR", BrotherInfection.red, new Vector3(0.051F, -0.072F, 0.004F), new Vector3(44.814F, 122.901F, 267.545F), new Vector3(0.063F, 0.063F, 0.063F));

            NetworkingAPI.RegisterMessageType<MysticsItemsTreasureMapZone.SyncZoneShouldBeActive>();

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
            HologramProjector hologramProjector = zonePrefab.AddComponent<HologramProjector>();
            hologramProjector.displayDistance = holdoutZone.baseRadius;
            hologramProjector.hologramPivot = zonePrefab.transform.Find("HologramPivot");
            hologramProjector.hologramPivot.transform.localScale *= 2f;
            hologramProjector.disableHologramRotation = false;
            captureZone.hologramProjector = hologramProjector;
            Decal decal = zonePrefab.transform.Find("Decal").gameObject.AddComponent<Decal>();
            decal.RenderMode = Decal.DecalRenderMode.Deferred;
            Material decalMaterial = new Material(Shader.Find("Decalicious/Deferred Decal"));
            decal.Material = decalMaterial;
            decalMaterial.name = Main.TokenPrefix + "TreasureMapDecal";
            Texture decalTexture = Main.AssetBundle.LoadAsset<Texture>("Assets/Items/Treasure Map/texTreasureMapDecal.png");
            decalMaterial.SetTexture("_MainTex", decalTexture);
            decalMaterial.SetTexture("_MaskTex", decalTexture);
            decalMaterial.SetFloat("_AngleLimit", 0f);
            decalMaterial.SetFloat("_DecalLayer", 1f);
            decalMaterial.SetFloat("_DecalBlendMode", 0f);
            decalMaterial.SetColor("_Color", new Color32(70, 10, 10, 255));
            decalMaterial.SetColor("_EmissionColor", Color.black);
            decal.Fade = 1f;
            decal.DrawAlbedo = true;
            decal.UseLightProbes = true;
            decal.DrawNormalAndGloss = false;
            decal.HighQualityBlending = false;
            decal.Reset();
            decal.gameObject.transform.localScale = Vector3.one * 10f;
            HG.ArrayUtils.ArrayAppend(ref captureZone.toggleObjects, decal.gameObject);

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

            CustomUtils.ReleaseNetID(zonePrefab, zoneNetID);
            
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

            effectPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Treasure Map/UnearthEffect.prefab");
            EffectComponent effectComponent = effectPrefab.AddComponent<EffectComponent>();
            VFXAttributes vfxAttributes = effectPrefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            effectPrefab.AddComponent<DestroyOnTimer>().duration = 1f;
            MysticsItemsContent.Resources.effectPrefabs.Add(effectPrefab);
        }

        public class MysticsItemsTreasureMapZone : MonoBehaviour, IHologramContentProvider
        {
            public TeamIndex teamIndex = TeamIndex.Player;
            public float baseCaptureTimeMax = 120f;
            public ItemDef itemDef;
            public HologramProjector hologramProjector;
            public HoldoutZoneController holdoutZoneController;
            public int cost;
            public CostTypeIndex costTypeIndex;
            public GameObject[] toggleObjects = new GameObject[] { };

            public void Start()
            {
                holdoutZoneController = GetComponent<HoldoutZoneController>();

                if (NetworkServer.active)
                {
                    PurchaseInteraction prefabPurchaseInteraction = rewardPrefab.GetComponent<PurchaseInteraction>();
                    if (prefabPurchaseInteraction) {
                        cost = Run.instance.GetDifficultyScaledCost(prefabPurchaseInteraction.cost);
                        costTypeIndex = prefabPurchaseInteraction.costType;
                        new SyncCostHologramData(gameObject.GetComponent<NetworkIdentity>().netId, cost, (int)costTypeIndex).Send(NetworkDestination.Clients);
                    }
                    ShouldBeActive = false;
                }

                holdoutZoneController.onCharged = new HoldoutZoneController.HoldoutZoneControllerChargedUnityEvent();
                holdoutZoneController.onCharged.AddListener(zone => Unearth());
            }

            public void Unearth()
            {
                EffectManager.SimpleEffect(effectPrefab, transform.position, Quaternion.identity, true);
                PointSoundManager.EmitSoundServer(soundEventDef.index, transform.position);

                GameObject reward = Object.Instantiate(rewardPrefab, transform.position, transform.rotation);

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
                    purchaseInteraction.Networkcost = cost;
                }

                Object.Destroy(gameObject);
            }

            public void FixedUpdate()
            {
                int itemCount = Util.GetItemCountForTeam(teamIndex, MysticsItemsContent.Items.TreasureMap.itemIndex, true);
                holdoutZoneController.baseChargeDuration = baseCaptureTimeMax * 1f / (1f + 0.5f * (itemCount - 1));
                bool anyoneHasItem = itemCount > 0;

                hologramProjector.displayDistance = holdoutZoneController.currentRadius + 15f;
                if (!holdoutZoneController.enabled) hologramProjector.displayDistance = 0f;
                hologramProjector.hologramPivot.position = transform.position + Vector3.up * holdoutZoneController.currentRadius * 0.5f;

                if (holdoutZoneController && holdoutZoneController.enabled != ShouldBeActive) holdoutZoneController.enabled = ShouldBeActive;

                if (!holdoutZoneController.wasCharged)
                {
                    if (NetworkServer.active)
                    {
                        if (!anyoneHasItem && ShouldBeActive) ShouldBeActive = false;
                        if (anyoneHasItem && !ShouldBeActive) ShouldBeActive = true;
                    }
                }
            }

            public bool ShouldDisplayHologram(GameObject viewer)
            {
                return !holdoutZoneController.wasCharged && ShouldBeActive;
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
                    CostTypeDef costTypeDef = CostTypeCatalog.GetCostTypeDef(costTypeIndex);
                    Color costColor = Color.white;
                    CostHologramContent.sharedStringBuilder.Clear();
                    if (costTypeDef != null)
                    {
                        costTypeDef.BuildCostStringStyled(cost, CostHologramContent.sharedStringBuilder, true, false);
                        costColor = costTypeDef.GetCostColor(true);
                    }
                    component.text = string.Format(
                        "<color=#{0}>{1}%</color>\n<color=#{3}>({2})</color>",
                        ColorUtility.ToHtmlStringRGB(new Color32(248, 235, 39, 255)),
                        Mathf.FloorToInt(holdoutZoneController.charge * 100f).ToString(),
                        CostHologramContent.sharedStringBuilder,
                        ColorUtility.ToHtmlStringRGB(costColor)
                    );
                    component.color = Color.white;
                }
            }

            public bool shouldBeActive = false;
            public bool ShouldBeActive
            {
                get
                {
                    return shouldBeActive;
                }
                set
                {
                    shouldBeActive = value;
                    foreach (GameObject toggleObject in toggleObjects)
                    {
                        toggleObject.SetActive(value);
                    }
                    if (NetworkServer.active) new SyncZoneShouldBeActive(gameObject.GetComponent<NetworkIdentity>().netId, value).Send(NetworkDestination.Clients);
                }
            }


            public class SyncCostHologramData : INetMessage
            {
                NetworkInstanceId objID;
                int cost;
                int costTypeIndex;

                public SyncCostHologramData()
                {
                }

                public SyncCostHologramData(NetworkInstanceId objID, int cost, int costTypeIndex)
                {
                    this.objID = objID;
                    this.cost = cost;
                    this.costTypeIndex = costTypeIndex;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                    cost = reader.ReadInt32();
                    costTypeIndex = reader.ReadInt32();
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
                            component.cost = cost;
                            component.costTypeIndex = (CostTypeIndex)costTypeIndex;
                        }
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                    writer.Write(cost);
                    writer.Write(costTypeIndex);
                }
            }

            public class SyncZoneShouldBeActive : INetMessage
            {
                NetworkInstanceId objID;
                bool value;

                public SyncZoneShouldBeActive()
                {
                }

                public SyncZoneShouldBeActive(NetworkInstanceId objID, bool value)
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
                            component.ShouldBeActive = value;
                        }
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                    writer.Write(value);
                }
            }
        }
    }
}
