using UnityEngine;

namespace Universe
{
    //Can't get property drawer to work with generic arguments
    public abstract class ReferenceBase { }
    
   [System.Serializable]
    public class ReferenceBase<TBase, TVariable> : ReferenceBase where TVariable : FactGeneric<TBase>
    {
        #region Exposed
        
        [SerializeField]
        protected bool _useConstant = false;
        [SerializeField]
        protected TBase _constantValue = default(TBase);
        [SerializeField]
        protected TVariable _fact = default(TVariable);

        #endregion
        
        
        #region Constructor
        
        public ReferenceBase() { }
        
        public ReferenceBase(TBase baseValue)
        {
            _useConstant = true;
            _constantValue = baseValue;
        }

        
        #endregion

        
        #region Main
        
        public TBase Get()
        {
            return (_useConstant || _fact == null) ? _constantValue : _fact.Get();
        }

        public void Set( TBase value )
        {
            if (!_useConstant && _fact != null)
            {
                _fact.Set(value);
            }
            else
            {
                _useConstant = true;
                _constantValue = value;
            }
        }
        
        #endregion
        
        
        #region Utilities
        
        public ReferenceBase CreateCopy()
        {
            var copy = (ReferenceBase<TBase, TVariable>)System.Activator.CreateInstance(GetType());
            copy._useConstant = _useConstant;
            copy._constantValue = _constantValue;
            copy._fact = _fact;

            return copy;
        }
        
        public override string ToString()
        {
            return this.Get().ToString();
        }
        
        #endregion
    }
}