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
        public BuffDef buffDef = new BuffDef();
        public BuffIndex buffIndex;
        public static Dictionary<System.Type, BaseBuff> registeredBuffs = new Dictionary<System.Type, BaseBuff>();

        public static BuffIndex GetFromType(System.Type type)
        {
            if (registeredBuffs.ContainsKey(type))
            {
                return registeredBuffs[type].buffIndex;
            }
            return BuffIndex.None;
        }

        public void Add()
        {
            registeredBuffs.Add(GetType(), this);
            PreAdd();
            buffDef.iconPath = Main.AssetPrefix + ":Assets/Buffs/" + buffDef.name + ".png";
            buffDef.name = Main.TokenPrefix + buffDef.name;
            buffIndex = BuffAPI.Add(new CustomBuff(buffDef));
            OnAdd();
        }

        public virtual void PreAdd() { }

        public virtual void OnAdd() { }

        private float StatModifierTimes(Main.GenericCharacterInfo genericCharacterInfo)
        {
            return genericCharacterInfo.body.HasBuff(buffIndex) ? genericCharacterInfo.body.GetBuffCount(buffIndex) : 0f;
        }
        public void AddLevelModifier(int amount)
        {
            CharacterStats.levelModifiers.Add(new CharacterStats.StatModifier
            {
                flat = amount,
                times = StatModifierTimes
            });
        }
        public void AddHealthModifier(float multiplier = 0f, float flat = 0f)
        {
            CharacterStats.healthModifiers.Add(new CharacterStats.StatModifier
            {
                multiplier = multiplier,
                flat = flat,
                times = StatModifierTimes
            });
        }
        public void AddShieldModifier(float multiplier = 0f, float flat = 0f)
        {
            CharacterStats.shieldModifiers.Add(new CharacterStats.StatModifier
            {
                multiplier = multiplier,
                flat = flat,
                times = StatModifierTimes
            });
        }
        public void AddRegenModifier(float flat)
        {
            CharacterStats.regenModifiers.Add(new CharacterStats.StatModifier
            {
                flat = flat,
                times = StatModifierTimes
            });
        }
        public void AddMoveSpeedModifier(float multiplier = 0f, float flat = 0f)
        {
            CharacterStats.moveSpeedModifiers.Add(new CharacterStats.StatModifier
            {
                multiplier = multiplier,
                flat = flat,
                times = StatModifierTimes
            });
        }
        public void AddDamageModifier(float multiplier = 0f, float flat = 0f)
        {
            CharacterStats.damageModifiers.Add(new CharacterStats.StatModifier
            {
                multiplier = multiplier,
                flat = flat,
                times = StatModifierTimes
            });
        }
        public void AddAttackSpeedModifier(float multiplier = 0f, float flat = 0f)
        {
            CharacterStats.attackSpeedModifiers.Add(new CharacterStats.StatModifier
            {
                multiplier = multiplier,
                flat = flat,
                times = StatModifierTimes
            });
        }
    }
}
