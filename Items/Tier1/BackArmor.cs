using RoR2;
using RoR2.Audio;
using R2API;
using R2API.Utils;
using UnityEngine;

namespace MysticsItems.Items
{
    public class BackArmor : BaseItem
    {
        public static float distance = 0.01f;
        public static GameObject visualEffect;

        public override void PreLoad()
        {
            itemDef.name = "BackArmor";
            itemDef.tier = ItemTier.Tier1;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility
            };
        }

        public override void OnLoad()
        {
            SetAssets("Spine Implant");
            Main.HopooShaderToMaterial.Standard.Gloss(GetModelMaterial(), 0.5f, 10f);
            SetModelPanelDistance(3f, 6f);
            AddDisplayRule("CommandoBody", "Chest", new Vector3(0.001F, 0.248F, -0.191F), new Vector3(10.681F, 0.007F, 0.071F), new Vector3(0.053F, 0.053F, 0.053F));
            AddDisplayRule("CommandoBody", "Chest", new Vector3(0.001F, 0.379F, -0.155F), new Vector3(31.254F, 0.064F, 0.228F), new Vector3(0.053F, 0.053F, 0.053F));
            AddDisplayRule("CommandoBody", "Chest", new Vector3(0.001F, 0.13F, -0.214F), new Vector3(355.88F, 0.248F, 359.081F), new Vector3(0.053F, 0.053F, 0.053F));
            AddDisplayRule("HuntressBody", "Chest", new Vector3(0.115F, 0.122F, -0.102F), new Vector3(350.768F, 327.948F, 7.106F), new Vector3(0.045F, 0.045F, 0.045F));
            AddDisplayRule("HuntressBody", "Chest", new Vector3(0.074F, 0.012F, -0.058F), new Vector3(358.146F, 332.414F, 353.239F), new Vector3(0.045F, 0.045F, 0.045F));
            AddDisplayRule("HuntressBody", "Chest", new Vector3(0.117F, 0.229F, -0.108F), new Vector3(9.933F, 316.317F, 358.989F), new Vector3(0.045F, 0.045F, 0.045F));
            AddDisplayRule("Bandit2Body", "Chest", new Vector3(0F, 0.279F, -0.168F), new Vector3(27.306F, 0F, 0F), new Vector3(0.058F, 0.058F, 0.058F));
            AddDisplayRule("Bandit2Body", "Chest", new Vector3(-0.014F, 0.131F, -0.181F), new Vector3(352.289F, 2.586F, 355.357F), new Vector3(0.058F, 0.058F, 0.058F));
            AddDisplayRule("Bandit2Body", "Chest", new Vector3(-0.025F, -0.02F, -0.144F), new Vector3(334.898F, 4.122F, 354.918F), new Vector3(0.058F, 0.058F, 0.058F));
            AddDisplayRule("ToolbotBody", "Chest", new Vector3(0.057F, 0.562F, -1.819F), new Vector3(0F, 0F, 0F), new Vector3(0.448F, 0.448F, 0.448F));
            AddDisplayRule("ToolbotBody", "Chest", new Vector3(0.057F + 1.128F, 0.562F, -1.819F), new Vector3(0F, 0F, 0F), new Vector3(0.448F, 0.448F, 0.448F));
            AddDisplayRule("ToolbotBody", "Chest", new Vector3(0.057F - 1.128F, 0.562F, -1.819F), new Vector3(0F, 0F, 0F), new Vector3(0.448F, 0.448F, 0.448F));
            AddDisplayRule("EngiBody", "Chest", new Vector3(0.001F, 0.332F, -0.212F), new Vector3(355.488F, 0.001F, 359.97F), new Vector3(0.065F, 0.065F, 0.065F));
            AddDisplayRule("EngiBody", "Chest", new Vector3(0.002F, -0.039F, -0.244F), new Vector3(8.511F, 0.004F, 0.056F), new Vector3(0.065F, 0.065F, 0.065F));
            AddDisplayRule("EngiTurretBody", "Head", new Vector3(0F, 0.594F, -1.52F), new Vector3(34.7F, 0F, 0F), new Vector3(0.1F, 0.1F, 0.1F));
            AddDisplayRule("EngiTurretBody", "Head", new Vector3(0F - 0.288F, 0.594F - 0.001F, -1.52F + 0.036F), new Vector3(34.7F, 17.268F, 0F), new Vector3(0.1F, 0.1F, 0.1F));
            AddDisplayRule("EngiTurretBody", "Head", new Vector3(0F + 0.288F, 0.594F - 0.001F, -1.52F + 0.036F), new Vector3(34.7F, 17.268F, 0F), new Vector3(0.1F, 0.1F, 0.1F));
            AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0F, 0.635F, -1.096F), new Vector3(340.889F, 0F, 0F), new Vector3(0.162F, 0.162F, 0.162F));
            AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0F - 0.403F, 0.635F, -1.096F), new Vector3(340.889F, 0F, 0F), new Vector3(0.162F, 0.162F, 0.162F));
            AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0F + 0.403F, 0.635F, -1.096F), new Vector3(340.889F, 0F, 0F), new Vector3(0.162F, 0.162F, 0.162F));
            AddDisplayRule("MageBody", "Chest", new Vector3(-0.113F, 0.047F, -0.333F), new Vector3(0F, 0F, 0F), new Vector3(0.074F, 0.074F, 0.074F));
            AddDisplayRule("MageBody", "Chest", new Vector3(0.111F, 0.047F, -0.335F), new Vector3(0F, 0F, 0F), new Vector3(0.074F, 0.074F, 0.074F));
            AddDisplayRule("MercBody", "Chest", new Vector3(0.016F, -0.077F, -0.157F), new Vector3(327.518F, 359.258F, 2.356F), new Vector3(0.044F, 0.044F, 0.044F));
            AddDisplayRule("TreebotBody", "HeadBase", new Vector3(0F, 0.303F, 0.623F), new Vector3(326.351F, 0F, 0F), new Vector3(0.13F, 0.13F, 0.13F));
            AddDisplayRule("TreebotBody", "HeadBase", new Vector3(0F, 0.556F, 0.381F), new Vector3(300.933F, 0F, 0F), new Vector3(0.13F, 0.13F, 0.13F));
            AddDisplayRule("TreebotBody", "HeadBase", new Vector3(0F, 0.677F, 0F), new Vector3(272.761F, 180F, 180F), new Vector3(0.13F, 0.13F, 0.13F));
            AddDisplayRule("LoaderBody", "MechBase", new Vector3(0.001F, 0.228F, -0.09F), new Vector3(0F, 0F, 0F), new Vector3(0.055F, 0.055F, 0.055F));
            AddDisplayRule("LoaderBody", "MechBase", new Vector3(0F, 0.079F, -0.109F), new Vector3(15.661F, 359.833F, 359.383F), new Vector3(0.055F, 0.055F, 0.055F));
            AddDisplayRule("CrocoBody", "SpineChest1", new Vector3(-0.001F, 0.227F, -0.262F), new Vector3(327.293F, 180.671F, 178.301F), new Vector3(0.868F, 0.868F, 0.868F));
            AddDisplayRule("CrocoBody", "SpineChest2", new Vector3(-0.007F, 0.238F, -0.127F), new Vector3(279.7F, 179.618F, 180.074F), new Vector3(0.868F, 0.868F, 0.868F));
            AddDisplayRule("CrocoBody", "SpineChest3", new Vector3(-0.006F, 0.28F, 0.702F), new Vector3(272.756F, 348.989F, 10.689F), new Vector3(0.868F, 0.868F, 0.868F));
            AddDisplayRule("CrocoBody", "SpineStomach2", new Vector3(0F, 0.125F, 0.764F), new Vector3(279.494F, 357.435F, 0.679F), new Vector3(0.868F, 0.868F, 0.868F));
            AddDisplayRule("CaptainBody", "Chest", new Vector3(-0.002F, 0.264F, -0.232F), new Vector3(12.656F, 0.806F, 356.471F), new Vector3(0.089F, 0.089F, 0.089F));
            AddDisplayRule("CaptainBody", "Chest", new Vector3(-0.003F, 0.007F, -0.229F), new Vector3(352.867F, 2.021F, 350.785F), new Vector3(0.089F, 0.089F, 0.089F));
            AddDisplayRule("CaptainBody", "Chest", new Vector3(-0.029F, -0.258F, -0.204F), new Vector3(354.747F, 17.2F, 357.393F), new Vector3(0.089F, 0.089F, 0.089F));
            AddDisplayRule("BrotherBody", "chest", BrotherInfection.white, new Vector3(0.007F, 0.271F, -0.109F), new Vector3(338.151F, 273.11F, 349.684F), new Vector3(0.063F, 0.063F, 0.063F));
            AddDisplayRule("BrotherBody", "chest", BrotherInfection.white, new Vector3(0.007F, 0.152F, -0.103F), new Vector3(354.823F, 265.92F, 16.498F), new Vector3(0.063F, 0.063F, 0.063F));
            AddDisplayRule("BrotherBody", "chest", BrotherInfection.white, new Vector3(0.004F, 0.368F, -0.116F), new Vector3(331.238F, 285.737F, 326.197F), new Vector3(0.063F, 0.063F, 0.063F));
            AddDisplayRule("ScavBody", "Backpack", new Vector3(-2.907F, 3.344F, -3.861F), new Vector3(359.95F, 0.351F, 16.311F), new Vector3(1.363F, 1.363F, 1.363F));
            AddDisplayRule("ScavBody", "Backpack", new Vector3(1.977F, 6.937F, -3.851F), new Vector3(359.832F, 358.468F, 326.995F), new Vector3(1.363F, 1.363F, 1.363F));
            AddDisplayRule("ScavBody", "Backpack", new Vector3(-1.588F, 9.252F, -3.947F), new Vector3(358.543F, 1.155F, 125.647F), new Vector3(1.363F, 1.363F, 1.363F));

            visualEffect = PrefabAPI.InstantiateClone(new GameObject(), Main.TokenPrefix + "BackArmorVFX", false);
            EffectComponent effectComponent = visualEffect.AddComponent<EffectComponent>();
            effectComponent.applyScale = true;
            effectComponent.parentToReferencedTransform = true;
            effectComponent.soundName = "MysticsItems_Play_item_proc_spineimplant";
            VFXAttributes vfxAttributes = visualEffect.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            visualEffect.AddComponent<DestroyOnTimer>().duration = 1f;
            MysticsItemsBackArmorVFX component = visualEffect.AddComponent<MysticsItemsBackArmorVFX>();

            GameObject particles = PrefabAPI.InstantiateClone(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Spine Implant/ProcParticles.prefab"), "ProcParticles", false);
            Material material = particles.GetComponent<ParticleSystemRenderer>().sharedMaterial;
            Main.HopooShaderToMaterial.Standard.Apply(material);
            Main.HopooShaderToMaterial.Standard.Emission(material, 2.5f, Color.red);
            particles.transform.SetParent(visualEffect.transform);

            component.particleSystem = particles.GetComponent<ParticleSystem>();
            component.effectComponent = effectComponent;

            MysticsItemsContent.Resources.effectPrefabs.Add(visualEffect);

            GenericGameEvents.BeforeTakeDamage += (damageInfo, characterInfo) =>
            {
                if (characterInfo.inventory)
                {
                    int itemCount = characterInfo.inventory.GetItemCount(MysticsItemsContent.Items.BackArmor);
                    if (itemCount > 0)
                    {
                        float distance = Vector3.Distance(damageInfo.position, characterInfo.body.corePosition);
                        if (distance >= BackArmor.distance)
                        {
                            if (BackstabManager.IsBackstab(characterInfo.body.corePosition - damageInfo.position, characterInfo.body))
                            {
                                BackArmorTempArmor tempArmor = characterInfo.gameObject.GetComponent<BackArmorTempArmor>();
                                if (!tempArmor) tempArmor = characterInfo.gameObject.AddComponent<BackArmorTempArmor>();
                                float tempArmorValue = 10f + 10f * (float)(itemCount - 1);
                                tempArmor.value += tempArmorValue;
                                characterInfo.body.SetPropertyValue("armor", characterInfo.body.armor + tempArmorValue);

                                EffectData effectData = new EffectData
                                {
                                    origin = characterInfo.body.corePosition,
                                    genericFloat = damageInfo.damage / characterInfo.healthComponent.combinedHealth,
                                    scale = 3.5f * characterInfo.body.radius
                                };
                                effectData.SetNetworkedObjectReference(characterInfo.gameObject);
                                EffectManager.SpawnEffect(visualEffect, effectData, true);
                            }
                        }
                    }
                }
            };
            GenericGameEvents.OnTakeDamage += (damageInfo, characterInfo) =>
            {
                BackArmorTempArmor tempArmor = characterInfo.gameObject.GetComponent<BackArmorTempArmor>();
                if (tempArmor)
                {
                    characterInfo.body.SetPropertyValue("armor", characterInfo.body.armor - tempArmor.value);
                    tempArmor.value = 0f;
                }
            };
        }

        public class BackArmorTempArmor : MonoBehaviour
        {
            public float value;
        }

        public class MysticsItemsBackArmorVFX : MonoBehaviour
        {
            public ParticleSystem particleSystem;
            public EffectComponent effectComponent;

            public void Start()
            {
                particleSystem.Emit(Mathf.Max(Mathf.CeilToInt(15f * effectComponent.effectData.genericFloat), 3));
            }
        }
    }
}
