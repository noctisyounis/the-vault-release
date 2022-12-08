using UnityEditor;
using Universe.DebugWatchTools.Runtime;
using Universe.Editor;

namespace Universe
{
	public class AddressableAutomation : AssetPostprocessor
	{
		#region Unity API
		
		/// <summary> Refresh addressable groups when post process assets is called.</summary>
		/// <param name="importedAssets"></param>
		/// <param name="deletedAssets"></param>
		/// <param name="movedAssets"></param>
		/// <param name="movedFromAssetPaths"></param>
		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			if (deletedAssets.Length == 0) return;
			
			UGroupHelper.RefreshAaGroups();
		}
		
		#endregion
	}
}