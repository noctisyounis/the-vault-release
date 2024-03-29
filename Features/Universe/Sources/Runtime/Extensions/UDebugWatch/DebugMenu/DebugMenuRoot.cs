using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using TMPro;

using static UnityEngine.InputSystem.InputAction;

namespace Universe.DebugWatch.Runtime
{
    public class DebugMenuRoot : UBehaviour
    {
        #region Public Members

        [Header("Generation")]
        public static DebugMenuRoot s_instance;
        public DebugMenuDatabase        m_bakedDatabase;
        public string               m_debugMenuName;
        public AssetReference       m_debugMenuPanel;
        public GameObject           m_tooltipRoot;
        public TMP_Text             m_tooltipText;

        [Header("Inputs")]
        public AxisToButtonConverter m_selection;
        public AxisToButtonConverter m_submission;
        public AxisToButtonConverter m_nextOption;
        public AxisToButtonConverter m_previousOption;

        #endregion


        #region Public Properties

        public DebugPanel CurrentMenu
        {
            get => _currentMenu;
            private set => _currentMenu = value;
        }

        #endregion


        #region Unity API

        public override void Awake()
        {
            s_instance      = this;
            _menus          = new Dictionary<string, DebugPanel>();
            _bufferedPaths  = new();

            m_selection.OnAxisPressed += UpdateSelection;
            m_submission.OnAxisPressed += value => Submit();
            m_nextOption.OnAxisPressed += value => SetOption(1);
            m_previousOption.OnAxisPressed += value => SetOption(-1);

            StartGeneration();
        }

        #endregion


        #region Public API

        public void SelectionAxisChanged(CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();

            m_selection.Evaluate( value.y );
        }

        public void SubmissionAxisChanged( CallbackContext context )
        {
            var value = context.ReadValue<float>();

            m_submission.Evaluate( value );
        }
        
        public void NextOptionAxisChanged(CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();

            m_nextOption.Evaluate( value.x );
        }

        public void PreviousOptionAxisChanged( CallbackContext context )
        {
            var value = context.ReadValue<Vector2>();
            
            m_previousOption.Evaluate( value.x );
        }

        public void ScrollAxisChanged(CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();

            _currentMenu.Scroll(value.y);
        }

        public void SelectNextButton() => CurrentMenu.SelectNextButton();
        public void SelectPreviousButton() => CurrentMenu.SelectPreviousButton();

        public void Back() => CurrentMenu.ReturnToParent();
        
        public void Submit()
        {
            var element = CurrentMenu.CurrentElement;

            if (!(element is IClickable clickable)) return;
            
            clickable.OnClick();
        }

        public void SetOption(int next)
        {
            var element = CurrentMenu.CurrentElement;

            if (!(element is ISettable settable)) return;
            
            settable.SetOption(next);
        }

        public void Execute(string path)
        {
            if (IsValidMenu(path))
            {
                DisplayPanel(path);
                return;
            }
            
            InvokeMethod(path);
        }
        
        public void Execute(string path, int option)
        {
            if (IsValidMenu(path))
            {
                DisplayPanel(path);
                return;
            }
            
            InvokeMethod(path, option);
        }
        
        public T Execute<T>(string path)
        {
            if (IsValidMenu(path))
            {
                DisplayPanel(path);
                return default;
            }
            
            return InvokeMethod<T>(path);
        }
        
        public T Execute<T>(string path, int option)
        {
            if (IsValidMenu(path))
            {
                DisplayPanel(path);
                return default;
            }
            
            return InvokeMethod<T>(path, option);
        }

        public void DisplayTooltip( string path )
        {
            var tooltip = m_bakedDatabase.GetTooltip( UnlinkPathFromRoot(path) );
            var isEmpty = string.IsNullOrEmpty(tooltip);

            m_tooltipRoot.SetActive( !isEmpty );
            m_tooltipText.text = tooltip;
        }

        public string[] GetOptionNames(string path)
        {
            path = UnlinkPathFromRoot(path);
            
            return m_bakedDatabase.GetOptionNames(path);
        }

        public OptionData[] GetOptionsDatas(string path)
        {
            path = UnlinkPathFromRoot(path);

            return m_bakedDatabase.GetOptionDatas(path);
        }

        public object GetLastResult(string path)
        {
            path = UnlinkPathFromRoot(path);

            return m_bakedDatabase.GetLastResult(path);
        }

        #endregion


        #region Main

        public void GeneratePanel(List<string> paths, int depth)
        {
            foreach (var path in paths)
            {
                var commands = path.Split('/');
                var panelName = commands[depth];
                var panelPath = "";
                var parentPath = "";

                panelPath = BuildPath(depth, commands, panelPath);
                parentPath = BuildPath(depth - 1, commands, parentPath);

                TrySpawnNewPanel(depth, panelName, panelPath, parentPath);
                AddPathToPanel(path, panelPath);
            }
        }

        private void AddPathToPanel(string path, string panelPath)
        {
            if (IsValidMenu(panelPath))
            {
                _menus[panelPath].AddPath(path);
                return;
            }
            
            if (!IsBuffered(panelPath))
                _bufferedPaths.Add(panelPath, new());
            
            _bufferedPaths[panelPath].Add(path);
        }

        private void TrySpawnNewPanel(int depth, string panelName, string panelPath, string parentPath)
        {
            if (!IsBuffered(panelPath))
            {
                Spawn(m_debugMenuPanel, transform, (menuPanel) =>
                {
                    var panel = menuPanel.GetComponent<DebugPanel>();
                    var bufferedPaths = new List<string>();
                    var rectTransform = ((RectTransform)panel.transform);

                    if (IsBuffered(panelPath))
                        bufferedPaths = _bufferedPaths[panelPath];

                    panel.Title = panelName;
                    panel.m_depth = depth;
                    panel.Paths = bufferedPaths;
                    panel.ParentPath = parentPath;
                    rectTransform.anchoredPosition = Vector3.zero;

                    panel.StartGenerate();
                    _menus.Add(panelPath, panel);
                    DisplayPanel(m_debugMenuName);
                });
            }
        }

        public void DisplayPanel(string path)
        {
            HidePanels();
            var menu = _menus[path];

            menu.gameObject.SetActive(true);
            CurrentMenu = menu;
            CurrentMenu.UpdateElements();
        }

        private void HidePanels()
        {
            foreach (var item in _menus) item.Value.gameObject.SetActive(false);
        }

        public void InvokeMethod(string path)
        {
            Verbose($"Action At {path}");
            m_bakedDatabase.InvokeMethod(UnlinkPathFromRoot(path));
        }
        
        public void InvokeMethod(string path, int option)
        {
            Verbose($"Action At {path} with option n°{option}");
            m_bakedDatabase.InvokeMethod(UnlinkPathFromRoot(path), option);
        }
        
        public T InvokeMethod<T>(string path)
        {
            Verbose($"Action At {path}");
            return m_bakedDatabase.InvokeMethod<T>(UnlinkPathFromRoot(path));
        }
        
        public T InvokeMethod<T>(string path, int option)
        {
            Verbose($"Action At {path} with option n°{option}");
            return m_bakedDatabase.InvokeMethod<T>(UnlinkPathFromRoot(path), option);
        }

        private void StartGeneration()
        {
            if (_generated) return;
            
            var debugPath   = m_bakedDatabase.GetPathsWithType();
            var rootedPaths = LinkPathsToRoot(new List<string>(debugPath));

            GeneratePanel(rootedPaths, 0);
            _generated = true;
        }

        private void UpdateSelection( float value )
        {
            if( value < 0 )
            {
                SelectNextButton();
                return;
            }

            if( value > 0 )
            {
                SelectPreviousButton();
                return;
            }
        }

        #endregion


        #region Utils

        private List<string> LinkPathsToRoot(List<string> paths)
        {
            var result = new List<string>();

            foreach (var path in paths) result.Add($"{m_debugMenuName}/{path}");

            return result;
        }

        private string UnlinkPathFromRoot(string path)
        {
            if( string.IsNullOrEmpty( path ) )
                return path;
            
            var result = path.Remove(0, m_debugMenuName.Length + 1);
            
            return result;
        }

        private static string BuildPath(int depth, string[] commands, string path)
        {
            var builder = new StringBuilder();
            builder.Append(path);

            for (var i = 0; i <= depth; i++)
            {
                if (i != 0)
                    builder.Append("/");

                builder.Append(commands[i]);
            }

            return builder.ToString();
        }

        private bool IsBuffered(string panelPath) => 
            _bufferedPaths.ContainsKey(panelPath);
        private bool IsValidMenu(string panelPath) => 
            _menus.ContainsKey(panelPath);

        #endregion


        #region Private Members

        private Dictionary<string, DebugPanel>      _menus;
        private Dictionary<string, List<string>>    _bufferedPaths;

        private bool        _generated;
        private DebugPanel  _currentMenu;

        #endregion
    }
}