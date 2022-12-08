using System;

namespace Universe.SceneTask.Runtime
{
    public static class Checkpoint
    {
        #region Exposed

        public static CheckpointData s_currentCheckpoint;

        #endregion


        #region Event

        public static Action<CheckpointData> OnCheckpointLoaded;
        public static Action<CheckpointData> OnCheckpointSaved;

        #endregion


        #region Main

        public static void USaveCheckpoint( this UBehaviour source )
        {
            source.USaveCheckpoint( s_currentCheckpoint );
        }

        public static void USaveCheckpoint( this UBehaviour source, CheckpointData checkpoint )
        {
            var currentLevel = Level.s_currentLevel;
            var currentSituation = Level.CurrentSituation;

            source.USaveCheckpoint( checkpoint, currentLevel, currentSituation );
        }

        public static void USaveCheckpoint( this UBehaviour source, CheckpointData checkpoint, LevelData level, SituationData situation )
        {
            checkpoint.m_level = level;
            checkpoint.m_situation = situation;

            OnCheckpointSaved?.Invoke(checkpoint);
        }

        public static void ULoadCheckpoint( this UBehaviour source, LoadLevelMode mode )
        {
            source.ULoadCheckpoint( s_currentCheckpoint, mode );
        }

        public static void ULoadCheckpoint( this UBehaviour source, CheckpointData checkpoint, LoadLevelMode mode = LoadLevelMode.LoadAll )
        {
            var level = checkpoint.m_level;
            var situation = checkpoint.m_situation;
            
            s_currentCheckpoint.m_level = level;
            s_currentCheckpoint.m_situation = situation;

            source.UChangeLevel( level, situation, mode );

            Level.OnLevelLoaded += LevelLoaded;
        }

        public static void UReloadCheckpoint( this UBehaviour source, LoadLevelMode mode = LoadLevelMode.LoadAll )
        {
            var level = s_currentCheckpoint.m_level;
            var situation = s_currentCheckpoint.m_situation;

            source.UChangeLevel( level, situation, mode );
         
            Level.OnLevelLoaded += LevelLoaded;
        }

        #endregion


        #region Utils

        private static void LevelLoaded( LevelData level )
        {
            OnCheckpointLoaded?.Invoke(s_currentCheckpoint);

            Level.OnLevelLoaded -= LevelLoaded;
        }

        #endregion
    }
}
