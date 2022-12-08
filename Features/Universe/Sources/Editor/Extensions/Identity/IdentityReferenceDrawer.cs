using UnityEditor;
using UnityEngine;

namespace Universe.Editor
{
    [CustomPropertyDrawer(typeof(IdentityReference))]
    public class IdentityReferenceDrawer : PropertyDrawer
    {
        SerializedProperty guidProp;
        SerializedProperty sceneProp;
        SerializedProperty nameProp;


        GUIContent sceneLabel = new GUIContent("Containing Scene", "The target object is expected in this scene asset.");
        GUIContent clearButtonGUI = new GUIContent("Clear", "Remove Cross Scene Reference");

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
           
            guidProp = property.FindPropertyRelative("_serializedGuid");
            nameProp = property.FindPropertyRelative("_cachedName");
            sceneProp = property.FindPropertyRelative("_cachedScene");
            
            EditorGUI.BeginProperty(position, label, property);

            position.height = EditorGUIUtility.singleLineHeight;
            
            var guidCompPosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            System.Guid currentGuid;
            GameObject currentGO = null;

            byte[] byteArray = new byte[16];
            int arraySize = guidProp.arraySize;
            for( int i = 0; i < arraySize; ++i )
            {
                var byteProp = guidProp.GetArrayElementAtIndex(i);
                byteArray[i] = (byte)byteProp.intValue;
            }

            currentGuid = new System.Guid(byteArray);
            currentGO = IdentityManager.ResolveGuid(currentGuid);
            Identity currentIdentity = currentGO != null ? currentGO.GetComponent<Identity>() : null;

            Identity component = null;

            if (currentGuid != System.Guid.Empty && currentIdentity == null)
            {
                float buttonWidth = 55.0f;
                
                guidCompPosition.xMax -= buttonWidth;

                bool guiEnabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUI.LabelField(guidCompPosition, new GUIContent(nameProp.stringValue, "Target GameObject is not currently loaded."), EditorStyles.objectField);
                GUI.enabled = guiEnabled;

                Rect clearButtonRect = new Rect(guidCompPosition);
                clearButtonRect.xMin = guidCompPosition.xMax;
                clearButtonRect.xMax += buttonWidth;

                if (GUI.Button(clearButtonRect, clearButtonGUI, EditorStyles.miniButton))
                {
                    ClearPreviousGuid();
                }
            }
            else
            {
                component = EditorGUI.ObjectField(guidCompPosition, currentIdentity, typeof(Identity), true) as Identity;
            }

            if (currentIdentity != null && component == null)
            {
                ClearPreviousGuid();
            }
            
            if (component != null)
            {
                nameProp.stringValue = component.name;
                string scenePath = component.gameObject.scene.path;
                sceneProp.objectReferenceValue = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

                if (component != currentIdentity)
                {
                    byteArray = component.GetGuid().ToByteArray();
                    arraySize = guidProp.arraySize;
                    for (int i = 0; i < arraySize; ++i)
                    {
                        var byteProp = guidProp.GetArrayElementAtIndex(i);
                        byteProp.intValue = byteArray[i];
                    }
                }
            }

            EditorGUI.indentLevel++;
            position.y += EditorGUIUtility.singleLineHeight;
            bool cachedGUIState = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.ObjectField(position, sceneLabel, sceneProp.objectReferenceValue, typeof(SceneAsset), false);
            GUI.enabled = cachedGUIState;
            EditorGUI.indentLevel--;
           
            EditorGUI.EndProperty();
        }

        void ClearPreviousGuid()
        {
            nameProp.stringValue = string.Empty;
            sceneProp.objectReferenceValue = null;

            int arraySize = guidProp.arraySize;
            for (int i = 0; i < arraySize; ++i)
            {
                var byteProp = guidProp.GetArrayElementAtIndex(i);
                byteProp.intValue = 0;
            }
        }
    }
}