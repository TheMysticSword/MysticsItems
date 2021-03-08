using RoR2;
using R2API.Utils;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;

namespace MysticsItems
{
    public static class Unlockables
    {
        public static Dictionary<string, UnlockableDef> registeredUnlockables = new Dictionary<string, UnlockableDef>();

        public static void Register(string name, UnlockableDef def)
        {
            registeredUnlockables.Add(name, def);
        }

        public static void Init()
        {
            IL.RoR2.UnlockableCatalog.Init += (il) =>
            {
                ILCursor c = new ILCursor(il);

                if (c.TryGotoNext(
                    MoveType.AfterLabel,
                    x => x.MatchLdsfld(typeof(UnlockableCatalog).GetField("nameToDefTable", Main.bindingFlagAll)),
                    x => x.MatchCallOrCallvirt(out _),
                    x => x.MatchNewarr<UnlockableDef>(),
                    x => x.MatchStsfld(typeof(UnlockableCatalog).GetField("indexToDefTable", Main.bindingFlagAll))
                ))
                {
                    c.EmitDelegate<System.Action>(() =>
                    {
                        foreach (KeyValuePair<string, UnlockableDef> keyValuePair in registeredUnlockables)
                        {
                            typeof(UnlockableCatalog).InvokeMethod("RegisterUnlockable", keyValuePair.Key, keyValuePair.Value);
                        }
                    });
                }
            };
        }
    }
}
