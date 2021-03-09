using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Networking;
using R2API.Networking.Interfaces;

namespace MysticsItems.Items
{
    public class TreasureMap : BaseItem
    {
        public static GameObject zonePrefab;
        public static SpawnCard zoneSpawnCard;

        public override void PreAdd()
        {
            itemDef.name = "TreasureMap";
            itemDef.tier = ItemTier.Tier1;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist
            };
            BanFromDeployables();
            SetAssets("Treasure Map");
            SetModelPanelDistance(3f, 6f);
            AddDisplayRule((int)Main.CommonBodyIndices.Commando, "LowerArmR", new Vector3(-0.084F, 0.183F, -0.006F), new Vector3(83.186F, 36.557F, 131.348F), new Vector3(0.053F, 0.053F, 0.053F));
            AddDisplayRule("mdlHuntress", "Muzzle", new Vector3(-0.527F, -0.032F, -0.396F), new Vector3(0.509F, 134.442F, 184.268F), new Vector3(0.042F, 0.042F, 0.042F));
            AddDisplayRule("mdlToolbot", "Head", new Vector3(0.198F, 3.655F, -0.532F), new Vector3(304.724F, 180F, 180F), new Vector3(0.448F, 0.448F, 0.448F));
            AddDisplayRule("mdlEngi", "WristDisplay", new Vector3(0.01F, -0.001F, 0.007F), new Vector3(86.234F, 155.949F, 155.218F), new Vector3(0.065F, 0.065F, 0.065F));
            AddDisplayRule("mdlMage", "LowerArmR", new Vector3(0.116F, 0.188F, 0.008F), new Vector3(88.872F, 20.576F, 290.58F), new Vector3(0.074F, 0.074F, 0.074F));
            AddDisplayRule("mdlMerc", "LowerArmR", new Vector3(-0.01F, 0.144F, -0.116F), new Vector3(277.017F, 64.808F, 295.358F), new Vector3(0.072F, 0.072F, 0.072F));
            AddDisplayRule("mdlTreebot", "HeadBase", new Vector3(-0.013F, 0.253F, -0.813F), new Vector3(1.857F, 5.075F, 0.053F), new Vector3(0.13F, 0.143F, 0.294F));
            AddDisplayRule("mdlLoader", "MechLowerArmR", new Vector3(-0.01F, 0.544F, -0.144F), new Vector3(275.35F, 95.995F, 266.284F), new Vector3(0.095F, 0.095F, 0.095F));
            AddDisplayRule("mdlCroco", "UpperArmR", new Vector3(1.735F, -0.575F, 0.196F), new Vector3(281.472F, 180.072F, 89.927F), new Vector3(0.868F, 0.868F, 0.868F));
            AddDisplayRule("mdlCaptain", "HandR", new Vector3(-0.066F, 0.087F, 0.011F), new Vector3(76.759F, 135.292F, 224.52F), new Vector3(0.059F, 0.053F, 0.059F));
            AddDisplayRule("mdlBrother", "HandR", BrotherInfection.white, new Vector3(0.051F, -0.072F, 0.004F), new Vector3(44.814F, 122.901F, 267.545F), new Vector3(0.063F, 0.063F, 0.063F));
        }

        public override void OnAdd()
        {
            zonePrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Treasure Map/TreasureMapZone.prefab");
            zonePrefab.AddComponent<NetworkIdentity>();
            GoldGainZone goldGainZone = zonePrefab.AddComponent<GoldGainZone>();
            goldGainZone.teamMask.AddTeam(TeamIndex.Player);
            goldGainZone.itemIndex = itemIndex;
            goldGainZone.visuals = zonePrefab.transform.Find("Visuals").gameObject;

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
                        DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(zoneSpawnCard, new DirectorPlacementRule
                        {
                            placementMode = DirectorPlacementRule.PlacementMode.Random
                        }, xoroshiro128Plus));
                    });
                }
            };
        }

        public class GoldGainZone : MonoBehaviour
        {
            public TeamMask teamMask = TeamMask.none;
            public float rate = 1f;
            public float timer = 0f;
            public float timerMax = 1f;
            public SphereSearch sphereSearch;
            public ItemIndex itemIndex;
            public GameObject visuals;
            public static float baseRadius = 20f;
            public float fractionBank = 0f;

            public void Awake()
            {
                rate = Run.instance.difficultyCoefficient;
                sphereSearch = new SphereSearch()
                {
                    mask = LayerIndex.entityPrecise.mask,
                    origin = transform.position,
                    queryTriggerInteraction = QueryTriggerInteraction.Collide,
                    radius = baseRadius
                };
                visuals.transform.localScale = Vector3.one * baseRadius * 2f;
            }

            public void FixedUpdate()
            {
                int itemCount = 0;
                foreach (CharacterMaster characterMaster in CharacterMaster.readOnlyInstancesList)
                    if (characterMaster.teamIndex == TeamIndex.Player)
                    {
                        itemCount += characterMaster.inventory.GetItemCount(itemIndex);
                    }

                if (itemCount > 0)
                {
                    if (!visuals.activeSelf && NetworkServer.active) SetVisualsActive(true);
                    timer += Time.fixedDeltaTime;
                    if (timer >= timerMax)
                    {
                        timer = 0f;
                        if (NetworkServer.active)
                        {
                            float rateMultiplier = 2f + 1f * (float)(itemCount - 1);
                            uint total = (uint)Mathf.Floor(rate * rateMultiplier);
                            fractionBank += rate * rateMultiplier - total;
                            if (fractionBank >= 1f)
                            {
                                total += (uint)Mathf.Floor(fractionBank);
                                fractionBank -= Mathf.Floor(fractionBank);
                            }

                            foreach (HurtBox hurtBox in sphereSearch.RefreshCandidates().FilterCandidatesByHurtBoxTeam(teamMask).FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes())
                            {
                                HealthComponent healthComponent = hurtBox.healthComponent;
                                if (healthComponent)
                                {
                                    CharacterBody body = healthComponent.body;
                                    if (body)
                                    {
                                        CharacterMaster master = body.master;
                                        if (master)
                                        {
                                            master.GiveMoney(total);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (visuals.activeSelf && NetworkServer.active) SetVisualsActive(false);
                }
            }

            public void SetVisualsActive(bool active)
            {
                visuals.SetActive(active);
                if (NetworkServer.active)
                {
                    new SyncSetVisualsActive(gameObject.GetComponent<NetworkIdentity>().netId, active).Send(NetworkDestination.Clients);
                }
            }

            public class SyncSetVisualsActive : INetMessage
            {
                NetworkInstanceId objID;
                bool active;

                public SyncSetVisualsActive()
                {
                }

                public SyncSetVisualsActive(NetworkInstanceId objID, bool active)
                {
                    this.objID = objID;
                    this.active = active;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                    active = reader.ReadBoolean();
                }

                public void OnReceived()
                {
                    if (NetworkServer.active) return;
                    GameObject obj = Util.FindNetworkObject(objID);
                    if (obj)
                    {
                        GoldGainZone component = obj.GetComponent<GoldGainZone>();
                        if (component)
                        {
                            component.SetVisualsActive(active);
                        }
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                    writer.Write(active);
                }
            }
        }
    }
}
