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

        internal static System.Type declaringType;

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

            declaringType = MethodBase.GetCurrentMethod().DeclaringType;

            RoR2Application.onLoad += PostGameLoad;

            //DebugTools.Init();

            Achievements.BaseAchievement.Init();
            CharacterStats.Init();
            ConCommandHelper.Init();
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

            // Load console commands
            ConCommandHelper.Load(declaringType.GetMethod("CCUnlockLogs", bindingFlagAll));
            ConCommandHelper.Load(declaringType.GetMethod("CCGrantAll", bindingFlagAll));

            // Load the content pack
            On.RoR2.ContentManager.SetContentPacks += (orig, newContentPacks) =>
            {
                newContentPacks.Add(new MysticsItemsContent());
                orig(newContentPacks);
            };
        }

        public static void PostGameLoad()
        {
            LanguageLoader.Load("MysticsItems.language");

            Items.BaseItem.PostGameLoad();
            
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(SoftDependencies.ItemStatsSoftDependency.PluginGUID)) SoftDependencies.ItemStatsSoftDependency.Init();
        }

        [ConCommand(commandName = Main.TokenPrefix + "unlocklogs", flags = ConVarFlags.None, helpText = "Unlocks all logbook entries")]
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

                // Temporary fix for BuffCatalog loading buffdefs from the wrong place
                IL.RoR2.BuffCatalog.Init += (il) =>
                {
                    ILCursor c = new ILCursor(il);

                    if (!c.Next.MatchLdsfld(typeof(RoR2Content.Buffs), nameof(RoR2Content.Buffs.buffDefs)))
                    {
                        Main.logger.LogMessage("Another mod has already fixed BuffCatalog or the game has updated, skipping...");
                        return;
                    }

                    c.Remove();
                    c.Emit(OpCodes.Ldsfld, typeof(ContentManager).GetField(nameof(ContentManager.buffDefs)));
                };
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
