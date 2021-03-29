using BepInEx;
using RoR2;
using R2API;
using R2API.Utils;
using R2API.Networking;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency(SoftDependencies.ItemStatsSoftDependency.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(NetworkingAPI), nameof(PrefabAPI), nameof(ResourcesAPI), nameof(SoundAPI))]

    public class MysticsItemsPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "com.themysticsword.mysticsitems";
        public const string PluginName = "MysticsItems";
        public const string PluginVersion = "1.1.4";

        internal static BepInEx.Logging.ManualLogSource logger;

        public void Awake()
        {
            logger = Logger;
            Main.Init();
        }
    }

    public static partial class Main
    {
        public const string TokenPrefix = MysticsItemsPlugin.PluginName + "_";
        
        public const string AssetPrefix = "@" + MysticsItemsPlugin.PluginName;
        public const string AssetPathRoot = "Assets/";
        public static AssetBundle AssetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("MysticsItems.mysticsitemsunityassetbundle"));

        internal const BindingFlags bindingFlagAll = (BindingFlags)(-1);
        internal static BepInEx.Logging.ManualLogSource logger;

        internal static bool isDedicatedServer = Application.isBatchMode;

        public static void Init()
        {
            logger = MysticsItemsPlugin.logger;
            ResourcesAPI.AddProvider(new AssetBundleResourcesProvider(AssetPrefix, AssetBundle));

            using (var soundBankStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MysticsItems.MysticsItemsWwiseSoundbank.bnk"))
            {
                var bytes = new byte[soundBankStream.Length];
                soundBankStream.Read(bytes, 0, bytes.Length);
                SoundAPI.SoundBanks.Add(bytes);
            }

            RoR2Application.onLoad += PostGameLoad;

            //DebugTools.Init();

            Achievements.BaseAchievement.Init();
            CharacterStats.Init();
            Items.BaseItem.Init();
            //Items.CharacterItems.Init();
            Equipment.BaseEquipment.Init();
            LanguageLoader.Init();
            Outlines.Init();
            Overlays.Init();
            PlainHologram.Init();

            //LaserTurret.Init();
            ShrineLegendary.Init();

            // Auto-load classes
            System.Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                if (!type.IsAbstract)
                {
                    if (type.BaseType == typeof(Achievements.BaseAchievement))
                    {
                        Achievements.BaseAchievement achievement = (Achievements.BaseAchievement)System.Activator.CreateInstance(type);
                        achievement.Add();
                    }
                }
            }

            // Load the content pack
            On.RoR2.ContentManager.SetContentPacks += (orig, newContentPacks) =>
            {
                newContentPacks.Add(new MysticsItemsContent());
                orig(newContentPacks);
            };

            // Hooks for easier handling of item effects
            On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, damageInfo, victim) =>
            {
                orig(self, damageInfo, victim);
                if (damageInfo.attacker && damageInfo.procCoefficient > 0f)
                {
                    CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    CharacterBody victimBody = victim ? victim.GetComponent<CharacterBody>() : null;
                    GenericCharacterInfo attackerInfo = new GenericCharacterInfo(attackerBody);
                    GenericCharacterInfo victimInfo = new GenericCharacterInfo(victimBody);
                    if (attackerBody && victimBody && OnHitEnemy != null) OnHitEnemy(damageInfo, attackerInfo, victimInfo);
                }
            };

            On.RoR2.GlobalEventManager.OnHitAll += (orig, self, damageInfo, hitObject) =>
            {
                orig(self, damageInfo, hitObject);
                if (damageInfo.attacker)
                {
                    CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    GenericCharacterInfo attackerInfo = new GenericCharacterInfo(attackerBody);
                    if (attackerBody && OnHitAll != null) OnHitAll(damageInfo, attackerInfo);
                }
            };

            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                    MoveType.AfterLabel,
                    x => x.MatchLdarg(1),
                    x => x.MatchLdfld<DamageInfo>("damage"),
                    x => x.MatchStloc(5)
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldarg_1);
                    c.Emit(OpCodes.Ldloc_1);
                    c.EmitDelegate<System.Action<HealthComponent, DamageInfo, CharacterBody>>((healthComponent, damageInfo, attackerBody) =>
                    {
                        CharacterBody victimBody = healthComponent.body;
                        GenericCharacterInfo attackerInfo = new GenericCharacterInfo(attackerBody);
                        GenericCharacterInfo victimInfo = new GenericCharacterInfo(victimBody);
                        if (attackerBody && BeforeDealDamage != null) BeforeDealDamage(damageInfo, attackerInfo);
                        if (victimBody && BeforeTakeDamage != null) BeforeTakeDamage(damageInfo, victimInfo);
                    });
                }
                ILLabel label = null;
                if (c.TryGotoNext(
                    x => x.MatchLdcR4(100f),
                    x => x.MatchMul(),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<HealthComponent>("body"),
                    x => x.MatchCallOrCallvirt<CharacterBody>("get_master"),
                    x => x.MatchCallOrCallvirt(typeof(Util), "CheckRoll"),
                    x => x.MatchBrfalse(out label)
                ))
                {
                    c.GotoLabel(label);
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldarg_1);
                    c.Emit(OpCodes.Ldloc_1);
                    c.EmitDelegate<System.Action<HealthComponent, DamageInfo, CharacterBody>>((healthComponent, damageInfo, attackerBody) =>
                    {
                        CharacterBody victimBody = healthComponent.body;
                        GenericCharacterInfo attackerInfo = new GenericCharacterInfo(attackerBody);
                        GenericCharacterInfo victimInfo = new GenericCharacterInfo(victimBody);
                        if (victimBody && OnTakeDamage != null) OnTakeDamage(damageInfo, victimInfo);
                    });
                }
            };
        }

        public static void PostGameLoad()
        {
            LanguageLoader.Load("MysticsItems.language");

            Items.BaseItem.PostGameLoad();
            
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(SoftDependencies.ItemStatsSoftDependency.PluginGUID)) SoftDependencies.ItemStatsSoftDependency.Init();
        }

        [ConCommand(commandName = Main.TokenPrefix + "unlocklogs", flags = ConVarFlags.Cheat, helpText = "Unlocks all logbook entries")]
        private static void CCUnlockLogs(ConCommandArgs args)
        {
            foreach (LocalUser user in LocalUserManager.readOnlyLocalUsersList)
            {
                foreach (Items.BaseItem item in Items.BaseItem.loadedItems)
                {
                    user.userProfile.DiscoverPickup(item.GetPickupIndex());
                }
                foreach (Equipment.BaseEquipment equipment in Equipment.BaseEquipment.loadedEquipment)
                {
                    user.userProfile.DiscoverPickup(equipment.GetPickupIndex());
                }
            }
        }

        [ConCommand(commandName = Main.TokenPrefix + "grantall", flags = ConVarFlags.None, helpText = "Grant all achievements")]
        private static void CCGrantAll(ConCommandArgs args)
        {
            foreach (LocalUser user in LocalUserManager.readOnlyLocalUsersList)
            {
                foreach (Achievements.BaseAchievement achievement in Achievements.BaseAchievement.registeredAchievements)
                {
                    AchievementManager.GetUserAchievementManager(user).GrantAchievement(achievement.achievementDef);
                }
            }
        }

        public struct GenericCharacterInfo
        {
            public GameObject gameObject;
            public CharacterBody body;
            public CharacterMaster master;
            public TeamComponent teamComponent;
            public HealthComponent healthComponent;
            public Inventory inventory;
            public TeamIndex teamIndex;
            public Vector3 aimOrigin;

            public GenericCharacterInfo(CharacterBody body)
            {
                this.body = body;
                gameObject = body ? body.gameObject : null;
                master = body ? body.master : null;
                teamComponent = body ? body.teamComponent : null;
                healthComponent = body ? body.healthComponent : null;
                inventory = master ? master.inventory : null;
                teamIndex = teamComponent ? teamComponent.teamIndex : TeamIndex.Neutral;
                aimOrigin = body ? body.aimOrigin : Random.insideUnitSphere.normalized;
            }
        }

        public static event System.Action<DamageInfo, GenericCharacterInfo, GenericCharacterInfo> OnHitEnemy;
        public static event System.Action<DamageInfo, GenericCharacterInfo> OnHitAll;
        public static event System.Action<DamageInfo, GenericCharacterInfo> BeforeDealDamage;
        public static event System.Action<DamageInfo, GenericCharacterInfo> BeforeTakeDamage;
        public static event System.Action<DamageInfo, GenericCharacterInfo> OnTakeDamage;

        public static List<GameObject> modifiedPrefabs = new List<GameObject>(); // Add to this list when modifying a base game prefab to keep Unity from destroying modified prefabs from cache

        public static class HopooShaderToMaterial
        {
            public struct Properties
            {
                public Dictionary<string, float> floats;
                public Dictionary<string, Color> colors;
                public Dictionary<string, Texture> textures;
            }

            public static void Apply(Material mat, Shader shader, Properties properties = default(Properties))
            {
                mat.shader = shader;
                if (properties.floats != null) foreach (KeyValuePair<string, float> keyValuePair in properties.floats) mat.SetFloat(keyValuePair.Key, keyValuePair.Value);
                if (properties.colors != null) foreach (KeyValuePair<string, Color> keyValuePair in properties.colors) mat.SetColor(keyValuePair.Key, keyValuePair.Value);
                if (properties.textures != null) foreach (KeyValuePair<string, Texture> keyValuePair in properties.textures) mat.SetTexture(keyValuePair.Key, keyValuePair.Value);
            }

            public class Standard
            {
                public static Shader shader = Resources.Load<Shader>("shaders/deferred/hgstandard");

                public static void Apply(Material mat, Properties properties = default(Properties))
                {
                    HopooShaderToMaterial.Apply(mat, shader, properties);
                    mat.SetTexture("_NormalTex", mat.GetTexture("_BumpMap"));
                    mat.SetTexture("_EmTex", mat.GetTexture("_EmissionMap"));
                }
                public static void Apply(params Material[] mats)
                {
                    foreach (Material mat in mats) Apply(mat);
                }

                public static void DisableEverything(Material mat)
                {
                    mat.DisableKeyword("DITHER");
                    mat.SetFloat("_DitherOn", 0f);
                    mat.DisableKeyword("FORCE_SPEC");
                    mat.SetFloat("_SpecularHighlights", 0f);
                    mat.SetFloat("_SpecularStrength", 0f);
                    mat.DisableKeyword("_EMISSION");
                    mat.SetFloat("_EmPower", 0f);
                    mat.SetColor("_EmColor", new Color(0f, 0f, 0f, 1f));
                    mat.DisableKeyword("FRESNEL_EMISSION");
                    mat.SetFloat("_FresnelBoost", 0f);
                }

                public static void Dither(Material mat)
                {
                    mat.EnableKeyword("DITHER");
                    mat.SetFloat("_DitherOn", 1f);
                }

                public static void Gloss(Material mat, float glossiness = 1f, float specularExponent = 10f, Color? color = null)
                {
                    mat.EnableKeyword("FORCE_SPEC");
                    mat.SetFloat("_SpecularHighlights", 1f);
                    mat.SetFloat("_SpecularExponent", specularExponent);
                    mat.SetFloat("_SpecularStrength", glossiness);
                    mat.SetColor("_SpecularTint", color ?? Color.white);
                }

                public static void Emission(Material mat, float power = 1f, Color? color = null)
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.EnableKeyword("FRESNEL_EMISSION");
                    mat.SetFloat("_EmPower", power);
                    mat.SetColor("_EmColor", color ?? Color.white);
                }
            }
        }
    }

    public class MysticsItemsContent : ContentPack
    {
        public MysticsItemsContent()
        {
            Init();
            bodyPrefabs = Resources.bodyPrefabs.ToArray();
            masterPrefabs = Resources.masterPrefabs.ToArray();
            projectilePrefabs = Resources.projectilePrefabs.ToArray();
            effectDefs = Resources.effectPrefabs.ConvertAll(x => new EffectDef(x)).ToArray();
            networkSoundEventDefs = Resources.networkSoundEvents.ConvertAll(x =>
            {
                NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
                networkSoundEventDef.eventName = x;
                return networkSoundEventDef;
            }).ToArray();
            unlockableDefs = Resources.unlockableDefs.ToArray();
            entityStateTypes = Resources.entityStateTypes.ToArray();
            skillDefs = Resources.skillDefs.ToArray();
            itemDefs = Items.itemDefs;
            equipmentDefs = Equipment.equipmentDefs;
            buffDefs = Buffs.buffDefs;
        }

        public static void Init()
        {
            Items.Load();
            Equipment.Load();
            Buffs.Load();
        }

        public static class Resources
        {
            public static List<GameObject> bodyPrefabs = new List<GameObject>();
            public static List<GameObject> masterPrefabs = new List<GameObject>();
            public static List<GameObject> projectilePrefabs = new List<GameObject>();
            public static List<GameObject> effectPrefabs = new List<GameObject>();
            public static List<string> networkSoundEvents = new List<string>();
            public static List<UnlockableDef> unlockableDefs = new List<UnlockableDef>();
            public static List<System.Type> entityStateTypes = new List<System.Type>();
            public static List<RoR2.Skills.SkillDef> skillDefs = new List<RoR2.Skills.SkillDef>();
        }

        public static class Items
        {
            public static void Load()
            {
                HealOrbOnBarrel = new MysticsItems.Items.HealOrbOnBarrel().Load();
                ScratchTicket = new MysticsItems.Items.ScratchTicket().Load();
                BackArmor = new MysticsItems.Items.BackArmor().Load();
                CoffeeBoostOnItemPickup = new MysticsItems.Items.CoffeeBoostOnItemPickup().Load();
                ExplosivePickups = new MysticsItems.Items.ExplosivePickups().Load();
                AllyDeathRevenge = new MysticsItems.Items.AllyDeathRevenge().Load();
                Spotter = new MysticsItems.Items.Spotter().Load();
                SpeedGivesDamage = new MysticsItems.Items.SpeedGivesDamage().Load();
                ExtraShrineUse = new MysticsItems.Items.ExtraShrineUse().Load();
                Voltmeter = new MysticsItems.Items.Voltmeter().Load();
                ThoughtProcessor = new MysticsItems.Items.ThoughtProcessor().Load();
                CrystalWorld = new MysticsItems.Items.CrystalWorld().Load();
                DasherDisc = new MysticsItems.Items.DasherDisc().Load();
                TreasureMap = new MysticsItems.Items.TreasureMap().Load();
                RiftLens = new MysticsItems.Items.RiftLens().Load();
                /*
                CommandoScope = new MysticsItems.Items.CommandoScope().Load();
                CommandoRevolverDrum = new MysticsItems.Items.CommandoRevolverDrum().Load();
                ArtificerNanobots = new MysticsItems.Items.ArtificerNanobots().Load();
                */
                itemDefs = MysticsItems.Items.BaseItem.loadedItems.ConvertAll(x => x.itemDef).ToArray();
            }

            public static ItemDef[] itemDefs;

            public static ItemDef HealOrbOnBarrel;
            public static ItemDef ScratchTicket;
            public static ItemDef BackArmor;
            public static ItemDef CoffeeBoostOnItemPickup;
            public static ItemDef ExplosivePickups;
            public static ItemDef AllyDeathRevenge;
            public static ItemDef Spotter;
            public static ItemDef SpeedGivesDamage;
            public static ItemDef ExtraShrineUse;
            public static ItemDef Voltmeter;
            public static ItemDef ThoughtProcessor;
            public static ItemDef CrystalWorld;
            public static ItemDef DasherDisc;
            public static ItemDef TreasureMap;
            public static ItemDef RiftLens;
            public static ItemDef CommandoScope;
            public static ItemDef CommandoRevolverDrum;
            public static ItemDef ArtificerNanobots;
        }

        public static class Equipment
        {
            public static void Load()
            {
                ArchaicMask = new MysticsItems.Equipment.ArchaicMask().Load();
                PrinterHacker = new MysticsItems.Equipment.PrinterHacker().Load();
                Microphone = new MysticsItems.Equipment.Microphone().Load();
                GateChalice = new MysticsItems.Equipment.GateChalice().Load();
                TuningFork = new MysticsItems.Equipment.TuningFork().Load();
                equipmentDefs = MysticsItems.Equipment.BaseEquipment.loadedEquipment.ConvertAll(x => x.equipmentDef).ToArray();
            }

            public static EquipmentDef[] equipmentDefs;

            public static EquipmentDef ArchaicMask;
            public static EquipmentDef PrinterHacker;
            public static EquipmentDef Microphone;
            public static EquipmentDef GateChalice;
            public static EquipmentDef TuningFork;
        }

        public static class Buffs
        {
            public static void Load()
            {
                AllyDeathRevenge = new MysticsItems.Buffs.AllyDeathRevenge().Load();
                CoffeeBoost = new MysticsItems.Buffs.CoffeeBoost().Load();
                DasherDiscActive = new MysticsItems.Buffs.DasherDiscActive().Load();
                DasherDiscCooldown = new MysticsItems.Buffs.DasherDiscCooldown().Load();
                Deafened = new MysticsItems.Buffs.Deafened().Load();
                GateChalice = new MysticsItems.Buffs.GateChalice().Load();
                RiftLens = new MysticsItems.Buffs.RiftLens().Load();
                SpeedGivesDamage = new MysticsItems.Buffs.SpeedGivesDamage().Load();
                SpotterMarked = new MysticsItems.Buffs.SpotterMarked().Load();
                buffDefs = MysticsItems.Buffs.BaseBuff.loadedBuffs.ConvertAll(x => x.buffDef).ToArray();
            }

            public static BuffDef[] buffDefs;

            public static BuffDef AllyDeathRevenge;
            public static BuffDef CoffeeBoost;
            public static BuffDef DasherDiscActive;
            public static BuffDef DasherDiscCooldown;
            public static BuffDef Deafened;
            public static BuffDef GateChalice;
            public static BuffDef RiftLens;
            public static BuffDef SpeedGivesDamage;
            public static BuffDef SpotterMarked;
        }
    }

    public static class BrotherInfection
    {
        public static GameObject white = Resources.Load<GameObject>("Prefabs/CharacterBodies/BrotherBody").GetComponentInChildren<CharacterModel>().itemDisplayRuleSet.FindDisplayRuleGroup(RoR2Content.Items.Hoof).rules[0].followerPrefab;
        public static GameObject green = Resources.Load<GameObject>("Prefabs/CharacterBodies/BrotherBody").GetComponentInChildren<CharacterModel>().itemDisplayRuleSet.FindDisplayRuleGroup(RoR2Content.Items.Feather).rules[0].followerPrefab;
        public static GameObject red = Resources.Load<GameObject>("Prefabs/CharacterBodies/BrotherBody").GetComponentInChildren<CharacterModel>().itemDisplayRuleSet.FindDisplayRuleGroup(RoR2Content.Items.ShockNearby).rules[0].followerPrefab;
        public static GameObject blue = Resources.Load<GameObject>("Prefabs/CharacterBodies/BrotherBody").GetComponentInChildren<CharacterModel>().itemDisplayRuleSet.FindDisplayRuleGroup(RoR2Content.Items.LunarDagger).rules[0].followerPrefab;
    }
}
