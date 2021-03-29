using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace MysticsItems
{
    public class GenericGameEvents
    {
        public struct GenericCharacterInfo
        {
            public GameObject gameObject;
            public CharacterBody body;
            public CharacterMaster master;
            public TeamComponent teamComponent;
            public HealthComponent healthComponent;
            public Inventory inventory;
            public TeamIndex teamIndex;
            public Vector3 aimOrigin;

            public GenericCharacterInfo(CharacterBody body)
            {
                this.body = body;
                gameObject = body ? body.gameObject : null;
                master = body ? body.master : null;
                teamComponent = body ? body.teamComponent : null;
                healthComponent = body ? body.healthComponent : null;
                inventory = master ? master.inventory : null;
                teamIndex = teamComponent ? teamComponent.teamIndex : TeamIndex.Neutral;
                aimOrigin = body ? body.aimOrigin : Random.insideUnitSphere.normalized;
            }
        }

        public static event System.Action<DamageInfo, GenericCharacterInfo, GenericCharacterInfo> OnHitEnemy;
        public static event System.Action<DamageInfo, GenericCharacterInfo> OnHitAll;
        public static event System.Action<DamageInfo, GenericCharacterInfo> BeforeDealDamage;
        public static event System.Action<DamageInfo, GenericCharacterInfo> BeforeTakeDamage;
        public static event System.Action<DamageInfo, GenericCharacterInfo> OnTakeDamage;

        public static void ErrorHookFailed(string name)
        {
            Main.logger.LogError("generic game event '" + name + "' hook failed");
        }
        public static void Init()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, damageInfo, victim) =>
            {
                orig(self, damageInfo, victim);
                if (damageInfo.attacker && damageInfo.procCoefficient > 0f)
                {
                    CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    CharacterBody victimBody = victim ? victim.GetComponent<CharacterBody>() : null;
                    GenericCharacterInfo attackerInfo = new GenericCharacterInfo(attackerBody);
                    GenericCharacterInfo victimInfo = new GenericCharacterInfo(victimBody);
                    if (attackerBody && victimBody && OnHitEnemy != null) OnHitEnemy(damageInfo, attackerInfo, victimInfo);
                }
            };

            On.RoR2.GlobalEventManager.OnHitAll += (orig, self, damageInfo, hitObject) =>
            {
                orig(self, damageInfo, hitObject);
                if (damageInfo.attacker)
                {
                    CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    GenericCharacterInfo attackerInfo = new GenericCharacterInfo(attackerBody);
                    if (attackerBody && OnHitAll != null) OnHitAll(damageInfo, attackerInfo);
                }
            };

            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                    MoveType.AfterLabel,
                    x => x.MatchLdarg(1),
                    x => x.MatchLdfld<DamageInfo>("damage"),
                    x => x.MatchStloc(6)
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldarg_1);
                    c.Emit(OpCodes.Ldloc_1);
                    c.EmitDelegate<System.Action<HealthComponent, DamageInfo, CharacterBody>>((healthComponent, damageInfo, attackerBody) =>
                    {
                        CharacterBody victimBody = healthComponent.body;
                        GenericCharacterInfo attackerInfo = new GenericCharacterInfo(attackerBody);
                        GenericCharacterInfo victimInfo = new GenericCharacterInfo(victimBody);
                        if (attackerBody && BeforeDealDamage != null) BeforeDealDamage(damageInfo, attackerInfo);
                        if (victimBody && BeforeTakeDamage != null) BeforeTakeDamage(damageInfo, victimInfo);
                    });
                }
                else
                {
                    ErrorHookFailed("before take damage");
                }
                ILLabel label = null;
                if (c.TryGotoNext(
                    x => x.MatchLdcR4(100f),
                    x => x.MatchMul(),
                    x => x.MatchLdcR4(0),
                    x => x.MatchLdnull(),
                    x => x.MatchCallOrCallvirt(typeof(Util), "CheckRoll"),
                    x => x.MatchBrfalse(out label)
                ))
                {
                    c.GotoLabel(label);
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldarg_1);
                    c.Emit(OpCodes.Ldloc_1);
                    c.EmitDelegate<System.Action<HealthComponent, DamageInfo, CharacterBody>>((healthComponent, damageInfo, attackerBody) =>
                    {
                        CharacterBody victimBody = healthComponent.body;
                        GenericCharacterInfo attackerInfo = new GenericCharacterInfo(attackerBody);
                        GenericCharacterInfo victimInfo = new GenericCharacterInfo(victimBody);
                        if (victimBody && OnTakeDamage != null) OnTakeDamage(damageInfo, victimInfo);
                    });
                }
                else
                {
                    ErrorHookFailed("on take damage");
                }
            };
        }
    }
}