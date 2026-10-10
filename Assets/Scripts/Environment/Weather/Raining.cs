using System.Collections;
using CrossoverWorlds.Environment.ScriptableObjects;
using UnityEngine;

namespace CrossoverWorlds.Environment
{
    public class Raining : MonoBehaviour
    {

        public RainingSO rainingSO; // Scriptable object for the raining parameters
        
        public bool isRaining = false; // Is raining now ?
        
        private GameObject rainVxf; // Raining Visual Effect Graph
        
        private Material skyboxMaterial; // Material for the current skybox
        private int hash_EveningScatterStrength = Shader.PropertyToID("_EveningScatterStrength");
        private int hash_DayColour = Shader.PropertyToID("_DayColour");
        private int hash_DayScatterStrength = Shader.PropertyToID("_DayScatterStrength");
        private int hash_CloudThickness = Shader.PropertyToID("_CloudThickness");
        private int hash_CloudBrightness = Shader.PropertyToID("_CloudBrightness");
        private int hash_DayBrightness = Shader.PropertyToID("_DayBrightness");
        
        // Sky parameters 
        private Color skyColorWhenItIsRaining;
        private float scatterStrengthWhenItIsRaining;
        
        // Custom parameters
        private float cloudThicknessDuration;
        private float cloudBrightnessDuration;
        private float dayBrightnessDuration;


        void Start()
        {
            // Get necessary components
            rainVxf = transform.GetChild(0).gameObject;
            skyboxMaterial = RenderSettings.skybox;
            
            // Initialize recomended parameters
            InitialValues();
            
            // Cutom parameteres initialization
            cloudThicknessDuration = (rainingSO.NormalCloudThickness - rainingSO.RainingCloudThickness) * 1000.0f;
            cloudBrightnessDuration = (rainingSO.NormalCloudBrightness - rainingSO.RainingCloudBrightness) / 5.0f;
            dayBrightnessDuration = (rainingSO.NormalDayBrightness - rainingSO.RainingDayBrightness) * 10.0f;
            
            // Get evening parameters to copy to raining parameters 
            scatterStrengthWhenItIsRaining = skyboxMaterial.GetFloat(hash_EveningScatterStrength);
        }

        private void InitialValues()
        {
            rainVxf.SetActive(false);
            skyboxMaterial.SetColor(hash_DayColour, rainingSO.NormalDayColor);
            skyboxMaterial.SetFloat(hash_DayScatterStrength, 1.0f);

            skyboxMaterial.SetFloat(hash_DayBrightness, rainingSO.NormalDayBrightness);
            skyboxMaterial.SetFloat(hash_CloudBrightness, rainingSO.NormalCloudBrightness);
            skyboxMaterial.SetFloat(hash_CloudThickness, rainingSO.NormalCloudThickness);
        }
        
        private void OnValidate()
        {
            if (rainVxf != null)
            {
                if (isRaining)
                {
                    rainVxf.SetActive(true);
                    skyboxMaterial.SetColor(hash_DayColour, rainingSO.RainingDayColor);
                    
                    if (rainingSO.ShouldChangeDayScatterStrength)
                    {
                        skyboxMaterial.SetFloat(hash_DayScatterStrength, scatterStrengthWhenItIsRaining);
                    }
                    
                    StartCoroutine(DayBrightnessEffect(rainingSO.NormalDayBrightness, true));
                    StartCoroutine(CloudBrightnessEffect(rainingSO.NormalCloudBrightness, true));
                    StartCoroutine(CloudThicknessEffect(rainingSO.NormalCloudThickness, true));
                }
                else
                {
                    rainVxf.SetActive(false);
                    skyboxMaterial.SetColor(hash_DayColour, rainingSO.NormalDayColor);
                    skyboxMaterial.SetFloat(hash_DayScatterStrength, 1.0f);

                    StartCoroutine(DayBrightnessEffect(rainingSO.RainingDayBrightness, false));
                    StartCoroutine(CloudBrightnessEffect(rainingSO.RainingCloudBrightness, false));
                    StartCoroutine(CloudThicknessEffect(rainingSO.RainingCloudThickness, false));
                }
            }
        }

        //***** Raining special effects *****//
        IEnumerator CloudThicknessEffect(float currentCloudThickness, bool isRaining)
        {
            for (int i = 0; i < cloudThicknessDuration; i++)
            {
                currentCloudThickness=isRaining?currentCloudThickness-0.001f:currentCloudThickness+0.001f;
                skyboxMaterial.SetFloat(hash_CloudThickness, currentCloudThickness);
                yield return new WaitForSeconds(0.001f);
            }
        }
        IEnumerator CloudBrightnessEffect(float currentCloudBrightness, bool isRaining)
        {
            for (int i = 0; i < cloudBrightnessDuration; i++)
            {
                currentCloudBrightness=isRaining?currentCloudBrightness-5.0f:currentCloudBrightness+1.0f;
                skyboxMaterial.SetFloat(hash_CloudBrightness, currentCloudBrightness);
                yield return new WaitForSeconds(0.5f);
            }

            // Restore original value
            if (!isRaining)
            {
                skyboxMaterial.SetFloat(hash_CloudBrightness, rainingSO.NormalCloudBrightness);
            }
        }
        IEnumerator DayBrightnessEffect(float currentDayBrightness, bool isRaining)
        {
            for (int i = 0; i < dayBrightnessDuration; i++)
            {
                currentDayBrightness=isRaining?currentDayBrightness-0.1f:currentDayBrightness+0.1f;
                skyboxMaterial.SetFloat(hash_DayBrightness, currentDayBrightness);
                yield return new WaitForSeconds(0.1f);
            }
        }
        //***********************************//
        
    }
}
