#ifndef GPU_INSTANCER_PRO_VARIATION_INCLUDED
#define GPU_INSTANCER_PRO_VARIATION_INCLUDED
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
    StructuredBuffer<float4> GrassVariationBuffer;
#endif
void setupVariationGPUI()
{
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
    setupGPUI();
#ifdef GPUI_MATERIAL_VARIATION
    
    _Pushed_Amount = GrassVariationBuffer[gpui_InstanceID * 2 + 0].x;
    _Foot_Position = GrassVariationBuffer[gpui_InstanceID * 2 + 1];
#endif
#endif
}
// Dummy methods for Shader Graph
void gpuiVaritationDummy_float(float3 inVector3, out float3 outVector3)
{
    outVector3 = inVector3;
}

void gpuiVaritationDummy_half(half3 inVector3, out half3 outVector3)
{
    outVector3 = inVector3;
}
#endif // GPU_INSTANCER_PRO_VARIATION_INCLUDED