using UnityEditor;
using Universe.DebugWatchTools.Runtime;
using Universe.Editor;

namespace Universe
{
	public class UAssetPostProcessor : AssetPostprocessor
	{
		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
			string[] movedFromAssetPaths)
		{
			if (deletedAssets.Length == 0) return;
			UGroupHelper.RefreshAaGroups();
			LevelManagement.BakeLevelDebug();
		}
	}
}