using UnityEngine;

namespace MysticsItems
{
    public class MysticsItemsScaleLight : MonoBehaviour
    {
        public void FixedUpdate()
        {
            foreach (Light light in GetComponents<Light>())
            {
                float lossyScale = (light.transform.lossyScale.x + light.transform.lossyScale.y + light.transform.lossyScale.z) / 3f;
                float localScale = (light.transform.localScale.x + light.transform.localScale.y + light.transform.localScale.z) / 3f;

                light.range = lossyScale / localScale;
            }
        }
    }
}