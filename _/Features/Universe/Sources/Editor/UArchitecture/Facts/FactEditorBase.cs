using UnityEditor;
using UnityEngine;

namespace Universe.Editor
{
    [CustomEditor(typeof(FactBase), true)]
    public class FactEditorBase : UnityEditor.Editor
    {
        #region Unity API
        
        protected virtual void OnEnable()
        {
            _valueProperty = serializedObject.FindProperty("_value");
            _washProperty = serializedObject.FindProperty("_defaultValue");
            _buttonDefaultBackgroundColor = GUI.backgroundColor;
        } 
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawValues();
        }
        
        protected virtual void DrawValues()
        {
            var selection = Selection.activeObject as FactBase;
            if (selection == null) return;
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                var content = "Cannot display value. No PropertyDrawer for (" + Target.Type + ") [" + Target.ToString() + "]";

                GUILayout.Space(10);
            
                GUILayout.BeginVertical(EditorStyles.helpBox);
                
                GUILayout.Label("Editor");
                DrawToggle("Is Favorite", ref selection.m_isFavorite, Color.yellow);
                DrawToggle("Use Verbose Log On Change", ref selection.m_useVerboseOnChange, Color.green);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Clone")) Clone(selection);
                GUILayout.EndHorizontal();
                
                GUILayout.EndVertical();
                
                
                GUILayout.BeginVertical(EditorStyles.helpBox);
                
                GUILayout.Label("Options");
                DrawToggle("Is Analytics", ref selection.m_isAnalytics, Color.green);
                
                GUILayout.EndVertical(); 
                
                 
                GUILayout.BeginVertical(EditorStyles.helpBox);  
                
                GUILayout.Label("Gameplay");
                DrawToggle("Is Read Only", ref selection.m_isReadOnly, Color.green);
                DrawToggle("Wash On Awake And Compilation", ref selection.m_washOnAwakeAndCompilation, Color.green);
                GUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 50;
                EditorGUIUtility.fieldWidth = 100;
                GenericPropertyDrawer.DrawObjectPropertyDrawer(Target.Type, new GUIContent("Value"), _valueProperty, new GUIContent( "content", "content" ) );
                if (selection.m_washOnAwakeAndCompilation)
                {
                    GenericPropertyDrawer.DrawObjectPropertyDrawer(Target.Type, new GUIContent("Wash To:"), _washProperty, new GUIContent( "content", "content" ) );    
                }
                GUILayout.EndHorizontal();
            
                GUILayout.EndVertical();
                 
                if (scope.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
        
        private void DrawToggle( string label, ref bool target, Color enabledColor )
        {
            GUI.backgroundColor = target ? enabledColor : _buttonDefaultBackgroundColor;
            
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            target = GUILayout.Toggle(target, label);
            GUILayout.EndHorizontal();
            
            GUI.backgroundColor = _buttonDefaultBackgroundColor;
        }
        
        private static void Clone( ScriptableObject objectToDuplicate )
        {
            const string title = "Save Cloned ScriptableObject";
            const string extension = "asset";
            var defaultName = $"Cloned {objectToDuplicate.name}.{extension}";
            const string message = "Enter a file name for the ScriptableObject.";
            var pathOfDuplicationTarget = AssetDatabase.GetAssetPath(objectToDuplicate);
                
            var path = EditorUtility.SaveFilePanelInProject( title, defaultName, extension, message, pathOfDuplicationTarget );
            if ( path == "" ) return;
            
            var asset = Instantiate( objectToDuplicate ); 
            
            AssetDatabase.CreateAsset( asset, path );
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate );
            EditorGUIUtility.PingObject( asset );
        }
        
        #endregion
        
        
        #region Private
        
        private FactBase Target => (FactBase)target;
        private SerializedProperty _valueProperty;
        private SerializedProperty _washProperty;
        private Color _buttonDefaultBackgroundColor;

        #endregion
    }
}