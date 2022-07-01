using System;
using static Universe.SceneTask.Runtime.LoadLevelMode;

namespace Universe.SceneTask.Runtime
{
    public static class Level
    {
        #region Exposed

        public static LevelData s_currentLevel;

        public static TaskData CurrentAudioTask =>
            s_currentLevel.m_audio;
        public static TaskData CurrentPlayerTask =>
            s_currentLevel.m_player;
        public static SituationData CurrentSituation =>
            GetSituation(CurrentSituationIndex);
        public static SituationData NextSituation =>
            GetSituation(CurrentSituationIndex + 1);
        public static SituationData PreviousSituation =>
            GetSituation(CurrentSituationIndex - 1);
        public static int CurrentSituationIndex =>
            _currentSituationIndex;

        public static bool IsFullyLoaded => _playerTaskLoaded && _audioTaskLoaded && AreAllSituationLoaded;

        #endregion


        #region Events

        public static Action<LevelData> OnLevelLoaded;

        #endregion


        #region Public API

        public static SituationData GetSituation(int index) =>
            s_currentLevel.GetSituation(index);

        #endregion


        #region Main

        public static void ULoadLevel(this UBehaviour source, LevelData level, SituationData situation = null)
        {
            s_currentLevel = level;
            if (!situation)
                situation = level.GetSituation(0);

            source.TryLoadAudioTask();
            source.TryLoadPlayerTask();
            source.LoadSituation(situation);
        }

        public static void UChangeLevel(this UBehaviour source, LevelData level, SituationData situation = null,
            LoadLevelMode mode = LoadAll)
        {
            if (s_currentLevel)
            {
                if (mode == LoadAll)
                {
                    source.UUnloadLevel(s_currentLevel);
                }
                else
                {
                    var audio = level.m_audio;
                    var player = level.m_player;

                    if (!IsCurrentAudioEquals(audio))
                        source.UnloadAudioTask(s_currentLevel);
                    if (!IsCurrentPlayerEquals(player))
                        source.UnloadPlayerTask(s_currentLevel);

                    source.UnloadSituations(s_currentLevel);
                }
            }

            CheckpointManager.ChangeCheckpointLevel(level);
            source.ULoadLevel(level, situation);
        }

        public static void UReloadLevel(this UBehaviour source, LoadLevelMode mode = LoadAll)
        {
            var level = s_currentLevel;
            var task = GetSituation(0);

            source.UChangeLevel(level, task, mode);
        }

        public static void UUnloadLevel(this UBehaviour source, LevelData level)
        {
            source.UnloadAudioTask(level);
            source.UnloadPlayerTask(level);
            source.UnloadSituations(level);

            _currentSituationIndex = 0;
        }

        private static void LoadSituation(this UBehaviour source, SituationData situation)
        {
            var tasks = s_currentLevel.Situations;
            var index = s_currentLevel.IndexOf(situation);
            if (index < 0)
                return;

            var gameplay = situation.m_gameplay;
            var loaded = Task.IsLoaded(gameplay);
            if (loaded)
                return;

            _currentSituationIndex = index;
            source.ULoadSituation(situation);

            if (Task.IsSubscribedOnTaskLoaded(typeof(Level), nameof(OnGameplayTaskLoaded)))
                return;
            Task.OnTaskLoaded += OnGameplayTaskLoaded;
        }

        private static void UnloadSituation(this UBehaviour source, SituationData situation)
        {
            var gameplay = situation.m_gameplay;
            var loaded = Task.GetLoadedScene(gameplay).Scene.IsValid();
            if (!loaded)
                return;

            source.UUnloadSituation(situation);
        }

        #endregion


        #region Utils

        private static void TryLoadPlayerTask(this UBehaviour source)
        {
            var player = s_currentLevel.m_player;

            _playerTaskLoaded = Task.IsLoaded(player);
            if (_playerTaskLoaded)
                return;

            source.ULoadTask(player);
            Task.OnTaskLoaded += OnPlayerTaskLoaded;
        }

        private static void TryLoadAudioTask(this UBehaviour source)
        {
            var audio = s_currentLevel.m_audio;

            _audioTaskLoaded = Task.IsLoaded(audio);
            if (_audioTaskLoaded)
                return;

            source.ULoadTask(audio);
            Task.OnTaskLoaded += OnAudioTaskLoaded;
        }

        private static void UnloadPlayerTask(this UBehaviour source, LevelData of)
        {
            var player = of.m_player;

            source.UUnloadTask(player);
        }

        private static void UnloadAudioTask(this UBehaviour source, LevelData of)
        {
            var audio = of.m_audio;

            source.UUnloadTask(audio);
        }

        private static void UnloadSituations(this UBehaviour source, LevelData of)
        {
            var situations = of.Situations;

            foreach (var situation in situations)
                source.UUnloadSituation(situation);
        }

        private static void OnAudioTaskLoaded(TaskData audio)
        {
            var current = CurrentAudioTask;
            if (!audio.Equals(current))
                return;

            _audioTaskLoaded = true;
            Task.OnTaskLoaded -= OnAudioTaskLoaded;

            if (!IsFullyLoaded)
                return;

            OnLevelLoaded?.Invoke(s_currentLevel);
        }

        private static void OnPlayerTaskLoaded(TaskData player)
        {
            var current = CurrentPlayerTask;
            if (!player.Equals(current))
                return;

            _playerTaskLoaded = true;
            Task.SetFocus(player);
            Task.OnTaskLoaded -= OnPlayerTaskLoaded;

            if (!IsFullyLoaded)
                return;

            OnLevelLoaded?.Invoke(s_currentLevel);
        }

        private static void OnGameplayTaskLoaded(TaskData gameplay)
        {
            if (!AreAllSituationLoaded)
                return;

            Task.OnTaskLoaded -= OnGameplayTaskLoaded;

            if (!IsFullyLoaded)
                return;

            OnLevelLoaded?.Invoke(s_currentLevel);
        }

        private static bool IsCurrentAudioEquals(TaskData to)
        {
            if (!HasCurrentLevel)
                return false;

            var current = CurrentAudioTask;
            var currentAssetReference = current.m_assetReference;
            var otherAssetReference = to.m_assetReference;

            return currentAssetReference.Equals(otherAssetReference);
        }

        private static bool IsCurrentPlayerEquals(TaskData to)
        {
            if (!HasCurrentLevel)
                return false;

            var current = CurrentPlayerTask;
            var currentAssetReference = current.m_assetReference;
            var otherAssetReference = to.m_assetReference;

            return currentAssetReference.Equals(otherAssetReference);
        }

        private static bool AreAllSituationLoaded =>
            Situation.AreAllSituationsLoaded;
        
    #endregion


    #region Private

    private static bool HasCurrentLevel => s_currentLevel;

    private static bool _playerTaskLoaded;
    private static bool _audioTaskLoaded;

    private static int _currentSituationIndex;
    private static int _previousLevelTaskIndex;

    #endregion
    }
}