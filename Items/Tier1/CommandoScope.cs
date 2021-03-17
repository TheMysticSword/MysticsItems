using RoR2;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Items
{
    public class CommandoScope : BaseItem
    {
        public override void PreAdd()
        {
            itemDef.name = "CommandoScope";
            itemDef.tier = ItemTier.Tier1;
            SetAssets("Commando Scope");
            AddDisplayRule((int)Main.CommonBodyIndices.Commando, "MuzzleLeft", new Vector3(0.0617f, 0.0008f, -0.1063f), new Vector3(-80.871f, -161.934f, 69.76801f), new Vector3(0.023f, 0.023f, 0.023f));
            AddDisplayRule((int)Main.CommonBodyIndices.Commando, "MuzzleRight", new Vector3(0.0617f, 0.0008f, -0.1063f), new Vector3(-80.871f, -161.934f, 69.76801f), new Vector3(0.023f, 0.023f, 0.023f));

            dontLoad = true;
        }

        public static float CalculateExtraDistance(int stacks)
        {
            return 5f + 5f * (stacks - 1);
        }

        public override void OnAdd()
        {
            CharacterItems.SetCharacterItem(this, "CommandoBody");

            On.RoR2.CharacterBody.Start += (orig, self) =>
            {
                orig(self);
                self.gameObject.AddComponent<ScopeDataHolder>();
            };
            On.EntityStates.Commando.CommandoWeapon.FirePistol.FireBullet += (orig, self, targetMuzzle) =>
            {
                ScopeDataHolder scopeDataHolder = self.outer.gameObject.GetComponent<ScopeDataHolder>();
                if (scopeDataHolder)
                {
                    scopeDataHolder.applyAttackChanges++;
                }
                orig(self, targetMuzzle);
            };
            On.EntityStates.Commando.CommandoWeapon.FirePistol2.FireBullet += (orig, self, targetMuzzle) =>
            {
                ScopeDataHolder scopeDataHolder = self.outer.gameObject.GetComponent<ScopeDataHolder>();
                if (scopeDataHolder)
                {
                    scopeDataHolder.applyAttackChanges++;
                }
                orig(self, targetMuzzle);
            };
            On.RoR2.BulletAttack.Fire += (orig, self) =>
            {
                if (self.owner)
                {
                    CharacterBody body = self.owner.GetComponent<CharacterBody>();
                    ScopeDataHolder scopeDataHolder = self.owner.gameObject.GetComponent<ScopeDataHolder>();
                    if (body && scopeDataHolder && scopeDataHolder.applyAttackChanges > 0)
                    {
                        Inventory inventory = body.inventory;
                        if (inventory && inventory.GetItemCount(itemIndex) > 0)
                        {
                            self.maxDistance += CalculateExtraDistance(inventory.GetItemCount(itemIndex));
                        }
                    }
                }
                orig(self);
            };
            IL.RoR2.BulletAttack.DefaultHitCallback += (il) =>
            {
                ILCursor c = new ILCursor(il);
                ILLabel[] switchLabels = null;
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdcR4(1),
                    x => x.MatchStloc(4)
                ) && c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<BulletAttack>("falloffModel"),
                    x => x.MatchStloc(9),
                    x => x.MatchLdloc(9),
                    x => x.MatchSwitch(out switchLabels)
                ))
                {
                    c.GotoLabel(switchLabels[1]);
                    if (c.TryGotoNext(
                        MoveType.After,
                        x => x.MatchMul(),
                        x => x.MatchAdd(),
                        x => x.MatchStloc(4)
                    ))
                    {
                        c.Emit(OpCodes.Ldarg, 0);
                        c.Emit(OpCodes.Ldarg, 1);
                        c.Emit(OpCodes.Ldfld, typeof(BulletAttack.BulletHit).GetField("distance"));
                        c.Emit(OpCodes.Ldloc, 4);
                        c.EmitDelegate<System.Func<BulletAttack, float, float, float>>((bulletAttack, distance, distanceDamageMultiplier) =>
                        {
                            if (distanceDamageMultiplier < 1f) // In case the multiplier is already 1 or higher, don't do anything
                            {
                                if (bulletAttack.owner)
                                {
                                    CharacterBody body = bulletAttack.owner.GetComponent<CharacterBody>();
                                    ScopeDataHolder scopeDataHolder = bulletAttack.owner.GetComponent<ScopeDataHolder>();
                                    if (body && scopeDataHolder && scopeDataHolder.applyAttackChanges > 0)
                                    {
                                        Inventory inventory = body.inventory;
                                        if (inventory && inventory.GetItemCount(itemIndex) > 0)
                                        {
                                            distanceDamageMultiplier = 0.5f + Mathf.Clamp01(Mathf.InverseLerp(60f + CalculateExtraDistance(inventory.GetItemCount(itemIndex)) * 2f, 25f + CalculateExtraDistance(inventory.GetItemCount(itemIndex)), distance)) * 0.5f;
                                            scopeDataHolder.applyAttackChanges--;
                                        }
                                    }
                                }
                            }
                            return distanceDamageMultiplier;
                        });
                        c.Emit(OpCodes.Stloc, 4);
                    }
                }
            };
        }

        public class ScopeDataHolder : MonoBehaviour
        {
            public int applyAttackChanges = 0;
        }
    }
}
