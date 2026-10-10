using GPUInstancerPro.PrefabModule;
using UnityEngine;

namespace CrossoverWorlds
{
    public class FootableInteraction : MonoBehaviour
    {
        GPUIMaterialVariationInstance gPUIMaterialVariationInstance; // Instance to handle the material
        public float slopeAmount = 0.1f; // Amount of slope
        private Vector3 footPosition; // Position of the foot or the object you are stepping on

        void Start()
        {
            // Get instance of GPUIMaterialVariationInstance
            gPUIMaterialVariationInstance=GetComponent<GPUIMaterialVariationInstance>();

        }

        public void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Foot"){

                // Calculate the slope  
                gPUIMaterialVariationInstance.values[0].x = 1;

                // Foot (or object) position to shader
                footPosition = other.transform.position;
                gPUIMaterialVariationInstance.values[1] = footPosition;

                // Apply material variation
                gPUIMaterialVariationInstance.ApplyVariation();
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if(other.tag == "Foot")
            {
                // Restore the slope to the initial value
                gPUIMaterialVariationInstance.values[0].x = 0.0f;

                // Initialize position
                gPUIMaterialVariationInstance.values[0] = new Vector3(0,0,0);

                // Apply material variation
                gPUIMaterialVariationInstance.ApplyVariation();
            }
        }

    }
}
