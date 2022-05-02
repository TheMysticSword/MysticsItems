using RoR2;
using UnityEngine;
using Rewired.ComponentControls.Effects;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Items
{
    public class MarwanAsh3 : BaseItem
    {
        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_MarwanAsh3";
            SetItemTierWhenAvailable(ItemTier.Tier1);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage,
                ItemTag.WorldUnique
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Marwan's Ash/Level 3/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Marwan's Ash/Level 3/IconWhite.png");

            HopooShaderToMaterial.Standard.Apply(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial);
            HopooShaderToMaterial.Standard.Emission(itemDef.pickupModelPrefab.GetComponentInChildren<Renderer>().sharedMaterial, 1f);
            itemDisplayPrefab = PrepareItemDisplayModel(PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Marwan's Ash/Level 3/DisplayModel.prefab")));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Chest", new Vector3(-0.04173F, 0.35874F, -0.17913F), new Vector3(56.32292F, 109.361F, 322.4278F), new Vector3(0.33648F, 0.33648F, 0.33648F));
                AddDisplayRule("HuntressBody", "Chest", new Vector3(0.11163F, 0.14292F, -0.11352F), new Vector3(46.06874F, 259.4763F, 115.5922F), new Vector3(0.30796F, 0.30796F, 0.30796F));
                AddDisplayRule("Bandit2Body", "Chest", new Vector3(0.00308F, 0.26108F, -0.1953F), new Vector3(58.10995F, 86.1719F, 88.37698F), new Vector3(0.31231F, 0.31231F, 0.31231F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(1.75859F, 1.74398F, -1.88158F), new Vector3(75.9173F, 270.7909F, 96.16955F), new Vector3(2.42414F, 2.42414F, 2.42414F));
                AddDisplayRule("EngiBody", "Chest", new Vector3(0.06489F, 0.38653F, -0.30449F), new Vector3(72.40103F, 244.3324F, 230.793F), new Vector3(0.30143F, 0.30143F, 0.30143F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.02096F, 0.72177F, 0.17551F), new Vector3(0.00001F, 124.692F, -0.00001F), new Vector3(0.7235F, 0.7235F, 0.7235F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(-0.10296F, 1.39139F, -0.2884F), new Vector3(359.9551F, 108.1378F, 1.80796F), new Vector3(0.58364F, 0.70995F, 0.57057F));
                AddDisplayRule("MageBody", "Chest", new Vector3(0.02271F, 0.14716F, -0.3046F), new Vector3(66.65702F, 250.4772F, 68.23966F), new Vector3(0.24126F, 0.24126F, 0.24126F));
                AddDisplayRule("MercBody", "Chest", new Vector3(0.16969F, 0.18685F, -0.20947F), new Vector3(78.62347F, 252.0916F, 65.17757F), new Vector3(0.25071F, 0.25071F, 0.25071F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(0.07016F, 0.97228F, 0F), new Vector3(90F, 0F, 0F), new Vector3(0.8264F, 0.8264F, 0.8264F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(0.17614F, 0.18596F, -0.1487F), new Vector3(75.94363F, 261.2244F, 76.51655F), new Vector3(0.33343F, 0.33343F, 0.33343F));
                AddDisplayRule("CrocoBody", "SpineChest2", new Vector3(0.67496F, 0.17271F, 1.07906F), new Vector3(354.0664F, 327.921F, 166.232F), new Vector3(2.30107F, 2.30107F, 2.30107F));
                AddDisplayRule("CaptainBody", "Chest", new Vector3(-0.10099F, 0.09012F, -0.25135F), new Vector3(59.89094F, 110.7751F, 111.0735F), new Vector3(0.30118F, 0.30118F, 0.30118F));
                AddDisplayRule("BrotherBody", "chest", BrotherInfection.red, new Vector3(-0.22101F, 0.42643F, -0.064F), new Vector3(0F, 0F, 281.5435F), new Vector3(0.04683F, 0.09274F, 0.10516F));
                AddDisplayRule("ScavBody", "Chest", new Vector3(8.46921F, -0.00509F, 2.72015F), new Vector3(73.6153F, 261.9773F, 57.57054F), new Vector3(5.2327F, 5.37682F, 4.26196F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Chest", new Vector3(0.00245F, 0.26265F, -0.33652F), new Vector3(69.9714F, 295.4293F, 296.8602F), new Vector3(0.21151F, 0.21151F, 0.21151F));
                AddDisplayRule("RailgunnerBody", "Backpack", new Vector3(0.24336F, 0.03694F, 0.13067F), new Vector3(90F, 318.2994F, 0F), new Vector3(0.28968F, 0.28968F, 0.28968F));
                AddDisplayRule("VoidSurvivorBody", "Chest", new Vector3(-0.11828F, 0.09191F, -0.02374F), new Vector3(358.1163F, 1.34265F, 144.5096F), new Vector3(0.25305F, 0.25305F, 0.25305F));
            };
        }
    }
}
