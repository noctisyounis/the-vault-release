using System;
using UnityEngine;
using Unity.Profiling;

namespace Universe.DebugWatchTools.Runtime
{
    [Serializable]
    public struct RendererProfilerBuilderData
    {
        #region Public Members

        [HideInInspector]
        public ProfilerCategory m_category;
        public string m_name;
        public string m_displayName;

        #endregion


        #region Constructor

        public RendererProfilerBuilderData(string name, string displayName)
        {
            m_name = name;
            m_displayName = displayName;
            m_category = ProfilerCategory.Render;
        }

        #endregion
    }
}