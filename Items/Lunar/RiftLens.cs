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

namespace MysticsItems.Items
{
    public class RiftLens : BaseItem
    {
        public static GameObject riftChest;
        public static InteractableSpawnCard riftChestSpawnCard;
        public static CostTypeIndex riftLensDebuffCostType;

        public override void PreAdd()
        {
            itemDef.name = "RiftLens";
            itemDef.tier = ItemTier.Lunar;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist
            };
            BanFromDeployables();
            SetAssets("Rift Lens");
            SetModelPanelDistance(2f, 6f);
            AddDisplayRule((int)Main.CommonBodyIndices.Commando, "Head", new Vector3(0.1f, 0.25f, 0.15f), new Vector3(20f, 210f, 0f), new Vector3(0.06f, 0.06f, 0.06f));
            AddDisplayRule("mdlHuntress", "Head", new Vector3(-0.0009F, 0.2635F, 0.1117F), new Vector3(0F, 180F, 0F), new Vector3(0.03F, 0.03F, 0.03F));
            AddDisplayRule("mdlToolbot", "Head", new Vector3(0.409F, 3.049F, -1.067F), new Vector3(60F, 0F, 180F), new Vector3(0.3F, 0.3F, 0.3F));
            AddDisplayRule("mdlEngi", "HeadCenter", new Vector3(0.098F, 0.019F, 0.127F), new Vector3(1.506F, 213.327F, 354.045F), new Vector3(0.029F, 0.029F, 0.029F));
            AddDisplayRule((int)Main.CommonBodyIndices.EngiTurret, "Head", new Vector3(0.005F, 0.525F, 2.043F), new Vector3(0F, 180F, 0F), new Vector3(0.108F, 0.083F, 0.083F));
            AddDisplayRule((int)Main.CommonBodyIndices.EngiWalkerTurret, "Head", new Vector3(0.006F, 0.774F, 0.853F), new Vector3(0F, 177.859F, 0F), new Vector3(0.306F, 0.306F, 0.306F));
            AddDisplayRule("mdlMage", "Head", new Vector3(0.048F, 0.06F, 0.117F), new Vector3(13.941F, 189.822F, 2.364F), new Vector3(0.026F, 0.026F, 0.026F));
            AddDisplayRule("mdlMerc", "Head", new Vector3(0.05F, 0.156F, 0.151F), new Vector3(10.716F, 202.078F, 355.897F), new Vector3(0.053F, 0.053F, 0.053F));
            AddDisplayRule("mdlTreebot", "HeadCenter", new Vector3(-0.005F, 0.058F, -0.002F), new Vector3(85.226F, 270F, 270F), new Vector3(0.098F, 0.098F, 0.098F));
            AddDisplayRule("mdlLoader", "Head", new Vector3(0.051F, 0.125F, 0.134F), new Vector3(10.267F, 205.465F, 354.736F), new Vector3(0.047F, 0.04F, 0.048F));
            AddDisplayRule("mdlCroco", "Head", new Vector3(-1.531F, 1.934F, 0.459F), new Vector3(14.526F, 104.513F, 346.531F), new Vector3(0.236F, 0.236F, 0.236F));
            AddDisplayRule("mdlCaptain", "HandR", new Vector3(-0.085F, 0.108F, 0.013F), new Vector3(69.075F, 70.114F, 350.542F), new Vector3(0.026F, 0.03F, 0.042F));
            AddDisplayRule("mdlBrother", "Head", BrotherInfection.blue, new Vector3(0.003F, -0.01F, 0.061F), new Vector3(349.888F, 70.121F, 339.729F), new Vector3(0.133F, 0.133F, 0.133F));
            AddDisplayRule("mdlScav", "Head", new Vector3(5.068F, 4.15F, -0.55F), new Vector3(46.576F, 301.45F, 310.155F), new Vector3(1.363F, 1.363F, 1.363F));

            riftChest = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/NetworkedObjects/Lockbox"), Main.TokenPrefix + "RiftChest");
            RiftChest component = riftChest.AddComponent<RiftChest>();
            riftChest.GetComponent<SfxLocator>().openSound = "Play_env_riftchest_open";
            //replace tokens
            riftChest.GetComponent<GenericDisplayNameProvider>().displayToken = Main.TokenPrefix.ToUpper() + "RIFTCHEST_NAME";
            riftChest.GetComponent<PurchaseInteraction>().displayNameToken = Main.TokenPrefix.ToUpper() + "RIFTCHEST_NAME";
            riftChest.GetComponent<PurchaseInteraction>().contextToken = Main.TokenPrefix.ToUpper() + "RIFTCHEST_CONTEXT";
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
            mdlChest1.transform.Find("Cube.001").GetComponent<SkinnedMeshRenderer>().material = Object.Instantiate(Resources.Load<GameObject>("Prefabs/NetworkedObjects/DamageZoneWard").transform.Find("Shrinker").Find("Totem").Find("Mesh").GetComponent<MeshRenderer>().material);
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

            //add a custom purchase cost type - we will require the interactor pay with the debuff so that players
            //without the debuff can't help them open chests faster
            CostTypeDef costTypeDef = new CostTypeDef();
            costTypeDef.costStringFormatToken = "COST_" + Main.TokenPrefix.ToUpper() + "RIFTLENSDEBUFF_FORMAT";
            costTypeDef.isAffordable = delegate (CostTypeDef costTypeDef2, CostTypeDef.IsAffordableContext context)
            {
                CharacterBody body = context.activator.gameObject.GetComponent<CharacterBody>();
                if (body)
                {
                    return body.HasBuff(Buffs.BaseBuff.GetFromType(typeof(Buffs.RiftLens)));
                }
                return false;
            };
            costTypeDef.payCost = delegate (CostTypeDef costTypeDef2, CostTypeDef.PayCostContext context)
            {
                foreach (CharacterBody body in CharacterBody.readOnlyInstancesList)
                {
                    if (body.HasBuff(Buffs.BaseBuff.GetFromType(typeof(Buffs.RiftLens)))) body.RemoveBuff(Buffs.BaseBuff.GetFromType(typeof(Buffs.RiftLens)));
                }
            };
            costTypeDef.colorIndex = ColorCatalog.ColorIndex.LunarItem;
            CostTypeCatalog.modHelper.getAdditionalEntries += (list) =>
            {
                list.Add(costTypeDef);
            };
            On.RoR2.CostTypeCatalog.Init += (orig) =>
            {
                orig();
                riftLensDebuffCostType = (CostTypeIndex)System.Array.IndexOf(typeof(CostTypeCatalog).GetFieldValue<CostTypeDef[]>("costTypeDefs"), costTypeDef);
                riftChest.GetComponent<PurchaseInteraction>().costType = riftLensDebuffCostType;
                On.RoR2.CostTypeCatalog.GetCostTypeDef += (orig2, costTypeIndex) =>
                {
                    if (costTypeIndex == riftLensDebuffCostType) return costTypeDef;
                    return orig2(costTypeIndex);
                };
            };
            riftChest.GetComponent<PurchaseInteraction>().cost = 1;
        }

        public override void OnAdd()
        {
            IL.RoR2.SceneDirector.PopulateScene += (il) =>
            {
                ILCursor c = new ILCursor(il);

                ILLabel after = null;
                ILLabel ourCheck = null;

                if (c.TryGotoNext(
                    x => x.MatchLdcI4(0),
                    x => x.MatchStloc(12)
                ) && c.TryGotoNext(
                    x => x.MatchLdloc(12),
                    x => x.MatchLdcI4(0),
                    x => x.MatchBle(out after)
                ))
                {
                    c.GotoLabel(after, MoveType.Before);
                    ourCheck = c.MarkLabel();
                    c.GotoPrev(
                        MoveType.After,
                        x => x.MatchLdloc(12),
                        x => x.MatchLdcI4(0),
                        x => x.MatchBle(out after)
                    );
                    c.Prev.Operand = ourCheck;
                    c.GotoLabel(ourCheck);
                    c.Emit(OpCodes.Ldloc, 11);
                    c.EmitDelegate<System.Action<Xoroshiro128Plus>>((xoroshiro128Plus) =>
                    {
                        int itemCount = 0;
                        foreach (CharacterMaster characterMaster in CharacterMaster.readOnlyInstancesList)
                            if (characterMaster.teamIndex == TeamIndex.Player)
                            {
                                itemCount += characterMaster.inventory.GetItemCount(itemIndex);
                            }
                        if (itemCount > 0)
                        {
                            for (int i = 0; i < itemCount; i++)
                            {
                                GameObject riftChest = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(riftChestSpawnCard, new DirectorPlacementRule
                                {
                                    placementMode = DirectorPlacementRule.PlacementMode.Random
                                }, xoroshiro128Plus));
                            }
                        }
                    });
                }
            };

            On.RoR2.CharacterBody.Start += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && TeamComponent.GetObjectTeam(self.gameObject) == TeamIndex.Player)
                {
                    Inventory inventory = self.inventory;
                    if (inventory && inventory.GetItemCount(itemIndex) > 0)
                    {
                        while (self.GetBuffCount(Buffs.BaseBuff.GetFromType(typeof(Buffs.RiftLens))) < RiftChest.unopenedCount)
                        {
                            self.AddBuff(Buffs.BaseBuff.GetFromType(typeof(Buffs.RiftLens)));
                        }
                    }
                }
            };

            NetworkingAPI.RegisterMessageType<RiftChest.SyncDestroyThingsOnOpen>();
        }

        public class RiftChest : MonoBehaviour
        {
            public bool open = true;
            public List<GameObject> destroyOnOpen = new List<GameObject>();

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
