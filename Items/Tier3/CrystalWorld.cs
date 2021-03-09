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
        public static GameObject pulsePrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Crystal World/Explosion.prefab");

        public static float freezeTime = 5f;

        public override void PreAdd()
        {
            itemDef.name = "CrystalWorld";
            itemDef.tier = ItemTier.Tier3;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage,
                ItemTag.Utility,
                ItemTag.AIBlacklist
            };
            BanFromDeployables();
            SetAssets("Crystal World");
            model.AddComponent<CrystalWorldContainer>();
            CopyModelToFollower();

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
                "Crystallize",
                "In Circles"
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
                            Main.HopooShaderToMaterial.Standard.Apply(material);
                            Main.HopooShaderToMaterial.Standard.DisableEverything(material);
                        }
                    }
                }
                // Run specific per-world code
                switch (worldName)
                {
                    case "Crystallize":
                        ParticleSystemRenderer snowRenderer = world.transform.Find("Snow").gameObject.GetComponent<ParticleSystemRenderer>();
                        Material snowMaterial = new Material(Main.HopooShaderToMaterial.Standard.shader);
                        snowRenderer.material = snowMaterial;
                        Main.HopooShaderToMaterial.Standard.Emission(snowMaterial, 2f, Color.white);
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
            ppSphere.radius = 100f;
            ppSphere.isTrigger = true;
            ppHolder.layer = LayerIndex.postProcess.intVal;
            PostProcessVolume pp = ppHolder.AddComponent<PostProcessVolume>();
            pp.blendDistance = 0.7f;
            pp.isGlobal = true;
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

            AssetManager.RegisterEffect(pulsePrefab);
        }

        public override void OnAdd()
        {
            GameObject teleporter = Resources.Load<GameObject>("Prefabs/NetworkedObjects/Teleporters/Teleporter1");
            TeleporterEffect teleporterEffect = teleporter.AddComponent<TeleporterEffect>();
            teleporterEffect.offset = new Vector3(0f, 3f, 0f);
        }

        public class TeleporterEffect : MonoBehaviour
        {
            public TeleporterInteraction teleporterInteraction;
            public GameObject model;
            public float prevPulse = -100f;
            public static float windup = 0.1f;
            public static float winddown = 0.03f;
            public float currentSpin = 0f;
            public float finalSpin = 500f;
            public float currentHeight = 0f;
            public float finalHeight = 5f;
            public Vector3 offset = Vector3.zero;
            public SphereSearch sphereSearch;
            public TeamMask teamMask = default;
            public AnimationCurve animationCurve;
            public float chargeFraction = 0f;
            private float predictionChargeSpeed = 0f;
            private float predictionChargeFractionLast = 0f;
            private float predictionChargeFractionDeltaTime = 0f;

            public void Awake()
            {
                teleporterInteraction = GetComponent<TeleporterInteraction>();
                model = Object.Instantiate(registeredItems[typeof(CrystalWorld)].model);
                model.transform.SetParent(transform);
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;

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
                float num = teleporterInteraction.chargeFraction - predictionChargeFractionLast;
                if (num != 0f)
                {
                    chargeFraction = teleporterInteraction.chargeFraction;
                    predictionChargeSpeed = num / predictionChargeFractionDeltaTime;
                    predictionChargeFractionDeltaTime = 0f;
                }
                predictionChargeFractionLast = teleporterInteraction.chargeFraction;
                chargeFraction = Mathf.Clamp01(chargeFraction + predictionChargeSpeed * Time.deltaTime);

                int itemCount = 0;
                foreach (CharacterMaster characterMaster in CharacterMaster.readOnlyInstancesList)
                    if (characterMaster.teamIndex == TeamIndex.Player)
                    {
                        itemCount += characterMaster.inventory.GetItemCount(registeredItems[typeof(CrystalWorld)].itemIndex);
                    }
                if (itemCount > 0)
                {
                    if (!model.activeSelf) model.SetActive(true);

                    float nextPulse = NextPulse(2 + (itemCount - 1));
                    if (!teleporterInteraction.isCharging) nextPulse = 100f;

                    float t = (1f - (nextPulse - chargeFraction) / windup) * 0.5f;
                    if (chargeFraction <= (prevPulse + winddown)) t = ((chargeFraction - prevPulse) / winddown) * 0.5f + 0.5f;
                    t = Mathf.Clamp01(t);

                    currentSpin = finalSpin * animationCurve.Evaluate(t);
                    currentHeight = finalHeight * animationCurve.Evaluate(t);
                    model.transform.Rotate(new Vector3(0f, currentSpin * Time.deltaTime, 0f), Space.Self);
                    model.transform.localPosition = offset + Vector3.up * currentHeight;

                    if (teleporterInteraction.chargeFraction > nextPulse)
                    {
                        prevPulse = nextPulse;
                        if (NetworkServer.active)
                        {
                            EffectManager.SpawnEffect(pulsePrefab, new EffectData
                            {
                                origin = transform.position,
                                scale = teleporterInteraction.holdoutZoneController.baseRadius * 2f
                            }, true);
                            sphereSearch.radius = teleporterInteraction.holdoutZoneController.baseRadius;
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
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (model.activeSelf) model.SetActive(false);
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
                foreach (GameObject crystalWorldContainer in CrystalWorldContainer.list)
                {
                    CrystalWorldContainer component = crystalWorldContainer.GetComponent<CrystalWorldContainer>();
                    if (component && component.HasWorld)
                    {
                        component.world.cameraComponent.Render(camera);

                        GameObject rendererObject = crystalWorldContainer.transform.Find("Projection Renderer").gameObject;
                        MeshRenderer renderer = rendererObject.GetComponent<MeshRenderer>();
                        rendererObject.transform.LookAt(rendererObject.transform.position + Vector3.Normalize(camera.transform.forward), Vector3.up);
                        rendererObject.transform.Rotate(new Vector3(90f, 180f, 0f), Space.Self);
                        rendererObject.SetActive(true);
                        rendererObject.layer = layerIndex.intVal;
                        renderer.material.mainTexture = component.world.renderTexture;
                    }
                }
                camera.Render();
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
