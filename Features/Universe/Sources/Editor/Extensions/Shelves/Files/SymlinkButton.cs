using System;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;

using static System.IO.Directory;
using static System.IO.Path;
using static Symlink.Editor.SymlinkEditor;
using static UnityEngine.Debug;
using static UnityEngine.GUILayout;
using static UnityEngine.GUI;
using static Universe.Editor.UPath;

namespace Universe.Toolbar.Editor
{
    public class SymlinkButtons
    {
        #region Main

        public static void Draw()
        {
            CreateDirectoryIfNotExist(SourceDirectoryPath);
            CreateDirectoryIfNotExist(SourceGraphicsTiersDirectoryPath);
            CreateDirectoryIfNotExist(TargetDirectoryPath);
            CreateDirectoryIfNotExist(TargetGraphicsTiersDirectoryPath);
            
            var directories = GetDirectories(SourceDirectoryPath);
            var graphicsDirectories = GetDirectories(SourceGraphicsTiersDirectoryPath);
            
            if (Button("Load All")) LoadAllSymlink(graphicsDirectories, TargetGraphicsTiersDirectoryPath);
            if (Button("Unload All")) RemoveAllSymlinks(graphicsDirectories, TargetGraphicsTiersDirectoryPath);

            DrawAllToggleButtons(directories, TargetDirectoryPath);
            DrawAllToggleButtons(graphicsDirectories, TargetGraphicsTiersDirectoryPath);
        }

        #endregion
        
        
        #region Utils

        private static void CreateDirectoryIfNotExist(string path)
        {
            if (Exists(path)) return;
            CreateDirectory(path);
        }

        private static void DrawAllToggleButtons([NotNull] string[] directories, string targetPath)
        {
            if (directories is null) throw new ArgumentNullException(nameof(directories));
            
            foreach (var s in directories)
            {
                var fileName = GetFileName(s);
                if (fileName.Contains("GraphicsTier")) continue;
                
                var target = $"{targetPath}\\{fileName}";

                DrawToggleButton(s, fileName, target);
            }
        }
        
        private static void DrawToggleButton(string path, string fileName, string targetPath)
        {
            if (Exists(targetPath))
            {
                var attributes = File.GetAttributes(targetPath);
                if ((attributes & FOLDER_SYMLINK_ATTRIBS) != FOLDER_SYMLINK_ATTRIBS)
                {
                    var defaultStyle = GUI.backgroundColor;
                    
                    backgroundColor = Color.red;
                    LogError($"[SHELF FILES]{fileName} isn't a symlink");
                    Button($"INVALID {fileName}");
                    backgroundColor = defaultStyle;
                    return;
                }
                
                if (Button($"Unload {fileName}"))
                {
                    RemoveSymlink(targetPath);
                }

                return;
            }

            if (Button($"Load {fileName}")) 
            {
                LoadSymlink(path, targetPath);
            }
        }
        
        #endregion
        
        
        #region Private
        
        private const FileAttributes FOLDER_SYMLINK_ATTRIBS = FileAttributes.Directory | FileAttributes.ReparsePoint;

        
        #endregion
    }
}