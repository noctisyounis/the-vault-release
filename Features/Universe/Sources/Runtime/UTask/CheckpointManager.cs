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


        #region Unity API

        public override void Awake()
        {
            base.Awake();
            s_editorCheckpoint = m_editorCheckpoint;
            s_gameplayCheckpoint = m_gameplayCheckpoint;

            Checkpoint.s_currentCheckpoint = m_gameplayCheckpoint;
        }

        #endregion


        #region Public API

        public static void ChangeCheckpointLevel(LevelData to)
        {
            if (to == null) return;
            
            GameplayCheckPoint.m_level = to;
        }

        public static void ChangeCheckpointSituation(SituationData to)
        {
            if (to == null) return;
            if (!GameplayCheckPoint) return;
            if (!GameplayCheckPoint.m_level) return;
            if (!GameplayCheckPoint.m_level.Situations.Contains(to)) return;

            GameplayCheckPoint.m_situation = to;
        }

        #endregion


        #region Private

        private static CheckpointData s_editorCheckpoint;
        private static CheckpointData s_gameplayCheckpoint;

        #endregion
    }
}