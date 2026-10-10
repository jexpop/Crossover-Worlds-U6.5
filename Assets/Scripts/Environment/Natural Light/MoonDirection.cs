using UnityEngine;

namespace CrossoverWorlds.Environment
{
    public class MoonDirection : MonoBehaviour
    {
        
        private int hash_MoonDirection = Shader.PropertyToID("_MoonDirection");
        
        
        void Update()
        {
            Shader.SetGlobalVector(hash_MoonDirection, transform.forward);
        }
    }
}
