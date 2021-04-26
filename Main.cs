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
using RoR2.ContentManagement;
using System.Collections;

namespace MysticsItems
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency(SoftDependencies.ItemStatsSoftDependency.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(SoftDependencies.Starstorm2SoftDependency.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(NetworkingAPI), nameof(PrefabAPI), nameof(SoundAPI))]

    public class MysticsItemsPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "com.themysticsword.mysticsitems";
        public const string PluginName = "MysticsItems";
        public const string PluginVersion = "1.1.8";

        internal static BepInEx.Logging.ManualLogSource logger;
        internal static BepInEx.Configuration.ConfigFile config;

        public void Awake()
        {
            logger = Logger;
            config = Config;
            Main.Init();
        }
    }

    public static partial class Main
    {
        public const string TokenPrefix = MysticsItemsPlugin.PluginName + "_";
        
        public static AssetBundle AssetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("MysticsItems.mysticsitemsunityassetbundle"));

        internal const BindingFlags bindingFlagAll = (BindingFlags)(-1);
        internal static BepInEx.Logging.ManualLogSource logger;

        internal static bool isDedicatedServer = Application.isBatchMode;

        internal static System.Type declaringType;

        public static void Init()
        {
            logger = MysticsItemsPlugin.logger;
            
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
            BaseItemLike.Init();
            CharacterStats.Init();
            ConCommandHelper.Init();
            CostTypeCreation.Init();
            CustomTempVFXManagement.Init();
            //Items.CharacterItems.Init();
            Equipment.BaseEquipment.Init();
            GenericCostTypes.Init();
            GenericGameEvents.Init();
            LanguageLoader.Init();
            Outlines.Init();
            Overlays.Init();
            PlainHologram.Init();
            StateSeralizerFix.Init();

            MysticsItems.ContentManagement.ContentLoadHelper.PluginAwakeLoad<Items.BaseItem>();
            MysticsItems.ContentManagement.ContentLoadHelper.PluginAwakeLoad<Equipment.BaseEquipment>();
            MysticsItems.ContentManagement.ContentLoadHelper.PluginAwakeLoad<Buffs.BaseBuff>();

            //LaserTurret.Init();
            ShrineLegendary.Init();

            // Load console commands
            ConCommandHelper.Load(declaringType.GetMethod("CCUnlockLogs", bindingFlagAll));
            ConCommandHelper.Load(declaringType.GetMethod("CCGrantAll", bindingFlagAll));

            LanguageLoader.Load("MysticsItemsStrings.json");

            // Load the content pack
            ContentManager.collectContentPackProviders += (addContentPackProvider) =>
            {
                addContentPackProvider(new MysticsItemsContent());
            };

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(SoftDependencies.Starstorm2SoftDependency.PluginGUID)) SoftDependencies.Starstorm2SoftDependency.Init();
        }

        public static void PostGameLoad()
        {
            BaseItemLike.PostGameLoad();
            
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

            public class CloudRemap
            {
                public static Shader shader = Resources.Load<Shader>("shaders/fx/hgcloudremap");

                public static void Apply(Material mat, Properties properties = default(Properties))
                {
                    HopooShaderToMaterial.Apply(mat, shader, properties);
                    mat.SetFloat("_Cull", 0f);
                    mat.SetFloat("_ExternalAlpha", 1f);
                    mat.SetFloat("_Fade", 1f);
                    mat.SetFloat("_InvFade", 2f);
                    mat.SetFloat("_SkyboxOnly", 0f);
                    mat.SetFloat("_ZWrite", 1f);
                    mat.SetFloat("_ZTest", 4f);
                }

                public static void Apply(Material mat, Texture remapTexture = null, Texture cloud1Texture = null, Texture cloud2Texture = null, Properties properties = default(Properties))
                {
                    Apply(mat, properties);
                    mat.SetTexture("_Cloud1Tex", cloud1Texture);
                    mat.SetTexture("_Cloud2Tex", cloud2Texture);
                    mat.SetTexture("_RemapTex", remapTexture);
                }

                public static void Boost(Material mat, float power = 1f)
                {
                    mat.SetFloat("_Boost", power);
                }
            }
        }
    }

    public class MysticsItemsContent : IContentPackProvider
    {
        public string identifier
        {
            get
            {
                return MysticsItemsPlugin.PluginName;
            }
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            contentPack.identifier = identifier;
            MysticsItems.ContentManagement.ContentLoadHelper contentLoadHelper = new MysticsItems.ContentManagement.ContentLoadHelper();
            System.Action[] loadDispatchers = new System.Action[]
            {
                () =>
                {
                    contentLoadHelper.DispatchLoad<ItemDef>(typeof(MysticsItems.Items.BaseItem), x => contentPack.itemDefs.Add(x));
                },
                () =>
                {
                    contentLoadHelper.DispatchLoad<EquipmentDef>(typeof(MysticsItems.Equipment.BaseEquipment), x => contentPack.equipmentDefs.Add(x));
                },
                () =>
                {
                    contentLoadHelper.DispatchLoad<BuffDef>(typeof(MysticsItems.Buffs.BaseBuff), x => contentPack.buffDefs.Add(x));
                },
                () =>
                {
                    contentLoadHelper.DispatchLoad<AchievementDef>(typeof(MysticsItems.Achievements.BaseAchievement), null);
                }
            };
            int num;
            for (int i = 0; i < loadDispatchers.Length; i = num)
            {
                loadDispatchers[i]();
                args.ReportProgress(Util.Remap((float)(i + 1), 0f, (float)loadDispatchers.Length, 0f, 0.05f));
                yield return null;
                num = i + 1;
            }
            while (contentLoadHelper.coroutine.MoveNext())
            {
                args.ReportProgress(Util.Remap(contentLoadHelper.progress.value, 0f, 1f, 0.05f, 0.95f));
                yield return contentLoadHelper.coroutine.Current;
            }
            loadDispatchers = new System.Action[]
            {
                () =>
                {
                    ContentLoadHelper.PopulateTypeFields<ItemDef>(typeof(Items), contentPack.itemDefs);
                    MysticsItems.ContentManagement.ContentLoadHelper.AddModPrefixToAssets<ItemDef>(contentPack.itemDefs);
                },
                () =>
                {
                    ContentLoadHelper.PopulateTypeFields<EquipmentDef>(typeof(Equipment), contentPack.equipmentDefs);
                    MysticsItems.ContentManagement.ContentLoadHelper.AddModPrefixToAssets<EquipmentDef>(contentPack.equipmentDefs);
                },
                () =>
                {
                    ContentLoadHelper.PopulateTypeFields<BuffDef>(typeof(Buffs), contentPack.buffDefs);
                    MysticsItems.ContentManagement.ContentLoadHelper.AddModPrefixToAssets<BuffDef>(contentPack.buffDefs);
                },
                () =>
                {
                    contentPack.bodyPrefabs.Add(Resources.bodyPrefabs.ToArray());
                    contentPack.masterPrefabs.Add(Resources.masterPrefabs.ToArray());
                    contentPack.projectilePrefabs.Add(Resources.projectilePrefabs.ToArray());
                    contentPack.effectDefs.Add(Resources.effectPrefabs.ConvertAll(x => new EffectDef(x)).ToArray());
                    contentPack.networkSoundEventDefs.Add(Resources.networkSoundEventDefs.ToArray());
                    contentPack.unlockableDefs.Add(Resources.unlockableDefs.ToArray());
                    contentPack.entityStateTypes.Add(Resources.entityStateTypes.ToArray());
                    contentPack.skillDefs.Add(Resources.skillDefs.ToArray());
                }
            };
            for (int i = 0; i < loadDispatchers.Length; i = num)
            {
                loadDispatchers[i]();
                args.ReportProgress(Util.Remap((float)(i + 1), 0f, (float)loadDispatchers.Length, 0.95f, 0.99f));
                yield return null;
                num = i + 1;
            }
            loadDispatchers = null;
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        private ContentPack contentPack = new ContentPack();

        public static class Resources
        {
            public static List<GameObject> bodyPrefabs = new List<GameObject>();
            public static List<GameObject> masterPrefabs = new List<GameObject>();
            public static List<GameObject> projectilePrefabs = new List<GameObject>();
            public static List<GameObject> effectPrefabs = new List<GameObject>();
            public static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();
            public static List<UnlockableDef> unlockableDefs = new List<UnlockableDef>();
            public static List<System.Type> entityStateTypes = new List<System.Type>();
            public static List<RoR2.Skills.SkillDef> skillDefs = new List<RoR2.Skills.SkillDef>();
        }

        public static class Items
        {
            public static ItemDef AllyDeathRevenge;
            //public static ItemDef ArtificerNanobots;
            public static ItemDef BackArmor;
            public static ItemDef CoffeeBoostOnItemPickup;
            //public static ItemDef CommandoRevolverDrum;
            //public static ItemDef CommandoScope;
            public static ItemDef CrystalWorld;
            public static ItemDef DasherDisc;
            public static ItemDef ExplosivePickups;
            public static ItemDef ExtraShrineUse;
            public static ItemDef GateChaliceDebuff;
            public static ItemDef HealOrbOnBarrel;
            //public static ItemDef KeepShopTerminalOpen;
            //public static ItemDef KeepShopTerminalOpenConsumed;
            //public static ItemDef Moonglasses;
            public static ItemDef RiftLens;
            public static ItemDef RiftLensDebuff;
            public static ItemDef ScratchTicket;
            public static ItemDef SpeedGivesDamage;
            public static ItemDef Spotter;
            public static ItemDef ThoughtProcessor;
            public static ItemDef TreasureMap;
            public static ItemDef Voltmeter;
        }

        public static class Equipment
        {
            public static EquipmentDef ArchaicMask;
            public static EquipmentDef GateChalice;
            public static EquipmentDef Microphone;
            public static EquipmentDef PrinterHacker;
            public static EquipmentDef TuningFork;
        }

        public static class Buffs
        {
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

        public static class Achievements
        {
            public static AchievementDef DiscDeath;
            public static AchievementDef EscapeMoonAlone;
            public static AchievementDef FindArchaicMask;
            //public static AchievementDef MultishopTerminalsOnly;
            public static AchievementDef RepairBrokenSpotter;
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
