using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Collections.Generic;
using RoR2.Audio;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.LegacyBalanceConfigManager;

namespace MysticsItems.Items
{
    public class BuffInTPRange : BaseItem
    {
        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_BuffInTPRange";
            SetItemTierWhenAvailable(ItemTier.Tier2);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.HoldoutZoneRelated
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Cat Ear Headphones/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Cat Ear Headphones/Icon.png");
            MysticsItemsContent.Resources.unlockableDefs.Add(GetUnlockableDef());

            itemDisplayPrefab = PrepareItemDisplayModel(PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Cat Ear Headphones/DisplayModel.prefab")));
            itemDisplayPrefab.AddComponent<MysticsItemsBuffInTPRangeItemDisplayHelper>();
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Head", new Vector3(-0.00816F, 0.38568F, 0.02439F), new Vector3(5.07642F, 0F, 0F), new Vector3(0.85862F, 0.85862F, 0.85862F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(-0.00692F, 0.31612F, -0.05489F), new Vector3(354.6689F, 0F, 0F), new Vector3(0.57462F, 0.62854F, 0.57462F));
                AddDisplayRule("Bandit2Body", "Head", new Vector3(-0.00004F, 0.18077F, 0.01298F), new Vector3(349.5076F, 0F, 0F), new Vector3(0.47957F, 0.50716F, 0.50716F));
                AddDisplayRule("ToolbotBody", "Head", new Vector3(0.01339F, 2.69805F, 1.9022F), new Vector3(277.9093F, 191.0181F, 350.0386F), new Vector3(8.8842F, 8.8842F, 8.8842F));
                AddDisplayRule("EngiBody", "HeadCenter", new Vector3(-0.00639F, 0.17129F, 0F), new Vector3(0F, 0F, 0F), new Vector3(0.82636F, 0.76351F, 0.74883F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0F, 0.8902F, -0.00008F), new Vector3(0F, 0F, 355F), new Vector3(4.30519F, 4.30519F, 4.30519F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.0006F, 1.56941F, -0.42273F), new Vector3(353.0465F, 0F, 0F), new Vector3(4.41544F, 4.41544F, 4.41544F));
                AddDisplayRule("MageBody", "Head", new Vector3(-0.00408F, 0.17284F, 0.00895F), new Vector3(6.1674F, 0F, 0F), new Vector3(0.49742F, 0.49742F, 0.49742F));
                AddDisplayRule("MercBody", "Head", new Vector3(-0.00848F, 0.26603F, 0.05728F), new Vector3(11.59233F, 0F, 0F), new Vector3(0.57628F, 0.57628F, 0.57628F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(0.00214F, 0.60333F, 0.03576F), new Vector3(0.26144F, 355F, 1.60241F), new Vector3(4.66764F, 5.04098F, 4.66179F));
                AddDisplayRule("LoaderBody", "Head", new Vector3(-0.00419F, 0.24669F, 0.01637F), new Vector3(0F, 0F, 0F), new Vector3(0.66542F, 0.66542F, 0.66542F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(0.00031F, 0.36012F, 1.58152F), new Vector3(272.6808F, 36.07847F, 143.5646F), new Vector3(8.19354F, 8.19354F, 8.19354F));
                AddDisplayRule("CaptainBody", "Head", new Vector3(-0.0079F, 0.25295F, 0.0517F), new Vector3(0F, 0F, 0F), new Vector3(0.59827F, 0.59827F, 0.59827F));
                AddDisplayRule("BrotherBody", "Head", BrotherInfection.green, new Vector3(0.1254F, 0.19383F, 0.01157F), new Vector3(354.958F, 1.78585F, 241.6238F), new Vector3(0.11074F, 0.06067F, 0.06067F));
                AddDisplayRule("BrotherBody", "Head", BrotherInfection.green, new Vector3(-0.12499F, 0.14703F, 0.02924F), new Vector3(0F, 0F, 302.4016F), new Vector3(0.10784F, 0.06095F, 0.06968F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Head", new Vector3(-0.00927F, 0.05782F, -0.24385F), new Vector3(275.246F, 180F, 180F), new Vector3(0.64491F, 0.64491F, 0.64491F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysDeputy) AddDisplayRule("DeputyBody", "Hat", new Vector3(0F, 0.08684F, 0.04154F), new Vector3(0F, 0F, 0F), new Vector3(0.53912F, 0.53912F, 0.53912F));
                AddDisplayRule("RailgunnerBody", "Head", new Vector3(0F, 0.18103F, 0.00001F), new Vector3(0F, 0F, 0F), new Vector3(0.49415F, 0.49415F, 0.49415F));
                AddDisplayRule("VoidSurvivorBody", "Head", new Vector3(0.00001F, 0.1899F, 0.00005F), new Vector3(0F, 0F, 0F), new Vector3(0.75921F, 0.75921F, 0.75921F));
            };

            Material mat;
            Renderer renderer = itemDef.pickupModelPrefab.GetComponentInChildren<SkinnedMeshRenderer>();

            // body
            mat = renderer.sharedMaterials[0];
            MysticsRisky2Utils.HopooShaderToMaterial.Standard.Apply(mat);
            MysticsRisky2Utils.HopooShaderToMaterial.Standard.Gloss(mat, 0.6f, 5f);

            // metal
            mat = renderer.sharedMaterials[1];
            MysticsRisky2Utils.HopooShaderToMaterial.Standard.Apply(mat);
            MysticsRisky2Utils.HopooShaderToMaterial.Standard.Gloss(mat, 1f, 14f);

            // emission
            mat = renderer.sharedMaterials[2];
            MysticsRisky2Utils.HopooShaderToMaterial.Standard.Apply(mat);
            MysticsRisky2Utils.HopooShaderToMaterial.Standard.Emission(mat, 1f, new Color32(136, 175, 232, 255));

            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            self.AddItemBehavior<MysticsItemsBuffInTPRangeBehaviour>(self.inventory.GetItemCount(itemDef));
        }

        public class MysticsItemsBuffInTPRangeItemDisplayHelper : MonoBehaviour
        {
            public CharacterBody body;
            public List<ParticleSystem> particleSystems;

            public void Start()
            {
                var model = GetComponentInParent<CharacterModel>();
                body = model ? model.body : null;
                particleSystems = GetComponentsInChildren<ParticleSystem>().ToList();
            }

            public void OnEnable()
            {
                InstanceTracker.Add(this);
            }

            public void OnDisable()
            {
                InstanceTracker.Remove(this);
            }

            public void Toggle(bool enableEffects)
            {
                foreach (var particleSystem in particleSystems)
                {
                    if (enableEffects && !particleSystem.isPlaying)
                    {
                        particleSystem.Play();
                    }
                    if (!enableEffects && particleSystem.isPlaying)
                    {
                        particleSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                    }
                }
            }

            public static void ToggleForBody(CharacterBody body, bool enableEffects)
            {
                foreach (var itemDisplayHelper in InstanceTracker.GetInstancesList<MysticsItemsBuffInTPRangeItemDisplayHelper>().Where(x => x.body == body))
                    itemDisplayHelper.Toggle(enableEffects);
            }
        }

        public class MysticsItemsBuffInTPRangeBehaviour : CharacterBody.ItemBehavior
        {
            public HealthComponent healthComponent;
            private bool _isInTPRange = false;
            public bool isInTPRange
            {
                get { return _isInTPRange; }
                set
                {
                    if (_isInTPRange != value)
                    {
                        _isInTPRange = value;
                        if (_isInTPRange)
                        {
                            if (NetworkServer.active)
                                body.AddBuff(MysticsItemsContent.Buffs.MysticsItems_BuffInTPRange);
                        }
                        else
                        {
                            if (NetworkServer.active)
                                body.RemoveBuff(MysticsItemsContent.Buffs.MysticsItems_BuffInTPRange);
                        }
                        MysticsItemsBuffInTPRangeItemDisplayHelper.ToggleForBody(body, _isInTPRange);
                    }
                }
            }

            public GameObject attachedEffect;

            public void Start()
            {
                healthComponent = GetComponent<HealthComponent>();
            }

            public void FixedUpdate()
            {
                var newIsInTPRange = false;

                foreach (var holdoutZoneController in InstanceTracker.GetInstancesList<HoldoutZoneController>())
                {
                    if (holdoutZoneController.isActiveAndEnabled && holdoutZoneController.IsBodyInChargingRadius(body))
                    {
                        newIsInTPRange = true;
                        break;
                    }
                }

                isInTPRange = newIsInTPRange;
            }

            public void OnDestroy()
            {
                isInTPRange = false;
            }
        }
    }
}
