using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Collections.Generic;
using RoR2.Audio;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Items
{
    public class TimePiece : BaseItem
    {
        public static ConfigurableValue<float> radius = new ConfigurableValue<float>(
            "Item: Time Dilator",
            "Radius",
            18f,
            "Effect range (in meters)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_TIMEPIECE_DESC"
            }
        );
        public static ConfigurableValue<float> projectileSlow = new ConfigurableValue<float>(
            "Item: Time Dilator",
            "ProjectileSlow",
            20f,
            "Projectile slowing effect (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_TIMEPIECE_DESC"
            }
        );
        public static ConfigurableValue<float> projectileSlowPerStack = new ConfigurableValue<float>(
            "Item: Time Dilator",
            "ProjectileSlowPerStack",
            20f,
            "Projectile slowing effect for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_TIMEPIECE_DESC"
            }
        );

        public static GameObject attachmentPrefab;

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_TimePiece";
            SetItemTierWhenAvailable(ItemTier.Tier2);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Hourglass/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Hourglass/Icon.png");
            var mat = itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial;
            HopooShaderToMaterial.Standard.Apply(mat);
            HopooShaderToMaterial.Standard.Emission(mat, 1.4f);

            var rotateAroundAxis = itemDef.pickupModelPrefab.transform.Find("mdlHourglass").gameObject.AddComponent<Rewired.ComponentControls.Effects.RotateAroundAxis>();
            rotateAroundAxis.relativeTo = Space.Self;
            rotateAroundAxis.rotateAroundAxis = Rewired.ComponentControls.Effects.RotateAroundAxis.RotationAxis.X;
            rotateAroundAxis.slowRotationSpeed = 40f;
            rotateAroundAxis.speed = Rewired.ComponentControls.Effects.RotateAroundAxis.Speed.Slow;

            itemDisplayPrefab = PrepareItemDisplayModel(PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Hourglass/FollowerModel.prefab")));
            itemDisplayPrefabs["spinny"] = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, "MysticsItems_TimePieceSpinnyDisplay"));

            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Pelvis", new Vector3(0.22235F, -0.08177F, -0.04821F), new Vector3(354.5442F, 166.3058F, 186.0703F), new Vector3(0.05728F, 0.08044F, 0.05728F));
                AddDisplayRule("HuntressBody", "Pelvis", new Vector3(-0.28241F, -0.11983F, -0.07156F), new Vector3(11.36557F, 276.8395F, 356.8842F), new Vector3(0.07347F, 0.06837F, 0.07347F));
                AddDisplayRule("Bandit2Body", "Pelvis", new Vector3(0.17123F, -0.00993F, -0.14942F), new Vector3(8.01832F, 13.47197F, 178.7169F), new Vector3(0.06678F, 0.06678F, 0.06678F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(1.94335F, 1.73306F, 3.82745F), new Vector3(5.94887F, 82.69791F, 5.90234F), new Vector3(0.54848F, 0.58979F, 0.54848F));
                AddDisplayRule("EngiBody", "Pelvis", new Vector3(0.24825F, -0.01657F, -0.22239F), new Vector3(3.001F, 0F, 0F), new Vector3(0.06117F, 0.06117F, 0.06117F));
                AddDisplayRule("EngiTurretBody", "Head", itemDisplayPrefabs["spinny"], new Vector3(0F, 1.56671F, 0F), new Vector3(0F, 0F, 0F), new Vector3(0.35125F, 0.35125F, 0.35125F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", itemDisplayPrefabs["spinny"], new Vector3(0F, 2.35769F, -0.55122F), new Vector3(0F, 0F, 0F), new Vector3(0.33011F, 0.33011F, 0.33011F));
                AddDisplayRule("MageBody", "Pelvis", new Vector3(0.1079F, -0.02925F, -0.18647F), new Vector3(14.08916F, 358.8586F, 355.321F), new Vector3(0.06188F, 0.06188F, 0.06188F));
                AddDisplayRule("MercBody", "Pelvis", new Vector3(-0.21011F, 0.01699F, -0.17222F), new Vector3(14.285F, 0F, 0F), new Vector3(0.0801F, 0.0801F, 0.0801F));
                AddDisplayRule("TreebotBody", "HeadBase", new Vector3(0.44547F, -0.25288F, -0.91029F), new Vector3(0F, 0F, 0F), new Vector3(0.14329F, 0.14329F, 0.14329F));
                AddDisplayRule("LoaderBody", "Pelvis", new Vector3(-0.29147F, 0.10046F, -0.05826F), new Vector3(13.13329F, 330.162F, 13.38464F), new Vector3(0.07853F, 0.07853F, 0.07853F));
                AddDisplayRule("CrocoBody", "Pelvis", new Vector3(2.82533F, 0.71367F, 0.00819F), new Vector3(338.6312F, 280.047F, 34.43082F), new Vector3(0.65748F, 0.66452F, 0.72824F));
                AddDisplayRule("CaptainBody", "Pelvis", new Vector3(0.04328F, -0.08768F, -0.23239F), new Vector3(359.5601F, 278.3878F, 336.4643F), new Vector3(0.06622F, 0.06622F, 0.06622F));
                AddDisplayRule("BrotherBody", "chest", BrotherInfection.green, new Vector3(-0.18058F, 0.21968F, 0.15686F), new Vector3(0F, 47.66693F, 0F), new Vector3(0.0706F, 0.0706F, 0.0706F));
                AddDisplayRule("ScavBody", "MuzzleEnergyCannon", new Vector3(2.68675F, -3.90626F, -13.5355F), new Vector3(90F, 0F, 0F), new Vector3(1.41172F, 1.41172F, 1.41172F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Stomach", new Vector3(0.19824F, 0.04089F, 0.18457F), new Vector3(2.2335F, 46.63262F, 357.826F), new Vector3(0.06079F, 0.06079F, 0.06079F));
                AddDisplayRule("RailgunnerBody", "Pelvis", new Vector3(0.21896F, 0.08779F, -0.06242F), new Vector3(5.45999F, 119.7464F, 17.75949F), new Vector3(0.07605F, 0.07605F, 0.07605F));
                AddDisplayRule("VoidSurvivorBody", "Pelvis", new Vector3(0.22483F, -0.09473F, -0.20079F), new Vector3(353.7015F, 1.64081F, 351.861F), new Vector3(0.06207F, 0.06207F, 0.06207F));
            };

            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;

            attachmentPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Hourglass/TimePieceAttachment.prefab");
            attachmentPrefab.AddComponent<NetworkIdentity>();
            var networkedBodyAttachment = attachmentPrefab.AddComponent<NetworkedBodyAttachment>();
            networkedBodyAttachment.shouldParentToAttachedBody = true;
            networkedBodyAttachment.forceHostAuthority = false;
            var teamFilter = attachmentPrefab.AddComponent<TeamFilter>();
            var slowDownProjectiles = attachmentPrefab.AddComponent<RoR2.Projectile.SlowDownProjectiles>();
            slowDownProjectiles.teamFilter = teamFilter;
            slowDownProjectiles.slowDownCoefficient = 0f;
            attachmentPrefab.AddComponent<MysticsItemsTimePieceAttachmentBehaviour>();
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (NetworkServer.active)
                self.AddItemBehavior<MysticsItemsTimePieceBehaviour>(self.inventory.GetItemCount(itemDef));
        }

        public class MysticsItemsTimePieceAttachmentBehaviour : MonoBehaviour
        {
            public Inventory inventory;
            public RoR2.Projectile.SlowDownProjectiles slowDownProjectiles;
            public TeamFilter teamFilter;

            public float interval = 0.5f;
            public float buffTimer;

            public class TimePieceSlowInfo
            {
                public List<MysticsItemsTimePieceAttachmentBehaviour> insideAttachments;
                public List<int> buffCounts;
            }
            public static Dictionary<CharacterBody, TimePieceSlowInfo> timePieceSlowInfos = new Dictionary<CharacterBody, TimePieceSlowInfo>();
            
            public static void CleanTimePieceSlowInfos()
            {
                timePieceSlowInfos = timePieceSlowInfos.Where(x => x.Key != null).ToDictionary(x => x.Key, y => y.Value);
            }

            public void Start()
            {
                slowDownProjectiles = GetComponent<RoR2.Projectile.SlowDownProjectiles>();
                teamFilter = GetComponent<TeamFilter>();
                var networkedBodyAttachment = GetComponent<NetworkedBodyAttachment>();
                if (networkedBodyAttachment)
                {
                    if (networkedBodyAttachment.attachedBody)
                    {
                        inventory = networkedBodyAttachment.attachedBody.inventory;
                    }
                }
            }

            public void FixedUpdate()
            {
                if (inventory)
                {
                    var itemCount = inventory.GetItemCount(MysticsItemsContent.Items.MysticsItems_TimePiece);
                    
                    transform.localScale = Vector3.one * radius;
                    if (NetworkServer.active)
                    {
                        buffTimer -= Time.fixedDeltaTime;
                        if (buffTimer <= 0f)
                        {
                            buffTimer = interval;
                            RunEffect(itemCount);
                        }
                    }
                    if (slowDownProjectiles)
                    {
                        if (itemCount > 0)
                        {
                            slowDownProjectiles.slowDownCoefficient = 1f / (1f + (projectileSlow + projectileSlowPerStack * (float)(itemCount - 1)) / 100f);
                        }
                    }
                }
            }

            public void RunEffect(int itemCount)
            {
                var radiusSqr = radius * radius;
                var position = transform.position;
                var buffDef = MysticsItemsContent.Buffs.MysticsItems_TimePieceSlow;
                for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex++)
                {
                    if (teamIndex != teamFilter.teamIndex)
                    {
                        foreach (var teamComponent in TeamComponent.GetTeamMembers(teamIndex))
                        {
                            var enemyBody = teamComponent.GetComponent<CharacterBody>();
                            if (enemyBody != null && enemyBody.isActiveAndEnabled)
                            {
                                if (!timePieceSlowInfos.ContainsKey(enemyBody))
                                {
                                    timePieceSlowInfos.Add(enemyBody, new TimePieceSlowInfo
                                    {
                                        insideAttachments = new List<MysticsItemsTimePieceAttachmentBehaviour>(),
                                        buffCounts = new List<int>()
                                    });
                                }
                                var timePieceSlowInfo = timePieceSlowInfos[enemyBody];

                                var buffCount = enemyBody.GetBuffCount(buffDef);
                                var distance = (teamComponent.transform.position - position).sqrMagnitude;
                                if (distance <= radiusSqr)
                                {
                                    if (!timePieceSlowInfo.insideAttachments.Contains(this))
                                    {
                                        timePieceSlowInfo.insideAttachments.Add(this);
                                        timePieceSlowInfo.buffCounts.Add(0);
                                    }
                                    timePieceSlowInfo.buffCounts[timePieceSlowInfo.insideAttachments.IndexOf(this)] = itemCount;

                                    var difference = timePieceSlowInfo.buffCounts.Max() - buffCount;
                                    for (var i = 0; i < difference; i++)
                                        enemyBody.AddBuff(buffDef);
                                    for (var i = 0; i < -difference; i++)
                                        enemyBody.RemoveBuff(buffDef);
                                }
                                else
                                {
                                    if (timePieceSlowInfo.insideAttachments.Contains(this))
                                    {
                                        var index = timePieceSlowInfo.insideAttachments.IndexOf(this);
                                        timePieceSlowInfo.buffCounts.RemoveAt(index);
                                        timePieceSlowInfo.insideAttachments.RemoveAt(index);
                                    }

                                    if (timePieceSlowInfo.insideAttachments.Count > 0)
                                    {
                                        var difference = timePieceSlowInfo.buffCounts.Max() - buffCount;
                                        for (var i = 0; i < difference; i++)
                                            enemyBody.AddBuff(buffDef);
                                        for (var i = 0; i < -difference; i++)
                                            enemyBody.RemoveBuff(buffDef);
                                    }
                                    else
                                    {
                                        for (var i = 0; i < buffCount; i++)
                                            enemyBody.RemoveBuff(buffDef);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public void OnDestroy()
            {
                RunEffect(0);
                CleanTimePieceSlowInfos();
            }
        }

        public class MysticsItemsTimePieceBehaviour : CharacterBody.ItemBehavior
        {
            public GameObject attachment;

            public void Start()
            {
                AddAttachmentIfDoesntExist();
            }

            public void AddAttachmentIfDoesntExist()
            {
                if (!attachment && body)
                {
                    attachment = Instantiate(attachmentPrefab, body.corePosition, Quaternion.identity);
                    attachment.GetComponent<TeamFilter>().teamIndex = body.teamComponent.teamIndex;
                    attachment.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(gameObject, null);
                }
            }

            public void OnEnable()
            {
                AddAttachmentIfDoesntExist();
            }

            public void OnDisable()
            {
                if (attachment)
                {
                    Destroy(attachment);
                    attachment = null;
                }
            }
        }
    }
}
