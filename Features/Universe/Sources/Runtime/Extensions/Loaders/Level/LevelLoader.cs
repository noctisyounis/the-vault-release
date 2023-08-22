using UnityEngine;

namespace Universe.SceneTask.Runtime
{
    public class LevelLoader : UBehaviour
    {
        #region Public Members
        
        [Header("Settings")]
        public UnityMessage m_loadTaskOn; 
        public LevelData m_levelData;
        
        #endregion
        
        
        #region Unity API
        
        public override void Awake()
        {
            if (!IsAwake()) return;

            Load();
        }

        private void Start()
        {
            if (!IsStart()) return;
            
            Load();
        }

        public override void OnEnable()
        {
            if (!IsOnEnable()) return;
            
            Load();
        }
        
        #endregion
        
        
        #region Main
        
        public void Load()
        {
            ChangeLevel(m_levelData);
            Verbose($"{m_levelData.name} Loaded");
        }
        
        #endregion
        
        
        #region Utils
        
        private bool IsAwake() => m_loadTaskOn == UnityMessage.Awake;
        
        private bool IsStart() => m_loadTaskOn == UnityMessage.Start;
        
        private bool IsOnEnable() => m_loadTaskOn == UnityMessage.OnEnable;
        
        #endregion
    }
}