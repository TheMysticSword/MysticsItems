using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Collections.Generic;
using RoR2.Audio;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.BalanceConfigManager;

namespace MysticsItems.Items
{
    public class AllyDeathRevenge : BaseItem
    {
        public static NetworkSoundEventDef sfx;

        public static ConfigurableValue<float> duration = new ConfigurableValue<float>(
            "Item: Vendetta",
            "Duration",
            16f,
            "How long should the buff last (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_ALLYDEATHREVENGE_DESC"
            }
        );
        public static ConfigurableValue<float> durationPerStack = new ConfigurableValue<float>(
            "Item: Vendetta",
            "DurationPerStack",
            16f,
            "How long should the buff last for each additional stack of this item (in seconds)",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_ALLYDEATHREVENGE_DESC"
            }
        );

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_AllyDeathRevenge";
            itemDef.tier = ItemTier.Tier2;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Damage,
                ItemTag.Utility
            };
            MysticsItemsContent.Resources.unlockableDefs.Add(GetUnlockableDef());
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Ally Death Revenge/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Ally Death Revenge/Icon.png");
            ModelPanelParameters modelPanelParams = itemDef.pickupModelPrefab.GetComponentInChildren<ModelPanelParameters>();
            modelPanelParams.minDistance = 0.75f;
            modelPanelParams.maxDistance = 1.5f;
            itemDisplayPrefab = PrepareItemDisplayModel(PrefabAPI.InstantiateClone(itemDef.pickupModelPrefab, itemDef.pickupModelPrefab.name + "Display", false));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "LowerArmR", new Vector3(0.001F, 0.274F, -0.078F), new Vector3(7.29F, 186.203F, 0.157F), new Vector3(0.277F, 0.389F, 0.277F));
                AddDisplayRule("HuntressBody", "HandL", new Vector3(-0.014F, 0.004F, 0.035F), new Vector3(6.909F, 1.748F, 74.816F), new Vector3(0.187F, 0.174F, 0.187F));
                AddDisplayRule("Bandit2Body", "Stomach", new Vector3(-0.069F, -0.12F, -0.197F), new Vector3(18.152F, 14.491F, 196.624F), new Vector3(0.348F, 0.348F, 0.348F));
                AddDisplayRule("ToolbotBody", "HandR", new Vector3(-0.059F, 0.587F, 1.939F), new Vector3(356.736F, 85.148F, 90.496F), new Vector3(3.014F, 3.241F, 3.014F));
                AddDisplayRule("EngiBody", "HandL", new Vector3(0F, 0.104F, 0.042F), new Vector3(3.001F, 0F, 0F), new Vector3(0.259F, 0.259F, 0.259F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0.026F, 0.602F, -1.541F), new Vector3(22.044F, 48.281F, 206.737F), new Vector3(0.74F, 0.74F, 0.74F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(-0.248F, 1.434F, -0.84F), new Vector3(300.601F, 223.502F, 297.144F), new Vector3(0.659F, 0.801F, 0.643F));
                AddDisplayRule("MageBody", "HandL", new Vector3(-0.011F, 0.074F, 0.104F), new Vector3(0F, 0F, 355.462F), new Vector3(0.22F, 0.22F, 0.22F));
                AddDisplayRule("MercBody", "HandR", new Vector3(0F, 0.112F, 0.103F), new Vector3(14.285F, 0F, 0F), new Vector3(0.427F, 0.427F, 0.427F));
                AddDisplayRule("TreebotBody", "WeaponPlatform", new Vector3(0F, 0.889F, 0.308F), new Vector3(0F, 0F, 0F), new Vector3(0.846F, 0.846F, 0.846F));
                AddDisplayRule("LoaderBody", "MechHandL", new Vector3(-0.073F, 0.379F, 0.15F), new Vector3(5.558F, 330.424F, 0F), new Vector3(0.36F, 0.36F, 0.36F));
                AddDisplayRule("CrocoBody", "HandL", new Vector3(-1.286F, 0.394F, 0.102F), new Vector3(56.075F, 280.047F, 0F), new Vector3(3.614F, 2.545F, 4.003F));
                AddDisplayRule("CaptainBody", "HandR", new Vector3(-0.086F, 0.125F, 0.016F), new Vector3(14.676F, 274.88F, 359.215F), new Vector3(0.248F, 0.248F, 0.248F));
                AddDisplayRule("BrotherBody", "HandL", BrotherInfection.green, new Vector3(0.019F, -0.013F, 0.017F), new Vector3(348.105F, 324.594F, 242.165F), new Vector3(0.061F, 0.019F, 0.061F));
                AddDisplayRule("ScavBody", "HandL", new Vector3(-3.491F, 2.547F, -2.4F), new Vector3(354.216F, 329.486F, 87.688F), new Vector3(7.501F, 7.7F, 7.501F));
            };
            itemDef.pickupModelPrefab.transform.Find("mdlAllyDeathRevenge").Rotate(new Vector3(0f, 0f, 160f), Space.Self);
            itemDef.pickupModelPrefab.transform.Find("mdlAllyDeathRevenge").localScale *= 0.8f;

            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;

            Overlays.CreateOverlay(Main.AssetBundle.LoadAsset<Material>("Assets/Items/Ally Death Revenge/matAllyDeathRevengeOverlay.mat"), delegate (CharacterModel model)
            {
                return model.body.HasBuff(MysticsItemsContent.Buffs.MysticsItems_AllyDeathRevenge);
            });

            GameObject burningVFX = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Ally Death Revenge/BurningVFX.prefab");
            CustomTempVFXManagement.MysticsRisky2UtilsTempVFX tempVFX = burningVFX.AddComponent<CustomTempVFXManagement.MysticsRisky2UtilsTempVFX>();
            tempVFX.rotateWithParent = true;
            tempVFX.enterObjects = new GameObject[]
            {
                burningVFX.transform.Find("Origin").gameObject
            };
            Material matBurningVFX = burningVFX.transform.Find("Origin/Left").gameObject.GetComponent<Renderer>().sharedMaterial;
            HopooShaderToMaterial.CloudRemap.Apply(
                matBurningVFX,
                Main.AssetBundle.LoadAsset<Texture>("Assets/Items/Ally Death Revenge/texRampAllyDeathRevengeBurningEyes.png")
            );
            HopooShaderToMaterial.CloudRemap.Boost(matBurningVFX, 3f);
            burningVFX.transform.Find("Origin").gameObject.AddComponent<RotateObject>().rotationSpeed = new Vector3(0f, 400f, 0f);
            CustomTempVFXManagement.allVFX.Add(new CustomTempVFXManagement.VFXInfo
            {
                prefab = burningVFX,
                condition = (x) => x.HasBuff(MysticsItemsContent.Buffs.MysticsItems_AllyDeathRevenge),
                radius = CustomTempVFXManagement.DefaultRadiusCall,
                child = "Head"
            });

            sfx = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            sfx.eventName = "Play_item_allydeathrevenge_proc";
            MysticsItemsContent.Resources.networkSoundEventDefs.Add(sfx);
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (NetworkServer.active && damageReport.victimMaster && damageReport.victimMaster.GetKillerBodyIndex() != BodyIndex.None)
            {
                foreach (var teamMember in TeamComponent.GetTeamMembers(damageReport.victimTeamIndex))
                {
                    if (teamMember.body && teamMember.body.inventory)
                    {
                        int itemCount = teamMember.body.inventory.GetItemCount(itemDef);
                        if (itemCount > 0)
                        {
                            EntitySoundManager.EmitSoundServer(sfx.index, teamMember.body.gameObject);
                            teamMember.body.AddTimedBuff(MysticsItemsContent.Buffs.MysticsItems_AllyDeathRevenge, duration + durationPerStack * (itemCount - 1));
                        }
                    }
                }
            }
        }
    }
}
