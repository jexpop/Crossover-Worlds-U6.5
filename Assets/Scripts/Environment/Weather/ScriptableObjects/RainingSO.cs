using UnityEngine;

namespace CrossoverWorlds.Environment.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Raining", menuName = "Environment/Raining")]
    public class RainingSO : ScriptableObject
    {
        [field: Header("Clouds")]
        [field: SerializeField] public float NormalCloudThickness { get; private set; }
        [field: SerializeField] public float RainingCloudThickness { get; private set; }
        [field: SerializeField] public float NormalCloudBrightness { get; private set; }
        [field: SerializeField] public float RainingCloudBrightness { get; private set; }
        
        [field: Header("Day")]
        [field: SerializeField] public float NormalDayBrightness { get; private set; }
        [field: SerializeField] public float RainingDayBrightness { get; private set; }
        [field: SerializeField] public Color NormalDayColor { get; private set; }
        [field: SerializeField] public Color RainingDayColor { get; private set; }
        [field: SerializeField] public bool ShouldChangeDayScatterStrength { get; private set; }
    }
}
