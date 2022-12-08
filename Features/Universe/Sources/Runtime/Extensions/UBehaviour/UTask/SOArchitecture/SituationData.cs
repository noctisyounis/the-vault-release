namespace Universe.SceneTask.Runtime
{
    public class SituationData : UniverseScriptableObject
    {
        #region Exposed

        public string m_name;
        public TaskData m_blockMeshEnvironment;
        public TaskData m_artEnvironment;
        public TaskData m_gameplay;
        public bool m_isCheckpoint;

        #endregion
    }
}