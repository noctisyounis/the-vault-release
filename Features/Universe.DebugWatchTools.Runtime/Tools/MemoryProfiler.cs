using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using Universe.DebugWatch.Runtime;

namespace Universe.DebugWatchTools.Runtime
{
    public class MemoryProfiler : UBehaviour
    {
        #region Public Members

        public GameObjectFact m_debugArrayManager;
        public MemoryProfilerBuilderData[] m_profilers;

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
                DisplayMemoryProfilers();
                return;
            }
 
            HideMemoryProfilers();
        }

        #endregion


        #region Utils

        private void DisplayMemoryProfilers()
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

                    var logValue = Mathf.Log10(value);

                    if(logValue >= 9)
                    {
                        var valueAsGb = value * BYTE_TO_GIGABYTE;
                        return $"{valueAsGb:0.00} Gb"; 
                    }
                    if(logValue >= 6)
                    {
                        var valueAsMb = value * BYTE_TO_MEGABYTE;
                        return $"{valueAsMb:0.00} Mb"; 
                    }
                    if(logValue >= 3)
                    {
                        var valueAsKb = value * BYTE_TO_KILOBYTE;
                        return $"{valueAsKb:0.00} kb"; 
                    }

                    return $"{value:0.00} b";
                });
            }
        }

        private void HideMemoryProfilers()
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

        private const float BYTE_TO_KILOBYTE = 0.001f;
        private const float BYTE_TO_MEGABYTE = 0.000001f;
        private const float BYTE_TO_GIGABYTE = 0.000000001f;

        private static event Action OnDisplayChanged;
        private static bool s_display;
        private Dictionary<string, ProfilerRecorder> _profilerRecorders;

        #endregion
    }
}