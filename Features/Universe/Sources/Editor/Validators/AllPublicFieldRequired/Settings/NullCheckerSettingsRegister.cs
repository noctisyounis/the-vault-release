using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Universe.Editor.NullChecker
{
    public static class NullCheckerSettingsRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateNullCheckerSettingsProvider()
        {
            var provider = new SettingsProvider("Project/NullCheckSettings", SettingsScope.Project)
            {
                label = "Null Checker",

                guiHandler = (searchContext) =>
                {
                    var settings = NullCheckerSettings.SerializedSettings;
                    var input = Event.current;

                    EditorGUILayout.PropertyField(settings.FindProperty("_linePixelSize"), new GUIContent("Line size in pixels"));
                    EditorGUILayout.PropertyField(settings.FindProperty("_linePixelSpacing"), new GUIContent("Line spacing in pixels"));
                    EditorGUILayout.PropertyField(settings.FindProperty("_validColor"), new GUIContent("Field color when filled"));
                    EditorGUILayout.PropertyField(settings.FindProperty("_errorColor"), new GUIContent("Field color when null"));
                    EditorGUILayout.PropertyField(settings.FindProperty("_defaultWarning"), new GUIContent("Warning message diplayed when null"));
                    EditorGUILayout.PropertyField(settings.FindProperty("_settingPathOverride"), new GUIContent("Setting path"));                    

                    settings.ApplyModifiedProperties(); 
                    if (input.keyCode != KeyCode.Return) return;
                    
                    NullCheckerSettings.Instance.CheckPath();
                },

                deactivateHandler = () => 
                {
                    if(NullCheckerSettings.Instance == null) NullCheckerSettings.GetOrCreateSettings();
                    NullCheckerSettings.Instance.CheckPath();
                },

                activateHandler = (_, _) => 
                {
                    if(NullCheckerSettings.Instance == null) NullCheckerSettings.GetOrCreateSettings();
                    NullCheckerSettings.Instance.CheckPath();
                },

                keywords = new HashSet<string>(new[]    {
                                                            "Line", 
                                                            "Size", "size", 
                                                            "Spacing", "spacing", 
                                                            "Color", "color", 
                                                            "Warning", 
                                                            "Assembly", "assembly"
                                                        })
            };

            return provider;
        }
    }
}