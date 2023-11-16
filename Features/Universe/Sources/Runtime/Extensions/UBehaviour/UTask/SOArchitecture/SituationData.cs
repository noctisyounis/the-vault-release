namespace Universe.SceneTask.Runtime
{
    public class SituationData : UniverseScriptableObject
    {
        #region Exposed

        public string m_name;
        public TaskData m_blockMeshEnvironment;
        public TaskData m_artEnvironment;
        public TaskData m_gameplay;

        public TaskData[] m_playstation5SpecificTasks;
        public TaskData[] m_metaSpecificTasks;
        public TaskData[] m_win64SpecificTasks;

        public bool m_isCheckpoint;

        #endregion
    }
}