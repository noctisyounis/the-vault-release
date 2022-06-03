using System;

namespace Universe.SceneTask.Runtime
{
    public static class Checkpoint
    {
        #region Exposed

        public static CheckpointData m_currentCheckpoint;

        #endregion


        #region Event

        public static Action<CheckpointData> OnCheckpointLoaded;

        #endregion


        #region Main

        public static void ULoadCheckpoint( this UBehaviour source, CheckpointData checkpoint, LoadLevelMode mode = LoadLevelMode.LoadAll )
        {
            var level = checkpoint.m_level;
            var task = checkpoint.m_task;
            
            m_currentCheckpoint.m_level = level;
            m_currentCheckpoint.m_task = task;

            source.UChangeLevel( level, task, mode );

            Level.OnLevelLoaded += LevelLoaded;
        }

        public static void UReloadCheckpoint( this UBehaviour source, LoadLevelMode mode = LoadLevelMode.LoadAll )
        {
            var level = m_currentCheckpoint.m_level;
            var task = m_currentCheckpoint.m_task;

            source.UChangeLevel( level, task, mode );
         
            Level.OnLevelLoaded += LevelLoaded;
        }

        #endregion


        #region Utils

        private static void LevelLoaded( LevelData level )
        {
            OnCheckpointLoaded?.Invoke(m_currentCheckpoint);

            Level.OnLevelLoaded -= LevelLoaded;
        }

        #endregion
    }
}