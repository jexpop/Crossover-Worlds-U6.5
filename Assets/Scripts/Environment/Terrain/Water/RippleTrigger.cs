using UnityEngine;

namespace CrossoverWorlds
{
    public class RippleTrigger : MonoBehaviour
    {
        [SerializeField] ParticleSystem ripple;

        private void OnTriggerEnter(Collider other)
        {

            ripple.Play();

        }

        private void OnTriggerExit(Collider other)
        {

            ripple.Stop();
        }
    }
}
