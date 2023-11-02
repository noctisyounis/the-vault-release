using System;
using UnityEditor;
using Universe.Editor;

using static UnityEngine.GUILayout;
using static UnityEngine.PlayerPrefs;
using static Universe.Editor.UPrefs;

namespace Universe.Toolbar.Editor
{
    public enum SHELVE
    {
        None,
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
        #region Main

        public static void Draw(SIDE side)
        {
            FlexibleSpace();

            var currentShelve = GetPlayerPrefShelveOrDefault(side);
            
            currentShelve = (SHELVE)EditorGUILayout.EnumPopup("", currentShelve, Width(130));
            SetPlayerPrefShelve(currentShelve, side);

            switch (currentShelve)
            {
                case SHELVE.None:
                    break;
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
        
        #endregion


        #region Utils

        private static string GetPlayerPrefKey(SIDE side) =>
            EDITOR_SHELF.Replace(REPLACABLE, side.ToString());


        private static SHELVE GetPlayerPrefShelveOrDefault(SIDE side)
        {
            var currentShelve = SHELVE.Files;
            var currentKey = GetPlayerPrefKey(side);

            if (!HasKey(currentKey)) return currentShelve;
            try
            {
                currentShelve = (SHELVE)Enum.Parse( typeof( SHELVE ), GetString( currentKey ) );
            }
            catch
            {}

            return currentShelve;
        }
        
        private static void SetPlayerPrefShelve(SHELVE currentShelve, SIDE side)
        {
            var currentKey = GetPlayerPrefKey(side);
            
            SetString(currentKey, currentShelve.ToString());
        }

        private static void DrawFiles()
        {
            SymlinkButtons.Draw();
        }

        private static void DrawAddressable()
        {
            OpenAddressableButton.Draw();
            ReloadAndBuildAddressable.Draw();
            ClearAddressable.Draw();
        }

        private static void DrawLocalisation()
        {
            RefreshLocalisation.Draw();
        }

        private static void DrawGraphics()
        {
            GraphicTierChanger.Draw();
        }

        private static void DrawLevel()
        {
            SelectLevel.Draw("Current", "EditorLevelPath", false);
            CreateLevelButton.Draw();
            AddSituationToLevel.Draw();
        }
        
        #endregion
    }
}