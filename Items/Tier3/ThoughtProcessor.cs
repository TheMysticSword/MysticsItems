using RoR2;
using R2API.Utils;
using UnityEngine;
using System;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.LegacyBalanceConfigManager;
using UnityEngine.Networking;

namespace MysticsItems.Items
{
    public class ThoughtProcessor : BaseItem
    {
        public static ConfigurableValue<float> cdr = new ConfigurableValue<float>(
            "Item: Thought Processor",
            "CDR",
            50f,
            "Using a skill with cooldown reduces all other skill cooldowns by this percentage of the used skill's cooldown (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_THOUGHTPROCESSOR_DESC"
            }
        );
        public static ConfigurableValue<float> cdrPerStack = new ConfigurableValue<float>(
            "Item: Thought Processor",
            "CDRPerStack",
            50f,
            "Using a skill with cooldown reduces all other skill cooldowns by this percentage of the used skill's cooldown for each additional stack of this item (in %)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_THOUGHTPROCESSOR_DESC"
            }
        );

        public static NetworkSoundEventDef sfx;

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_ThoughtProcessor";
            SetItemTierWhenAvailable(ItemTier.Tier3);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility
            };
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Thought Processor/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Thought Processor/Icon.png");
            itemDef.pickupModelPrefab.AddComponent<MysticsItemsThoughtProcessorBladeSpin>();
            Material matThoughtProcessor = itemDef.pickupModelPrefab.transform.Find("Base").gameObject.GetComponent<MeshRenderer>().sharedMaterial;
            HopooShaderToMaterial.Standard.Apply(matThoughtProcessor);
            HopooShaderToMaterial.Standard.Gloss(matThoughtProcessor);
            HopooShaderToMaterial.Standard.Dither(matThoughtProcessor);
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Head", new Vector3(0F, 0.5F, 0.023F), new Vector3(0F, 270F, 180F), new Vector3(0.2F, 0.2F, 0.2F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(-0.001F, 0.395F, -0.076F), new Vector3(359.536F, 90.36F, 165.383F), new Vector3(0.187F, 0.174F, 0.187F));
                AddDisplayRule("Bandit2Body", "Hat", new Vector3(0.008F, 0.072F, -0.029F), new Vector3(0F, 254.385F, 200.576F), new Vector3(0.137F, 0.137F, 0.137F));
                AddDisplayRule("ToolbotBody", "HandR", new Vector3(0.338F, 1.246F, -0.155F), new Vector3(356.736F, 85.148F, 3.991F), new Vector3(1.5F, 1.613F, 1.5F));
                AddDisplayRule("EngiBody", "HeadCenter", new Vector3(0.001F, 0.3F, 0.022F), new Vector3(359.991F, 272.116F, 173.922F), new Vector3(0.216F, 0.216F, 0.216F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(-0.007F, 0.574F, -0.307F), new Vector3(356.042F, 91.028F, 94.927F), new Vector3(0.133F, 0.133F, 0.133F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(-0.024F, 0.774F, -0.23F), new Vector3(0.234F, 271.606F, 270.571F), new Vector3(0.407F, 0.495F, 0.397F));
                AddDisplayRule("MageBody", "Head", new Vector3(0.003F, 0.232F, -0.181F), new Vector3(1.434F, 264.707F, 230.524F), new Vector3(0.149F, 0.149F, 0.149F));
                AddDisplayRule("MercBody", "Head", new Vector3(0F, 0.315F, 0.116F), new Vector3(1.129F, 273.826F, 158.027F), new Vector3(0.16F, 0.16F, 0.16F));
                AddDisplayRule("TreebotBody", "PlatformBase", new Vector3(0.466F, 0.259F, 0.204F), new Vector3(0F, 143.502F, 0F), new Vector3(0.083F, 0.083F, 0.083F));
                AddDisplayRule("LoaderBody", "Head", new Vector3(0F, 0.291F, 0.019F), new Vector3(0.389F, 270F, 180F), new Vector3(0.171F, 0.171F, 0.171F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(-0.125F, 0.241F, 2.181F), new Vector3(0F, 90F, 290F), new Vector3(1.271F, 0.895F, 1.408F));
                AddDisplayRule("CaptainBody", "Stomach", new Vector3(0.001F, 0.276F, 0.016F), new Vector3(0.318F, 257.881F, 182.808F), new Vector3(0.158F, 0.158F, 0.158F));
                AddDisplayRule("BrotherBody", "Head", BrotherInfection.red, new Vector3(0.011F, 0.129F, 0.071F), new Vector3(28.594F, 22.166F, 285.147F), new Vector3(0.125F, 0.125F, 0.125F));
                AddDisplayRule("ScavBody", "MuzzleEnergyCannon", new Vector3(0F, 0.001F, -22.578F), new Vector3(0F, 270F, 90F), new Vector3(1.733F, 1.733F, 1.733F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Head", new Vector3(-0.00267F, 0.1063F, -0.295F), new Vector3(1.93567F, 269.8339F, 255.3638F), new Vector3(0.18268F, 0.12134F, 0.18268F));
                AddDisplayRule("RailgunnerBody", "Head", new Vector3(0F, 0.28127F, 0.00146F), new Vector3(0F, 270F, 172.1766F), new Vector3(0.18744F, 0.15509F, 0.16573F));
                AddDisplayRule("VoidSurvivorBody", "Neck", new Vector3(0.00001F, 0.01907F, -0.00523F), new Vector3(0F, 270F, 0F), new Vector3(0.22184F, 0.15033F, 0.2149F));
            };

            On.RoR2.CharacterBody.OnSkillActivated += CharacterBody_OnSkillActivated;

            sfx = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            sfx.eventName = "MysticsItems_Play_item_proc_blender";
            MysticsItemsContent.Resources.networkSoundEventDefs.Add(sfx);
        }

        private void CharacterBody_OnSkillActivated(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody self, GenericSkill skill)
        {
            orig(self, skill);
            Inventory inventory = self.inventory;
            if (inventory)
            {
                var itemCount = inventory.GetItemCount(itemDef);
                if (itemCount > 0)
                {
                    if (skill.finalRechargeInterval > 0f && skill.skillDef.stockToConsume > 0)
                    {
                        var coeff = Util.ConvertAmplificationPercentageIntoReductionPercentage(cdr + cdrPerStack * (float)(itemCount - 1)) / 100f;
                        var cooldownsReducedBy = skill.finalRechargeInterval * coeff;
                        foreach (SkillSlot skillSlot in Enum.GetValues(typeof(SkillSlot)))
                        {
                            GenericSkill otherSkill = self.skillLocator.GetSkill(skillSlot);
                            if (skill != otherSkill && otherSkill != null && otherSkill.stock < otherSkill.maxStock)
                            {
                                otherSkill.rechargeStopwatch += cooldownsReducedBy;
                            }
                        }
                        var fxHelper = self.GetComponent<MysticsItemsThoughtProcessorFXHelper>();
                        if (!fxHelper) fxHelper = self.gameObject.AddComponent<MysticsItemsThoughtProcessorFXHelper>();
                        fxHelper.OnTrigger(cooldownsReducedBy);
                    }
                }
            }
        }

        public class MysticsItemsThoughtProcessorFXHelper : MonoBehaviour
        {
            public float timeBetweenSkills = 0f;
            public float maxTimeBetweenSkills = 1.2f;
            public int skillsUsed = 0;
            public int skillsRequiredForSound = 3;
            public float cooldownsReducedBy = 0f;
            public float minCooldownsReducedByForEffect = 3f;

            public void OnTrigger(float cooldownsReducedBy)
            {
                timeBetweenSkills = 0f;
                skillsUsed++;
                this.cooldownsReducedBy += cooldownsReducedBy;
                if (skillsUsed >= skillsRequiredForSound && this.cooldownsReducedBy >= minCooldownsReducedByForEffect)
                {
                    if (NetworkServer.active)
                    {
                        RoR2.Audio.EntitySoundManager.EmitSoundServer(sfx.index, gameObject);
                    }
                }
            }

            public void FixedUpdate()
            {
                if (timeBetweenSkills < maxTimeBetweenSkills)
                {
                    timeBetweenSkills += Time.fixedDeltaTime;
                    if (timeBetweenSkills >= maxTimeBetweenSkills)
                    {
                        skillsUsed = 0;
                        cooldownsReducedBy = 0;
                    }
                }
            }
        }

        public class MysticsItemsThoughtProcessorBladeSpin : MonoBehaviour
        {
            public Transform blades;

            public void Awake()
            {
                blades = transform.Find("Cutter");
            }

            public void Update()
            {
                blades.Rotate(new Vector3(0f, 0f, -1200f * Time.deltaTime), Space.Self);
            }
        }
    }
}
