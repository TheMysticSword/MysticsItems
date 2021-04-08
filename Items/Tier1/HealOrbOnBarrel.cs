using RoR2;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Items
{
    public class HealOrbOnBarrel : BaseItem
    {
        public override void PreLoad()
        {
            itemDef.name = "HealOrbOnBarrel";
            itemDef.tier = ItemTier.Tier1;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Healing,
                ItemTag.Utility,
                ItemTag.AIBlacklist
            };
        }

        public override void OnLoad()
        {
            SetAssets("Donut");
            foreach (Transform childTransform in model.transform.Find("Torus.001"))
            {
                GameObject child = childTransform.gameObject;
                Renderer renderer = child.GetComponentInChildren<Renderer>();
                Color.RGBToHSV(renderer.material.color, out float h, out float s, out float v);
                h += Random.value;
                renderer.material.color = Color.HSVToRGB(h % 1, s, v);
            }
            CopyModelToFollower();
            AddDisplayRule("CommandoBody", "Head", new Vector3(0f, 0.35f, 0f), new Vector3(0f, 180f, 0f), new Vector3(0.15f, 0.15f, 0.15f));
            AddDisplayRule("HuntressBody", "Head", new Vector3(0F, 0.302F, -0.049F), new Vector3(0F, 180F, 0F), new Vector3(0.12F, 0.12F, 0.12F));
            AddDisplayRule("Bandit2Body", "Hat", new Vector3(0F, 0.055F, -0.016F), new Vector3(336.039F, 0F, 0F), new Vector3(0.209F, 0.209F, 0.209F));
            AddDisplayRule("ToolbotBody", "Head", new Vector3(0.053F, 2.57F, 1.265F), new Vector3(55.266F, 359.983F, 0.119F), new Vector3(1.5F, 1.5F, 1.5F));
            AddDisplayRule("EngiBody", "HeadCenter", new Vector3(0F, 0.131F, -0.014F), new Vector3(356.315F, 0.001F, 359.976F), new Vector3(0.175F, 0.175F, 0.175F));
            AddDisplayRule("EngiTurretBody", "Head", new Vector3(0F, 0.548F, 0F), new Vector3(0F, 180F, 0F), new Vector3(1.447F, 1.447F, 1.447F));
            AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0F, 1.13F, -1.52F), new Vector3(351.692F, 180F, 0F), new Vector3(0.684F, 0.399F, 0.478F));
            AddDisplayRule("MageBody", "Head", new Vector3(0F, 0.112F, -0.121F), new Vector3(66.476F, 180F, 0F), new Vector3(0.149F, 0.149F, 0.149F));
            AddDisplayRule("MercBody", "Chest", new Vector3(0.013F, 0.184F, -0.259F), new Vector3(71.925F, 180F, 0F), new Vector3(0.15F, 0.15F, 0.15F));
            AddDisplayRule("TreebotBody", "HandL", new Vector3(0.055F, 0.638F, 0.354F), new Vector3(12.464F, 0.568F, 10.583F), new Vector3(0.315F, 0.315F, 0.315F));
            AddDisplayRule("LoaderBody", "Head", new Vector3(0F, 0.231F, 0F), new Vector3(0F, 180F, 0F), new Vector3(0.15F, 0.15F, 0.15F));
            AddDisplayRule("CrocoBody", "Head", new Vector3(-0.946F, 3.963F, -0.229F), new Vector3(279.836F, 0F, 170.118F), new Vector3(1.602F, 1.602F, 1.602F));
            AddDisplayRule("CaptainBody", "Stomach", new Vector3(0.002F, 0.134F, 0.176F), new Vector3(313.466F, 271.294F, 278.969F), new Vector3(0.086F, 0.086F, 0.086F));
            AddDisplayRule("BrotherBody", "Head", BrotherInfection.white, new Vector3(-0.01F, 0.044F, 0.12F), new Vector3(65.585F, 339.303F, 255.053F), new Vector3(0.107F, 0.107F, 0.107F));

            On.EntityStates.Barrel.Opening.OnEnter += (orig, self) =>
            {
                orig(self);
                GameObject gameObject = (GameObject)typeof(EntityStates.EntityState).GetProperty("gameObject", Main.bindingFlagAll).GetValue(self);
                if (gameObject)
                {
                    SpawnOrbOnBarrelOpen spawnOrbOnBarrelOpen = gameObject.GetComponent<SpawnOrbOnBarrelOpen>();
                    if (spawnOrbOnBarrelOpen && spawnOrbOnBarrelOpen.interactor)
                    {
                        CharacterBody interactorBody = spawnOrbOnBarrelOpen.interactor.GetComponent<CharacterBody>();
                        if (interactorBody)
                        {
                            SpawnOrb(gameObject.transform.position, gameObject.transform.rotation, interactorBody.teamComponent.teamIndex, interactorBody.inventory.GetItemCount(MysticsItemsContent.Items.HealOrbOnBarrel));
                        }
                        if (NetworkServer.active) GameObject.Destroy(spawnOrbOnBarrelOpen);
                    }
                }
            };
            IL.RoR2.GlobalEventManager.OnInteractionBegin += (il) =>
            {
                ILCursor c = new ILCursor(il);

                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdloc(4),
                    x => x.MatchLdarg(3),
                    x => x.MatchCallOrCallvirt<GameObject>("GetComponent"),
                    x => x.MatchStfld(out _)
                ))
                {
                    c.Emit(OpCodes.Ldloc_0);
                    c.Emit(OpCodes.Ldloc_3);
                    c.Emit(OpCodes.Ldarg_1);
                    c.Emit(OpCodes.Ldarg_2);
                    c.Emit(OpCodes.Ldarg_3);
                    c.EmitDelegate<System.Action<CharacterBody, Inventory, Interactor, IInteractable, GameObject>>((interactorBody, inventory, interactor, interactable, interactableObject) =>
                    {
                        if (inventory.GetItemCount(MysticsItemsContent.Items.HealOrbOnBarrel) > 0)
                        {
                            GenericDisplayNameProvider genericDisplayNameProvider = interactableObject.GetComponent<GenericDisplayNameProvider>();
                            if (genericDisplayNameProvider && genericDisplayNameProvider.displayToken.Contains("BARREL"))
                            {
                                SpawnOrbOnBarrelOpen spawnOrbOnBarrelOpen = interactableObject.AddComponent<SpawnOrbOnBarrelOpen>();
                                spawnOrbOnBarrelOpen.interactor = interactor;
                            }
                        }
                    });
                }
            };

            On.RoR2.GravitatePickup.FixedUpdate += (orig, self) =>
            {
                var ror1style = self.GetComponent<GravitatePickupRoR1Style>();
                if (ror1style && !ror1style.normalBehaviour)
                {
                    var positionDifference = Vector3.Distance(ror1style.targetPosition, self.transform.position);
                    if (positionDifference > ror1style.lastPositionDifference || ror1style.moveTime >= ror1style.moveTimeMax)
                    {
                        ror1style.moveTime = ror1style.moveTimeMax;
                        self.rigidbody.velocity = Vector3.MoveTowards(self.rigidbody.velocity, Vector3.zero, self.acceleration * 0.25f);
                        self.rigidbody.transform.localScale = Vector3.Lerp(self.rigidbody.transform.localScale, ror1style.targetScale, ror1style.scaleDifference.magnitude / ror1style.floatTimeMax * Time.fixedDeltaTime);
                        if (ror1style.floatTime < ror1style.floatTimeMax)
                        {
                            ror1style.floatTime += Time.fixedDeltaTime;
                            if (ror1style.floatTime >= ror1style.floatTimeMax)
                            {
                                ror1style.floatTime = ror1style.floatTimeMax;
                                ror1style.normalBehaviour = true;
                                self.rigidbody.useGravity = true;
                            }
                        }
                    }
                    else
                    {
                        self.rigidbody.velocity = Vector3.MoveTowards(self.rigidbody.velocity, (ror1style.targetPosition - self.transform.position).normalized * self.maxSpeed, self.acceleration);
                        ror1style.moveTime += Time.fixedDeltaTime;
                    }
                    ror1style.lastPositionDifference = positionDifference;
                }
                else
                {
                    orig(self);
                }
            };

            GenericGameEvents.OnPopulateScene += (rng) =>
            {
                int itemCount = 0;
                foreach (CharacterMaster characterMaster in CharacterMaster.readOnlyInstancesList)
                    if (characterMaster.teamIndex == TeamIndex.Player)
                    {
                        itemCount += characterMaster.inventory.GetItemCount(itemDef);
                    }
                if (itemCount > 0)
                {
                    for (int i = 0; i < itemCount; i++)
                    {
                        DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(Resources.Load<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard/iscBarrel1"), new DirectorPlacementRule
                        {
                            placementMode = DirectorPlacementRule.PlacementMode.Random
                        }, rng));
                    }
                }
            };
        }

        public static void SpawnOrb(Vector3 position, Quaternion rotation, TeamIndex teamIndex, int itemCount)
        {
            if (NetworkServer.active)
            {
                var orb = Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/HealPack"), position, Random.rotation);
                orb.GetComponent<TeamFilter>().teamIndex = teamIndex;
                orb.GetComponentInChildren<HealthPickup>().flatHealing = 8;
                orb.GetComponentInChildren<HealthPickup>().fractionalHealing = 0.1f + 0.1f * (itemCount - 1);
                var ror1style = orb.GetComponentInChildren<GravitatePickup>().gameObject.AddComponent<GravitatePickupRoR1Style>();
                ror1style.targetPosition = position + rotation * Vector3.up * 4f;
                ror1style.targetScale = orb.transform.localScale + (orb.transform.localScale * orb.GetComponentInChildren<HealthPickup>().fractionalHealing);
                ror1style.scaleDifference = ror1style.targetScale - orb.transform.localScale;
                orb.GetComponent<Rigidbody>().useGravity = false;
                NetworkServer.Spawn(orb);
            }
        }

        public class GravitatePickupRoR1Style : MonoBehaviour
        {
            public Vector3 targetPosition;
            public Vector3 targetScale;
            public Vector3 scaleDifference;
            public float lastPositionDifference = Mathf.Infinity;
            public float moveTime = 0f;
            public float moveTimeMax = 1f;
            public float floatTime = 0f;
            public float floatTimeMax = 1f;
            public bool normalBehaviour = false;
        }

        public class SpawnOrbOnBarrelOpen : MonoBehaviour
        {
            public Interactor interactor;
        }
    }
}
