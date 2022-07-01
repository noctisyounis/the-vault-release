using UnityEngine.AddressableAssets;

namespace Universe.SceneTask.Runtime
{
    public class TaskData : UniverseScriptableObject
    {
        #region Exposed

        public AssetReference m_assetReference;
        public TaskPriority m_priority = TaskPriority.ALWAYS_UPDATE;
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
    }
}