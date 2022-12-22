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
            if (Button(GetShowFact() ? "Show signal" : "Show fact", MaxWidth(100))) SetShowFact(!GetShowFact());
            if (Button(GetShowFavorite() ? "Show all" : "Show favorite", MaxWidth(100))) SetShowFavorite(!GetShowFavorite());
            
            EditorGUILayout.EndHorizontal();

            Space(15);

            if (_facts.IsNullOrEmpty() || _signals.IsNullOrEmpty())
            {
                Initialize();
            }
            
            GUILine(1);
            
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, Width(Screen.width), Height(Screen.height - 60));

            if(GetShowFact()) DrawFactEnumeration();
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
                if (GetShowFavorite() && !fact.m_isFavorite) continue;
                
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
                if (GetShowFavorite() && !signal.m_isFavorite) continue;
                
                BeginHorizontal();
                _contentStyle.alignment = TextAnchor.MiddleLeft;
                if (Button($"{signal.name}", _contentStyle, Width(Screen.width  * 0.5f)))
                {
                    Selection.activeObject = signal;
                }
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

        private void SetShowFact(bool state) => PlayerPrefs.SetInt("[Vault]ShowFact", state ? 1 : 0);

        private bool GetShowFact()
        {
            if (!PlayerPrefs.HasKey("[Vault]ShowFact")) PlayerPrefs.SetInt("[Vault]ShowFact", 0);
            return PlayerPrefs.GetInt("[Vault]ShowFact") == 1;
        }
        
        private void SetShowFavorite(bool state) => PlayerPrefs.SetInt("[Vault]ShowFavorite", state ? 1 : 0);

        private bool GetShowFavorite()
        {
            if (!PlayerPrefs.HasKey("[Vault]ShowFavorite")) PlayerPrefs.SetInt("[Vault]ShowFavorite", 0);
            return PlayerPrefs.GetInt("[Vault]ShowFavorite") == 1;
        }
        
        #endregion
        
        
        #region Private

        private static FactBase[] _facts;
        private static SignalBase[] _signals;

        private Vector2 _scrollPos;
        private static GUIStyle _contentStyle;
        
        #endregion
    }    
}