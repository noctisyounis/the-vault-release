using System;
using System.Collections.Generic;
using UnityEngine;
using static Universe.SceneTask.Runtime.LoadLevelMode;
using static Universe.SceneTask.Runtime.Task;
using static Universe.SceneTask.Runtime.Situation;
using static Universe.SceneTask.Runtime.CheckpointManager;

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
        public static Action<LevelData> OnLevelUnloading;
        public static Action<LevelData> OnLevelUnloaded;

        #endregion


        #region Public API

        public static SituationData GetSituation(int index) =>
            s_currentLevel.GetSituation(index);

        public static void UpdateCurrentSituation(SituationData situation)
        {
            var index = s_currentLevel.IndexOf(situation);
            if (index < 0)
                return;
            
            _currentSituationIndex = index;
        }

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

                    if (situation == null)
                    {
                        UnloadingLevel.Add(s_currentLevel);
                        UnloadingLevelStatus.Add(s_currentLevel, 0b00);
                        TrySubscribeOnUnloads();
                    }

                    if (!IsCurrentAudioEquals(audio))
                        source.UnloadAudioTask(s_currentLevel);

                    if (!IsCurrentPlayerEquals(player))
                        source.UnloadPlayerTask(s_currentLevel);

                    source.UnloadOtherSituations(s_currentLevel, situation);
                    source.UReloadGameplay(situation);
                }
            }

            ChangeCheckpointLevel(level);
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
            OnLevelUnloading?.Invoke(level);

            if (!IsUnloading(level))
            {
                UnloadingLevel.Add(level);
                UnloadingLevelStatus.Add(level, 0b00);
            }
            
            TrySubscribeOnUnloads();
            
            source.UnloadAudioTask(level);
            source.UnloadPlayerTask(level);
            source.UnloadSituations(level);

            _currentSituationIndex = 0;
        }

        private static void LoadSituation(this UBehaviour source, SituationData situation)
        {
            var index = s_currentLevel.IndexOf(situation);
            if (index < 0)
                return;

            var gameplay = situation.m_gameplay;
            var loaded = IsLoaded(gameplay);
            if (loaded)
                return;

            _currentSituationIndex = index;
            source.ULoadSituation(situation);

            if (Task.IsSubscribedOnTaskLoaded(typeof(Level), nameof(OnGameplayTaskLoaded)))
                return;
            OnTaskLoaded += OnGameplayTaskLoaded;
        }

        private static void UnloadSituation(this UBehaviour source, SituationData situation)
        {
            var gameplay = situation.m_gameplay;
            var loaded = GetLoadedScene(gameplay).Scene.IsValid();
            if (!loaded)
                return;

            source.UUnloadSituation(situation);
        }

        #endregion


        #region Utils

        private static void TryLoadPlayerTask(this UBehaviour source)
        {
            var player = s_currentLevel.m_player;

            _playerTaskLoaded = IsLoaded(player);
            if (_playerTaskLoaded)
                return;

            source.ULoadTask(player);
            OnTaskLoaded += OnPlayerTaskLoaded;
        }

        private static void TryLoadAudioTask(this UBehaviour source)
        {
            var audio = s_currentLevel.m_audio;

            _audioTaskLoaded = IsLoaded(audio);
            if (_audioTaskLoaded)
                return;

            source.ULoadTask(audio);
            OnTaskLoaded += OnAudioTaskLoaded;
        }

        private static void UnloadPlayerTask(this UBehaviour source, LevelData of)
        {
            var player = of.m_player;

            source.UUnloadTask(player);
            if(IsUnloading(s_currentLevel)) UnloadingLevelStatus[s_currentLevel] |= 0b01;
        }

        private static void UnloadAudioTask(this UBehaviour source, LevelData of)
        {
            var audio = of.m_audio;

            source.UUnloadTask(audio);
            if(IsUnloading(s_currentLevel)) UnloadingLevelStatus[s_currentLevel] |= 0b10;
        }

        private static void UnloadSituations(this UBehaviour source, LevelData of)
        {
            var situations = of.Situations;

            foreach (var situation in situations)
                source.UUnloadSituation(situation);
        }

        private static void UnloadOtherSituations(this UBehaviour source, LevelData of, SituationData keep)
        {
            var situations = of.Situations;

            foreach (var situation in situations)
            {
                if (situation.Equals(keep)) continue;
                
                source.UUnloadSituation(situation);
            }
        }

        private static void OnAudioTaskLoaded(TaskData audio)
        {
            var current = CurrentAudioTask;
            if (!audio.Equals(current))
                return;

            _audioTaskLoaded = true;
            OnTaskLoaded -= OnAudioTaskLoaded;

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
            SetFocus(player);
            OnTaskLoaded -= OnPlayerTaskLoaded;

            if (!IsFullyLoaded)
                return;

            OnLevelLoaded?.Invoke(s_currentLevel);
        }

        private static void OnGameplayTaskLoaded(TaskData gameplay)
        {
            if (!AreAllSituationLoaded)
                return;

            OnTaskLoaded -= OnGameplayTaskLoaded;

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
        
        private static void RefreshUnloadingLevel( TaskData task )
        {
            var count = UnloadingLevel.Count;
            
            for( var i = count-1; i >= 0; i-- )
            {
                var level = UnloadingLevel[i];
                var status = UnloadingLevelStatus[level];
                var player = level.m_player;
                var audio = level.m_audio;

                if (task.Equals(player)) status &= 0b10;
                if (task.Equals(audio)) status &= 0b01;

                UnloadingLevelStatus[level] = status;

                if (status > 0 || !AreAllSituationUnloaded) continue;

                UnloadingLevel.Remove(level);
                UnloadingLevelStatus.Remove(level);
                OnLevelUnloaded?.Invoke(level);
            }

            TryUnsubscribe();
        }
        
        private static void RefreshUnloadingLevel( SituationData situation )
        {
            var count = UnloadingLevel.Count;
            
            for( var i = count-1; i >= 0; i-- )
            {
                var level = UnloadingLevel[i];
                var status = UnloadingLevelStatus[level];

                if (status > 0 || !AreAllSituationUnloaded) continue;

                UnloadingLevel.Remove(level);
                UnloadingLevelStatus.Remove(level);
                OnLevelUnloaded?.Invoke(level);
            }

            TryUnsubscribe();
        }

        private static void TrySubscribeOnUnloads()
        {
            if (!IsSubscribedOnSituationUnloaded(typeof(Level), nameof(RefreshUnloadingLevel)))
                OnSituationUnloaded += RefreshUnloadingLevel;
            if (!IsSubscribedOnTaskUnloaded(typeof(Level), nameof(RefreshUnloadingLevel)))
                OnTaskUnloaded += RefreshUnloadingLevel;
        }
        
        private static void TryUnsubscribe()
        {
            if (UnloadingLevelStatus.Count > 0) return;

            OnTaskUnloaded -= RefreshUnloadingLevel;
            OnSituationUnloaded -= RefreshUnloadingLevel;
        }

        private static bool IsUnloading(LevelData level) => UnloadingLevel.Contains(level);
        private static bool AreAllSituationLoaded => AreAllSituationsLoaded;
        private static bool AreAllSituationUnloaded => AreAllSituationsUnloaded;
        private static List<LevelData> UnloadingLevel => _unloadingLevel ??= new();
        private static Dictionary<LevelData, byte> UnloadingLevelStatus => _unloadingLevelStatus ??= new();

        #endregion


    #region Private

    private static bool HasCurrentLevel => s_currentLevel;

    private static bool _playerTaskLoaded;
    private static bool _audioTaskLoaded;

    private static int _currentSituationIndex;
    private static int _previousLevelTaskIndex;
    
    private static List<LevelData> _unloadingLevel;
    private static Dictionary<LevelData, byte> _unloadingLevelStatus;

    #endregion
    }
}