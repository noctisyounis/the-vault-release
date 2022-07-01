//
// URP Debug Views for Unity
// (c) 2019 PH Graphics
// Source code may be used and modified for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset from a pirate site, please consider buying it from the Unity asset store. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.WSA;
using Application = UnityEngine.Application;

namespace URPDebugViews
{
    [InitializeOnLoad]
    public class GettingStartedWindowShow
    {
        private const string SettingsPath = "Assets/Plugins/URPDebugViews/Settings.asset";
        static GettingStartedWindowShow()
        {
            var settings = AssetDatabase.LoadAssetAtPath(SettingsPath, typeof(URPDebugViewsSettings)) as URPDebugViewsSettings;
            bool showWindow = !(settings && !settings.OpenGettingStartedWindow);
            if (showWindow)
            {
                EditorApplication.update += OnUpdate;

            }
        }

        private static void OnUpdate()
        {
            EditorApplication.update -= OnUpdate;
            
            URPDebugViewsSettings newSettings = ScriptableObject.CreateInstance<URPDebugViewsSettings>();
            AssetDatabase.CreateAsset(newSettings, SettingsPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            GettingStartedWindow window = (GettingStartedWindow)EditorWindow.GetWindow(typeof(GettingStartedWindow));
            window.Show();
        }
    }
    
    
    public class GettingStartedWindow : EditorWindow
    {
        private const string URPDebugViewsVersion = "1.0.4";
        
        private const string URPDebugViewsPath = "Assets/Plugins/URPDebugViews";
        private const string URPDebugViewsLogoFileName = "Textures/URP_Icon_Image_160x160.png";
        private const string ManualFileName = "URPDebugViews_Documentation.pdf";
        private const string ChangeLogName = "ChangeLog.txt";

        private GUIStyle _wrapLabelStyle;

        private Texture2D _logoTexture;

        private bool _hasError;
        
        // various sizes
        private const int LogoTextureSize = 128;
        private const int Margin = 10;
        private const int ButtonHeight = 30;
        private const int ButtonWidth = 120;
        private const int LargeButtonWidth = 160;
        
        private Vector2 _scrollPosition = Vector2.zero;
        private Vector2 _defaultWindowSize = Vector2.zero;

        private bool _initialized = false;
        
        [MenuItem("Window/Debug Views/Getting Started")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            GettingStartedWindow window = (GettingStartedWindow)EditorWindow.GetWindow(typeof(GettingStartedWindow));
            window.Show();
        }

        private bool GetInternalFile(string pathFromURPDebugViews, out string fullPath)
        {
            fullPath = Path.Combine(URPDebugViewsPath, pathFromURPDebugViews);
            if (File.Exists(fullPath)) 
                return true;
            
            Debug.LogError("File " + fullPath + " doesn't exist. Did you move the URP Debug Views root folder from Assets/Plugins ? Unfortunately this isn't supported yet.");
            _hasError = true;
            return false;
        }

        private void Awake()
        {
            string fullLogoPath;
            if (!GetInternalFile(URPDebugViewsLogoFileName, out fullLogoPath))
            {
                return;
            }

            _logoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(fullLogoPath);
        }

        private bool OpenFileWithDefaultEditor(string path)
        {
            string fullPath;
            if (!GetInternalFile(path, out fullPath))
            {
                return false;
            }

            fullPath = Path.GetFullPath(fullPath);

            if (!File.Exists(fullPath))
            {
                Debug.LogError("File " + fullPath + " doesn't exist. Did you move the URPDebugViews root folder from Assets/ ? Unfortunately this isn't supported yet.");
                _hasError = true;
                return false;
            }
            
#if UNITY_EDITOR_WIN
            System.Diagnostics.Process.Start($@"{fullPath}");
#elif UNITY_EDITOR_OSX
            EditorUtility.RevealInFinder($@"{fullPath}");
#endif

            return true;
        }

        private bool CheckForErrors()
        {
            if (_hasError)
            {
                GUILayout.Label(
                    "There was an error constructing this window. Please check your console for errors. If you can't fix it, please don't hesitate to ask for support");
                return true;
            }

            return false;
        }

        private void InitializeWindow()
        {
            titleContent.text = "Getting Started";
            minSize = new Vector2(520, 450);
            
            _defaultWindowSize = new Vector2(520, 450);
            Vector2 initialPosition = 0.5f * (new Vector2(Screen.currentResolution.width, Screen.currentResolution.height) - _defaultWindowSize);
            position = new Rect(initialPosition, _defaultWindowSize);
            
            _wrapLabelStyle = new GUIStyle(EditorStyles.label) {wordWrap = true};
        }

        void OnGUI()
        {
            if (!_initialized)
            {
                InitializeWindow();
                _initialized = true;
            }

            if (CheckForErrors()) return;

            _scrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), _scrollPosition,
                new Rect(0, 0, _defaultWindowSize.x - 10, _defaultWindowSize.y - 10), false, false);

            float yOffset = 0;
            float defaultXSize = position.width - Margin - Margin;
            
            // Header
            GUI.BeginGroup(new Rect(Margin, Margin, defaultXSize, LogoTextureSize));

            float xOffset = 0;
            GUI.DrawTexture(new Rect(xOffset, 0, LogoTextureSize, LogoTextureSize), _logoTexture);
            xOffset += LogoTextureSize + Margin;
            
            GUI.Label(new Rect(xOffset, 0, 100, 30), "Version: " + URPDebugViewsVersion);
            if (GUI.Button(new Rect(xOffset, 30, ButtonWidth, ButtonHeight), "View Changelog"))
            {
                OpenChangelog();
                if (CheckForErrors()) return;
            }
            
            if (GUI.Button(new Rect(xOffset, 30 + ButtonHeight + Margin, ButtonWidth, ButtonHeight), "View Manual"))
            {
                OpenManual();
                if (CheckForErrors()) return;
            }

            xOffset += ButtonWidth + Margin;
            
            if (GUI.Button(new Rect(xOffset, 30, LargeButtonWidth, ButtonHeight), "View Offline Changelog"))
            {
                OpenFileWithDefaultEditor(ChangeLogName);
                if (CheckForErrors()) return;
            }
            
            if (GUI.Button(new Rect(xOffset, 30 + ButtonHeight + Margin, LargeButtonWidth, ButtonHeight), "View Offline Manual"))
            {
                OpenFileWithDefaultEditor(ManualFileName);
                if (CheckForErrors()) return;
            }
            
            GUI.EndGroup();

            yOffset += Margin + LogoTextureSize; 
            
            GUI.Label(new Rect(Margin, yOffset + Margin, defaultXSize, 10), "", GUI.skin.horizontalSlider);

            yOffset += Margin + Margin;
            
            // Getting started title
            yOffset += Margin;
            GUI.BeginGroup(new Rect(Margin, yOffset, defaultXSize, 20));
            
            GUI.Label(new Rect(defaultXSize / 2f - 50, 0, 100, 20), "Getting Started", EditorStyles.largeLabel);
            
            GUI.EndGroup();

            yOffset += Margin + 20;
            
            // Setup section
            yOffset += Margin;
            GUI.BeginGroup(new Rect(Margin, yOffset, defaultXSize, 140));
            
            GUI.Label(new Rect(0, 0, defaultXSize, 20), "To properly setup the plugin, there is one manual step. ", _wrapLabelStyle);
            
            GUI.Label(new Rect(0, 30, defaultXSize, 30), "You have to add a \"Debug View Feature\" pass to the Renderer feature list in your Forward renderer.", _wrapLabelStyle);
            
            GUI.Label(new Rect(0, 70, defaultXSize, 30), "Screenshots and more explanation are available in the setup documentation.", _wrapLabelStyle);

            if (GUI.Button(new Rect(0, 110, defaultXSize, ButtonHeight), "Open setup documentation"))
            {
                OpenSetup();
            }

            GUI.EndGroup();

            yOffset += 140;

            // Help title
            yOffset += Margin;
            
            GUI.Label(new Rect(Margin, yOffset, defaultXSize, 10), "", GUI.skin.horizontalSlider);
            
            yOffset += Margin + 10;

            GUI.BeginGroup(new Rect(Margin, yOffset, defaultXSize, 30));
            
            GUI.Label(new Rect(defaultXSize / 2f - 15, 0, 100, 20), "Help", EditorStyles.largeLabel);
            
            GUI.EndGroup();

            yOffset += Margin + 20;
            
            // Help
            GUI.BeginGroup(new Rect(Margin, yOffset, defaultXSize, ButtonHeight));
            
            if (GUI.Button(new Rect(0, 0, defaultXSize / 2 - Margin, ButtonHeight), "Discord"))
            {
                OpenDiscordHelp();
            }
            
            if (GUI.Button(new Rect(defaultXSize / 2 + Margin, 0, defaultXSize / 2 - Margin, ButtonHeight), "Email"))
            {
                OpenEmailHelp();
            }
            
            
            GUI.EndGroup();
            
            GUI.EndScrollView();
        }

        private void OpenEmailHelp()
        {
            Application.OpenURL("mailto:ph.graphics.unity@gmail.com");
        }

        private void OpenDiscordHelp()
        {
            Application.OpenURL("https://discord.gg/ksURBah");
        }

        private void OpenManual()
        {
            Application.OpenURL("http://assetstore.phbarralis.com/urpdebugviews/features.html");
        }

        private void OpenChangelog()
        {
            Application.OpenURL("http://assetstore.phbarralis.com/urpdebugviews/changelog.html");
        }
        
        private void OpenSetup()
        {
            Application.OpenURL("http://assetstore.phbarralis.com/urpdebugviews/getting_started.html#setup");
        }
    }
}
