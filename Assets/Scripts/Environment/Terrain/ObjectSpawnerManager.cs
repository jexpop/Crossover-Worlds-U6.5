using UnityEngine;
using System.Collections.Generic; 
using GPUInstancerPro;
using GPUInstancerPro.PrefabModule;

namespace CrossoverWorlds
{
    [System.Serializable]
    public class SpawnableObject
    {
        [Tooltip("Prefab of the object to spawn (tree, rock, etc.)")]
        public GameObject prefab;
        [Tooltip("Terrain textures to match")]
        public TerrainLayer[] terrainLayers;
        [Tooltip("Probability of occurrence (0 to 1)")] 
        public float spawnChance;

        [Header("Grass system")]
        public bool isGrass = false;
        [Tooltip("Define separation of the grasses")]
        public float grassPadding = 1f;

        [Header("Rotation and scaling options")]
        [Tooltip("Apply random rotation?")]
        public bool randomRotation = false;
        [Tooltip("Apply random scale?")]
        public bool randomScale = false;
        [Tooltip("Range of scales (minimum, maximum)")]
        public Vector2 scaleRange = new Vector2(1f, 1f);

        [Header("Slope options")]
        [Tooltip("Allow slope?")]
        public bool allowSlope = false;
        [Tooltip("Minimum allowable slope angle")]
        public float minSlopeAngle = 0f;
        [Tooltip("Maximum allowable slope angle")]
        public float maxSlopeAngle = 30f;

        [Header("Proximity options")]
        [Tooltip("Minimum height to instantiate the object")]
        public float minHeight = 0f;
        [Tooltip("Minimum distance from other objects")]
        public float minDistanceFromObjects = 5f;
        [Tooltip("Tags of objects to avoid")]
        public string[] proximityTags;
    }

    public class ObjectSpawnerManager : MonoBehaviour
    {
        [Tooltip("List of objects that may appear")]
        public SpawnableObject[] objectsToSpawn;
        [Tooltip("Spawn attempts by terrain")]
        public int spawnAttemptsPerTerrain = 50;

        public GPUIPrefabManager gpuIPrefabManager;

        // Reference to GPUInstancer Manager
        private GPUIManager gpuInstancerManager;

        void Start()
        {
            // Get the GPUInstancer Manager from the scenario
            gpuInstancerManager = FindFirstObjectByType<GPUIManager>();

            if (gpuInstancerManager == null)
            {
                Debug.LogError("GPUInstancerManager not found in scene.");
                return;
            }

            // It checks prototypes created manually
            int prototypeIndex = gpuIPrefabManager.GetPrototypeCount();

            // Registering prefabs in GPUInstancer
            RegisterPrefabs();

            // Search for all lands that are children of the current GameObject
            Terrain[] terrains = GetComponentsInChildren<Terrain>();

            // Generate objects for each terrain
            foreach (Terrain terrain in terrains)
            {
                SpawnObjectsOnTerrain(terrain, prototypeIndex);
            }
        }

        // Registering prefabs in the GPUIManager
        private void RegisterPrefabs()
        {
            // Set to track which prefabs have already been registered
            HashSet<GameObject> registeredPrefabs = new HashSet<GameObject>();

            foreach (var obj in objectsToSpawn)
            {
                if (obj.prefab != null && !registeredPrefabs.Contains(obj.prefab))
                {
                    // If the prefab has not been registered, add it to the manager
                    gpuInstancerManager.AddPrototype(obj.prefab);
                    // Add the prefab to the set so it won't be added again
                    registeredPrefabs.Add(obj.prefab);
                }
            }
        }

        void SpawnObjectsOnTerrain(Terrain terrain, int prototypeIndex)
        {
            TerrainData terrainData = terrain.terrainData;
            Vector3 terrainPosition = terrain.transform.position;

            // Dimensions of the terrain
            int terrainWidth = terrainData.alphamapWidth;
            int terrainHeight = terrainData.alphamapHeight;

            // Obtain terrain texture maps
            float[,,] splatMaps = terrainData.GetAlphamaps(0, 0, terrainWidth, terrainHeight);

            // Next index free (to avoid errors with manually placed objects)
            int index = prototypeIndex;

            foreach (var obj in objectsToSpawn)
            {
                if (obj.terrainLayers == null || obj.terrainLayers.Length == 0)
                {
                    Debug.LogError($"No terrain layers assigned for prefab '{obj.prefab.name}'");
                    continue;
                }

                List<int> textureIndices = GetTextureIndicesByLayers(terrainData, obj.terrainLayers);
                if (textureIndices.Count == 0)
                {
                    Debug.LogError($"None of the specified terrain layers found for prefab '{obj.prefab.name}'");
                    continue;
                }

                if(obj.isGrass)
                {
                    SpawnGrass(terrainData, terrainWidth, terrainHeight, textureIndices, splatMaps, terrain, obj, terrainPosition, index);
                }
                else
                {
                    Spawn(terrainData, terrainWidth, terrainHeight, textureIndices, splatMaps, terrain, obj, terrainPosition, index);
                }
                

                index++;
            }
        }

        private void Spawn(TerrainData terrainData, int terrainWidth, int terrainHeight, List<int> textureIndices, float[,,] splatMaps, Terrain terrain, SpawnableObject obj, Vector3 terrainPosition, int index)
        {
            for (int i = 0; i < spawnAttemptsPerTerrain; i++)
            {
                // Generate a random position on the terrain
                float x = Random.Range(0, terrainData.size.x);
                float z = Random.Range(0, terrainData.size.z);

                // Normalize for splat map
                float normalizedX = x / terrainData.size.x; 
                float normalizedZ = z / terrainData.size.z;

                // Convert normalized position to texture map coordinates
                int splatX = Mathf.FloorToInt(normalizedX * terrainWidth);
                int splatZ = Mathf.FloorToInt(normalizedZ * terrainHeight);

                // Verify if any of the textures match
                bool textureMatch = false;
                foreach (int textureIndex in textureIndices)
                {
                    if (splatMaps[splatZ, splatX, textureIndex] > 0.5f) // Texture threshold
                    {
                        textureMatch = true;
                        break;
                    }
                }

                if (textureMatch && Random.value <= obj.spawnChance) // Probability threshold
                {
                    // Define the initial position of the raycast (well above the terrain).
                    Vector3 rayOrigin = new Vector3(x + terrainPosition.x, 500f, z + terrainPosition.z);

                    // Throw a beam down to find the surface of the terrain
                    Ray ray = new Ray(rayOrigin, Vector3.down);
                    if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
                    {
                        // Ensure that ray strikes the terrain
                        if (hit.collider.GetComponent<Terrain>() != null)
                        {
                            // Verify if the height is above minimum height
                            if (hit.point.y >= obj.minHeight)
                            {

                                // Use the exact position of the impact as the height of the object.
                                Vector3 position = hit.point;

                                // Slope filter (if enabled, allowSlope = false)
                                if(obj.allowSlope)
                                {
                                    Vector3 terrainNormal = terrainData.GetInterpolatedNormal(normalizedX, normalizedZ);
                                    float slopeAngle = Vector3.Angle(Vector3.up, terrainNormal);
                                    if (slopeAngle < obj.minSlopeAngle || slopeAngle > obj.maxSlopeAngle)
                                        continue;
                                }

                                // Proximity filter
                                if (!IsPositionValid(position, obj.minDistanceFromObjects, obj.proximityTags))
                                continue;

                                // Random rotation (if enabled)
                                Quaternion rotation = obj.randomRotation 
                                    ? Quaternion.Euler(0, Random.Range(0, 360), 0) 
                                    : Quaternion.identity;


                                // Create object
                                GameObject newObject = Instantiate(obj.prefab, position, rotation, terrain.transform);

                                // Random scale (if enabled)
                                if (obj.randomScale)
                                {
                                    float randomScale = Random.Range(obj.scaleRange.x, obj.scaleRange.y);
                                    newObject.transform.localScale = Vector3.one * randomScale;
                                }

                                // Instantiate object using GPUInstancer
                                InstantiateWithGPUInstancer(newObject, index);
                            }
                        }
                    }
                }
            }
        }

        // This method gets the textures
        private List<int> GetTextureIndicesByLayers(TerrainData terrainData, TerrainLayer[] layers)
        {
            List<int> indices = new List<int>();
            TerrainLayer[] terrainLayers = terrainData.terrainLayers;

            foreach (TerrainLayer layer in layers)
            {
                for (int i = 0; i < terrainLayers.Length; i++)
                {
                    if (terrainLayers[i] == layer)
                    {
                        indices.Add(i);
                    }
                }
            }
            return indices;
        }

        private void SpawnGrass(TerrainData terrainData, int terrainWidth, int terrainHeight, List<int> textureIndices, float[,,] splatMaps, Terrain terrain, SpawnableObject obj, Vector3 terrainPosition, int index)
        {
            // Generating points with Poisson Disk Sampling for a uniform distribution
            List<Vector2> poissonPoints = GeneratePoissonSamples(terrainData.size.x, terrainData.size.z, obj.grassPadding);

            foreach (Vector2 point in poissonPoints)
            {
                float x = point.x + Random.Range(-0.5f, 0.5f);
                float z = point.y + Random.Range(-0.5f, 0.5f);

                // Normalise coordinates for the splatMap
                float normalizedX = x / terrainData.size.x;
                float normalizedZ = z / terrainData.size.z;

                int splatX = Mathf.FloorToInt(normalizedX * terrainWidth);
                int splatZ = Mathf.FloorToInt(normalizedZ * terrainHeight);

                // Verify if any of the textures match
                bool textureMatch = false;
                foreach (int textureIndex in textureIndices)
                {
                    if (splatX >= 0 && splatX < terrainWidth &&
                        splatZ >= 0 && splatZ < terrainHeight &&
                        textureIndex >= 0 && textureIndex < splatMaps.GetLength(2))
                    {
                        if (splatMaps[splatZ, splatX, textureIndex] > 0.2f) // Texture threshold
                        {
                            textureMatch = true;
                            break;
                        }
                    }
                }

                if (textureMatch && Random.value <= obj.spawnChance) // Probability threshold
                {
                    // Define the starting position of the raycast
                    Vector3 rayOrigin = new Vector3(x + terrainPosition.x, 500f, z + terrainPosition.z);

                    // Launch a beam downwards to find the ground surface.
                    Ray ray = new Ray(rayOrigin, Vector3.down);
                    if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
                    {
                        if (hit.collider.GetComponent<Terrain>() != null && hit.point.y >= obj.minHeight)
                        {
                            // Slope filter (if enabled)
                            if (obj.allowSlope)
                            {
                                Vector3 terrainNormal = terrainData.GetInterpolatedNormal(normalizedX, normalizedZ);
                                float slopeAngle = Vector3.Angle(Vector3.up, terrainNormal);
                                if (slopeAngle < obj.minSlopeAngle || slopeAngle > obj.maxSlopeAngle)
                                    continue;
                            }

                            // Proximity filter
                            if (!IsPositionValid(hit.point, obj.minDistanceFromObjects, obj.proximityTags))
                                continue;

                            // Create the object with rotation and random scale
                            Quaternion rotation = obj.randomRotation 
                                ? Quaternion.Euler(0, Random.Range(0, 360), 0) 
                                : Quaternion.identity;

                            GameObject newObject = Instantiate(obj.prefab, hit.point, rotation, terrain.transform);

                            if (obj.randomScale)
                            {
                                float randomScale = Random.Range(obj.scaleRange.x, obj.scaleRange.y);
                                newObject.transform.localScale = Vector3.one * randomScale;
                            }

                            // Instantiate the object using GPUInstancer
                            InstantiateWithGPUInstancer(newObject, index);
                        }
                    }
                }
            }


        }

        List<Vector2> GeneratePoissonSamples(float width, float height, float minDistance, int maxTries = 30)
        {
            List<Vector2> samples = new List<Vector2>();
            List<Vector2> activeSamples = new List<Vector2>();

            float cellSize = minDistance / Mathf.Sqrt(2);
            int gridWidth = Mathf.CeilToInt(width / cellSize);
            int gridHeight = Mathf.CeilToInt(height / cellSize);
            Vector2[,] grid = new Vector2[gridWidth, gridHeight];

            Vector2 initialSample = new Vector2(Random.Range(0, width), Random.Range(0, height));
            samples.Add(initialSample);
            activeSamples.Add(initialSample);

            while (activeSamples.Count > 0)
            {
                int index = Random.Range(0, activeSamples.Count);
                Vector2 sample = activeSamples[index];
                bool found = false;

                for (int i = 0; i < maxTries; i++)
                {
                    float angle = Random.Range(0, Mathf.PI * 2);
                    float radius = Random.Range(minDistance, 2 * minDistance);
                    Vector2 newSample = sample + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

                    if (newSample.x >= 0 && newSample.x < width && newSample.y >= 0 && newSample.y < height)
                    {
                        int gridX = Mathf.FloorToInt(newSample.x / cellSize);
                        int gridY = Mathf.FloorToInt(newSample.y / cellSize);

                        bool valid = true;
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                int nx = gridX + dx;
                                int ny = gridY + dy;
                                if (nx >= 0 && nx < gridWidth && ny >= 0 && ny < gridHeight)
                                {
                                    if (grid[nx, ny] != Vector2.zero && (grid[nx, ny] - newSample).magnitude < minDistance)
                                    {
                                        valid = false;
                                        break;
                                    }
                                }
                            }
                            if (!valid) break;
                        }

                        if (valid)
                        {
                            samples.Add(newSample);
                            activeSamples.Add(newSample);
                            grid[gridX, gridY] = newSample;
                            found = true;
                            break;
                        }
                    }
                }

                if (!found) activeSamples.RemoveAt(index);
            }

            return samples;
        }


        // Method for instantiating with GPU Instancer
        private void InstantiateWithGPUInstancer(GameObject prefab, int index)
        {
            GPUIPrefabAPI.AddPrefabInstance(gpuIPrefabManager, prefab, index);
        }

        // Method to check proximity to other objects
        bool IsPositionValid(Vector3 position, float minDistance, string[] tagsToAvoid)
        {
            Collider[] nearbyObjects = Physics.OverlapSphere(position, minDistance);

            foreach (Collider col in nearbyObjects)
            {
                foreach (string tag in tagsToAvoid)
                {
                    if (col.CompareTag(tag))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }

}
