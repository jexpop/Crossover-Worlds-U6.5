using UnityEngine;

namespace CrossoverWorlds.Environment
{
    public class DayNightCycle : MonoBehaviour
    {

        [Header("Time patrameters")] 
        [SerializeField] private float initialPhaseSize = 0.09f;
        [SerializeField] private int maxMoonPhases = 16;
        [SerializeField] private int dayMoonPhases = 0;
        [SerializeField] [Range(0.0f,24.0f)]private float hour = 12.0f;
        [SerializeField] private float dayDurationInMinutes = 24.0f;
        
        private Material skyboxMaterial; // Material for the current skybox
        private int hashStarDensity = Shader.PropertyToID("_Star_Density");
        private int hashMoonPhases = Shader.PropertyToID("_Moon_Mask_Size");

        private float sunX;
        private GameObject sun;
        private Light sunLight;
        [SerializeField] private GameObject nightLight;
        
        private int lastDayMoonPhase = 0;
        private int mediumMoonCycle;
        private int inverseDayMoonPhases;
        
        private Raining rainingScript;
        
        
        void Start()
        {
            skyboxMaterial = RenderSettings.skybox;
            
            this.transform.eulerAngles = new Vector3(
                this.transform.eulerAngles.x-90,
                this.transform.eulerAngles.y,
                this.transform.eulerAngles.z
            );
            
            sun = this.transform.GetChild(0).gameObject;
            sunLight = sun.GetComponent<Light>();

            mediumMoonCycle = maxMoonPhases / 2;
            inverseDayMoonPhases = mediumMoonCycle;
            
            rainingScript = Camera.main.GetComponentInChildren<Raining>();
        }

        void Update()
        {
            hour += Time.deltaTime * (24 / (60 * dayDurationInMinutes));
            
            if (hour >= 24.0f)
            {
                hour = 0.0f;
                lastDayMoonPhase = dayMoonPhases;
            }
            SunRotation();
            
            // Phases of the moon, change when the moon is hidden
            if (hour >= 12.0f & hour <= 12.9f)
            {
                if (dayMoonPhases == lastDayMoonPhase)
                {
                    MoonPhases();
                    dayMoonPhases++;
                    if (dayMoonPhases >= maxMoonPhases+1)
                    {
                        dayMoonPhases = 0;
                    }
                }
            }

        }

        private void MoonPhases()
        {
            float newPhase;
            if (dayMoonPhases < mediumMoonCycle)
            {
                inverseDayMoonPhases = mediumMoonCycle;
                newPhase = initialPhaseSize + (dayMoonPhases/100.0f);
            }
            else
            {
                inverseDayMoonPhases--;
                newPhase = initialPhaseSize + (inverseDayMoonPhases/100.0f);
            }

            skyboxMaterial.SetFloat(hashMoonPhases, newPhase);
        }

        private void SunRotation()
        {
            sunX = 15.0f * hour;
            sun.transform.localEulerAngles = new Vector3(sunX, 0.0f, 0.0f);

            // Real night
            if (hour < 7.0f || hour > 16.0f)
            {
                nightLight.SetActive(true);
            }
            else
            {
                nightLight.SetActive(false);
            }
            
            if (hour < 6.0f || hour > 18.0f || rainingScript.isRaining)
            {
                sunLight.intensity = 0.5f;

                if (hour < 6.0f || hour > 18.0f)
                {// Show stars
                    skyboxMaterial.SetFloat(hashStarDensity, 2.0f);
                }
            }
            else
            {
                sunLight.intensity = 1.0f;
                
                // Hide stars
                skyboxMaterial.SetFloat(hashStarDensity, 0.0f);
            }
        }
        
    }
}
