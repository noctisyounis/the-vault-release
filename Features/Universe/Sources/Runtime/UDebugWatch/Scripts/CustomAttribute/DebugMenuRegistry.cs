using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using static UnityEngine.Debug;
using static System.AppDomain;

namespace Universe.DebugWatch.Runtime
{
    public class DebugMenuRegistry
    {
        #region Public Members

        public static DebugMenuData s_bakedData;

        #endregion


        #region Public Properties

        public static string[] Paths => 
            Methods.Keys.ToArray();

        #endregion


        #region Main

        public static void InitializeMethods()
        {
            InitializeDictionnary();
            ValidateDictionary();
        }

        public static MethodInfo GetMethodInfoAt(string path) => 
            Methods[path];

        #endregion


        #region Utils

        private static void InitializeDictionnary()
        {
            var length = Assemblies.Length;

            Methods = new Dictionary<string, MethodInfo>();

            for (var i = 0; i < length; i++)
            {
                var assembly = Assemblies[i];
                var assemblyDictionary = assembly
                            .GetTypes()
                            .SelectMany(classType       => classType.GetMethods())
                            .Where(classMethod          => classMethod.GetCustomAttributes().OfType<DebugMenuAttribute>().Any())
                            .ToDictionary(methodInfo    => methodInfo.GetCustomAttributes().OfType<DebugMenuAttribute>().FirstOrDefault().Path);

                if (assemblyDictionary is null) continue;
                
                Methods.Merge(assemblyDictionary);
            }
        }

        private static void ValidateDictionary()
        {
            var validCount          = 0;
            var initialMethodCount  = Methods.Count;
            var keys                = Paths;

            s_bakedData.Methods.Clear();

            for (var i = 0; i < initialMethodCount; i++)
            {
                var key = keys[i];
                var method = Methods[key];

                if (!method.IsStatic)
                {
                    LogError($"<color=orange>{method.Name} of class {method.ReflectedType} must be static</color>");
                }
                else
                {
                    s_bakedData.m_methods.Add(key, method);
                    validCount++;
                }
            }

            var multiplicity = (validCount > 0) ? $"{validCount} {(validCount > 1 ? "were" : "was")}" : "none was";

            Log($"<color=cyan>{initialMethodCount} methods were tested and {multiplicity} valid</color>");
        }

        private static void InitializeAssemblies() => _assemblies = CurrentDomain.GetAssemblies();

        #endregion
        

        #region Private Properties

        private static Assembly[] Assemblies
        {
            get
            {
                if (_assemblies == null)
                    InitializeAssemblies();
                
                return _assemblies;
            }
        }

        private static Dictionary<string, MethodInfo> Methods
        {
            get => _methods ??= new();
            set => _methods = value;
        }

        #endregion


        #region Private Members

        private static Assembly[] _assemblies;
        private static Dictionary<string, MethodInfo> _methods;

        #endregion
    }
}