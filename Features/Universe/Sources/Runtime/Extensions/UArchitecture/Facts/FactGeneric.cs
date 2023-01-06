using System;
using UnityEngine;

namespace Universe
{
    public class FactGeneric<T> : FactBase
    {
        #region Main
        
        public T Get()
        {
            return _value;
        }
        
        public T Set(T value)
        {
            if ( m_isReadOnly )
            {
                Debug.LogWarning( $"You Tried To Change: \"{name}\" but it's Read Only, the value: \"{value}\" will not be applied!" );
                return _value;
            }

            if ( m_useVerboseOnChange )
            {
                Debug.Log( $"Fact: \"{name}\" was changed to: \"{value}\"!" );
            }
            
            _value = value;
            OnValueChanged?.Invoke(this);
            
            return _value;
        }
        
        #endregion
        
        
        #region Unity API

        private void OnEnable()
        {
            if ( m_washOnAwakeAndCompilation )
            {
                _value = _defaultValue;
            }
            
            UniverseManager.Facts.Add(this);
        }

        private void OnDisable()
        {
            UniverseManager.Facts.Remove(this); 
        }
        
        #endregion
        
        
        #region Utilities

        public override string ToString()
        {
            return Value == null ? "null" : _value.ToString();
        }

        public static implicit operator T( FactGeneric<T> fact )
        {
            return fact.Value;
        }

        #endregion


        #region Private

        public override Type Type => typeof(T);

        public T Value
        {
            get => _value;
            set => _value = Set(value);
        }

        public T DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        [SerializeField]
        private T _value = default(T);
        [SerializeField]
        protected T _defaultValue = default(T);
        
        #endregion
    }
}