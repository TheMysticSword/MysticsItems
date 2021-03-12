using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace MysticsItems.Items
{
    public class SpeedGivesDamage : BaseItem
    {
        public static BuffIndex buffIndex;

        public static int maxDamageBoost = 10;
        public static float maxSpeedMultiplierRequirement = 7f;

        public static float percentPerBuffStack = 10f;
        public static float speedRequirementPerBuffStack = 100f;

        public override void PreAdd()
        {
            itemDef.name = "SpeedGivesDamage";
            itemDef.tier = ItemTier.Tier2;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist
            };
            SetAssets("Nuclear Accelerator");
            Main.HopooShaderToMaterial.Standard.Apply(GetModelMaterial());
            Main.HopooShaderToMaterial.Standard.Emission(GetModelMaterial());
            model.AddComponent<MysticsItemsNuclearAcceleratorGlow>();
            CopyModelToFollower();

            AddDisplayRule((int)Main.CommonBodyIndices.Commando, "CalfR", new Vector3(0.01F, 0.221F, 0.091F), new Vector3(283.928F, 181.008F, 20.003F), new Vector3(0.039F, 0.039F, 0.039F));
            AddDisplayRule("mdlHuntress", "CalfR", new Vector3(0.01F, 0.28F, 0.087F), new Vector3(295.556F, 204.302F, 333.758F), new Vector3(0.033F, 0.033F, 0.033F));
            AddDisplayRule("mdlToolbot", "CalfR", new Vector3(0.037F, -0.291F, -0.267F), new Vector3(3.679F, 176.776F, 177.782F), new Vector3(0.495F, 0.495F, 0.495F));
            AddDisplayRule("mdlEngi", "CannonHeadR", new Vector3(-0.185F, 0.279F, -0.044F), new Vector3(356.274F, 1.409F, 91.632F), new Vector3(0.052F, 0.093F, 0.052F));
            AddDisplayRule((int)Main.CommonBodyIndices.EngiTurret, "LegBar3", new Vector3(0F, -0.013F, 0.071F), new Vector3(270.02F, 180F, 0F), new Vector3(0.211F, 0.211F, 0.211F));
            AddDisplayRule((int)Main.CommonBodyIndices.EngiWalkerTurret, "LegBar3", new Vector3(0F, -0.013F, 0.071F), new Vector3(270.02F, 180F, 0F), new Vector3(0.211F, 0.211F, 0.211F));
            AddDisplayRule("mdlMage", "Chest", new Vector3(0.115F, 0.315F, -0.182F), new Vector3(351.323F, 180F, 0F), new Vector3(0.047F, 0.047F, 0.047F));
            AddDisplayRule("mdlMage", "Chest", new Vector3(-0.111F, 0.315F, -0.182F), new Vector3(351.323F, 180F, 0F), new Vector3(0.047F, 0.047F, 0.047F));
            AddDisplayRule("mdlMerc", "CalfR", new Vector3(0F, 0.08F, 0.105F), new Vector3(85.174F, 0F, 0F), new Vector3(0.039F, 0.039F, 0.039F));
            AddDisplayRule("mdlTreebot", "FootBackR", new Vector3(0.124F, -0.044F, 0.006F), new Vector3(1.536F, 177.979F, 88.624F), new Vector3(0.077F, 0.077F, 0.077F));
            AddDisplayRule("mdlLoader", "MechHandR", new Vector3(0.069F, 0.125F, 0.115F), new Vector3(279.578F, 350.353F, 218.5F), new Vector3(0.043F, 0.043F, 0.043F));
            AddDisplayRule("mdlCroco", "CalfR", new Vector3(0.189F, 1.849F, 0.453F), new Vector3(279.59F, 160.263F, 10.136F), new Vector3(0.387F, 0.387F, 0.387F));
            AddDisplayRule("mdlCaptain", "CalfR", new Vector3(0.015F, 0.214F, 0.085F), new Vector3(74.208F, 0F, 0F), new Vector3(0.045F, 0.045F, 0.045F));
            AddDisplayRule("mdlBrother", "CalfR", BrotherInfection.green, new Vector3(0.038F, 0.121F, 0.051F), new Vector3(43.102F, 358.401F, 241.259F), new Vector3(0.078F, 0.078F, 0.078F));

            model.transform.Find("speedpower_powerspeed").Rotate(new Vector3(60f, 0f, 0f), Space.Self);
        }

        public override void OnAdd()
        {
            CharacterStats.damageModifiers.Add(new CharacterStats.StatModifier
            {
                multiplier = 1f,
                times = (genericCharacterInfo) =>
                {
                    Inventory inventory = genericCharacterInfo.inventory;
                    if (inventory)
                    {
                        int itemCount = inventory.GetItemCount(itemIndex);
                        return itemCount != 0 ? Mathf.Max((0.01f + 0.005f * (float)(itemCount - 1)) * ((genericCharacterInfo.body.moveSpeed / (genericCharacterInfo.body.baseMoveSpeed + genericCharacterInfo.body.levelMoveSpeed * genericCharacterInfo.body.level) - 1f) / 0.025f), 0f) : 0f;
                    }
                    return 0f;
                }
            });
            /* Old behaviour
            On.RoR2.CharacterBody.Awake += (orig, self) =>
            {
                orig(self);
                self.onInventoryChanged += delegate ()
                {
                    if (NetworkServer.active) self.AddItemBehavior<SpeedGivesDamageBehaviour>(self.inventory.GetItemCount(itemIndex));
                };
            };
            */
        }

        public class MysticsItemsNuclearAcceleratorGlow : MonoBehaviour
        {
            public Material material;
            public float stopwatch;

            public void Awake()
            {
                Renderer renderer = GetComponentInChildren<Renderer>();
                material = Object.Instantiate(renderer.material);
                renderer.material = material;
            }

            public void Update()
            {
                stopwatch += Time.deltaTime;

                float wave = Mathf.Sin(stopwatch * Mathf.PI * 2f);
                material.SetFloat("_EmPower", 1f * (0.75f + wave * 0.25f));
                float rgb = 0.75f + 0.25f * wave;
                material.SetColor("_EmColor", new Color(rgb, rgb, rgb, 1f));
            }

            public void OnDestroy()
            {
                Object.Destroy(material);
            }
        }

        public class SpeedGivesDamageBehaviour : CharacterBody.ItemBehavior
        {
            public int maxStack = 0;

            public void Start()
            {
                timedBuffs = (IList)timedBuffsField.GetValue(body);
            }

            public void FixedUpdate()
            {
                if (NetworkServer.active)
                {
                    if (maxStack > 0 && !body.HasBuff(buffIndex)) maxStack = 0;
                    CharacterMotor characterMotor = body.characterMotor;
                    if (characterMotor)
                    {
                        float speed = characterMotor.velocity.magnitude;
                        float threshold = body.baseMoveSpeed;
                        if (speed > threshold)
                        {
                            int buffStack = Mathf.FloorToInt(((speed - threshold) / threshold) / (speedRequirementPerBuffStack / 100f));
                            if (buffStack > maxStack)
                            {
                                int newStacks = buffStack;
                                float buffTime = 6f + 4f * (float)(stack - 1);
                                // Refresh old stacks
                                foreach (var timedBuff in timedBuffs)
                                {
                                    if (buffStack <= 0) break;
                                    if ((BuffIndex)timedBuffIndex.GetValue(timedBuff) == buffIndex)
                                    {
                                        timedBuffTimer.SetValue(timedBuff, buffTime);
                                        newStacks--;
                                    }
                                }
                                // Add new stacks if we have leftover amount of newStacks
                                for (var i = 0; i < newStacks; i++) body.AddTimedBuff(buffIndex, buffTime);
                                maxStack = buffStack;
                            }
                        }
                    }
                }
                /*
                if (NetworkServer.active)
                {
                    CharacterMotor characterMotor = body.characterMotor;
                    if (characterMotor)
                    {
                        float speed = characterMotor.velocity.magnitude;
                        float threshold = body.moveSpeed * body.sprintingSpeedMultiplier;
                        if (speed > threshold)
                        {
                            int buffStack = Mathf.Min(Mathf.FloorToInt((float)maxDamageBoost + (float)(maxDamageBoost - 1) * Mathf.Max((speed - threshold) / (threshold * maxSpeedMultiplierRequirement), 0f)), maxDamageBoost);
                            float buffTime = 4f + 2f * (float)(stack - 1);
                            // Refresh old stacks
                            foreach (var timedBuff in timedBuffs)
                            {
                                if (buffStack <= 0) break;
                                if ((BuffIndex)timedBuffIndex.GetValue(timedBuff) == buffIndex)
                                {
                                    timedBuffTimer.SetValue(timedBuff, buffTime);
                                    buffStack--;
                                }
                            }
                            // Add new stacks if we have leftover amount of buffStack
                            for (var i = 0; i < buffStack; i++) body.AddTimedBuff(buffIndex, buffTime);
                        }
                    }
                }
                */
            }

            public IList timedBuffs;

            public static readonly FieldInfo timedBuffsField = typeof(CharacterBody).GetField("timedBuffs", BindingFlags.NonPublic | BindingFlags.Instance);
            public static readonly FieldInfo timedBuffTimer = typeof(CharacterBody).GetNestedType("TimedBuff", BindingFlags.NonPublic | BindingFlags.Instance).GetField("timer");
            public static readonly FieldInfo timedBuffIndex = typeof(CharacterBody).GetNestedType("TimedBuff", BindingFlags.NonPublic | BindingFlags.Instance).GetField("buffIndex");
        }
    }
}
