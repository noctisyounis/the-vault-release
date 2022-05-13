using System;
using System.Collections;
using System.Collections.Generic;
using Symlink.Editor;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;
using Universe;
using Universe.Editor;

using static System.IO.Directory;
using static UnityEditor.AddressableAssets.Settings.AddressableAssetSettings;
using static UnityEngine.Application;
using static Universe.Editor.UGroupHelper;

using Debug = UnityEngine.Debug;


namespace Universe.Editor
{
    public class DeploymentEditor : AssetPostprocessor, IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        #region Constant

        //Absolute paths
        private const string STEAM_CONTENT_BUILDER_PATH = "D:\\steamworks_sdk_153a\\sdk\\tools\\ContentBuilder\\content";

        //Project relative
        private const string TASK_GAME_STARTER_PATH = "Assets\\_\\GameStarter\\GameStarter.unity";
        private const string BUILD_PATH = "..\\Builds";
        private const string BUILD_LOG_PATH = "..\\BuildLog.txt";
        private const string BUILD_VERSION_PATH = "..\\..\\{Application.productName}_LastBuildVersion.txt";
        private const string BUILD_ZIPPER_PATH = "..\\Jenkins_Slack_Uploader.bat";
        private const string BUILD_STEAM_MOVER_PATH = "..\\MoveBuildToSteam.bat";

        //.bat relative
        private const string UPLOAD_PATH = ".\\SlackUpload";

        //static names
        private const string DEVELOPMENT_BUILD_PREFIX = "[DEV]";
        private const string RELEASE_BUILD_PREFIX = "[Release]";
        private const string DO_NOT_SHIP_SUFFIX = "_BurstDebugInformation_DoNotShip";

        private const string PLATFORM_DISPLAY_NAME_PC = "Win64";
        private const string PLATFORM_DISPLAY_NAME_ANDROID = "Oculus";

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

        [MenuItem("Vault/CI/Build PC")]
        public static void BuildJenkinsTest()
        {
            _sw = File.AppendText(BUILD_LOG_PATH);
            _sw.WriteLine($"[{DateTime.Now}] PC Build Requested");
            UpgradeVersionBundle();
            _sw.WriteLine($"[{DateTime.Now}] Editor Version Updated");
            UpdateRuntimeVersion();
            _sw.WriteLine($"[{DateTime.Now}] Runtime Version Updated");

            CleanPlayerContent();

            var directories = GetDirectories(SourceDirectoryPath);
            var graphicsDirectories = GetDirectories(SourceGraphicsTiersDirectoryPath);

            SymlinkEditor.LoadAllSymlink(graphicsDirectories, TargetGraphicsTiersDirectoryPath);
            OnRefreshCompleted += StartWin64Builds;
            ReloadAndBuildAddressable.Execute();
        }

        private static void StartWin64Builds()
        {
            var target = BuildTarget.StandaloneWindows64;

            OnRefreshCompleted -= StartWin64Builds;
            _sw.WriteLine($"[{DateTime.Now}] PC Builds Pending");

            ClearZipper();
            ClearSteamMover();

            StartBuild(target, true);
            StartBuild(target, false);

            //TryStartNextBuild();

            var graphicsDirectories = GetDirectories(SourceGraphicsTiersDirectoryPath);

            _sw.WriteLine($"[{DateTime.Now}] Build Finished");
            Debug.Log($"[{DateTime.Now}] Build Finished");

            SymlinkEditor.RemoveAllSymlinks(graphicsDirectories, TargetGraphicsTiersDirectoryPath);
            _sw.WriteLine($"[{DateTime.Now}] Symlink unloaded");
            _sw.WriteLine($"[{DateTime.Now}] Stream Closed");
            _sw.WriteLine($"------------------------------------------------------------");
            _sw.Close();
        }

        private static bool TryStartNextBuild()
        {
            var count = _pendingBuilds.Count;
            if (count == 0) return false;

            var build = _pendingBuilds[0];
            build.Invoke();

            _pendingBuilds.Remove(build);

            return true;
        }

        private static void StartBuild(BuildTarget target, bool developmentBuild)
        {
            var targetName = TargetNameConverter(target);
            var version = PlayerSettings.bundleVersion;
            var name = productName;
            var prefix = developmentBuild ? DEVELOPMENT_BUILD_PREFIX : RELEASE_BUILD_PREFIX;
            var fullName = $"{prefix}{name}_{targetName}_{version}";
            var folderPath = $"{BUILD_PATH}\\{targetName}\\{fullName}";
            var option = new BuildPlayerOptions
            {
                scenes = new string[] { TASK_GAME_STARTER_PATH },
                locationPathName = $"{folderPath}\\{name}.exe",
                target = target,
                options = developmentBuild ? BuildOptions.Development : 0
            };

            PreparePackage(targetName, name, version, developmentBuild);

            _sw.WriteLine($"[{DateTime.Now}] {fullName} Build Started");

            BuildPipeline.BuildPlayer(option);
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            var summary = report.summary;
            var platform = summary.platform;
            var path = summary.outputPath;
            var isDevelopment = (summary.options & BuildOptions.Development) != 0;

            _sw.WriteLine($"[{DateTime.Now}] Preparing build for {platform} in {path} for {(isDevelopment ? "Development" : "Release")}");
            Debug.Log($"[{DateTime.Now}] Preparing build for {platform} in {path} for {(isDevelopment ? "Development" : "Release")}");
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            var deltaTime = report.summary.buildEndedAt - report.summary.buildStartedAt;

            EditorCoroutineUtility.StartCoroutineOwnerless(WaitForNextBuild());
        }

        private static IEnumerator WaitForNextBuild()
        {
            while (BuildPipeline.isBuildingPlayer)
            {
                yield return 0;
            }
        }

        #endregion


        #region Utils

        private static string TargetNameConverter(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneWindows64: return PLATFORM_DISPLAY_NAME_PC;
                case BuildTarget.Android: return PLATFORM_DISPLAY_NAME_ANDROID;
                default: return target.ToString();
            }
        }

        [MenuItem("Vault/Build/Upgrade Version Bundle")]
        private static void UpgradeVersionBundle()
        {
            var incrementUpAt = 9; //if this is set to 9, then 1.0.9 will become 1.1.0
            var versionBundleText = PlayerSettings.bundleVersion;
            var androidVersionBundleCode = PlayerSettings.Android.bundleVersionCode;
            var buildVersionPath = BUILD_VERSION_PATH.Replace("{Application.productName}", productName);

            try
            {
                using (var sr = File.OpenText(buildVersionPath))
                {
                    try
                    {
                        versionBundleText = sr.ReadLine();
                        androidVersionBundleCode = int.Parse(sr.ReadLine());
                    }
                    catch (Exception e) { }
                }
            }
            catch (Exception e) { }

            if (string.IsNullOrEmpty(versionBundleText))
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

                if (lines.Length > 0) majorVersion = int.Parse(lines[0]);
                if (lines.Length > 1) minorVersion = int.Parse(lines[1]);
                if (lines.Length > 2) subMinorVersion = int.Parse(lines[2]);

                subMinorVersion++;
                if (subMinorVersion > incrementUpAt)
                {
                    minorVersion++;
                    subMinorVersion = 0;
                }
                if (minorVersion > incrementUpAt)
                {
                    majorVersion++;
                    minorVersion = 0;
                }

                versionBundleText = majorVersion.ToString("0") + "." + minorVersion.ToString("0") + "." + subMinorVersion.ToString("0");

            }
            Debug.Log("Version Incremented to " + versionBundleText);
            _sw.WriteLine($"[{DateTime.Now}] Version Incremented to {versionBundleText}");

            androidVersionBundleCode++;

            _lastVersion = versionBundleText;
            _lastAndroidBundleCode = androidVersionBundleCode;
            PlayerSettings.bundleVersion = versionBundleText;
            PlayerSettings.Android.bundleVersionCode = androidVersionBundleCode;

            _sw.WriteLine($"[{DateTime.Now}] Bundle Version Incremented to {androidVersionBundleCode}");
            Debug.Log("Bundle Version Code Incremented To " + androidVersionBundleCode);

            File.WriteAllText(buildVersionPath, string.Empty);
            using (var sw = File.AppendText(buildVersionPath))
            {
                sw.WriteLine(versionBundleText);
                sw.WriteLine(androidVersionBundleCode);
            }
        }

        private static void UpdateRuntimeVersion()
        {
            var version = USettingsHelper.GetSettings<CISettings>();

            version.m_buildTime = $"{DateTime.Now}";
            version.m_version = _lastVersion;
            version.m_androidBundleCode = _lastAndroidBundleCode;

            version.Save();
        }

        public static void PreparePackage(string platform, string name, string version, bool developmentBuild)
        {

            var prefix = developmentBuild ? DEVELOPMENT_BUILD_PREFIX : RELEASE_BUILD_PREFIX;
            var doNotShipName = $"{name}{DO_NOT_SHIP_SUFFIX}";

            var batRelativeBuildPath = BUILD_PATH.Replace("..", ".");
            var fullName = $"{prefix}{name}_{platform}_{version}";
            var path = $"{batRelativeBuildPath}\\{platform}\\{fullName}";
            var zipPath = $"{path}.zip";
            var copiedZipPath = $"{UPLOAD_PATH}\\{fullName}.zip";
            var doNotShipPath = $"{path}\\{doNotShipName}";
            var isolationPath = $"{batRelativeBuildPath}\\tmp\\{prefix}\\{doNotShipName}";

            using (var sw = File.AppendText(BUILD_ZIPPER_PATH))
            {
                var createTmpIfNotExists = $"if not exist \"{isolationPath}\" mkdir \"{isolationPath}\"";
                var isolateDoNotShip = $"move \"{doNotShipPath}\" \"{isolationPath}\"";
                var zipping = $"7z a -tzip \"{zipPath}\" \"{path}\"";
                var recoverDoNotShip = $"move \"{isolationPath}\" \"{doNotShipPath}\"";
                var createUploadIfNotExists = $"if not exist \"{UPLOAD_PATH}\" mkdir \"{UPLOAD_PATH}\"";
                var copyToUploadFolder = $"copy \"{zipPath}\" \"{copiedZipPath}\"";

                sw.WriteLine(createTmpIfNotExists);
                sw.WriteLine(isolateDoNotShip);
                sw.WriteLine(zipping);
                sw.WriteLine(recoverDoNotShip);
                sw.WriteLine(createUploadIfNotExists);
                sw.WriteLine(copyToUploadFolder);
            }

            if (developmentBuild) return;

            var steamContentZipPath = $"{STEAM_CONTENT_BUILDER_PATH}\\{fullName}.zip";

            using (var sw = File.AppendText(BUILD_STEAM_MOVER_PATH))
            {
                var moveShip = $"copy {copiedZipPath} {steamContentZipPath}";

                sw.WriteLine(moveShip);
            }
        }

        public static void ClearZipper()
        {
            File.WriteAllText(BUILD_ZIPPER_PATH, string.Empty);
        }

        public static void ClearSteamMover()
        {
            File.WriteAllText(BUILD_STEAM_MOVER_PATH, string.Empty);
        }

        #endregion


        #region OLD

        public static void BuildPicoReady()
        {
            // Scene
            //  DisableStoreAndLIVManagers();
            // Enable(OCULUS_MANAGER_NAME);

            // Publishing      
            ApplyDRM();
            UpgradeVersionBundle();
            ApplyPatchToForwardRenderer();

            // Build
            BuildPico();
        }


        public static void ReplaceAndroidManifest(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget != BuildTarget.Android) return;
            Debug.Log("AndroidManifest swapped with Pico");
            var manifestPath = Application.dataPath + "/../Temp/StagingArea/AndroidManifest.xml";
            var customManifestPath = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";

            FileUtil.ReplaceFile(customManifestPath, manifestPath);
        }

        public static void BuildRiftReady()
        {
            // Scene
            DisableStoreAndLIVManagers();
            Enable(OCULUS_MANAGER_NAME);

            // Publishing
            UpgradeVersionBundle();

            // Build
            BuildPC();

            // Post Build
            RemoveDllForOculusStore();
        }


        public static void BuildSteamReady()
        {
            // Scene
            DisableStoreAndLIVManagers();
            Enable(STEAM_MANAGER_NAME);
            Enable(LIV_MANAGER_NAME);

            // Publishing
            UpgradeVersionBundle();

            // Build
            BuildPC();
        }

        public static void BuildAll()
        {
            BuildPicoReady();
            BuildRiftReady();
            BuildSteamReady();
        }

        #endregion


        #region Upload

        private static void UploadQuestToIkimashoInternal()
        {
            var process = new Process
            {
                StartInfo =
            {
                  FileName = "powershell.exe",
                  Arguments =
                        Environment
                              .CurrentDirectory + "\\Assets\\Oculus\\VR\\Editor\\Tools\\ovr-platform-util.exe"
                                                + " upload-quest-build --app-id 3043045549099845 --app_secret 45c03e32c4d69b42cc668abf57103ae4 --channel IkimashoInternal --apk "
                                                + GetLastBuildPath(),
                  RedirectStandardOutput = false,
                  RedirectStandardError = false,
                  UseShellExecute = false
            }
            };

            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            Debug.Log("output " + output);
            Debug.Log("error" + error);
            process.WaitForExit();
            process.Close();
        }

        private static void UploadRiftBuildToOculusStore()
        {
            var process = new Process
            {
                StartInfo =
            {
                  FileName = "powershell.exe",
                  Arguments =
                        Environment
                              .CurrentDirectory + "\\Assets\\Oculus\\VR\\Editor\\Tools\\ovr-platform-util.exe"
                                                + " upload-rift-build --app-id 2067955279976047 --app_secret ca6b1c4ab0cda7f4577841e3b93ed02b --channel IkimashoInternal --apk "
                                                + GetLastBuildPath(),
                  RedirectStandardOutput = false,
                  RedirectStandardError = false,
                  UseShellExecute = false
            }
            };

            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            Debug.Log("output " + output);
            Debug.Log("error" + error);
            process.WaitForExit();
            process.Close();
        }

        #endregion


        #region Build And Upload

        public static void BuildQuestAndUploadToOculusStore()
        {
            BuildPicoReady();
            UploadQuestToIkimashoInternal();
        }

        public static void BuildRiftAndUploadToOculusStore()
        {
            BuildRiftReady();
            UploadRiftBuildToOculusStore();
        }

        #endregion


        #region Tools

        private static string GetLastBuildPath()
        {
            var path = Environment.CurrentDirectory + "\\Builds\\";
            var fileInfo = Directory.GetFiles(path)
               .Select(x => new FileInfo(x))
               .OrderByDescending(x => x.LastWriteTime)
               .Take(1)
               .ToArray();

            return fileInfo[0] != null ? fileInfo[0].FullName : "";
        }

        private static void ApplyDRM()
        {
            PlayerSettings.Android.keystorePass = "C0rporate!";
            PlayerSettings.Android.keyaliasName = "key0";
            PlayerSettings.Android.keyaliasPass = "C0rporate!KEY";
        }

        private static void ApplyPatchToForwardRenderer()
        {
            var forwardRendererPath = "Packages/com.unity.render-pipelines.universal/Runtime/ForwardRenderer.cs";
            var patch = "return false;";
            var lineToEdit = 531;

            var arrLine = File.ReadAllLines(forwardRendererPath);
            arrLine[lineToEdit - 1] = "return false;";
            arrLine[lineToEdit] = "";

            File.WriteAllLines(forwardRendererPath, arrLine);
        }


        public static void RemoveDllForOculusStore()
        {
            var pathToOpenVR = "Builds\\StarShaman_" + Enum.GetName(typeof(BuildTarget), EditorUserBuildSettings.activeBuildTarget) + "_" + PlayerSettings.bundleVersion + "\\StarShaman_Data\\Plugins\\openvr_api.dll";

            File.Delete(pathToOpenVR);

            var pathToSteamApi64 = "Builds\\StarShaman_" + Enum.GetName(typeof(BuildTarget), EditorUserBuildSettings.activeBuildTarget) + "_" + PlayerSettings.bundleVersion + "\\StarShaman_Data\\Plugins\\steam_api64.dll";

            File.Delete(pathToSteamApi64);

            var pathToVivePort = "Builds\\StarShaman_" + Enum.GetName(typeof(BuildTarget), EditorUserBuildSettings.activeBuildTarget) + "_" + PlayerSettings.bundleVersion + "\\StarShaman_Data\\Plugins\\viveport_api64.dll";

            File.Delete(pathToVivePort);

            var pathToLIV = "Builds\\StarShaman_" + Enum.GetName(typeof(BuildTarget), EditorUserBuildSettings.activeBuildTarget) + "_" + PlayerSettings.bundleVersion + "\\StarShaman_Data\\Plugins\\LIV_MR.dll";

            File.Delete(pathToLIV);
        }


        private static void BuildPico()
        {
            Build(BuildTarget.Android, ".apk");
        }

        private static void BuildPC()
        {
            Build(BuildTarget.StandaloneWindows64, ".exe");
        }

        private static void Build(BuildTarget buildTarget, string fileExtention)
        {
            var path = "Builds/TigerBlade" + "_" + Enum.GetName(typeof(BuildTarget), buildTarget) + "_" + PlayerSettings.bundleVersion;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Game.unity" },
                locationPathName = path + "/StarShaman" + fileExtention,
                target = buildTarget,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;

            //System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }
        }

        #endregion


        #region ActivationState

        private static void Enable(string target)
        {
            ChangeActivationStateOf(target, true);
        }

        private static void Disable(string target)
        {
            ChangeActivationStateOf(target, false);
        }

        private static void DisableStoreAndLIVManagers()
        {
            Disable(STEAM_MANAGER_NAME);
            Disable(OCULUS_MANAGER_NAME);
            Disable(LIV_MANAGER_NAME);
        }

        private static void ChangeActivationStateOf(string target, bool status)
        {
            var managers = GameObject.Find(MANAGERS_PARENT_NAME);
            managers.transform.GetComponentsInChildren<Transform>(true)
                  .FirstOrDefault(t => t.name == target)
                  ?.gameObject.SetActive(status);
        }

        #endregion


        #region Private

        private static StreamWriter _sw;
        private static string _lastVersion;
        private static int _lastAndroidBundleCode;

        private static List<Action> _pendingBuilds = new();

        #endregion
    }
}