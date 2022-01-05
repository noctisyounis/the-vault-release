using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Universe
{
    using static EditorGUI;
    using static AssetDatabase;
    using static GUIUtility;

    [CustomPropertyDrawer( typeof( USpreadsheetProvider ) )]
    public class USpreadsheetProviderDrawer : PropertyDrawer
    {
        #region Unity API

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            BeginProperty(position, label, property);

            position = CreateLabel(position, label);
            CreateSpreadsheetProviderDropdownList(position, property);

            EndProperty();
        }

        #endregion


        #region GUI

        private static void CreateSpreadsheetProviderDropdownList(Rect position, SerializedProperty property)
        {
            var provider = property.objectReferenceValue as USpreadsheetProvider;
            var providers = GetAllUSpreadsheetProviderAssets<USpreadsheetProvider>();

            var typeIndex = providers.IndexOf(provider);
            typeIndex = Popup(position, typeIndex, providers.Select(a => a.name).ToArray());
            property.objectReferenceValue = providers[typeIndex];
        }

        private static Rect CreateLabel( Rect position, GUIContent label ) => PrefixLabel( position, GetControlID( FocusType.Passive ), label );

        #endregion


        #region Utilities

        private static List<T> GetAllUSpreadsheetProviderAssets<T>() where T : USpreadsheetProvider
        {
            return FindAssets($"t:{typeof(T).Name}")
                                       .Select(guid => GUIDToAssetPath(guid))
                                       .Select(path => LoadAssetAtPath(path, typeof(T)))
                                       .Select(obj => (T)obj)
                                       .OrderBy(a => a.name)
                                       .ToList();
        }

        #endregion
    }
}