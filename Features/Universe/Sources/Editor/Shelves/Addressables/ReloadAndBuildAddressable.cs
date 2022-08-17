using UnityEngine;
using Universe.DebugWatch.Editor;
using Universe.DebugWatchTools.Runtime;

using static UnityEditor.AddressableAssets.Settings.AddressableAssetSettings;
using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUILayout;
using static Universe.Editor.UGroupHelper;

namespace Universe.Editor
{
	public class ReloadAndBuildAddressable
	{
		#region Main
		
		public static void Draw()
        {
            var tex = IconContent(_iconContext).image;
            if( !Button( new GUIContent( _buttonText, tex, _buttonTooltip ) ) ) return;

            Execute();
        }

        public static void Execute()
        {
	        RefreshAaGroups();
			LevelManagement.BakeLevelDebug();
            DebugWatchDictionary.TryValidate();
            OnRefreshCompleted += RebuildAddressable;
            RefreshAaGroups();
        }

        #endregion


        #region Utils

        public static void RebuildAddressable()
		{
			BuildPlayerContent();
			OnRefreshCompleted -= RebuildAddressable;
		}
		
		#endregion
		
		
		#region Private

		private static string _buttonText = "Rebuild";
		private static string _buttonTooltip = "Refresh addressable then rebuild them";
		private static string _iconContext = @"d_Profiler.NetworkOperations";

		#endregion
	}
}