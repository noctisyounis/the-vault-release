using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.AddressableAssets;

using static UnityEngine.InputSystem.InputAction;

namespace Universe.DebugWatch.Runtime
{
    public class DebugMenuRoot : UBehaviour
    {
        #region Public Members

        public static DebugMenuRoot m_instance;
        public DebugMenuData        m_bakedData;
        public string               m_debugMenuName;
        public AssetReference       m_debugMenuPanel;

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
            m_instance      = this;
            _menus          = new Dictionary<string, DebugPanel>();
            _bufferedPaths  = new();

            StartGeneration();
        }

        #endregion


        #region Public API

        public void SelectionAxisChanged(CallbackContext context)
        {
            if(!(context.interaction is PressInteraction)) return;
            
            var value = context.ReadValue<Vector2>();

            if(value.y < 0) SelectNextButton();
            if(value.y > 0) SelectPreviousButton();
        }

        public void SelectNextButton() => CurrentMenu.SelectNextButton();
        public void SelectPreviousButton() => CurrentMenu.SelectPreviousButton();
        public void Submit() => CurrentMenu.CurrentButton.OnClick();
        public void Back() => CurrentMenu.ReturnToParent();

        public void Execute(string path)
        {
            if (IsValidMenu(path))
            {
                DisplayPanel(path);
                return;
            }
            
            InvokeMethod(path);
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
            CurrentMenu.UpdateButtons();
        }

        private void HidePanels()
        {
            foreach (var item in _menus) item.Value.gameObject.SetActive(false);
        }

        public void InvokeMethod(string path)
        {
            Verbose($"Action At {path}");
            m_bakedData.InvokeMethod(UnlinkPathFromRoot(path));
        }

        private void StartGeneration()
        {
            if (_generated) return;
            
            var debugPath   = m_bakedData.GetPaths();
            var rootedPaths = LinkPathsToRoot(new List<string>(debugPath));

            GeneratePanel(rootedPaths, 0);
            _generated = true;
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