using RoR2;
using UnityEngine;
using System;

namespace MysticsItems.Items
{
    public class ThoughtProcessor : BaseItem
    {
        public override void PreAdd()
        {
            itemDef.name = "ThoughtProcessor";
            itemDef.tier = ItemTier.Tier3;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility
            };
            SetAssets("Thought Processor");
            model.AddComponent<BladeSpin>();
            Material matThoughtProcessor = model.transform.Find("Base").gameObject.GetComponent<MeshRenderer>().sharedMaterial;
            Main.HopooShaderToMaterial.Standard.Apply(matThoughtProcessor);
            Main.HopooShaderToMaterial.Standard.Gloss(matThoughtProcessor);
            Main.HopooShaderToMaterial.Standard.Dither(matThoughtProcessor);
            CopyModelToFollower();
            AddDisplayRule((int)Main.CommonBodyIndices.Commando, "Head", new Vector3(0F, 0.5F, 0.023F), new Vector3(0F, 270F, 180F), new Vector3(0.2F, 0.2F, 0.2F));
            AddDisplayRule("mdlHuntress", "Head", new Vector3(-0.001F, 0.395F, -0.076F), new Vector3(359.536F, 90.36F, 165.383F), new Vector3(0.187F, 0.174F, 0.187F));
            AddDisplayRule("mdlToolbot", "HandR", new Vector3(0.338F, 1.246F, -0.155F), new Vector3(356.736F, 85.148F, 3.991F), new Vector3(1.5F, 1.613F, 1.5F));
            AddDisplayRule("mdlEngi", "HeadCenter", new Vector3(0.001F, 0.3F, 0.022F), new Vector3(359.991F, 272.116F, 173.922F), new Vector3(0.216F, 0.216F, 0.216F));
            AddDisplayRule((int)Main.CommonBodyIndices.EngiTurret, "Head", new Vector3(-0.007F, 0.574F, -0.307F), new Vector3(356.042F, 91.028F, 94.927F), new Vector3(0.133F, 0.133F, 0.133F));
            AddDisplayRule((int)Main.CommonBodyIndices.EngiWalkerTurret, "Head", new Vector3(-0.024F, 0.774F, -0.23F), new Vector3(0.234F, 271.606F, 270.571F), new Vector3(0.407F, 0.495F, 0.397F));
            AddDisplayRule("mdlMage", "Head", new Vector3(0.003F, 0.232F, -0.181F), new Vector3(1.434F, 264.707F, 230.524F), new Vector3(0.149F, 0.149F, 0.149F));
            AddDisplayRule("mdlMerc", "Head", new Vector3(0F, 0.315F, 0.116F), new Vector3(1.129F, 273.826F, 158.027F), new Vector3(0.16F, 0.16F, 0.16F));
            AddDisplayRule("mdlTreebot", "PlatformBase", new Vector3(0.466F, 0.259F, 0.204F), new Vector3(0F, 143.502F, 0F), new Vector3(0.083F, 0.083F, 0.083F));
            AddDisplayRule("mdlLoader", "Head", new Vector3(0F, 0.291F, 0.019F), new Vector3(0.389F, 270F, 180F), new Vector3(0.171F, 0.171F, 0.171F));
            AddDisplayRule("mdlCroco", "Head", new Vector3(-0.125F, 0.241F, 2.181F), new Vector3(0F, 90F, 290F), new Vector3(1.271F, 0.895F, 1.408F));
            AddDisplayRule("mdlCaptain", "Stomach", new Vector3(0.001F, 0.276F, 0.016F), new Vector3(0.318F, 257.881F, 182.808F), new Vector3(0.158F, 0.158F, 0.158F));
            AddDisplayRule("mdlBrother", "Head", BrotherInfection.red, new Vector3(0.011F, 0.129F, 0.071F), new Vector3(28.594F, 22.166F, 285.147F), new Vector3(0.125F, 0.125F, 0.125F));
            AddDisplayRule("mdlScav", "MuzzleEnergyCannon", new Vector3(0F, 0.001F, -22.578F), new Vector3(0F, 270F, 90F), new Vector3(1.733F, 1.733F, 1.733F));
        }

        public float CalculateCoefficient(int itemCount)
        {
            return 0.1f + 0.05f * (float)(itemCount - 1);
        }

        public override void OnAdd()
        {
            On.RoR2.CharacterBody.OnSkillActivated += (orig, self, skill) =>
            {
                orig(self, skill);
                Inventory inventory = self.inventory;
                if (inventory && inventory.GetItemCount(itemIndex) > 0) {
                    float coeff = CalculateCoefficient(inventory.GetItemCount(itemIndex));
                    if (skill.baseRechargeInterval > 0f)
                    {
                        foreach (SkillSlot skillSlot in Enum.GetValues(typeof(SkillSlot)))
                        {
                            GenericSkill otherSkill = self.skillLocator.GetSkill(skillSlot);
                            if (skill != otherSkill && otherSkill != null)
                            {
                                otherSkill.rechargeStopwatch += otherSkill.CalculateFinalRechargeInterval() * coeff;
                            }
                        }
                    }
                }
            };
        }

        public class BladeSpin : MonoBehaviour
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
