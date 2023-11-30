using System;
using System.Collections.Generic;

namespace Universe.SceneTask.Runtime
{
    public static class Situation
    {
        #region Exposed

        public static Environment CurrentEnvironment
        {
            get => _currentEnvironment;
            set => _currentEnvironment = value;
        }

        public static bool AreAllSituationsLoaded   => ( LoadingSituations.Count == 0 && LoadedSituations.Count > 0); 
        public static bool CanLoadArt               => ( CurrentEnvironment & Environment.ART ) != 0;
        public static bool CanLoadBlockMesh         => ( CurrentEnvironment & Environment.BLOCK_MESH ) != 0;

        #endregion


        #region Events

        public static Action<SituationData> OnSituationLoaded;

        #endregion


        #region Main

        public static void ULoadSituation( this UBehaviour source, SituationData situation )
        {
            if(LoadingSituations.Contains(situation)) return;
            if(LoadedSituations.Contains(situation)) return;

            LoadingSituations.Add( situation );

            if( !Task.IsSubscribedOnTaskLoaded( typeof( Situation ), nameof( OnAnyTaskLoaded ) ) )
                Task.OnTaskLoaded += OnAnyTaskLoaded;

            source.TryLoadBlockMeshTask( situation );
            source.TryLoadArtTask( situation );
            source.TryLoadGameplayTask( situation );
            source.TryLoadPlatformSpecificTask( situation );
            
            Level.UpdateCurrentSituation(situation);

            if (!situation.m_isCheckpoint)
                return;
            
            CheckpointManager.ChangeCheckpointSituation(situation);
        }

        public static void UUnloadSituation( this UBehaviour source, SituationData situation )
        {
            if(!LoadedSituations.Contains( situation )) return;

            LoadedSituations.Remove( situation );

            source.TryUnloadBlockMeshTask( situation );
            source.TryUnloadArtTask( situation );
            source.TryUnloadGameplayTask( situation );
            source.TryUnloadPlatformSpecificTask( situation );
        }

        public static void UReloadGameplay(this UBehaviour source, SituationData of)
        {
            if(!LoadedSituations.Contains( of )) return;
            
            var gameplay = of.m_gameplay;

            source.UUnloadTask( gameplay );
            source.ULoadTask( gameplay );
        }

        #endregion


        #region Utils

        private static void TryLoadBlockMeshTask( this UBehaviour source, SituationData of )
        {
            if( CanLoadBlockMesh )
            {
                var blockMesh = of.m_blockMeshEnvironment;
                var loaded = Task.IsLoaded( blockMesh );
                if( loaded )
                    return;

                source.ULoadTask( blockMesh );
            }
        }

        private static void TryLoadArtTask( this UBehaviour source, SituationData of )
        {
            if( CanLoadArt )
            {
                var art = of.m_artEnvironment;
                var loaded = Task.IsLoaded( art );

                if( loaded )
                    return;

                source.ULoadTask( art );
            }
        }

        private static void TryLoadGameplayTask( this UBehaviour source, SituationData of )
        {
            var gameplay = of.m_gameplay;
            var loaded = Task.IsLoaded( gameplay );

            if( loaded )
                return;

            source.ULoadTask( gameplay );
        }

        private static void TryLoadPlatformSpecificTask(this UBehaviour source, SituationData of)
        {
            TaskData[] data;
#if IKIMASHO_PS5
            data = of.m_playstation5SpecificTasks; 
#elif IKIMASHO_META
            data = of.m_metaSpecificTasks;
#elif IKIMASHO_PC || UNITY_EDITOR
            data = of.m_win64SpecificTasks; 
#endif
            
            for (var i = 0; i < data.Length; i++)
            {
                var currentTask = data[i];
                if (currentTask == null || Task.IsLoaded(currentTask)) return;
                
                source.ULoadTask(currentTask);
            }
        }

        private static void TryUnloadBlockMeshTask( this UBehaviour source, SituationData of )
        {
            var blockMesh = of.m_blockMeshEnvironment;

            if( IsStillNeeded( blockMesh ) )
                return;

            source.UUnloadTask( blockMesh );
        }

        private static void TryUnloadArtTask( this UBehaviour source, SituationData of )
        {
            var art = of.m_artEnvironment;

            if( IsStillNeeded( art ) )
                return;

            source.UUnloadTask( art );
        }

        private static void TryUnloadGameplayTask( this UBehaviour source, SituationData of )
        {
            var gameplay = of.m_gameplay;

            if( IsStillNeeded( gameplay ) )
                return;

            source.UUnloadTask( gameplay );
        }

        private static void TryUnloadPlatformSpecificTask(this UBehaviour source, SituationData of)
        {
            TaskData[] data;
#if IKIMASHO_PS5
            data = of.m_playstation5SpecificTasks; 
#elif IKIMASHO_META
            data = of.m_metaSpecificTasks;
#elif IKIMASHO_PC || UNITY_EDITOR
            data = of.m_win64SpecificTasks; 
#endif
            if(data == null) return;

            for (var i = 0; i < data.Length; i++)
            {
                var currentTask = data[i];

                if (IsStillNeeded(currentTask)) return;
                
                source.UUnloadTask(currentTask);
            }
        }

        private static bool IsStillNeeded( TaskData task )
        {
            if (!task) return false;
            
            var loadingCount    = LoadingSituations.Count;
            var loadedCount     = LoadedSituations.Count;

            for( var i = 0; i < loadingCount; i++ )
            {
                var situation          = LoadingSituations[i];
                var blockMesh   = situation.m_blockMeshEnvironment;
                var art         = situation.m_artEnvironment;
                var gameplay    = situation.m_gameplay;

                if(blockMesh && blockMesh.Equals(task))  return true;
                if(art && art.Equals(task))        return true;
                if(gameplay && gameplay.Equals(task))   return true;
                
                TaskData[] data;
#if IKIMASHO_PS5
                data = situation.m_playstation5SpecificTasks; 
#elif IKIMASHO_META
                data = situation.m_metaSpecificTasks;
#elif IKIMASHO_PC || UNITY_EDITOR
                data = situation.m_win64SpecificTasks; 
#endif
                if(data != null)
                {
                    for (var j = 0; j < data.Length; j++)
                    {
                        var currentTask = data[j];
    
                        if (currentTask.Equals(task)) return true;
                    }
                }
            }

            for( var i = 0; i < loadedCount; i++ )
            {
                var situation          = LoadedSituations[i];
                var blockMesh   = situation.m_blockMeshEnvironment;
                var art         = situation.m_artEnvironment;
                var gameplay    = situation.m_gameplay;

                if(blockMesh && blockMesh.Equals(task))  return true;
                if(art && art.Equals(task))        return true;
                if(gameplay && gameplay.Equals(task))   return true;
                
                TaskData[] data;
#if IKIMASHO_PS5
                data = situation.m_playstation5SpecificTasks; 
#elif IKIMASHO_META
                data = situation.m_metaSpecificTasks;
#elif IKIMASHO_PC || UNITY_EDITOR
                data = situation.m_win64SpecificTasks; 
#endif
                if (data != null)
                {
                    for (var j = 0; j < data.Length; j++)
                    {
                        var currentTask = data[j];

                        if (currentTask.Equals(task)) return true;
                    }
                }
            }

            return false;
        }

        private static void OnAnyTaskLoaded( TaskData task ) =>
            RefreshLoadingSituations();

        private static void RefreshLoadingSituations()
        {
            var count = LoadingSituations.Count;

            for( var i = count-1; i >= 0; i-- )
            {
                var situation          = LoadingSituations[i];
                var blockMesh   = situation.m_blockMeshEnvironment;
                var art         = situation.m_artEnvironment;
                var gameplay    = situation.m_gameplay;

                if(CanLoadBlockMesh && !Task.IsLoaded(blockMesh))   continue;
                if(CanLoadArt && !Task.IsLoaded(art))               continue;
                if(!Task.IsLoaded(gameplay))                        continue;

                LoadingSituations.Remove( situation );
                LoadedSituations.Add( situation );
                OnSituationLoaded?.Invoke( situation );
            }

            if( LoadingSituations.Count == 0 )
                Task.OnTaskLoaded -= OnAnyTaskLoaded;
        }

        private static List<SituationData> LoadingSituations =>
            _loadingSituations ??= new();
        private static List<SituationData> LoadedSituations =>
            _loadedSituations ??= new();

        #endregion


        #region Private

        private static Environment _currentEnvironment;

        private static List<SituationData> _loadingSituations;
        private static List<SituationData> _loadedSituations;

        #endregion
    }
}