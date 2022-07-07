using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Universe.SceneTask.Runtime
{
    public class TaskData : UniverseScriptableObject, IComparable<TaskData>
    {
        #region Exposed

        public AssetReference m_assetReference;
        public TaskPriority m_priority = TaskPriority.ALWAYS_UPDATE;
        [Tooltip("Has input priority when multiple high priority tasks are loaded?")]
        public bool m_inputPriority = false;
        public bool m_canBeLoadOnlyOnce = true;

        #endregion


        #region Public API

        public string GetTrimmedName()
        {
            var splited = name.Split('-');
            var trimmed = "";
            var amount = splited.Length;

            for( var i = 1; i < amount; i++ )
            {
                trimmed += splited[i];
                if( i < amount - 1 )
                    trimmed += "-";
            }

            return trimmed;
        }

        #endregion
        
        
        #region Utils

        public int CompareTo(TaskData other)
        {
            var otherPriority = other.m_priority;
            var otherInputPriority = other.m_inputPriority;
            var comparedPriority = m_priority.CompareTo(otherPriority);

            if (comparedPriority != 0)                  
                return comparedPriority;

            var comparedInputPriority = m_inputPriority.CompareTo(otherInputPriority);
            return comparedInputPriority;
        }
        
        #endregion
    }
}