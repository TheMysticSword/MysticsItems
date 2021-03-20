using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Collections.Generic;

namespace MysticsItems.Items
{
    public class AllyDeathRevenge : BaseItem
    {
        public static BuffIndex buffIndex;

        public override void PreAdd()
        {
            itemDef.name = "AllyDeathRevenge";
            itemDef.tier = ItemTier.Tier2;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage,
                ItemTag.Utility
            };
            SetUnlockable();
            SetAssets("Ally Death Revenge");
            SetModelPanelDistance(0.75f, 1.5f);
            CopyModelToFollower();
            model.transform.Find("mdlAllyDeathRevenge").Rotate(new Vector3(0f, 0f, 160f), Space.Self);
            model.transform.Find("mdlAllyDeathRevenge").localScale *= 0.8f;

            AddDisplayRule((int)Main.CommonBodyIndices.Commando, "LowerArmR", new Vector3(0.001F, 0.274F, -0.078F), new Vector3(7.29F, 186.203F, 0.157F), new Vector3(0.277F, 0.389F, 0.277F));
            AddDisplayRule("mdlHuntress", "HandL", new Vector3(-0.014F, 0.004F, 0.035F), new Vector3(6.909F, 1.748F, 74.816F), new Vector3(0.187F, 0.174F, 0.187F));
            AddDisplayRule("mdlToolbot", "HandR", new Vector3(-0.059F, 0.587F, 1.939F), new Vector3(356.736F, 85.148F, 90.496F), new Vector3(3.014F, 3.241F, 3.014F));
            AddDisplayRule("mdlEngi", "HandL", new Vector3(0F, 0.104F, 0.042F), new Vector3(3.001F, 0F, 0F), new Vector3(0.259F, 0.259F, 0.259F));
            AddDisplayRule((int)Main.CommonBodyIndices.EngiTurret, "Head", new Vector3(0.026F, 0.602F, -1.541F), new Vector3(22.044F, 48.281F, 206.737F), new Vector3(0.74F, 0.74F, 0.74F));
            AddDisplayRule((int)Main.CommonBodyIndices.EngiWalkerTurret, "Head", new Vector3(-0.248F, 1.434F, -0.84F), new Vector3(300.601F, 223.502F, 297.144F), new Vector3(0.659F, 0.801F, 0.643F));
            AddDisplayRule("mdlMage", "HandL", new Vector3(-0.011F, 0.074F, 0.104F), new Vector3(0F, 0F, 355.462F), new Vector3(0.22F, 0.22F, 0.22F));
            AddDisplayRule("mdlMerc", "HandR", new Vector3(0F, 0.112F, 0.103F), new Vector3(14.285F, 0F, 0F), new Vector3(0.427F, 0.427F, 0.427F));
            AddDisplayRule("mdlTreebot", "WeaponPlatform", new Vector3(0F, 0.889F, 0.308F), new Vector3(0F, 0F, 0F), new Vector3(0.846F, 0.846F, 0.846F));
            AddDisplayRule("mdlLoader", "MechHandL", new Vector3(-0.073F, 0.379F, 0.15F), new Vector3(5.558F, 330.424F, 0F), new Vector3(0.36F, 0.36F, 0.36F));
            AddDisplayRule("mdlCroco", "HandL", new Vector3(-1.286F, 0.394F, 0.102F), new Vector3(56.075F, 280.047F, 0F), new Vector3(3.614F, 2.545F, 4.003F));
            AddDisplayRule("mdlCaptain", "HandR", new Vector3(-0.086F, 0.125F, 0.016F), new Vector3(14.676F, 274.88F, 359.215F), new Vector3(0.248F, 0.248F, 0.248F));
            AddDisplayRule("mdlBrother", "HandL", BrotherInfection.green, new Vector3(0.019F, -0.013F, 0.017F), new Vector3(348.105F, 324.594F, 242.165F), new Vector3(0.061F, 0.019F, 0.061F));
            AddDisplayRule("mdlScav", "HandL", new Vector3(-3.491F, 2.547F, -2.4F), new Vector3(354.216F, 329.486F, 87.688F), new Vector3(7.501F, 7.7F, 7.501F));
        }

        public override void OnAdd()
        {
            On.RoR2.CharacterMaster.Awake += (orig, self) =>
            {
                orig(self);
                self.gameObject.AddComponent<SurvivedStageCounter>();
            };

            On.RoR2.Stage.Start += (orig, self) =>
            {
                orig(self);
                foreach (CharacterMaster characterMaster in CharacterMaster.readOnlyInstancesList)
                {
                    SurvivedStageCounter component = characterMaster.GetComponent<SurvivedStageCounter>();
                    if (component)
                    {
                        component.count++;
                    }
                }
            };

            On.RoR2.CharacterMaster.OnBodyDeath += (orig, self, body) =>
            {
                orig(self, body);
                GameObject playSoundObject = null;
                if (NetworkServer.active)
                {
                    TeamIndex teamIndex = TeamComponent.GetObjectTeam(body.gameObject);
                    foreach (CharacterBody body2 in CharacterBody.readOnlyInstancesList)
                    {
                        Inventory inventory = body2.inventory;
                        if (inventory && inventory.GetItemCount(itemIndex) > 0 && TeamComponent.GetObjectTeam(body2.gameObject) == teamIndex)
                        {
                            playSoundObject = body2.gameObject;

                            float time = 15f + 5f * (inventory.GetItemCount(itemIndex) - 1);
                            float sameStageDeathTime = 2f + 0.5f * (inventory.GetItemCount(itemIndex) - 1);
                            if (body.master)
                            {
                                SurvivedStageCounter survivedStageCounter = body.master.GetComponent<SurvivedStageCounter>();
                                if (survivedStageCounter && survivedStageCounter.count <= 0)
                                {
                                    time = sameStageDeathTime;
                                    playSoundObject = null;
                                }
                            }
                            else
                            {
                                // Masterless bodies don't get moved to the next stage anyway, so they definitely died on the same stage
                                time = sameStageDeathTime;
                                playSoundObject = null;
                            }
                            body2.AddTimedBuff(buffIndex, time);
                        }
                    }
                }
                if (playSoundObject) Util.PlaySound("Play_item_allydeathrevenge_proc", playSoundObject);
            };

            Main.Overlays.CreateOverlay(Main.AssetBundle.LoadAsset<Material>("Assets/Misc/Materials/matAllyDeathRevengeOverlay.mat"), delegate (CharacterModel model)
            {
                return model.body.HasBuff(buffIndex);
            });
        }

        public class SurvivedStageCounter : MonoBehaviour
        {
            public int count = 0;
        }
    }
}
