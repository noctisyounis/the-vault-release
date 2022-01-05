using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace Universe
{
    [CustomEditor(typeof(Injector))]
    public class UInjectorEditor : UnityEditor.Editor
    {
        #region Properties

        private Dictionary<string, PropertyInfo> Targets
        {
            get
            {
                if(_targets == null)
                {
                    InitializeTargets();
                }

                return _targets;
            }

            set
            {
                _targets = value;
            }
        }
        private Dictionary<string, PropertyInfo> Sources
        {
            get
            {
                if(_sources == null)
                {
                    InitializeSources();
                }

                return _sources;
            }

            set
            {
                _sources = value;
            }
        }

        private string[] TargetOptions
        {
            get
            {
                if(_targetOptions == null)
                {
                    _targetOptions = GetAllTargetProperties();
                }

                return _targetOptions;
            }

            set
            {
                _targetOptions = value;
            }
        }

        private string[] SourcePropertyOptions
        {
            get
            {
                if(_sourcePropertyOptions == null)
                {
                    _sourcePropertyOptions = GetAllSourceProperties();
                }

                return _sourcePropertyOptions;
            }

            set
            {
                _sourcePropertyOptions = value;
            }
        }

        #endregion


        #region Unity API

        private void OnEnable() 
        {
            _targetedScript = target as Injector;
            
            InitializeHeaderStyle();
            InitializeDataInjectorValues();
            InitializeIndexes();
            InitializeProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawSource();
            GUILayout.Space(8f);
            DrawTarget();

            serializedObject.ApplyModifiedProperties();

            if(!CanDrawButtonOrWarning()) return;

            GUILayout.Space(8f);

            if(_targetedScript.CanSetTargetValue())
            {
                DrawInjectButton();
            }
            else
            {
                DrawWarning();
            }

        }

        #endregion


        #region Main

        private void InitializeHeaderStyle()
        {
            _headerStyle = new GUIStyle();
            _headerStyle.fontStyle = FontStyle.Bold;
            _headerStyle.normal.textColor = Color.white;
        }

        private void InitializeDataInjectorValues()
        {
            _targetObjectSerialized = serializedObject.FindProperty("_targetObject");
            _targetProperty = _targetedScript.TargetProperty;
            _targetPropertyNameSerialized = serializedObject.FindProperty("_targetPropertyName");

            _sourceObjectSerialized = serializedObject.FindProperty("_sourceObject");
            _sourceProperty = _targetedScript.SourceProperty;
            _sourcePropertyNameSerialized = serializedObject.FindProperty("_sourcePropertyName");
        }

        private void InitializeIndexes()
        {
            var targetIndex = GetIndexOfTarget();
            var sourceIndex = GetIndexOfSource();

            _targetIndex = targetIndex >= 0 ? targetIndex : 0;
            _sourceIndex = sourceIndex >= 0 ? sourceIndex : 0;
        }

        private void InitializeProperties()
        {
            UpdateSourceProperty();
            UpdateTargetProperty();
        }

        private void UpdateTargetProperty()
        {
            _targetProperty = Targets[TargetOptions[_targetIndex]];
            _targetedScript.TargetProperty = _targetProperty;   
            _targetPropertyNameSerialized.stringValue = TargetOptions[_targetIndex];
        }

        private void UpdateSourceProperty()
        {
            _sourceProperty = Sources[SourcePropertyOptions[_sourceIndex]];
            _targetedScript.SourceProperty = _sourceProperty;
            _sourcePropertyNameSerialized.stringValue = SourcePropertyOptions[_sourceIndex];
        }

        private void DrawTarget()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("2.Apply It To", _headerStyle);

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_targetObjectSerialized, new GUIContent());

            if(EditorGUI.EndChangeCheck())
            {
                TargetOptions = GetAllTargetProperties();
                InitializeTargets();

                _targetIndex = 0;

                UpdateTargetProperty();
            }

            EditorGUI.BeginChangeCheck();

            _targetIndex = EditorGUILayout.Popup(_targetIndex, TargetOptions);

            if(EditorGUI.EndChangeCheck())
            {
                UpdateTargetProperty();
            }

            EditorGUILayout.EndHorizontal();

            if (_targetProperty is null)
            {
                EditorGUILayout.EndVertical();
                return;
            }
            
            EditorGUILayout.LabelField($"{_targetPropertyNameSerialized.stringValue} = {_targetProperty.GetValue(_targetObjectSerialized.objectReferenceValue)}", EditorStyles.helpBox);

            EditorGUILayout.EndVertical();
        }

        private void DrawSource()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("1.Source Value From", _headerStyle);

            EditorGUILayout.BeginHorizontal();


            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_sourceObjectSerialized, new GUIContent());

            if(EditorGUI.EndChangeCheck())
            {
                SourcePropertyOptions = GetAllSourceProperties();
                InitializeSources();

                _sourceIndex = 0;

                UpdateSourceProperty();
            }

            EditorGUI.BeginChangeCheck();
            
            _sourceIndex = EditorGUILayout.Popup(_sourceIndex, SourcePropertyOptions);

            if(EditorGUI.EndChangeCheck())
            {
                UpdateSourceProperty();
            }

            EditorGUILayout.EndHorizontal();

            if (_sourceProperty is null)
            {
                EditorGUILayout.EndVertical();
                return;
            }
            EditorGUILayout.LabelField($"{_sourcePropertyNameSerialized.stringValue} = {_sourceProperty.GetValue(_sourceObjectSerialized.objectReferenceValue)}", EditorStyles.helpBox);
            
            EditorGUILayout.EndVertical();
        }

        private void DrawInjectButton()
        {
            if(GUILayout.Button("Inject"))
            {
                _targetedScript.TrySetTargetValueFromSource();
            }
        }

        private void DrawWarning()
        {
            EditorGUILayout.HelpBox($"Source \"{_sourceProperty.Name}\"'s type ({_sourceProperty.PropertyType})\ndoesn't match\nTarget \"{_targetProperty.Name}\"'s type ({_targetProperty.PropertyType})", MessageType.Warning);
        }

        #endregion


        #region Utils

        private bool CanDrawButtonOrWarning()
        {
            return _sourceProperty != null && _targetProperty != null;
        }

        private int GetIndexOfTarget()
        {
            return Array.IndexOf(TargetOptions, _targetPropertyNameSerialized.stringValue);
        }

        private int GetIndexOfSource()
        {
            return Array.IndexOf(SourcePropertyOptions, _sourcePropertyNameSerialized.stringValue);
        }

        private string[] GetAllTargetsProperties()
        {
            Targets = new Dictionary<string, PropertyInfo>();

            var resultList = new List<string>();

            var gameObject = (serializedObject.targetObject as Component).gameObject;
            var components = gameObject.GetComponents(typeof(Component));

            foreach(var component in components)
            {
                if(component.Equals((Component)serializedObject.targetObject)) continue;

                var componentPath = $"{component.GetType().Name}/{component.GetInstanceID()}";
                var properties = component.GetType().GetTypeInfo().GetProperties();

                component.GetInstanceID();

                foreach (var property in properties.OrderBy(property => property.Name))
                {
                    var propertyPath = $"{componentPath}/{property.Name}";

                    if(property.CanWrite)
                    {
                        _targets.Add(propertyPath, property);
                        resultList.Add(propertyPath);
                    }
                }
            }

            var result = resultList.ToArray();

            return result;
        }
        
        private string[] GetAllTargetProperties()
        {
            var resultList = new List<string>();

            if(_targetObjectSerialized.objectReferenceValue == null) 
            {
                resultList.Add("None");
                return resultList.ToArray();
            }

            var properties = _targetObjectSerialized.objectReferenceValue.GetType().GetTypeInfo().GetProperties();

            foreach (var property in properties.OrderBy(property => property.Name))
            {
                var propertyPath = $"{property.Name}";

                if(property.CanWrite)
                {
                    resultList.Add(propertyPath);
                }
            }
            
            var result = resultList.ToArray();

            return result;
        }

        private void InitializeTargets()
        {
            Targets = new Dictionary<string, PropertyInfo>();

            if(_targetObjectSerialized.objectReferenceValue == null) 
            {
                Targets.Add("None", null);
                return;
            }

            var properties = _targetObjectSerialized.objectReferenceValue.GetType().GetTypeInfo().GetProperties();

            foreach (var property in properties.OrderBy(property => property.Name))
            {
                var propertyPath = $"{property.Name}";

                if(property.CanWrite)
                {
                    _targets.Add(propertyPath, property);
                }
            }
        }

        private string[] GetAllSourceProperties()
        {
            var resultList = new List<string>();

            if(_sourceObjectSerialized.objectReferenceValue == null) 
            {
                resultList.Add("None");
                return resultList.ToArray();
            }

            var properties = _sourceObjectSerialized.objectReferenceValue.GetType().GetTypeInfo().GetProperties();

            foreach (var property in properties.OrderBy(property => property.Name))
            {
                var propertyPath = $"{property.Name}";

                if(property.CanWrite)
                {
                    resultList.Add(propertyPath);
                }
            }
            
            var result = resultList.ToArray();

            return result;
        }

        private void InitializeSources()
        {
            Sources = new Dictionary<string, PropertyInfo>();

            if(_sourceObjectSerialized.objectReferenceValue == null) 
            {
                Sources.Add("None", null);
                return;
            }

            var properties = _sourceObjectSerialized.objectReferenceValue.GetType().GetTypeInfo().GetProperties();

            foreach (var property in properties.OrderBy(property => property.Name))
            {
                var propertyPath = $"{property.Name}";

                if(property.CanWrite)
                {
                    _sources.Add(propertyPath, property);
                }
            }
        }

        #endregion


        #region Private

        private GUIStyle _headerStyle;

        private Injector _targetedScript;

        private Dictionary<string, PropertyInfo> _targets;
        private Dictionary<string, PropertyInfo> _sources;

        private int _targetIndex;
        private int _sourceIndex;

        private string[] _targetOptions;
        private string[] _sourcePropertyOptions;

        private Type _bufferedTargetType;
        private Type _bufferedSourceType;

        private SerializedProperty _canUpdateSerialized;
        private SerializedProperty _targetObjectSerialized;
        private PropertyInfo _targetProperty;
        private SerializedProperty _targetPropertyNameSerialized;
        private SerializedProperty _sourceObjectSerialized;
        private PropertyInfo _sourceProperty;
        private SerializedProperty _sourcePropertyNameSerialized;

        #endregion
    }
}