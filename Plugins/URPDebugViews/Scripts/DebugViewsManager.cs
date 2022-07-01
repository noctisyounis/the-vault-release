//
// URP Debug Views for Unity
// (c) 2019 PH Graphics
// Source code may be used and modified for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset from a pirate site, please consider buying it from the Unity asset store. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace URPDebugViews
{
    public class DebugViewsManager
    {
        private static DebugViewsManager _instance;

        public static DebugViewsManager Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = new DebugViewsManager();
                return _instance;
            }
        }

        private const string RPAssetPathEditorPref = "URPDebugViews_RPAssetPath";
        private const string DebugViewIndexEditorPref = "URPDebugViews_DebugViewIndex";

        private DebugViewData _currentViewData;
        private List<DebugViewData> _debugViewDatas = new List<DebugViewData>();

#if !UNITY_EDITOR
        private UniversalRenderPipelineAsset _urpAsset;
        
#endif
        private DebugViewRenderPipelineAsset _debugViewRenderPipelineAsset;

        private bool _correctlyLoaded;

        public DebugViewData CurrentViewData
        {
            get { return _currentViewData; }
        }
        
        public List<DebugViewData> AllAvailableDebugViews
        {
            get { return _debugViewDatas; }
        }

        private RenderPipelineAsset GetCurrentRenderPipelineAsset()
        {
            var qualityRPAsset = QualitySettings.renderPipeline;
            if (qualityRPAsset)
                return qualityRPAsset;

            return GraphicsSettings.currentRenderPipeline;
        }

        private void SetNewRenderPipelineAsset(RenderPipelineAsset rpAsset)
        {
            GraphicsSettings.renderPipelineAsset = rpAsset;
        }

        private DebugViewsManager()
        {
            LoadAllData();

            ValidateRenderPipeline();
            
            #if UNITY_EDITOR
                EditorApplication.quitting += EditorApplicationOnQuitting;
                EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
            #endif
        }

#if UNITY_EDITOR
        private void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange stateChange)
        { 
            if (stateChange == PlayModeStateChange.ExitingEditMode)
            {
                PopURPAssetIfPossible();
            }
            else if (stateChange == PlayModeStateChange.EnteredEditMode)
            {
                _currentViewData = null;
            }
        }
        
        private void EditorApplicationOnQuitting()
        {
            PopURPAssetIfPossible();
        }
        #endif

        private void ValidateRenderPipeline()
        {
            if (!CheckForCorrectlyLoaded(false))
                return;

            if (GetCurrentRenderPipelineAsset() == _debugViewRenderPipelineAsset)
            {
                const int notSetValue = -1;
                int savedDebugViewIndex = notSetValue;
                
#if UNITY_EDITOR
                savedDebugViewIndex = EditorPrefs.GetInt(DebugViewIndexEditorPref, notSetValue);
#endif
                
                if (savedDebugViewIndex == notSetValue || savedDebugViewIndex >= _debugViewDatas.Count)
                {
                    // load a default view that can use the debug view render pipeline
                    _currentViewData = _debugViewDatas.FirstOrDefault(data => !data.RenderObjectsNormallyBefore);
                }
                else
                {
                    _currentViewData = _debugViewDatas[savedDebugViewIndex];
                }
            }
        }

        private void SaveURPAssetIfPossible()
        {
            var urpAsset = GetCurrentRenderPipelineAsset() as UniversalRenderPipelineAsset;
            if (urpAsset)
            {
                BackupURPAsset(urpAsset);
            }
        }

        public bool PopURPAssetIfPossible()
        {
            var urpAsset = GetSavedURPAsset();
            if (!urpAsset)
                return false;

            SetNewRenderPipelineAsset(urpAsset);

            return true;
        }

        private void BackupURPAsset(UniversalRenderPipelineAsset urpAsset)
        {
#if UNITY_EDITOR
            var urpAssetPath = AssetDatabase.GetAssetPath(urpAsset);
            EditorPrefs.SetString(RPAssetPathEditorPref, urpAssetPath);
            return;
#else
            _urpAsset = urpAsset;  
#endif
        }

        private UniversalRenderPipelineAsset GetSavedURPAsset()
        {
#if UNITY_EDITOR
            var urpAssetPath = EditorPrefs.GetString(RPAssetPathEditorPref);
            return AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(urpAssetPath);
#else

            return _urpAsset;
#endif
        }

        public void GetShadowsDistances(out Vector4 shadowDistances)
        {
            shadowDistances = Vector4.zero;
            
            if (!CheckForCorrectlyLoaded())
            {
                return;
            }

            var urpAsset = GetSavedURPAsset();

            if (!urpAsset)
                return;

            float shadowDistance = urpAsset.shadowDistance;
            switch (urpAsset.shadowCascadeCount)
            {
                case 0:
                    break;
                
                case 2:
                {
                    shadowDistances.x = urpAsset.cascade2Split * shadowDistance;
                    shadowDistances.y = shadowDistance;
                    break;
                }
                
                case 4:
                {
                    shadowDistances.x = urpAsset.cascade4Split.x * shadowDistance;
                    shadowDistances.y = urpAsset.cascade4Split.y * shadowDistance;
                    shadowDistances.z = urpAsset.cascade4Split.z * shadowDistance;
                    shadowDistances.w = shadowDistance;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        
#if UNITY_EDITOR
        
        private void LoadAllData()
        {
            // Debug View Data
            _debugViewDatas = new List<DebugViewData>();
            var assets = AssetDatabase.FindAssets("t:DebugViewData");
            if (assets != null)
            {
                foreach (var assetGUID in assets)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
                    var data = AssetDatabase.LoadAssetAtPath<DebugViewData>(assetPath);
                    _debugViewDatas.Add(data);
                }
            }

            // Debug View Render Pipeline
            var renderPipelines = AssetDatabase.FindAssets("t:DebugViewRenderPipelineAsset");
            if (renderPipelines != null && renderPipelines.Length > 0)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(renderPipelines[0]);
                _debugViewRenderPipelineAsset = AssetDatabase.LoadAssetAtPath<DebugViewRenderPipelineAsset>(assetPath);
            }
            
            // URP Render Pipeline
            SaveURPAssetIfPossible();
            
            RefreshCorrectlyLoaded();
        }
        
        #else

        private void LoadAllData()
        {
            const string DebugViewFolder = "DebugViews";
            
            var debugViewDatas = Resources.LoadAll<DebugViewData>(DebugViewFolder);
            if (debugViewDatas != null && debugViewDatas.Length > 0)
            {
                _debugViewDatas = new List<DebugViewData>(debugViewDatas);
            }
            
            var renderPipelines = Resources.LoadAll<DebugViewRenderPipelineAsset>(DebugViewFolder);
            if (renderPipelines != null && renderPipelines.Length > 0)
            {
                _debugViewRenderPipelineAsset = renderPipelines[0];
            }

            RefreshCorrectlyLoaded();
        }

        #endif

        private void RefreshCorrectlyLoaded()
        {
            _correctlyLoaded = _debugViewDatas != null && _debugViewDatas.Count > 0 && _debugViewRenderPipelineAsset;
        }

        private bool CheckForCorrectlyLoaded(bool outputLog = true)
        {
            #if UNITY_EDITOR
            if (!_correctlyLoaded)
            {
                // attempt to fix the problem
                LoadAllData();

                if (_correctlyLoaded)
                    return _correctlyLoaded;
            }
            #endif
            
            if (!_correctlyLoaded)
            {
                if (outputLog)
                {
                    #if UNITY_EDITOR
                        Debug.LogError("Can't enable debug views some data isn't correctly loaded. Try restarting Unity. If it persists, please contact support.");
                    #else
                        Debug.LogError("Can't enable debug views some data isn't correctly loaded. Have you forgotten to rename the Data folder to Resources? If you don't know, please refer to the documentation.");
                    #endif
                }
            }

            return _correctlyLoaded;
        }

        /// <summary>
        /// Enable a specific debug view.
        /// Pass null to disable debug views.
        /// You can browse available debug views with AllAvailableDebugViews property.
        /// </summary>
        /// <param name="data"></param>
        public void EnableView(DebugViewData data)
        {
            if (!CheckForCorrectlyLoaded())
                return;
            
            SaveURPAssetIfPossible();

            bool success;
            
            if (!data || data.RenderObjectsNormallyBefore)
            {
                success = PopURPAssetIfPossible();
            }
            else
            {
                success = true;
                SetNewRenderPipelineAsset(_debugViewRenderPipelineAsset);
            }

            if (success)
            {
                _currentViewData = data;
                
#if UNITY_EDITOR
                EditorPrefs.SetInt(DebugViewIndexEditorPref, _debugViewDatas.IndexOf(_currentViewData));
#endif
            }
        }
        
        public void EnableView(int index)
        {
            if (!CheckForCorrectlyLoaded())
                return;

            if (index > _debugViewDatas.Count - 1)
            {
                Debug.LogError("Debug Views: Error in EnableView(index), trying to access out of bound element.");
                return;
            }

            var selectedData = _debugViewDatas[index];
            EnableView(selectedData);
        }
        

        /// <summary>
        /// Shortcut to use in hardcoded code like menu items.
        /// Enable a debug view by material name. If none exist, it'll disable debug views.
        /// </summary>
        /// <param name="viewName"></param>
        public void EnableViewWithMaterialName(string viewName)
        {
            if (!CheckForCorrectlyLoaded())
                return;
            
            var selectedData = _debugViewDatas.FirstOrDefault(data => data.Material.name == viewName);
            EnableView(selectedData);
        }
    }
}