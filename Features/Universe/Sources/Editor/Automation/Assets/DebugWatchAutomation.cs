using UnityEditor;
using Universe.DebugWatchTools.Runtime;
namespace Universe
{
    public class DebugWatchAutomation : AssetPostprocessor
    {
        #region Unity API
		
        /// <summary> Add levels to the debug watch.</summary>
        /// <param name="importedAssets"></param>
        /// <param name="deletedAssets"></param>
        /// <param name="movedAssets"></param>
        /// <param name="movedFromAssetPaths"></param>
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            LevelManagement.BakeLevelDebug();
        }
		
        #endregion
    }
}