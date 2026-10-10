// GPU Instancer Pro
// Copyright (c) GurBu Technologies

using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace GPUInstancerPro
{
    public class GPUITransformBufferData : IGPUIDisposable
    {
        private GPUIRenderSourceGroup _renderSourceGroup;
        public GPUIRenderSourceGroup RenderSourceGroup {  get { return _renderSourceGroup; } }

        /// <summary>
        /// Contains transform buffers (per camera when CameraBased)
        /// </summary>
        private Dictionary<int, GPUIShaderBuffer> _transformBufferDict;
        /// <summary>
        /// Contains transform buffers (per camera when CameraBased)
        /// </summary>
        public Dictionary<int, GPUIShaderBuffer>.ValueCollection TransformBufferValues => _transformBufferDict == null ? null : _transformBufferDict.Values;

        public GraphicsBuffer PreviousFrameTransformBuffer { get; private set; }
        private bool _hasPreviousFrameTransformBuffer;
        public bool HasPreviousFrameTransformBuffer
        {
            get => _hasPreviousFrameTransformBuffer;
            private set
            {
                if (_hasPreviousFrameTransformBuffer != value)
                {
                    _hasPreviousFrameTransformBuffer = value;
                    if (!GPUIShaderBindings.Instance.stripObjectMotionVectorVariants)
                    {
                        if (value)
                        {
                            if (_renderSourceGroup.AddShaderKeyword(GPUIConstants.Kw_GPUI_OBJECT_MOTION_VECTOR_ON))
                                _renderSourceGroup.RemoveReplacementMaterials();
                        }
                        else
                        {
                            if (_renderSourceGroup.RemoveShaderKeyword(GPUIConstants.Kw_GPUI_OBJECT_MOTION_VECTOR_ON))
                                _renderSourceGroup.RemoveReplacementMaterials();
                        }
                    }
                    else if (value)
                    {
                        Debug.LogError(GPUIConstants.LOG_PREFIX + "Can not generate Per Object Motion Vector data. Disabled by the Editor Settings.");
                        _hasPreviousFrameTransformBuffer = false;
                    }
                }
            }
        }
        private int _previousFrameBufferFrameNo;

        internal GraphicsBuffer _perInstanceLightProbesBuffer;
        private bool _hasPerInstanceLightProbes;
        public bool HasPerInstanceLightProbes
        {
            get => _hasPerInstanceLightProbes;
            private set
            {
                if (_hasPerInstanceLightProbes != value)
                {
                    _hasPerInstanceLightProbes = value;
                    if (!GPUIShaderBindings.Instance.stripPerInstanceLightProbeVariants)
                    {
                        if (value)
                        {
                            if (_renderSourceGroup.AddShaderKeyword(GPUIConstants.Kw_GPUI_PER_INSTANCE_LIGHTPROBES_ON))
                                _renderSourceGroup.RemoveReplacementMaterials();
                        }
                        else
                        {
                            if (_renderSourceGroup.RemoveShaderKeyword(GPUIConstants.Kw_GPUI_PER_INSTANCE_LIGHTPROBES_ON))
                                _renderSourceGroup.RemoveReplacementMaterials();
                        }
                    }
                    else if (value)
                    {
                        Debug.LogError(GPUIConstants.LOG_PREFIX + "Can not generate Per Instance Light Probe data. Disabled by the Editor Settings.");
                        _hasPerInstanceLightProbes = false;
                    }
                }
            }
        }
        private bool _isAllowPerInstanceLightProbes;

        /// <summary>
        /// Contains results of the visibility calculations for LODs and shadows (such as instanceID and crossFadeValue) for each camera
        /// </summary>
        private Dictionary<int, GPUIShaderBuffer> _instanceDataBufferDict;

        public int resetCrossFadeDataFrame;

        public bool IsCameraBasedBuffer => _renderSourceGroup.TransformBufferType == GPUITransformBufferType.CameraBased;

        /// <summary>
        /// Keeps bit-wise enabled/disabled status for each instance's optional renderers. Each instance is represented with a 32 bit uint - so they can have maximum 32 optional renderers. 0 means enabled, 1 means disabled, so all renderers would be enabled by default.
        /// </summary>
        private GraphicsBuffer _optionalRendererStatusBuffer;

        public bool IsGeneratePerInstanceLightProbes => _isAllowPerInstanceLightProbes && _renderSourceGroup.Profile.lightProbeSetting == GPUILightProbeSetting.PerInstance;

        public GPUITransformBufferData(GPUIRenderSourceGroup renderSourceGroup)
        {
            _renderSourceGroup = renderSourceGroup;

            if (Application.isPlaying && !IsCameraBasedBuffer && !GPUIRuntimeSettings.Instance.DisableShaderBuffers && !GPUIRuntimeSettings.Instance.IsBuiltInRP && renderSourceGroup.LODGroupData != null && renderSourceGroup.Profile != null && renderSourceGroup.Profile.enablePerObjectMotionVectors && !GPUIShaderBindings.Instance.stripObjectMotionVectorVariants)
            {
                if (renderSourceGroup.LODGroupData.HasObjectMotion())
                {
                    HasPreviousFrameTransformBuffer = true;
                    _previousFrameBufferFrameNo = -1;
                }
            }

            _isAllowPerInstanceLightProbes = Application.isPlaying && !IsCameraBasedBuffer && !GPUIRuntimeSettings.Instance.DisableShaderBuffers && !GPUIRuntimeSettings.IsAdaptiveProbeVolumesEnabled() && !GPUIShaderBindings.Instance.stripPerInstanceLightProbeVariants;
        }

        public void ReleaseBuffers()
        {
            ReleaseTransformBuffers();
            ReleaseInstanceDataBuffers();
            ReleaseOptionalRendererBuffers();
            ReleaseLightProbeBuffers();
        }

        internal void ReleaseTransformBuffers()
        {
            if (_transformBufferDict != null)
            {
                foreach (var tb in _transformBufferDict.Values)
                {
                    if (tb != null)
                        tb.Dispose();
                }
                _transformBufferDict = null;
            }
            if (PreviousFrameTransformBuffer != null)
                PreviousFrameTransformBuffer.Dispose();
        }

        internal void ReleaseInstanceDataBuffers()
        {
            if (_instanceDataBufferDict != null)
            {
                foreach (GPUIShaderBuffer instanceDataBuffer in _instanceDataBufferDict.Values)
                {
                    if (instanceDataBuffer != null)
                        instanceDataBuffer.Dispose();
                }
                _instanceDataBufferDict = null;
            }
        }

        internal void ReleaseOptionalRendererBuffers()
        {
            if (_optionalRendererStatusBuffer != null)
            {
                _optionalRendererStatusBuffer.Dispose();
                _optionalRendererStatusBuffer = null;
            }
        }

        internal void ReleaseLightProbeBuffers()
        {
            HasPerInstanceLightProbes = false;
            if (_perInstanceLightProbesBuffer != null)
            {
                _perInstanceLightProbesBuffer.Dispose();
                _perInstanceLightProbesBuffer = null;
            }
        }

        internal void ReleaseInstanceDataBuffers(GPUICameraData cameraData)
        {
            int key = cameraData.ActiveCamera.GetInstanceID();
            if (_instanceDataBufferDict != null && _instanceDataBufferDict.TryGetValue(key, out GPUIShaderBuffer instanceDataBuffer))
            {
                if (instanceDataBuffer != null)
                    instanceDataBuffer.Dispose();
                _instanceDataBufferDict.Remove(key);
            }
        }

        public void Dispose()
        {
            ReleaseBuffers();
            _transformBufferDict = null;
            _instanceDataBufferDict = null;
        }

        internal void Dispose(GPUICameraData cameraData)
        {
            if (IsCameraBasedBuffer && cameraData != null && cameraData.ActiveCamera != null)
            {
                int key = cameraData.ActiveCamera.GetInstanceID();
                if (_transformBufferDict != null && _transformBufferDict.TryGetValue(key, out var tb))
                {
                    tb.Dispose();
                    _transformBufferDict.Remove(key);
                }
                if (_instanceDataBufferDict != null && _instanceDataBufferDict.TryGetValue(key, out var idb))
                {
                    idb.Dispose();
                    _instanceDataBufferDict.Remove(key);
                }
            }
        }

        internal void ResizeTransformBuffer(bool isCopyPreviousData)
        {
            if (IsCameraBasedBuffer)
            {
                Dispose();
                return;
            }

            if (_transformBufferDict == null)
                _transformBufferDict = new();

            _transformBufferDict.TryGetValue(0, out GPUIShaderBuffer previousTransformBuffer);
            bool hasPreviousBuffer = previousTransformBuffer != null;
            int previousBufferSize = hasPreviousBuffer ? previousTransformBuffer.BufferSize : 0;

            if (!hasPreviousBuffer || previousBufferSize != _renderSourceGroup.BufferSize)
            {
                GPUIShaderBuffer transformBuffer = CreateTransformBuffer();
                _transformBufferDict[0] = transformBuffer;

                if (previousTransformBuffer != null)
                {
                    if (isCopyPreviousData)
                        transformBuffer.Buffer.SetData(previousTransformBuffer.Buffer, 0, 0, Math.Min(_renderSourceGroup.BufferSize, previousBufferSize));
                    previousTransformBuffer.Dispose();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="previousTransformBuffer"></param>
        /// <param name="transformBuffer"></param>
        /// <returns>True if an existing buffer is replaced with a new one</returns>
        internal bool ResizeTransformBuffer(out GPUIShaderBuffer previousTransformBuffer, out GPUIShaderBuffer transformBuffer)
        {
            previousTransformBuffer = null;
            transformBuffer = null;
            if (IsCameraBasedBuffer)
            {
                Dispose();
                return false;
            }

            if (_transformBufferDict == null)
                _transformBufferDict = new();

            _transformBufferDict.TryGetValue(0, out previousTransformBuffer);
            bool hasPreviousBuffer = previousTransformBuffer != null;
            int previousBufferSize = hasPreviousBuffer ? previousTransformBuffer.BufferSize : 0;

            if (!hasPreviousBuffer || previousBufferSize != _renderSourceGroup.BufferSize)
            {
                transformBuffer = CreateTransformBuffer();
                _transformBufferDict[0] = transformBuffer;
                return true;
            }
            return false;
        }

        public unsafe void CalculateInterpolatedLightAndOcclusionProbes<T>(NativeArray<T> matrices, int managedBufferStartIndex, int graphicsBufferStartIndex, int count) where T : unmanaged
        {
            if (IsGeneratePerInstanceLightProbes)
            {
                HasPerInstanceLightProbes = true;
                Vector3 offset = RenderSourceGroup.Profile.lightProbePositionOffset;
                offset += RenderSourceGroup.LODGroupData.bounds.center;
                GPUIRenderingSystem.Instance.CalculateInterpolatedLightAndOcclusionProbes(this, NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(matrices), managedBufferStartIndex, graphicsBufferStartIndex, count, RenderSourceGroup.BufferSize, offset);
            }
            else if (HasPerInstanceLightProbes)
                ReleaseLightProbeBuffers();
        }

        public unsafe void CalculateInterpolatedLightAndOcclusionProbes<T>(T[] matrices, int managedBufferStartIndex, int graphicsBufferStartIndex, int count) where T : unmanaged
        {
            if (IsGeneratePerInstanceLightProbes)
            {
                HasPerInstanceLightProbes = true;
                Vector3 offset = RenderSourceGroup.Profile.lightProbePositionOffset;
                offset += RenderSourceGroup.LODGroupData.bounds.center;
                fixed (void* ptr = matrices)
                    GPUIRenderingSystem.Instance.CalculateInterpolatedLightAndOcclusionProbes(this, ptr, managedBufferStartIndex, graphicsBufferStartIndex, count, RenderSourceGroup.BufferSize, offset);
            }
            else if (HasPerInstanceLightProbes)
                ReleaseLightProbeBuffers();
        }

        public unsafe void CalculateInterpolatedLightAndOcclusionProbes<T>(List<T> matrices, int managedBufferStartIndex, int graphicsBufferStartIndex, int count) where T : unmanaged
        {
            if (IsGeneratePerInstanceLightProbes)
            {
                HasPerInstanceLightProbes = true;
                Vector3 offset = RenderSourceGroup.Profile.lightProbePositionOffset;
                offset += RenderSourceGroup.LODGroupData.bounds.center;
                T[] matricesArray = GPUIUtility.GetListInternalArray(matrices);
                fixed (void* ptr = matricesArray)
                    GPUIRenderingSystem.Instance.CalculateInterpolatedLightAndOcclusionProbes(this, ptr, managedBufferStartIndex, graphicsBufferStartIndex, count, RenderSourceGroup.BufferSize, offset);
            }
            else if (HasPerInstanceLightProbes)
                ReleaseLightProbeBuffers();
        }

        internal void SetTransformBufferData<T>(NativeArray<T> matrices, int managedBufferStartIndex, int graphicsBufferStartIndex, int count, bool isOverwritePreviousFrameBuffer) where T : unmanaged
        {
            GPUIShaderBuffer transformBuffer = GetTransformBuffer();
            transformBuffer.Buffer.SetData(matrices, managedBufferStartIndex, graphicsBufferStartIndex, count);
            if (isOverwritePreviousFrameBuffer && HasPreviousFrameTransformBuffer && PreviousFrameTransformBuffer != null && graphicsBufferStartIndex < PreviousFrameTransformBuffer.count)
                PreviousFrameTransformBuffer.SetData(matrices, managedBufferStartIndex, graphicsBufferStartIndex, Math.Min(count, PreviousFrameTransformBuffer.count - graphicsBufferStartIndex));

            CalculateInterpolatedLightAndOcclusionProbes(matrices, managedBufferStartIndex, graphicsBufferStartIndex, count);

            OnTransformDataModified();
        }

        internal void SetTransformBufferData<T>(T[] matrices, int managedBufferStartIndex, int graphicsBufferStartIndex, int count, bool isOverwritePreviousFrameBuffer) where T : unmanaged
        {
            GPUIShaderBuffer transformBuffer = GetTransformBuffer();
            transformBuffer.Buffer.SetData(matrices, managedBufferStartIndex, graphicsBufferStartIndex, count);
            if (isOverwritePreviousFrameBuffer && HasPreviousFrameTransformBuffer && PreviousFrameTransformBuffer != null && graphicsBufferStartIndex < PreviousFrameTransformBuffer.count)
                PreviousFrameTransformBuffer.SetData(matrices, managedBufferStartIndex, graphicsBufferStartIndex, Math.Min(count, PreviousFrameTransformBuffer.count - graphicsBufferStartIndex));

            CalculateInterpolatedLightAndOcclusionProbes(matrices, managedBufferStartIndex, graphicsBufferStartIndex, count);

            OnTransformDataModified();
        }

        internal void SetTransformBufferData<T>(List<T> matrices, int managedBufferStartIndex, int graphicsBufferStartIndex, int count, bool isOverwritePreviousFrameBuffer) where T : unmanaged
        {
            GPUIShaderBuffer transformBuffer = GetTransformBuffer();
            transformBuffer.Buffer.SetData(matrices, managedBufferStartIndex, graphicsBufferStartIndex, count);
            if (isOverwritePreviousFrameBuffer && HasPreviousFrameTransformBuffer && PreviousFrameTransformBuffer != null && graphicsBufferStartIndex < PreviousFrameTransformBuffer.count)
                PreviousFrameTransformBuffer.SetData(matrices, managedBufferStartIndex, graphicsBufferStartIndex, Math.Min(count, PreviousFrameTransformBuffer.count - graphicsBufferStartIndex));

            CalculateInterpolatedLightAndOcclusionProbes(matrices, managedBufferStartIndex, graphicsBufferStartIndex, count);

            OnTransformDataModified();
        }

        internal void RemoveIndexes(int startIndex, int count)
        {
            if (IsCameraBasedBuffer)
            {
                Debug.LogError(GPUIConstants.LOG_PREFIX + "RemoveIndexes method can not be used with Camera Based transform buffers.");
                return;
            }

            GPUIShaderBuffer transformBuffer = CreateTransformBuffer();

            if (_transformBufferDict.TryGetValue(0, out GPUIShaderBuffer previousTransformBuffer))
            {
                transformBuffer.Buffer.SetData(previousTransformBuffer.Buffer, 0, 0, startIndex);
                transformBuffer.Buffer.SetData(previousTransformBuffer.Buffer, startIndex + count, startIndex, _renderSourceGroup.BufferSize - startIndex);
                previousTransformBuffer.Dispose();
            }

            _transformBufferDict[0] = transformBuffer;
            OnTransformDataModified();
        }

        private GPUIShaderBuffer CreateTransformBuffer()
        {
            //Debug.Log(GPUIConstants.LOG_PREFIX + "Creating new buffer with size: " + _renderSourceGroup.bufferSize);
            ReleaseInstanceDataBuffers();
            return new GPUIShaderBuffer(_renderSourceGroup.BufferSize, 4 * 4 * 4);
        }

        private GPUIShaderBuffer CreateInstanceDataBuffer(int instanceDataBufferSize)
        {
            resetCrossFadeDataFrame = Time.frameCount;
            return new GPUIShaderBuffer(instanceDataBufferSize, 4 * 4);
        }

        public GPUIShaderBuffer GetTransformBuffer()
        {
            if (IsCameraBasedBuffer)
            {
                Debug.LogError(GPUIConstants.LOG_PREFIX + "GetTransformBuffer method can not be used with Camera Based transform buffers.");
                return null;
            }
            if (_transformBufferDict == null)
                _transformBufferDict = new();
            else if (_transformBufferDict.TryGetValue(0, out GPUIShaderBuffer transformBuffer) && transformBuffer != null)
                return transformBuffer;
            GPUIShaderBuffer result = CreateTransformBuffer();
            _transformBufferDict[0] = result;
            return result;
        }

        public GPUIShaderBuffer GetTransformBuffer(GPUICameraData cameraData)
        {
            if (!IsCameraBasedBuffer || cameraData == null)
                return GetTransformBuffer();
            int key = cameraData.ActiveCamera.GetInstanceID();
            if (_transformBufferDict == null)
                _transformBufferDict = new();
            if (!_transformBufferDict.TryGetValue(key, out var result) || result == null)
            {
                result = CreateTransformBuffer();
                _transformBufferDict[key] = result;
            }
            return result;
        }

        public GPUIShaderBuffer GetInstanceDataBuffer(GPUICameraData cameraData)
        {
            int key = cameraData.ActiveCamera.GetInstanceID();
            if (_instanceDataBufferDict == null)
                _instanceDataBufferDict = new();

            GPUILODGroupData lodGroupData = _renderSourceGroup.LODGroupData;
            Debug.Assert(lodGroupData != null, "Can not find GPUILODGroupData");

            int instanceDataBufferSize = _renderSourceGroup.BufferSize * ((lodGroupData.Length + lodGroupData.optionalRendererCount)
                * (_renderSourceGroup.Profile.isShadowCasting ? 2 : 1)  // multiply with 2 for shadows
                + (_renderSourceGroup.Profile.isLODCrossFade && _renderSourceGroup.Profile.isAnimateCrossFade && !IsCameraBasedBuffer ? 1 : 0) // +1 for keeping crossfading state
                );

            if (!_instanceDataBufferDict.TryGetValue(key, out GPUIShaderBuffer instanceDataBuffer) || instanceDataBuffer == null || instanceDataBuffer.BufferSize != instanceDataBufferSize)
            {
                if (instanceDataBuffer != null)
                    instanceDataBuffer.Dispose();
                instanceDataBuffer = CreateInstanceDataBuffer(instanceDataBufferSize);
                _instanceDataBufferDict[key] = instanceDataBuffer;
            }
            return instanceDataBuffer;
        }

        public void SetMPBBuffers(MaterialPropertyBlock mpb, GPUICameraData cameraData)
        {
            GPUIShaderBuffer transformBuffer = GetTransformBuffer(cameraData);
            GPUIShaderBuffer instanceDataBuffer = GetInstanceDataBuffer(cameraData);
            if (GPUIRuntimeSettings.Instance.DisableShaderBuffers)
            {
                mpb.SetTexture(GPUIConstants.PROP_gpuiTransformBufferTexture, transformBuffer.Texture);
                mpb.SetTexture(GPUIConstants.PROP_gpuiInstanceDataBufferTexture, instanceDataBuffer.Texture);
            }
            else
            {
                mpb.SetBuffer(GPUIConstants.PROP_gpuiTransformBuffer, transformBuffer.Buffer);
                mpb.SetBuffer(GPUIConstants.PROP_gpuiInstanceDataBuffer, instanceDataBuffer.Buffer);

                #region Motion Vectors
                if (HasPreviousFrameTransformBuffer)
                {
                    bool hasMotionVectorBuffer = PreviousFrameTransformBuffer != null;
                    mpb.SetInt(GPUIConstants.PROP_hasPreviousFrameTransformBuffer, hasMotionVectorBuffer ? 1 : 0);
                    if (hasMotionVectorBuffer)
                    {
                        int bufferSize = transformBuffer.Buffer.count;
                        if (PreviousFrameTransformBuffer.count < bufferSize)
                        {
                            int previousBufferSize = PreviousFrameTransformBuffer.count;
                            GraphicsBuffer newBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, bufferSize, 4 * 4 * 4);
                            newBuffer.SetData(PreviousFrameTransformBuffer, 0, 0, previousBufferSize);
                            newBuffer.SetData(transformBuffer.Buffer, previousBufferSize, previousBufferSize, bufferSize - previousBufferSize);
                            PreviousFrameTransformBuffer.Release();
                            PreviousFrameTransformBuffer = newBuffer;
                        }
                        mpb.SetBuffer(GPUIConstants.PROP_gpuiPreviousFrameTransformBuffer, PreviousFrameTransformBuffer);
                    }
                    else
                        mpb.SetBuffer(GPUIConstants.PROP_gpuiPreviousFrameTransformBuffer, GPUIRenderingSystem.Instance.DummyGraphicsBuffer);
                }
                #endregion Motion Vectors

                #region Light Probes
                if (HasPerInstanceLightProbes)
                {
                    if (RenderSourceGroup.Profile.lightProbeSetting != GPUILightProbeSetting.PerInstance)
                    {
                        ReleaseLightProbeBuffers();
                    }
                    else
                    {
                        bool hasPerInstanceLightProbeBuffer = _perInstanceLightProbesBuffer != null;
                        mpb.SetInt(GPUIConstants.PROP_hasPerInstanceLightProbes, hasPerInstanceLightProbeBuffer ? 1 : 0);
                        if (hasPerInstanceLightProbeBuffer)
                            mpb.SetBuffer(GPUIConstants.PROP_gpuiPerInstanceLightProbesBuffer, _perInstanceLightProbesBuffer);
                        else
                            mpb.SetBuffer(GPUIConstants.PROP_gpuiPerInstanceLightProbesBuffer, GPUIRenderingSystem.Instance.DummyGraphicsBuffer);
                    }
                }
                #endregion Light Probes
            }
            mpb.SetFloat(GPUIConstants.PROP_transformBufferSize, _renderSourceGroup.BufferSize);
            mpb.SetFloat(GPUIConstants.PROP_instanceDataBufferSize, instanceDataBuffer.BufferSize);
            mpb.SetFloat(GPUIConstants.PROP_maxTextureSize, GPUIConstants.TEXTURE_MAX_SIZE);
        }

        internal void UpdateData(int frameNo)
        {
            if (HasPreviousFrameTransformBuffer)
            {
                if (_renderSourceGroup.BufferSize > 0 && frameNo > _previousFrameBufferFrameNo && _transformBufferDict.TryGetValue(0, out GPUIShaderBuffer transformBuffer) && transformBuffer.Buffer != null)
                {
                    _previousFrameBufferFrameNo = frameNo;
                    int bufferSize = transformBuffer.Buffer.count;
                    if (PreviousFrameTransformBuffer == null)
                        PreviousFrameTransformBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, bufferSize, 4 * 4 * 4);
                    else if (PreviousFrameTransformBuffer.count != bufferSize)
                    {
                        PreviousFrameTransformBuffer.Release();
                        PreviousFrameTransformBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, bufferSize, 4 * 4 * 4);
                    }

                    PreviousFrameTransformBuffer.SetData(transformBuffer.Buffer, 0, 0, bufferSize);
                }
            }
        }

        public void OnTransformDataModified()
        {
            if (_transformBufferDict == null)
                return;
            foreach (var tb in _transformBufferDict.Values)
            {
                if (tb != null)
                    tb.OnDataModified();
            }
            GPUIRenderingSystem.OnBufferDataModified?.Invoke(this);
        }

        public void OnTransformDataModified(GPUIShaderBuffer tb)
        {
            tb.OnDataModified();
            GPUIRenderingSystem.OnBufferDataModified?.Invoke(this);
        }

        public void ResetPreviousFrameBuffer()
        {
            UpdateData(_previousFrameBufferFrameNo + 1);
            _previousFrameBufferFrameNo = -1;
        }

        public void SetOptionalRendererStatusBufferData(NativeArray<uint> optionalRendererStatusData, int bufferStartIndex)
        {
            int bufferSize = RenderSourceGroup.BufferSize;
            if (_optionalRendererStatusBuffer != null && _optionalRendererStatusBuffer.count != bufferSize)
            {
                _optionalRendererStatusBuffer.Dispose();
                _optionalRendererStatusBuffer = null;
            }
            if (_optionalRendererStatusBuffer == null)
            {
                _optionalRendererStatusBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, bufferSize, 4);
                _optionalRendererStatusBuffer.ClearBufferData();
            }
            _optionalRendererStatusBuffer.SetData(optionalRendererStatusData, 0, bufferStartIndex, Mathf.Min(optionalRendererStatusData.Length, bufferSize - bufferStartIndex));
        }

        public GraphicsBuffer GetOptionalRendererStatusBuffer()
        {
            if (_optionalRendererStatusBuffer == null)
                return null;
            int bufferSize = RenderSourceGroup.BufferSize;
            if (_optionalRendererStatusBuffer.count != bufferSize)
            {
                var previousBuffer = _optionalRendererStatusBuffer;
                _optionalRendererStatusBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, bufferSize, 4);
                _optionalRendererStatusBuffer.ClearBufferData();
                _optionalRendererStatusBuffer.SetData(previousBuffer, 0, 0, Mathf.Min(previousBuffer.count, bufferSize));
                previousBuffer.Dispose();
            }
            return _optionalRendererStatusBuffer;
        }
    }

    public enum GPUITransformBufferType
    {
        /// <summary>
        /// One buffer for each prototype
        /// </summary>
        Default = 0,
        /// <summary>
        /// One buffer for each prototype and camera combination
        /// </summary>
        CameraBased = 1,
    }

    [Serializable]
    public struct GPUITransformData : IEquatable<GPUITransformData>
    {
        public Vector3 position;    // Translation (12 bytes)
        public Quaternion rotation; // Quaternion for rotation (16 bytes)
        public Vector3 scale;       // Non-uniform scale (12 bytes)

        public const int STRIDE = 40;

        public void SetFromMatrix(Matrix4x4 matrix)
        {
            position = matrix.GetPosition();
            rotation = matrix.rotation;
            scale = matrix.lossyScale;
        }

        public Matrix4x4 ToMatrix()
        {
            return Matrix4x4.TRS(position, rotation, scale);
        }

        public void SetToTransform(Transform transform)
        {
            transform.SetPositionAndRotation(position, rotation);
            transform.localScale = scale;
        }

        public void SetTransformRelativeToParent(Matrix4x4 parentLTW, Matrix4x4 childLTW)
        {
            // The calculations can be performed in a single line of code, as shown below. However, manually calculating without allocations yields a more performant result.
            // SetFromMatrix(parentLTW.inverse * childLTW);

            // Parent scale and inverse scale
            float invSx = 1f / Mathf.Sqrt(parentLTW.m00 * parentLTW.m00 + parentLTW.m10 * parentLTW.m10 + parentLTW.m20 * parentLTW.m20);
            float invSy = 1f / Mathf.Sqrt(parentLTW.m01 * parentLTW.m01 + parentLTW.m11 * parentLTW.m11 + parentLTW.m21 * parentLTW.m21);
            float invSz = 1f / Mathf.Sqrt(parentLTW.m02 * parentLTW.m02 + parentLTW.m12 * parentLTW.m12 + parentLTW.m22 * parentLTW.m22);

            // Inverse rotation (transpose + scale correction)
            float ir0x = parentLTW.m00 * invSx, ir0y = parentLTW.m01 * invSy, ir0z = parentLTW.m02 * invSz;
            float ir1x = parentLTW.m10 * invSx, ir1y = parentLTW.m11 * invSy, ir1z = parentLTW.m12 * invSz;
            float ir2x = parentLTW.m20 * invSx, ir2y = parentLTW.m21 * invSy, ir2z = parentLTW.m22 * invSz;

            // Relative rotation matrix: r = inverse(parentRot) * childRot
            float rr0x = ir0x * childLTW.m00 + ir0y * childLTW.m10 + ir0z * childLTW.m20;
            float rr0y = ir1x * childLTW.m00 + ir1y * childLTW.m10 + ir1z * childLTW.m20;
            float rr0z = ir2x * childLTW.m00 + ir2y * childLTW.m10 + ir2z * childLTW.m20;

            float rr1x = ir0x * childLTW.m01 + ir0y * childLTW.m11 + ir0z * childLTW.m21;
            float rr1y = ir1x * childLTW.m01 + ir1y * childLTW.m11 + ir1z * childLTW.m21;
            float rr1z = ir2x * childLTW.m01 + ir2y * childLTW.m11 + ir2z * childLTW.m21;

            float rr2x = ir0x * childLTW.m02 + ir0y * childLTW.m12 + ir0z * childLTW.m22;
            float rr2y = ir1x * childLTW.m02 + ir1y * childLTW.m12 + ir1z * childLTW.m22;
            float rr2z = ir2x * childLTW.m02 + ir2y * childLTW.m12 + ir2z * childLTW.m22;

            // Extract scale
            float sx = Mathf.Sqrt(rr0x * rr0x + rr0y * rr0y + rr0z * rr0z);
            float sy = Mathf.Sqrt(rr1x * rr1x + rr1y * rr1y + rr1z * rr1z);
            float sz = Mathf.Sqrt(rr2x * rr2x + rr2y * rr2y + rr2z * rr2z);
            scale.x = sx;
            scale.y = sy;
            scale.z = sz;

            // Normalize rotation axes
            rr0x /= sx; rr0y /= sx; rr0z /= sx;
            rr1x /= sy; rr1y /= sy; rr1z /= sy;
            rr2x /= sz; rr2y /= sz; rr2z /= sz;

            // Build quaternion manually from rotation matrix
            float trace = rr0x + rr1y + rr2z;
            float qw, qx, qy, qz;

            if (trace > 0f)
            {
                float s = Mathf.Sqrt(trace + 1f) * 0.5f;
                float invS = 0.25f / s;
                qw = s;
                qx = (rr1z - rr2y) * invS;
                qy = (rr2x - rr0z) * invS;
                qz = (rr0y - rr1x) * invS;
            }
            else if (rr0x > rr1y && rr0x > rr2z)
            {
                float s = Mathf.Sqrt(1f + rr0x - rr1y - rr2z) * 0.5f;
                float invS = 0.25f / s;
                qw = (rr1z - rr2y) * invS;
                qx = s;
                qy = (rr0y + rr1x) * invS;
                qz = (rr2x + rr0z) * invS;
            }
            else if (rr1y > rr2z)
            {
                float s = Mathf.Sqrt(1f + rr1y - rr0x - rr2z) * 0.5f;
                float invS = 0.25f / s;
                qw = (rr2x - rr0z) * invS;
                qx = (rr0y + rr1x) * invS;
                qy = s;
                qz = (rr1z + rr2y) * invS;
            }
            else
            {
                float s = Mathf.Sqrt(1f + rr2z - rr0x - rr1y) * 0.5f;
                float invS = 0.25f / s;
                qw = (rr0y - rr1x) * invS;
                qx = (rr2x + rr0z) * invS;
                qy = (rr1z + rr2y) * invS;
                qz = s;
            }

            rotation.x = qx;
            rotation.y = qy;
            rotation.z = qz;
            rotation.w = qw;

            // Relative position = inverseRot * (childPos - parentPos)
            float dx = childLTW.m03 - parentLTW.m03;
            float dy = childLTW.m13 - parentLTW.m13;
            float dz = childLTW.m23 - parentLTW.m23;

            position.x = (ir0x * dx + ir0y * dy + ir0z * dz) * invSx;
            position.y = (ir1x * dx + ir1y * dy + ir1z * dz) * invSy;
            position.z = (ir2x * dx + ir2y * dy + ir2z * dz) * invSz;
        }

        public bool Equals(GPUITransformData other)
        {
            return position == other.position && rotation == other.rotation && scale == other.scale;
        }

        public override bool Equals(object obj)
        {
            if (obj is GPUITransformData crowdTransformData)
                return Equals(crowdTransformData);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return GPUIUtility.GenerateHash(position.GetHashCode(), rotation.GetHashCode(), scale.GetHashCode());
        }

        public static bool operator ==(GPUITransformData left, GPUITransformData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GPUITransformData left, GPUITransformData right)
        {
            return !(left == right);
        }

        private static readonly string TO_STRING_TEXT = "Position: {0}, Rotation: {1}, Scale: {2}";
        public override string ToString()
        {
            return string.Format(TO_STRING_TEXT, position.ToString("F4"), rotation.ToString("F4"), scale.ToString("F4"));
        }
    };
}