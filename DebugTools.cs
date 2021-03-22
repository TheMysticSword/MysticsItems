using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.UI;

namespace MysticsItems
{
    public class DebugTools
    {
        public static void Init()
        {
            enabled = true;

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
        }

        public const string ConCommandPrefix = Main.TokenPrefix + "debug_";
        public static bool ConCommandCheck()
        {
            if (!enabled)
            {
                Main.logger.LogWarning("Debug tools are not enabled in the current build");
                return false;
            }
            return true;
        }

        public class MysticsItemsSpectator : MonoBehaviour
        {
            public bool spectating = false;
        }

        public static bool enabled = false;

        [ConCommand(commandName = ConCommandPrefix + "spectator", flags = ConVarFlags.Cheat, helpText = "Become invisible and remove ability to pick items up")]
        private static void CCSpectator(ConCommandArgs args)
        {
            if (!ConCommandCheck()) return;
            MysticsItemsSpectator component = args.senderBody.GetComponent<MysticsItemsSpectator>();
            if (!component) component = args.senderBody.gameObject.AddComponent<MysticsItemsSpectator>();
            component.spectating = !component.spectating;
            args.senderBody.modelLocator.modelTransform.GetComponentInChildren<CharacterModel>().invisibilityCount += 999 * (component.spectating ? 1 : -1);
        }

        public static Dictionary<ItemIndex, int> monsterItems = new Dictionary<ItemIndex, int>();

        [ConCommand(commandName = ConCommandPrefix + "givemonsters", flags = ConVarFlags.Cheat, helpText = "Give monsters an item")]
        private static void CCGiveMonstersItem(ConCommandArgs args)
        {
            if (!ConCommandCheck()) return;
            ItemIndex finalItemIndex = ItemIndex.None;
            int count = args.Count >= 2 ? int.Parse(args[1]) : 1;
            foreach (ItemIndex itemIndex in ItemCatalog.allItems)
            {
                ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
                if (Language.GetString(itemDef.nameToken).StartsWith(args[0]) || itemDef.name.StartsWith(args[0]))
                {
                    finalItemIndex = itemIndex;
                    break;
                }
            }
            if (finalItemIndex != ItemIndex.None)
            {
                Debug.Log("Giving enemies " + count + " " + Language.GetString(ItemCatalog.GetItemDef(finalItemIndex).nameToken));
                if (monsterItems.ContainsKey(finalItemIndex)) monsterItems[finalItemIndex] += count;
                else monsterItems.Add(finalItemIndex, count);
            }
        }

        [ConCommand(commandName = ConCommandPrefix + "resetmonsteritems", flags = ConVarFlags.Cheat, helpText = "Reset monster items")]
        private static void CCClearMonsterItems(ConCommandArgs args)
        {
            if (!ConCommandCheck()) return;
            monsterItems.Clear();
        }

        [ConCommand(commandName = ConCommandPrefix + "notification", flags = ConVarFlags.None, helpText = "Create a notification at the bottom of the screen")]
        private static void CCNotification(ConCommandArgs args)
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
                    currentNotification.iconImage.texture = Resources.Load<Texture>(args[3]);
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
    }
}