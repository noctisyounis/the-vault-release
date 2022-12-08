using System.Reflection;
using UnityEditor;
using UnityEngine;
using static System.Reflection.BindingFlags;
using static UnityEditor.EditorGUILayout;
using static UnityEditor.EditorStyles;
using static UnityEngine.GUILayout;

namespace Universe.Editor
{
    [CustomEditor(typeof(SignalGeneric<>), true)]
    public class TypedSignalEditor : SignalEditorBase
    {
        #region Unity API
        
        protected override void OnEnable()
        {
            base.OnEnable();

            var baseType = target.GetType().BaseType;
            if (!(baseType is null))
            {
                _emitMethod = baseType.GetMethod("Emit",
                                              DeclaredOnly | Instance | Public);
            }
                
        }
        
        protected override void DrawEmitButton(SignalBase emitTarget)
        { 
            var property = serializedObject.FindProperty("_debugValue");
            GUILayout.BeginHorizontal(helpBox);
            
            PropertyField(property);
            if (Button("Emit"))
            {
                CallMethod(GetDebugValue(property));
                
            }

            GUILayout.EndHorizontal();
        }
        
        #endregion
          
          
        #region Utilities
        
        private static object GetDebugValue( SerializedProperty property )
        {
            var targetType = property.serializedObject.targetObject.GetType();
            var targetField = targetType.GetField("_debugValue", Instance | NonPublic);
            
            return (targetField == null) ? null : targetField.GetValue(property.serializedObject.targetObject);
        }

        private void CallMethod(object value)
        {
            _emitMethod.Invoke(target, new []{ value } );
        }

        #endregion
        
        
        #region Private And Protected
        
        private MethodInfo _emitMethod;
        
        #endregion
    }
}