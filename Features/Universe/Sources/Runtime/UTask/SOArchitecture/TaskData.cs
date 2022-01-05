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

        #endregion
    }
}