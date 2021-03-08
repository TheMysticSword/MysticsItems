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
    }
}
