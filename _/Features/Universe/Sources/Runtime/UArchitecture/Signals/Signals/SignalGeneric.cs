using System.Collections.Generic;
using UnityEngine;

namespace Universe
{
    public class SignalGeneric<T> : SignalBase
    {
        #region Exposed
        
        [SerializeField]
        protected T _debugValue = default(T);
        
        #endregion
        
        
        #region Main
        
        public void Emit(T value)
        {
            for (var i = _typedListeners.Count - 1; i >= 0; i--)
            {
                _typedListeners[i].OnSignalEmitted(value);
            }

            for (var i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].OnSignalEmitted();
            }
            
            for (var i = _typedActions.Count - 1; i >= 0; i--)
            {
                _typedActions[i](value);
            }
            
            for (var i = _actions.Count - 1; i >= 0; i--)
            {
                _actions[i]();
            }
            
            if( m_useVerboseLog ) Debug.Log($"[USignal] {name} Emited with parameter at {value}");
        }
        
        public void AddListener(IGameSignalReceptor<T> receptor)
        {
            if (!_typedListeners.Contains(receptor))
            {
                _typedListeners.Add(receptor);
            }
        }
        
        public void AddListener(System.Action<T> action)
        {
            if (!_typedActions.Contains(action))
            {
                _typedActions.Add(action);
            }
        }
        
        public void RemoveListener(IGameSignalReceptor<T> receptor)
        {
            if (_typedListeners.Contains(receptor))
            {
                _typedListeners.Remove(receptor);
            }
        }
        
        public void RemoveListener(System.Action<T> action)
        {
            if (_typedActions.Contains(action))
            {
                _typedActions.Remove(action);
            }
        }
        
        #endregion
        
        
        #region Utilities
        
        public override string ToString()
        {
            return "GameSignalBase<" + typeof(T) + ">";
        }
        
        #endregion
        
        
        #region Private And Protected
        
        private readonly List<IGameSignalReceptor<T>> _typedListeners = new List<IGameSignalReceptor<T>>();
        private readonly List<System.Action<T>> _typedActions = new List<System.Action<T>>();

        #endregion
    }
}