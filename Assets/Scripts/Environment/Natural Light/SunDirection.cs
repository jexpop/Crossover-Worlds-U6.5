using UnityEngine;

namespace CrossoverWorlds.Environment
{
    public class SunDirection : MonoBehaviour
    {
        
        private int hash_SunDirection = Shader.PropertyToID("_SunDirection");
        
        
        void Update()
        {
            Shader.SetGlobalVector(hash_SunDirection, transform.forward);
        }
    }
}
