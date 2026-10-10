// GPU Instancer Pro
// Copyright (c) GurBu Technologies

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GPUInstancerPro
{
    [InitializeOnLoad]
    public static class GPUIDefines
    {
        public static readonly uint GPUI_PRO_BUILD_NO = 29;
        public static Version DEMO_UPDATE_VERSION = Version.Parse("0.10.0"); // The package version where the demo scenes were last updated and needs to be reimported.
        public static Version REGENERATE_SHADERS_VERSION = Version.Parse("0.10.2"); // The package version where the automatic shader converter has been modified and the shaders will be regenerated.
        public static readonly string PACKAGE_NAME = "com.gurbu.gpui-pro";
        private static readonly string[] AUTO_PACKAGE_IMPORTER_GUIDS = { "aefcba3f5637c0a419117e2bbe53b7df" };
        public static readonly string INITIAL_PACKAGE_PATH = "Packages/com.gurbu.gpui-pro/Editor/InitialPackage.unitypackage";
        public static readonly string INITIAL_PACKAGE_URP_PATH = "Packages/com.gurbu.gpui-pro/Editor/InitialPackageURP.unitypackage";
        public static readonly string INITIAL_PACKAGE_HDRP_PATH = "Packages/com.gurbu.gpui-pro/Editor/InitialPackageHDRP.unitypackage";

        public static event Action<string> OnPackageVersionChanged;
        public static event Action<bool> OnImportPackages;
        public static event Action OnImportDemos;

        private static bool _executeOnImportDemosAfterPackageImport = false;

        private static UnityEditor.PackageManager.Requests.ListRequest _packageListRequest;

        static GPUIDefines()
        {
            GPUIRenderingSystem.editor_UpdateMethod = OnEditorUpdate;
            // Delayed to wait for asset loading
            EditorApplication.delayCall -= DelayedInitialization;
            EditorApplication.delayCall += DelayedInitialization;
        }

        static void DelayedInitialization()
        {
            string locatorPath = AssetDatabase.GUIDToAssetPath(GPUIConstants.PATH_LOCATOR_GUID);
            if (string.IsNullOrEmpty(locatorPath) || AssetDatabase.LoadAssetAtPath<GPUIPathLocator>(locatorPath) == null)
                ImportInitialPackages();

            GPUIEditorSettings editorSettings = GPUIEditorSettings.Instance;

            if (!editorSettings.HasValidVersion() || editorSettings._requirePackageReload || GPUI_PRO_BUILD_NO != editorSettings.GetBuildNo())
            {
                LoadPackageDefinitions();
                editorSettings.SetBuildNo(GPUI_PRO_BUILD_NO);
            }
            UnityEditor.PackageManager.Events.registeredPackages -= OnRegisteredPackages;
            UnityEditor.PackageManager.Events.registeredPackages += OnRegisteredPackages;

            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;

            GPUIShaderUtility.AddDefaultShaderVariants();
            GPUIShaderUtility.CheckForShaderModifications();

            editorSettings.UpdateShaderBindingValues();
        }

        public static void ImportInitialPackages()
        {
#if GPUIPRO_DEVMODE
            Debug.Log(GPUIConstants.LOG_PREFIX + "Importing initial packages...");
#endif
            GPUIEditorSettings.Instance.ImportPackageAtPath(INITIAL_PACKAGE_PATH);
            if (GPUIRuntimeSettings.Instance.IsURP)
                GPUIEditorSettings.Instance.ImportPackageAtPath(INITIAL_PACKAGE_URP_PATH);
            else if (GPUIRuntimeSettings.Instance.IsHDRP)
                GPUIEditorSettings.Instance.ImportPackageAtPath(INITIAL_PACKAGE_HDRP_PATH);
        }

        private static void OnRegisteredPackages(UnityEditor.PackageManager.PackageRegistrationEventArgs obj)
        {
            LoadPackageDefinitions();
        }

        private static void LoadPackageDefinitions()
        {
#if GPUIPRO_DEVMODE
            Debug.Log(GPUIConstants.LOG_PREFIX + "GPUIDefines Loading Package Definitions...");
#endif
            GPUIEditorSettings.Instance._requirePackageReload = false;
            EditorUtility.SetDirty(GPUIEditorSettings.Instance);
            _packageListRequest = UnityEditor.PackageManager.Client.List(true);
            EditorApplication.update -= PackageListRequestHandler;
            EditorApplication.update += PackageListRequestHandler;
        }

        private static void PackageListRequestHandler()
        {
            bool isVersionChanged = false;
            string previousVersion = GPUIEditorSettings.Instance.GetVersion();
            try
            {
                if (_packageListRequest != null)
                {
                    if (!_packageListRequest.IsCompleted)
                        return;
                    if (_packageListRequest.Result != null)
                    {
                        foreach (var packageInfo in _packageListRequest.Result)
                        {
                            if (packageInfo.name.Equals(PACKAGE_NAME))
                            {
                                if (GPUIEditorSettings.Instance.SetVersion(packageInfo.version))
                                {
#if GPUIPRO_DEVMODE
                                    Debug.Log(PACKAGE_NAME + " version changed to " + packageInfo.version);
#endif
                                    isVersionChanged = true;
                                }
                            }
                            else if (packageInfo.name.StartsWith(PACKAGE_NAME) && !packageInfo.name.EndsWith(".tests"))
                            {
                                if (GPUIEditorSettings.Instance.SetSubModuleVersion(packageInfo.name, packageInfo.version))
                                {
#if GPUIPRO_DEVMODE
                                    Debug.Log(packageInfo.name + " version changed to " + packageInfo.version);
#endif
                                    OnPackageVersionChanged?.Invoke(packageInfo.name);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            _packageListRequest = null;
            EditorApplication.update -= PackageListRequestHandler;

            if (isVersionChanged)
                DoVersionUpdate(previousVersion);
        }


        private static void DoVersionUpdate(string previousVersionText)
        {
            if (Version.TryParse(previousVersionText, out Version previousVersion) && Version.TryParse(GPUIEditorSettings.Instance.GetVersion(), out Version currentVersion))
            {
                #region Reimport demo scenes
                if (GPUIProDemoImporter.IsDemosImported() && previousVersion.CompareTo(DEMO_UPDATE_VERSION) < 0 && currentVersion.CompareTo(DEMO_UPDATE_VERSION) >= 0)
                {
                    Debug.Log(GPUIConstants.LOG_PREFIX + "GPUI Pro is importing new demo scenes for version " + GPUIEditorSettings.Instance.GetVersion() + "...");
                    GPUIRuntimeSettings.Instance.DetermineRenderPipeline();
                    GPUIProDemoImporter.ImportDemos(GPUIRuntimeSettings.Instance.RenderPipeline);
                }
                #endregion Reimport demo scenes
                #region Regenerate shaders
                if ( previousVersion.CompareTo(REGENERATE_SHADERS_VERSION) < 0 && currentVersion.CompareTo(REGENERATE_SHADERS_VERSION) >= 0)
                {
                    if (GPUIShaderBindings.Instance.shaderInstances != null && GPUIShaderBindings.Instance.shaderInstances.Count > 0)
                    {
                        Debug.Log(GPUIConstants.LOG_PREFIX + "GPUI Pro is regenerating shaders for version " + GPUIEditorSettings.Instance.GetVersion() + "...");
                        GPUIShaderUtility.RegenerateShaders();
                    }
                }
                #endregion Regenerate shaders
            }

            ImportPackages(false);
        }

        public static void ImportPackages(bool forceReimport)
        {
            GPUIProPackageImporter.ImportPackages(AUTO_PACKAGE_IMPORTER_GUIDS, forceReimport);
            GPUIConstants.ReimportComputeShaders();
            OnImportPackages?.Invoke(forceReimport);
        }

        public static void OnDemosImported()
        {
            _executeOnImportDemosAfterPackageImport = true;
        }

        #region Editor Update

        private static void OnEditorUpdate()
        {
            GPUIEditorSettings editorSettings = GPUIEditorSettings.Instance;
            if (GPUIRenderingSystem.IsActive)
            {
                if (editorSettings.isAutoShaderConversion)
                    GPUIShaderUtility.AutoShaderConverterUpdate();
            }

            if (!Application.isPlaying)
            {
                if (editorSettings.packageImportList != null && editorSettings.packageImportList.Count > 0)
                {
                    string packagePath = editorSettings.packageImportList[0];
                    editorSettings.packageImportList.RemoveAt(0);
                    EditorUtility.SetDirty(editorSettings);
                    if (System.IO.File.Exists(packagePath))
                        AssetDatabase.ImportPackage(packagePath, false);
#if GPUIPRO_DEVMODE
                    else
                        Debug.LogWarning(GPUIConstants.LOG_PREFIX + "Can not find Demo package at path: " + packagePath);
#endif

                    if (_executeOnImportDemosAfterPackageImport && editorSettings.packageImportList.Count == 0)
                    {
                        _executeOnImportDemosAfterPackageImport = false;
                        OnImportDemos?.Invoke();
                    }
                }
            }
        }

        #endregion Editor Update
    }
}
