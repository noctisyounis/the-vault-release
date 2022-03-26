using UnityEngine;

namespace Universe.Editor
{
	public static class USettings
	{
		#region Main

		public static T GetSettings<T>() where T : ScriptableObject
		{
			return ScriptableHelper.GetScriptable<T>(_settingsFolder);
		}	

		#endregion


		#region Private

		private static string _settingsFolder = "Assets/_/Content/Database/Editor/Settings/Universe";

		#endregion
	}
}