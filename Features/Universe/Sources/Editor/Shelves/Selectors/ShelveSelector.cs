using System;
using UnityEditor;

using static UnityEngine.GUILayout;
using static UnityEngine.PlayerPrefs;

namespace Universe.Toolbar.Editor
{
    public enum SHELVE
    {
        Files,
        Database,
        Graphics,
        Level,
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
            currentShelve = (SHELVE)EditorGUILayout.EnumPopup("", currentShelve, Width(100));
            SetPlayerPrefShelve(currentShelve, side);

            switch (currentShelve)
            {
                case SHELVE.Files:
                    DrawFiles();
                    break;
                case SHELVE.Database:
                    DrawDatabase();
                    break;
                case SHELVE.Graphics:
                    DrawGraphics();
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
                currentShelve = (SHELVE)Enum.Parse(typeof(SHELVE), GetString( side + _shelvePlayerPrefKey) );
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

        static void DrawDatabase()
        {
            OpenAddressableButton.Draw();
            ReloadAndBuildAddressable.Draw();
        }

        static void DrawGraphics()
        {
            GraphicTierChanger.Draw();
        }

        static void DrawLevel()
        {
            SelectStartingLevel.Draw();
            OpenLevel.Draw();
            SwitchLevel.Draw();
            AddTaskToLevel.Draw();
            CreateLevelButton.Draw();
        }

        private static string _shelvePlayerPrefKey = "shelve";
    }
}