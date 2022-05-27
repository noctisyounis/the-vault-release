using System.Collections.Generic;
using UnityEngine;

namespace Universe
{
    public class SignalBase : UniverseScriptableObject
    {
        #region Public
        
        public bool m_isFavorite;
        public bool m_isAnalytics;
        public bool m_isAchievement;
        public bool m_useVerboseLog;
        public bool m_triggerOnce;
        
        [HideInInspector]
        public bool m_consumed;
        
        #endregion 
        
        
        #region Unity API

        private void OnEnable()
        {
            if (m_triggerOnce)
            {
                m_consumed = false;
            }
            UniverseManager.Signals?.Add(this);
        }
        
        #endregion
        
        
        #region Main

        public void Emit()
        {
            if ( m_triggerOnce && m_consumed ) return;
            
            for (var i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].OnSignalEmitted();
            }
            
            for (var i = _actions.Count - 1; i >= 0; i--)
            {
                _actions[i]();
            }

            m_consumed = true;
            
            if( m_useVerboseLog ) Debug.Log($"{name} Signal Emitted");
        }
        
        public void AddListener(IGameSignalReceptor receptor)
        {
            if (!_listeners.Contains(receptor))
            {
                _listeners.Add(receptor);
            }
        }
        
        public void RemoveListener(IGameSignalReceptor receptor)
        {
            if (_listeners.Contains(receptor))
            {
                _listeners.Remove(receptor);
            }
        }
        
        public void AddListener(System.Action action)
        {
            if (!_actions.Contains(action))
            {
                _actions.Add(action);
            }
        }
        
        public void RemoveListener(System.Action action)
        {
            if (_actions.Contains(action))
            {
                _actions.Remove(action);
            }
        }
        
        public virtual void RemoveAll()
        {
            _listeners.RemoveRange(0, _listeners.Count);
            _actions.RemoveRange(0, _actions.Count);
        }
        
        #endregion
        
        
        #region Private And Protected
        
        protected List<IGameSignalReceptor> _listeners = new List<IGameSignalReceptor>();
        protected List<System.Action> _actions = new List<System.Action>();

        #endregion
    }
}