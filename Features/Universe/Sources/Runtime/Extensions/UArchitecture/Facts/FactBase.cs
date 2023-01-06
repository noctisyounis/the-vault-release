using System;
using UnityEngine;

namespace Universe
{
    public abstract class FactBase : UniverseScriptableObject
    {
        #region Public
        public new abstract string ToString();
        public virtual Type Type { get; set; }
        
        [Header("Editor"), Space(10)]
        public bool m_isFavorite;
        public bool m_useVerboseOnChange;
        
        [Header("Options"), Space(10)]
        public bool m_isAnalytics;
        public USaveLevel m_saveLevel;
        
        [Header("Gameplay")]
        public bool m_isReadOnly;
        public bool m_washOnAwakeAndCompilation = true;

        #endregion
        
        
        #region Events

        public delegate void OnValueChangedHandler(FactBase next);
        public OnValueChangedHandler OnValueChanged;

        #endregion
    }
}