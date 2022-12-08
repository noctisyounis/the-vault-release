using System.Reflection;
using System;
using CSharpExtensions;
using UnityEditor;
using UnityEngine;

using static UnityEditor.EditorGUI;
using static UnityEditor.EditorGUILayout;
using static UnityEditor.EditorUtility;
using Object = UnityEngine.Object;

namespace Universe.Editor
{
    public static class GenericPropertyDrawer
    {
        public static void DrawPropertyDrawer(Rect rect, GUIContent label, Type type, SerializedProperty property, GUIContent errorLabel)
        {
            if (typeof(Object).IsAssignableFrom(type) || type.IsEnum)
            {
                if (typeof(Object).IsAssignableFrom(type)
                    && !IsPersistent(property.objectReferenceValue)
                    && property.objectReferenceValue != null)
                {
                    using (new DisabledGroupScope(true))
                    {
                        property.objectReferenceValue = ObjectField( rect, label, property.objectReferenceValue, type, false);
                    }
                }
                else if (type.IsAssignableFrom(typeof(Quaternion)))
                {
                    var displayValue = property.quaternionValue.ToVector4();

                    property.quaternionValue = Vector4Field(rect, label, displayValue).ToQuaternion();
                }
                else if (type.IsAssignableFrom(typeof(Vector4)))
                {
                    property.vector4Value = Vector4Field(rect, label, property.vector4Value);
                }
                else
                {
                    PropertyField(rect, property, label);
                }
            }
            else
            {
                LabelField(rect, errorLabel);
            }
        }
        
        public static void DrawObjectPropertyDrawer(Type type, GUIContent label, SerializedProperty property, GUIContent errorLabel)
        {
            if ( !(type is null) || type.IsEnum)
            {
                if(typeof(Object).IsAssignableFrom(type))
                {
                    var referenceValue = property.objectReferenceValue;

                    if (typeof(object).IsAssignableFrom(type) && !IsPersistent(referenceValue)
                                                          && referenceValue != null)
                    {
                        using (new DisabledGroupScope(true))
                        {
                            property.objectReferenceValue = ObjectField( label, referenceValue, type, false);
                        }
                    }
                }
                else if (type.IsAssignableFrom(typeof(Quaternion)))
                {
                    var displayValue = property.quaternionValue.ToVector4();

                    property.quaternionValue = Vector4Field(label, displayValue).ToQuaternion();
                }
                else if (type.IsAssignableFrom(typeof(Vector4)))
                {
                    property.vector4Value = Vector4Field(label, property.vector4Value);
                }
                else
                {
                    PropertyField(property, label);
                }       
            }
            else
            {
                LabelField(errorLabel);
            }
        }
    }
}