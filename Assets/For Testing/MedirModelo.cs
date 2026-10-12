using UnityEngine;

public class MedirModelo : MonoBehaviour
{
    void Start()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        Bounds b = renderers[0].bounds;
        foreach (var r in renderers) b.Encapsulate(r.bounds);
        Debug.Log("Tamaño total: " + b.size);
    }
}