using System.Reflection;
using System;
using UnityEditor;
using UnityEngine;

using Component = UnityEngine.Component;

namespace Universe.Editor.NullChecker
{
    [CustomPropertyDrawer(typeof(UnityEngine.Object), true)]
    public class ObjectDrawer : PropertyDrawer
    {
        public ObjectDrawer() : base()
        {
            ConformToSettings();
            PopulateAssemblyNames();
        }

        #region Unity API

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) 
        {
            ConformToSettings();
            _property = property;

            if(_type == null)
            {
                _type = DeterminePropertyType();
            }

            EditorGUI.BeginProperty(rect, label, _property);
            EditorGUI.DrawRect(rect, WillNeedFix() ? _errorColor : _okColor);

            var lineHeight = EditorGUIUtility.singleLineHeight + _lineHeight;
            var fieldRect = new Rect(rect.x, rect.y, rect.width, lineHeight);
            EditorGUI.PropertyField(fieldRect, _property, label);

            if(!_property.objectReferenceValue)
            {
                var warningRect = new Rect(rect.x, 
                                           rect.y + lineHeight + _lineSpacing, 
                                           rect.width, 
                                           lineHeight);

                if(_warningText.Length > 0) DrawWarningLabel(warningRect);

                if(_type != null && _ownerMono != null)
                {
                    var buttonRect = new Rect(rect.x,
                                              (_warningText.Length > 0 ? warningRect.y : rect.y) + lineHeight + _lineSpacing, 
                                              rect.width, 
                                              lineHeight);

                    if(_type.Equals(typeof(GameObject)))
                    {
                        DrawFixGameObjectButton(buttonRect);
                    }
                    else if(_type.IsSubclassOf(typeof(Component)))
                    {
                        DrawFixComponentButton(buttonRect);
                    }
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) 
        {
            int lineCount = 1;
            if(WillNeedFix())
            {
                if(_warningText.Length > 0) lineCount++;
                if(CanBeFixed()) lineCount++;
            }

            return (EditorGUIUtility.singleLineHeight + _lineHeight) * lineCount + _lineSpacing * (lineCount - 1);
        }

        #endregion


        #region Main

        private void DrawWarningLabel(Rect rect)
        {
            EditorGUI.LabelField(rect, _warningText);
        }

        private void DrawFixGameObjectButton(Rect rect)
        {
            if(GUI.Button(rect, "FIX GameObject"))
            {
                FindValueToFixGameObject();
            }
        }

        private void DrawFixComponentButton(Rect rect)
        {
            if(GUI.Button(rect, "FIX component"))
            {
                FindValueToFixComponent();

                _warningText = _defaultWarning;
                _warningText += " (Not found)";
            }
        }

        #endregion


        #region Utils

        private void ConformToSettings()
        {
            _settings = NullCheckerSettings.GetOrCreateSettings();

            _lineHeight = _settings.LinePixelSize;
            _lineSpacing = _settings.LinePixelSpacing;
            _okColor = _settings.ValidColor;
            _errorColor = _settings.ErrorColor;
            _defaultWarning = _settings.DefaultWarning;
            _warningText = _defaultWarning;
        }

        private bool WillNeedFix()
        {
            if(_property == null) return false;
            if(!_property.objectReferenceValue) return true;

            return false;
        }

        private bool CanBeFixed()
        {
            if(_type == null) return false;
            if(_type.Equals(typeof(GameObject)) || _type.IsSubclassOf(typeof(Component))) return true;

            return false;
        }

        private Type DeterminePropertyType()
        {
            var targetObject = _property.serializedObject.targetObject;
            if(targetObject is MonoBehaviour)
            {
                _ownerMono = (MonoBehaviour)targetObject;
            }

            var targetObjectType = targetObject.GetType();
            var property = targetObjectType.GetProperty(_property.name, 
                                                        BindingFlags.Instance | 
                                                        BindingFlags.NonPublic | 
                                                        BindingFlags.Public);

            if(property != null) return property.PropertyType;
            
            var field = targetObjectType.GetField(_property.name, 
                                                    BindingFlags.Instance | 
                                                    BindingFlags.NonPublic | 
                                                    BindingFlags.Public);

            if(field != null) return field.FieldType;

            else
            {
                var element = FindTypeInAssemblies(_property.type);
                if(element != null) return element;
            }

            throw new Exception($"NullChecker: No suitable type found for '{_property.name}'({_property.displayName}) of type '{_property.type}' in '{_property.serializedObject.targetObject.name}'");
        }

        private void FindValueToFixGameObject()
        {
            _property.objectReferenceValue = _ownerMono.gameObject;
        }

        private void FindValueToFixComponent()
        {
            var method = typeof(Component).GetMethod("GetComponent", new Type[]{}).MakeGenericMethod(_type);
            _property.objectReferenceValue = (UnityEngine.Object)method.Invoke(_ownerMono, new object[]{});
        }

        private void PopulateAssemblyNames()
        {
            _assemblyNames = new string[_assemblies.Length];
            for (int i = 0; i < _assemblies.Length; i++)
            {
                _assemblyNames[i] = _assemblies[i].FullName.Split(',')[0];
            }
        }

        private Type FindTypeInAssemblies(string type)
        {
            if(type.Contains("PPtr<$"))
            {
                type = type.Replace("PPtr<$", "");
                type = type.Replace(">", "");
            }

            foreach (var assembly in _assemblyNames)
            {
                var path = $"{assembly}.{type}, {assembly}";

                try
                {
                    return Type.GetType(path, true);
                }
                
                catch(Exception){}
            }

            return Type.GetType($"{type}, Assembly-CSharp");
        }
        
        #endregion


        #region Private

        private NullCheckerSettings _settings;
        private float _lineHeight = 0;
        private float _lineSpacing = 0;
        private Color _okColor = Color.green;
        private Color _errorColor = Color.red;
        private string _defaultWarning;

        private SerializedProperty _property;
        private MonoBehaviour _ownerMono;
        private Type _type;
        private string _warningText;
        private string[] _assemblyNames;
        private Assembly[] _assemblies = AppDomain.CurrentDomain.GetAssemblies();

        #endregion
    }
}