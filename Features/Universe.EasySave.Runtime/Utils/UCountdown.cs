using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Universe
{
    
    public class UCountdown : UBehaviour
    {
        #region Public

        public float m_timeBeforeEvent;
        public FloatFact m_timeBeforeEventVariable;
        public bool m_isRepeated;
        [EnumToggleButtons]
        public TimeMode m_timeMode;
        public UnityEvent OnCountdownReached;


        public enum TimeMode { Normal, Unscaled };

        [Header( "Internal" ), Space( 10 )]
        [SerializeField]
        public bool m_isConsumed;

        #endregion


        #region System

        private void Awake()
        {
            if( enabled )
                Initialize();
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void OnDisable()
        {
            _started = false;

        }

        private void Update()
        {
            if (!_started) return;
            
            float timeBeforeEvent = m_timeBeforeEvent;
            if( m_timeBeforeEventVariable != null )
                timeBeforeEvent = m_timeBeforeEventVariable.Value;

            if( m_timeMode == TimeMode.Normal )
            {
                if( Time.time > _startTime + timeBeforeEvent && !m_isConsumed )
                {
                    Verbose( "Countdown.Update m_isRepeated = " + m_isRepeated + ", current = " + Time.time + ", Invoke event" );
                    OnCountdownReached.Invoke();
                    if( m_isRepeated )
                    {
                        Initialize();
                        return;
                    }
                    m_isConsumed = true;
                }
            }
            else if( m_timeMode == TimeMode.Unscaled )
            {
                if( Time.unscaledTime > _startTime + timeBeforeEvent && !m_isConsumed )
                {
                    Verbose( "Countdown.Update m_isRepeated = " + m_isRepeated + ", current = " + Time.unscaledTime + ", Invoke event" );
                    OnCountdownReached.Invoke();
                    if( m_isRepeated )
                    {
                        Initialize();
                        return;
                    }
                    m_isConsumed = true;
                }
            }
        }

        #endregion


        #region Utils

        private void Initialize()
        {
            _started = true;
            if( m_timeMode == TimeMode.Normal )
            {
                _startTime = Time.time;
            }
            else if( m_timeMode == TimeMode.Unscaled )
            {
                _startTime = Time.unscaledTime;
            }
            Verbose( "Countdown.Initialize _startTIme = " + _startTime );
        }

        #endregion


        #region Private

        private float _startTime;
        private bool _started = false;

        #endregion
    }
}