using System;
using UnityEditor;
using UnityEngine;

using static UnityEditor.EditorGUILayout;

namespace Universe.Editor
{
    [CustomEditor( typeof( FontSettingsTable ) )]
    public class FontSettingsCollectionEditor : UnityEditor.Editor
    {
        #region Unity API

        private void OnEnable()
        {
            _fontSettingsList = serializedObject.FindProperty( "m_list" );
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Space();

            var enumArray = Enum.GetNames( typeof( FontSettingsType ) );
            var labelStyle = new GUIStyle();

            BeginVertical();
            var enumCount = enumArray.Length;
            for( var i = 0; i < enumCount; i++ )
            {
                DrawFontSettingsEntry( enumArray, labelStyle, i );
            }
            EndVertical();

            serializedObject.Update();
        }

        #endregion


        #region Utilities

        private void DrawFontSettingsEntry( string[] enumArray, GUIStyle labelStyle, int index )
        {
            BeginHorizontal();

            PrefixLabel( enumArray[index], labelStyle );
            InsertEmptyElementIfDoesntExist( index );
            CreateFontSettingsField( index );

            EndHorizontal();
        }

        private void CreateFontSettingsField( int index )
        {
            var objValue = _fontSettingsList.GetArrayElementAtIndex( index ).objectReferenceValue;

            _fontSettingsList.GetArrayElementAtIndex( index ).objectReferenceValue = ObjectField( objValue, typeof( FontSettings ), false );

            serializedObject.ApplyModifiedProperties();
        }

        private void InsertEmptyElementIfDoesntExist( int index )
        {
            if( index < _fontSettingsList.arraySize ) return;

            _fontSettingsList.InsertArrayElementAtIndex( index );
        }

        #endregion


        #region Private

        private SerializedProperty _fontSettingsList;

        #endregion
    }
}