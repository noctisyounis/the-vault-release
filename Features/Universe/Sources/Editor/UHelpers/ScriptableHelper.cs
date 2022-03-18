using System.IO;
using UnityEngine;
using Universe.Toolbar.Editor;

using static UnityEditor.AssetDatabase;
using static System.IO.Path;

namespace Universe.Editor
{
	public static class ScriptableHelper
	{
		#region Main

		public static T GetScriptable<T>(string targetFolder = DEFAULT_FOLDER) where T : ScriptableObject
		{
			var path 		= Join(targetFolder, $"{typeof(T).Name}.asset");
			var fullPath 	= GetFullPath(path).Replace(DirectorySeparatorChar, AltDirectorySeparatorChar);

			if(!IsValidFolder(targetFolder)) FolderHelper.CreatePath(targetFolder);

			var possibleAssetGUIDs = FindAssets($"t:{typeof(T)}");

			if(possibleAssetGUIDs.Length > 0)
			{
				var settingPath = GUIDToAssetPath(possibleAssetGUIDs[0]);

				return LoadAssetAtPath<T>(settingPath);
			}
			
			var settings = ScriptableObject.CreateInstance<T>();
			
			CreateAsset(settings, path);
			SaveAssets();
			return settings;
		}

		#endregion


		#region Private

		private const string DEFAULT_FOLDER = "Assets/_/Content/Database";

		#endregion
	}
}