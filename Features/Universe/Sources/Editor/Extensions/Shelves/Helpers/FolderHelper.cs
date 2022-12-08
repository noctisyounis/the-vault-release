using static System.IO.Path;
using static UnityEditor.AssetDatabase;

namespace Universe.Toolbar.Editor
{
    static class FolderHelper
	{
		public static string CreatePath(string path)
		{
			var fullPath 			= path.Replace(AltDirectorySeparatorChar, DirectorySeparatorChar);
			var folders 			= fullPath.Split(DirectorySeparatorChar);
			var subPaths 			= GenerateSubPaths(fullPath);
			var parentFolderPath 	= string.Empty;
			var steps 				= subPaths.Length;
			var finalFolderGUID		= "";

			for(var i = 0; i < steps; i++)
			{
				var subPath = subPaths[i];
				var folder 	= folders[i];

				if(!IsValidFolder(subPath) || i == steps - 1)
				{
					finalFolderGUID = CreateFolder(parentFolderPath, folder);
				}

				parentFolderPath = subPath;
			}

			return GUIDToAssetPath(finalFolderGUID);
		}

		private static string[] GenerateSubPaths(string path)
		{
			var subPaths = path.Split(DirectorySeparatorChar);
			var currentPath = string.Empty;
			
			for (var i = 0; i < subPaths.Length; i++)
			{
				currentPath = Join(currentPath, subPaths[i]);
				subPaths[i] = currentPath;
			}

			return subPaths;
		}
	}
}