#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Universe.SceneTask.Runtime
{
    public class LevelSettings : UniverseScriptableObject
    {
        #region Exposed

        public Environment m_startingEnvironment;
        public CheckpointData m_runtimeCheckpoint;
        public CheckpointData m_editorCheckpoint;

        #endregion


        #region Main

        public override void SaveAsset()
        {
            base.SaveAsset();
#if UNITY_EDITOR
            m_runtimeCheckpoint.SaveAsset();
            m_editorCheckpoint.SaveAsset();
#endif
        }

        #endregion
    }
}
