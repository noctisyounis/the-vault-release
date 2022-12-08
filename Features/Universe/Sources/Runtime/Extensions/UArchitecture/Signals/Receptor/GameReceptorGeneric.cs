using UnityEngine;
using UnityEngine.Events;

namespace Universe
{
    public abstract class GameReceptorGeneric<TType, TEvent, TResponse> : UniverseMonobehaviour, IGameSignalReceptor<TType> 
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

        public void OnSignalEmitted(TType value)
        {
            _response.Invoke(value);
        }
        
        #endregion
        
        
        #region Unity API
        
        private void OnEnable()
        {
            if (m_signal != null)
            {
                m_signal.AddListener(this);
            }
        }
        
        
        private void OnDisable()
        {
            if (m_signal != null)
            {
                m_signal.RemoveListener(this);
            }
        }
        
        #endregion
        
        
        #region Private
        
        [SerializeField]
        private TResponse _response = default(TResponse);
        
        #endregion
    }
}