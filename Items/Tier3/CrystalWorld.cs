using RoR2;
using RoR2.UI;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;
using System.Linq;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Items
{
    public class CrystalWorld : BaseItem
    {
        public struct WorldInfo
        {
            public GameObject prefab;
            public GameObject currentObject;
            public CrystalWorldCamera cameraComponent;
            public List<GameObject> shownInContainers;
            public RenderTexture renderTexture;
        }

        public static List<WorldInfo> worlds = new List<WorldInfo>();
        public static GameObject pulsePrefab;
        public static GameObject ballPrefab;

        public static ConfigurableValue<float> freezeTime = new ConfigurableValue<float>(
            "Item: Crystallized World",
            "FreezeTime",
            5f,
            "Freeze duration (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_CRYSTALWORLD_DESC"
            }
        );
        public static ConfigurableValue<int> pulses = new ConfigurableValue<int>(
            "Item: Crystallized World",
            "Pulses",
            2,
            "Total freeze pulses per Teleporter event",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_CRYSTALWORLD_DESC"
            }
        );
        public static ConfigurableValue<int> pulsesPerStack = new ConfigurableValue<int>(
            "Item: Crystallized World",
            "PulsesPerStack",
            1,
            "Extra freeze pulses per Teleporter event for each additional stack of this item",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_CRYSTALWORLD_DESC"
            }
        );
        public static ConfigurableValue<float> minPulseRadius = new ConfigurableValue<float>(
            "Item: Crystallized World",
            "MinPulseRadius",
            60f,
            "The minimum radius of the freezing pulse. If the holdout zone is smaller than this (for example, if it's a Commencement Pillar), the pulse will not become smaller, and will retain this value"
        );

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_CrystalWorld";
            SetItemTierWhenAvailable(ItemTier.Tier3);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Crystal World/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Crystal World/Icon.png");
            itemDef.pickupModelPrefab.AddComponent<CrystalWorldContainer>();
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Stomach", new Vector3(-0.17426F, 0.07766F, -0.05266F), new Vector3(16.68701F, 66.665F, 36.228F), new Vector3(0.042F, 0.042F, 0.042F));
                AddDisplayRule("HuntressBody", "Muzzle", new Vector3(-0.1516F, -0.0345F, -0.09869F), new Vector3(355.162F, 32.177F, 180.96F), new Vector3(0.042F, 0.042F, 0.042F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(0.19815F, 0.04837F, 0.02337F), new Vector3(350.191F, 244.703F, 340.178F), new Vector3(0.037F, 0.037F, 0.037F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(3.83001F, 1.891F, 0.03063F), new Vector3(29.795F, 9.384F, 2.716F), new Vector3(0.489F, 0.489F, 0.489F));
                AddDisplayRule("EngiBody", "Chest", new Vector3(-0.20791F, 0.30973F, 0.18735F), new Vector3(4.991F, 46.464F, 181.437F), new Vector3(0.065F, 0.065F, 0.065F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0F, 0.56045F, 0F), new Vector3(33.04002F, 48.09F, 359.072F), new Vector3(0.168F, 0.168F, 0.168F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0F, 0.78124F, 0.82794F), new Vector3(22.677F, 152.024F, 24.393F), new Vector3(0.134F, 0.163F, 0.131F));
                AddDisplayRule("MageBody", "Chest", new Vector3(0.06498F, 0.25775F, 0.30228F), new Vector3(0.366F, 347.899F, 165.881F), new Vector3(0.0827F, 0.0827F, 0.0827F));
                AddDisplayRule("MercBody", "HandR", new Vector3(0.00372F, 0.11846F, 0.0863F), new Vector3(0F, 0F, 0F), new Vector3(0.07229F, 0.07229F, 0.07217F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(-0.45015F, 0.77395F, -0.18053F), new Vector3(0F, 0F, 0F), new Vector3(0.29135F, 0.29135F, 0.29135F));
                AddDisplayRule("LoaderBody", "MechUpperArmL", new Vector3(0.07707F, 0.07703F, -0.00595F), new Vector3(7.628F, 218.893F, 342.184F), new Vector3(0.10413F, 0.10413F, 0.10413F));
                AddDisplayRule("CrocoBody", "SpineChest2", new Vector3(-1.42325F, 2.10075F, 1.05927F), new Vector3(337.83F, 226.76F, 273.311F), new Vector3(0.411F, 0.411F, 0.411F));
                AddDisplayRule("CaptainBody", "MuzzleGun", new Vector3(-0.0034F, 0.03444F, -0.31976F), new Vector3(0F, 0F, 0F), new Vector3(0.04057F, 0.03674F, 0.04057F));
                AddDisplayRule("BrotherBody", "UpperArmL", new Vector3(0.02255F, -0.01451F, -0.00259F), new Vector3(303.36F, 82.77999F, 101.5723F), new Vector3(0.05297F, 0.08504F, 0.08504F));
                AddDisplayRule("ScavBody", "UpperArmL", new Vector3(0.32551F, 0.61566F, 1.17648F), new Vector3(0F, 0F, 0F), new Vector3(2.65712F, 2.73031F, 2.65712F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Chest", new Vector3(0.1151F, 0.32048F, -0.33018F), new Vector3(10.681F, 0.007F, 0.071F), new Vector3(0.07588F, 0.07588F, 0.07588F));
                AddDisplayRule("RailgunnerBody", "Backpack", new Vector3(0.09864F, 0.17726F, -0.08467F), new Vector3(0F, 0F, 0F), new Vector3(0.09998F, 0.09998F, 0.09998F));
                AddDisplayRule("VoidSurvivorBody", "Chest", new Vector3(0.36683F, 0.1762F, 0.00002F), new Vector3(0F, 0F, 0F), new Vector3(0.16615F, 0.16615F, 0.16615F));
            };
            ballPrefab = PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, "MysticsItems_CrystalWorldBall", false);

            // DefaultDisplayRule("Head", new Vector3(0f, 0.5f, 0f), new Vector3(0f, -90f, 180f), new Vector3(0.2f, 0.2f, 0.2f));

            /*
             * How the world projection works:
             * When a model (aka a World Container) is spawned in, a random Crystal World is also created at very distant coords
             * Each Crystal World has a Camera that renders the World to a RenderTexture, with a custom skybox
             * Each active camera has a Projector child that draws the RenderTexture as billboards where the Containers are right now, and outputs it into the final Projection RenderTexture
             * Active cameras also have a Container Prerender component that sets every active container's inner display texture to the texture with projections
             */

            // Register all worlds using the same code block
            string[] worldsToLoad = {
                "Crystallize"
                // "In Circles"
            };
            foreach (string worldName in worldsToLoad)
            {
                GameObject world = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Crystal World/Worlds/" + worldName + ".prefab");
                GameObject cameraObject = world.transform.Find("CameraPivot").Find("Camera").gameObject;
                cameraObject.AddComponent<CrystalWorldCamera>();
                RenderTexture renderTexture = null;
                if (!Main.isDedicatedServer) renderTexture = new RenderTexture(1028, 1028, 0, RenderTextureFormat.ARGB32)
                {
                    name = "Crystal World \"" + worldName + "\""
                };
                cameraObject.GetComponent<Camera>().targetTexture = renderTexture;
                foreach (Transform worldObjectTransform in world.transform)
                {
                    GameObject worldObject = worldObjectTransform.gameObject;
                    Renderer renderer = worldObject.GetComponent<MeshRenderer>();
                    if (renderer)
                    {
                        Material material = renderer.material;
                        if (material.shader.name == "Standard")
                        {
                            HopooShaderToMaterial.Standard.Apply(material);
                            HopooShaderToMaterial.Standard.DisableEverything(material);
                        }
                    }
                }
                // Run specific per-world code
                switch (worldName)
                {
                    case "Crystallize":
                        ParticleSystemRenderer snowRenderer = world.transform.Find("Snow").gameObject.GetComponent<ParticleSystemRenderer>();
                        Material snowMaterial = new Material(HopooShaderToMaterial.Standard.shader);
                        snowRenderer.material = snowMaterial;
                        HopooShaderToMaterial.Standard.Emission(snowMaterial, 2f, Color.white);
                        break;
                }
                worlds.Add(new WorldInfo
                {
                    prefab = world,
                    shownInContainers = new List<GameObject>(),
                    renderTexture = renderTexture
                });
            }

            On.RoR2.SceneCamera.Awake += (orig, self) =>
            {
                orig(self);
                GameObject projector = new GameObject
                {
                    name = "Crystal World Projector"
                };
                projector.transform.SetParent(self.transform);
                projector.transform.localPosition = Vector3.zero;
                projector.transform.localRotation = Quaternion.identity;
                CrystalWorldProjector component = projector.AddComponent<CrystalWorldProjector>();
                CrystalWorldContainerPrerender prerenderComponent = self.gameObject.AddComponent<CrystalWorldContainerPrerender>();
                prerenderComponent.renderTexture = component.projectionRenderTexture;
            };

            pulsePrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Crystal World/Explosion.prefab");
            EffectComponent effectComponent = pulsePrefab.AddComponent<EffectComponent>();
            effectComponent.applyScale = true;
            effectComponent.disregardZScale = false;
            effectComponent.soundName = "Play_item_proc_iceRingSpear";
            VFXAttributes vfxAttributes = pulsePrefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
            pulsePrefab.AddComponent<DestroyOnTimer>().duration = 1f;

            GameObject ppHolder = pulsePrefab.transform.Find("PP").gameObject;
            SphereCollider ppSphere = ppHolder.AddComponent<SphereCollider>();
            ppSphere.radius = 60f;
            ppSphere.isTrigger = true;
            ppHolder.layer = LayerIndex.postProcess.intVal;
            PostProcessVolume pp = ppHolder.AddComponent<PostProcessVolume>();
            pp.blendDistance = 30f;
            pp.isGlobal = false;
            pp.weight = 0.2f;
            pp.priority = 10;
            PostProcessProfile ppProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
            ppProfile.name = "ppCrystalWorldExplosion";
            ColorGrading colorGrading = ppProfile.AddSettings<ColorGrading>();
            Color c = Color.white;
            float intensity = 3f;
            c.r *= intensity; c.g *= intensity; c.b *= intensity;
            colorGrading.colorFilter.value = c;
            colorGrading.colorFilter.overrideState = true;
            colorGrading.saturation.value = -100f;
            colorGrading.saturation.overrideState = true;
            colorGrading.contrast.value = -100f;
            colorGrading.contrast.overrideState = true;
            pp.sharedProfile = ppProfile;

            PostProcessDuration postProcessDuration = ppHolder.AddComponent<PostProcessDuration>();
            postProcessDuration.ppVolume = pp;
            postProcessDuration.ppWeightCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 0.35f),
                    new Keyframe(1f, 0f)
                }
            };
            postProcessDuration.maxDuration = 1f;

            MysticsItemsContent.Resources.effectPrefabs.Add(pulsePrefab);

            On.RoR2.HoldoutZoneController.Awake += (orig, self) =>
            {
                orig(self);
                MysticsItemsCrystalWorldTeleporterEffect component = self.GetComponent<MysticsItemsCrystalWorldTeleporterEffect>();
                if (!component)
                {
                    component = self.gameObject.AddComponent<MysticsItemsCrystalWorldTeleporterEffect>();
                    switch (MysticsRisky2Utils.Utils.TrimCloneFromString(self.gameObject.name))
                    {
                        case "Teleporter1":
                            component.displayModel = true;
                            component.offset = new Vector3(0f, 3f, 0f);
                            break;
                        case "LunarTeleporter Variant":
                            component.displayModel = true;
                            component.offset = new Vector3(0f, 3f, 0f);
                            break;
                    }
                }
            };
        }

        public class MysticsItemsCrystalWorldTeleporterEffect : MonoBehaviour
        {
            public HoldoutZoneController holdoutZoneController;
            public GameObject model;
            public float prevPulse = -100f;
            public static float windup = 0.1f;
            public static float winddown = 0.03f;
            public float currentSpin = 0f;
            public float finalSpin = 500f;
            public float currentHeight = 0f;
            public float finalHeight = 5f;
            public Vector3 offset = Vector3.zero;
            public bool displayModel = false;
            public SphereSearch sphereSearch;
            public TeamMask teamMask = default;
            public AnimationCurve animationCurve;
            public float chargeFraction = 0f;
            private float predictionChargeSpeed = 0f;
            private float predictionChargeFractionLast = 0f;
            private float predictionChargeFractionDeltaTime = 0f;

            public void Awake()
            {
                holdoutZoneController = GetComponent<HoldoutZoneController>();

                animationCurve = new AnimationCurve
                {
                    keys = new Keyframe[]
                    {
                        new Keyframe(0f, 0f),
                        new Keyframe(0.5f, 1f),
                        new Keyframe(1f, 0f)
                    },
                    preWrapMode = WrapMode.Clamp,
                    postWrapMode = WrapMode.Clamp
                };

                sphereSearch = new SphereSearch
                {
                    mask = LayerIndex.entityPrecise.mask,
                    origin = transform.position,
                    queryTriggerInteraction = QueryTriggerInteraction.Collide,
                    radius = 0f
                };
                teamMask = TeamMask.AllExcept(TeamIndex.Player);
            }

            public void Start()
            {
                if (displayModel)
                {
                    model = Object.Instantiate(ballPrefab);
                    model.transform.SetParent(transform);
                    model.transform.localPosition = Vector3.zero;
                    model.transform.localRotation = Quaternion.identity;
                    model.transform.localScale = Vector3.one;
                }
            }

            public float NextPulse(int totalPulseCount)
            {
                float timeBetweenPulses = 1f / (float)(totalPulseCount + 1);
                for (int i = 1; i <= totalPulseCount; i++)
                {
                    float timeThisPulse = (float)i * timeBetweenPulses;
                    if (timeThisPulse > prevPulse)
                    {
                        return timeThisPulse;
                    }
                }
                return 1f;
            }

            public void Update()
            {
                // The charge fraction is synced over network only every few frames, so the animation becomes laggy
                // Predict the teleporter charge locally to make it smooth
                predictionChargeFractionDeltaTime += Time.deltaTime;
                float num = holdoutZoneController.charge - predictionChargeFractionLast;
                if (num != 0f)
                {
                    chargeFraction = holdoutZoneController.charge;
                    predictionChargeSpeed = num / predictionChargeFractionDeltaTime;
                    predictionChargeFractionDeltaTime = 0f;
                }
                predictionChargeFractionLast = holdoutZoneController.charge;
                chargeFraction = Mathf.Clamp01(chargeFraction + predictionChargeSpeed * Time.deltaTime);

                int itemCount = Util.GetItemCountForTeam(TeamIndex.Player, MysticsItemsContent.Items.MysticsItems_CrystalWorld.itemIndex, true);
                if (itemCount > 0)
                {
                    float nextPulse = NextPulse(pulses + pulsesPerStack * (itemCount - 1));
                    if (!holdoutZoneController.enabled) nextPulse = 100f;
                    if (nextPulse >= 1f) nextPulse = 100f;

                    float t = (1f - (nextPulse - chargeFraction) / windup) * 0.5f;
                    if (chargeFraction <= (prevPulse + winddown)) t = ((chargeFraction - prevPulse) / winddown) * 0.5f + 0.5f;
                    t = Mathf.Clamp01(t);

                    currentSpin = finalSpin * animationCurve.Evaluate(t);
                    currentHeight = finalHeight * animationCurve.Evaluate(t);
                    if (model)
                    {
                        if (!model.activeSelf) model.SetActive(true);
                        model.transform.Rotate(new Vector3(0f, currentSpin * Time.deltaTime, 0f), Space.Self);
                        model.transform.localPosition = offset + Vector3.up * currentHeight;
                    }

                    if (holdoutZoneController.charge > nextPulse)
                    {
                        prevPulse = nextPulse;
                        if (NetworkServer.active)
                        {
                            var currentPulseRadius = Mathf.Max(holdoutZoneController.currentRadius, minPulseRadius);
                            EffectManager.SpawnEffect(pulsePrefab, new EffectData
                            {
                                origin = transform.position,
                                scale = currentPulseRadius * 2f
                            }, true);
                            sphereSearch.radius = currentPulseRadius;
                            foreach (HurtBox hurtBox in sphereSearch.RefreshCandidates().FilterCandidatesByHurtBoxTeam(teamMask).FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes())
                            {
                                HealthComponent healthComponent = hurtBox.healthComponent;
                                if (healthComponent)
                                {
                                    CharacterBody body = healthComponent.body;
                                    if (body)
                                    {
                                        SetStateOnHurt setStateOnHurt = body.gameObject.GetComponent<SetStateOnHurt>();
                                        if (setStateOnHurt && setStateOnHurt.canBeFrozen)
                                        {
                                            setStateOnHurt.SetFrozen(freezeTime);

                                            // Deal a bit of damage to force execute
                                            DamageInfo damageInfo = new DamageInfo
                                            {
                                                attacker = null,
                                                damage = 12f * 2.4f * TeamManager.instance.GetTeamLevel(TeamIndex.Player),
                                                procCoefficient = 0f,
                                                position = hurtBox.transform.position,
                                                crit = false
                                            };
                                            healthComponent.TakeDamage(damageInfo);
                                        }
                                        else
                                        {
                                            body.AddTimedBuff(MysticsItemsContent.Buffs.MysticsItems_Crystallized, freezeTime);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (model && model.activeSelf) model.SetActive(false);
                }
            }
        }

        public class CrystalWorldCamera : MonoBehaviour
        {
            public Camera camera;

            public void Awake()
            {
                camera = gameObject.GetComponent<Camera>();
                camera.enabled = false;
            }

            public void Render(Camera targetCamera)
            {
                Vector3 targetRotation = targetCamera.transform.rotation.eulerAngles;
                ModelCamera modelCamera = ModelCamera.instance;
                if (modelCamera) targetRotation = modelCamera.attachedCamera.transform.rotation.eulerAngles;

                Vector3 localRotation = camera.gameObject.transform.parent.localRotation.eulerAngles;
                float xRot = targetRotation.x;
                if (xRot > 180f) xRot -= 360f;
                xRot /= 8f;
                localRotation.x = xRot;
                localRotation.y = targetRotation.y;
                localRotation.z = 0f;
                camera.gameObject.transform.parent.localRotation = Quaternion.Euler(localRotation);

                bool revertFogState = RenderSettings.fog;
                RenderSettings.fog = false;

                if (!Main.isDedicatedServer) camera.Render();

                RenderSettings.fog = revertFogState;
            }
        }

        public class CrystalWorldProjector : MonoBehaviour
        {
            public Camera camera;
            public Camera parentCamera;
            public RenderTexture projectionRenderTexture;
            public LayerIndex layerIndex = LayerIndex.transparentFX;
            
            public void Awake()
            {
                parentCamera = transform.parent.gameObject.GetComponent<Camera>();

                if (!Main.isDedicatedServer) projectionRenderTexture = new RenderTexture(1028, 1028, 0, RenderTextureFormat.ARGB32);

                camera = gameObject.AddComponent<Camera>();
                camera.forceIntoRenderTexture = true;
                camera.targetTexture = projectionRenderTexture;
                camera.cullingMask = layerIndex.mask;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.black;
                camera.enabled = false;
            }

            public void LateUpdate()
            {
                if (Main.isDedicatedServer) return;
                camera.fieldOfView = parentCamera.fieldOfView;
                camera.aspect = parentCamera.aspect;
                int renderedCount = 0;
                foreach (GameObject crystalWorldContainer in CrystalWorldContainer.list)
                {
                    CrystalWorldContainer component = crystalWorldContainer.GetComponent<CrystalWorldContainer>();
                    if (component && component.HasWorld)
                    {
                        GameObject rendererObject = crystalWorldContainer.transform.Find("Projection Renderer").gameObject;
                        MeshRenderer renderer = rendererObject.GetComponent<MeshRenderer>();
                        Renderer shapeRenderer = crystalWorldContainer.GetComponentInChildren<Renderer>();
                        if (shapeRenderer.isVisible)
                        {
                            renderedCount++;

                            component.world.cameraComponent.Render(camera);

                            rendererObject.transform.LookAt(rendererObject.transform.position + Vector3.Normalize(camera.transform.forward), Vector3.up);
                            rendererObject.transform.Rotate(new Vector3(90f, 180f, 0f), Space.Self);
                            rendererObject.SetActive(true);
                            rendererObject.layer = layerIndex.intVal;
                            renderer.material.mainTexture = component.world.renderTexture;
                        }
                    }
                }
                if (renderedCount > 0) camera.Render();
                foreach (GameObject crystalWorldContainer in CrystalWorldContainer.list)
                {
                    GameObject rendererObject = crystalWorldContainer.transform.Find("Projection Renderer").gameObject;
                    rendererObject.SetActive(false);
                }
            }

            public void OnDestroy()
            {
                Object.Destroy(projectionRenderTexture);
            }
        }

        public class CrystalWorldContainerPrerender : MonoBehaviour
        {
            public RenderTexture renderTexture;

            public void OnPreRender()
            {
                if (!renderTexture) return;
                foreach (GameObject crystalWorldContainer in CrystalWorldContainer.list)
                {
                    Material material = crystalWorldContainer.transform.Find("Icosphere").gameObject.GetComponent<MeshRenderer>().material;
                    material.SetTexture("_OverrideBackgroundTex", renderTexture);
                }
            }
        }

        public class CrystalWorldContainer : MonoBehaviour
        {
            public static List<GameObject> list = new List<GameObject>();

            public WorldInfo world;
            public bool HasWorld
            {
                get
                {
                    return !world.Equals(default(WorldInfo)) && world.currentObject;
                }
            }

            public WorldInfo GetWorld()
            {
                if (worlds.Count > 0)
                {
                    int index = Mathf.FloorToInt(worlds.Count * (Random.value * 0.99f));
                    WorldInfo worldInfo = worlds.ElementAt(index);
                    if (!worldInfo.currentObject)
                    {
                        GameObject newWorld = Object.Instantiate(worldInfo.prefab, new Vector3(-9000f + index * -4000f, -14000f, 5000f), Quaternion.identity);
                        worldInfo.currentObject = newWorld;
                        worldInfo.cameraComponent = newWorld.transform.Find("CameraPivot").Find("Camera").gameObject.GetComponent<CrystalWorldCamera>();
                    }
                    return worldInfo;
                }
                return default;
            }

            public void Update()
            {
                if (!HasWorld)
                {
                    world = GetWorld();
                    world.shownInContainers.Add(gameObject);
                }
            }

            public void OnEnable()
            {
                list.Add(gameObject);
            }

            public void OnDisable()
            {
                list.Remove(gameObject);
            }

            public void OnDestroy()
            {
                if (HasWorld)
                {
                    world.shownInContainers.Remove(gameObject);
                    if (world.shownInContainers.Count <= 0)
                    {
                        Object.Destroy(world.currentObject);
                    }
                }
            }
        }
    }
}
