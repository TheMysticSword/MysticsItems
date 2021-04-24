using RoR2;
using System.Collections.Generic;
using MysticsItems.Items;
using Starstorm2;
using Starstorm2.Cores.Items;
using MonoMod.RuntimeDetour;
using System;
using UnityEngine;
using System.Reflection;
using UnityEngine.Networking;

namespace MysticsItems.SoftDependencies
{
    public static class Starstorm2SoftDependency
    {
        public const string PluginGUID = "com.TeamMoonstorm.Starstorm2";

        public static void Init()
        {
            // Contraband Gunpowder explodes on Stirring Soul pickup
            new Hook(typeof(SoulPickup).GetMethod("OnTriggerStay", Main.bindingFlagAll), new Action<Action<SoulPickup, Collider>, SoulPickup, Collider>((orig, self, other) =>
            {
                bool flag = NetworkServer.active && self.alive && TeamComponent.GetObjectTeam(other.gameObject) == self.team.teamIndex;
                orig(self, other);
                if (flag && !self.alive)
                {
                    CharacterBody body = other.GetComponent<CharacterBody>();
                    if (body)
                    {
                        Inventory inventory = body.inventory;
                        if (inventory && inventory.GetItemCount(MysticsItemsContent.Items.ExplosivePickups) > 0)
                        {
                            ExplosivePickups.Explode(body);
                        }
                    }
                }
            }));
        }
    }
}
