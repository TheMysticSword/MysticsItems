using RoR2;
using R2API.Utils;
using UnityEngine;
using System;
using MysticsRisky2Utils;
using MysticsRisky2Utils.BaseAssetTypes;
using R2API;
using static MysticsItems.LegacyBalanceConfigManager;
using UnityEngine.Networking;

namespace MysticsItems.Items
{
    public class VyraelCommandments : BaseItem
    {
        public static ConfigurableValue<int> hits = new ConfigurableValue<int>(
            "Item: Ten Commandments of Vyrael",
            "Hits",
            10,
            "Hits required for triggering this item's effect",
            new System.Collections.Generic.List<string>()
            {
                "ITEM_MYSTICSITEMS_VYRAELCOMMANDMENTS_PICKUP",
                "ITEM_MYSTICSITEMS_VYRAELCOMMANDMENTS_DESC"
            }
        );

        public static GameObject procVFX;

        public override void OnLoad()
        {
            base.OnLoad();
            itemDef.name = "MysticsItems_VyraelCommandments";
            SetItemTierWhenAvailable(ItemTier.Tier3);
            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility
            };
            
            itemDef.pickupModelPrefab = PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Ten Commandments of Vyrael/Model.prefab"));
            itemDef.pickupIconSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/Items/Ten Commandments of Vyrael/Icon.png");
            Material mat = itemDef.pickupModelPrefab.transform.GetComponentInChildren<Renderer>().sharedMaterial;
            HopooShaderToMaterial.Standard.Apply(mat);
            HopooShaderToMaterial.Standard.Gloss(mat, 0.3f, 10f, new Color32(255, 253, 163, 255));

            ModelPanelParameters modelPanelParameters = itemDef.pickupModelPrefab.GetComponent<ModelPanelParameters>();
            modelPanelParameters.minDistance = 9.5f;
            modelPanelParameters.maxDistance = 15f;

            itemDisplayPrefab = PrepareItemDisplayModel(PrepareModel(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Ten Commandments of Vyrael/DisplayModel.prefab")));
            itemDisplayPrefab.transform.Find("ProcVFXPivot").gameObject.AddComponent<MysticsItemsVyraelCommandmentsEffect>();
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "LowerArmR", new Vector3(-0.04456F, 0.33549F, -0.07683F), new Vector3(0F, 182.0939F, 354.8922F), new Vector3(0.11933F, 0.11933F, 0.11933F));
                AddDisplayRule("HuntressBody", "Muzzle", new Vector3(0F, -0.02274F, 0F), new Vector3(90F, 0F, 0F), new Vector3(0.099F, 0.09212F, 0.099F));
                AddDisplayRule("Bandit2Body", "MainWeapon", new Vector3(-0.14985F, 0.85556F, -0.00516F), new Vector3(0F, 85.98335F, 0F), new Vector3(0.07573F, 0.07573F, 0.07573F));
                AddDisplayRule("ToolbotBody", "HandR", new Vector3(0.191F, 0.73729F, 1.91911F), new Vector3(356.736F, 85.148F, 89.97712F), new Vector3(0.93071F, 1.00082F, 0.93071F));
                AddDisplayRule("EngiBody", "HandR", new Vector3(0.09099F, 0.14777F, -0.0519F), new Vector3(24.80013F, 176.6263F, 79.46233F), new Vector3(0.10693F, 0.10693F, 0.10693F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(-0.007F, 0.55275F, 1.25599F), new Vector3(0F, 90F, 90F), new Vector3(0.35706F, 0.35706F, 0.35706F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.0057F, 0.78448F, 0.831F), new Vector3(0.234F, 271.606F, 270.571F), new Vector3(0.31878F, 0.38771F, 0.31095F));
                AddDisplayRule("MageBody", "LowerArmR", new Vector3(0.03837F, 0.31418F, 0.07639F), new Vector3(0F, 0F, 0F), new Vector3(0.08926F, 0.08926F, 0.08926F));
                AddDisplayRule("MercBody", "HandR", new Vector3(0.12937F, 0.19176F, 0.02447F), new Vector3(0F, 0F, 279.658F), new Vector3(0.1108F, 0.1108F, 0.1108F));
                AddDisplayRule("TreebotBody", "MuzzleSyringe", new Vector3(0.00002F, -0.22914F, -0.37278F), new Vector3(90F, 0F, 0F), new Vector3(0.17589F, 0.17589F, 0.17589F));
                AddDisplayRule("LoaderBody", "MechHandRight", new Vector3(0.15519F, 0.30881F, -0.01494F), new Vector3(2.73792F, 180.065F, 91.36073F), new Vector3(0.12046F, 0.12046F, 0.12046F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(-0.13273F, 2.94508F, -0.52332F), new Vector3(340.0608F, 175.3457F, 271.5903F), new Vector3(1.271F, 1.271F, 1.271F));
                AddDisplayRule("CaptainBody", "HandR", new Vector3(0.01675F, 0.15901F, 0.156F), new Vector3(6.32065F, 89.68613F, 87.15139F), new Vector3(0.10349F, 0.10349F, 0.10349F));
                AddDisplayRule("BrotherBody", "HandR", BrotherInfection.red, new Vector3(-0.01763F, 0.08509F, 0.0165F), new Vector3(28.59401F, 22.166F, 205.7942F), new Vector3(0.06467F, 0.06467F, 0.06467F));
                AddDisplayRule("ScavBody", "HandL", new Vector3(-2.04041F, 3.10486F, -2.06395F), new Vector3(349.0269F, 326.6412F, 73.87372F), new Vector3(1.733F, 1.733F, 1.733F));
                if (SoftDependencies.SoftDependenciesCore.itemDisplaysSniper) AddDisplayRule("SniperClassicBody", "Muzzle", new Vector3(-2.02132F, 0.0648F, 0.16052F), new Vector3(10.03741F, 180F, 90F), new Vector3(0.30723F, 0.30723F, 0.30723F));
                AddDisplayRule("RailgunnerBody", "TopRail", new Vector3(0.00006F, 0.68001F, 0.05789F), new Vector3(0F, 0F, 0F), new Vector3(0.06583F, 0.06583F, 0.06583F));
                AddDisplayRule("VoidSurvivorBody", "ForeArmL", new Vector3(0.05651F, 0.30521F, 0.01208F), new Vector3(354.2681F, 80.14155F, 0F), new Vector3(0.08341F, 0.08341F, 0.08341F));
            };

            procVFX = Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Ten Commandments of Vyrael/ProcVFX.prefab");
            EffectComponent effectComponent = procVFX.AddComponent<EffectComponent>();
            effectComponent.applyScale = true;
            effectComponent.soundName = "MysticsItems_Play_item_proc_VyraelCommandments";
            VFXAttributes vfxAttributes = procVFX.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Low;
            procVFX.AddComponent<DestroyOnTimer>().duration = 0.5f;
            MysticsItemsContent.Resources.effectPrefabs.Add(procVFX);

            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            GenericGameEvents.OnHitEnemy += GenericGameEvents_OnHitEnemy;
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            body.gameObject.AddComponent<MysticsItemsVyraelCommandmentsHelper>();
        }

        private void GenericGameEvents_OnHitEnemy(DamageInfo damageInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo attackerInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo victimInfo)
        {
            if (!damageInfo.rejected && damageInfo.procCoefficient > 0f && attackerInfo.body && attackerInfo.inventory)
            {
                var itemCount = attackerInfo.inventory.GetItemCount(itemDef);
                if (itemCount > 0)
                {
                    var component = attackerInfo.body.GetComponent<MysticsItemsVyraelCommandmentsHelper>();
                    if (!component) component = attackerInfo.body.gameObject.AddComponent<MysticsItemsVyraelCommandmentsHelper>();
                    if (component.bonusActive <= 0)
                    {
                        component.hitCount += 1;
                        if (component.hitCount >= (float)hits)
                        {
                            component.hitCount -= (float)hits;
                            component.bonusActive++;

                            for (var i = 0; i < itemCount; i++)
                                GlobalEventManager.instance.OnHitEnemy(damageInfo, victimInfo.gameObject);

                            component.bonusActive--;
                            component.playEffect = true;
                        }
                    }
                }
            }
        }

        public class MysticsItemsVyraelCommandmentsHelper : MonoBehaviour
        {
            public float hitCount = 0;
            public int bonusActive = 0;
            public bool playEffect = false;
        }

        public class MysticsItemsVyraelCommandmentsEffect : MonoBehaviour
        {
            public MysticsItemsVyraelCommandmentsHelper helper;
            
            public void Start()
            {
                var body = GetComponentInParent<CharacterModel>().body;
                if (body)
                {
                    helper = body.GetComponent<MysticsItemsVyraelCommandmentsHelper>();
                }
            }

            public void FixedUpdate()
            {
                if (helper && helper.playEffect && procVFX)
                {
                    helper.playEffect = false;

                    if (NetworkServer.active)
                    {
                        var effectData = new EffectData
                        {
                            origin = transform.position,
                            rotation = transform.rotation,
                            scale = transform.lossyScale.x * 2f
                        };
                        EffectManager.SpawnEffect(procVFX, effectData, true);
                    }
                }
            }
        }
    }
}
