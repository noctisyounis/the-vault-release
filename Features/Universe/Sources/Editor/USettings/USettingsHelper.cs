using System.IO;
using UnityEngine;
using Universe.Toolbar.Editor;

using static UnityEditor.AssetDatabase;
using static System.IO.Path;

namespace Universe
{
	public static class USettings
	{
		#region Main

		public static T GetSettings<T>() where T : ScriptableObject
		{
			var path 		= Join(_settingsFolder, $"{typeof(T).Name}.asset");
			var fullPath 	= GetFullPath(path);

			if(!IsValidFolder(_settingsFolder)) FolderHelper.CreatePath(_settingsFolder);

			var possibleSettingGUID = FindAssets($"t:{typeof(T)}");
			
			if(possibleSettingGUID.Length > 0)
			{
				var settingPath = GUIDToAssetPath(possibleSettingGUID[0]);
				return LoadAssetAtPath<T>(settingPath);
			}
			
			var settings = ScriptableObject.CreateInstance<T>();
			
			CreateAsset(settings, path);
			SaveAssets();
			return settings;
		}	

		#endregion


		#region Private

		private static string _settingsFolder = "Assets/Settings/Universe";

		#endregion
	}
}