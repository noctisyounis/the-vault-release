using UnityEngine;
using UnityEditor;
using Universe.Editor;
using Universe.DebugWatch.Runtime;

namespace Universe.DebugWatch.Editor
{
    public static class DebugDictionaryValidation
    {
        #region Main

        [MenuItem("Vault/Debug Watch/Bake Methods"), UnityEditor.Callbacks.DidReloadScripts]
        public static void TryValidate()
        {
            var bakeTarget = ScriptableHelper.GetScriptable<DebugMenuData>();

            DebugMenuRegistry.s_bakedData = bakeTarget;
            DebugMenuRegistry.InitializeMethods();
            
            bakeTarget.OnValidate();
        }

        #endregion
    }
}