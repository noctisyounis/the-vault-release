using UnityEngine;

namespace Universe.SceneTask.Runtime
{
    public enum UnityMessage
    {
        Script,
        Awake,
        Start,
        OnEnable,
    }
    
    public class TaskLoader : UBehaviour
    {
        #region Public Members
        
        [Header("Settings")]
        public UnityMessage m_loadTaskOn; 
        public TaskData m_taskData;

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

        private void OnEnable()
        {
            if (!IsOnEnable()) return;
            
            Load();
        }
    
        #endregion
        
        
        #region Main
        
        public void Load()
        {
            Verbose($"{m_taskData.name} Loaded");
            LoadTask(m_taskData);
        }
        
        #endregion
        
        
        #region Utils

        private bool IsAwake() => m_loadTaskOn == UnityMessage.Awake;
        
        private bool IsStart() => m_loadTaskOn == UnityMessage.Start;
        
        private bool IsOnEnable() => m_loadTaskOn == UnityMessage.OnEnable;

        #endregion
    }
}