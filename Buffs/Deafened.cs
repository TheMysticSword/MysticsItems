using RoR2;
using R2API.Utils;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Buffs
{
    public class Deafened : BaseBuff
    {
        public static float multiplier = 1.5f;
        
        public override void PreAdd() {
            buffDef.name = "Deafened";
            buffDef.buffColor = new Color32(255, 195, 112, 255);
            buffDef.isDebuff = true;
        }

        public override void OnAdd() {
            Equipment.Microphone.buffIndex = buffIndex;

            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                // movement speed reduction
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdcR4(1),
                    x => x.MatchStloc(55)
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<System.Func<CharacterBody, float>>((characterBody) =>
                    {
                        if (characterBody.HasBuff(buffIndex))
                        {
                            return 0.5f;
                        }
                        return 0;
                    });
                    c.Emit(OpCodes.Ldloc, 55);
                    c.Emit(OpCodes.Add);
                    c.Emit(OpCodes.Stloc, 55);
                }
                // armor reduction
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdarg(0),
                    x => x.MatchCallOrCallvirt<CharacterBody>("get_armor"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdcI4(0x2B),
                    x => x.MatchCallOrCallvirt<CharacterBody>("HasBuff"),
                    x => x.MatchBrtrue(out _),
                    x => x.MatchLdcR4(0),
                    x => x.MatchBr(out _),
                    x => x.MatchLdcR4(500),
                    x => x.MatchAdd(),
                    x => x.MatchCallOrCallvirt<CharacterBody>("set_armor")
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<System.Action<CharacterBody>>((characterBody) =>
                    {
                        if (characterBody.HasBuff(buffIndex))
                        {
                            characterBody.SetPropertyValue("armor", characterBody.armor - 20f);
                        }
                    });
                }
                // force skill value recalculation
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<System.Action<CharacterBody>>((characterBody) =>
                {
                    if (characterBody.HasBuff(buffIndex))
                    {
                        if (characterBody.skillLocator.primary) characterBody.skillLocator.primary.RecalculateValues();
                        if (characterBody.skillLocator.secondary) characterBody.skillLocator.secondary.RecalculateValues();
                        if (characterBody.skillLocator.utility) characterBody.skillLocator.utility.RecalculateValues();
                        if (characterBody.skillLocator.special) characterBody.skillLocator.special.RecalculateValues();
                    }
                });
            };
            // cooldown increase (can't do this in RecalculateStats because this function makes it so the modified cooldown can't be higher than the base cooldown)
            On.RoR2.GenericSkill.CalculateFinalRechargeInterval += (orig, self) =>
            {
                return orig(self) * (self.characterBody.HasBuff(buffIndex) ? multiplier : 1f);
            };

            // when the debuff is first received, add a few seconds to current skill cooldowns
            On.RoR2.CharacterBody.OnBuffFirstStackGained += (orig, self, buffDef) =>
            {
                if (buffDef.buffIndex == buffIndex)
                {
                    GenericSkill[] skills =
                    {
                        self.skillLocator.primary,
                        self.skillLocator.secondary,
                        self.skillLocator.utility,
                        self.skillLocator.special
                    };
                    foreach (GenericSkill skill in skills)
                    {
                        if (skill)
                        {
                            if (skill.stock > 0) skill.DeductStock(1);
                            skill.rechargeStopwatch = Mathf.Min(skill.CalculateFinalRechargeInterval() / multiplier, skill.rechargeStopwatch);
                        }
                    }
                }
                orig(self, buffDef);
            };
        }
    }
}
