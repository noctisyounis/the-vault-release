using UnityEngine;

using static UnityEngine.Debug;

namespace Universe
{
    public abstract class UTextGeneric<T> : UText
    {
        [HideInInspector]
        public T m_component;

        protected override void GetTextComponent()
        {
            if( IsComponentNotNull() ) return;
            TryGetComponent( out m_component );
        }

        protected bool IsComponentOrFontSettingsNull()
        {
            var isNull = IsComponentNull() || IsFontSettingsNull();
            if (isNull) LogError( "ERROR Component or font settings is null" );

            return isNull;
        }

        protected bool IsComponentNull() => m_component == null;
        protected bool IsComponentNotNull() => m_component != null;
    }
}