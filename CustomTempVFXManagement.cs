using RoR2;
using R2API.Utils;
using UnityEngine;
using System.Collections.Generic;

namespace MysticsItems
{
    public static class CustomTempVFXManagement
    {
        public struct VFXInfo
        {
            public GameObject prefab;
            public string child;
            public System.Func<CharacterBody, bool> condition;
            public System.Func<CharacterBody, float> radius;
        }

        public static float DefaultRadiusCall(CharacterBody body) { return body.radius; }
        public static float DefaultBestFitRadiusCall(CharacterBody body) { return body.bestFitRadius; }

        public static List<VFXInfo> allVFX = new List<VFXInfo>();

        public static void Init()
        {
            On.RoR2.CharacterBody.Awake += (orig, self) =>
            {
                orig(self);
                self.gameObject.AddComponent<MysticsItemsCharacterCustomTempVFXHolder>();
            };

            On.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += (orig, self) =>
            {
                orig(self);
                MysticsItemsCharacterCustomTempVFXHolder component = self.GetComponent<MysticsItemsCharacterCustomTempVFXHolder>();
                foreach (VFXInfo vfxInfo in allVFX)
                {
                    bool active = vfxInfo.condition(self);
                    MysticsItemsCustomTempVFX tempVFX = component.dictionary[vfxInfo.prefab];
                    if (active != (tempVFX != null))
                    {
                        if (active)
                        {
                            if (!tempVFX)
                            {
                                GameObject gameObject = Object.Instantiate<GameObject>(vfxInfo.prefab, self.corePosition, Quaternion.identity);
                                
                                tempVFX = gameObject.GetComponent<MysticsItemsCustomTempVFX>();
                                component.dictionary[vfxInfo.prefab] = tempVFX;
                                tempVFX.parentTransform = self.coreTransform;
                                tempVFX.visualState = MysticsItemsCustomTempVFX.VisualState.Enter;
                                tempVFX.healthComponent = self.healthComponent;
                                tempVFX.radius = vfxInfo.radius(self);

                                LocalCameraEffect localCameraEffect = gameObject.GetComponent<LocalCameraEffect>();
                                if (localCameraEffect) localCameraEffect.targetCharacter = self.gameObject;

                                if (!string.IsNullOrEmpty(vfxInfo.child))
                                {
                                    ModelLocator modelLocator = self.modelLocator;
                                    if (modelLocator)
                                    {
                                        Transform modelTransform = modelLocator.modelTransform;
                                        if (modelTransform)
                                        {
                                            ChildLocator childLocator = modelTransform.GetComponent<ChildLocator>();
                                            if (childLocator)
                                            {
                                                Transform transform = childLocator.FindChild(vfxInfo.child);
                                                if (transform)
                                                {
                                                    tempVFX.parentTransform = transform;
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (tempVFX) tempVFX.visualState = MysticsItemsCustomTempVFX.VisualState.Exit;
                        }
                    }
                }
            };
        }

        public class MysticsItemsCustomTempVFX : MonoBehaviour
        {
            public Transform parentTransform;
            public bool rotateWithParent = false;
            public HealthComponent healthComponent;
            public GameObject[] enterObjects = new GameObject[] { };
            public MonoBehaviour[] enterBehaviours = new MonoBehaviour[] { };
            public GameObject[] exitObjects = new GameObject[] { };
            public MonoBehaviour[] exitBehaviours = new MonoBehaviour[] { };
            public VisualState visualState;
            public VisualState prevVisualState;
            public float radius;

            public void Start()
            {
                RebuildVisuals();
            }

            public void LateUpdate()
            {
                if (!healthComponent || !healthComponent.alive) visualState = VisualState.Exit;
                if (parentTransform)
                {
                    transform.position = parentTransform.position;
                    if (rotateWithParent) transform.rotation = parentTransform.rotation;
                    transform.localScale = Vector3.one * radius;
                    if (visualState != prevVisualState)
                    {
                        prevVisualState = visualState;
                        RebuildVisuals();
                    }
                }
                else
                {
                    Object.Destroy(gameObject);
                }
            }

            public void RebuildVisuals()
            {
                bool enterState = visualState == VisualState.Enter;
                foreach (GameObject obj in enterObjects) obj.SetActive(enterState);
                foreach (MonoBehaviour behaviours in enterBehaviours) behaviours.enabled = enterState;
                foreach (GameObject obj in exitObjects) obj.SetActive(!enterState);
                foreach (MonoBehaviour behaviours in exitBehaviours) behaviours.enabled = !enterState;
            }

            public enum VisualState
            {
                Enter,
                Exit
            }
        }

        public class MysticsItemsCharacterCustomTempVFXHolder : MonoBehaviour
        {
            public Dictionary<GameObject, MysticsItemsCustomTempVFX> dictionary = new Dictionary<GameObject, MysticsItemsCustomTempVFX>();

            public void Awake()
            {
                foreach (VFXInfo vfxInfo in allVFX) dictionary.Add(vfxInfo.prefab, default);
            }
        }
    }
}
