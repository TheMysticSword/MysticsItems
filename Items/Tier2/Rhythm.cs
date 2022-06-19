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
        public static GameObject rhythmHUDUnderCrosshair;
        public static GameObject rhythmHUDOverSkills;

        public static ConfigurableValue<float> interval = new ConfigurableValue<float>(
            "Item: Metronome",
            "Interval",
            30f,
            "How much time between each metronome beat & how long the buff lasts (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_RHYTHM_PICKUP",
                "ITEM_MYSTICSITEMS_RHYTHM_DESC"
            }
        );
        public static float hudFadeInTime = 0.8f;
        public static float hudFadeOutTime = 0.3f;
        public static ConfigurableValue<float> readTime = new ConfigurableValue<float>(
            "Item: Metronome",
            "ReadTime",
            2f,
            "How long should a note be visible on-screen before a beat (in seconds)"
        );
        public static ConfigurableValue<int> prepareTicks = new ConfigurableValue<int>(
            "Item: Metronome",
            "PrepareTicks",
            4,
            "How many low-pitch preparation ticks should play before a beat (in seconds)"
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

        public static ConfigOptions.ConfigurableValue<bool> rhythmUIUnderCrosshair = ConfigOptions.ConfigurableValue.CreateBool(
            ConfigManager.General.categoryGUID,
            ConfigManager.General.categoryName,
            ConfigManager.General.config,
            "UI",
            "Metronome UI (Under Crosshair)",
            true,
            "Enable Metronome's UI indicator under the crosshair"
        );
        public static ConfigOptions.ConfigurableValue<bool> rhythmUIOverSkills = ConfigOptions.ConfigurableValue.CreateBool(
            ConfigManager.General.categoryGUID,
            ConfigManager.General.categoryName,
            ConfigManager.General.config,
            "UI",
            "Metronome UI (Over Skills)",
            true,
            "Enable Metronome's UI indicator over skill cooldown icons"
        );
        public static ConfigOptions.ConfigurableValue<bool> rhythmUIComboText = ConfigOptions.ConfigurableValue.CreateBool(
            ConfigManager.General.categoryGUID,
            ConfigManager.General.categoryName,
            ConfigManager.General.config,
            "UI",
            "Metronome UI Combo Text",
            true,
            "Enable the combo counter near Metronome's UI indicators"
        );

        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            NetworkingAPI.RegisterMessageType<MysticsItemsRhythmBehaviour.SyncCombo>();
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

            rhythmHUDUnderCrosshair = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Metronome/RhythmHUDUnderCrosshair.prefab");
            rhythmHUDUnderCrosshair.AddComponent<HudElement>();
            MysticsItemsRhythmHUDUnderCrosshair indicatorComponent = rhythmHUDUnderCrosshair.AddComponent<MysticsItemsRhythmHUDUnderCrosshair>();
            indicatorComponent.canvasGroup = rhythmHUDUnderCrosshair.GetComponent<CanvasGroup>();
            indicatorComponent.noteTemplate = rhythmHUDUnderCrosshair.transform.Find("Bar/NoteLeft").gameObject;
            indicatorComponent.comboText = rhythmHUDUnderCrosshair.transform.Find("ComboText").GetComponent<TextMeshProUGUI>();
            indicatorComponent.barTransform = rhythmHUDUnderCrosshair.transform.Find("Bar");
            indicatorComponent.comboTickAnimation = animationCurveHolder.startSize.curve;
            indicatorComponent.comboTickAnimationMultiplier = 2f;
            indicatorComponent.comboHitAnimation = animationCurveHolder.startLifetime.curve;
            indicatorComponent.comboHitAnimationMultiplier = 2f;
            indicatorComponent.comboBreakAnimation = animationCurveHolder.startSpeed.curve;
            indicatorComponent.comboBreakAnimationMultiplier = 2f;
            indicatorComponent.noteFadeInAnimation = AnimationCurve.EaseInOut(0f, 0f, 0.3f, 1f);

            RectTransform rectTransform = indicatorComponent.noteTemplate.GetComponent<RectTransform>();
            var size = Mathf.Abs(rectTransform.localPosition.x) / readTime * beatWindowEarly * 2f;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            rectTransform = indicatorComponent.barTransform.Find("Judgement") as RectTransform;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size + 4f);

            rhythmHUDOverSkills = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Metronome/RhythmHUDOverSkills.prefab");
            rhythmHUDOverSkills.AddComponent<HudElement>();
            MysticsItemsRhythmHUDOverSkills indicatorComponent2 = rhythmHUDOverSkills.AddComponent<MysticsItemsRhythmHUDOverSkills>();
            indicatorComponent2.canvasGroup = rhythmHUDOverSkills.GetComponent<CanvasGroup>();
            indicatorComponent2.overSkillSingleTemplate = rhythmHUDOverSkills.transform.Find("OverSkillSingleTemplate").gameObject;
            indicatorComponent2.noteTemplate = rhythmHUDOverSkills.transform.Find("OverSkillSingleTemplate/Offset/NoteTemplate").gameObject;
            indicatorComponent2.comboText = rhythmHUDOverSkills.transform.Find("ComboText").GetComponent<TextMeshProUGUI>();
            indicatorComponent2.comboTickAnimation = animationCurveHolder.startSize.curve;
            indicatorComponent2.comboTickAnimationMultiplier = 2f;
            indicatorComponent2.comboHitAnimation = animationCurveHolder.startLifetime.curve;
            indicatorComponent2.comboHitAnimationMultiplier = 2f;
            indicatorComponent2.comboBreakAnimation = animationCurveHolder.startSpeed.curve;
            indicatorComponent2.comboBreakAnimationMultiplier = 5f;
            indicatorComponent2.noteFadeInAnimation = AnimationCurve.EaseInOut(0f, 0f, 0.3f, 1f);

            rectTransform = indicatorComponent2.noteTemplate.GetComponent<RectTransform>();
            size = Mathf.Abs((rectTransform.parent.Find("Bar") as RectTransform).rect.height) / readTime * beatWindowEarly;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
            rectTransform = rectTransform.parent.Find("Judgement") as RectTransform;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size + 4f);

            MusicBPMHelper.Init();
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
                }
            }

            public static MusicTrackDef oldMusicTrackDef;
            public static float currentBPM = 60;

            public static Dictionary<string, float> songBPM = new Dictionary<string, float>()
            {
                // Vanilla
                { "muEscape", 100 }, // Escape track
                { "muFULLSong02", 100 }, // Into the Doldrums
                { "muFULLSong06", 60 }, // Nocturnal Emission
                { "muFULLSong07", 60 }, // Evapotranspiration
                { "muFULLSong18", 130 }, // Disdrometer
                { "muFULLSong19", 60 }, // Terra Pluviam
                { "muGameplayBase_09", 100 }, // They Might As Well Be Dead
                { "muIntroCutscene", 120 }, // The comment for this MusicTrackDef says "No music". Let's use the default 120 BPM here then
                { "muLogbook", 60 }, // The Dehydration of Risk of Rain 2
                { "muMainEndingFull", 50 }, // Lacrimosum
                { "muMainEndingOutroA", 50 }, // Also Lacrimosum I think?
                { "muMainEndingOutroB", 50 }, // ^
                { "muMenu", 60 }, // Risk of Rain 2
                { "muNone", 120 }, // No music. Using the default 120 BPM then
                { "muSong04", 60 }, // Parjanya
                { "muSong05", 107 }, // Thermodynamic Equilibrium
                { "muSong08", 76 }, // A Glacier Eventually Farts (And Don't You Listen to the Song of Life)
                { "muSong13", 110 }, // The Raindrop that Fell to the Sky
                { "muSong14", 130 }, // The Rain Formerly Known as Purple
                { "muSong16", 150 }, // Hydrophobia
                { "muSong21", 30 }, // Petrichor V. Doesn't have a "defined" rhythm I think, and it generally has a calm feel, so let's use 30 BPM
                { "muSong22", 120 }, // Köppen As Fuck
                { "muSong23", 160 }, // Antarctic Oscillation
                { "muSong24", 50 }, // ...con lentitud poderosa
                { "muSong25", 160 }, // You're Gonna Need a Bigger Ukulele
                // DLC1
                { "muBossfightDLC1_10", 162 }, // Having Fallen, It Was Blood
                { "muBossfightDLC1_12", 134 }, // A Boat Made from a Sheet of Newspaper
                { "muGameplayDLC1_01", 60 }, // Once in a Lullaby
                { "muGameplayDLC1_03", 96 }, // Out of Whose Womb Came the Ice?
                { "muGameplayDLC1_06", 48 }, // Who Can Fathom the Soundless Depths?
                { "muGameplayDLC1_08", 76 }, // A Placid Island of Ignorance. I'm not sure about this one, the rhythm is hard for me to catch
                { "muMenuDLC1", 48 }, // Prelude in D flat major. No idea about this one's BPM, let's use 48
                { "muRaidfightDLC1_07", 114 } // The Face of the Deep
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

            public virtual void OnCombo() { }

            public static void OnComboBreakForInstance(MysticsItemsRhythmBehaviour rhythmBehaviour)
            {
                foreach (var instance in instancesList) if (instance.rhythmBehaviour == rhythmBehaviour)
                        instance.OnComboBreak();
            }

            public virtual void OnComboBreak() { }

            public static void OnBeginReadForInstance(MysticsItemsRhythmBehaviour rhythmBehaviour)
            {
                foreach (var instance in instancesList) if (instance.rhythmBehaviour == rhythmBehaviour)
                        instance.OnBeginRead();
            }

            public virtual void OnBeginRead() { }

            public static void OnTickForInstance(MysticsItemsRhythmBehaviour rhythmBehaviour)
            {
                foreach (var instance in instancesList) if (instance.rhythmBehaviour == rhythmBehaviour)
                        instance.OnTick();
            }

            public virtual void OnTick() { }

            public static List<MysticsItemsRhythmHUD> instancesList = new List<MysticsItemsRhythmHUD>();

            public static void RefreshAll()
            {
                foreach (var hudInstance in HUD.readOnlyInstanceList)
                {
                    MysticsItemsRhythmHUDUnderCrosshair.RefreshForHUDInstance(hudInstance);
                    MysticsItemsRhythmHUDOverSkills.RefreshForHUDInstance(hudInstance);
                }
            }

            public static void RefreshForHUDInstance(HUD hudInstance)
            {
                MysticsItemsRhythmHUDUnderCrosshair.RefreshForHUDInstance(hudInstance);
                MysticsItemsRhythmHUDOverSkills.RefreshForHUDInstance(hudInstance);
            }

            public void OnEnable()
            {
                instancesList.Add(this);
            }

            public void OnDisable()
            {
                instancesList.Remove(this);
            }

            public float CalculateFadeAlpha()
            {
                if (rhythmBehaviour)
                {
                    if (rhythmBehaviour.currentTime >= (rhythmBehaviour.interval - rhythmBehaviour.timePerMeasure * 1.4f))
                    {
                        return Mathf.Clamp01(1f - (rhythmBehaviour.interval - rhythmBehaviour.timePerMeasure * 1.4f + Rhythm.hudFadeInTime - rhythmBehaviour.currentTime) / Rhythm.hudFadeInTime);
                    }
                    if (rhythmBehaviour.currentTime >= rhythmBehaviour.beatWindowLate)
                    {
                        return Mathf.Clamp01(1f - (rhythmBehaviour.currentTime - rhythmBehaviour.beatWindowLate) / Rhythm.hudFadeOutTime);
                    }
                    else
                    {
                        return 1f;
                    }
                }
                return 0f;
            }

            public HUD hud;
            public MysticsItemsRhythmBehaviour rhythmBehaviour;
        }

        public class MysticsItemsRhythmHUDUnderCrosshair : MysticsItemsRhythmHUD
        {
            public static new void RefreshForHUDInstance(HUD hudInstance)
            {
                CharacterMaster targetMaster = hudInstance.targetMaster;
                CharacterBody targetBody = targetMaster ? targetMaster.GetBody() : null;
                MysticsItemsRhythmBehaviour rhythmBehaviour = targetBody ? targetBody.GetComponent<MysticsItemsRhythmBehaviour>() : null;

                bool shouldDisplay = rhythmUIUnderCrosshair.Value && rhythmBehaviour;
                
                MysticsItemsRhythmHUDUnderCrosshair targetIndicatorInstance = instancesList.Where(x => x is MysticsItemsRhythmHUDUnderCrosshair).Select(x => x as MysticsItemsRhythmHUDUnderCrosshair).FirstOrDefault(x => x.hud == hudInstance);

                if (targetIndicatorInstance != shouldDisplay)
                {
                    if (!targetIndicatorInstance)
                    {
                        if (hudInstance.mainUIPanel)
                        {
                            var transform = (RectTransform)hudInstance.mainUIPanel.transform.Find("SpringCanvas");
                            if (transform)
                            {
                                targetIndicatorInstance = Instantiate(rhythmHUDUnderCrosshair, transform).GetComponent<MysticsItemsRhythmHUDUnderCrosshair>();
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

            public void Update()
            {
                if (canvasGroup)
                {
                    canvasGroup.alpha = CalculateFadeAlpha();
                }

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
                            case ComboAnimationType.Break:
                                var pos = comboText.transform.localPosition;
                                pos.x = comboBreakAnimation.Evaluate(t) * comboBreakAnimationMultiplier;
                                comboText.transform.localPosition = pos;
                                break;
                        }
                        if (t >= 1f) currentComboAnimationType = ComboAnimationType.None;
                    }
                }
            }

            public override void OnBeginRead()
            {
                base.OnBeginRead();
                var note = Instantiate(noteTemplate, barTransform);
                note.SetActive(true);
                MysticsItemsRhythmHUDUnderCrosshairNote noteComponent = note.AddComponent<MysticsItemsRhythmHUDUnderCrosshairNote>();
                noteComponent.maxTime = noteComponent.remainingTime = rhythmBehaviour.readTime;
                noteComponent.right = false;
                note.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                noteComponent.xOffset = 60f;
                noteComponent.noteFadeInAnimation = noteFadeInAnimation;
                noteComponent.materialInstance = Material.Instantiate(note.GetComponent<Image>().material);
                note.GetComponent<Image>().material = noteComponent.materialInstance;
                noteComponent.UpdateAlpha();
                notes.Add(noteComponent);

                note = Instantiate(noteTemplate, barTransform);
                note.SetActive(true);
                noteComponent = note.AddComponent<MysticsItemsRhythmHUDUnderCrosshairNote>();
                noteComponent.maxTime = noteComponent.remainingTime = rhythmBehaviour.readTime;
                noteComponent.right = true;
                note.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                noteComponent.xOffset = 60f;
                noteComponent.noteFadeInAnimation = noteFadeInAnimation;
                noteComponent.materialInstance = Material.Instantiate(note.GetComponent<Image>().material);
                note.GetComponent<Image>().material = noteComponent.materialInstance;
                noteComponent.UpdateAlpha();
                notes.Add(noteComponent);
            }

            public override void OnTick()
            {
                base.OnTick();
                
                currentComboAnimationType = ComboAnimationType.Tick;
                comboAnimationTime = 0f;
                comboAnimationDuration = 0.1f;
            }

            public override void OnCombo()
            {
                base.OnCombo();
                UpdateText();
                
                currentComboAnimationType = ComboAnimationType.Hit;
                comboAnimationTime = 0f;
                comboAnimationDuration = 0.1f;

                notes.RemoveAll(x => x == null);
                foreach (var note in notes.Where(x => x.remainingTime <= rhythmBehaviour.beatWindowEarly))
                {
                    Destroy(note.gameObject);
                }
            }

            public override void OnComboBreak()
            {
                base.OnComboBreak();
                UpdateText();

                currentComboAnimationType = ComboAnimationType.Break;
                comboAnimationTime = 0f;
                comboAnimationDuration = 0.2f;
            }

            public void UpdateText()
            {
                if (comboText)
                {
                    comboText.enabled = rhythmUIComboText.Value;
                    if (comboText.enabled && rhythmBehaviour)
                        comboText.text = "x" + rhythmBehaviour.combo;
                }
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
            public AnimationCurve comboBreakAnimation;
            public float comboBreakAnimationMultiplier = 1f;
            public enum ComboAnimationType
            {
                None,
                Tick,
                Hit,
                Break
            }
            public ComboAnimationType currentComboAnimationType = ComboAnimationType.None;
            public GameObject noteTemplate;
            public List<MysticsItemsRhythmHUDUnderCrosshairNote> notes;
            public AnimationCurve noteFadeInAnimation;

            public class MysticsItemsRhythmHUDUnderCrosshairNote : MonoBehaviour
            {
                public float maxTime;
                public float remainingTime;
                public float xOffset = 60f;
                public bool right;
                public RectTransform rectTransform;
                public AnimationCurve noteFadeInAnimation;
                public Material materialInstance;
                public bool destroyMaterialInstanceOnDestroy = true;

                public void Awake()
                {
                    rectTransform = transform as RectTransform;
                    UpdateAlpha();
                }

                public void Update()
                {
                    var pos = transform.localPosition;
                    pos.x = (remainingTime / maxTime) * xOffset * (right ? 1 : -1);
                    transform.localPosition = pos;

                    remainingTime -= Time.deltaTime;

                    if (materialInstance)
                    {
                        UpdateAlpha();

                        Vector4 renderPortion;
                        if (right)
                        {
                            var a = Mathf.Clamp01(1f - (pos.x + rectTransform.rect.width * 0.5f) / rectTransform.rect.width);
                            if (a >= 1f)
                            {
                                Destroy(gameObject);
                                return;
                            }
                            renderPortion = new Vector4(a, 1f, 0f, 1f);
                        }
                        else
                        {
                            var a = Mathf.Clamp01(-(pos.x - rectTransform.rect.width * 0.5f) / rectTransform.rect.width);
                            if (a <= 0f)
                            {
                                Destroy(gameObject);
                                return;
                            }
                            renderPortion = new Vector4(0f, a, 0f, 1f);
                        }
                        materialInstance.SetVector("_RenderPortion", renderPortion);
                    }
                }

                public void UpdateAlpha()
                {
                    if (materialInstance)
                    {
                        var col = materialInstance.GetColor("_Color");
                        col.a = noteFadeInAnimation.Evaluate(1f - (remainingTime / maxTime));
                        materialInstance.SetColor("_Color", col);
                    }
                }

                public void OnDestroy()
                {
                    if (destroyMaterialInstanceOnDestroy)
                    {
                        Destroy(materialInstance);
                    }
                }
            }
        }


        public class MysticsItemsRhythmHUDOverSkills : MysticsItemsRhythmHUD
        {
            public static new void RefreshForHUDInstance(HUD hudInstance)
            {
                CharacterMaster targetMaster = hudInstance.targetMaster;
                CharacterBody targetBody = targetMaster ? targetMaster.GetBody() : null;
                MysticsItemsRhythmBehaviour rhythmBehaviour = targetBody ? targetBody.GetComponent<MysticsItemsRhythmBehaviour>() : null;

                bool shouldDisplay = rhythmUIOverSkills.Value && rhythmBehaviour;

                MysticsItemsRhythmHUDOverSkills targetIndicatorInstance = instancesList.Where(x => x is MysticsItemsRhythmHUDOverSkills).Select(x => x as MysticsItemsRhythmHUDOverSkills).FirstOrDefault(x => x.hud == hudInstance);

                if (targetIndicatorInstance != shouldDisplay)
                {
                    if (!targetIndicatorInstance)
                    {
                        if (hudInstance.mainUIPanel)
                        {
                            var transform = (RectTransform)hudInstance.mainUIPanel.transform.Find("SpringCanvas/BottomRightCluster/Scaler");
                            if (transform)
                            {
                                targetIndicatorInstance = Instantiate(rhythmHUDOverSkills, transform).GetComponent<MysticsItemsRhythmHUDOverSkills>();
                                targetIndicatorInstance.transform.localScale = Vector3.one;
                                targetIndicatorInstance.hud = hudInstance;
                                targetIndicatorInstance.overSkillSingleTransforms = new Transform[] { };
                                HG.ArrayUtils.ArrayAppend(ref targetIndicatorInstance.disconnectedCanvasGroups, targetIndicatorInstance.comboText.GetComponent<CanvasGroup>());
                                targetIndicatorInstance.comboText.transform.SetParent(hudInstance.mainUIPanel.transform.Find("SpringCanvas/BottomRightCluster/Scaler/Skill1Root"), false);
                                var arr = new Transform[]
                                {
                                    hudInstance.mainUIPanel.transform.Find("SpringCanvas/BottomRightCluster/Scaler/Skill2Root"),
                                    hudInstance.mainUIPanel.transform.Find("SpringCanvas/BottomRightCluster/Scaler/Skill3Root"),
                                    hudInstance.mainUIPanel.transform.Find("SpringCanvas/BottomRightCluster/Scaler/Skill4Root")
                                };
                                foreach (var skillRoot in arr)
                                {
                                    var overSkillSingle = Instantiate(targetIndicatorInstance.overSkillSingleTemplate, skillRoot);
                                    overSkillSingle.transform.localScale = Vector3.one;
                                    overSkillSingle.SetActive(true);
                                    HG.ArrayUtils.ArrayAppend(ref targetIndicatorInstance.overSkillSingleTransforms, overSkillSingle.transform.Find("Offset"));
                                    HG.ArrayUtils.ArrayAppend(ref targetIndicatorInstance.disconnectedCanvasGroups, overSkillSingle.GetComponent<CanvasGroup>());
                                }
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

            public void Update()
            {
                if (canvasGroup)
                {
                    var a = CalculateFadeAlpha();
                    canvasGroup.alpha = a;
                    foreach (var disconnectedCanvasGroup in disconnectedCanvasGroups)
                    {
                        if (disconnectedCanvasGroup) disconnectedCanvasGroup.alpha = a;
                    }
                }

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
                            case ComboAnimationType.Break:
                                var pos = comboText.transform.localPosition;
                                pos.x = comboBreakAnimation.Evaluate(t) * comboBreakAnimationMultiplier;
                                comboText.transform.localPosition = pos;
                                break;
                        }
                        if (t >= 1f) currentComboAnimationType = ComboAnimationType.None;
                    }
                }
            }

            public override void OnBeginRead()
            {
                base.OnBeginRead();
                foreach (var overSkillTransform in overSkillSingleTransforms)
                {
                    var note = Instantiate(noteTemplate, overSkillTransform);
                    note.SetActive(true);
                    MysticsItemsRhythmHUDOverSkillsNote noteComponent = note.AddComponent<MysticsItemsRhythmHUDOverSkillsNote>();
                    noteComponent.maxTime = noteComponent.remainingTime = rhythmBehaviour.readTime;
                    noteComponent.yOffset = (overSkillTransform.Find("Bar") as RectTransform).rect.height;
                    noteComponent.noteFadeInAnimation = noteFadeInAnimation;
                    noteComponent.materialInstance = Material.Instantiate(note.GetComponent<Image>().material);
                    note.GetComponent<Image>().material = noteComponent.materialInstance;
                    noteComponent.UpdateAlpha();
                    notes.Add(noteComponent);
                }
            }

            public override void OnTick()
            {
                base.OnTick();

                currentComboAnimationType = ComboAnimationType.Tick;
                comboAnimationTime = 0f;
                comboAnimationDuration = 0.1f;
            }

            public override void OnCombo()
            {
                base.OnCombo();
                UpdateText();

                currentComboAnimationType = ComboAnimationType.Hit;
                comboAnimationTime = 0f;
                comboAnimationDuration = 0.1f;

                notes.RemoveAll(x => x == null);
                foreach (var note in notes.Where(x => x.remainingTime <= rhythmBehaviour.beatWindowEarly))
                {
                    Destroy(note.gameObject);
                }
            }

            public override void OnComboBreak()
            {
                base.OnComboBreak();
                UpdateText();

                currentComboAnimationType = ComboAnimationType.Break;
                comboAnimationTime = 0f;
                comboAnimationDuration = 0.2f;
            }

            public void UpdateText()
            {
                if (comboText)
                {
                    comboText.enabled = rhythmUIComboText.Value;
                    if (comboText.enabled && rhythmBehaviour)
                        comboText.text = "x" + rhythmBehaviour.combo;
                }
            }

            public CanvasGroup canvasGroup;
            public TextMeshProUGUI comboText;
            public GameObject overSkillSingleTemplate;
            public Transform[] overSkillSingleTransforms;
            public CanvasGroup[] disconnectedCanvasGroups = new CanvasGroup[] { };
            public float comboAnimationTime;
            public float comboAnimationDuration;
            public AnimationCurve comboTickAnimation;
            public float comboTickAnimationMultiplier = 1f;
            public AnimationCurve comboHitAnimation;
            public float comboHitAnimationMultiplier = 1f;
            public AnimationCurve comboBreakAnimation;
            public float comboBreakAnimationMultiplier = 1f;
            public enum ComboAnimationType
            {
                None,
                Tick,
                Hit,
                Break
            }
            public ComboAnimationType currentComboAnimationType = ComboAnimationType.None;
            public GameObject noteTemplate;
            public List<MysticsItemsRhythmHUDOverSkillsNote> notes;
            public AnimationCurve noteFadeInAnimation;

            public class MysticsItemsRhythmHUDOverSkillsNote : MonoBehaviour
            {
                public float maxTime;
                public float remainingTime;
                public float yOffset = 60f;
                public RectTransform rectTransform;
                public AnimationCurve noteFadeInAnimation;
                public Material materialInstance;
                public bool destroyMaterialInstanceOnDestroy = true;

                public void Awake()
                {
                    rectTransform = transform as RectTransform;
                    UpdateAlpha();
                }

                public void Update()
                {
                    var pos = transform.localPosition;
                    pos.y = (remainingTime / maxTime) * yOffset + rectTransform.rect.height;
                    transform.localPosition = pos;

                    remainingTime -= Time.deltaTime;

                    if (materialInstance)
                    {
                        UpdateAlpha();

                        Vector4 renderPortion;
                        var a = Mathf.Clamp01(1f - (pos.y + rectTransform.rect.height * 0.5f) / rectTransform.rect.height);
                        if (a >= 1f)
                        {
                            Destroy(gameObject);
                            return;
                        }
                        renderPortion = new Vector4(0f, 1f, a, 1f);
                        materialInstance.SetVector("_RenderPortion", renderPortion);
                    }
                }

                public void UpdateAlpha()
                {
                    if (materialInstance)
                    {
                        var col = materialInstance.GetColor("_Color");
                        col.a = noteFadeInAnimation.Evaluate(1f - (remainingTime / maxTime));
                        materialInstance.SetColor("_Color", col);
                    }
                }

                public void OnDestroy()
                {
                    if (destroyMaterialInstanceOnDestroy)
                    {
                        Destroy(materialInstance);
                    }
                }
            }
        }

        public class MysticsItemsRhythmBehaviour : CharacterBody.ItemBehavior
        {
            private int _combo;
            public int combo
            {
                get { return _combo; }
                set
                {
                    value = Mathf.Max(value, 0);
                    if (_combo != value)
                    {
                        var difference = value - _combo;
                        _combo = value;
                        if (NetworkServer.active)
                        {
                            if (difference > 0)
                            {
                                for (var i = 0; i < difference; i++)
                                    body.AddBuff(MysticsItemsContent.Buffs.MysticsItems_RhythmCombo);
                            }
                            else
                            {
                                for (var i = 0; i < Math.Abs(difference); i++)
                                    if (body.HasBuff(MysticsItemsContent.Buffs.MysticsItems_RhythmCombo))
                                        body.RemoveBuff(MysticsItemsContent.Buffs.MysticsItems_RhythmCombo);
                            }
                        }
                        new SyncCombo(gameObject.GetComponent<NetworkIdentity>().netId, value).Send(NetworkServer.active ? NetworkDestination.Clients : NetworkDestination.Server);
                    }
                }
            }
            
            public class SyncCombo : INetMessage
            {
                NetworkInstanceId objID;
                int combo;

                public SyncCombo()
                {
                }

                public SyncCombo(NetworkInstanceId objID, int combo)
                {
                    this.objID = objID;
                    this.combo = combo;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                    combo = reader.ReadInt32();
                }

                public void OnReceived()
                {
                    GameObject obj = Util.FindNetworkObject(objID);
                    if (obj)
                    {
                        MysticsItemsRhythmBehaviour component = obj.GetComponent<MysticsItemsRhythmBehaviour>();
                        if (component) component.combo = combo;
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                    writer.Write(combo);
                }
            }

            public float timePerBeat
            {
                get
                {
                    return 1f / (MusicBPMHelper.currentBPM / 60f);
                }
            }
            public float timePerMeasure
            {
                get
                {
                    return timePerBeat * (float)(Rhythm.prepareTicks + 1);
                }
            }
            public float interval
            {
                get
                {
                    var calculatedMeasures = Mathf.Round(Rhythm.interval / timePerMeasure);
                    return timePerMeasure * calculatedMeasures;
                }
            }
            private float _readTime = Rhythm.readTime;
            public float readTime
            {
                get { return _readTime; }
                set
                {
                    _readTime = Mathf.Min(value, interval);
                }
            }
            public float beatWindowEarly = Rhythm.beatWindowEarly;
            public float beatWindowLate = Rhythm.beatWindowLate;

            public float currentTime = 0f;
            public bool readStarted = false;

            public bool beatNotPressedYet = true;

            public float prepareTicks = Rhythm.prepareTicks;
            public int preparePhase = 9999;
            
            public void Start()
            {
                MysticsItemsRhythmHUD.RefreshAll();
                currentTime = interval - timePerMeasure * 2f;
            }

            public void Update()
            {
                var wasInBeatWindow = currentTime <= beatWindowLate;

                currentTime += Time.deltaTime;

                if (body.hasEffectiveAuthority && wasInBeatWindow && currentTime > beatWindowLate)
                {
                    if (beatNotPressedYet)
                    {
                        if (combo > 0)
                        {
                            combo = 0;
                            body.statsDirty = true;
                            MysticsItemsRhythmHUD.OnComboBreakForInstance(this);
                        }
                    }
                    beatNotPressedYet = true;
                }

                if (!readStarted && currentTime > (interval - readTime))
                {
                    readStarted = true;
                    MysticsItemsRhythmHUD.OnBeginReadForInstance(this);
                }

                if (currentTime >= interval)
                {
                    currentTime -= interval;
                    readStarted = false;
                    preparePhase = Mathf.FloorToInt(interval);
                    Beat();
                }
                else
                {
                    if (beatNotPressedYet)
                    {
                        var newPreparePhase = Mathf.FloorToInt((interval - currentTime) / timePerBeat);
                        if (newPreparePhase < preparePhase)
                        {
                            preparePhase = newPreparePhase;
                            if (preparePhase <= (prepareTicks - 1))
                            {
                                if (body.hasEffectiveAuthority) Util.PlayAttackSpeedSound(prepareSoundString, gameObject, 1f + 0.1f * (combo - 1f));
                                MysticsItemsRhythmHUD.OnTickForInstance(this);
                            }
                        }
                    }
                }
            }

            public void Beat()
            {
                if (body.hasEffectiveAuthority) Util.PlayAttackSpeedSound(beatSoundString, gameObject, 1f + 0.1f * (combo - 1f));
            }

            public bool IsOnBeat()
            {
                return currentTime >= (interval - beatWindowEarly) || currentTime <= (beatWindowLate);
            }

            public void OnSkillActivatedAuthority()
            {
                if (IsOnBeat() && beatNotPressedYet)
                {
                    combo++;
                    body.statsDirty = true;
                    beatNotPressedYet = false;
                    Util.PlayAttackSpeedSound(hitSoundString, gameObject, 1f + 0.1f * (combo - 1f));
                    MysticsItemsRhythmHUD.OnComboForInstance(this);
                }
            }

            public static string beatSoundString = "MysticsItems_Play_item_proc_rhythm_beat";
            public static string hitSoundString = "MysticsItems_Play_item_proc_rhythm_hit";
            public static string prepareSoundString = "MysticsItems_Play_item_proc_rhythm_lower";
        }
    }
}
