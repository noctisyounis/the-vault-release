using UnityEngine;
using UnityEngine.Events;

namespace Universe
{
    public abstract class ReceptorBase<TEvent, TResponse> : UniverseMonobehaviour, IGameSignalReceptor
        where TEvent : SignalBase
        where TResponse : UnityEvent
    {
        #region Public
        
        public TEvent m_signal = default(TEvent);   
        
        #endregion
        
        
        #region Main
        
        public void OnSignalEmitted() => _response.Invoke();

        #endregion
        
        
        #region Unity API
        
        private void OnEnable()
        {
            if (m_signal == null) return;
            
            m_signal.AddListener(this);
        }
        
        private void OnDisable()
        {
            if (m_signal == null) return;
            
            m_signal.RemoveListener(this);
        }
        
        #endregion
        
        
        #region Private And Protected
        
        [SerializeField]
        private TResponse _response = default(TResponse);
        
        #endregion
    }
    
    public abstract class ReceptorBase<TType, TEvent, TResponse> : UniverseMonobehaviour, IGameSignalReceptor<TType>
        where TEvent : SignalGeneric<TType>
        where TResponse : UnityEvent<TType>
    {
        #region Public
        
        public TEvent m_signal = default(TEvent);
        
        #endregion
     
        
        #region Main

        public void OnEventRaised(TType value)
        {
            throw new System.NotImplementedException();
        }

        public void OnSignalEmitted(TType value) => _response.Invoke(value);

        #endregion
        

        #region Unity API
        private void OnEnable()
        {
            if (m_signal == null) return;
            
            Register();
        }
        private void OnDisable()
        {
            if (m_signal == null) return;
            
            m_signal.RemoveListener(this);
        }
        
        #endregion
        
        
        #region Utilities
        
        private void Register() => m_signal.AddListener(this);

        #endregion
        
        
        #region Private And Protected

        [SerializeField]
        protected TType _debugValue = default(TType);
        
        [SerializeField]
        private TResponse _response = default(TResponse);

        #endregion
    }
}