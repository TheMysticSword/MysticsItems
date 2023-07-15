using UnityEngine;
using MysticsRisky2Utils;
using RoR2;
using RoR2.Skills;
using UnityEngine.Networking;
using MysticsRisky2Utils.BaseAssetTypes;
using RoR2.Navigation;

namespace MysticsItems.CharacterBodies
{
    public class UnstableRiftShooter : BaseCharacterBody
    {
        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            prefab = Utils.CreateBlankPrefab("MysticsItems_UnstableRiftShooterBody", true);
			prefab.GetComponent<NetworkIdentity>().localPlayerAuthority = true;
        }

        public override void OnLoad()
        {
            base.OnLoad();

            Utils.CopyChildren(Main.AssetBundle.LoadAsset<GameObject>("Assets/Items/Rift Lens/UnstableRiftShooter.prefab"), prefab);
            bodyName = "MysticsItems_UnstableRiftShooter";

            modelBaseTransform = prefab.transform.Find("ModelBase");
            modelTransform = prefab.transform.Find("ModelBase/EmptyMesh");
            meshObject = modelTransform.gameObject;
            Prepare();

            SetUpChildLocator(new ChildLocator.NameTransformPair[0]);

            modelLocator.dontReleaseModelOnDeath = false;
            modelLocator.autoUpdateModelTransform = false;
            modelLocator.dontDetatchFromParent = true;
            modelLocator.noCorpse = true;

            // body
            CharacterBody characterBody = prefab.GetComponent<CharacterBody>();
            characterBody.bodyFlags = CharacterBody.BodyFlags.ImmuneToGoo | CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.ImmuneToVoidDeath | CharacterBody.BodyFlags.HasBackstabImmunity;
            characterBody.baseMaxHealth = 100000f;
            characterBody.baseDamage = 12f;
            characterBody.baseMoveSpeed = 0f;
            characterBody.baseAcceleration = 0f;
            characterBody.baseJumpPower = 0f;
            characterBody.baseJumpCount = 0;
            characterBody.portraitIcon = Main.AssetBundle.LoadAsset<Texture>("Assets/Items/Rift Lens/EnemyIcon.png");
			characterBody.bodyColor = new Color32(65, 218, 244, 255);
            characterBody.aimOriginTransform = modelBaseTransform;
            AfterCharacterBodySetup();
            characterBody.baseNameToken = "MYSTICSITEMS_RIFTCHEST_NAME";
            characterBody.subtitleNameToken = "";
            prefab.AddComponent<MysticsItemsUnstableRiftShooterComponent>();

            // state machines
            EntityStateMachine bodyStateMachine = SetUpEntityStateMachine("Body", typeof(EntityStates.Uninitialized), typeof(EntityStates.Uninitialized));
            
            CharacterDeathBehavior characterDeathBehavior = prefab.GetComponent<CharacterDeathBehavior>();
            characterDeathBehavior.deathStateMachine = bodyStateMachine;
            characterDeathBehavior.deathState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Turret1.DeathState));

            // model
            CharacterModel characterModel = modelTransform.GetComponent<CharacterModel>();
            characterModel.baseRendererInfos = new CharacterModel.RendererInfo[0];
			AfterCharacterModelSetUp();
        }

        public class MysticsItemsUnstableRiftShooterComponent : MonoBehaviour, IOnIncomingDamageServerReceiver
        {
            public void OnIncomingDamageServer(DamageInfo damageInfo)
            {
                damageInfo.rejected = true;
            }
        }
    }
}
