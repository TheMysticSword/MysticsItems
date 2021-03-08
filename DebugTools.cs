using RoR2;
using R2API;
using UnityEngine;

namespace MysticsItems
{
    public class DebugTools
    {
        [ConCommand(commandName = Main.TokenPrefix + "spectator", flags = ConVarFlags.Cheat, helpText = "Become invisible and remove ability to pick items up")]
        private static void CCSpectator(ConCommandArgs args)
        {
            if (!enabled)
            {
                Main.logger.LogWarning("Debug tools are not enabled in the current build");
                return;
            }
            MysticsItemsSpectator component = args.senderBody.GetComponent<MysticsItemsSpectator>();
            if (!component) component = args.senderBody.gameObject.AddComponent<MysticsItemsSpectator>();
            component.spectating = !component.spectating;
            args.senderBody.modelLocator.modelTransform.GetComponentInChildren<CharacterModel>().invisibilityCount += 999 * (component.spectating ? 1 : -1);
        }

        public class MysticsItemsSpectator : MonoBehaviour
        {
            public bool spectating = false;
        }

        public static bool enabled = false;

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
        }
    }
}