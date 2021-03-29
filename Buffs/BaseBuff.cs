using RoR2;
using R2API;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MysticsItems.Buffs
{
    public abstract class BaseBuff
    {
        public BuffDef buffDef;
        public static List<BaseBuff> loadedBuffs = new List<BaseBuff>();

        public virtual void OnLoad() { }

        public BuffDef Load()
        {
            buffDef = ScriptableObject.CreateInstance<BuffDef>();
            OnLoad();
            buffDef.iconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Buffs/" + buffDef.name + ".png");
            buffDef.name = Main.TokenPrefix + buffDef.name;
            loadedBuffs.Add(this);
            return buffDef;
        }

        private float StatModifierTimes(Main.GenericCharacterInfo genericCharacterInfo)
        {
            return genericCharacterInfo.body.HasBuff(buffDef) ? genericCharacterInfo.body.GetBuffCount(buffDef) : 0f;
        }
        private float StatModifierTimesNoStack(Main.GenericCharacterInfo genericCharacterInfo)
        {
            return genericCharacterInfo.body.HasBuff(buffDef) ? 1f : 0f;
        }
        public void AddModifier(List<CharacterStats.StatModifier> list, float multiplier, float flat, bool stacks)
        {
            CharacterStats.StatModifier statModifier = new CharacterStats.StatModifier
            {
                multiplier = multiplier,
                flat = flat,
                times = StatModifierTimes
            };
            if (!stacks) statModifier.times = StatModifierTimesNoStack;
            list.Add(statModifier);
        }
        public void AddModifier(List<CharacterStats.FlatStatModifier> list, float amount, bool stacks)
        {
            CharacterStats.FlatStatModifier statModifier = new CharacterStats.FlatStatModifier
            {
                amount = amount,
                times = StatModifierTimes
            };
            if (!stacks) statModifier.times = StatModifierTimesNoStack;
            list.Add(statModifier);
        }
        public void AddLevelModifier(int amount, bool stacks = true)
        {
            AddModifier(CharacterStats.levelModifiers, amount, stacks);
        }
        public void AddHealthModifier(float multiplier = 0f, float flat = 0f, bool stacks = true)
        {
            AddModifier(CharacterStats.healthModifiers, multiplier, flat, stacks);
        }
        public void AddShieldModifier(float multiplier = 0f, float flat = 0f, bool stacks = true)
        {
            AddModifier(CharacterStats.shieldModifiers, multiplier, flat, stacks);
        }
        public void AddRegenModifier(float amount, bool stacks = true)
        {
            AddModifier(CharacterStats.regenModifiers, amount, stacks);
        }
        public void AddMoveSpeedModifier(float multiplier = 0f, float flat = 0f, bool stacks = true)
        {
            AddModifier(CharacterStats.moveSpeedModifiers, multiplier, flat, stacks);
        }
        public void AddDamageModifier(float multiplier = 0f, float flat = 0f, bool stacks = true)
        {
            AddModifier(CharacterStats.damageModifiers, multiplier, flat, stacks);
        }
        public void AddAttackSpeedModifier(float multiplier = 0f, float flat = 0f, bool stacks = true)
        {
            AddModifier(CharacterStats.attackSpeedModifiers, multiplier, flat, stacks);
        }
        public void AddCritModifier(float amount, bool stacks = true)
        {
            AddModifier(CharacterStats.critModifiers, amount, stacks);
        }
        public void AddArmorModifier(float amount, bool stacks = true)
        {
            AddModifier(CharacterStats.armorModifiers, amount, stacks);
        }
        public void AddCooldownModifier(float multiplier = 0f, float flat = 0f, bool stacks = true)
        {
            AddModifier(CharacterStats.cooldownModifiers, multiplier, flat, stacks);
        }
    }
}
