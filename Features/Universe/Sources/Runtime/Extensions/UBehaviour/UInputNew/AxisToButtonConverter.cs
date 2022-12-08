using System;
using UnityEngine;

namespace Universe
{
    [Serializable]
    public class AxisToButtonConverter
    {
        #region Public Members

        public bool m_useAbsoluteValue;
        public float m_triggerThreshold;
        public bool m_triggerUnderThreshold;

        #endregion


        #region Public Properties

        public bool IsPressed => _pressed;

        #endregion


        #region Events

        public Action<float> OnAxisPressed;
        public Action<float> OnAxisReleased;

        #endregion


        #region Constructor

        public AxisToButtonConverter() : this( 0.5f ) { }      

        public AxisToButtonConverter( float threshold )
        {
            m_triggerThreshold = threshold;
            _pressed = false;
        }

        #endregion


        #region Public API

        public void Evaluate( float next )
        {
            if (m_triggerUnderThreshold) EvaluateUnder(next);
            else EvaluateAbove(next);
        }

        #endregion
        
        
        #region Utils

        private void EvaluateAbove(float next)
        {
            var value = m_useAbsoluteValue ? Mathf.Abs(next) : next;
            
            if( value < m_triggerThreshold && IsPressed )
            {
                _pressed = false;
                OnAxisReleased?.Invoke( next );
            }

            if( value >= m_triggerThreshold && !IsPressed )
            {
                _pressed = true;
                OnAxisPressed?.Invoke( next );
            }
        }

        private void EvaluateUnder(float next)
        {
            var value = m_useAbsoluteValue ? Mathf.Abs(next) : next;
            
            if( value > m_triggerThreshold && IsPressed )
            {
                _pressed = false;
                OnAxisReleased?.Invoke( next );
            }

            if( value <= m_triggerThreshold && !IsPressed )
            {
                _pressed = true;
                OnAxisPressed?.Invoke( next );
            }
        }
        
        #endregion


        #region Private Members

        private bool _pressed;

        #endregion
    }
}