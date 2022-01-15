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
using static MysticsItems.BalanceConfigManager;

namespace MysticsItems.Items
{
    public class Rhythm : BaseItem
    {
        public static GameObject rhythmHUDUnderCrosshair;
        public static GameObject rhythmHUDOverSkills;

        public static BepInEx.Configuration.ConfigEntry<bool> hudUnderCrosshair = Main.configGeneral.Bind("UI", "RhythmItemHUDUnderCrosshair", true, "Enable Metronome's HUD indicator under the crosshair.");
        public static BepInEx.Configuration.ConfigEntry<bool> hudOverSkills = Main.configGeneral.Bind("UI", "RhythmItemHUDOverSkills", true, "Enable Metronome's HUD indicator over skill cooldown icons.");
        public static BepInEx.Configuration.ConfigEntry<bool> hudComboText = Main.configGeneral.Bind("UI", "RhythmItemHUDComboText", true, "Enable the combo counter to Metronome's HUD indicators.");

        public static ConfigurableValue<float> interval = new ConfigurableValue<float>(
            "Item: Metronome",
            "Interval",
            45f,
            "How much time between each metronome beat & how long the buff lasts (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_RHYTHM_PICKUP",
                "ITEM_MYSTICSITEMS_RHYTHM_DESC"
            }
        );
        public static ConfigurableValue<float> hudAppearTime = new ConfigurableValue<float>(
            "Item: Metronome",
            "HUDAppearTime",
            7f,
            "How long should the HUD indiactors be visible on-screen before a beat (in seconds)"
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
            3,
            "How many low-pitch preparation ticks should play before a beat (in seconds)"
        );
        public static ConfigurableValue<float> prepareTickInterval = new ConfigurableValue<float>(
            "Item: Metronome",
            "PrepareTickInterval",
            0.5f,
            "How much time between each preparation tick (in seconds)"
        );
        public static ConfigurableValue<float> beatWindowEarly = new ConfigurableValue<float>(
            "Item: Metronome",
            "BeatWindowEarly",
            0.1f,
            "How early can you press to score a hit (in seconds)"
        );
        public static ConfigurableValue<float> beatWindowLate = new ConfigurableValue<float>(
            "Item: Metronome",
            "BeatWindowLate",
            0.2f,
            "How late can you press to score a hit (in seconds)"
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
            itemDef.tier = ItemTier.Tier2;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage,
                ItemTag.AIBlacklist
            };
            
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Metronome/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Metronome/Icon.png");
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "GunMeshR", new Vector3(-0.13951F, -0.01505F, 0.13151F), new Vector3(0F, 0F, 90F), new Vector3(0.00856F, 0.00856F, 0.00856F));
                AddDisplayRule("HuntressBody", "UpperArmL", new Vector3(0.06909F, 0.10681F, -0.00977F), new Vector3(3.66903F, 357.0302F, 178.0301F), new Vector3(0.01358F, 0.01358F, 0.01358F));
                AddDisplayRule("Bandit2Body", "MainWeapon", new Vector3(-0.05477F, 0.2274F, -0.04443F), new Vector3(359.4865F, 89.48757F, 206.7464F), new Vector3(0.0135F, 0.00485F, 0.00485F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(-1.77361F, 2.53066F, 1.76556F), new Vector3(0F, 90F, 90F), new Vector3(0.10065F, 0.10065F, 0.10065F));
                AddDisplayRule("EngiBody", "LowerArmR", new Vector3(0.0113F, 0.13437F, -0.05836F), new Vector3(1.34564F, 72.93568F, 188.458F), new Vector3(0.01476F, 0.01476F, 0.01476F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.035F, 0.89075F, -1.47928F), new Vector3(0F, 90F, 303.695F), new Vector3(0.07847F, 0.07847F, 0.07847F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.03562F, 1.40676F, -1.39837F), new Vector3(0F, 90F, 303.1705F), new Vector3(0.08093F, 0.09844F, 0.07912F));
                AddDisplayRule("MageBody", "Chest", new Vector3(-0.10398F, 0.07562F, -0.31389F), new Vector3(359.7522F, 90.11677F, 8.18118F), new Vector3(0.01236F, 0.01035F, 0.00964F));
                AddDisplayRule("MageBody", "Chest", new Vector3(0.11942F, 0.07423F, -0.30928F), new Vector3(359.136F, 95.88205F, 8.14244F), new Vector3(0.01236F, 0.01035F, 0.00787F));
                AddDisplayRule("MercBody", "HandL", new Vector3(0.01326F, 0.1146F, 0.04565F), new Vector3(88.10731F, 183.3846F, 89.99922F), new Vector3(0.00961F, 0.00961F, 0.00965F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(0.69564F, -0.5422F, -0.29426F), new Vector3(46.13942F, 241.7613F, 12.79626F), new Vector3(0.03647F, 0.03647F, 0.03647F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(0.01517F, -0.06288F, -0.17121F), new Vector3(90F, 90F, 0F), new Vector3(0.0207F, 0.0207F, 0.0207F));
                AddDisplayRule("CrocoBody", "SpineChest1", new Vector3(1.39693F, -0.10569F, -0.18201F), new Vector3(55.10429F, 175.6143F, 292.3791F), new Vector3(0.1379F, 0.1379F, 0.1379F));
                AddDisplayRule("CaptainBody", "MuzzleGun", new Vector3(0.00467F, 0.05642F, -0.1194F), new Vector3(357.9892F, 90.52832F, 89.76476F), new Vector3(0.05388F, 0.01322F, 0.0146F));
                AddDisplayRule("BrotherBody", "UpperArmL", BrotherInfection.green, new Vector3(0.06646F, 0.22781F, -0.00154F), new Vector3(77.05167F, 128.9087F, 289.6219F), new Vector3(0.04861F, 0.10534F, 0.10724F));
                AddDisplayRule("ScavBody", "Stomach", new Vector3(-0.92389F, 11.6509F, -5.90638F), new Vector3(20.93637F, 118.4181F, 332.9505F), new Vector3(0.24839F, 0.25523F, 0.24839F));
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
            indicatorComponent.comboHitAnimation = animationCurveHolder.startLifetime.curve;
            indicatorComponent.comboHitAnimationMultiplier = 2f;
            indicatorComponent.comboBreakAnimation = animationCurveHolder.startSpeed.curve;
            indicatorComponent.comboBreakAnimationMultiplier = 2f;
            indicatorComponent.noteFadeInAnimation = AnimationCurve.EaseInOut(0f, 0f, 0.3f, 1f);

            rhythmHUDOverSkills = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Metronome/RhythmHUDOverSkills.prefab");
            rhythmHUDOverSkills.AddComponent<HudElement>();
            MysticsItemsRhythmHUDOverSkills indicatorComponent2 = rhythmHUDOverSkills.AddComponent<MysticsItemsRhythmHUDOverSkills>();
            indicatorComponent2.canvasGroup = rhythmHUDOverSkills.GetComponent<CanvasGroup>();
            indicatorComponent2.overSkillSingleTemplate = rhythmHUDOverSkills.transform.Find("OverSkillSingleTemplate").gameObject;
            indicatorComponent2.noteTemplate = rhythmHUDOverSkills.transform.Find("OverSkillSingleTemplate/Offset/NoteTemplate").gameObject;
            indicatorComponent2.comboText = rhythmHUDOverSkills.transform.Find("ComboText").GetComponent<TextMeshProUGUI>();
            indicatorComponent2.comboHitAnimation = animationCurveHolder.startLifetime.curve;
            indicatorComponent2.comboHitAnimationMultiplier = 2f;
            indicatorComponent2.comboBreakAnimation = animationCurveHolder.startSpeed.curve;
            indicatorComponent2.comboBreakAnimationMultiplier = 5f;
            indicatorComponent2.noteFadeInAnimation = AnimationCurve.EaseInOut(0f, 0f, 0.3f, 1f);
        }

        private void HUD_onHudTargetChangedGlobal(HUD obj)
        {
            MysticsItemsRhythmHUD.RefreshAll();
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

            public static List<MysticsItemsRhythmHUD> instancesList = new List<MysticsItemsRhythmHUD>();

            public static void RefreshAll()
            {
                foreach (var hudInstance in HUD.readOnlyInstanceList)
                {
                    MysticsItemsRhythmHUDUnderCrosshair.RefreshForHUDInstance(hudInstance);
                    MysticsItemsRhythmHUDOverSkills.RefreshForHUDInstance(hudInstance);
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

            public float CalculateFadeAlpha()
            {
                if (rhythmBehaviour)
                {
                    if (rhythmBehaviour.currentTime >= (rhythmBehaviour.interval - Rhythm.hudAppearTime))
                    {
                        return Mathf.Clamp01(1f - (rhythmBehaviour.interval - Rhythm.hudAppearTime + Rhythm.hudFadeInTime - rhythmBehaviour.currentTime) / Rhythm.hudFadeInTime);
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
            public static void RefreshForHUDInstance(HUD hudInstance)
            {
                CharacterMaster targetMaster = hudInstance.targetMaster;
                CharacterBody targetBody = targetMaster ? targetMaster.GetBody() : null;
                MysticsItemsRhythmBehaviour rhythmBehaviour = targetBody ? targetBody.GetComponent<MysticsItemsRhythmBehaviour>() : null;

                bool shouldDisplay = hudUnderCrosshair.Value && rhythmBehaviour;
                
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
                        Destroy(targetIndicatorInstance);
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
                notes.Add(noteComponent);
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
                    comboText.enabled = Rhythm.hudComboText.Value;
                    if (comboText.enabled && rhythmBehaviour)
                        comboText.text = "x" + rhythmBehaviour.combo;
                }
            }

            public CanvasGroup canvasGroup;
            public TextMeshProUGUI comboText;
            public Transform barTransform;
            public float comboAnimationTime;
            public float comboAnimationDuration;
            public AnimationCurve comboHitAnimation;
            public float comboHitAnimationMultiplier = 1f;
            public AnimationCurve comboBreakAnimation;
            public float comboBreakAnimationMultiplier = 1f;
            public enum ComboAnimationType
            {
                None,
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
            public static void RefreshForHUDInstance(HUD hudInstance)
            {
                CharacterMaster targetMaster = hudInstance.targetMaster;
                CharacterBody targetBody = targetMaster ? targetMaster.GetBody() : null;
                MysticsItemsRhythmBehaviour rhythmBehaviour = targetBody ? targetBody.GetComponent<MysticsItemsRhythmBehaviour>() : null;

                bool shouldDisplay = hudOverSkills.Value && rhythmBehaviour;

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
                        Destroy(targetIndicatorInstance);
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
                    noteComponent.yOffset = (overSkillTransform.Find("Bar") as RectTransform).rect.height + (note.transform as RectTransform).rect.height;
                    noteComponent.noteFadeInAnimation = noteFadeInAnimation;
                    noteComponent.materialInstance = Material.Instantiate(note.GetComponent<Image>().material);
                    note.GetComponent<Image>().material = noteComponent.materialInstance;
                    notes.Add(noteComponent);
                }
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
                    comboText.enabled = Rhythm.hudComboText.Value;
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
            public AnimationCurve comboHitAnimation;
            public float comboHitAnimationMultiplier = 1f;
            public AnimationCurve comboBreakAnimation;
            public float comboBreakAnimationMultiplier = 1f;
            public enum ComboAnimationType
            {
                None,
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
                    pos.y = (remainingTime / maxTime) * yOffset;
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

            public float interval = Rhythm.interval;
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

            public float prepareTicks = 3;
            public int preparePhase = 9999;
            
            public void Start()
            {
                MysticsItemsRhythmHUD.RefreshAll();
                currentTime = interval - 8f;
            }

            public void Update()
            {
                var wasInBeatWindow = currentTime > beatWindowLate;

                currentTime += Time.deltaTime;

                if (body.hasEffectiveAuthority && !wasInBeatWindow && currentTime > beatWindowLate)
                {
                    if (beatNotPressedYet)
                    {
                        if (combo > 0)
                        {
                            combo = 0;
                            body.statsDirty = true;
                            currentTime = 0f;
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
                    currentTime = 0f;
                    readStarted = false;
                    preparePhase = Mathf.FloorToInt(interval);
                    Beat();
                }
                else
                {
                    if (beatNotPressedYet)
                    {
                        var newPreparePhase = Mathf.FloorToInt((interval - currentTime) / prepareTickInterval);
                        if (newPreparePhase < preparePhase)
                        {
                            preparePhase = newPreparePhase;
                            if (preparePhase <= (prepareTicks - 1))
                            {
                                Util.PlayAttackSpeedSound(prepareSoundString, gameObject, 1f + 0.1f * (combo - 1f));
                            }
                        }
                    }
                }
            }

            public void Beat()
            {
                Util.PlayAttackSpeedSound(beatSoundString, gameObject, 1f + 0.1f * (combo - 1f));
            }

            public bool IsOnBeat()
            {
                return currentTime >= (interval - beatWindowEarly) || currentTime <= (beatWindowLate);
            }

            public void OnSkillActivatedAuthority()
            {
                if (IsOnBeat())
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
