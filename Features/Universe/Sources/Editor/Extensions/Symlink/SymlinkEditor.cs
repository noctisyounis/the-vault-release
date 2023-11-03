using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
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
            //projectWindowItemOnGUI += OnProjectWindowItemGUI;
            //EditorApplication.update += UnityEngine.PlayerLoop.Update;
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
            if (!InternalEditorUtility.inBatchMode && !DisplayDialog("Vault Warning",
                    $"Are you sure you want to LOAD SYMLINKS? \n{sourcePath} \n(This operation can take a long time)", 
                    "Let's go",
                    "No way")) return;
            using (var cmd = Start(_commandLineExecutableName, $"/C mklink /J \"{targetPath}\" \"{sourcePath}\""))
            {
                cmd?.WaitForExit();
            }
            Refresh(ForceUpdate);
        }
        
        public static void LoadAllSymlink(string[] directories, string targetPath)
        {
            if (!InternalEditorUtility.inBatchMode && !DisplayDialog("Vault Warning",
                            "Are you sure you want to LOAD ALL SYMLINKS? \n(This operation can take a long time)", 
                            "Let's go",
                            "No way")) return;
            
            foreach (var s in directories)
            {
                var name = GetFileName(s);
                if (name.Contains("GraphicsTier")) continue;

                var target = $"{targetPath}\\{name}";
                
                LoadSymlink(s, target);
            }
            Refresh(ForceUpdate);
        }

        public static void RemoveSymlink(string folderPath)
        {
            if (!Directory.Exists(folderPath)) return;
            if (!InternalEditorUtility.inBatchMode && !DisplayDialog("Vault Warning",
                    $"Are you sure you want to UNLOAD SYMLINK? \n{folderPath} \n(This operation can take a long time)",
                    "Let's go", 
                    "No way")) return;

            Directory.Delete( folderPath );
            Delete ($"{folderPath}.meta");
                
            Refresh(ForceUpdate);
        }
        
        public static void RemoveAllSymlinks([NotNull] string[] directories, string targetPath)
        {
            if (directories == null) throw new ArgumentNullException(nameof(directories));
            if (!InternalEditorUtility.inBatchMode && !DisplayDialog("Vault Warning",
                    "Are you sure you want to UNLOAD ALL SYMLINK? \n(This operation can take a long time)", 
                    "Let's go",
                    "No way")) return;

            foreach (var s in directories)
            {
                var name = GetFileName(s);
                if (name.Contains("GraphicsTier")) continue;
                var folderPath = $"{targetPath}\\{name}";
                
                RemoveSymlink(folderPath);
            }
            Refresh(ForceUpdate);
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
        
        // private static void OnProjectWindowItemGUI(string guid, Rect r)
        // {
        //     try
        //     {
        //         TryDrawSymlink(guid, r);
        //         TryDrawGit(guid, r);
        //     }
        //     catch
        //     {
        //         
        //     }
        // }

        private static void TryDrawSymlink(string guid, Rect r)
        {
            var path = GUIDToAssetPath(guid);

            if (string.IsNullOrEmpty(path)) return;
                
            var attributes = GetAttributes(path);
                
                
            if ((attributes & FOLDER_SYMLINK_ATTRIBS) != FOLDER_SYMLINK_ATTRIBS) return;
                
            Label(r, "<=>", SymlinkMarkerStyle);
        }

    // //--------------------------------------------------------------------------------------------------------------
    // // Git Prototype
    // //--------------------------------------------------------------------------------------------------------------
    //
    // private static Dictionary<string, string> fileStatusCache = new Dictionary<string, string>();
    // private static double lastUpdateTime = 0;
    // private const double updateInterval = 60.0; // In seconds
    //
    // private static void TryDrawGit(string guid, Rect selectionRect)
    // {
    //     var path = AssetDatabase.GUIDToAssetPath(guid);
    //     if (GetExtension(path) != ".unity" && GetExtension(path) != ".prefab") return;
    //
    //     string label;
    //
    //     if (!fileStatusCache.TryGetValue(path, out label))
    //     {
    //         string fileStatus = ExecuteGitCommand($"status --porcelain \"{path}\"");
    //         if (fileStatus.StartsWith("A") || fileStatus.StartsWith("??"))
    //         {
    //             label = "ADDED";
    //         }
    //         else if (fileStatus.StartsWith("M"))  // Checking if the scene is modified locally
    //         {
    //             label = "TO COMMIT";
    //         }
    //         else
    //         {
    //             bool isLatest = IsLatestCommitInBranch(GetLatestCommitOfFile(path), GetCurrentBranch(), path);
    //             label = isLatest ? "UP TO DATE" : "OUTDATED";
    //         }
    //
    //         fileStatusCache[path] = label; // Cache the status
    //     }
    //
    //     // Adjust the style based on the label
    //     GUIStyle styleToUse;
    //     if (label == "UP TO DATE")
    //     {
    //         styleToUse = GitUpToDateStyle;
    //     }
    //     else if (label == "OUTDATED")
    //     {
    //         styleToUse = GitOutdatedStyle;
    //     }
    //     else if (label == "TO COMMIT")
    //     {
    //         styleToUse = GitToCommitStyle;  // Assuming you have defined this GUIStyle for "TO COMMIT" label
    //     }
    //     else if (label == "ADDED")
    //     {
    //         styleToUse = GitAddedStyle;
    //     }
    //     else
    //     {
    //         styleToUse = SymlinkMarkerStyle;
    //     }
    //
    //     EditorGUI.LabelField(selectionRect, label, styleToUse);
    // }
    //
    // private static void Update()
    // {
    //     double currentTime = EditorApplication.timeSinceStartup;
    //     if (currentTime - lastUpdateTime > updateInterval)
    //     {
    //         fileStatusCache.Clear(); // Clear the cache to re-check Git status
    //         lastUpdateTime = currentTime;
    //     }
    // }
    //
    // private static string GetLatestCommitOfFile(string filePath)
    // {
    //     return ExecuteGitCommand($"log -n 1 --pretty=format:\"%H\" \"{filePath}\"");
    // }
    //
    // private static string GetCurrentBranch()
    // {
    //     return ExecuteGitCommand("rev-parse --abbrev-ref HEAD");
    // }
    //
    // private static bool IsLatestCommitInBranch(string commitHash, string branchName, string path)
    // {
    //     // List all commit hashes that affected the file since the specified commit
    //     string allCommitsSince = ExecuteGitCommand($"log {commitHash}..{branchName} --pretty=format:\"%H\" \"{path}\"");
    //
    //     // If the scene's commit hash isn't the last on that list, it's not the latest version.
    //     string[] commits = allCommitsSince.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
    //
    //     return commits.Length == 0; // If no commits since, then it's the latest
    // }
    //
    // private static string ExecuteGitCommand(string command)
    // {
    //     try
    //     {
    //         Process gitProcess = new Process();
    //         gitProcess.StartInfo.FileName = "git";
    //         gitProcess.StartInfo.Arguments = command;
    //         gitProcess.StartInfo.RedirectStandardOutput = true;
    //         gitProcess.StartInfo.UseShellExecute = false;
    //         gitProcess.StartInfo.CreateNoWindow = true;
    //         gitProcess.Start();
    //
    //         string output = gitProcess.StandardOutput.ReadToEnd();
    //         gitProcess.WaitForExit();
    //
    //         return output.Trim(); // Remove any newlines or whitespace
    //     }
    //     catch
    //     {
    //         return string.Empty;
    //     }
    // }

    //--------------------------------------------------------------------------------------------------------------
    // End Git Prototype
    //-----
        
        #endregion

        
        #region Private
        
        private const FileAttributes FOLDER_SYMLINK_ATTRIBS = FileAttributes.Directory | ReparsePoint;
        private const string _commandLineExecutableName = "CMD.exe";
        
        #endregion
    }
}