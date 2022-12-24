using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API.Networking.Interfaces;
using R2API.Networking;
using RoR2.UI;
using TMPro;
using UnityEngine.UI;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Items
{
    public class Rhythm : BaseItem
    {
        public static GameObject rhythmHUD;
        
        public static ConfigurableValue<float> critBonus = new ConfigurableValue<float>(
            "Item: Metronome",
            "CritBonus",
            0.5f,
            "How much crit to add for each successful rhythm beat (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_RHYTHM_DESC"
            }
        );
        public static ConfigurableValue<float> critBonusPerStack = new ConfigurableValue<float>(
            "Item: Metronome",
            "CritBonusPerStack",
            0.5f,
            "How much crit to add for each successful rhythm beat (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_RHYTHM_DESC"
            }
        );
        public static ConfigurableValue<float> beatWindowEarly = new ConfigurableValue<float>(
            "Item: Metronome",
            "BeatWindowEarly",
            0.105f,
            "How early can you press to score a hit (in seconds)"
        );
        public static ConfigurableValue<float> beatWindowLate = new ConfigurableValue<float>(
            "Item: Metronome",
            "BeatWindowLate",
            0.105f,
            "How late can you press to score a hit (in seconds)"
        );
        public static ConfigOptions.ConfigurableValue<bool> soundsEnabled = new ConfigOptions.ConfigurableValue<bool>(
            ConfigManager.General.config,
            "Effects",
            "Metronome SFX",
            true,
            "Should the Metronome item play sounds?"
        );
        public static ConfigOptions.ConfigurableValue<bool> critLossEnabled = new ConfigOptions.ConfigurableValue<bool>(
            ConfigManager.General.config,
            "Gameplay",
            "Metronome Crit Loss",
            false,
            "Should the Metronome item reduce crit stacks for missing a beat?"
        );

        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            NetworkingAPI.RegisterMessageType<MysticsItemsRhythmBehaviour.SyncRhythmBonus>();
        }

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_Rhythm";
            SetItemTierWhenAvailable(ItemTier.Tier2);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage,
                ItemTag.CannotCopy,
                ItemTag.AIBlacklist
            };
            
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Metronome/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Metronome/Icon.png");
            MysticsItemsContent.Resources.unlockableDefs.Add(GetUnlockableDef());
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "GunMeshR", new Vector3(-0.14977F, -0.00011F, 0.19943F), new Vector3(357.2297F, 89.56014F, 94.90321F), new Vector3(0.06413F, 0.06413F, 0.06413F));
                AddDisplayRule("HuntressBody", "Muzzle", new Vector3(0.5712F, -0.07049F, -0.35215F), new Vector3(6.74464F, 202.348F, 181.4547F), new Vector3(0.06599F, 0.06599F, 0.06599F));
                AddDisplayRule("Bandit2Body", "MainWeapon", new Vector3(-0.04183F, 0.35156F, -0.0992F), new Vector3(88.13229F, 267.2944F, 94.61585F), new Vector3(0.08705F, 0.08705F, 0.08705F));
                AddDisplayRule("ToolbotBody", "Head", new Vector3(-0.91116F, 1.81749F, -1.53393F), new Vector3(307.0614F, 172.1482F, 186.2799F), new Vector3(1.12349F, 1.12349F, 1.12349F));
                AddDisplayRule("EngiBody", "Chest", new Vector3(0.15956F, 0.09173F, 0.2025F), new Vector3(25.66347F, 26.21173F, 15.26607F), new Vector3(0.10282F, 0.10282F, 0.10282F));
                //AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.035F, 0.89075F, -1.47928F), new Vector3(0F, 90F, 303.695F), new Vector3(0.07847F, 0.07847F, 0.07847F));
                //AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.03562F, 1.40676F, -1.39837F), new Vector3(0F, 90F, 303.1705F), new Vector3(0.08093F, 0.09844F, 0.07912F));
                AddDisplayRule("MageBody", "LowerArmR", new Vector3(0.00535F, 0.17996F, 0.16359F), new Vector3(78.29745F, 168.3501F, 168.913F), new Vector3(0.11636F, 0.11636F, 0.11636F));
                AddDisplayRule("MercBody", "Head", new Vector3(0.04902F, 0.12013F, -0.1775F), new Vector3(71.07915F, 177.6901F, 358.0208F), new Vector3(0.06645F, 0.06645F, 0.06645F));
                AddDisplayRule("TreebotBody", "Chest", new Vector3(0.24295F, -0.27633F, 0.51083F), new Vector3(23.09669F, 28.90149F, 30.20801F), new Vector3(0.21041F, 0.21041F, 0.20824F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(0.11134F, 0.33975F, 0.46839F), new Vector3(0F, 0F, 0F), new Vector3(0.09109F, 0.09109F, 0.09109F));
                AddDisplayRule("CrocoBody", "LowerArmL", new Vector3(-0.95799F, 3.65747F, -1.35262F), new Vector3(72.10529F, 137.362F, 285.0087F), new Vector3(1F, 1F, 1F));
                AddDisplayRule("CaptainBody", "MuzzleGun", new Vector3(0.00434F, 0.07836F, 0.00366F), new Vector3(355.4746F, 178.9148F, 3.6623F), new Vector3(0.08307F, 0.08307F, 0.08307F));
                AddDisplayRule("BrotherBody", "LowerArmL", BrotherInfection.green, new Vector3(0.06027F, 0.23329F, 0.00112F), new Vector3(39.08279F, 113.2626F, 275.5628F), new Vector3(0.04861F, 0.04367F, 0.10724F));
                //AddDisplayRule("ScavBody", "Stomach", new Vector3(-0.92389F, 11.6509F, -5.90638F), new Vector3(20.93637F, 118.4181F, 332.9505F), new Vector3(0.24839F, 0.25523F, 0.24839F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Chest", new Vector3(0.00594F, 0.11541F, -0.28484F), new Vector3(17.80415F, 179.5633F, 359.9077F), new Vector3(0.09333F, 0.09333F, 0.09333F));
                AddDisplayRule("RailgunnerBody", "GunScope", new Vector3(0F, 0.31801F, 0.29244F), new Vector3(0F, 180F, 0F), new Vector3(0.09512F, 0.09512F, 0.09512F));
                AddDisplayRule("VoidSurvivorBody", "Chest", new Vector3(-0.09581F, 0.08226F, 0.18489F), new Vector3(351.5411F, 0.02671F, 5.65843F), new Vector3(0.0887F, 0.0887F, 0.05188F));
            };
            
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            HUD.onHudTargetChangedGlobal += HUD_onHudTargetChangedGlobal;
            
            var animationCurveHolderObj = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Metronome/AnimationCurveHolder.prefab").GetComponent<ParticleSystem>();
            var animationCurveHolder = animationCurveHolderObj.main;

            rhythmHUD = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Metronome/RhythmHUD.prefab");
            rhythmHUD.AddComponent<HudElement>();
            rhythmHUD.transform.localPosition += 20f * Vector3.right;
            rhythmHUD.transform.localScale *= 1.4f;
            MysticsItemsRhythmHUD indicatorComponent = rhythmHUD.AddComponent<MysticsItemsRhythmHUD>();
            indicatorComponent.canvasGroup = rhythmHUD.GetComponent<CanvasGroup>();
            indicatorComponent.slidingNote = rhythmHUD.transform.Find("Bar/Note").gameObject;
            indicatorComponent.comboText = rhythmHUD.transform.Find("ComboText").GetComponent<TextMeshProUGUI>();
            indicatorComponent.barTransform = rhythmHUD.transform.Find("Bar");
            indicatorComponent.comboTickAnimation = animationCurveHolder.startSize.curve;
            indicatorComponent.comboTickAnimationMultiplier = 2f;
            indicatorComponent.comboHitAnimation = animationCurveHolder.startLifetime.curve;
            indicatorComponent.comboHitAnimationMultiplier = 2f;
            
            /*
            RectTransform rectTransform = indicatorComponent.slidingNote.GetComponent<RectTransform>();
            var size = Mathf.Abs(rectTransform.localPosition.x) / readTime * beatWindowEarly * 2f;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            rectTransform = indicatorComponent.barTransform.Find("Judgement") as RectTransform;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size + 4f);
            */

            MusicBPMHelper.Init();

            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var component = sender.GetComponent<MysticsItemsRhythmBehaviour>();
            if (component)
            {
                args.critAdd += component.critBonus;
            }
        }

        public static class MusicBPMHelper
        {
            public static void Init()
            {
                On.RoR2.MusicController.UpdateState += MusicController_UpdateState;
            }

            private static void MusicController_UpdateState(On.RoR2.MusicController.orig_UpdateState orig, MusicController self)
            {
                orig(self);
                if (self.currentTrack && self.currentTrack != oldMusicTrackDef)
                {
                    oldMusicTrackDef = self.currentTrack;
                    if (songBPM.TryGetValue(self.currentTrack.cachedName, out var newBPM)) currentBPM = newBPM;
                    else currentBPM = 60;

                    foreach (var rhythmBehaviour in InstanceTracker.GetInstancesList<MysticsItemsRhythmBehaviour>())
                    {
                        rhythmBehaviour.totalTime = 0;
                        rhythmBehaviour.currentTime = 0;
                    }
                }
            }

            public static MusicTrackDef oldMusicTrackDef;
            public static float currentBPM = 60;

            public static Dictionary<string, float> songBPM = new Dictionary<string, float>()
            {
                // Vanilla
                { "muEscape", 50 }, // Escape track
                { "muFULLSong02", 50 }, // Into the Doldrums
                { "muFULLSong06", 60 }, // Nocturnal Emission
                { "muFULLSong07", 60 }, // Evapotranspiration
                { "muFULLSong18", 65 }, // Disdrometer
                { "muFULLSong19", 60 }, // Terra Pluviam
                { "muGameplayBase_09", 50 }, // They Might As Well Be Dead
                { "muIntroCutscene", 60 }, // The comment for this MusicTrackDef says "No music". Let's use the default 120 BPM here then
                { "muLogbook", 60 }, // The Dehydration of Risk of Rain 2
                { "muMainEndingFull", 50 }, // Lacrimosum
                { "muMainEndingOutroA", 50 }, // Also Lacrimosum I think?
                { "muMainEndingOutroB", 50 }, // ^
                { "muMenu", 60 }, // Risk of Rain 2
                { "muNone", 60 }, // No music. Using the default 120 BPM then
                { "muSong04", 60 }, // Parjanya
                { "muSong05", 53.5f }, // Thermodynamic Equilibrium
                { "muSong08", 76 }, // A Glacier Eventually Farts (And Don't You Listen to the Song of Life)
                { "muSong13", 55 }, // The Raindrop that Fell to the Sky
                { "muSong14", 65 }, // The Rain Formerly Known as Purple
                { "muSong16", 75 }, // Hydrophobia
                { "muSong21", 30 }, // Petrichor V. Doesn't have a "defined" rhythm I think, and it generally has a calm feel, so let's use 30 BPM
                { "muSong22", 60 }, // Köppen As Fuck
                { "muSong23", 80 }, // Antarctic Oscillation
                { "muSong24", 50 }, // ...con lentitud poderosa
                { "muSong25", 80 }, // You're Gonna Need a Bigger Ukulele
                // DLC1
                { "muBossfightDLC1_10", 81 }, // Having Fallen, It Was Blood
                { "muBossfightDLC1_12", 67 }, // A Boat Made from a Sheet of Newspaper
                { "muGameplayDLC1_01", 60 }, // Once in a Lullaby
                { "muGameplayDLC1_03", 48 }, // Out of Whose Womb Came the Ice?
                { "muGameplayDLC1_06", 48 }, // Who Can Fathom the Soundless Depths?
                { "muGameplayDLC1_08", 38 }, // A Placid Island of Ignorance. I'm not sure about this one, the rhythm is hard for me to catch
                { "muMenuDLC1", 48 }, // Prelude in D flat major. No idea about this one's BPM, let's use 48
                { "muRaidfightDLC1_07", 57 } // The Face of the Deep
            };
        }

        private void HUD_onHudTargetChangedGlobal(HUD obj)
        {
            MysticsItemsRhythmHUD.RefreshForHUDInstance(obj);
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            self.AddItemBehavior<MysticsItemsRhythmBehaviour>(self.inventory.GetItemCount(itemDef));
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody characterBody)
        {
            characterBody.onSkillActivatedAuthority += CharacterBody_onSkillActivatedAuthority;
        }

        private void CharacterBody_onSkillActivatedAuthority(GenericSkill skill)
        {
            var characterBody = skill.characterBody;
            if (characterBody)
            {
                var skillLocator = characterBody.skillLocator;
                if (skillLocator)
                {
                    var skillSlot = skillLocator.FindSkillSlot(skill);
                    if (skillSlot == SkillSlot.None || skillSlot == SkillSlot.Primary) return;

                    MysticsItemsRhythmBehaviour rhythmBehaviour = characterBody.GetComponent<MysticsItemsRhythmBehaviour>();
                    if (rhythmBehaviour) rhythmBehaviour.OnSkillActivatedAuthority();
                }
            }
        }

        public class MysticsItemsRhythmHUD : MonoBehaviour
        {
            public static void OnComboForInstance(MysticsItemsRhythmBehaviour rhythmBehaviour)
            {
                foreach (var instance in instancesList) if (instance.rhythmBehaviour == rhythmBehaviour)
                        instance.OnCombo();
            }

            public void OnCombo()
            {
                UpdateText();

                if (soundsEnabled && rhythmBehaviour && hud?.cameraRigController?.viewer?.localUser != null)
                {
                    Util.PlayAttackSpeedSound(MysticsItemsRhythmBehaviour.hitSoundString, hud.cameraRigController.targetBody?.gameObject, 1f + rhythmBehaviour.critBonus / 20f);
                }

                currentComboAnimationType = ComboAnimationType.Hit;
                comboAnimationTime = 0f;
                comboAnimationDuration = 0.1f;
            }

            public static void OnTickForInstance(MysticsItemsRhythmBehaviour rhythmBehaviour)
            {
                foreach (var instance in instancesList) if (instance.rhythmBehaviour == rhythmBehaviour)
                        instance.OnTick();
            }

            public void OnTick()
            {
                if (soundsEnabled && rhythmBehaviour && rhythmBehaviour.beatsSinceLastHit <= 5 && hud?.cameraRigController?.viewer?.localUser != null)
                {
                    Util.PlayAttackSpeedSound(MysticsItemsRhythmBehaviour.prepareSoundString, hud.cameraRigController.targetBody?.gameObject, 1f + rhythmBehaviour.critBonus / 20f);
                }
                if (currentComboAnimationType != ComboAnimationType.Hit)
                {
                    currentComboAnimationType = ComboAnimationType.Tick;
                    comboAnimationTime = 0f;
                    comboAnimationDuration = 0.1f;
                }
            }

            public static List<MysticsItemsRhythmHUD> instancesList = new List<MysticsItemsRhythmHUD>();

            public static void RefreshAll()
            {
                foreach (var hudInstance in HUD.readOnlyInstanceList)
                {
                    RefreshForHUDInstance(hudInstance);
                }
            }

            public static void RefreshForHUDInstance(HUD hudInstance)
            {
                CharacterMaster targetMaster = hudInstance.targetMaster;
                CharacterBody targetBody = targetMaster ? targetMaster.GetBody() : null;
                MysticsItemsRhythmBehaviour rhythmBehaviour = targetBody ? targetBody.GetComponent<MysticsItemsRhythmBehaviour>() : null;

                bool shouldDisplay = rhythmBehaviour;

                MysticsItemsRhythmHUD targetIndicatorInstance = instancesList.Where(x => x is MysticsItemsRhythmHUD).FirstOrDefault(x => x.hud == hudInstance);

                if (targetIndicatorInstance != shouldDisplay)
                {
                    if (!targetIndicatorInstance)
                    {
                        if (hudInstance.mainUIPanel)
                        {
                            var transform = (RectTransform)hudInstance.mainUIPanel.transform.Find("SpringCanvas/BottomRightCluster/Scaler");
                            if (transform)
                            {
                                targetIndicatorInstance = Instantiate(rhythmHUD, transform).GetComponent<MysticsItemsRhythmHUD>();
                                targetIndicatorInstance.hud = hudInstance;
                            }
                        }
                    }
                    else
                    {
                        Destroy(targetIndicatorInstance.gameObject);
                    }
                }

                if (shouldDisplay)
                {
                    targetIndicatorInstance.rhythmBehaviour = rhythmBehaviour;
                    targetIndicatorInstance.UpdateText();
                }
            }

            public void OnEnable()
            {
                instancesList.Add(this);
            }

            public void OnDisable()
            {
                instancesList.Remove(this);
            }

            public HUD hud;
            public MysticsItemsRhythmBehaviour rhythmBehaviour;
            public float selfDestructTimer = 5f;

            public void Update()
            {
                if (rhythmBehaviour)
                {
                    if (comboText)
                    {
                        if (currentComboAnimationType != ComboAnimationType.None)
                        {
                            var t = Mathf.Clamp01(comboAnimationTime / comboAnimationDuration);
                            comboAnimationTime += Time.deltaTime;
                            switch (currentComboAnimationType)
                            {
                                case ComboAnimationType.Tick:
                                    comboText.transform.localScale = Vector3.one * comboTickAnimation.Evaluate(t) * comboTickAnimationMultiplier;
                                    break;
                                case ComboAnimationType.Hit:
                                    comboText.transform.localScale = Vector3.one * comboHitAnimation.Evaluate(t) * comboHitAnimationMultiplier;
                                    break;
                            }
                            if (t >= 1f) currentComboAnimationType = ComboAnimationType.None;
                        }
                    }
                    if (slidingNote)
                    {
                        var pos = slidingNote.transform.localPosition;
                        pos.x = -60f + 120f * (Mathf.Abs(((rhythmBehaviour.totalTime + rhythmBehaviour.timePerBeat * 0.5f) % (rhythmBehaviour.timePerBeat * 2f)) - rhythmBehaviour.timePerBeat) / (rhythmBehaviour.timePerBeat));
                        slidingNote.transform.localPosition = pos;
                    }
                }
                else
                {
                    selfDestructTimer -= Time.deltaTime;
                    if (selfDestructTimer <= 0)
                    {
                        Destroy(gameObject);
                    }
                }
            }

            public void UpdateText()
            {
                if (comboText && comboText.enabled && rhythmBehaviour)
                    comboText.text = "+" + rhythmBehaviour.critBonus.ToString("0.##") + "%";
            }

            public CanvasGroup canvasGroup;
            public TextMeshProUGUI comboText;
            public Transform barTransform;
            public float comboAnimationTime;
            public float comboAnimationDuration;
            public AnimationCurve comboTickAnimation;
            public float comboTickAnimationMultiplier = 1f;
            public AnimationCurve comboHitAnimation;
            public float comboHitAnimationMultiplier = 1f;
            public enum ComboAnimationType
            {
                None,
                Tick,
                Hit
            }
            public ComboAnimationType currentComboAnimationType = ComboAnimationType.None;
            public GameObject slidingNote;
        }

        public class MysticsItemsRhythmBehaviour : CharacterBody.ItemBehavior
        {
            private float _critBonus;
            public float critBonus
            {
                get { return _critBonus; }
                set
                {
                    value = Mathf.Max(value, 0);
                    if (_critBonus != value)
                    {
                        _critBonus = value;
                        if (body.hasEffectiveAuthority)
                            new SyncRhythmBonus(gameObject.GetComponent<NetworkIdentity>().netId, value, NetworkServer.active).Send(NetworkServer.active ? NetworkDestination.Clients : NetworkDestination.Server);
                    }
                }
            }

            public class SyncRhythmBonus : INetMessage
            {
                NetworkInstanceId objID;
                float critBonus;
                bool isServer;

                public SyncRhythmBonus()
                {
                }

                public SyncRhythmBonus(NetworkInstanceId objID, float critBonus, bool isServer)
                {
                    this.objID = objID;
                    this.critBonus = critBonus;
                    this.isServer = isServer;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                    critBonus = reader.ReadSingle();
                    isServer = reader.ReadBoolean();
                }

                public void OnReceived()
                {
                    if (isServer == NetworkServer.active) return;
                    GameObject obj = Util.FindNetworkObject(objID);
                    if (obj)
                    {
                        MysticsItemsRhythmBehaviour component = obj.GetComponent<MysticsItemsRhythmBehaviour>();
                        if (component) component.critBonus = critBonus;
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                    writer.Write(critBonus);
                    writer.Write(isServer);
                }
            }

            public float timePerBeat
            {
                get
                {
                    return 1f / (MusicBPMHelper.currentBPM / 60f);
                }
            }
            
            public float currentTime = 0f;
            public float totalTime = 0f;

            public bool beatNotPressedYet = true;
            public int beatsSinceLastHit = 0;

            public void Start()
            {
                MysticsItemsRhythmHUD.RefreshAll();
            }

            public void Update()
            {
                var wasInBeatWindow = currentTime <= beatWindowLate;

                currentTime += Time.deltaTime;
                totalTime += Time.deltaTime;

                if (currentTime >= timePerBeat)
                {
                    currentTime -= timePerBeat;
                    beatsSinceLastHit++;
                    MysticsItemsRhythmHUD.OnTickForInstance(this);
                }

                if (body.hasEffectiveAuthority && wasInBeatWindow && currentTime > beatWindowLate)
                {
                    beatNotPressedYet = true;
                }
            }

            public bool IsOnBeat()
            {
                return currentTime >= (timePerBeat - beatWindowEarly) || currentTime <= (beatWindowLate);
            }

            public void OnSkillActivatedAuthority()
            {
                var isOnBeat = IsOnBeat();
                if (isOnBeat && beatNotPressedYet)
                {
                    critBonus += Rhythm.critBonus + Rhythm.critBonusPerStack * (float)(stack - 1);
                    beatsSinceLastHit = 0;
                    body.statsDirty = true;
                    beatNotPressedYet = false;
                    if (soundsEnabled)
                        Util.PlayAttackSpeedSound(hitSoundString, gameObject, 1f + critBonus / 60f);
                    MysticsItemsRhythmHUD.OnComboForInstance(this);
                }
                if (!isOnBeat && critLossEnabled)
                {
                    critBonus -= Rhythm.critBonus + Rhythm.critBonusPerStack * (float)(stack - 1);
                    body.statsDirty = true;
                    foreach (var instance in MysticsItemsRhythmHUD.instancesList) if (instance.rhythmBehaviour == this)
                            instance.UpdateText();
                }
            }

            public void OnEnable()
            {
                InstanceTracker.Add(this);
            }

            public void OnDisable()
            {
                InstanceTracker.Remove(this);
            }

            public static string beatSoundString = "MysticsItems_Play_item_proc_rhythm_beat";
            public static string hitSoundString = "MysticsItems_Play_item_proc_rhythm_hit";
            public static string prepareSoundString = "MysticsItems_Play_item_proc_rhythm_lower";
        }
    }
}
