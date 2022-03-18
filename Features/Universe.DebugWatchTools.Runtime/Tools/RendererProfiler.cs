using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using Universe.DebugWatch.Runtime;


namespace Universe.DebugWatchTools.Runtime
{
    public class RendererProfiler : UBehaviour
    {
        #region Public Members

        [Header("Profiler Settings")]
        public GameObjectFact m_debugArrayManager;

        public RendererProfilerBuilderData[] m_profilers;

        #endregion


        #region Unity API

        public override void Awake()
        {
            OnDisplayChanged += UpdateDisplay;

            _profilerRecorders = new();
        }

        public override void OnDestroy() => OnDisplayChanged -= UpdateDisplay;
        
        #endregion


        #region Main

        public static void ToggleDisplay()
        {
            s_display = !s_display;

            OnDisplayChanged?.Invoke();
        }
        
        private void UpdateDisplay()
        {
            if(s_display)
            {
                DisplayRendererProfilers();
                return;
            }
            
            HideRendererProfilers();
        }

        #endregion


        #region Utils

        private void DisplayRendererProfilers()
        {
            GetDebugArrayManager(out var debugArrayManager);
            if(!debugArrayManager) return;

            foreach (var profiler in m_profilers)
            {
                var category    = profiler.m_category;
                var name        = profiler.m_name;
                var displayName = profiler.m_displayName;

                if(_profilerRecorders.ContainsKey(name)) continue;

                var newRecorder = ProfilerRecorder.StartNew(category, name);

                _profilerRecorders.Add(name, newRecorder);

                debugArrayManager.AddEntry(profiler.m_displayName, () =>
                {
                    var value = _profilerRecorders[profiler.m_name].LastValue;

                    return $"{value}";
                });
            }
        }

        private void HideRendererProfilers()
        {
            GetDebugArrayManager(out var debugArrayManager);
            if(!debugArrayManager) return;

            foreach (var profiler in m_profilers)
            {
                var name        = profiler.m_name;
                var displayName = profiler.m_displayName;

                if(!_profilerRecorders.ContainsKey(name)) continue;

                _profilerRecorders[name].Dispose();
                _profilerRecorders.Remove(name);
                debugArrayManager.RemoveEntry(displayName);
            }
        }

        private void GetDebugArrayManager(out DebugArrayManager debugArrayManager)
        {
            debugArrayManager = null;
            var debugArrayObject = m_debugArrayManager.Value;
            if(!debugArrayObject) return;

            debugArrayManager = debugArrayObject.GetComponent<DebugArrayManager>();
        }
        
        #endregion


        #region Private Members

        private static event Action OnDisplayChanged;
        private static bool s_display;
        private Dictionary<string, ProfilerRecorder> _profilerRecorders;

        #endregion
    }
}