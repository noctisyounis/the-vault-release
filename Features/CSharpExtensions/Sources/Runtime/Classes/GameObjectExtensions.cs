using UnityEngine;

namespace CSharpExtentions
{
    public static class GameObjectExtensions
    {
        public static string GetScenePath( this GameObject obj )
        {
            var path = " > " + obj.name;
            while ( obj.transform.parent != null )
            {
                obj = obj.transform.parent.gameObject;
                path = " > " + obj.name + path;
            }
            return path;
        }
    }
}