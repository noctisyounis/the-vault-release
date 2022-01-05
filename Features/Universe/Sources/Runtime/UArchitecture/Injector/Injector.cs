using System.Reflection;
using UnityEngine;

namespace Universe
{
    public class Injector : MonoBehaviour
    {
        #region Exposed
        
        [SerializeField]
        private ScriptableObject _sourceObject;
        
        private PropertyInfo _sourceProperty;

        [SerializeField]
        private string _sourcePropertyName;

        [SerializeField, Space(10)]
        private Object _targetObject;

        private PropertyInfo _targetProperty;

        [SerializeField]
        private string _targetPropertyName;
        
        #endregion


        #region Properties

        public PropertyInfo TargetProperty
        {
            get => _targetProperty;

            set
            {
                _targetProperty = value;
            }
        }

        public PropertyInfo SourceProperty
        {
            get => _sourceProperty;

            set => _sourceProperty = value;
        }

        #endregion


        #region Unity API

        private void Update() 
        {
            TrySetTargetValueFromSource();
        }

        #endregion


        #region Main

        [ContextMenu("UpdateTargetValue")]
        public void TrySetTargetValueFromSource()
        {
            if(CanSetTargetValue())
            {
                _targetProperty.SetValue(_targetObject, _sourceProperty.GetValue(_sourceObject)); 
            }
        }

        #endregion


        #region Utils

        public bool CanSetTargetValue()
        {
            return _targetProperty != null && _sourceProperty != null && _targetProperty.PropertyType.Equals(_sourceProperty.PropertyType);
        }

        #endregion
    }
}