using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Collections.Generic;
using System.Linq;

namespace MysticsItems.Items
{
    public class ArtificerNanobots : BaseItem
    {
        public static List<GameObject> checkedProjectiles = new List<GameObject>();

        public override void PreLoad()
        {
			itemDef.name = "ArtificerNanobots";
			itemDef.tier = ItemTier.NoTier;
		}

        public override void OnLoad()
        {
            SetAssets("Proximity Nanobots");
			Main.HopooShaderToMaterial.Standard.Emission(GetModelMaterial());
			Main.HopooShaderToMaterial.Standard.Emission(GetFollowerModelMaterial());
			AddDisplayRule("MageBody", "HandL", new Vector3(-0.076F, -0.001F, 0.092F), new Vector3(4.907F, 241.513F, 278.407F), new Vector3(0.012F, 0.012F, 0.012F));
			AddDisplayRule("MageBody", "HandL", new Vector3(0.021F, -0.131F, 0.12F), new Vector3(358.086F, 282.82F, 271.422F), new Vector3(0.009F, 0.009F, 0.009F));
			AddDisplayRule("MageBody", "HandR", new Vector3(-0.117F, -0.064F, 0.055F), new Vector3(1.989F, 25.573F, 85.852F), new Vector3(0.008F, 0.008F, 0.008F));

			CharacterItems.SetCharacterItem(this, "MageBody");

			On.EntityStates.Mage.Weapon.FireFireBolt.FireGauntlet += (orig, self) =>
			{
				GameObject projectilePrefab = self.projectilePrefab;
				if (!checkedProjectiles.Contains(projectilePrefab))
				{
					checkedProjectiles.Add(projectilePrefab);
					MysticsItemsProjectileProximityMissController proximityMiss = projectilePrefab.AddComponent<MysticsItemsProjectileProximityMissController>();
					proximityMiss.itemIndex = itemDef.itemIndex;
				}
				orig(self);
			};
		}

		public class MysticsItemsProjectileProximityMissController : MonoBehaviour
		{
			public TeamIndex myTeamIndex
			{
				get
				{
					if (!teamFilter) return TeamIndex.Neutral;
					return teamFilter.teamIndex;
				}
			}

			public void Start()
			{
				if (NetworkServer.active)
				{
					projectileController = GetComponent<ProjectileController>();
					teamFilter = projectileController.teamFilter;
					projectileDamage = GetComponent<ProjectileDamage>();
					search = new BullseyeSearch();
					damageCoefficient = baseDamageCoefficient;
					if (itemIndex != ItemIndex.None)
					{
						CharacterBody ownerBody = projectileController.owner.GetComponent<CharacterBody>();
						if (ownerBody)
						{
							Inventory inventory = ownerBody.inventory;
							if (inventory && inventory.GetItemCount(itemIndex) > 0)
							{
								damageCoefficient += (1f - 1f / (1f + stackDamageCoefficient * (inventory.GetItemCount(itemIndex) - 1))) * (maxDamageCoefficient - baseDamageCoefficient);
								return;
							}
						}
						Object.Destroy(this);
						return;
					}
					return;
				}
				enabled = false;
			}

			public void FixedUpdate()
			{
				if (NetworkServer.active)
				{
					UpdateServer();
					return;
				}
				enabled = false;
			}

			public void UpdateServer()
			{
				Vector3 position = transform.position;
				List<HurtBox> hurtBoxes = FindNearbyTargets(position);
				foreach (HurtBox hurtBox in hurtBoxes)
                {
					if (!targetsNearby.Contains(hurtBox) && !targetsAttacked.Contains(hurtBox))
                    {
						targetsNearby.Add(hurtBox);
                    }
				}
				targetsNearby.RemoveAll(x => !x); // Remove non-existent targets from the list
				float sqrMissDistance = missDistance * missDistance;
				foreach (HurtBox hurtBox in targetsNearby)
                {
					if ((hurtBox.transform.position - position).sqrMagnitude > sqrMissDistance)
                    {
						targetsAttacked.Add(hurtBox);

						LightningOrb lightningOrb = new LightningOrb();
						lightningOrb.bouncedObjects = new List<HealthComponent>();
						lightningOrb.attacker = projectileController.owner;
						lightningOrb.inflictor = gameObject;
						lightningOrb.teamIndex = myTeamIndex;
						lightningOrb.damageValue = projectileDamage.damage * damageCoefficient;
						lightningOrb.isCrit = projectileDamage.crit;
						lightningOrb.origin = position;
						lightningOrb.bouncesRemaining = 0;
						lightningOrb.lightningType = lightningType;
						lightningOrb.procCoefficient = procCoefficient;
						lightningOrb.target = hurtBox;
						lightningOrb.damageColorIndex = projectileDamage.damageColorIndex;
						if (inheritDamageType)
						{
							lightningOrb.damageType = projectileDamage.damageType;
						}
						OrbManager.instance.AddOrb(lightningOrb);
					}
                }
				targetsNearby.RemoveAll(x => targetsAttacked.Contains(x)); // Remove attacked targets from the list
			}

			public List<HurtBox> FindNearbyTargets(Vector3 position)
			{
				search.searchOrigin = position;
				search.sortMode = BullseyeSearch.SortMode.Distance;
				search.teamMaskFilter = TeamMask.allButNeutral;
				search.teamMaskFilter.RemoveTeam(myTeamIndex);
				search.maxDistanceFilter = missDistance;
				search.RefreshCandidates();
				return search.GetResults().ToList();
			}

			public ProjectileController projectileController;
			public ProjectileDamage projectileDamage;
			public TeamFilter teamFilter;
			public float procCoefficient = 0.1f;
			public float damageCoefficient = 1f;
			public float baseDamageCoefficient = 0.2f;
			public float stackDamageCoefficient = 0.2f;
			public float maxDamageCoefficient = 1f;
			public ItemIndex itemIndex = ItemIndex.None;
			public bool inheritDamageType;
			public LightningOrb.LightningType lightningType = LightningOrb.LightningType.RazorWire;
			public float missDistance = 8f;
			public BullseyeSearch search;
			public List<HurtBox> targetsNearby = new List<HurtBox>();
			public List<HurtBox> targetsAttacked = new List<HurtBox>();
		}
	}
}
