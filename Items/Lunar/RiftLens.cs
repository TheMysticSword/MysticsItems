using RoR2;
using RoR2.Navigation;
using R2API;
using R2API.Utils;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using RoR2.UI;
using UnityEngine.Rendering.PostProcessing;

namespace MysticsItems.Items
{
    public class RiftLens : BaseItem
    {
        public static GameObject riftChest;
        public static InteractableSpawnCard riftChestSpawnCard;
        public static CostTypeIndex riftLensDebuffCostType;

        public override void PreLoad()
        {
            itemDef.name = "RiftLens";
            itemDef.tier = ItemTier.Lunar;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy
            };
        }

        public override void OnPluginAwake()
        {
            riftChest = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/NetworkedObjects/Lockbox"), Main.TokenPrefix + "RiftChest");

            OnRiftLensCostTypeRegister += (costTypeIndex) =>
            {
                riftChest.GetComponent<PurchaseInteraction>().costType = costTypeIndex;
                riftChest.GetComponent<PurchaseInteraction>().cost = 1;
            };

            //add a custom purchase cost type - we will require the interactor pay with the debuff so that players
            //without the debuff can't help them open chests faster
            CostTypeDef costTypeDef = new CostTypeDef();
            costTypeDef.costStringFormatToken = "COST_" + Main.TokenPrefix.ToUpper() + "RIFTLENSDEBUFF_FORMAT";
            costTypeDef.isAffordable = delegate (CostTypeDef costTypeDef2, CostTypeDef.IsAffordableContext context)
            {
                CharacterBody body = context.activator.gameObject.GetComponent<CharacterBody>();
                if (body)
                {
                    Inventory inventory = body.inventory;
                    return inventory ? inventory.GetItemCount(MysticsItemsContent.Items.RiftLensDebuff) > 0 : false;
                }
                return false;
            };
            costTypeDef.payCost = delegate (CostTypeDef costTypeDef2, CostTypeDef.PayCostContext context)
            {
                CharacterBody body = context.activator.gameObject.GetComponent<CharacterBody>();
                if (body)
                {
                    Inventory inventory = body.inventory;
                    if (inventory.GetItemCount(MysticsItemsContent.Items.RiftLensDebuff) > 0) inventory.RemoveItem(MysticsItemsContent.Items.RiftLensDebuff);
                }
            };
            costTypeDef.colorIndex = ColorCatalog.ColorIndex.LunarItem;
            CostTypeCreation.CreateCostType(new CostTypeCreation.CustomCostTypeInfo
            {
                costTypeDef = costTypeDef,
                onRegister = OnRiftLensCostTypeRegister
            });

            NetworkingAPI.RegisterMessageType<RiftChest.SyncDestroyThingsOnOpen>();
        }

        public static System.Action<CostTypeIndex> OnRiftLensCostTypeRegister;

        public override void OnLoad()
        {
            base.OnLoad();
            SetAssets("Rift Lens");
            SetModelPanelDistance(2f, 6f);
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Head", new Vector3(0.1f, 0.25f, 0.15f), new Vector3(20f, 210f, 0f), new Vector3(0.06f, 0.06f, 0.06f));
                AddDisplayRule("HuntressBody", "Head", new Vector3(-0.0009F, 0.2635F, 0.1117F), new Vector3(0F, 180F, 0F), new Vector3(0.03F, 0.03F, 0.03F));
                AddDisplayRule("Bandit2Body", "Head", new Vector3(0F, 0.057F, 0.135F), new Vector3(0F, 180F, 180F), new Vector3(0.028F, 0.028F, 0.028F));
                AddDisplayRule("ToolbotBody", "Head", new Vector3(0.409F, 3.049F, -1.067F), new Vector3(60F, 0F, 180F), new Vector3(0.3F, 0.3F, 0.3F));
                AddDisplayRule("EngiBody", "HeadCenter", new Vector3(0.098F, 0.019F, 0.127F), new Vector3(1.506F, 213.327F, 354.045F), new Vector3(0.029F, 0.029F, 0.029F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.005F, 0.525F, 2.043F), new Vector3(0F, 180F, 0F), new Vector3(0.108F, 0.083F, 0.083F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.006F, 0.774F, 0.853F), new Vector3(0F, 177.859F, 0F), new Vector3(0.306F, 0.306F, 0.306F));
                AddDisplayRule("MageBody", "Head", new Vector3(0.048F, 0.06F, 0.117F), new Vector3(13.941F, 189.822F, 2.364F), new Vector3(0.026F, 0.026F, 0.026F));
                AddDisplayRule("MercBody", "Head", new Vector3(0.05F, 0.156F, 0.151F), new Vector3(10.716F, 202.078F, 355.897F), new Vector3(0.053F, 0.053F, 0.053F));
                AddDisplayRule("TreebotBody", "HeadCenter", new Vector3(-0.005F, 0.058F, -0.002F), new Vector3(85.226F, 270F, 270F), new Vector3(0.098F, 0.098F, 0.098F));
                AddDisplayRule("LoaderBody", "Head", new Vector3(0.051F, 0.125F, 0.134F), new Vector3(10.267F, 205.465F, 354.736F), new Vector3(0.047F, 0.04F, 0.048F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(-1.531F, 1.934F, 0.459F), new Vector3(14.526F, 104.513F, 346.531F), new Vector3(0.236F, 0.236F, 0.236F));
                AddDisplayRule("CaptainBody", "HandR", new Vector3(-0.085F, 0.108F, 0.013F), new Vector3(69.075F, 70.114F, 350.542F), new Vector3(0.026F, 0.03F, 0.042F));
                AddDisplayRule("BrotherBody", "Head", BrotherInfection.blue, new Vector3(0.003F, -0.01F, 0.061F), new Vector3(349.888F, 70.121F, 339.729F), new Vector3(0.133F, 0.133F, 0.133F));
                AddDisplayRule("ScavBody", "Head", new Vector3(5.068F, 4.15F, -0.55F), new Vector3(46.576F, 301.45F, 310.155F), new Vector3(1.363F, 1.363F, 1.363F));
            };

            RiftChest component = riftChest.AddComponent<RiftChest>();
            riftChest.GetComponent<SfxLocator>().openSound = "Play_env_riftchest_open";
            //replace tokens
            riftChest.GetComponent<GenericDisplayNameProvider>().displayToken = Main.TokenPrefix.ToUpper() + "RIFTCHEST_NAME";
            riftChest.GetComponent<PurchaseInteraction>().displayNameToken = Main.TokenPrefix.ToUpper() + "RIFTCHEST_NAME";
            riftChest.GetComponent<PurchaseInteraction>().contextToken = Main.TokenPrefix.ToUpper() + "RIFTCHEST_CONTEXT";
            ChestBehavior chestBehavior = riftChest.GetComponent<ChestBehavior>();
            chestBehavior.tier1Chance = 80f;
            chestBehavior.tier2Chance = 20f;
            chestBehavior.tier3Chance = 1f;
            Transform modelBase = riftChest.transform.Find("ModelBase");
            //replace lockbox model with chest model
            GameObject regularChest = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/NetworkedObjects/Chest/Chest1"), Main.TokenPrefix + "RiftChest_TempRegularChest", false);
            Object.Destroy(riftChest.transform.Find("ModelBase").Find("mdlKeyLockbox").gameObject);
            GameObject mdlChest1 = regularChest.transform.Find("mdlChest1").gameObject;
            mdlChest1.transform.SetParent(modelBase);
            mdlChest1.GetComponent<EntityLocator>().entity = riftChest;
            mdlChest1.transform.Find("Cube.001").gameObject.GetComponent<EntityLocator>().entity = riftChest;
            riftChest.GetComponent<ModelLocator>().modelBaseTransform = riftChest.transform.Find("ModelBase").transform;
            riftChest.GetComponent<ModelLocator>().modelTransform = mdlChest1.transform;
            riftChest.GetComponent<Highlight>().targetRenderer = mdlChest1.transform.Find("Cube.001").gameObject.GetComponent<SkinnedMeshRenderer>();
            //custom visuals to make it stand out:
            //new material
            Material matRiftChest = Object.Instantiate(Resources.Load<GameObject>("Prefabs/NetworkedObjects/DamageZoneWard").transform.Find("Shrinker").Find("Totem").Find("Mesh").GetComponent<MeshRenderer>().material);
            matRiftChest.SetFloat("_SplatmapOn", 0f);
            mdlChest1.transform.Find("Cube.001").GetComponent<SkinnedMeshRenderer>().material = matRiftChest;
            //light beam
            GameObject timeCrystal = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/TimeCrystalBody"), Main.TokenPrefix + "RiftChest_TempTimeCrystal", false);
            GameObject beam = timeCrystal.transform.Find("ModelBase").Find("Mesh").Find("Beam").gameObject;
            beam.transform.SetParent(modelBase);
            beam.transform.localPosition = Vector3.zero;
            beam.GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", Main.AssetBundle.LoadAsset<Texture>("Assets/Items/Rift Lens/texRampRiftChest.png"));
            component.destroyOnOpen.Add(beam);
            //point light
            GameObject pointLight = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/NetworkedObjects/DamageZoneWard").transform.Find("Shrinker").Find("Totem").Find("Point Light").gameObject, "Point Light", false);
            pointLight.transform.SetParent(modelBase);
            pointLight.transform.localPosition = Vector3.zero;
            component.destroyOnOpen.Add(pointLight);
            //custom vfx
            GameObject customVFX = Object.Instantiate(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Rift Lens Debuff/RiftLensChestVFX.prefab"), modelBase, false);
            component.destroyOnOpen.Add(customVFX);
            //post processing
            GameObject ppHolder = Object.Instantiate(PrefabAPI.InstantiateClone(new GameObject("RiftLensPostProcessing"), "RiftLensPostProcessing", false), modelBase);
            ppHolder.layer = LayerIndex.postProcess.intVal;
            PostProcessVolume pp = ppHolder.AddComponent<PostProcessVolume>();
            pp.isGlobal = false;
            pp.weight = 1f;
            pp.priority = 100;
            pp.blendDistance = 10f;
            SphereCollider sphereCollider = ppHolder.AddComponent<SphereCollider>();
            sphereCollider.radius = 5f;
            sphereCollider.isTrigger = true;
            PostProcessProfile ppProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
            ppProfile.name = "ppRiftLens";
            LensDistortion lensDistortion = ppProfile.AddSettings<LensDistortion>();
            lensDistortion.SetAllOverridesTo(true);
            lensDistortion.intensity.value = -50f;
            lensDistortion.scale.value = 1f;
            ColorGrading colorGrading = ppProfile.AddSettings<ColorGrading>();
            colorGrading.colorFilter.value = new Color32(178, 242, 255, 255);
            colorGrading.colorFilter.overrideState = true;
            pp.sharedProfile = ppProfile;
            PostProcessDuration ppDuration = pp.gameObject.AddComponent<PostProcessDuration>();
            ppDuration.ppVolume = pp;
            ppDuration.ppWeightCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 1f, 0f, Mathf.Tan(-45f * Mathf.Deg2Rad)),
                    new Keyframe(1f, 0f, Mathf.Tan(135f * Mathf.Deg2Rad), 0f)
                },
                preWrapMode = WrapMode.Clamp,
                postWrapMode = WrapMode.Clamp
            };
            ppDuration.maxDuration = 1;
            ppDuration.destroyOnEnd = true;
            component.ppDuration = ppDuration;

            riftChestSpawnCard = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            riftChestSpawnCard.name = "isc" + Main.TokenPrefix + "RiftChest";
            riftChestSpawnCard.directorCreditCost = 0;
            riftChestSpawnCard.forbiddenFlags = NodeFlags.NoChestSpawn;
            riftChestSpawnCard.hullSize = HullClassification.Human;
            riftChestSpawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
            riftChestSpawnCard.occupyPosition = true;
            riftChestSpawnCard.orientToFloor = true;
            riftChestSpawnCard.sendOverNetwork = true;
            riftChestSpawnCard.prefab = riftChest;

            GenericGameEvents.OnPopulateScene += (rng) =>
            {
                int itemCount = 0;
                foreach (CharacterMaster characterMaster in CharacterMaster.readOnlyInstancesList)
                    if (characterMaster.teamIndex == TeamIndex.Player)
                    {
                        int thisItemCount = characterMaster.inventory.GetItemCount(itemDef);
                        if (thisItemCount > 0)
                        {
                            characterMaster.inventory.RemoveItem(MysticsItemsContent.Items.RiftLensDebuff, characterMaster.inventory.GetItemCount(MysticsItemsContent.Items.RiftLensDebuff));
                            characterMaster.inventory.GiveItem(MysticsItemsContent.Items.RiftLensDebuff, thisItemCount);
                            itemCount += thisItemCount;
                        }
                    }
                if (itemCount > 0)
                {
                    for (int i = 0; i < itemCount; i++)
                    {
                        GameObject riftChest = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(riftChestSpawnCard, new DirectorPlacementRule
                        {
                            placementMode = DirectorPlacementRule.PlacementMode.Random
                        }, rng));
                    }
                }
            };
        }

        public class RiftChest : MonoBehaviour
        {
            public bool open = true;
            public List<GameObject> destroyOnOpen = new List<GameObject>();
            public PostProcessDuration ppDuration;

            public void Start()
            {
                PurchaseInteraction purchaseInteraction = GetComponent<PurchaseInteraction>();
                purchaseInteraction.onPurchase = new PurchaseEvent();
                purchaseInteraction.onPurchase.AddListener((interactor) =>
                {
                    purchaseInteraction.SetAvailable(false);
                    GetComponent<ChestBehavior>().Open();
                    open = false;
                    unopenedCount--;
                    DestroyThingsOnOpen();
                });
            }

            public void DestroyThingsOnOpen()
            {
                if (ppDuration) ppDuration.enabled = true;
                foreach (GameObject gameObject in destroyOnOpen)
                {
                    Object.Destroy(gameObject);
                }
                if (NetworkServer.active)
                {
                    new SyncDestroyThingsOnOpen(gameObject.GetComponent<NetworkIdentity>().netId).Send(NetworkDestination.Clients);
                }
            }

            public class SyncDestroyThingsOnOpen : INetMessage
            {
                NetworkInstanceId objID;

                public SyncDestroyThingsOnOpen()
                {
                }

                public SyncDestroyThingsOnOpen(NetworkInstanceId objID)
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
                        RiftChest component = obj.GetComponent<RiftChest>();
                        if (component)
                        {
                            component.DestroyThingsOnOpen();
                        }
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                }
            }

            public void OnEnable()
            {
                if (open) unopenedCount++;
            }

            public void OnDisable()
            {
                if (open) unopenedCount--;
            }

            public static int unopenedCount = 0;
        }
    }
}
