using UnityEngine;

namespace CrossoverWorlds.Objects_Pool
{
    public class EnemiesDespawn : MonoBehaviour
    {
        public EnemiesSpawner spawner;  // The enemies spawner.
                                        // Start is called before the first frame update
        void Start()
        {
            // Find our enemies spawner.
            // This will get weird if you try to make more than one spawner.
            spawner = FindAnyObjectByType<EnemiesSpawner>();
        }

        private void Update()
        {
            if (transform.position.y < -4)
            {
                if (spawner != null)
                {
                    // Call a function on our spawner.
                    spawner.RemoveEnemies(gameObject);
                }
            }
        }
    }
}