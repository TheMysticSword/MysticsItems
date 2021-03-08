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
            model.transform.Find("mdlAllyDeathRevenge").Rotate(new Vector3(0f, 0f, 160f), Space.Self);
            model.transform.Find("mdlAllyDeathRevenge").localScale *= 0.8f;
            CopyModelToFollower();
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
                if (NetworkServer.active)
                {
                    TeamIndex teamIndex = body.teamComponent.teamIndex;
                    foreach (CharacterBody body2 in CharacterBody.readOnlyInstancesList)
                    {
                        Inventory inventory = body2.inventory;
                        if (inventory && inventory.GetItemCount(itemIndex) > 0 && body2.teamComponent.teamIndex == teamIndex)
                        {
                            float time = 15f + 5f * (inventory.GetItemCount(itemIndex) - 1);
                            if (body.master)
                            {
                                SurvivedStageCounter survivedStageCounter = body.master.GetComponent<SurvivedStageCounter>();
                                if (survivedStageCounter && survivedStageCounter.count <= 0)
                                {
                                    time = 2f;
                                }
                            }
                            else
                            {
                                // Masterless bodies don't get moved to the next stage anyway, so they definitely died on the same stage
                                time = 2f;
                            }
                            body2.AddTimedBuff(buffIndex, time);
                            Util.PlaySound("Play_item_allydeathrevenge_proc", body2.gameObject);
                        }
                    }
                }
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
