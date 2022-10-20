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

        public static DebugMenuDatabase s_bakedDatabase;

        #endregion


        #region Public Properties

        public static string[] Paths => 
            Attributes.Keys.ToArray();

        #endregion


        #region Main

        public static void InitializeMethods()
        {
            InitializeDictionnary();
            ValidateDictionary();
        }

        public static AttributeData GetAttributeDataAt(string path) => 
            Attributes[path];

        #endregion


        #region Utils

        private static void InitializeDictionnary()
        {
            var length = Assemblies.Length;

            Attributes = new Dictionary<string, AttributeData>();

            for (var i = 0; i < length; i++)
            {
                var assembly = Assemblies[i];
                var assemblyList = assembly
                            .GetTypes()
                            .SelectMany(classType       => classType.GetMethods())
                            .Where(classMethod          => classMethod.GetCustomAttributes().OfType<DebugMenuAttribute>().Any())
                            .ToList();

                if (assemblyList is null) continue;
                
                AddAll(assemblyList);
            }

            Attributes.OrderBy(attribute =>
                attribute.Value.m_method.GetCustomAttributes().OfType<DebugMenuAttribute>().FirstOrDefault().SortingOrder);
        }

        private static void AddAll(List<MethodInfo> methods)
        {
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<DebugMenuAttribute>();
                var data = new AttributeData()
                {
                    m_attributeType = attribute.GetType(),
                    m_method = method,
                    m_options = attribute.GetOptions()
                };

                Attributes.Add(attribute.Path, data);
            }
        }

        private static void ValidateDictionary()
        {
            var validCount          = 0;
            var initialMethodCount  = Attributes.Count;
            var keys                = Paths;

            s_bakedDatabase.Attributes.Clear();

            for (var i = 0; i < initialMethodCount; i++)
            {
                var key = keys[i];
                var attribute = Attributes[key];
                var method = attribute.m_method;

                if (!method.IsStatic)
                {
                    LogError($"<color=orange>{method.Name} of class {method.ReflectedType} must be static</color>");
                }
                else
                {
                    s_bakedDatabase.Attributes.Add(key, attribute);
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

        private static Dictionary<string, AttributeData> Attributes
        {
            get => _attributes ??= new();
            set => _attributes = value;
        }

        #endregion


        #region Private Members

        private static Assembly[] _assemblies;
        private static Dictionary<string, AttributeData> _attributes;

        #endregion
    }
}