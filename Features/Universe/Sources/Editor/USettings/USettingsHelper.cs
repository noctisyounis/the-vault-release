using UnityEngine;

namespace Universe.Editor
{
	public static class USettingsHelper
	{
		#region Main

		public static T GetSettings<T>() where T : UniverseScriptableObject
		{
			return ScriptableHelper.GetScriptable<T>(_settingsFolder);
		}	

		#endregion


		#region Private

		private static string _settingsFolder = "Assets/_/Content/Database/Editor/Settings/Universe";

		#endregion
	}
}