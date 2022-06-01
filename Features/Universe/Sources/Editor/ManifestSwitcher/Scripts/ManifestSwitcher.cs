using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Universe.Editor
{
    public class ManifestSwitcher : IActiveBuildTargetChanged
    {
        #region Unity API
        
        public int callbackOrder { get; }


        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            Events.registeredPackages += UpdateCurrentBufferedManifest;
        }
        
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            UpdateManifests( previousTarget, newTarget );
        }

        #endregion


        #region Main

        public static void UpdateCurrentBufferedManifest( PackageRegistrationEventArgs packageRegistrationEventArgs )
        {
            var currentTarget = EditorUserBuildSettings.activeBuildTarget;

            UpdateManifests( currentTarget, currentTarget );
        }

        public static void UpdateManifests( BuildTarget previousTarget, BuildTarget newTarget )
        {
            InitializePackageVariant();
            SavePreviousManifestToVariant( previousTarget );
            LoadNewManifestFromVariant( newTarget );
        }
        
        public static void InitializePackageVariant()
        {
            var packageFolderPath = $"{Application.dataPath}/../Packages";
            var manifestFilePath = $"{packageFolderPath}/manifest.json";
            var platformSpecificFolderPath = $"{Application.dataPath}/../Packages/PlatformSpecific/";

            Directory.CreateDirectory(platformSpecificFolderPath);
            
            var originalManifestContent = File.ReadAllLines(manifestFilePath);
            
            foreach (var target in (BuildTarget[]) Enum.GetValues(typeof(BuildTarget)))
            {
                var currentManifestPath = $"{platformSpecificFolderPath}/{target}-manifest.json";
                if (!File.Exists(currentManifestPath))
                {
                    using(var sw = File.AppendText(currentManifestPath))
                    {
                        foreach (var line in originalManifestContent)
                        {
                            sw.Write(line);
                            sw.Write("\r");
                        }
                    }
                }
            }
        }

        private static void SavePreviousManifestToVariant(BuildTarget previousBuildTarget)
        {
            var manifestFilePath = $"{Application.dataPath}/../Packages/manifest.json";
            var manifestVariantFilePath = $"{Application.dataPath}/../Packages/PlatformSpecific/{previousBuildTarget}-manifest.json";
            
            File.WriteAllText(manifestVariantFilePath, string.Empty);
            var originalManifestContent = File.ReadAllLines(manifestFilePath);
            
            using(var sw = File.AppendText(manifestVariantFilePath))
            {
                foreach (var line in originalManifestContent)
                {
                    sw.Write(line);
                    sw.Write("\r");
                }
            }
        }

        private static void LoadNewManifestFromVariant(BuildTarget newTarget)
        {
            var manifestFilePath = $"{Application.dataPath}/../Packages/manifest.json";
            var manifestVariantPath = $"{Application.dataPath}/../Packages/PlatformSpecific/{newTarget}-manifest.json";
            File.WriteAllText(manifestFilePath, string.Empty);
            var variantManifestContent = File.ReadAllLines(manifestVariantPath);
            
            using(var sw = File.AppendText(manifestFilePath))
            {
                foreach (var line in variantManifestContent)
                {
                    sw.Write(line);
                    sw.Write("\r");
                }
            }
        }

        #endregion
    }
}