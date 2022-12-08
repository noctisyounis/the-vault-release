using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

using static UnityEngine.Debug;

namespace Universe.DebugWatch.Runtime
{
    public class DebugMenuDatabase : SerializedScriptableObject
    {
        #region Public Members

        public const string ELEMENT_DEFAULT = "BUTTON";
        public const string ELEMENT_SELECTOR = "SELECTOR";

        [Header("Debug"), ReadOnly] public int m_methodsCount;

        [SerializeField, DictionaryDrawerSettings]
        public Dictionary<string, AttributeData> m_attributes;

        #endregion


        #region Public Properties

        public Dictionary<string, AttributeData> Attributes => m_attributes ??= new();

        #endregion


        #region Unity API

        private void OnEnable()
        {
            ClearResults();
        }

        #endregion


        #region Main

        public string GetTooltip(string path)
        {
            if (!IsValidMethod(path)) return string.Empty;

            var attribute = Attributes[path];
            var method = attribute.m_method;

            return method.GetCustomAttribute<DebugMenuAttribute>().Tooltip;
        }

        public string[] GetOptionNames(string path)
        {
            if (!IsValidMethod(path)) return new string[0];

            var attribute = Attributes[path];

            return attribute.GetOptionNames();
        }
        
        public OptionData[] GetOptionDatas(string path)
        {
            if (!IsValidMethod(path)) return new OptionData[0];

            var attribute = Attributes[path];

            return attribute.m_options;
        }

        public object GetLastResult(string path)
        {
            if (!IsValidMethod(path)) return null;

            var attribute = Attributes[path];

            return attribute.m_lastResult;
        }

        public void InvokeMethod(string path)
        {
            if (!IsValidMethod(path)) return;

            var attribute = Attributes[path];
            var method = attribute.m_method;

            try
            {
                method.Invoke(method.ReflectedType, new object[0]);
            }
            catch (Exception e)
            {
                LogError(e.StackTrace);
            }
        }

        public void InvokeMethod(string path, int option)
        {
            if (!IsValidMethod(path)) return;

            var attribute = Attributes[path];
            var method = attribute.m_method;
            var selectedOption = attribute.m_options[option].m_value;

            try
            {
                method.Invoke(method.ReflectedType, new object[] { selectedOption });
            }
            catch (Exception e)
            {
                LogError(e.StackTrace);
            }
        }

        public T InvokeMethod<T>(string path)
        {
            if (!IsValidMethod(path)) return default(T);

            var result = default(T);
            var attribute = Attributes[path];
            var method = attribute.m_method;

            try
            {
                var tmp = method.Invoke(method.ReflectedType, new object[0]);
                if (tmp is null)
                    return default;

                if (tmp is not T typeResult)
                    throw new Exception(
                        $"Return type of {method.Name} [{tmp.GetType()}] cannot be casted to {typeof(T)}");

                result = typeResult;
            }
            catch (Exception e)
            {
                LogError(e.StackTrace);
            }

            attribute.m_lastResult = result;
            Attributes[path] = attribute;
            return result;
        }

        public T InvokeMethod<T>(string path, int option)
        {
            if (!IsValidMethod(path)) return default(T);

            var result = default(T);
            var attribute = Attributes[path];
            var method = attribute.m_method;
            var selectedOption = attribute.m_options[option].m_value;

            try
            {
                var tmp = method.Invoke(method.ReflectedType, new object[] { selectedOption });
                if (tmp is null)
                    return default;

                if (tmp is not T typeResult)
                    throw new Exception(
                        $"Return type of {method.Name} [{tmp.GetType()}] cannot be casted to {typeof(T)}");

                result = typeResult;
            }
            catch (Exception e)
            {
                LogError(e.StackTrace);
            }

            attribute.m_lastResult = result;
            Attributes[path] = attribute;
            return result;
        }

        public void InvokeMethod(string path, object[] parameters)
        {
            if (!IsValidMethod(path)) return;

            var attribute = Attributes[path];
            var method = attribute.m_method;

            try
            {
                method.Invoke(method.ReflectedType, parameters);
            }
            catch (Exception e)
            {
                LogError(e.StackTrace);
            }
        }

        public T InvokeMethod<T>(string path, object[] parameters)
        {
            if (!IsValidMethod(path)) return default(T);

            var result = default(T);
            var attribute = Attributes[path];
            var method = attribute.m_method;

            try
            {
                result = (T)method.Invoke(method.ReflectedType, parameters);
            }
            catch (Exception e)
            {
                LogError(e.StackTrace);
            }

            attribute.m_lastResult = result;
            return result;
        }

        public string[] GetQuickPaths()
        {
            var result = new List<string>();

            foreach (var entry in Attributes)
            {
                var attribute = entry.Value;
                var method = attribute.m_method;

                if (!IsQuickMenu(method)) continue;

                result.Add(entry.Key);
            }

            return result.ToArray();
        }

        private void ClearResults()
        {
            var count = Attributes.Count;
            var keys = Attributes.Keys.ToList();
            
            for(var i = 0; i < count; i++)
            {
                var key = keys[i];
                var attribute = Attributes[key];
                
                attribute.m_lastResult = null;
                Attributes[key] = attribute;
            }
        }

    #endregion


        #region Utils

        public string[] GetPathsWithType()
        {
            var keys = Attributes.Keys.ToArray();
            var length = keys.Length;
            var paths = new string[length];
            
            for (var i = 0; i < length; i++)
            {
                var key = keys[i];
                var attribute = Attributes[key];
                var path = key;

                if (typeof(DebugMenuSelectorAttribute).IsAssignableFrom(attribute.m_attributeType))
                    path = $"{path}/{ELEMENT_SELECTOR}";
                else
                    path = $"{path}/{ELEMENT_DEFAULT}";

                paths[i] = path;
            }

            return paths;
        }
        public MethodInfo GetMethodInfoAt(string path) => 
            Attributes[path].m_method;

        private bool IsValidMethod(string path) => 
            (Attributes.ContainsKey(path) && !Attributes[path].m_method.IsPrivate);
        private static bool IsQuickMenu(MethodInfo method) =>
            method.GetCustomAttribute<DebugMenuAttribute>().IsQuickMenu;

        #endregion


        #region Unity API

        public void OnValidate() 
        {
            if(m_attributes is null) return;

            m_methodsCount = Attributes.Count;
            _methodNames.Clear();

            foreach(var entry in Attributes)
            {
                var attribute = entry.Value;
                var method = attribute.m_method;
                if(method is null) continue;

                _methodNames.Add($"{method.DeclaringType.Name}/{method.Name}");
            }
        }

        #endregion


        #region Private Members

        [ReadOnly, SerializeField]
        private List<string> _methodNames = new();

        #endregion
    }
}