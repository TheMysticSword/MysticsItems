using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Equipment
{
    public class SirenPole : BaseEquipment
    {
        public static ILHook onTeleporterBeginChargingGlobalHook;
        public static ILHook onTeleporterChargedGlobalHook;
        public static GameObject inWorldPrefab;

        public static ConfigurableValue<float> baseRadius = new ConfigurableValue<float>(
            "Equipment: Warning System",
            "BaseRadius",
            75f,
            "Charge zone base radius (in meters)"
        );
        public static ConfigurableValue<float> chargeTime = new ConfigurableValue<float>(
            "Equipment: Warning System",
            "ChargeTime",
            45f,
            "Zone charge time (in seconds)",
            new List<string>()
            {
                "EQUIPMENT_MYSTICSITEMS_SIRENPOLE_DESC"
            }
        );
        public static ConfigurableValue<int> totalWaves = new ConfigurableValue<int>(
            "Equipment: Warning System",
            "TotalWaves",
            4,
            "Total enemy spawn waves while charging"
        );

        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            inWorldPrefab = PrefabAPI.InstantiateClone(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Siren Pole/InWorld.prefab"), "MysticsItems_SirenPoleInWorld", false);
            inWorldPrefab.AddComponent<NetworkIdentity>();
            PrefabAPI.RegisterNetworkPrefab(inWorldPrefab);
        }

        public override void OnLoad()
        {
            equipmentDef.name = "MysticsItems_SirenPole";
            ConfigManager.Balance.CreateEquipmentCooldownOption(equipmentDef, "Equipment: Warning System", 75f);
            equipmentDef.canDrop = true;
            ConfigManager.Balance.CreateEquipmentEnigmaCompatibleOption(equipmentDef, "Equipment: Warning System", true);
            ConfigManager.Balance.CreateEquipmentCanBeRandomlyTriggeredOption(equipmentDef, "Equipment: Warning System", true);
            equipmentDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Siren Pole/Model.prefab"));
            equipmentDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Equipment/Siren Pole/Icon.png");

            foreach (var mat in equipmentDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterials)
            {
                HopooShaderToMaterial.Standard.Apply(mat);
                HopooShaderToMaterial.Standard.Gloss(mat);
            }

            itemDisplayPrefab = PrepareItemDisplayModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Equipment/Siren Pole/ItemDisplayModel.prefab"));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Chest", new Vector3(0.17606F, 0.10938F, -0.13135F), new Vector3(0.29792F, 101.7337F, 14.83983F), new Vector3(0.07862F, 0.07862F, 0.07862F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(0.09063F, 0.20626F, -0.04851F), new Vector3(305F, 0F, 0F), new Vector3(0.019F, 0.019F, 0.019F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(-0.09086F, 0.20552F, -0.04818F), new Vector3(305F, 0F, 0F), new Vector3(0.019F, 0.019F, 0.019F));
                AddDisplayRule("Bandit2Body", "Chest", new Vector3(-0.14208F, 0.31664F, -0.2016F), new Vector3(41.46172F, 268.9803F, 180F), new Vector3(0.0478F, 0.0478F, 0.0478F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(-1.82578F, 2.33771F, -1.55785F), new Vector3(0F, 36.0168F, 0F), new Vector3(0.349F, 0.349F, 0.349F));
                AddDisplayRule("EngiBody", "Chest", new Vector3(-0.08444F, 0.34718F, -0.26302F), new Vector3(10.10721F, 23.50117F, 4.36377F), new Vector3(0.047F, 0.047F, 0.047F));
                AddDisplayRule("MageBody", "Chest", new Vector3(0.1608F, 0.06538F, -0.13697F), new Vector3(8.71729F, 3.87203F, 0.58771F), new Vector3(0.05396F, 0.05396F, 0.05396F));
                AddDisplayRule("MercBody", "HandR", new Vector3(0.17621F, 0.22065F, 0F), new Vector3(0F, 0F, 101.9694F), new Vector3(0.08846F, 0.08846F, 0.08846F));
                AddDisplayRule("TreebotBody", "PlatformBase", new Vector3(0.57548F, 1.38046F, -0.57101F), new Vector3(0F, 33.34396F, 0F), new Vector3(0.16617F, 0.16617F, 0.16617F));
                AddDisplayRule("LoaderBody", "MechHandL", new Vector3(0.31421F, 0.28839F, 0.14368F), new Vector3(9.63979F, 331.6965F, 84.84715F), new Vector3(0.12826F, 0.12826F, 0.12703F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(-4.57107F, 4.06093F, 0.00616F), new Vector3(283.0015F, 97.09286F, 178.9819F), new Vector3(1.08323F, 1.08323F, 1.08323F));
                AddDisplayRule("CaptainBody", "Chest", new Vector3(0.26173F, 0.18196F, 0.04468F), new Vector3(0F, 304.076F, 0F), new Vector3(0.09072F, 0.09072F, 0.09072F));
                AddDisplayRule("ScavBody", "Backpack", new Vector3(0.93854F, 9.81353F, 0.67947F), new Vector3(23.62728F, 47.52236F, 14.61215F), new Vector3(1.66171F, 1.66171F, 1.66171F));
                AddDisplayRule("EquipmentDroneBody", "GunBarrelBase", new Vector3(0.00002F, -0.72671F, 0.39234F), new Vector3(47.37738F, -0.00013F, 0.0009F), new Vector3(0.42601F, 0.42601F, 0.42601F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "AntennaL", new Vector3(-0.00958F, 0.16798F, 0.00024F), new Vector3(0F, 91.3699F, 0F), new Vector3(0.0678F, 0.0678F, 0.0678F));
                AddDisplayRule("RailgunnerBody", "Backpack", new Vector3(-0.19222F, -0.15375F, 0.0742F), new Vector3(0F, 90F, 0F), new Vector3(0.09801F, 0.09801F, 0.09801F));
                AddDisplayRule("VoidSurvivorBody", "Head", new Vector3(0.47319F, 0.1069F, 0.00462F), new Vector3(-0.00001F, 180F, 259.7787F), new Vector3(0.08856F, 0.09671F, 0.08856F));
            };

            MysticsItemsSirenPoleController sirenPoleController = inWorldPrefab.AddComponent<MysticsItemsSirenPoleController>();
            sirenPoleController.poleTransform = inWorldPrefab.transform.Find("PoleTransform");

            HoldoutZoneController holdoutZoneController = inWorldPrefab.AddComponent<HoldoutZoneController>();
            holdoutZoneController.baseRadius = baseRadius.Value;
            holdoutZoneController.baseChargeDuration = chargeTime.Value;
            holdoutZoneController.radiusSmoothTime = 0.3f;
            holdoutZoneController.radiusIndicator = inWorldPrefab.transform.Find("RadiusIndicator").gameObject.GetComponent<Renderer>();
            holdoutZoneController.inBoundsObjectiveToken = "OBJECTIVE_MYSTICSITEMS_CHARGE_SIRENPOLE";
            holdoutZoneController.outOfBoundsObjectiveToken = "OBJECTIVE_MYSTICSITEMS_CHARGE_SIRENPOLE";
            holdoutZoneController.applyHealingNova = true;
            holdoutZoneController.applyFocusConvergence = true;
            holdoutZoneController.playerCountScaling = 0f;
            holdoutZoneController.dischargeRate = 0f;
            holdoutZoneController.enabled = false;
            sirenPoleController.holdoutZoneController = holdoutZoneController;

            On.RoR2.HoldoutZoneController.ChargeHoldoutZoneObjectiveTracker.ShouldBeFlashing += (orig, self) =>
            {
                if (self.sourceDescriptor.master)
                {
                    HoldoutZoneController holdoutZoneController2 = (HoldoutZoneController)self.sourceDescriptor.source;
                    if (holdoutZoneController2 && holdoutZoneController.gameObject.GetComponent<MysticsItemsSirenPoleController>())
                    {
                        return false;
                    }
                }
                return orig(self);
            };

            CombatDirector phaseCombatDirector = inWorldPrefab.AddComponent<CombatDirector>();
            phaseCombatDirector.customName = "WeakMonsters";
            phaseCombatDirector.expRewardCoefficient = 0.1f;
            phaseCombatDirector.minSeriesSpawnInterval = 0.5f;
            phaseCombatDirector.maxSeriesSpawnInterval = 0.5f;
            phaseCombatDirector.minRerollSpawnInterval = 2f;
            phaseCombatDirector.maxRerollSpawnInterval = 4f;
            phaseCombatDirector.moneyWaveIntervals = new RangeFloat[] { };
            phaseCombatDirector.teamIndex = TeamIndex.Monster;
            phaseCombatDirector.creditMultiplier = 1f;
            phaseCombatDirector.spawnDistanceMultiplier = 1f;
            phaseCombatDirector.shouldSpawnOneWave = true;
            phaseCombatDirector.targetPlayers = false;
            phaseCombatDirector.skipSpawnIfTooCheap = false;
            phaseCombatDirector.resetMonsterCardIfFailed = true;
            phaseCombatDirector.maximumNumberToSpawnBeforeSkipping = 5;
            phaseCombatDirector.eliteBias = 1f;
            phaseCombatDirector.ignoreTeamSizeLimit = true;
            phaseCombatDirector.fallBackToStageMonsterCards = false;
            phaseCombatDirector.monsterSpawnTimer = 0f;
            phaseCombatDirector.enabled = false;
            sirenPoleController.waveCombatDirector = phaseCombatDirector;

            TeleporterInteraction teleporterInteraction = inWorldPrefab.AddComponent<TeleporterInteraction>();
            teleporterInteraction.enabled = false;

            ShakeEmitter shakeEmitter = inWorldPrefab.AddComponent<ShakeEmitter>();
            shakeEmitter.duration = 0.2f;
            shakeEmitter.amplitudeTimeDecay = true;
            shakeEmitter.radius = 20f;
            shakeEmitter.shakeOnStart = true;
            shakeEmitter.shakeOnEnable = false;
            shakeEmitter.wave.frequency = 8f;
            shakeEmitter.wave.amplitude = 5f;

            shakeEmitter = inWorldPrefab.AddComponent<ShakeEmitter>();
            shakeEmitter.duration = 0.2f;
            shakeEmitter.amplitudeTimeDecay = true;
            shakeEmitter.radius = 60f;
            shakeEmitter.shakeOnStart = false;
            shakeEmitter.shakeOnEnable = true;
            shakeEmitter.wave.frequency = 8f;
            shakeEmitter.wave.amplitude = 5f;
            sirenPoleController.waveShakeEmitter = shakeEmitter;

            inWorldPrefab.AddComponent<GenericDisplayNameProvider>().displayToken = "MYSTICSITEMS_SIRENPOLE_NAME";
            inWorldPrefab.AddComponent<ProxyInteraction>();
            inWorldPrefab.transform.Find("PoleTransform/InteractionCollider").gameObject.AddComponent<EntityLocator>().entity = inWorldPrefab;

            onTeleporterBeginChargingGlobalHook = new ILHook(
                typeof(MysticsItemsSirenPoleController).GetMethod("RunOnTeleporterBeginChargingGlobalHook"),
                il =>
                {
                    ILCursor c = new ILCursor(il);
                    c.Emit(OpCodes.Ldsfld, typeof(TeleporterInteraction).GetField("onTeleporterBeginChargingGlobal", Main.bindingFlagAll));
                    c.Emit(OpCodes.Ldarg, 1);
                    c.Emit(OpCodes.Callvirt, typeof(System.Action<TeleporterInteraction>).GetMethodCached("Invoke"));
                }
            );
            onTeleporterChargedGlobalHook = new ILHook(
                typeof(MysticsItemsSirenPoleController).GetMethod("RunOnTeleporterChargedGlobalHook"),
                il =>
                {
                    ILCursor c = new ILCursor(il);
                    c.Emit(OpCodes.Ldsfld, typeof(TeleporterInteraction).GetField("onTeleporterChargedGlobal", Main.bindingFlagAll));
                    c.Emit(OpCodes.Ldarg, 1);
                    c.Emit(OpCodes.Callvirt, typeof(System.Action<TeleporterInteraction>).GetMethodCached("Invoke"));
                }
            );

            On.RoR2.TeleporterInteraction.Awake += (orig, self) =>
            {
                if (!self.GetComponent<MysticsItemsSirenPoleController>()) orig(self);
            };

            On.RoR2.TeleporterInteraction.IdleToChargingState.OnEnter += (orig, self) =>
            {
                MysticsItemsSirenPoleController.ForceEnd();
                orig(self);
            };

            On.RoR2.Language.GetLocalizedStringByToken += Language_GetLocalizedStringByToken;
        }

        private string Language_GetLocalizedStringByToken(On.RoR2.Language.orig_GetLocalizedStringByToken orig, Language self, string token)
        {
            var result = orig(self, token);
            if (token == "EQUIPMENT_MYSTICSITEMS_SIRENPOLE_DESC")
                result = Utils.FormatStringByDict(result, new System.Collections.Generic.Dictionary<string, string>()
                {
                    { "WaveSpawnInterval", (100f / (float)totalWaves).ToString(System.Globalization.CultureInfo.InvariantCulture) }
                });
            return result;
        }

        public override bool OnUse(EquipmentSlot equipmentSlot)
        {
            if (MysticsItemsSirenPoleController.instance) return false;
            var inst = Object.Instantiate(inWorldPrefab, equipmentSlot.characterBody.corePosition, Quaternion.identity);
            NetworkServer.Spawn(inst);
            return true;
        }

        public class MysticsItemsSirenPoleController : MonoBehaviour
        {
            public HoldoutZoneController holdoutZoneController;
            public int wave = -1;
            public int totalWaves = SirenPole.totalWaves;
            public CombatDirector waveCombatDirector;
            public float baseMonsterCredit = 50f;
            public float monsterCredit;
            public int minimumToSpawn = 3;
            public ShakeEmitter waveShakeEmitter;

            public float delay = 1f;
            public float delayMax = 1f;

            public Transform poleTransform;
            public Vector3 poleTransformLocalPositionTarget;

            public static MysticsItemsSirenPoleController instance;

            public bool runTeleporterEvents = false;

            public void Awake()
            {
                runTeleporterEvents = !(TeleporterInteraction.instance && (TeleporterInteraction.instance.currentState is TeleporterInteraction.IdleToChargingState || TeleporterInteraction.instance.currentState is TeleporterInteraction.ChargingState));

                holdoutZoneController.onCharged = new HoldoutZoneController.HoldoutZoneControllerChargedUnityEvent();
                holdoutZoneController.onCharged.AddListener((zone) =>
                {
                    if (runTeleporterEvents) RunOnTeleporterChargedGlobal(GetComponent<TeleporterInteraction>());
                    zone.enabled = false;
                    delay = delayMax;
                    Util.PlaySound("Play_loader_shift_release", gameObject);
                    ShakeEmitter.CreateSimpleShakeEmitter(transform.position, new Wave { amplitude = 7f, frequency = 2.4f }, 0.1f, zone.currentRadius, true);

                    if (NetworkServer.active && (!TeleporterInteraction.instance || !TeleporterInteraction.instance.isCharged))
                    {
                        foreach (var purchaseInteraction in InstanceTracker.GetInstancesList<PurchaseInteraction>().Where(x => x.setUnavailableOnTeleporterActivated))
                        {
                            purchaseInteraction.SetAvailable(true);
                        }
                    }
                });
                holdoutZoneController.calcChargeRate += HoldoutZoneController_calcChargeRate;
                waveCombatDirector.currentSpawnTarget = gameObject;
                monsterCredit = baseMonsterCredit/* * (Run.instance ? Run.instance.difficultyCoefficient : 1f)*/;

                if (Physics.Raycast(transform.position, Vector3.down, out var raycastHit, 500f, LayerIndex.world.mask))
                {
                    transform.position = raycastHit.point;
                    transform.up = raycastHit.normal;
                }

                ForceEnd();
                instance = this;

                Util.PlaySound("MysticsItems_Play_item_use_sirenpole", gameObject);

                if (poleTransform)
                {
                    poleTransformLocalPositionTarget = poleTransform.localPosition;
                    poleTransform.localPosition = Vector3.zero;
                }

                ShakeEmitter.CreateSimpleShakeEmitter(transform.position, new Wave { amplitude = 0.5f, frequency = 10f }, 0.1f, 15f, true);
            }

            private void HoldoutZoneController_calcChargeRate(ref float rate)
            {
                if (holdoutZoneController)
                {
                    holdoutZoneController.dischargeRate = -rate;
                }
            }

            public void RunOnTeleporterBeginChargingGlobal(TeleporterInteraction teleporterInteraction)
            {
                try
                {
                    RunOnTeleporterBeginChargingGlobalHook(teleporterInteraction);
                }
                catch { }
                if (NetworkServer.active)
                {
                    foreach (var teamComponent in TeamComponent.GetTeamMembers(TeamIndex.Player))
                    {
                        CharacterBody body = teamComponent.body;
                        if (body)
                        {
                            CharacterMaster master = teamComponent.body.master;
                            if (master)
                            {
                                int itemCount = master.inventory.GetItemCount(RoR2Content.Items.WardOnLevel);
                                if (itemCount > 0)
                                {
                                    GameObject gameObject = Object.Instantiate<GameObject>(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard"), body.transform.position, Quaternion.identity);
                                    gameObject.GetComponent<TeamFilter>().teamIndex = TeamIndex.Player;
                                    gameObject.GetComponent<BuffWard>().Networkradius = 8f + 8f * (float)itemCount;
                                    NetworkServer.Spawn(gameObject);
                                }
                            }
                        }
                    }
                }
            }

            public void RunOnTeleporterBeginChargingGlobalHook(TeleporterInteraction teleporterInteraction) { }

            public void RunOnTeleporterChargedGlobal(TeleporterInteraction teleporterInteraction)
            {
                try
                {
                    RunOnTeleporterChargedGlobalHook(teleporterInteraction);
                }
                catch { }
            }

            public void RunOnTeleporterChargedGlobalHook(TeleporterInteraction teleporterInteraction) { }

            public void FixedUpdate()
            {
                if (delay > 0)
                {
                    delay -= Time.fixedDeltaTime;
                    if (poleTransform) poleTransform.localPosition = (holdoutZoneController.charge < 1f ? 1f - delay / delayMax : delay / delayMax) * poleTransformLocalPositionTarget;
                    if (delay <= 0)
                    {
                        if (holdoutZoneController.charge < 1f)
                        {
                            holdoutZoneController.enabled = true;
                            if (runTeleporterEvents) RunOnTeleporterBeginChargingGlobal(GetComponent<TeleporterInteraction>());
                        }
                        else
                        {
                            if (NetworkServer.active) NetworkServer.Destroy(gameObject);
                            else Object.Destroy(gameObject);
                        }
                    }
                }
                if (holdoutZoneController && holdoutZoneController.enabled)
                {
                    int newWave = Mathf.FloorToInt(holdoutZoneController.charge / (1f / (float)totalWaves));
                    if (newWave > wave && newWave < totalWaves)
                    {
                        wave = newWave;
                        TriggerPhase();
                    }
                }
            }

            public void TriggerPhase()
            {
                ShakeEmitter.CreateSimpleShakeEmitter(transform.position, new Wave { amplitude = 3f, frequency = 4f }, 0.15f, 50f, true);
                Util.PlaySound("MysticsItems_Play_env_sirenpole", gameObject);
                if (NetworkServer.active) SummonEnemies();
            }

            public void SummonEnemies()
            {
                for (var i = 0; i < minimumToSpawn; i++)
                {
                    var selection = Util.CreateReasonableDirectorCardSpawnList(monsterCredit, waveCombatDirector.maximumNumberToSpawnBeforeSkipping, minimumToSpawn - i);
                    if (selection.Count > 0)
                    {
                        waveCombatDirector.hasStartedWave = false;
                        waveCombatDirector.monsterSpawnTimer = 0f;
                        waveCombatDirector.monsterCredit = monsterCredit;
                        waveCombatDirector.OverrideCurrentMonsterCard(selection.Evaluate(RoR2Application.rng.nextNormalizedFloat));
                        waveCombatDirector.currentActiveEliteTier = EliteAPI.VanillaEliteTiers[0];
                        waveCombatDirector.currentActiveEliteDef = RoR2Application.rng.NextElementUniform(waveCombatDirector.currentActiveEliteTier.eliteTypes);
                        waveCombatDirector.enabled = true;
                        break;
                    }
                }
            }

            public static void ForceEnd()
            {
                if (instance) instance.holdoutZoneController.charge = 1f;
            }
        }
    }
}
