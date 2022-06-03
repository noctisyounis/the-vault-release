namespace Universe.SceneTask.Runtime
{
    public class CheckpointManager : UBehaviour
    {
        #region Exposed

        public CheckpointData m_editorCheckpoint;
        public CheckpointData m_gameplayCheckpoint;

        public static CheckpointData EditorCheckPoint => s_editorCheckpoint;
        public static CheckpointData GameplayCheckPoint => s_gameplayCheckpoint;

        #endregion


        #region Unit API

        public override void Awake()
        {
            base.Awake();
            s_editorCheckpoint = m_editorCheckpoint;
            s_gameplayCheckpoint = m_gameplayCheckpoint;

            Checkpoint.m_currentCheckpoint = m_gameplayCheckpoint;
        }

        #endregion


        #region Private

        private static CheckpointData s_editorCheckpoint;
        private static CheckpointData s_gameplayCheckpoint;

        #endregion
    }
}