using UnityEngine;

namespace CrossoverWorlds
{
    public class PetalsFalling : MonoBehaviour
    {

        [SerializeField] private ParticleSystem prefab;
        [SerializeField] private AudioSource hit;


        private void OnTriggerEnter(Collider other) 
        {
            if(other.transform.tag=="Hit")
            {
                hit.Play();
                prefab.Play();
            }    
        }

    }
}
