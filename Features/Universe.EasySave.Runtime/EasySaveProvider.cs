using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static System.Enum;
using static ES3;
using static ES3.Location;
using static Universe.USaveLevel;

namespace Universe.EasySave.Runtime
{
    using static GUILayout;

    public class EasySaveProvider : ISaveProvider
    {
        #region Public

        public string m_saveFile = "SaveData.save";
        public uint m_numberOfDataByFrame = 1;

        #endregion


        #region Main

        public override void Load( USaveLevel saveLevel )
        {
            if( SaveDoesntExist() ) return;
            
            var levelFacts = GetAllFactsInCurrentSaveLevel( saveLevel );
            
            Verbose( "Load level  = ", saveLevel );
            CacheSaveFile();
            InitializeCompletionPercentage( GetCountSaveLevelFacts( levelFacts ) );
            LoadListFacts( levelFacts );
        }

        public override void Load( FactBase fact )
        {
            if( SaveDoesntExist() ) return;

            var levelFacts = new List<FactBase>();
            
            Verbose( "Load fact  = ", fact );
            levelFacts.Add( fact );
            CacheSaveFile();
            InitializeCompletionPercentage( GetCountSaveLevelFacts( levelFacts ) );
            LoadListFacts( levelFacts );
        }

        public override void Load( List<FactBase> levelFacts )
        {
            if( SaveDoesntExist() ) return;
            
            Verbose( "Load facts list  = ", levelFacts );
            CacheSaveFile();
            InitializeCompletionPercentage( GetCountSaveLevelFacts( levelFacts ) );
            LoadListFacts( levelFacts );
        }

        public override void LoadAll()
        {
            if( SaveDoesntExist() ) return;
            
            Verbose( "Load All" );
            CacheSaveFile();
            ParseAllSaveLevelSetter();
            InitializeCompletionPercentage( GetCountAllSaveLevelFactsCount );

            LoadAllSaveLevels();
        }

        public override void Save( List<FactBase> facts )
        {
            Verbose( "Save facts list  = ", facts );

            InitializeCompletionPercentage( GetCountSaveLevelFacts( facts ) );
            SaveListFacts( facts );
        }

        public override void Save( FactBase fact )
        {
            Verbose( "Save fact  = ", fact );

            var listfact = new List<FactBase>();
            listfact.Add( fact );

            InitializeCompletionPercentage( GetCountSaveLevelFacts( listfact ) );
            SaveListFacts( listfact );
        }

        public override void Save( USaveLevel saveLevel )
        {
            Verbose( "Save level  = ", saveLevel );
            var levelFacts = GetAllFactsInCurrentSaveLevel( saveLevel );

            InitializeCompletionPercentage( GetCountSaveLevelFacts( levelFacts ) );
            SaveListFacts( levelFacts );
        }

        public override void SaveAll()
        {
            Verbose( "Save All" );
            ParseAllSaveLevelSetter();
            InitializeCompletionPercentage( GetCountAllSaveLevelFactsCount );

            SaveAllSaveLevels();
        }

        #endregion


        #region Unity API

        public void Start()
        {
            USaver.m_normalizedPercentageOfCompletion = m_normalizedPercentageOfCompletion;
            USaver.OnLoadFinish = OnLoadFinish;
            USaver.OnSaveFinish = OnSaveFinish;
        }

        #endregion


        #region Utils

        private void SaveAllSaveLevels()
        {
            LoadOrSaveAllSaveLevels( true );
        }

        private void LoadAllSaveLevels()
        {
            LoadOrSaveAllSaveLevels( false );
        }

        private void LoadOrSaveAllSaveLevels( bool save )
        {
            var listFacts = GetAllSavableFacts();

            if( save ) SaveListFacts( listFacts );
            else LoadListFacts( listFacts );
        }

        private void SaveListFacts( List<FactBase> levelFacts )
        {
            var settings = GetEasySaveCache();
            Verbose( $"Level Facts = {levelFacts}" );

            SaveFacts( settings, levelFacts );
        }

        private void LoadListFacts( List<FactBase> levelFacts )
        {
            var settings = GetEasySaveCache();

            LoadFacts( settings, levelFacts );
        }

        private void InitializeCompletionPercentage( int count )
        {
            _completionPercentage.InitializeCompletionPercentage( count );
        }

        private void ParseAllSaveLevelSetter()
        {
            _parseAllSaveLevel = true;
        }

        private void DontParseAllSaveLevels()
        {
            _parseAllSaveLevel = false;
        }

        #endregion


        #region USaver API

        private void IncreaseCompletionPercentage()
        {
            _completionPercentage.IncreasePercentage();
            USaver.m_normalizedPercentageOfCompletion.Value = _completionPercentage.GetNormalizedPercentage();

            Verbose( $"percentage = {USaver.m_normalizedPercentageOfCompletion.Value}" );
        }

        private static void EmitLoadFinishSignal()
        {
            USaver.OnLoadFinish.Emit();
        }

        private static void EmitSaveFinishSignal()
        {
            USaver.OnSaveFinish.Emit();
        }

        #endregion


        #region Easy Save API

        private static void StoreEasySaveCacheFile()
        {
            StoreCachedFile();
        }

        private void CacheSaveFile()
        {
            CacheFile( m_saveFile );
        }

        private void SaveFacts( ES3Settings settings, List<FactBase> levelFacts )
        {
            this.StartRoutine( m_numberOfDataByFrame, levelFacts, SaveOneData ).OnComplete += OnSaveFinished;
        }

        private void SaveOneData( object obj )
        {
            var entry = (FactBase)obj;
            var settings = GetEasySaveCache();
            
            Verbose( $"EasySaveProvider.SaveOneData entry = {entry.name} type = {entry.Type}" );
            ES3.Save( entry.name, entry, settings );
            IncreaseCompletionPercentage();
        }

        private void OnSaveFinished()
        {
            Verbose( "OnSaveFinished" );
            DontParseAllSaveLevels();
            EmitSaveFinishSignal();
            StoreEasySaveCacheFile();
        }

        private void LoadFacts( ES3Settings settings, List<FactBase> levelFacts )
        {
            this.StartRoutine( m_numberOfDataByFrame, levelFacts, LoadOneData ).OnComplete += OnLoadFinished;
        }

        private void LoadOneData( object obj )
        {
            var entry = (FactBase)obj;
            var settings = GetEasySaveCache();

            if( DoesntEntryExist( entry ) ) return;

            LoadInto( entry.name, entry, settings );
            IncreaseCompletionPercentage();
        }

        private void OnLoadFinished()
        {
            Verbose( "OnLoadFinished" );
            EmitLoadFinishSignal();
            DontParseAllSaveLevels();
        }

        private bool CheckIfSaveFileExists()
        {
            var locationSet = new ES3Settings( File );
            return FileExists( locationSet );
        }

        #endregion


        #region Private

        private bool IsParsingOneSaveLevel() => !_parseAllSaveLevel;
        private static bool IsSavable( USaveLevel saveLevel ) => saveLevel != None;
        private ES3Settings GetEasySaveCache() => new ES3Settings( Location.Cache );
        private List<FactBase> GetAllFactsInCurrentSaveLevel( USaveLevel saveLevel )
            => m_factList.FindAll( x => x.m_saveLevel == saveLevel );
        private List<FactBase> GetAllSavableFacts()
            => m_factList.FindAll( x => x.m_saveLevel != None );

        private int GetCountAllSaveLevelFactsCount => m_factList.Count( x => x.m_saveLevel != None );
        private int GetCountSaveLevelFacts( USaveLevel saveLevel ) => GetAllFactsInCurrentSaveLevel( saveLevel ).Count;
        private int GetCountSaveLevelFacts( List<FactBase> levelFacts ) => levelFacts.Count;

        private static bool DoesntEntryExist( FactBase entry ) => !KeyExists( entry.name );
        private static Array GetAllSaveLevels => GetValues( typeof( USaveLevel ) );

        private bool SaveDoesntExist() => !CheckIfSaveFileExists();

        private bool _parseAllSaveLevel;

        private UPercentage _completionPercentage = new UPercentage();

        #endregion
    }
}