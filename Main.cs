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
    [R2APISubmoduleDependency(nameof(BuffAPI), nameof(ItemAPI), nameof(ItemDropAPI), nameof(LanguageAPI), nameof(LoadoutAPI), nameof(NetworkingAPI), nameof(PrefabAPI), nameof(ResourcesAPI), nameof(SoundAPI))]

    public class MysticsItemsPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "com.themysticsword.mysticsitems";
        public const string PluginName = "MysticsItems";
        public const string PluginVersion = "1.1.3";

        internal static BepInEx.Logging.ManualLogSource logger;

        public void Awake()
        {
            logger = Logger;
            Main.Init();
        }

        public void Start()
        {
            Main.PostGameLoad();
        }
    }

    public static class Main
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

            //DebugTools.Init();

            Achievements.BaseAchievement.Init();
            AssetManager.Init();
            CharacterStats.Init();
            ItemDropFixes.Init();
            Items.BaseItem.Init();
            Items.CharacterItems.Init();
            Equipment.BaseEquipment.Init();
            Outlines.Init();
            Overlays.Init();
            PlainHologram.Init();
            Unlockables.Init();

            //LaserTurret.Init();
            ShrineLegendary.Init();

            // Auto-load classes
            System.Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                if (!type.IsAbstract)
                {
                    if (type.BaseType == typeof(Items.BaseItem))
                    {
                        Items.BaseItem item = (Items.BaseItem)System.Activator.CreateInstance(type);
                        item.Add();
                    }
                    if (type.BaseType == typeof(Equipment.BaseEquipment))
                    {
                        Equipment.BaseEquipment equipment = (Equipment.BaseEquipment)System.Activator.CreateInstance(type);
                        equipment.Add();
                    }
                    if (type.BaseType == typeof(Buffs.BaseBuff))
                    {
                        Buffs.BaseBuff buff = (Buffs.BaseBuff)System.Activator.CreateInstance(type);
                        buff.Add();
                    }
                    if (type.BaseType == typeof(Achievements.BaseAchievement))
                    {
                        Achievements.BaseAchievement achievement = (Achievements.BaseAchievement)System.Activator.CreateInstance(type);
                        achievement.Add();
                    }
                }
            }

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
            Items.BaseItem.PostGameLoad();
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(SoftDependencies.ItemStatsSoftDependency.PluginGUID)) SoftDependencies.ItemStatsSoftDependency.Init();
        }

        [ConCommand(commandName = Main.TokenPrefix + "unlocklogs", flags = ConVarFlags.Cheat, helpText = "Unlocks all logbook entries")]
        private static void CCUnlockLogs(ConCommandArgs args)
        {
            foreach (LocalUser user in LocalUserManager.readOnlyLocalUsersList)
            {
                foreach (KeyValuePair<System.Type, Items.BaseItem> keyValuePair in Items.BaseItem.registeredItems)
                {
                    user.userProfile.DiscoverPickup(keyValuePair.Value.GetPickupIndex());
                }
                foreach (KeyValuePair<System.Type, Equipment.BaseEquipment> keyValuePair in Equipment.BaseEquipment.registeredEquipment)
                {
                    user.userProfile.DiscoverPickup(keyValuePair.Value.GetPickupIndex());
                }
            }
        }

        public enum CommonBodyIndices
        {
            Commando = 26,
            EngiTurret = 36,
            EngiWalkerTurret = 37
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

        public static class Overlays
        {
            public struct OverlayInfo
            {
                public Material material;
                public System.Func<CharacterModel, bool> condition;

                public OverlayInfo(Material material, System.Func<CharacterModel, bool> condition)
                {
                    this.material = material;
                    this.condition = condition;
                }
            }

            public static List<OverlayInfo> overlays = new List<OverlayInfo>();

            public static void Init()
            {
                On.RoR2.CharacterModel.UpdateOverlays += (orig, self) =>
                {
                    orig(self);
                    if (self.body)
                    {
                        foreach (OverlayInfo overlayInfo in overlays)
                        {
                            if (self.GetFieldValue<int>("activeOverlayCount") >= typeof(CharacterModel).GetFieldValue<int>("maxOverlays"))
                            {
                                return;
                            }
                            if (overlayInfo.condition(self))
                            {
                                Material[] array = self.GetFieldValue<Material[]>("currentOverlays");
                                int num = self.GetFieldValue<int>("activeOverlayCount");
                                self.SetFieldValue("activeOverlayCount", num + 1);
                                array[num] = overlayInfo.material;
                            }
                        }
                    }
                };
            }

            public static void CreateOverlay(Material material, System.Func<CharacterModel, bool> condition)
            {
                overlays.Add(new OverlayInfo(material, condition));
            }
        }

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

    public static class BrotherInfection
    {
        public static GameObject white = Resources.Load<GameObject>("Prefabs/CharacterBodies/BrotherBody").GetComponentInChildren<CharacterModel>().itemDisplayRuleSet.FindItemDisplayRuleGroup("Hoof").rules[0].followerPrefab;
        public static GameObject green = Resources.Load<GameObject>("Prefabs/CharacterBodies/BrotherBody").GetComponentInChildren<CharacterModel>().itemDisplayRuleSet.FindItemDisplayRuleGroup("Feather").rules[0].followerPrefab;
        public static GameObject red = Resources.Load<GameObject>("Prefabs/CharacterBodies/BrotherBody").GetComponentInChildren<CharacterModel>().itemDisplayRuleSet.FindItemDisplayRuleGroup("ShockNearby").rules[0].followerPrefab;
        public static GameObject blue = Resources.Load<GameObject>("Prefabs/CharacterBodies/BrotherBody").GetComponentInChildren<CharacterModel>().itemDisplayRuleSet.FindItemDisplayRuleGroup("LunarDagger").rules[0].followerPrefab;
    }
}
