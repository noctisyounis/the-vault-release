using UnityEditor;
using Universe.Editor;
using Universe.DebugWatch.Runtime;

namespace Universe.DebugWatch.Editor
{
    public static class DebugWatchDictionary
    {
        #region Main

        [MenuItem("Vault/Debug Watch/Bake Methods")]
        public static void TryValidate()
        {
            var bakeTarget = ScriptableHelper.GetScriptable<DebugMenuDatabase>();

            DebugMenuRegistry.s_bakedDatabase = bakeTarget;
            DebugMenuRegistry.InitializeMethods();
            
            bakeTarget.OnValidate();

            EditorUtility.SetDirty( bakeTarget );
            AssetDatabase.SaveAssetIfDirty( bakeTarget );
        }

        #endregion
    }
}