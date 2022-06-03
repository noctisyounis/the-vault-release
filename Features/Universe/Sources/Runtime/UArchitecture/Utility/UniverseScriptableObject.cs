using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Universe
{
    public abstract class UniverseScriptableObject : ScriptableObject 
    {
        #region Main

        public virtual void SaveAsset()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty( this );
            AssetDatabase.SaveAssetIfDirty( this );
#endif
        }

        #endregion
    }
}