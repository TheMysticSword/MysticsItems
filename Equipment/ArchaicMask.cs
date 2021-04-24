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

namespace MysticsItems.Equipment
{
    public class ArchaicMask : BaseEquipment
    {
        public static GameObject crosshairPrefab;
        public static GameObject unlockInteractablePrefab;
        public static GameObject forcedPickupPrefab;

        public override void PreLoad()
        {
            equipmentDef.name = "ArchaicMask";
            equipmentDef.cooldown = 140f;
            equipmentDef.canDrop = true;
            equipmentDef.enigmaCompatible = true;
            SetUnlockable();
        }

        public override void OnPluginAwake()
        {
            unlockInteractablePrefab = CustomUtils.CreateBlankPrefab(Main.TokenPrefix + "ArchaicMaskUnlockInteractable", true);
            forcedPickupPrefab = CustomUtils.CreateBlankPrefab(Main.TokenPrefix + "ArchaicMaskForcedPickup", true);
        }

        public override void OnLoad()
        {
            SetAssets("Archaic Mask");
            SetScalableChildEffect("Mask/Effects/Point Light");
            SetScalableChildEffect("Mask/Effects/Fire");
            CopyModelToFollower();
            CustomUtils.CopyChildren(PrefabAPI.InstantiateClone(model, Main.TokenPrefix + "ArchaicMaskUnlockInteractable", false), unlockInteractablePrefab);
            
            AddDisplayRule("CommandoBody", "Head", new Vector3(-0.001F, 0.253F, 0.124F), new Vector3(0.055F, 269.933F, 20.48F), new Vector3(0.147F, 0.147F, 0.147F));
            AddDisplayRule("HuntressBody", "Head", new Vector3(-0.001F, 0.221F, 0.039F), new Vector3(0.073F, 269.903F, 25.101F), new Vector3(0.119F, 0.119F, 0.121F));
            AddDisplayRule("Bandit2Body", "Head", new Vector3(0F, 0.047F, 0.102F), new Vector3(0F, 270F, 0F), new Vector3(0.097F, 0.097F, 0.097F));
            AddDisplayRule("ToolbotBody", "Head", new Vector3(0.321F, 3.4F, -0.667F), new Vector3(0F, 90F, 56.608F), new Vector3(0.933F, 0.933F, 0.933F));
            AddDisplayRule("EngiBody", "Head", new Vector3(0F, 0F, 0.117F), new Vector3(0F, 270F, 0F), new Vector3(0.12F, 0.12F, 0.12F));
            AddDisplayRule("MageBody", "Head", new Vector3(0F, 0.05F, 0.104F), new Vector3(0F, 270F, 0F), new Vector3(0.07F, 0.07F, 0.07F));
            AddDisplayRule("MercBody", "Head", new Vector3(0F, 0.135F, 0.117F), new Vector3(0F, 270F, 11.977F), new Vector3(0.123F, 0.123F, 0.123F));
            AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(0.2F, 0.873F, 0.344F), new Vector3(0F, 295.888F, 344.74F), new Vector3(0.283F, 0.283F, 0.283F));
            AddDisplayRule("LoaderBody", "Head", new Vector3(0F, 0.097F, 0.116F), new Vector3(0F, 270F, 4.685F), new Vector3(0.117F, 0.117F, 0.117F));
            AddDisplayRule("CrocoBody", "Head", new Vector3(0.012F, 4.425F, 1.271F), new Vector3(355.778F, 271.534F, 2.51F), new Vector3(0.973F, 0.973F, 0.973F));
            AddDisplayRule("CaptainBody", "Head", new Vector3(0F, 0.064F, 0.12F), new Vector3(0F, 270F, 0F), new Vector3(0.104F, 0.107F, 0.118F));
            AddDisplayRule("ScavBody", "Backpack", new Vector3(5.161F, 3.722F, -0.213F), new Vector3(342.055F, 261.468F, 115.963F), new Vector3(1.363F, 1.363F, 1.363F));
            AddDisplayRule("EquipmentDroneBody", "HeadCenter", new Vector3(0F, -0.507F, -0.381F), new Vector3(0F, 270F, 270F), new Vector3(0.965F, 0.965F, 0.965F));

            crosshairPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/WoodSpriteIndicator"), Main.TokenPrefix + "ArchaicMaskIndicator", false);
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
            unlockInteractablePrefab.AddComponent<GenericDisplayNameProvider>().displayToken = "EQUIPMENT_" + Main.TokenPrefix.ToUpper() + equipmentDef.name.ToUpper() + "_NAME";

            Highlight highlight = unlockInteractablePrefab.AddComponent<Highlight>();
            highlight.targetRenderer = unlockInteractablePrefab.GetComponentInChildren<Renderer>();
            highlight.highlightColor = Highlight.HighlightColor.interactive;

            GameObject entityLocatorHolder = unlockInteractablePrefab.transform.Find("EntityLocatorHolder").gameObject;
            entityLocatorHolder.layer = LayerIndex.pickups.intVal;
            SphereCollider sphereCollider = entityLocatorHolder.AddComponent<SphereCollider>();
            sphereCollider.radius = 1.5f;
            sphereCollider.isTrigger = true;
            entityLocatorHolder.AddComponent<EntityLocator>().entity = unlockInteractablePrefab;

            CustomUtils.CopyChildren(PrefabAPI.InstantiateClone(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Archaic Mask/ForcedPickup.prefab"), "ForcedPickup"), forcedPickupPrefab);
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
            CurrentTarget targetInfo = equipmentSlot.GetComponent<CurrentTarget>();
            if (targetInfo && targetInfo.obj)
            {
                CharacterMaster master = equipmentSlot.characterBody.master;
                if (master)
                {
                    ArchaicMaskSummonLimit summonLimit = master.GetComponent<ArchaicMaskSummonLimit>();
                    if (!summonLimit) summonLimit = master.gameObject.AddComponent<ArchaicMaskSummonLimit>();
                    HurtBox targetHB = targetInfo.obj.GetComponent<CharacterBody>().mainHurtBox;
                    if (targetHB)
                    {
                        DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest((SpawnCard)Resources.Load("SpawnCards/CharacterSpawnCards/cscArchWisp"), new DirectorPlacementRule
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
                            wispMaster.inventory.GiveItem(RoR2Content.Items.HealthDecay, 45);
                            wispMaster.inventory.GiveItem(RoR2Content.Items.BoostDamage, 20);
                            wispMaster.inventory.GiveItem(RoR2Content.Items.BoostHp, 10);
                            wispMaster.inventory.GiveItem(RoR2Content.Items.AlienHead, 10);
                            wispMaster.GetComponent<RoR2.CharacterAI.BaseAI>().currentEnemy.gameObject = targetHB.healthComponent.gameObject;
                            wispMaster.GetComponent<RoR2.CharacterAI.BaseAI>().currentEnemy.bestHurtBox = targetHB;
                            summonLimit.Add(wispMasterObject);
                        };
                        DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
                        
                        targetInfo.Invalidate();
                        return true;
                    }
                }
            }
            return false;
        }

        public class MysticsItemsArchaicMaskUnlockInteraction : NetworkBehaviour, IInteractable
        {
            public string contextString = Main.TokenPrefix.ToUpper() + "ARCHAICMASK_CONTEXT";
            public int lockTime = 600;
            public int minFadeTime = 590;
            public GameObject effects;
            public float initialLightIntensity = 0f;
            public Color initialColor;
            public GenericPickupController genericPickupController;

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
                    inventory.SetEquipmentIndex(MysticsItemsContent.Equipment.ArchaicMask.equipmentIndex);
                    typeof(GenericPickupController).InvokeMethod("SendPickupMessage", inventory.GetComponent<CharacterMaster>(), PickupCatalog.FindPickupIndex(MysticsItemsContent.Equipment.ArchaicMask.equipmentIndex));

                    GameObject forcedPickup = Object.Instantiate(forcedPickupPrefab, transform.position, transform.rotation);
                    forcedPickup.GetComponent<MysticsItemsArchaicMaskForcedPickup>().pickupIndex = PickupCatalog.FindPickupIndex(currentEquipmentIndex);
                    NetworkServer.Spawn(forcedPickup);
                    
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
                    ParticleSystem.MainModule main = effects.GetComponentInChildren<ParticleSystem>().main;
                    initialColor = main.startColor.color;
                }
                genericPickupController = GetComponent<GenericPickupController>();
            }

            public void FixedUpdate()
            {
                if (effects)
                {
                    float t = Mathf.Clamp01((float)remainingTime / (float)(lockTime - minFadeTime));
                    effects.GetComponentInChildren<Light>().intensity = initialLightIntensity * t;
                    ParticleSystem.MainModule main = effects.GetComponentInChildren<ParticleSystem>().main;
                    main.startColor = Color.Lerp(Color.black, initialColor, t);
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
            public int max = 3;
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
