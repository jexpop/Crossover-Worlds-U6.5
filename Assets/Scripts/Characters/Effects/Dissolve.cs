using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CrossoverWorlds
{    

    class MaterialDuration
    {
        Material _material;
        float _extraDuration;

        public Material Material { get { return _material; }}
        public float ExtraDuration { get { return _extraDuration; }}

        public MaterialDuration(Material material, float duration)
        {
            this._material = material;
            this._extraDuration = duration;
        }
    }


    public class Dissolve : MonoBehaviour
    {
        public GameObject[] materialsGameObject;
        public float dissolveDuration = 2f; // Duración total de la disolución
        public float extraDelayForMeshRenderers = 0.5f; // Tiempo extra antes de iniciar la disolución para Mesh Renderers

        private List<MaterialDuration> skinnedMeshMaterials = new List<MaterialDuration>();
        private List<MaterialDuration> meshMaterials = new List<MaterialDuration>();

        private bool isDissolving = false; // Bandera para iniciar la disolución
        private float dissolveStep;
        private bool isDirectionDissolve = false;

        void Awake()
        {
            if (materialsGameObject == null || materialsGameObject.Length == 0)
            {
                Debug.LogError("No GameObjects assigned to materialsGameObject. Please assign at least one. - " + transform.parent.name);
                return;
            }

            HashSet<Material> uniqueMaterials = new HashSet<Material>(); // Usamos HashSet para evitar duplicados

            foreach (GameObject gameObject in materialsGameObject)
            {
                // Recorremos todos los renderers en este GameObject y sus hijos
                Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(true); // Obtiene todos los renderers en el objeto y sus hijos

                foreach (Renderer rend in renderers)
                {
                    if (rend != null && rend.materials.Length > 0)
                    {
                        foreach (Material mat in rend.materials.Distinct())
                        {
                            if (mat != null && mat.HasProperty("_Cutout")) // Verificamos que el material no sea nulo y contega la propiedad Cutout
                            {
                                if (!uniqueMaterials.Contains(mat))
                                {
                                    // Identificamos el tipo de renderer
                                    bool isMeshRenderer = rend.GetComponent<MeshRenderer>();

                                    // Nueva duración extra si es necesario
                                    float newExtraDuration = isMeshRenderer? extraDelayForMeshRenderers : 0.0f;

                                    MaterialDuration materialDuration = new MaterialDuration(mat, newExtraDuration);
                                    uniqueMaterials.Add(mat);
                                    if(isMeshRenderer){
                                        meshMaterials.Add(new MaterialDuration(mat, newExtraDuration));
                                    }else{
                                        skinnedMeshMaterials.Add(new MaterialDuration(mat, newExtraDuration));
                                    }
                                    //Debug.Log($"Added material: {mat.name} from GameObject: {gameObject.name}");
                                }
                            }
                            /*else
                            {
                                Debug.LogWarning($"Material {mat?.name} does not have a _Cutout property.");
                            }*/
                        }
                    }
                    /*else
                    {
                        Debug.LogWarning($"No materials found in Renderer of GameObject: {gameObject.name}");
                    }*/
                }
            }
        }

        void Update()
        {
            if (!isDissolving) return;
            if (skinnedMeshMaterials.Count > 0)
                StartCoroutine(DissolveCoroutine(skinnedMeshMaterials, Time.deltaTime, 0.0f));
            if (meshMaterials.Count > 0)
                StartCoroutine(DissolveCoroutine(meshMaterials, Time.deltaTime, 0.5f));
        }

        private IEnumerator DissolveCoroutine(List<MaterialDuration> materials, float deltaTime, float delay)
        {
            

            // Esperar el retardo (si existe)
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }
            
            bool stillDissolving = false;
            float step = deltaTime * dissolveStep;

            do
            {
                stillDissolving = false;
                foreach (MaterialDuration material in materials)
                {
                    float currentCutout = material.Material.GetFloat("_Cutout");

                    if (Mathf.Abs(currentCutout - (isDirectionDissolve ? 1 : 0)) > 0.01f)
                    {
                        stillDissolving = true;
                        float newCutout = isDirectionDissolve
                            ? Mathf.Min(1, currentCutout + step)
                            : Mathf.Max(0, currentCutout - step);
                        material.Material.SetFloat("_Cutout", newCutout);
                    }
                }

                yield return null;
            } while (stillDissolving);
        }

        public void SetEffect(float value)
        {
            foreach (MaterialDuration material in meshMaterials)
            {               
                material.Material.SetFloat("_Cutout", value);
            }foreach (MaterialDuration material in skinnedMeshMaterials)
            {               
                material.Material.SetFloat("_Cutout", value);
            }
        }

        public void StartEffect(bool direction)
        {
            isDirectionDissolve = direction;
            dissolveStep = 1.0f / dissolveDuration;
            SetEffect(isDirectionDissolve ? 0 : 1);
            isDissolving = true;
        }
    }
}