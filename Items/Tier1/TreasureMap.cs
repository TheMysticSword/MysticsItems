using RoR2;
using RoR2.Hologram;
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

            NetworkingAPI.RegisterMessageType<MysticsItemsTreasureMapZone.SyncCaptured>();
        }

        public override void OnAdd()
        {
            zonePrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Treasure Map/TreasureMapZone.prefab");
            zonePrefab.AddComponent<NetworkIdentity>();
            MysticsItemsTreasureMapZone captureZone = zonePrefab.AddComponent<MysticsItemsTreasureMapZone>();
            captureZone.itemIndex = itemIndex;
            captureZone.visuals = zonePrefab.transform.Find("Visuals").gameObject;
            captureZone.visuals.transform.Find("Point Light").gameObject.AddComponent<MysticsItemsScaleLight>();
            HologramProjector hologramProjector = zonePrefab.AddComponent<HologramProjector>();
            hologramProjector.displayDistance = captureZone.baseRadius;
            hologramProjector.hologramPivot = zonePrefab.transform.Find("HologramPivot");
            hologramProjector.hologramPivot.transform.localScale *= 2f;
            hologramProjector.disableHologramRotation = false;
            captureZone.hologramProjector = hologramProjector;

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

        public class MysticsItemsTreasureMapZone : NetworkBehaviour, IHologramContentProvider
        {
            public TeamIndex teamIndex = TeamIndex.Player;
            public float baseReward = 100f;
            public float captureTime = 0f;
            public float baseCaptureTimeMax = 60f;
            public float captureTimeMax = 60f;
            public float CaptureProgress
            {
                get
                {
                    return captureTime / captureTimeMax;
                }
                set
                {
                    captureTime = captureTimeMax * value;
                }
            }
            public bool _captured = false;
            public bool captured {
                get
                {
                    return _captured;
                }
                set
                {
                    if (NetworkServer.active)
                    {
                        _captured = value;
                        new SyncCaptured(gameObject.GetComponent<NetworkIdentity>().netId, _captured).Send(NetworkDestination.Clients);
                    }
                }
            }
            public ItemIndex itemIndex;
            public GameObject visuals;
            public float baseRadius = 15f;
            public float radius = 0f;
            public float radiusVelocity = 0f;
            public HologramProjector hologramProjector;

            public void FixedUpdate()
            {
                int itemCount = 0;
                foreach (CharacterMaster characterMaster in CharacterMaster.readOnlyInstancesList)
                    if (characterMaster.teamIndex == TeamIndex.Player)
                    {
                        itemCount += characterMaster.inventory.GetItemCount(itemIndex);
                    }
                captureTimeMax = baseCaptureTimeMax * 1f / (1f + 0.1f * (itemCount - 1));
                bool anyoneHasItem = itemCount > 0;

                float targetRadius = baseRadius;
                if (captured || !anyoneHasItem) targetRadius = 0f;
                radius = Mathf.SmoothDamp(radius, targetRadius, ref radiusVelocity, 1f, float.PositiveInfinity, Time.fixedDeltaTime);
                if (radius <= 0.01f) radius = 0f;

                visuals.transform.localScale = Vector3.one * radius * 2f;
                hologramProjector.displayDistance = radius + 15f;
                hologramProjector.hologramPivot.position = transform.position + Vector3.up * radius * 0.5f;

                if (!captured)
                {
                    if (anyoneHasItem)
                    {
                        if (NetworkServer.active)
                        {
                            bool anyoneCapturing = false;
                            ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamIndex);
                            for (int i = 0; i < teamMembers.Count; i++)
                            {
                                TeamComponent teamComponent = teamMembers[i];
                                if (teamComponent.body.isPlayerControlled && (teamComponent.body.corePosition - transform.position).sqrMagnitude <= (radius * radius))
                                {
                                    anyoneCapturing = true;
                                }
                            }
                            if (anyoneCapturing)
                            {
                                captureTime += Time.fixedDeltaTime;
                            }
                        }
                    }

                    if (CaptureProgress >= 1f && !captured && NetworkServer.active)
                    {
                        captured = true;

                        if (NetworkServer.active)
                        {
                            uint goldReward = (uint)(baseReward * Run.instance.difficultyCoefficient);
                            TeamManager.instance.GiveTeamMoney(teamIndex, goldReward);
                            EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/CoinEmitter"), new EffectData
                            {
                                origin = transform.position,
                                genericFloat = goldReward,
                                scale = 1f
                            }, true);
                            Util.PlaySound("MysticsItems_Play_env_treasuremap", gameObject);
                        }
                    }
                }
            }

            public bool ShouldDisplayHologram(GameObject viewer)
            {
                return !captured;
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
                    component.text = Mathf.FloorToInt(CaptureProgress * 100f).ToString() + "%";
                    component.color = new Color32(248, 235, 39, 255);
                }
            }


            public class SyncCaptured : INetMessage
            {
                NetworkInstanceId objID;
                bool captured;

                public SyncCaptured()
                {
                }

                public SyncCaptured(NetworkInstanceId objID, bool captured)
                {
                    this.objID = objID;
                    this.captured = captured;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                    captured = reader.ReadBoolean();
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
                            component.captured = captured;
                        }
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                    writer.Write(captured);
                }
            }
        }
    }
}
