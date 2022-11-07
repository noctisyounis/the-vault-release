using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Universe.Editor
{
    public class FactBrowserWindow : EditorWindow
    {
        #region Menu Item

        [MenuItem("Vault/Fact/Fact _a")]
        public static void ShowDialog()
        {
            _array =  GetAllInstances<FactBase>();
            GetWindow<FactBrowserWindow>();
        }
        
        #endregion
        
        
        #region Main

        private void OnGUI()
        {
            _contentStyle = new GUIStyle(GUI.skin.button);
            
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("refresh", GUILayout.MaxWidth(100))) _array =  GetAllInstances<FactBase>();
            if (!_onlyFavorite && GUILayout.Button("show favorite", GUILayout.MaxWidth(100))) _onlyFavorite = !_onlyFavorite;
            if (_onlyFavorite && GUILayout.Button("show all", GUILayout.MaxWidth(100))) _onlyFavorite = !_onlyFavorite;
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(15);

            if (_array.IsNullOrEmpty())
            {
                _array = GetAllInstances<FactBase>();
            }
            
            GUILine(1);
            
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height - 60));
            foreach (var fact in _array)
            {
                if (_onlyFavorite && !fact.m_isFavorite) continue;
                
                GUILayout.BeginHorizontal();
                _contentStyle.alignment = TextAnchor.MiddleLeft;
                if (GUILayout.Button($"{fact.name}", _contentStyle, GUILayout.Width(Screen.width  * 0.5f)))
                {
                    Selection.activeObject = fact;
                }
                
                GUILayout.Label($"=> {fact.ToString()}");
                GUILayout.EndHorizontal();
                GUILine(1);
            }
            EditorGUILayout.EndScrollView();
        }
        
        #endregion
        
        
        #region Utils
        
        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:"+ typeof(T).Name);  //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for(int i =0;i<guids.Length;i++)         //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }
 
            return a;
        }
        
        private void GUILine( int i_height = 1 )
        {

            Rect rect = EditorGUILayout.GetControlRect(false, i_height );

            rect.height = i_height;

            EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );

        }
        
        #endregion
        
        
        #region Private

        private static FactBase[] _array;
        private static bool _onlyFavorite;
        private Vector2 _scrollPos;
        private static GUIStyle _contentStyle;
        
        #endregion
    }    
}