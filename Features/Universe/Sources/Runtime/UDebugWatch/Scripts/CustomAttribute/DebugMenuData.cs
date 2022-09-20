using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

using static UnityEngine.Debug;

namespace Universe.DebugWatch.Runtime
{
    public class DebugMenuData : SerializedScriptableObject
    {
        #region Public Members

        [Header("Debug"), ReadOnly]
        public int m_methodsCount;

        [SerializeField, DictionaryDrawerSettings]
        public Dictionary<string, MethodInfo> m_methods;

        #endregion


        #region Public Properties

        public Dictionary<string, MethodInfo> Methods => m_methods ??= new();

        #endregion


        #region Main

        public string GetTooltip( string path )
        {
            if(!IsValidMethod(path)) return string.Empty;

            var method = Methods[path];

            return method.GetCustomAttribute<DebugMenuAttribute>().Tooltip;
        }

        public void InvokeMethod(string path)
        {
            if (!IsValidMethod(path)) return;

            var method = Methods[path];

            try
            {
                method.Invoke(method.ReflectedType, new object[0]);
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
            var method = Methods[path];

            try
            {
                result = (T)method.Invoke(method.ReflectedType, new object[0]);
            }
            catch (Exception e)
            {
                LogError(e.StackTrace);
            }

            return result;
        }

        public void InvokeMethod(string path, object[] parameters)
        {
            if (!IsValidMethod(path)) return;

            var method = Methods[path];

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
            var method = Methods[path];

            try
            {
                result = (T)method.Invoke(method.ReflectedType, parameters);
            }
            catch (Exception e)
            {
                LogError(e.StackTrace);
            }

            return result;
        }

        public string[] GetQuickPaths()
        {
            var result = new List<string>();

            foreach (var entry in Methods)
            {
                var method = entry.Value;

                if (!IsQuickMenu(method)) continue;

                result.Add(entry.Key);
            }

            return result.ToArray();
        }

        #endregion


        #region Utils

        public string[] GetPaths() => 
            Methods.Keys.ToArray();
        public MethodInfo GetMethodInfoAt(string path) => 
            Methods[path];

        private bool IsValidMethod(string path) => 
            (Methods.ContainsKey(path) && !Methods[path].IsPrivate);
        private static bool IsQuickMenu(MethodInfo method) =>
            method.GetCustomAttribute<DebugMenuAttribute>().IsQuickMenu;

        #endregion


        #region Unity API

        public void OnValidate() 
        {
            if(m_methods is null) return;

            m_methodsCount = Methods.Count;
            _methodNames.Clear();

            foreach(var entry in Methods)
            {
                var method = entry.Value;
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