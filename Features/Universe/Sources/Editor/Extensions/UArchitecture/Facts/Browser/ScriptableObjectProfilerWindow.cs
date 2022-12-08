using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

using static UnityEngine.GUILayout;

namespace Universe.Editor
{
    public class ScriptableObjectProfilerWindow : EditorWindow
    {
        #region Menu Item

        [MenuItem("Vault/Scriptable Object/Profiler")]
        public static void ShowDialog()
        {
            Initialize();
            GetWindow<ScriptableObjectProfilerWindow>();
        }

        #endregion
        
        
        #region Main

        private void OnGUI()
        {
            _contentStyle = new GUIStyle(GUI.skin.button);
            
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (Button("Refresh", MaxWidth(100))) _facts =  GetAllInstances<FactBase>();
            Space(15);
            if (Button(_showFact ? "Show signal" : "Show fact", MaxWidth(100))) _showFact = !_showFact;
            if (Button( _onlyFavorite ? "Show all" : "Show favorite", MaxWidth(100))) _onlyFavorite = !_onlyFavorite;
            
            EditorGUILayout.EndHorizontal();

            Space(15);

            if (_facts.IsNullOrEmpty() || _signals.IsNullOrEmpty())
            {
                Initialize();
            }
            
            GUILine(1);
            
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, Width(Screen.width), Height(Screen.height - 60));

            if(_showFact) DrawFactEnumeration();
            else DrawSignalEnumeration();

            EditorGUILayout.EndScrollView();
        }
        
        #endregion
        
        
        #region Utils

        private static void Initialize()
        {
            _facts =  GetAllInstances<FactBase>();
            _signals = GetAllInstances<SignalBase>();
        }
        
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

        private void DrawFactEnumeration()
        {
            foreach (var fact in _facts)
            {
                if (_onlyFavorite && !fact.m_isFavorite) continue;
                
                BeginHorizontal();
                _contentStyle.alignment = TextAnchor.MiddleLeft;
                if (Button($"{fact.name}", _contentStyle, Width(Screen.width  * 0.5f)))
                {
                    Selection.activeObject = fact;
                }
                
                Label($"=> {fact.ToString()}");
                EndHorizontal();
                GUILine(1);
            }
        }

        private void DrawSignalEnumeration()
        {
            foreach (var signal in _signals)
            {
                if (_onlyFavorite && !signal.m_isFavorite) continue;
                
                BeginHorizontal();
                _contentStyle.alignment = TextAnchor.MiddleLeft;
                if (Button($"{signal.name}", _contentStyle, Width(Screen.width  * 0.5f)))
                {
                    Selection.activeObject = signal;
                }
                //Label($"=> {signal.ToString()}");
                EndHorizontal();
                GUILine(1);
            }
        }
        
        private void GUILine( int i_height = 1 )
        {

            Rect rect = EditorGUILayout.GetControlRect(false, i_height );

            rect.height = i_height;

            EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );

        }
        
        #endregion
        
        
        #region Private

        private static FactBase[] _facts;
        private static SignalBase[] _signals;
        
        private static bool _showFact;
        private static bool _onlyFavorite;
        
        private Vector2 _scrollPos;
        private static GUIStyle _contentStyle;
        
        #endregion
    }    
}