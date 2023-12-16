using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;
using System.Collections.Generic;
using R2API.Networking.Interfaces;
using R2API.Networking;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Equipment
{
    public class ArchaicMask : BaseEquipment
    {
        public static CharacterSpawnCard ArchWispSpawnCard = LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscArchWisp");
        public static GameObject crosshairPrefab;
        public static GameObject unlockInteractablePrefab;
        public static GameObject forcedPickupPrefab;

        public static ConfigurableValue<float> duration = new ConfigurableValue<float>(
            "Equipment: Legendary Mask",
            "Duration",
            45f,
            "Life time of the Wisp (in seconds)",
            new List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_ARCHAICMASK_PICKUP",
                "EQUIPMENT_MYSTICSITEMS_ARCHAICMASK_DESC"
            }
        );
        public static ConfigurableValue<float> wispDamage = new ConfigurableValue<float>(
            "Equipment: Legendary Mask",
            "WispDamage",
            300f,
            "Damage multiplier of the Wisp (in %)",
            new List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_ARCHAICMASK_DESC"
            }
        );
        public static ConfigurableValue<float> wispHealth = new ConfigurableValue<float>(
            "Equipment: Legendary Mask",
            "WispHealth",
            200f,
            "Health multiplier of the Wisp (in %)",
            new List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_ARCHAICMASK_DESC"
            }
        );
        public static ConfigurableValue<float> wispCDR = new ConfigurableValue<float>(
            "Equipment: Legendary Mask",
            "WispCDR",
            50f,
            "Skill cooldown reduction multiplier of the Wisp (in %)",
            new List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_ARCHAICMASK_DESC"
            }
        );
        public static ConfigurableValue<float> wispAttackSpeed = new ConfigurableValue<float>(
            "Equipment: Legendary Mask",
            "WispAttackSpeed",
            150f,
            "Attack speed multiplier of the Wisp (in %)",
            new List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_ARCHAICMASK_DESC"
            }
        );
        public static ConfigurableValue<int> summonLimit = new ConfigurableValue<int>(
            "Equipment: Legendary Mask",
            "SummonLimit",
            3,
            "How many Wisps can be active at once"
        );

        public override void OnPluginAwake()
        {
            unlockInteractablePrefab = MysticsRisky2Utils.Utils.CreateBlankPrefab("MysticsItems_ArchaicMaskUnlockInteractable", true);
            forcedPickupPrefab = MysticsRisky2Utils.Utils.CreateBlankPrefab("MysticsItems_ArchaicMaskForcedPickup", true);
        }

        public override void OnLoad()
        {
            equipmentDef.name = "MysticsItems_ArchaicMask";
            ConfigManager.Balance.CreateEquipmentCooldownOption(equipmentDef, "Equipment: Legendary Mask", 140f);
            equipmentDef.canDrop = true;
            ConfigManager.Balance.CreateEquipmentEnigmaCompatibleOption(equipmentDef, "Equipment: Legendary Mask", true);
            ConfigManager.Balance.CreateEquipmentCanBeRandomlyTriggeredOption(equipmentDef, "Equipment: Legendary Mask", true);
            equipmentDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Archaic Mask/Model.prefab"));
            equipmentDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Equipment/Archaic Mask/Icon.png");
            MysticsItemsContent.Resources.unlockableDefs.Add(GetUnlockableDef());

            SetScalableChildEffect(equipmentDef.pickupModelPrefab, "Mask/Effects/Point Light");
            SetScalableChildEffect(equipmentDef.pickupModelPrefab, "Mask/Effects/Fire");
            Material matArchaicMaskFire = equipmentDef.pickupModelPrefab.transform.Find("Mask/Effects/Fire").gameObject.GetComponent<Renderer>().sharedMaterial;
            HopooShaderToMaterial.CloudRemap.Apply(
                matArchaicMaskFire,
                Main.AssetBundle.LoadAsset<Texture>("Assets/Equipment/Archaic Mask/texRampArchaicMaskFire.png")
            );
            HopooShaderToMaterial.CloudRemap.Boost(matArchaicMaskFire, 0.5f);
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(equipmentDef.pickupModelPrefab, equipmentDef.pickupModelPrefab.name + "Display", false));
            MysticsRisky2Utils.Utils.CopyChildren(equipmentDef.pickupModelPrefab, unlockInteractablePrefab);

            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Head", new Vector3(-0.001F, 0.253F, 0.124F), new Vector3(0.055F, 269.933F, 20.48F), new Vector3(0.147F, 0.147F, 0.147F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(-0.001F, 0.221F, 0.039F), new Vector3(0.073F, 269.903F, 25.101F), new Vector3(0.119F, 0.119F, 0.121F));
                AddDisplayRule("Bandit2Body", "Head", new Vector3(0F, 0.047F, 0.102F), new Vector3(0F, 270F, 0F), new Vector3(0.097F, 0.097F, 0.097F));
                AddDisplayRule("ToolbotBody", "Head", new Vector3(0.321F, 3.4F, -0.667F), new Vector3(0F, 90F, 56.608F), new Vector3(0.933F, 0.933F, 0.933F));
                AddDisplayRule("EngiBody", "HeadCenter", new Vector3(0F, 0F, 0.117F), new Vector3(0F, 270F, 0F), new Vector3(0.12F, 0.12F, 0.12F));
                AddDisplayRule("MageBody", "Head", new Vector3(0F, 0.05F, 0.104F), new Vector3(0F, 270F, 0F), new Vector3(0.07F, 0.07F, 0.07F));
                AddDisplayRule("MercBody", "Head", new Vector3(0F, 0.135F, 0.117F), new Vector3(0F, 270F, 11.977F), new Vector3(0.123F, 0.123F, 0.123F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(0.2F, 0.873F, 0.344F), new Vector3(0F, 295.888F, 344.74F), new Vector3(0.283F, 0.283F, 0.283F));
                AddDisplayRule("LoaderBody", "Head", new Vector3(0F, 0.097F, 0.116F), new Vector3(0F, 270F, 4.685F), new Vector3(0.117F, 0.117F, 0.117F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(0.012F, 4.425F, 1.271F), new Vector3(355.778F, 271.534F, 2.51F), new Vector3(0.973F, 0.973F, 0.973F));
                AddDisplayRule("CaptainBody", "Head", new Vector3(0F, 0.064F, 0.12F), new Vector3(0F, 270F, 0F), new Vector3(0.104F, 0.107F, 0.118F));
                AddDisplayRule("ScavBody", "MuzzleEnergyCannon", new Vector3(-0.26641F, -0.48169F, 0.74377F), new Vector3(9.57747F, 91.16208F, 178.8642F), new Vector3(2.78505F, 2.78505F, 2.78505F));
                AddDisplayRule("EquipmentDroneBody", "HeadCenter", new Vector3(0F, -0.507F, -0.381F), new Vector3(0F, 270F, 270F), new Vector3(0.965F, 0.965F, 0.965F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Head", new Vector3(-0.0039F, 0.15803F, -0.07916F), new Vector3(356.8718F, 270F, 87.82172F), new Vector3(0.14049F, 0.14049F, 0.14049F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysDeputy) AddDisplayRule("DeputyBody", "Hat", new Vector3(-0.00039F, -0.06261F, 0.09188F), new Vector3(0F, 270F, 20.90335F), new Vector3(0.09649F, 0.09649F, 0.09649F));
                AddDisplayRule("RailgunnerBody", "Head", new Vector3(0F, 0.04792F, 0.09549F), new Vector3(0F, 270F, 352.3218F), new Vector3(0.10505F, 0.10505F, 0.10505F));
                AddDisplayRule("VoidSurvivorBody", "Head", new Vector3(0.01936F, 0.09013F, 0.11825F), new Vector3(2.71926F, 280.5363F, 29.17954F), new Vector3(0.15526F, 0.15526F, 0.15526F));
            };

            crosshairPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/WoodSpriteIndicator"), "MysticsItems_ArchaicMaskIndicator", false);
            Object.Destroy(crosshairPrefab.GetComponentInChildren<Rewired.ComponentControls.Effects.RotateAroundAxis>());
            crosshairPrefab.GetComponentInChildren<SpriteRenderer>().sprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Equipment/Archaic Mask/Crosshair.png");
            crosshairPrefab.GetComponentInChildren<SpriteRenderer>().color = new Color(190f / 255f, 65f / 255f, 255f / 255f);
            crosshairPrefab.GetComponentInChildren<SpriteRenderer>().transform.rotation = Quaternion.identity;
            crosshairPrefab.GetComponentInChildren<TMPro.TextMeshPro>().color = new Color(190f / 255f, 65f / 255f, 255f / 255f);
            while (crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.length > 0) crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.RemoveKey(0);
            crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(0f, 2f);
            crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(0.5f, 1f);
            crosshairPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(1f, 1f);

            MysticsItemsArchaicMaskUnlockInteraction unlockInteraction = unlockInteractablePrefab.AddComponent<MysticsItemsArchaicMaskUnlockInteraction>();
            unlockInteraction.effects = unlockInteractablePrefab.transform.Find("Mask/Effects").gameObject;
            
            ParticleSystem particleSystem = unlockInteraction.effects.GetComponentInChildren<ParticleSystem>();
            ParticleSystem.MainModule particleSystemMain = particleSystem.main;
            particleSystemMain.startLifetime = 3.5f;

            unlockInteractablePrefab.AddComponent<GenericDisplayNameProvider>().displayToken = "EQUIPMENT_" + equipmentDef.name.ToUpper(System.Globalization.CultureInfo.InvariantCulture) + "_NAME";

            Highlight highlight = unlockInteractablePrefab.AddComponent<Highlight>();
            highlight.targetRenderer = unlockInteractablePrefab.GetComponentInChildren<Renderer>();
            highlight.highlightColor = Highlight.HighlightColor.interactive;

            GameObject entityLocatorHolder = unlockInteractablePrefab.transform.Find("EntityLocatorHolder").gameObject;
            entityLocatorHolder.layer = LayerIndex.pickups.intVal;
            SphereCollider sphereCollider = entityLocatorHolder.AddComponent<SphereCollider>();
            sphereCollider.radius = 1.5f;
            sphereCollider.isTrigger = true;
            entityLocatorHolder.AddComponent<EntityLocator>().entity = unlockInteractablePrefab;

            MysticsRisky2Utils.Utils.CopyChildren(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Archaic Mask/ForcedPickup.prefab"), forcedPickupPrefab);
            GenericPickupController genericPickupController = forcedPickupPrefab.AddComponent<GenericPickupController>();
            forcedPickupPrefab.AddComponent<Highlight>();
            PickupDisplay pickupDisplay = forcedPickupPrefab.transform.Find("PickupDisplay").gameObject.AddComponent<PickupDisplay>();
            pickupDisplay.spinSpeed = 0f;
            pickupDisplay.verticalWave = new Wave { amplitude = 0f };
            pickupDisplay.coloredParticleSystems = new ParticleSystem[] { };
            genericPickupController.pickupDisplay = pickupDisplay;
            forcedPickupPrefab.transform.Find("PickupTrigger").gameObject.layer = LayerIndex.pickups.intVal;
            forcedPickupPrefab.transform.Find("PickupTrigger").gameObject.AddComponent<EntityLocator>().entity = forcedPickupPrefab;
            forcedPickupPrefab.AddComponent<MysticsItemsArchaicMaskForcedPickup>();

            On.RoR2.SceneDirector.PopulateScene += (orig, self) =>
            {
                orig(self);
                if (SceneCatalog.GetSceneDefForCurrentScene().baseSceneName == "wispgraveyard")
                {
                    Vector3 position = Vector3.zero;
                    Vector3 rotation = Vector3.zero;
                    int variant = Random.Range(0, 2);
                    switch (variant)
                    {
                        case 0:
                            position = new Vector3(78.64633f, 42.3367f, 35.37148f);
                            rotation = new Vector3(4.26f, 150f, 20f);
                            break;
                        case 1:
                            position = new Vector3(-350.1f, 7.494953f, -72.9473f);
                            rotation = new Vector3(0f, 270f, 30f);
                            break;
                    }
                    GameObject obj = Object.Instantiate(unlockInteractablePrefab, position, Quaternion.Euler(rotation));
                    NetworkServer.Spawn(obj);
                }
            };

            UseTargetFinder(TargetFinderType.Enemies, crosshairPrefab);
        }

        public override bool OnUse(EquipmentSlot equipmentSlot)
        {
            CharacterMaster master = equipmentSlot.characterBody.master;
            if (master)
            {
                ArchaicMaskSummonLimit summonLimit = master.GetComponent<ArchaicMaskSummonLimit>();
                if (!summonLimit) summonLimit = master.gameObject.AddComponent<ArchaicMaskSummonLimit>();

                MysticsRisky2UtilsEquipmentTarget targetInfo = equipmentSlot.GetComponent<MysticsRisky2UtilsEquipmentTarget>();
                if (targetInfo && targetInfo.obj)
                {
                    HurtBox targetHB = targetInfo.obj.GetComponent<CharacterBody>().mainHurtBox;
                    if (targetHB)
                    {
                        DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest((SpawnCard)ArchWispSpawnCard, new DirectorPlacementRule
                        {
                            placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                            spawnOnTarget = targetHB.transform
                        }, RoR2Application.rng)
                        {
                            summonerBodyObject = equipmentSlot.characterBody.gameObject
                        };
                        directorSpawnRequest.onSpawnedServer += (spawnResult) =>
                        {
                            GameObject wispMasterObject = spawnResult.spawnedInstance;
                            CharacterMaster wispMaster = wispMasterObject.GetComponent<CharacterMaster>();
                            wispMaster.inventory.GiveItem(RoR2Content.Items.HealthDecay, (int)duration.Value);
                            wispMaster.inventory.GiveItem(RoR2Content.Items.BoostDamage, (int)(wispDamage.Value - 100f) / 10);
                            wispMaster.inventory.GiveItem(RoR2Content.Items.BoostHp, (int)(wispHealth.Value - 100f) / 10);
                            wispMaster.inventory.GiveItem(RoR2Content.Items.AlienHead, (int)(wispCDR.Value - 100f) / 10);
                            wispMaster.inventory.GiveItem(RoR2Content.Items.BoostAttackSpeed, (int)(wispAttackSpeed.Value - 100f) / 10);
                            wispMaster.GetComponent<RoR2.CharacterAI.BaseAI>().currentEnemy.gameObject = targetHB.healthComponent.gameObject;
                            wispMaster.GetComponent<RoR2.CharacterAI.BaseAI>().currentEnemy.bestHurtBox = targetHB;
                            AttemptAddRiskyModAllyItems(wispMaster.inventory);
                            summonLimit.Add(wispMasterObject);
                        };
                        DirectorCore.instance.TrySpawnObject(directorSpawnRequest);

                        targetInfo.Invalidate();
                        return true;
                    }
                }

                {
                    DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest((SpawnCard)LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscArchWisp"), new DirectorPlacementRule
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                        position = equipmentSlot.GetAimRay().origin
                    }, RoR2Application.rng)
                    {
                        summonerBodyObject = equipmentSlot.characterBody.gameObject
                    };
                    directorSpawnRequest.onSpawnedServer += (spawnResult) =>
                    {
                        GameObject wispMasterObject = spawnResult.spawnedInstance;
                        CharacterMaster wispMaster = wispMasterObject.GetComponent<CharacterMaster>();
                        wispMaster.inventory.GiveItem(RoR2Content.Items.HealthDecay, (int)duration.Value);
                        wispMaster.inventory.GiveItem(RoR2Content.Items.BoostDamage, (int)(wispDamage.Value - 100f) / 10);
                        wispMaster.inventory.GiveItem(RoR2Content.Items.BoostHp, (int)(wispHealth.Value - 100f) / 10);
                        wispMaster.inventory.GiveItem(RoR2Content.Items.AlienHead, (int)(wispCDR.Value) / 10);
                        wispMaster.inventory.GiveItem(RoR2Content.Items.BoostAttackSpeed, (int)(wispAttackSpeed.Value - 100f) / 10);
                        AttemptAddRiskyModAllyItems(wispMaster.inventory);
                        summonLimit.Add(wispMasterObject);
                    };
                    DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
                    return true;
                }
            }
            return false;
        }

        //Adds RiskyMod ally items if they exist. Can be run without having to actually set a softdependency.
        //The items only run their code if the holder is an NPC on the player team, KKA is not active. and RiskyMod ally changes are enabled in the config.
        private static void AttemptAddRiskyModAllyItems(Inventory inventory)
        {
            //These 2 are standard, and have a lot of stuff tied to them.
            ItemIndex riskyModAllyMarker = ItemCatalog.FindItemIndex("RiskyModAllyMarkerItem");
            if (riskyModAllyMarker != ItemIndex.None)
            {
                inventory.GiveItem(riskyModAllyMarker);
            }
            ItemIndex riskyModAllyScaling = ItemCatalog.FindItemIndex("RiskyModAllyScalingItem");
            if (riskyModAllyScaling != ItemIndex.None)
            {
                inventory.GiveItem(riskyModAllyScaling);
            }
            
            //These 2 allow the ally to die to Void Implosions and Grandparent Suns.
            //Usually expendable/replaceable allies (ex. Beetle Guards) can die to them, while stuff you have to buy (like Drones) don't.
            ItemIndex riskyModAllyAllowVoidDeath = ItemCatalog.FindItemIndex("RiskyModAllyAllowVoidDeathItem");
            if (riskyModAllyScaling != ItemIndex.None)
            {
                inventory.GiveItem(riskyModAllyAllowVoidDeath);
            }
            ItemIndex riskyModAllyAllowOverheatDeath = ItemCatalog.FindItemIndex("RiskyModAllyAllowOverheatDeathItem");
            if (riskyModAllyScaling != ItemIndex.None)
            {
                inventory.GiveItem(riskyModAllyAllowOverheatDeath);
            }
        }

        public class MysticsItemsArchaicMaskUnlockInteraction : NetworkBehaviour, IInteractable
        {
            public string contextString = "MYSTICSITEMS_ARCHAICMASK_CONTEXT";
            public int lockTime = 600;
            public int minFadeTime = 590;
            public GameObject effects;
            public float initialLightIntensity = 0f;
            public Color initialColor;
            public GenericPickupController genericPickupController;

            public MaterialPropertyBlock materialPropertyBlock;

            public string GetContextString(Interactor activator)
            {
                return Language.GetString(contextString);
            }

            public Interactability GetInteractability(Interactor activator)
            {
                if (!locked)
                {
                    return Interactability.Available;
                }
                return Interactability.ConditionsNotMet;
            }

            public void OnInteractionBegin(Interactor activator)
            {
                Inventory inventory = activator.GetComponent<CharacterBody>().inventory;
                if (inventory)
                {
                    if (OnUnlock != null) OnUnlock(activator);

                    EquipmentIndex currentEquipmentIndex = inventory.currentEquipmentIndex;
                    inventory.SetEquipmentIndex(MysticsItemsContent.Equipment.MysticsItems_ArchaicMask.equipmentIndex);
                    typeof(GenericPickupController).InvokeMethod("SendPickupMessage", inventory.GetComponent<CharacterMaster>(), PickupCatalog.FindPickupIndex(MysticsItemsContent.Equipment.MysticsItems_ArchaicMask.equipmentIndex));

                    GameObject forcedPickup = Object.Instantiate(forcedPickupPrefab, transform.position, transform.rotation);
                    forcedPickup.GetComponent<MysticsItemsArchaicMaskForcedPickup>().pickupIndex = PickupCatalog.FindPickupIndex(currentEquipmentIndex);
                    NetworkServer.Spawn(forcedPickup);

                    if (NetworkServer.active) NetworkServer.UnSpawn(gameObject);
                    Object.Destroy(gameObject);
                }
            }

            public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
            {
                return false;
            }

            public bool ShouldShowOnScanner()
            {
                return true;
            }

            public void Awake()
            {
                if (effects)
                {
                    initialLightIntensity = effects.GetComponentInChildren<Light>().intensity;
                    ParticleSystemRenderer particleSystemRenderer = effects.GetComponentInChildren<ParticleSystemRenderer>();
                    initialColor = particleSystemRenderer.material.GetColor("_TintColor");
                }
                genericPickupController = GetComponent<GenericPickupController>();
                materialPropertyBlock = new MaterialPropertyBlock();
            }

            public void FixedUpdate()
            {
                if (effects)
                {
                    float t = Mathf.Clamp01((float)remainingTime / (float)(lockTime - minFadeTime));
                    effects.GetComponentInChildren<Light>().intensity = initialLightIntensity * t;
                    ParticleSystem.MainModule main = effects.GetComponentInChildren<ParticleSystem>().main;
                    ParticleSystemRenderer particleSystemRenderer = effects.GetComponentInChildren<ParticleSystemRenderer>();
                    particleSystemRenderer.GetPropertyBlock(materialPropertyBlock);
                    materialPropertyBlock.SetColor("_TintColor", Color.Lerp(Color.black, initialColor, t));
                    particleSystemRenderer.SetPropertyBlock(materialPropertyBlock);
                }
            }

            public int remainingTime
            {
                get
                {
                    float num = 0f;
                    if (Run.instance)
                    {
                        num = Run.instance.GetRunStopwatch();
                    }
                    return (int)(lockTime - num);
                }
            }

            public bool locked
            {
                get
                {
                    return remainingTime <= 0;
                }
            }

            public static event System.Action<Interactor> OnUnlock;
        }

        public class ArchaicMaskSummonLimit : MonoBehaviour
        {
            public int max = summonLimit.Value;
            public List<GameObject> current;

            public void Awake()
            {
                current = new List<GameObject>();
            }

            public void Add(GameObject newWisp)
            {
                current.Add(newWisp);
                while (current.Count > max)
                {
                    GameObject first = current[0];
                    if (first)
                    {
                        CharacterMaster characterMaster = first.GetComponent<CharacterMaster>();
                        if (characterMaster.hasBody)
                        {
                            HealthComponent healthComponent = characterMaster.GetBody().healthComponent;
                            if (healthComponent.alive) healthComponent.Suicide();
                        }
                    }
                    current.RemoveAt(0);
                }
            }
        }

        public class MysticsItemsArchaicMaskForcedPickup : MonoBehaviour
        {
            public PickupIndex pickupIndex = PickupIndex.none;

            public void Start()
            {
                if (NetworkServer.active)
                {
                    GetComponent<GenericPickupController>().NetworkpickupIndex = pickupIndex;
                }
            }
        }
    }
}
