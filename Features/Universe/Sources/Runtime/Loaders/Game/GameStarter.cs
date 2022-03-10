using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

using static UnityEditor.AssetDatabase;

#endif

namespace Universe.SceneTask.Runtime
{
    public class GameStarter : UBehaviour
    {
        #region Public Members
        
        [Header("Settings")]
        public UnityMessage m_loadLevelOn; 
        public LevelData m_levelData;
        
        #endregion
        
        
        #region Unity API
        
        public override void Awake()
        {
#if UNITY_EDITOR
            Load();
            return;
#endif
#pragma warning disable 0162
            if (!IsAwake()) return;

            Load();
#pragma warning restore 0162
        }

        private void Start()
        {
            if (!IsStart()) return;
            
            Load();
        }

        private void OnEnable()
        {
            if (!IsOnEnable()) return;
            
            Load();
        }
        
        #endregion
        
        
        #region Main
        
        public void Load()
        {
            
#if UNITY_EDITOR

            var startLevelPath = PlayerPrefs.GetString(_startLevelPlayerPref);
            var startLevelData = LoadAssetAtPath<LevelData>(startLevelPath);

            LoadLevelAbsolute(startLevelData);

            return;
#else
            Verbose($"{m_levelData.name} Loaded");
            LoadLevelAbsolute(m_levelData);
#endif
        }
        
        #endregion
        
        
        #region Utils
        
        private bool IsAwake() => m_loadLevelOn == UnityMessage.Awake;
        
        private bool IsStart() => m_loadLevelOn == UnityMessage.Start;
        
        private bool IsOnEnable() => m_loadLevelOn == UnityMessage.OnEnable;
        
        #endregion


        #region Private Members

        private static string _startLevelPlayerPref = "PlaymodeLevelPath";

        #endregion
    }
}