using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RoR2.UI;

namespace MysticsItems
{
    public class DebugTools
    {
        internal static System.Type declaringType;

        public static void Init()
        {
            enabled = true;
            Main.logger.LogWarning("Debug tools enabled! Please contact the mod author if you see this in a public build.");

            declaringType = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;

            ConCommandHelper.Load(declaringType.GetMethod("CCSpectator", Main.bindingFlagAll));
            ConCommandHelper.Load(declaringType.GetMethod("CCGiveMonstersItem", Main.bindingFlagAll));
            ConCommandHelper.Load(declaringType.GetMethod("CCClearMonsterItems", Main.bindingFlagAll));
            ConCommandHelper.Load(declaringType.GetMethod("CCNotification", Main.bindingFlagAll));
            ConCommandHelper.Load(declaringType.GetMethod("CCNoEnemies", Main.bindingFlagAll));
            ConCommandHelper.Load(declaringType.GetMethod("CCStage", Main.bindingFlagAll));
            ConCommandHelper.Load(declaringType.GetMethod("CCGiveItem", Main.bindingFlagAll));
            ConCommandHelper.Load(declaringType.GetMethod("CCGiveEquip", Main.bindingFlagAll));
            ConCommandHelper.Load(declaringType.GetMethod("CCSpawnInteractable", Main.bindingFlagAll));
            ConCommandHelper.Load(declaringType.GetMethod("CCGold", Main.bindingFlagAll));

            On.RoR2.GenericPickupController.AttemptGrant += (orig, self, body) =>
            {
                MysticsItemsSpectator component = body.GetComponent<MysticsItemsSpectator>();
                if (component && component.spectating) return;
                orig(self, body);
            };
            On.RoR2.GenericPickupController.GetInteractability += (orig, self, interactor) =>
            {
                CharacterBody body = interactor.GetComponent<CharacterBody>();
                if (body)
                {
                    MysticsItemsSpectator component = body.GetComponent<MysticsItemsSpectator>();
                    if (component && component.spectating) return Interactability.Disabled;
                }
                return orig(self, interactor);
            };

            CharacterMaster.onStartGlobal += (master) =>
            {
                if (master.teamIndex == TeamIndex.Monster)
                    foreach (KeyValuePair<ItemIndex, int> keyValuePair in monsterItems)
                    {
                        master.inventory.GiveItem(keyValuePair.Key, keyValuePair.Value);
                    }
            };

            SceneDirector.onPrePopulateSceneServer += (director) =>
            {
                if (noEnemies) director.SetFieldValue("monsterCredit", 0);
            };
        }

        public static ItemDef GetItem(string name)
        {
            name = name.ToLowerInvariant();
            ItemDef finalItemDef = null;
            foreach (ItemIndex itemIndex in ItemCatalog.allItems)
            {
                ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
                if (Language.GetString(itemDef.nameToken).ToLowerInvariant().StartsWith(name) || itemDef.name.ToLowerInvariant().StartsWith(name))
                {
                    finalItemDef = itemDef;
                    break;
                }
            }
            return finalItemDef;
        }

        public static EquipmentDef GetEquipment(string name)
        {
            name = name.ToLowerInvariant();
            EquipmentDef finalEquipmentDef = null;
            foreach (EquipmentIndex equipmentIndex in EquipmentCatalog.allEquipment)
            {
                EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
                if (Language.GetString(equipmentDef.nameToken).ToLowerInvariant().StartsWith(name) || equipmentDef.name.ToLowerInvariant().StartsWith(name))
                {
                    finalEquipmentDef = equipmentDef;
                    break;
                }
            }
            return finalEquipmentDef;
        }

        public const string ConCommandPrefix = "msd_";
        public static bool OnlineCheck()
        {
            return PlayerCharacterMasterController.instances.Count > 1;
        }

        public class MysticsItemsSpectator : MonoBehaviour
        {
            public bool spectating = false;
        }

        public static bool enabled = false;

        [ConCommand(commandName = ConCommandPrefix + "spectator", flags = ConVarFlags.ExecuteOnServer, helpText = "Become invisible and remove ability to pick items up")]
        public static void CCSpectator(ConCommandArgs args)
        {
            if (OnlineCheck()) return;
            MysticsItemsSpectator component = args.senderBody.GetComponent<MysticsItemsSpectator>();
            if (!component) component = args.senderBody.gameObject.AddComponent<MysticsItemsSpectator>();
            component.spectating = !component.spectating;
            args.senderBody.modelLocator.modelTransform.GetComponentInChildren<CharacterModel>().invisibilityCount += 999 * (component.spectating ? 1 : -1);
        }

        public static Dictionary<ItemIndex, int> monsterItems = new Dictionary<ItemIndex, int>();

        [ConCommand(commandName = ConCommandPrefix + "givemonsters", flags = ConVarFlags.ExecuteOnServer, helpText = "Give monsters an item")]
        public static void CCGiveMonstersItem(ConCommandArgs args)
        {
            int count = args.Count >= 2 ? int.Parse(args[1]) : 1;
            ItemDef itemDef = GetItem(args[0]);
            if (itemDef)
            {
                Debug.Log("Giving enemies " + count + " " + Language.GetString(itemDef.nameToken));
                if (monsterItems.ContainsKey(itemDef.itemIndex)) monsterItems[itemDef.itemIndex] += count;
                else monsterItems.Add(itemDef.itemIndex, count);
            }
        }

        [ConCommand(commandName = ConCommandPrefix + "resetmonsteritems", flags = ConVarFlags.ExecuteOnServer, helpText = "Reset monster items")]
        public static void CCClearMonsterItems(ConCommandArgs args)
        {
            monsterItems.Clear();
        }

        [ConCommand(commandName = ConCommandPrefix + "notification", flags = ConVarFlags.None, helpText = "Create a notification at the bottom of the screen")]
        public static void CCNotification(ConCommandArgs args)
        {
            foreach (NotificationQueue notificationQueue in NotificationQueue.readOnlyInstancesList)
            {
                if (notificationQueue.hud.targetMaster == args.senderMaster)
                {
                    GenericNotification currentNotification = Object.Instantiate(Resources.Load<GameObject>("Prefabs/NotificationPanel2")).GetComponent<GenericNotification>();
                    if (bool.Parse(args[0])) // custom text
                    {
                        currentNotification.titleText.token = "MYSTICSITEMS_EMPTY_FORMAT";
                        currentNotification.titleText.SetPropertyValue("formatArgs", new object[] { args[1] });
                        currentNotification.descriptionText.token = "MYSTICSITEMS_EMPTY_FORMAT";
                        currentNotification.descriptionText.SetPropertyValue("formatArgs", new object[] { args[2] });
                    }
                    else // tokens
                    {
                        currentNotification.titleText.token = args[1];
                        currentNotification.descriptionText.token = args[2];
                    }
                    if (args[3].StartsWith("@")) // custom icon
                    {
                        currentNotification.iconImage.texture = Main.AssetBundle.LoadAsset<Texture>(args[3].Remove(0, 1));
                    }
                    else // ingame icon
                    {
                        currentNotification.iconImage.texture = Resources.Load<Texture>(args[3]);
                    }
                    switch (args[4])
                    {
                        case "lockedachievement":
                            currentNotification.iconImage.color = Color.black;
                            currentNotification.titleTMP.color = Color.white;
                            break;
                        default:
                            currentNotification.titleTMP.color = ColorCatalog.GetColor((ColorCatalog.ColorIndex)int.Parse(args[4]));
                            break;
                    }
                    notificationQueue.SetFieldValue("currentNotification", currentNotification);
                    currentNotification.GetComponent<RectTransform>().SetParent(notificationQueue.GetComponent<RectTransform>(), false);
                }
            }
        }

        public static bool noEnemies = false;
        [ConCommand(commandName = ConCommandPrefix + "no_enemies", flags = ConVarFlags.ExecuteOnServer, helpText = "Kill all enemies and prevent more enemies from spawning")]
        public static void CCNoEnemies(ConCommandArgs args)
        {
            noEnemies = !noEnemies;
            typeof(CombatDirector).GetFieldValue<RoR2.ConVar.BoolConVar>("cvDirectorCombatDisable").SetBool(noEnemies);
            foreach (CharacterBody body in CharacterBody.readOnlyInstancesList)
            {
                if (body.teamComponent && body.teamComponent.teamIndex == TeamIndex.Monster && body.healthComponent)
                {
                    body.healthComponent.Suicide();
                }
            }
            Debug.Log("Enemies " + (noEnemies ? "disabled" : "enabled"));
        }

        [ConCommand(commandName = ConCommandPrefix + "stage", flags = ConVarFlags.ExecuteOnServer, helpText = "Change current stage")]
        public static void CCStage(ConCommandArgs args)
        {
            SceneDef sceneDef = SceneCatalog.GetSceneDefFromSceneName(args[0]);
            if (sceneDef)
            {
                Run.instance.AdvanceStage(sceneDef);
            }
            else
            {
                Debug.LogWarning("Stage " + args[0] + " doesn't exist");
            }
        }

        [ConCommand(commandName = ConCommandPrefix + "give_item", flags = ConVarFlags.ExecuteOnServer, helpText = "Give yourself an item")]
        public static void CCGiveItem(ConCommandArgs args)
        {
            int count = args.Count >= 2 ? int.Parse(args[1]) : 1;
            ItemDef itemDef = GetItem(args[0]);
            if (itemDef)
            {
                Debug.Log("Giving " + count + " " + Language.GetString(itemDef.nameToken));
                if (args.senderMaster)
                {
                    args.senderMaster.inventory.GiveItem(itemDef, count);
                }
            }
        }

        [ConCommand(commandName = ConCommandPrefix + "give_equip", flags = ConVarFlags.ExecuteOnServer, helpText = "Give yourself equipment")]
        public static void CCGiveEquip(ConCommandArgs args)
        {
            EquipmentDef equipmentDef = GetEquipment(args[0]);
            if (equipmentDef)
            {
                Debug.Log("Giving " + Language.GetString(equipmentDef.nameToken));
                if (args.senderMaster)
                {
                    args.senderMaster.inventory.SetEquipmentIndex(equipmentDef.equipmentIndex);
                }
            }
        }

        [ConCommand(commandName = ConCommandPrefix + "gold", flags = ConVarFlags.ExecuteOnServer, helpText = "Set your gold")]
        public static void CCGold(ConCommandArgs args)
        {
            if (args.senderMaster)
            {
                args.senderMaster.money = uint.Parse(args[0]);
            }
        }

        [ConCommand(commandName = ConCommandPrefix + "spawn_interactable", flags = ConVarFlags.ExecuteOnServer, helpText = "Spawn an interactable")]
        public static void CCSpawnInteractable(ConCommandArgs args)
        {
            InteractableSpawnCard interactableSpawnCard = Resources.LoadAll<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard").ToList().FirstOrDefault(x => x.name == args[0] || x.name.Remove(0, 3) == args[0]);
            if (interactableSpawnCard)
            {
                Debug.Log("Spawning " + interactableSpawnCard.name);
                if (args.senderBody)
                {
                    RaycastHit hitInfo;
                    if (args.senderBody.inputBank.GetAimRaycast(1000f, out hitInfo)) {
                        interactableSpawnCard.DoSpawn(hitInfo.point, Quaternion.identity, new DirectorSpawnRequest(
                            interactableSpawnCard,
                            new DirectorPlacementRule
                            {
                                placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                                maxDistance = 100f,
                                minDistance = 0f,
                                position = hitInfo.point,
                                preventOverhead = true
                            },
                            RoR2Application.rng
                        ));
                    }
                }
            }
        }
    }
}