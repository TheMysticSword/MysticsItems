using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using RoR2;
using UnityEngine;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Items
{
    public class Moonglasses : BaseItem
    {
        public static ConfigurableValue<float> critDamageIncrease = new ConfigurableValue<float>(
            "Item: Moonglasses",
            "CritDamageIncrease",
            150f,
            "How much more damage should Critical Strikes deal (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_MOONGLASSES_DESC"
            }
        );
        public static ConfigurableValue<float> critDamageIncreasePerStack = new ConfigurableValue<float>(
            "Item: Moonglasses",
            "CritDamageIncreasePerStack",
            150f,
            "How much more damage should Critical Strikes deal for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_MOONGLASSES_DESC"
            }
        );
        public static ConfigOptions.ConfigurableValue<bool> halveNonCritDamageForBackstabbers = ConfigOptions.ConfigurableValue.CreateBool(
            ConfigManager.General.categoryGUID,
            ConfigManager.General.categoryName,
            ConfigManager.General.config,
            "Gameplay",
            "Moonglasses Affect Backstabbers",
            false,
            "All non-crit damage for characters with a backstab passive (e.g. Bandit) will be halved when using the Moonglasses item to compensate for their ability to deal guaranteed crits"
        );

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_Moonglasses";
            SetItemTierWhenAvailable(ItemTier.Lunar);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Moonglasses/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Moonglasses/Icon.png");

            ModelPanelParameters modelPanelParameters = itemDef.pickupModelPrefab.GetComponent<ModelPanelParameters>();
            modelPanelParameters.minDistance = 0.1f;
            modelPanelParameters.maxDistance = 0.2f;

            HopooShaderToMaterial.Standard.Apply(itemDef.pickupModelPrefab.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial);
            ColorUtility.TryParseHtmlString("#3D8AFF", out Color color);
            HopooShaderToMaterial.Standard.Emission(itemDef.pickupModelPrefab.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial, 0.5f, color);
            itemDisplayPrefab = PrepareItemDisplayModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Moonglasses/FollowerModel.prefab"));
            HopooShaderToMaterial.Standard.Apply(itemDisplayPrefab.GetComponentInChildren<MeshRenderer>().sharedMaterial);
            HopooShaderToMaterial.Standard.Emission(itemDisplayPrefab.GetComponentInChildren<MeshRenderer>().sharedMaterial, 0.5f, color);
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Head", new Vector3(-0.01762F, 0.24554F, 0.07167F), new Vector3(0F, 180F, 0F), new Vector3(6.15862F, 7.5286F, 7.5286F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(0F, 0.21282F, 0.05994F), new Vector3(21.54405F, 180F, 90F), new Vector3(4.43197F, 4.43197F, 4.43197F));
                AddDisplayRule("Bandit2Body", "Head", new Vector3(-0.00002F, 0.12569F, 0.06395F), new Vector3(20.98219F, 180F, 0F), new Vector3(4.59836F, 4.59836F, 4.59836F));
                AddDisplayRule("ToolbotBody", "Head", new Vector3(0.19809F, 3.45535F, -0.13022F), new Vector3(58.95989F, 0F, 0F), new Vector3(35.90774F, 35.90774F, 35.90774F));
                AddDisplayRule("EngiBody", "HeadCenter", new Vector3(-0.01717F, 0.03966F, 0.05917F), new Vector3(350.4524F, 180F, 0F), new Vector3(6.37725F, 6.51145F, 6.68287F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(-0.05338F, 0.72787F, 1.74686F), new Vector3(17.32109F, 180F, 0F), new Vector3(17.95457F, 17.95457F, 17.95457F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(-0.04661F, 0.78199F, 0.79482F), new Vector3(0F, 180F, 0F), new Vector3(15.20336F, 15.20336F, 15.20336F));
                AddDisplayRule("MageBody", "Head", new Vector3(-0.00995F, 0.06475F, 0.07292F), new Vector3(0F, 180F, 0F), new Vector3(3.30476F, 3.30476F, 3.30476F));
                AddDisplayRule("MercBody", "Head", new Vector3(-0.01789F, 0.16286F, 0.10037F), new Vector3(0F, 180F, 0F), new Vector3(4.21862F, 4.92004F, 4.92004F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(0.22407F, 1.40964F, 0.54203F), new Vector3(0F, 195.014F, 354.3025F), new Vector3(18.07387F, 19.70414F, 19.70414F));
                AddDisplayRule("LoaderBody", "Head", new Vector3(-0.01495F, 0.13577F, 0.07205F), new Vector3(0F, 180F, 0F), new Vector3(4.31587F, 5.0651F, 5.0651F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(-1.07602F, 2.29072F, 0.17833F), new Vector3(13.06829F, 126.1897F, 98.669F), new Vector3(30.7851F, 30.7851F, 30.7851F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(1.07743F, 2.15423F, 0.28268F), new Vector3(12.64774F, 241.14F, 260.3011F), new Vector3(30.7851F, 30.7851F, 30.7851F));
                AddDisplayRule("CaptainBody", "Head", new Vector3(-0.01724F, 0.18671F, 0.06974F), new Vector3(21.27818F, 180F, 0F), new Vector3(4.28179F, 5.0806F, 5.0806F));
                AddDisplayRule("BrotherBody", "chest", BrotherInfection.blue, new Vector3(-0.05805F, 0.3807F, 0.05423F), new Vector3(322.4044F, 96.87862F, 314.7067F), new Vector3(0.12293F, 0.12293F, 0.12293F));
                AddDisplayRule("ScavBody", "Head", new Vector3(-0.78245F, -0.32694F, 0.85024F), new Vector3(66.96695F, 8.55508F, 183.8605F), new Vector3(273.8666F, 291.7011F, 291.7011F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Head", new Vector3(-0.01805F, 0.12711F, -0.12674F), new Vector3(75.45986F, 179.9725F, 0.06272F), new Vector3(4.6839F, 5.28335F, 5.28335F));
                AddDisplayRule("RailgunnerBody", "Head", new Vector3(-0.01121F, 0.06064F, 0.07488F), new Vector3(0F, 180F, 0F), new Vector3(3.19157F, 3.19157F, 3.19157F));
                AddDisplayRule("VoidSurvivorBody", "Head", new Vector3(-0.01512F, 0.07926F, 0.08407F), new Vector3(30.07312F, 185.1942F, 0F), new Vector3(6.38275F, 6.38275F, 6.38275F));
            };

            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            GenericGameEvents.OnApplyDamageIncreaseModifiers += GenericGameEvents_OnApplyDamageIncreaseModifiers;
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            int critPosition = -1;
            if (c.TryGotoNext(
                x => x.MatchLdfld<CharacterBody>("baseCrit")
            ) && c.TryGotoNext(
                MoveType.AfterLabel,
                x => x.MatchLdfld<CharacterBody>("levelCrit")
            ) && c.TryGotoNext(
                MoveType.AfterLabel,
                x => x.MatchStloc(out critPosition)
            ))
            {
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchCallOrCallvirt<CharacterBody>("set_crit")
                ))
                {
                    c.Emit(OpCodes.Ldarg, 0);
                    c.EmitDelegate<System.Action<CharacterBody>>((body) =>
                    {
                        Inventory inventory = body.inventory;
                        if (inventory)
                        {
                            int itemCount = body.inventory.GetItemCount(itemDef);
                            if (itemCount > 0)
                            {
                                body.crit /= Mathf.Pow(2, itemCount);
                            }
                        }
                    });
                }

                if (c.TryGotoNext(
                    x => x.MatchCallOrCallvirt<CharacterBody>("get_critMultiplier")
                ) && c.TryGotoNext(
                    x => x.MatchLdloc(critPosition)
                ) && c.TryGotoNext(
                    x => x.MatchLdcR4(out _)
                ) && c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchMul()
                ))
                {
                    c.Emit(OpCodes.Ldarg, 0);
                    c.EmitDelegate<System.Func<float, CharacterBody, float>>((currentCritMultiplier, body) =>
                    {
                        Inventory inventory = body.inventory;
                        if (inventory)
                        {
                            int itemCount = body.inventory.GetItemCount(itemDef);
                            if (itemCount > 0)
                            {
                                currentCritMultiplier /= Mathf.Pow(2, itemCount);
                            }
                        }
                        return currentCritMultiplier;
                    });
                }
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var itemCount = sender.inventory.GetItemCount(itemDef);
                if (itemCount > 0)
                {
                    args.critDamageMultAdd += critDamageIncrease.Value / 100f + critDamageIncreasePerStack.Value / 100f * (float)(itemCount - 1);
                }
            }
        }

        public void GenericGameEvents_OnApplyDamageIncreaseModifiers(DamageInfo damageInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo attackerInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo victimInfo, ref float damage)
        {
            if (!damageInfo.crit && attackerInfo.inventory)
            {
                var itemCount = attackerInfo.inventory.GetItemCount(itemDef);
                if (attackerInfo.body && itemCount > 0)
                {
                    if (halveNonCritDamageForBackstabbers && attackerInfo.body.bodyFlags.HasFlag(CharacterBody.BodyFlags.HasBackstabPassive))
                        damage /= Mathf.Pow(2, itemCount);
                }
            }
        }
    }
}
