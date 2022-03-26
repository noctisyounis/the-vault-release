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
            var bakeTarget = ScriptableHelper.GetScriptable<DebugMenuData>();

            DebugMenuRegistry.s_bakedData = bakeTarget;
            DebugMenuRegistry.InitializeMethods();
            
            bakeTarget.OnValidate();
        }

        #endregion
    }
}