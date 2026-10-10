using UnityEngine;
using UnityEngine.Pool;

namespace CrossoverWorlds.Objects_Pool
{
    // Note that this class is not a MonoBehavior. Just a pure C# class.
    public class GameObjectPool
    {
        GameObject prefab;
        ObjectPool<GameObject> pool;
        int defaultSize;
        int maxSize;

        // Our class's constructor. Takes the prefab to spawn as an argument.
        public GameObjectPool(GameObject[] prefab, int defaultSize = 20, int maxSize = 100)
        {
            this.prefab = prefab[Random.Range(0, prefab.Length-1)];  // The prefab to spawn.
            this.defaultSize = defaultSize;  // Pool's starting number of objects.
            this.maxSize = maxSize;  // Max size for our pool.
                                     // Initializing our pool.
            pool = new ObjectPool<GameObject>(
                CreatePooledObject,
                OnGetFromPool,
                OnReturnToPool,
                OnDestroyPooledObject,
                true,
                defaultSize,
                maxSize
                );
        }

        // Wrapper function for pool.Get. Gets object and sets position.
        public GameObject GetObject(Vector3 position)
        {
            GameObject obj = pool.Get();
            obj.transform.position = position;
            float size = Random.Range(0.25f, 2f);
            obj.transform.localScale = new Vector3(size, size, size);
            return obj;
        }

        // Wrapper function for pool.Release
        public void ReleaseObject(GameObject obj)
        {
            pool.Release(obj);
        }

        // Return the number of objects in the pool
        public int GetActiveObjectsInThePool()
        {
            return pool.CountActive;
        }


        // Return a brand new GameObject instance for our pool to use.
        // We have to specify GameObject.Instantiate because this isn't a Monobehavior.
        GameObject CreatePooledObject()
        {
            GameObject newObject = GameObject.Instantiate(prefab);
            return newObject;
        }

        // When an object is taken from the pool, activate it.
        void OnGetFromPool(GameObject pooledObject)
        {
            pooledObject.SetActive(true);
        }

        // When an object is returned to the pool, deactivate it.
        void OnReturnToPool(GameObject pooledObject)
        {
            pooledObject.SetActive(false);
        }

        // When the pool discards an object, destroy the GameObject.
        void OnDestroyPooledObject(GameObject pooledObject)
        {
            GameObject.Destroy(pooledObject);
        }

    }
}