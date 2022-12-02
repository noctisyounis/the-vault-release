using System;
using UnityEditor;
using UnityEngine;
using Universe.Editor;

using static UnityEngine.GUILayout;
using static UnityEngine.PlayerPrefs;
using static Universe.SceneTask.Runtime.Environment;

namespace Universe.Toolbar.Editor
{
    public enum SHELVE
    {
        Files,
        Addressable,
        Localisation,
        Graphics,
        Level
    }

    public enum SIDE
    {
        Left,
        Right
    }
    
    public class ShelveSelector
    {
        public static void Draw(SIDE side)
        {
            FlexibleSpace();

            var currentShelve = GetPlayerPrefShelveOrDefault(side);
            
            currentShelve = (SHELVE)EditorGUILayout.EnumPopup("", currentShelve, Width(130));
            SetPlayerPrefShelve(currentShelve, side);

            switch (currentShelve)
            {
                case SHELVE.Files:
                    DrawFiles();
                    break;
                case SHELVE.Addressable:
                    DrawAddressable();
                    break;
                case SHELVE.Graphics:
                    DrawGraphics();
                    break;
                case SHELVE.Localisation:
                    DrawLocalisation();
                    break;
                case SHELVE.Level:
                    DrawLevel();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        static SHELVE GetPlayerPrefShelveOrDefault(SIDE side)
        {
            var currentShelve = SHELVE.Files;
            var currentSide = side.ToString();
			
            if (HasKey(side + _shelvePlayerPrefKey))
            {
                try
                {
                    currentShelve = (SHELVE)Enum.Parse( typeof( SHELVE ), GetString( side + _shelvePlayerPrefKey ) );
                }
                catch
                {}
            }

            return currentShelve;
        }

        static void SetPlayerPrefShelve(SHELVE currentShelve, SIDE side)
        {
            var currentSide = side.ToString();
            
            SetString(side + _shelvePlayerPrefKey, currentShelve.ToString());
        }

        static void DrawFiles()
        {
            SymlinkButtons.Draw();
        }

        static void DrawAddressable()
        {
            OpenAddressableButton.Draw();
            ReloadAndBuildAddressable.Draw();
            ClearAddressable.Draw();
        }

        static void DrawLocalisation()
        {
            RefreshLocalisation.Draw();
        }

        static void DrawGraphics()
        {
            GraphicTierChanger.Draw();
        }

        static void DrawLevel()
        {
            SelectLevel.Draw("Current", "EditorLevelPath", false);
            CreateLevelButton.Draw();
            AddSituationToLevel.Draw();
        }

        private static string _shelvePlayerPrefKey = "shelve";
    }
}