using UnityEngine;

namespace CSharpExtentions
{
    public static class ObjectExtensions
    {
        public static void RequiredIn<T>( this T obj, Object context ) where T : Object
        {
            if (obj == null)
            {
                Debug.LogError( $"<color=yellow>MISSING</color> - <color=blue>{typeof(T)}</color> - on {context}!", context );
            }
        }
    }
}