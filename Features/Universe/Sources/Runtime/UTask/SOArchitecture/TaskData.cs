using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Universe.SceneTask.Runtime
{
    public class TaskData : ScriptableObject
    {
        #region Exposed

        public AssetReference m_assetReference;
        public TaskPriority m_priority = TaskPriority.PERSISTENT;
        public bool m_canBeLoadOnlyOnce = true;
        public bool m_alwaysUpdated = false;

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
    }
}