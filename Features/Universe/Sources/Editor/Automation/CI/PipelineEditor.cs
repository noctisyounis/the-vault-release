using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;
using Universe.DebugWatch.Editor;
using Universe.DebugWatchTools.Runtime;
using Universe.SceneTask.Runtime;

using static System.DateTime;
using static System.IO.File;
using static System.IO.Directory;
using static UnityEditor.BuildTarget;
using static UnityEditor.BuildOptions;
using static UnityEditor.BuildPipeline;
using static UnityEngine.Debug;
using static UnityEngine.Application;
using static UnityEngine.Mathf;
using static Universe.Editor.UPath;
using static Universe.Editor.UPlatform;
using static Universe.Editor.UGroupHelper;
using static Universe.Editor.USettingsHelper;
using static Universe.SceneTask.Runtime.Environment;
using static Symlink.Editor.SymlinkEditor;

namespace Universe.Editor
{
    public class PipelineEditor : AssetPostprocessor, IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        #region Constant
        
        private static Dictionary<BuildTarget, string> TARGET_NAMES = new ()
        {
            {StandaloneWindows64, PLATFORM_DISPLAY_NAME_WIN64},
            {Android, PLATFORM_DISPLAY_NAME_ANDROID},
            {PS5, PLATFORM_DISPLAY_NAME_PS5}
        }; 
        
        private static Dictionary<BuildTarget, string> TARGET_EXTENSIONS = new ()
        {
            {StandaloneWindows64, PLATFORM_EXTENSION_WIN64},
            {Android, PLATFORM_EXTENSION_ANDROID},
            {PS5, PLATFORM_EXTENSION_PS5}
        }; 

        public int callbackOrder => 0;

        //if this is set to 9, then 1.0.9 will become 1.1.0
        private static int  s_versionIncrementUpAt = 999; 
        
        #endregion


        #region Build
        
        [MenuItem( "Vault/CI/Load Version" )]
        public static void LoadVersionBundle()
        {
            var id = "0";
            using (var fs = OpenRead(LOCAL_COMMIT_ID_PATH))
            {
                using (var sr = new StreamReader(fs))
                {
                    id = sr.ReadLine();
                }
            }

            var versionPath = $"{VERSION_PATH}";
            var version = "0.000.001";
            
            using (var fs = OpenRead(versionPath))
            {
                using (var sr = new StreamReader(fs))
                {
                    var json = sr.ReadLine();
                    var infos = JsonUtility.FromJson<CommitInfos>(json);

                    version = infos.version;
                }
            }

            ApplyVersion(version);
        }

        [MenuItem("Vault/CI/Increment Win64 Version")]
        public static void RequestWin64VersionIncrement() =>
            UpgradeVersionBundle( PLATFORM_DISPLAY_NAME_WIN64 );
        
        [MenuItem("Vault/CI/Increment Android Version")]
        public static void RequestAndroidVersionIncrement() =>
            UpgradeVersionBundle( PLATFORM_DISPLAY_NAME_ANDROID );
        
        
        [MenuItem("Vault/CI/Increment PS5 Version")]
        public static void RequestPS5VersionIncrement() =>
            UpgradeVersionBundle( PLATFORM_DISPLAY_NAME_PS5 );
        
        
        public static void RequestSymlinkLoad()
        {
            var graphicsDirectories = GetDirectories(SourceGraphicsTiersDirectoryPath);

            LoadAllSymlink( graphicsDirectories, TargetGraphicsTiersDirectoryPath );
        }

        public static void RequestSymlinkUnload()
        {
            var graphicsDirectories = GetDirectories(SourceGraphicsTiersDirectoryPath);

            RemoveAllSymlinks( graphicsDirectories, TargetGraphicsTiersDirectoryPath );
        }

        public static void RequestAddressablePurgeAndRefresh()
        {
            BuildCache.PurgeCache(false);
            
            RequestAddressableRefresh();
        }
        
        public static void RequestAddressableRefresh()
        {
            RefreshAaGroups();
            LevelManagement.BakeLevelDebug();
            DebugWatchDictionary.TryValidate();
            RefreshAaGroups();
        }

        [MenuItem("Vault/CI/Clear Movers .bat")]
        public static void ClearMovers()
        {
            ClearSlackMover();
            ClearSteamMover();
        }

        [MenuItem( "Vault/CI/Build Win64/[DEV]" )]
        public static void RequestWin64DevBuild()
        {
            UpdateRuntimeVersion();
            
            _CITriggered = true;
            StartBuild(StandaloneWindows64, true );
        }

        [MenuItem( "Vault/CI/Build Win64/[Release]" )]
        public static void RequestWin64ReleaseBuild()
        {
            UpdateRuntimeVersion();
            
            _CITriggered = true;
            StartBuild(StandaloneWindows64, false );
        }
        
        [MenuItem( "Vault/CI/Build Android/[DEV]" )]
        public static void RequestAndroidDevBuild()
        {
            UpdateRuntimeVersion();
            
            _CITriggered = true;
           
            InjectAndroidKeystore();
            StartBuild(Android, true );
        }

        [MenuItem( "Vault/CI/Build Android/[Release]" )]
        public static void RequestAndroidReleaseBuild()
        {
            UpdateRuntimeVersion();
            
            _CITriggered = true;
            
            InjectAndroidKeystore();
            StartBuild(Android, false );
        }
        
        [MenuItem( "Vault/CI/Build PS5/[DEV]" )]
        public static void RequestPS5DevBuild()
        {
            UpdateRuntimeVersion();
            
            _CITriggered = true;
            StartBuild(PS5, true );
        }
        
        [MenuItem( "Vault/CI/Build PS5/[Release]" )]
        public static void RequestPS5ReleaseBuild()
        {
            UpdateRuntimeVersion();
            
            _CITriggered = true;
            StartBuild(PS5, false );
        }

        private static void StartBuild( BuildTarget target, bool developmentBuild )
        {
            var targetName  = TARGET_NAMES[target];
            var extension   = TARGET_EXTENSIONS[target];
            var version     = PlayerSettings.bundleVersion;
            var name        = productName;
            var prefix      = developmentBuild ? DEVELOPMENT_BUILD_PREFIX : RELEASE_BUILD_PREFIX;
            var fullName    = $"{prefix}{name}_{targetName}_{version}";
            var folderPath  = $"{BUILD_PATH}\\{targetName}\\{fullName}";
            var option      = new BuildPlayerOptions
            {
                scenes              = new string[] { TASK_GAME_STARTER_PATH },
                locationPathName    = $"{folderPath}\\{name}{extension}",
                target              = target,
                options             = developmentBuild ? Development : 0
            };
            
            var levelSettings = GetSettings<LevelSettings>();

            levelSettings.m_startingEnvironment = BOTH;
            levelSettings.SaveAsset();
            UpdateBundleCode( developmentBuild );

            switch( target )
            {
                case StandaloneWindows64:
                {
                    PrepareWin64Package( targetName, name, version, developmentBuild );
                    break;
                }
                case Android:
                {
                    PrepareAndroidPackage( targetName, name, version, developmentBuild );
                    break;
                }
                case PS5:
                {
                    PreparePS5Package( targetName, name, version, developmentBuild );
                    break;
                }
            }

            BuildPlayer( option );
        }

        public void OnPreprocessBuild( BuildReport report )
        {
            var summary = report.summary;
            var platform= summary.platform;
            var path= summary.outputPath;
            var isDevelopment = (summary.options & Development) != 0;

            Log( $"[{Now}] Preparing build for {platform} in {path} for {( isDevelopment ? "Development" : "Release" )}" );
        }
        
        [PostProcessBuild]
        public static void ReplaceAndroidManifest(BuildTarget buildTarget, string pathToBuiltProject)
        {
          if (buildTarget != BuildTarget.Android) return;
           
          var manifestPath = Application.dataPath + ANDROID_TEMPORARY_MANIFEST;
          var customManifestPath = Application.dataPath + ANDROID_CUSTOM_MANIFEST; 
        
          FileUtil.ReplaceFile(customManifestPath, manifestPath);
        }

        public void OnPostprocessBuild( BuildReport report )
        {
            var name                = productName;
            var summary                  = report.summary;
            var path                = summary.outputPath;
            var platform        = summary.platform;
            
            if(!_CITriggered) return;
            
            _CITriggered = false;

            if (summary.result.Equals(BuildResult.Failed))
            {
                EditorApplication.Exit( 1 );
                return;
            }

            if (platform == Android)
                GenerateAndroidDeploymentFiles(path, name);
            
            if(!_CITriggered) return;
            
            _CITriggered = false;

            if (platform == PS5)
                UpdatePS5DeploymentFiles(path, name);
        }

        #endregion


        #region Utils

        [MenuItem( "Vault/Build/Upgrade Version Bundle" )]
        private static void UpgradeVersionBundle() =>
            UpgradeVersionBundle( PLATFORM_DISPLAY_NAME_WIN64 );

        private static void UpgradeVersionBundle( string platform )
        {
            var versionBundleText         = PlayerSettings.bundleVersion;

            if( string.IsNullOrEmpty( versionBundleText ) )
            {
                versionBundleText = "0.000.001";
            }
            else
            {
                var lines = VersionSplit(versionBundleText);

                var majorVersion = 0;
                var minorVersion = 0;
                var subMinorVersion = 0;

                if( lines.Length > 0 ) majorVersion = int.Parse( lines[0] );
                if( lines.Length > 1 ) minorVersion = int.Parse( lines[1] );
                if( lines.Length > 2 ) subMinorVersion = int.Parse( lines[2] );

                subMinorVersion++;
                if( subMinorVersion > s_versionIncrementUpAt)
                {
                    minorVersion++;
                    subMinorVersion = 0;
                }
                
                if( minorVersion > s_versionIncrementUpAt )
                {
                    majorVersion++;
                    minorVersion = 0;
                }

                versionBundleText = $"{majorVersion:00}.{minorVersion:000}.{subMinorVersion:000}";
            }
            
            ApplyVersion(versionBundleText);
            Log( $"Version Incremented to {versionBundleText}");
        }

        private static void ApplyVersion(string version)
        {
            var androidVersionBundleCode = VersionToInt(version);
            
            _lastVersion = version;
            _lastAndroidBundleCode = androidVersionBundleCode;
            PlayerSettings.bundleVersion = _lastVersion;
            PlayerSettings.Android.bundleVersionCode = _lastAndroidBundleCode;
            Log( $"Bundle Version Set To {androidVersionBundleCode}" );
        }            

        private static string[] VersionSplit(string version)
        {
            version = version.Trim(); //clean up whitespace if necessary
            var lines = version.Split('.');

            return lines;
        }

        private static int VersionToInt(string version)
        {
            var result = 0;
            var lines = VersionSplit(version);
            var major = 0;
            var medior = 0;
            var minor = 0;

            if( lines.Length > 0 )
                major = int.Parse( lines[0] );
            if( lines.Length > 1 )
                medior = int.Parse( lines[1] );
            if( lines.Length > 2 )
                minor = int.Parse( lines[2] );

            var versionBase = s_versionIncrementUpAt + 1;

            result = minor + versionBase * medior + (int)Pow(versionBase, 2) * major;

            return result;
        }

        private static void UpdateBundleCode(bool developmentBuild)
        {
            var version = PlayerSettings.bundleVersion;
            var bundleCode = VersionToInt(version);
            
            bundleCode *= 100;
            bundleCode += developmentBuild ? 0 : 5;

            Debug.Log(bundleCode);

            PlayerSettings.Android.bundleVersionCode = bundleCode;
        }

        private static void UpdateRuntimeVersion()
        {
            var version = GetSettings<CISettings>();

            version.m_buildTime = $"{Now}";
            version.m_version = _lastVersion;
            version.m_androidBundleCode = _lastAndroidBundleCode;

            version.SaveAsset();
        }

        private static void ClearSlackMover() =>
            WriteAllText( BUILD_SLACK_MOVER_PATH, string.Empty );
        
        
        private static void ClearSteamMover() =>
            WriteAllText( BUILD_STEAM_MOVER_PATH, $"del /s /q {STEAM_CONTENT_BUILDER_PATH}\n" );
        
        
        private static void PrepareWin64Package( string platform, string name, string version, bool developmentBuild )
        {
            var prefix                 = developmentBuild ? DEVELOPMENT_BUILD_PREFIX : RELEASE_BUILD_PREFIX;
            var doNotShipBurstName          = $"{name}{DO_NOT_SHIP_BURST_SUFFIX}";
            var doNotShipILName             = $"{name}{DO_NOT_SHIP_IL_SUFFIX}";
            var batRelativeBuildPath   = BUILD_PATH.Replace("..", ".");
            var fullName                    = $"{prefix}{name}_{platform}_{version}";
            var path                        = $"{batRelativeBuildPath}\\{platform}\\{fullName}";
            var zipPath                     = $"{UPLOAD_PATH}\\{fullName}.zip";
            var doNotShipBurstPath          = $"{path}\\{doNotShipBurstName}";
            var doNotShipILPath             = $"{path}\\{doNotShipILName}";

            using( var sw = AppendText( BUILD_SLACK_MOVER_PATH ) )
            {
                var createUploadIfNotExists = $"if not exist \"{UPLOAD_PATH}\" mkdir \"{UPLOAD_PATH}\"";
                var deleteDoNotShipBurst    = $"rmdir /s /q \"{doNotShipBurstPath}\"";
                var deleteDoNotShipIL       = $"rmdir /s /q \"{doNotShipILPath}\"";
                var zipping                 = $"powershell -Command \"Add-Type -Assembly 'System.IO.Compression.FileSystem'; [System.IO.Compression.ZipFile]::CreateFromDirectory('./{path}/', './{zipPath}')\"";

                sw.WriteLine( createUploadIfNotExists );
                if (!developmentBuild)
                {
                    sw.WriteLine( deleteDoNotShipBurst );
                    sw.WriteLine( deleteDoNotShipIL );
                }
                sw.WriteLine( zipping );
            }

            var steamContentSubPath = developmentBuild ? "debug" : "release";
            var steamContentZipPath = $"{STEAM_CONTENT_BUILDER_PATH}\\{steamContentSubPath}\\{fullName}.zip";

            using( var sw = AppendText( BUILD_STEAM_MOVER_PATH ) )
            {
                var moveShip = $"copy \"{zipPath}\" \"{steamContentZipPath}\"";

                sw.WriteLine( moveShip );
            }
        }

        private static void InjectAndroidKeystore()
        {
            using (var fs = OpenRead(KEYSTORE_PATH))
            {
                using (var sr = new StreamReader(fs))
                {
                    var password = sr.ReadLine();
                    
                    PlayerSettings.Android.keystorePass = password;
                    PlayerSettings.Android.keyaliasPass = password;
                }
            }
            
            PlayerSettings.Android.keyaliasName = productName.Replace(" ", "").ToLower();
        }
        
        private static void PrepareAndroidPackage( string platform, string name, string version, bool developmentBuild )
        {
            var androidBundleName      = identifier;
            var androidBundleVersion     = PlayerSettings.Android.bundleVersionCode;
            var baseObbName                 = $"{name}.main.obb";
            var finalObbName                = $"main.{androidBundleVersion}.{androidBundleName}.obb";
            
            var prefix                 = developmentBuild ? DEVELOPMENT_BUILD_PREFIX : RELEASE_BUILD_PREFIX;
            var doNotShipBurstName          = $"{name}{DO_NOT_SHIP_BURST_SUFFIX}";
            var doNotShipILName             = $"{name}{DO_NOT_SHIP_IL_SUFFIX}";
            var batRelativeBuildPath   = BUILD_PATH.Replace("..", ".");
            var fullName                    = $"{prefix}{name}_{platform}_{version}";
            var path                        = $"{batRelativeBuildPath}\\{platform}\\{fullName}";
            var obbPath                     = $"{path}\\{baseObbName}";
            var zipPath                     = $"{UPLOAD_PATH}\\{fullName}.zip";
            var doNotShipBurstPath          = $"{path}\\{doNotShipBurstName}";
            var doNotShipILPath             = $"{path}\\{doNotShipILName}";

            using( var sw = AppendText( BUILD_SLACK_MOVER_PATH ) )
            {
                var createUploadIfNotExists = $"if not exist \"{UPLOAD_PATH}\" mkdir \"{UPLOAD_PATH}\"";
                var renameObb               = $"ren \"{obbPath}\" \"{finalObbName}\"";
                var deleteDoNotShipBurst    = $"rmdir /s /q \"{doNotShipBurstPath}\"";
                var deleteDoNotShipIL       = $"rmdir /s /q \"{doNotShipILPath}\"";
                var zipping                 = $"powershell -Command \"Add-Type -Assembly 'System.IO.Compression.FileSystem'; [System.IO.Compression.ZipFile]::CreateFromDirectory('./{path}/', './{zipPath}')\"";

                sw.WriteLine( createUploadIfNotExists );
                sw.WriteLine( renameObb );
                if (!developmentBuild)
                {
                    sw.WriteLine( deleteDoNotShipBurst );
                    sw.WriteLine( deleteDoNotShipIL );
                }
                sw.WriteLine( zipping );
            }
        }

        private static void GenerateAndroidDeploymentFiles(string path, string projectName)
        {
            var deployPath          = $"{path}";
            
            deployPath = deployPath.Replace($"{PLATFORM_EXTENSION_ANDROID}", string.Empty);
            var deployAndRunPath    = $"{deployPath}_{DEPLOY_AND_RUN_SUFFIX}.bat";
            
            deployPath = $"{deployPath}_{DEPLOY_SUFFIX}.bat";

            if (!File.Exists(deployPath)) Create(deployPath).Close();
            else WriteAllText(deployPath, string.Empty);
            
            if (!File.Exists(deployAndRunPath)) Create(deployAndRunPath).Close();
            else WriteAllText(deployAndRunPath, string.Empty);

            var androidBundleName      = identifier;
            var androidBundleVersion     = PlayerSettings.Android.bundleVersionCode;
            var obbName                = $"main.{androidBundleVersion}.{androidBundleName}.obb";
            var writter = new StreamWriter(deployPath);

            var uninstallPrevious = $"adb uninstall \"{androidBundleName}\"";
            
            var installApk  = $"adb install \"{projectName}.apk\"";
            var createObbFolder = $"adb shell mkdir \"/sdcard/Android/obb/{androidBundleName}/\"";
            var installObb = $"adb push -p \"{obbName}\" \"/sdcard/Android/obb/{androidBundleName}/\"";
            var run = $"adb shell monkey -p {androidBundleName} -c android.intent.category.LAUNCHER 1";

            writter.WriteLine(uninstallPrevious);
            writter.WriteLine(installApk);
            writter.WriteLine(createObbFolder);
            writter.WriteLine(installObb);
            writter.Close();
            
            writter = new StreamWriter(deployAndRunPath);
            writter.WriteLine(uninstallPrevious);
            writter.WriteLine(installApk);
            writter.WriteLine(createObbFolder);
            writter.WriteLine(installObb);
            writter.WriteLine(run);
            writter.Close();
        }
        
        private static void PreparePS5Package( string platform, string name, string version, bool developmentBuild )
        {
            var prefix                  = developmentBuild ? DEVELOPMENT_BUILD_PREFIX : RELEASE_BUILD_PREFIX;
            var batRelativeBuildPath    = BUILD_PATH.Replace("..", ".");
            var fullName                = $"{prefix}{name}_{platform}_{version}";
            var path                    = $"{batRelativeBuildPath}\\{platform}\\{fullName}\\{name}";
            var exportPath                 = $"{UPLOAD_PATH}\\{fullName}";

            using( var sw = AppendText( BUILD_SLACK_MOVER_PATH ) )
            {
                var createUploadIfNotExists = $"if not exist \"{UPLOAD_PATH}\" mkdir \"{UPLOAD_PATH}\"";
                var exporting               = $"move \"{path}\" \"{exportPath}\"";

                sw.WriteLine( createUploadIfNotExists );
                sw.WriteLine( exporting );
            }
        }

        private static void UpdatePS5DeploymentFiles(string path, string projectName)
        {
            var deployPath          = $"{path}{projectName}_{DEPLOY_SUFFIX}.bat";
            var deployAndRunPath    = $"{path}{projectName}_{DEPLOY_AND_RUN_SUFFIX}.bat";
            var workspace      = isDebugBuild ? PS5_WORKSPACE_DEVELOPMENT : PS5_WORKSPACE_RELEASE;

            if( !File.Exists( deployPath ) ) return;

            var adaptedDeploy       = ReadAllText(deployPath);
            var adaptedDeployAndRun = ReadAllText(deployAndRunPath);

            adaptedDeploy = adaptedDeploy.Replace( PS5_WORKSPACE_DEFAULT, workspace );
            adaptedDeployAndRun = adaptedDeployAndRun.Replace( PS5_WORKSPACE_DEFAULT, workspace );

            WriteAllText( deployPath, adaptedDeploy );
            WriteAllText( deployAndRunPath, adaptedDeployAndRun );
        }

        #endregion


        #region Private

        private static string _lastVersion;
        private static int _lastAndroidBundleCode;
        private static bool _CITriggered;

        #endregion
    }
}