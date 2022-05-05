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
        Database,
        Graphics,
        Level_Management,
        Level_Loading
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
                case SHELVE.Level_Management:
                    DrawLevelManagement();
                    break;
                case SHELVE.Level_Loading:
                    DrawLevelLoading();
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
                {
                    currentShelve = SHELVE.Database;
                }
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
            RefreshLocalisation.Draw();
        }

        static void DrawGraphics()
        {
            GraphicTierChanger.Draw();
        }

        static void DrawLevelManagement()
        {
            if( SelectLevel.Draw( "Play: ", "PlaymodeLevelPath", true ) )
            {
                SelectTask.Draw( " On: ", "PlaymodeLevelPath", true );
            }
            CreateLevelButton.Draw();
            AddTaskToLevel.Draw();
        }

        static void DrawLevelLoading()
        {
            ToggleEnvironment.Draw( "EditorLevelPath", BLOCK_MESH );
            ToggleEnvironment.Draw( "EditorLevelPath", ART );
            OpenLevel.Draw();
        }

        private static string _shelvePlayerPrefKey = "shelve";
    }
}