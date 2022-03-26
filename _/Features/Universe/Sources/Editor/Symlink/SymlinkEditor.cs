using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEditor;

using static System.Diagnostics.Process;
using static System.IO.File;
using static System.IO.FileAttributes;
using static System.IO.Path;
using static System.StringSplitOptions;
using static Symlink.Editor.SymllinkGUIStyle;
using static UnityEditor.AssetDatabase;
using static UnityEditor.EditorApplication;
using static UnityEditor.EditorUtility;
using static UnityEditor.ImportAssetOptions;
using static UnityEditor.Selection;
using static UnityEngine.Application;
using static UnityEngine.Debug;
using static UnityEngine.GUI;
using Directory = System.IO.Directory;


namespace Symlink.Editor
{
   [InitializeOnLoad]
    public static class SymlinkEditor
    {
        #region Constructor
         
        static SymlinkEditor()
        {
            projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }
        
        #endregion
        
      
        #region Main

        public static void Symlink()
        {
            var sourceFolderPath = GetSourceFolderPath( OpenFolderPanel("Select Folder Source", "", "") );
            if (sourceFolderPath == null) return;
            var targetPath = GetTargetFolderPath(sourceFolderPath);

            LoadSymlink(sourceFolderPath, targetPath);
        }

        public static void LoadSymlink(string sourcePath, string targetPath)
        {
            using (var cmd = Start(_commandLineExecutableName, $"/C mklink /J \"{targetPath}\" \"{sourcePath}\""))
            {
                cmd?.WaitForExit();
            }
            Refresh(ForceUpdate);
        }
        
        public static void LoadAllSymlink(string[] directories)
        {
            foreach (var s in directories)
            {
                var name = GetFileName(s);
                LoadSymlink(s, $"{dataPath}\\_\\{name}");
            }
        }

        public static void RemoveSymlink(string folderPath)
        {
            if (!Directory.Exists(folderPath)) return;

            Directory.Delete( folderPath );
            Delete ($"{folderPath}.meta");
                
            Refresh(ForceUpdate);
        }
        
        public static void RemoveAllSymlinks([NotNull] string[] directories)
        {
            if (directories == null)
            {
                throw new ArgumentNullException(nameof(directories));
            }

            foreach (var s in directories)
            {
                var name = GetFileName(s);
                var folderPath = $"{dataPath}\\_\\{name}";
                
               RemoveSymlink(folderPath);
            }
        }
        
        #endregion
        
        
        #region Utils

        [CanBeNull]
        public static string GetSourceFolderPath(string sourceFolderPath)
        {
            if (string.IsNullOrEmpty(sourceFolderPath)) // Cancelled dialog
                return null;

            if (sourceFolderPath.Contains(dataPath))
            {
                LogWarning("Cannot create a symlink to folder in your project!");
                return null;
            }

            var sourceFolderName = sourceFolderPath.Split(new char[] { '/', '\\' }).LastOrDefault();

            if (!string.IsNullOrEmpty(sourceFolderName)) return sourceFolderPath;
            LogWarning("Couldn't deduce the folder name?");
            
            return null;
        }

        [CanBeNull]
        public static string GetTargetFolderPath(string sourceFolderPath)
        {
            var unitySelection = activeObject;
            var targetPath = unitySelection != null ? AssetDatabase.GetAssetPath(unitySelection) : null;

            if (string.IsNullOrEmpty(targetPath))
                targetPath = "Assets";

            var attribs = GetAttributes(targetPath);

            if ((attribs & FileAttributes.Directory) != FileAttributes.Directory)
                targetPath = GetDirectoryName(targetPath);

            // Get path to project.
            var pathToProject = dataPath.Split(new string[1] { "/Assets" }, None)[0];

            targetPath = $"{pathToProject}/{targetPath}/{GetFileName(sourceFolderPath)}";

            if (!Directory.Exists(targetPath)) return targetPath;
            
            LogWarning(
                $"A folder already exists at this location, aborting link.\n{sourceFolderPath} -> {targetPath}");
            return null;
        }
        
        private static void OnProjectWindowItemGUI(string guid, Rect r)
        {
            try
            {
                var path = GUIDToAssetPath(guid);

                if (string.IsNullOrEmpty(path)) return;
                
                var attributes = GetAttributes(path);
                
                
                if ((attributes & FOLDER_SYMLINK_ATTRIBS) != FOLDER_SYMLINK_ATTRIBS) return;
                
                Label(r, "<=>", SymlinkMarkerStyle);
            }
            catch
            {
                
            }
        }

        #endregion

        
        #region Private
        
        private const FileAttributes FOLDER_SYMLINK_ATTRIBS = FileAttributes.Directory | ReparsePoint;
        private const string _commandLineExecutableName = "CMD.exe";
        
        #endregion
    }
}