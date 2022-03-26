using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using static UnityEditor.Selection;
using static UnityEditor.AssetDatabase;

using static UnityEngine.GUILayout;
using static UnityEditor.EditorApplication;

using static System.AppDomain;
using static System.IO.Path;
using static System.IO.Directory;

using static Sirenix.Utilities.Editor.GUIHelper;
using static Sirenix.Utilities.AssemblyUtilities;
using static Sirenix.Utilities.Editor.SirenixEditorGUI;

namespace Universe.Editor
{
    public class ScriptableObjectCreator : OdinMenuEditorWindow
    { 
       #region Menu Item
        
        [MenuItem("Assets/Create/Scriptable Object _u", priority = -1000)]
        private static void ShowDialog()
        {
            InitializeTypes();
            
            var path = "Assets";
            var obj = activeObject;
            if (obj && AssetDatabase.Contains(obj))
            {
                path = GetAssetPath(obj);
                if (!Exists(path))
                {
                    path = GetDirectoryName(path);
                }
            }

            var window = CreateInstance<ScriptableObjectCreator>();
                window.ShowUtility();
                window.position = GetEditorWindowRect();
                window.maxSize = new Vector2(800, 500);
                window.titleContent = new GUIContent(path);
            
            if (path != null)
            {
                window.targetFolder = path.Trim('/');
            }
        }
        
        #endregion
        
        
        #region Main
        
         private static void InitializeTypes()
         {
             var tempScriptableObjectTypes = GetTypes(AssemblyTypeFlags.CustomTypes)
                 .Where(t =>
                     t.IsClass &&
                     typeof(ScriptableObject).IsAssignableFrom(t) &&
                     !typeof(EditorWindow).IsAssignableFrom(t) &&
                     !typeof(UnityEditor.Editor).IsAssignableFrom(t));
             scriptableObjectTypes = Enumerable.ToList(tempScriptableObjectTypes);
             
             var currentDomain = CurrentDomain;
             currentDomain.Load("Universe.Runtime");
             var assems = currentDomain.GetAssemblies();
             
             foreach (var assem in assems)
             {
                 var tempTypes = assem.GetTypes().Where(t =>
                     t.IsClass &&
                     typeof(ScriptableObject).IsAssignableFrom(t) &&
                     !typeof(EditorWindow).IsAssignableFrom(t) &&
                     !typeof(UnityEditor.Editor).IsAssignableFrom(t));
                 
                 var types = Enumerable.ToList(tempTypes);
                 foreach (var t in types.Where(t => !t.ToString().Contains("Generic")))
                 {
                     scriptableObjectTypes.Add(t);
                 }
             }
         }

         protected override OdinMenuTree BuildMenuTree()
        {
            MenuWidth = 270;
            WindowPadding = Vector4.zero;

            var tree = new OdinMenuTree(false)
            {
                Config = {DrawSearchToolbar = true}, DefaultMenuStyle = OdinMenuStyle.TreeViewStyle
            };
            
            tree.AddRange(scriptableObjectTypes.Where(x => !x.IsAbstract), GetMenuPathForType).AddThumbnailIcons();
            tree.SortMenuItemsByName();
            tree.Selection.SelectionConfirmed += x => CreateAsset();
            
            tree.Selection.SelectionChanged += e =>
            {
                if (previewObject && !AssetDatabase.Contains(previewObject))
                {
                    DestroyImmediate(previewObject);
                }

                if (e != SelectionChangedType.ItemAdded)
                {
                    return;
                }

                var t = SelectedType;
                if (t != null && !t.IsAbstract)
                {
                    previewObject = CreateInstance(t);
                }
            };

            return tree;
        }
         
        #endregion

        
        #region Utils
        
        private string GetMenuPathForType(Type t)
        {
            if (t == null || !scriptableObjectTypes.Contains(t)) return "";

            var firstName = t.Name.Split('`').First();
            return GetMenuPathForType(t.BaseType) + "/" + firstName;
        }

        protected override IEnumerable<object> GetTargets()
        {
            yield return previewObject;
        }

        protected override void DrawEditor(int index)
        {
            scroll = BeginScrollView(scroll);
            {
                base.DrawEditor(index);
            }
            EndScrollView();

            if (!previewObject) return;
            
            FlexibleSpace();
            HorizontalLineSeparator();
            if (Button("Create Asset"))
            {
                CreateAsset();
            }
        }

        private void CreateAsset()
        {
            if (!previewObject) return;
            
            var destination = targetFolder + "/New " + MenuTree.Selection.First().Name + ".asset";
            destination = GenerateUniqueAssetPath(destination);
            AssetDatabase.CreateAsset(previewObject, destination);
            Refresh();
            activeObject = previewObject;
            delayCall += Close;
        }
        
        #endregion
        
        
         #region Private and Protected
         
         private static List<Type> scriptableObjectTypes;
         private ScriptableObject previewObject;
         private string targetFolder;
         private Vector2 scroll;

         private Type SelectedType
         {
             get
             {
                 var m = MenuTree.Selection.LastOrDefault();
                 return m?.Value as Type;
             }
         }
         
         #endregion
    }
}