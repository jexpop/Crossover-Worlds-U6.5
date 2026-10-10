using System.Collections;
using UnityEngine;

namespace CrossoverWorlds.Objects_Pool
{
    public class EnemiesSpawner : MonoBehaviour
    {
        [SerializeField] private float cooldown = 0.5f;
        [SerializeField] private GameObject[] prefab;  // The object to spawn.
        Bounds bounds;  // Our spawn area's boundaries.
        [SerializeField] private int numGroupEnemies;

        private GameObjectPool enemiesPool;  // Our new pool!


        // Start is called before the first frame update
        void Start()
        {
            bounds = GetComponent<Collider>().bounds;
            // Initializing our new pool with our enemy prefab,
            // default size 25, and max size 100.
            enemiesPool = new GameObjectPool(prefab, 25, 100);
            StartCoroutine(SpawnEnemies());
        }

        // Spawn enemies every x seconds, x=cooldown.
        private IEnumerator SpawnEnemies()
        {
            while (enemiesPool.GetActiveObjectsInThePool() < numGroupEnemies)
            {
                yield return new WaitForSeconds(cooldown);
                enemiesPool.GetObject(RandomPointInBounds());
            }
        }

        // Lets a enemy tell the spawner it needs to be deleted,
        // without giving enemies access to the pool.
        public void RemoveEnemies(GameObject enemy)
        {
            enemiesPool.ReleaseObject(enemy);
        }

        // Get a random point within our collider's bounds.
        private Vector3 RandomPointInBounds()
        {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
                );
        }
    }
}