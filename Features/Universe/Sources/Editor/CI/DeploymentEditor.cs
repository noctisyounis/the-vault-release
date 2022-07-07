using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

using static System.DateTime;
using static System.IO.File;
using static System.IO.Directory;
using static UnityEditor.BuildTarget;
using static UnityEditor.BuildOptions;
using static UnityEditor.BuildPipeline;
using static UnityEditor.AddressableAssets.Settings.AddressableAssetSettings;
using static UnityEngine.Debug;
using static UnityEngine.Application;
using static Universe.Editor.UGroupHelper;
using static Universe.Editor.USettingsHelper;
using static Symlink.Editor.SymlinkEditor;

namespace Universe.Editor
{
    public class DeploymentEditor : AssetPostprocessor, IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        #region Constant

        //Absolute paths
        private const string STEAM_CONTENT_BUILDER_PATH = "D:\\steamworks_sdk_153a\\sdk\\tools\\ContentBuilder\\content";

        //Project relative
        private const string TASK_GAME_STARTER_PATH = "Assets\\_\\GameStarter\\GameStarter.unity";
        private const string BUILD_PATH             = "..\\Builds";
        private const string BUILD_LOG_PATH         = "..\\BuildLog.txt";
        private const string BUILD_VERSION_PATH     = "..\\..\\{productName}_{platform}_LastBuildVersion.txt";
        private const string BUILD_SLACK_MOVER_PATH = "..\\Jenkins_Slack_Uploader.bat";
        private const string BUILD_STEAM_MOVER_PATH = "..\\Jenkins_Steam_Mover.bat";

        //.bat relative
        private const string UPLOAD_PATH = ".\\SlackUpload";

        //static names
        private const string DEVELOPMENT_BUILD_PREFIX       = "[DEV]";
        private const string RELEASE_BUILD_PREFIX           = "[Release]";
        private const string DO_NOT_SHIP_BURST_SUFFIX       = "_BurstDebugInformation_DoNotShip";
        private const string DO_NOT_SHIP_IL_SUFFIX          = "_BackUpThisFolder_ButDontShipItWithYourGame";

        private const string PLATFORM_DISPLAY_NAME_PC       = "Win64";
        private const string PLATFORM_EXTENSION_PC          = ".exe";
        private const string PLATFORM_DISPLAY_NAME_ANDROID  = "Android";
        private const string PLATFORM_EXTENSION_ANDROID     = ".apk";
        private const string PLATFORM_DISPLAY_NAME_PS5      = "PS5";
        private const string PLATFORM_EXTENSION_PS5         = "\\";

        private const string PS5_WORKSPACE_DEVELOPMENT  = "workspace2";
        private const string PS5_WORKSPACE_RELEASE      = "workspace1";

        private const string MANAGERS_PARENT_NAME = "Managers";
        private const string OCULUS_MANAGER_NAME = "OculusManager";
        private const string STEAM_MANAGER_NAME = "SteamManager";
        private const string LIV_MANAGER_NAME = "LIV";

        public int callbackOrder => 0;

        private static string SourceDirectoryPath => $"{dataPath}\\..\\..\\Symlinks";
        private static string SourceGraphicsTiersDirectoryPath => $"{SourceDirectoryPath}\\GraphicsTier";
        private static string TargetDirectoryPath => $"{dataPath}\\_\\Content";
        private static string TargetGraphicsTiersDirectoryPath => $"{TargetDirectoryPath}\\GraphicsTier";

        #endregion


        #region Build

        [MenuItem("Vault/CI/Increment PC Version")]
        public static void RequestWin64VersionIncrement()
        {
            UpgradeVersionBundle( PLATFORM_DISPLAY_NAME_PC );
        }
        
        [MenuItem("Vault/CI/Increment Android Version")]
        public static void RequestAndroidVersionIncrement()
        {
            UpgradeVersionBundle( PLATFORM_DISPLAY_NAME_ANDROID );
        }
        
        [MenuItem("Vault/CI/Increment PS5 Version")]
        public static void RequestPS5VersionIncrement()
        {
            UpgradeVersionBundle( PLATFORM_DISPLAY_NAME_PS5 );
        }

        [MenuItem( "Vault/CI/Build PC" )]
        public static void RequestWin64Builds()
        {
            _sw = AppendText( BUILD_LOG_PATH );
            _sw.WriteLine( $"[{Now}] PC Build Requested" );
            UpdateRuntimeVersion();
            _sw.WriteLine( $"[{Now}] Runtime Version Updated" );

            CleanPlayerContent();

            var directories         = GetDirectories(SourceDirectoryPath);
            var graphicsDirectories = GetDirectories(SourceGraphicsTiersDirectoryPath);

            LoadAllSymlink( graphicsDirectories, TargetGraphicsTiersDirectoryPath );
            OnRefreshCompleted += StartWin64Builds;
            ReloadAndBuildAddressable.Execute();
        }

        [MenuItem( "Vault/CI/Build Android" )]
        public static void RequestAndroidBuilds()
        {
            _sw = AppendText( BUILD_LOG_PATH );
            _sw.WriteLine( $"[{Now}] Android Build Requested" );
            UpdateRuntimeVersion();
            _sw.WriteLine( $"[{Now}] Runtime Version Updated" );

            CleanPlayerContent();

            var directories         = GetDirectories(SourceDirectoryPath);
            var graphicsDirectories = GetDirectories(SourceGraphicsTiersDirectoryPath);

            LoadAllSymlink( graphicsDirectories, TargetGraphicsTiersDirectoryPath );
            OnRefreshCompleted += StartAndroidBuilds;
            ReloadAndBuildAddressable.Execute();
        }

        [MenuItem( "Vault/CI/Build PS5" )]
        public static void RequestPS5Builds()
        {
            _sw = AppendText( BUILD_LOG_PATH );
            _sw.WriteLine( $"[{Now}] PS5 Build Requested" );
            UpdateRuntimeVersion();
            _sw.WriteLine( $"[{Now}] Runtime Version Updated" );

            CleanPlayerContent();

            var directories         = GetDirectories(SourceDirectoryPath);
            var graphicsDirectories = GetDirectories(SourceGraphicsTiersDirectoryPath);

            LoadAllSymlink( graphicsDirectories, TargetGraphicsTiersDirectoryPath );
            OnRefreshCompleted += StartPS5Builds;
            ReloadAndBuildAddressable.Execute();
        }

        private static void StartWin64Builds()
        {
            var target = StandaloneWindows64;

            OnRefreshCompleted -= StartWin64Builds;
            _sw.WriteLine( $"[{Now}] PC Builds Pending" );

            ClearSlackMover();
            ClearSteamMover();

            StartBuild( target, false );
            StartBuild( target, true );

            var graphicsDirectories = GetDirectories(SourceGraphicsTiersDirectoryPath);

            _sw.WriteLine( $"[{Now}] Build Finished" );
            Log( $"[{Now}] Build Finished" );

            RemoveAllSymlinks( graphicsDirectories, TargetGraphicsTiersDirectoryPath );
            _sw.WriteLine( $"[{Now}] Symlink unloaded" );
            _sw.WriteLine( $"[{Now}] Stream Closed" );
            _sw.WriteLine( $"------------------------------------------------------------" );
            _sw.Close();
        }

        private static void StartAndroidBuilds()
        {
            var target = Android;

            OnRefreshCompleted -= StartAndroidBuilds;
            _sw.WriteLine( $"[{Now}] Android Builds Pending" );

            ClearSlackMover();

            StartBuild( target, true );
            StartBuild( target, false );

            var graphicsDirectories = GetDirectories(SourceGraphicsTiersDirectoryPath);

            _sw.WriteLine( $"[{Now}] Build Finished" );
            Log( $"[{Now}] Build Finished" );

            RemoveAllSymlinks( graphicsDirectories, TargetGraphicsTiersDirectoryPath );
            _sw.WriteLine( $"[{Now}] Symlink unloaded" );
            _sw.WriteLine( $"[{Now}] Stream Closed" );
            _sw.WriteLine( $"------------------------------------------------------------" );
            _sw.Close();
        }

        private static void StartPS5Builds()
        {
            var target = PS5;

            OnRefreshCompleted -= StartPS5Builds;
            _sw.WriteLine( $"[{Now}] PS5 Builds Pending" );

            ClearSlackMover();
            ClearSteamMover();

            StartBuild( target, true );
            StartBuild( target, false );

            var graphicsDirectories = GetDirectories(SourceGraphicsTiersDirectoryPath);

            _sw.WriteLine( $"[{Now}] Build Finished" );
            Log( $"[{Now}] Build Finished" );

            RemoveAllSymlinks( graphicsDirectories, TargetGraphicsTiersDirectoryPath );
            _sw.WriteLine( $"[{Now}] Symlink unloaded" );
            _sw.WriteLine( $"[{Now}] Stream Closed" );
            _sw.WriteLine( $"------------------------------------------------------------" );
            _sw.Close();
        }

        private static void StartBuild( BuildTarget target, bool developmentBuild )
        {
            var targetName  = TargetNameConverter(target);
            var extension   = TargetExtensionConverter(target);
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

            _sw.WriteLine( $"[{Now}] {fullName} Build Started" );

            BuildPlayer( option );
        }

        public void OnPreprocessBuild( BuildReport report )
        {
            var summary         = report.summary;
            var platform        = summary.platform;
            var path            = summary.outputPath;
            var isDevelopment   = (summary.options & Development) != 0;

            _sw.WriteLine( $"[{Now}] Preparing build for {platform} in {path} for {( isDevelopment ? "Development" : "Release" )}" );
            Log( $"[{Now}] Preparing build for {platform} in {path} for {( isDevelopment ? "Development" : "Release" )}" );
        }

        public void OnPostprocessBuild( BuildReport report )
        {
            var name                = productName;
            var path                = report.summary.outputPath;
            var workspace           = isDebugBuild ? PS5_WORKSPACE_DEVELOPMENT : PS5_WORKSPACE_RELEASE;
            var deployPath          = $"{path}{name}_Deploy.bat";
            var deployAndRunPath    = $"{path}{name}_DeployAndRun.bat";

            if( !File.Exists( deployPath ) )
                return;

            var adaptedDeploy       = ReadAllText(deployPath);
            var adaptedDeployAndRun = ReadAllText(deployAndRunPath);

            adaptedDeploy = adaptedDeploy.Replace( "workspaceX", workspace );
            adaptedDeployAndRun = adaptedDeployAndRun.Replace( "workspaceX", workspace );

            WriteAllText( deployPath, adaptedDeploy );
            WriteAllText( deployAndRunPath, adaptedDeployAndRun );
        }

        #endregion


        #region Utils

        private static string TargetNameConverter( BuildTarget target )
        {
            switch( target )
            {
                case StandaloneWindows64:
                    return PLATFORM_DISPLAY_NAME_PC;
                case Android:
                    return PLATFORM_DISPLAY_NAME_ANDROID;
                case PS5:
                    return PLATFORM_DISPLAY_NAME_PS5;
                default:
                    return target.ToString();
            }
        }

        private static string TargetExtensionConverter( BuildTarget target )
        {
            switch( target )
            {
                case StandaloneWindows64:
                    return PLATFORM_EXTENSION_PC;
                case Android:
                    return PLATFORM_EXTENSION_ANDROID;
                case PS5:
                    return PLATFORM_EXTENSION_PS5;
                default:
                    return target.ToString();
            }
        }

        [MenuItem( "Vault/Build/Upgrade Version Bundle" )]
        private static void UpgradeVersionBundle()
        {
            UpgradeVersionBundle( PLATFORM_DISPLAY_NAME_PC );
        }

        private static void UpgradeVersionBundle( string platform )
        {
            var incrementUpAt               = 9; //if this is set to 9, then 1.0.9 will become 1.1.0
            var versionBundleText           = PlayerSettings.bundleVersion;
            var androidVersionBundleCode    = PlayerSettings.Android.bundleVersionCode;
            var buildVersionPath            = BUILD_VERSION_PATH.Replace("{productName}", productName).Replace("{platform}", platform);

            try
            {
                using( var sr = OpenText( buildVersionPath ) )
                {
                    try
                    {
                        versionBundleText = sr.ReadLine();
                        androidVersionBundleCode = int.Parse( sr.ReadLine() );
                    }
                    catch { }
                }
            }
            catch { }

            if( string.IsNullOrEmpty( versionBundleText ) )
            {
                versionBundleText = "0.0.1";
            }
            else
            {
                versionBundleText = versionBundleText.Trim(); //clean up whitespace if necessary
                var lines = versionBundleText.Split('.');

                var majorVersion = 0;
                var minorVersion = 0;
                var subMinorVersion = 0;

                if( lines.Length > 0 )
                    majorVersion = int.Parse( lines[0] );
                if( lines.Length > 1 )
                    minorVersion = int.Parse( lines[1] );
                if( lines.Length > 2 )
                    subMinorVersion = int.Parse( lines[2] );

                subMinorVersion++;
                if( subMinorVersion > incrementUpAt )
                {
                    minorVersion++;
                    subMinorVersion = 0;
                }
                if( minorVersion > incrementUpAt )
                {
                    majorVersion++;
                    minorVersion = 0;
                }

                versionBundleText = majorVersion.ToString( "0" ) + "." + minorVersion.ToString( "0" ) + "." + subMinorVersion.ToString( "0" );

            }
            Log( "Version Incremented to " + versionBundleText );
            androidVersionBundleCode++;
            _lastVersion = versionBundleText;
            _lastAndroidBundleCode = androidVersionBundleCode;
            PlayerSettings.bundleVersion = versionBundleText;
            PlayerSettings.Android.bundleVersionCode = androidVersionBundleCode;
            Log( "Bundle Version Code Incremented To " + androidVersionBundleCode );

            WriteAllText( buildVersionPath, string.Empty );
            using( var sw = AppendText( buildVersionPath ) )
            {
                sw.WriteLine( versionBundleText );
                sw.WriteLine( androidVersionBundleCode );
            }
        }

        private static void UpdateRuntimeVersion()
        {
            var version = GetSettings<CISettings>();

            version.m_buildTime = $"{Now}";
            version.m_version = _lastVersion;
            version.m_androidBundleCode = _lastAndroidBundleCode;

            version.SaveAsset();
        }

        private static void PrepareWin64Package( string platform, string name, string version, bool developmentBuild )
        {

            var useIL = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Standalone) == ScriptingImplementation.IL2CPP;
            
            var prefix                 = developmentBuild ? DEVELOPMENT_BUILD_PREFIX : RELEASE_BUILD_PREFIX;
            var doNotShipBurstName          = $"{name}{DO_NOT_SHIP_BURST_SUFFIX}";
            var doNotShipILName             = $"{name}{DO_NOT_SHIP_IL_SUFFIX}";
            var batRelativeBuildPath   = BUILD_PATH.Replace("..", ".");
            var fullName                    = $"{prefix}{name}_{platform}_{version}";
            var path                        = $"{batRelativeBuildPath}\\{platform}\\{fullName}";
            var zipPath                     = $"{UPLOAD_PATH}\\{fullName}.zip";
            var doNotShipBurstPath          = $"{path}\\{doNotShipBurstName}";
            var doNotShipILPath             = $"{path}\\{doNotShipILName}";
            var burstIsolationPath          = $"{batRelativeBuildPath}\\tmp\\{platform}\\{prefix}\\{doNotShipBurstName}";
            var ILIsolationPath             = $"{batRelativeBuildPath}\\tmp\\{platform}\\{prefix}\\{doNotShipILName}";

            using( var sw = AppendText( BUILD_SLACK_MOVER_PATH ) )
            {
                var createBurstTmpIfNotExists    = $"if not exist \"{burstIsolationPath}\" mkdir \"{burstIsolationPath}\"";
                var createILTmpIfNotExists    = useIL ? $"if not exist \"{ILIsolationPath}\" mkdir \"{ILIsolationPath}\"" : "";
                var createUploadIfNotExists = $"if not exist \"{UPLOAD_PATH}\" mkdir \"{UPLOAD_PATH}\"";
                var isolateDoNotShipBurst   = $"move \"{doNotShipBurstPath}\" \"{burstIsolationPath}\"";
                var isolateDoNotShipIL      = useIL ? $"move \"{doNotShipILPath}\" \"{ILIsolationPath}\"" : "";
                var zipping                 = $"7z a -tzip \"{zipPath}\" \"{path}\\*\"";
                var recoverDoNotShipBurst   = $"move \"{burstIsolationPath}\" \"{doNotShipBurstPath}\"";
                var recoverDoNotShipIL      = useIL ? $"move \"{ILIsolationPath}\" \"{doNotShipILPath}\"" : "";

                sw.WriteLine( createBurstTmpIfNotExists );
                sw.WriteLine( createILTmpIfNotExists );
                sw.WriteLine( createUploadIfNotExists );
                sw.WriteLine( isolateDoNotShipBurst );
                sw.WriteLine( isolateDoNotShipIL );
                sw.WriteLine( zipping );
                sw.WriteLine( recoverDoNotShipBurst );
                sw.WriteLine( recoverDoNotShipIL );
            }

            var steamContentSubPath = developmentBuild ? "debug" : "release";
            var steamContentZipPath = $"{STEAM_CONTENT_BUILDER_PATH}\\{steamContentSubPath}\\{fullName}.zip";

            using( var sw = AppendText( BUILD_STEAM_MOVER_PATH ) )
            {
                var moveShip = $"copy \"{zipPath}\" \"{steamContentZipPath}\"";

                sw.WriteLine( moveShip );
            }
        }

        private static void PrepareAndroidPackage( string platform, string name, string version, bool developmentBuild )
        {

            var prefix                  = developmentBuild ? DEVELOPMENT_BUILD_PREFIX : RELEASE_BUILD_PREFIX;
            var batRelativeBuildPath    = BUILD_PATH.Replace("..", ".");
            var fullName                = $"{prefix}{name}_{platform}_{version}";
            var path                    = $"{batRelativeBuildPath}\\{platform}\\{fullName}\\{name}";
            var apkPath                 = $"{path}.apk";
            var copiedApkPath           = $"{UPLOAD_PATH}\\{fullName}.apk";

            using( var sw = AppendText( BUILD_SLACK_MOVER_PATH ) )
            {
                var createUploadIfNotExists = $"if not exist \"{UPLOAD_PATH}\" mkdir \"{UPLOAD_PATH}\"";
                var copyToUploadFolder      = $"copy \"{apkPath}\" \"{copiedApkPath}\"";

                sw.WriteLine( createUploadIfNotExists );
                sw.WriteLine( copyToUploadFolder );
            }
        }

        private static void PreparePS5Package( string platform, string name, string version, bool developmentBuild )
        {
            var prefix                  = developmentBuild ? DEVELOPMENT_BUILD_PREFIX : RELEASE_BUILD_PREFIX;
            var batRelativeBuildPath    = BUILD_PATH.Replace("..", ".");
            var fullName                = $"{prefix}{name}_{platform}_{version}";
            var path                    = $"{batRelativeBuildPath}\\{platform}\\{fullName}\\{name}";
            var zipPath                 = $"{UPLOAD_PATH}\\{fullName}.zip";

            using( var sw = AppendText( BUILD_SLACK_MOVER_PATH ) )
            {
                var createUploadIfNotExists = $"if not exist \"{UPLOAD_PATH}\" mkdir \"{UPLOAD_PATH}\"";
                var zipping                 = $"7z a -tzip \"{zipPath}\" \"{path}\\*\"";

                sw.WriteLine( createUploadIfNotExists );
                sw.WriteLine( zipping );
            }
        }

        private static void ClearSlackMover()
        {
            WriteAllText( BUILD_SLACK_MOVER_PATH, string.Empty );
        }

        private static void ClearSteamMover()
        {
            WriteAllText( BUILD_STEAM_MOVER_PATH, $"del /s /q {STEAM_CONTENT_BUILDER_PATH}\n" );
        }

        #endregion


        #region Private

        private static StreamWriter _sw;
        private static string _lastVersion;
        private static int _lastAndroidBundleCode;

        #endregion
    }
}