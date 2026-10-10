using System;
using UnityEngine;
using System.Collections;
using Update = UnityEngine.PlayerLoop.Update;

public class treefind : MonoBehaviour
{
    private Terrain terrain;
    private TerrainData data;
    private TreeInstance[] trees;
    private TreeInstance Nearest;
    
    // Use this for initialization
    void Start () {
            terrain = Terrain.activeTerrain;
            data = terrain.terrainData;
            trees = data.treeInstances;
            if(trees.Length > 0)
            {
                Nearest = trees[0];
                //float width = (float)data.heightmapResolution;
                //float height = (float)data.heightmapResolution;
                /*Vector3 NearPosition = Vector3.Scale(Nearest.position, data.size) + terrain.transform.position;
                foreach (TreeInstance Location in trees) {
                    Vector3 position = Vector3.Scale(Location.position, data.size) + terrain.transform.position;
                    if (Vector3.Distance (position, this.gameObject.transform.position) < Vector3.Distance (NearPosition, this.gameObject.transform.position)) {
                        Nearest = Location;
                        NearPosition = position;
                    }
                }
                NearPosition.y = terrain.SampleHeight(NearPosition);
                Debug.Log(NearPosition);*/
                /*GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = NearPosition;*/
            }
    }

    private void Update()
    {
        if (trees.Length > 0)
        {
            Vector3 NearPosition = Vector3.Scale(Nearest.position, data.size) + terrain.transform.position;
            foreach (TreeInstance Location in trees)
            {
                Vector3 position = Vector3.Scale(Location.position, data.size) + terrain.transform.position;
                if (Vector3.Distance(position, this.gameObject.transform.position) <
                    Vector3.Distance(NearPosition, this.gameObject.transform.position))
                {
                    Nearest = Location;
                    NearPosition = position;
                }
            }

            NearPosition.y = terrain.SampleHeight(NearPosition);
            Debug.Log(NearPosition);
        }
    }
    
}