using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using TMPro;

using static Universe.DebugWatch.Runtime.DebugMenuRoot;

namespace Universe.DebugWatch.Runtime
{
    public class DebugPanel : UBehaviour
    {
        #region Public Members

        public int m_depth;

        public float m_buttonHeight;
        public float m_displayCapacity;
        public float m_textSpacing;
        
        public TMP_Text         m_headerTitle;
        public RectTransform    m_backgroundMenu;
        public AssetReference   m_prefabButton;
        public RectTransform    m_parentMenuButton;
        public RectTransform    m_mask;

        #endregion


        #region Public Properties

        public List<string> Paths { get; set; }
        public string Title
        {
            get => m_headerTitle.text;
            set => m_headerTitle.text = value;
        }

        public string ParentPath
        {
            get => _parent;
            set => _parent = value;
        }

        public DebugButton CurrentButton
        {
            get => _currentButton;
            set => _currentButton = value;
        }

        #endregion


        #region Unity API

#if UNITY_EDITOR
        private void OnValidate() 
        {
            var buttonAsset = ((GameObject)m_prefabButton.editorAsset);
            if(!buttonAsset) return;

            m_buttonHeight = buttonAsset.GetComponent<RectTransform>().rect.height;
        }
#endif

        private void OnEnable()
        {
            if(m_parentMenuButton.childCount == 0) return;

            _currentButtonChildIndex = 0;
            UpdateButtons();
        }

        #endregion


        #region Public API

        public void ReturnToParent()
        {
            if (!HasParent) return;

            m_instance.Execute(ParentPath);
        }
        public void SelectNextButton()
        {
            _currentButtonChildIndex++;
            ValidateCurrentButtonChildIndex();
            UpdateButtons();
        }

        public void SelectPreviousButton()
        {
            _currentButtonChildIndex--;
            ValidateCurrentButtonChildIndex();
            UpdateButtons();
        }
        
        public void StartGenerate()
        {
            if (_isGenerated) return;

            _isGenerated = true;
            GenerateButtons(Paths.ToArray());
        }

        public void AddPath(string path)
        {
            var paths = new string[]{path};

            Paths.Add(path);
            GenerateButtons(paths);
        }

        public void UpdateButtons()
        {
            if(m_parentMenuButton.childCount == 0) return;

            if(_currentButton)
                _currentButton.HideArrow();

            var currentChild = m_parentMenuButton.GetChild(_currentButtonChildIndex);

            _currentButton = currentChild.GetComponent<DebugButton>();
            _currentButton.DisplayArrow();
        }

        #endregion


        #region Utils

        private void ValidateCurrentButtonChildIndex()
        {
            var childCount = m_parentMenuButton.childCount;

            if (_currentButtonChildIndex < 0)
                _currentButtonChildIndex = childCount - 1;

            if (_currentButtonChildIndex >= childCount)
                _currentButtonChildIndex = 0;
        }

        #endregion


        #region Utils

        private void ResponsiveMenu()
        {
            var titleHeigth     = m_headerTitle.rectTransform.rect.height;
            var parentWidth     = m_parentMenuButton.rect.width;
            var backGroundWidth = m_backgroundMenu.rect.width;
            var displayedAmount = IsBelowDisplayCapacity ? _buttonAmount : m_displayCapacity;
            var menuSize        = m_buttonHeight * displayedAmount;

            m_backgroundMenu.sizeDelta      = new Vector2(backGroundWidth, titleHeigth + menuSize + m_textSpacing);
            m_mask.sizeDelta                = new Vector2(backGroundWidth, menuSize);
            m_parentMenuButton.sizeDelta    = new Vector2(parentWidth, menuSize);
        }

        private void GenerateButtons(string[] paths)
        {
            var currentPanel    = new Dictionary<string, string>();
            var otherPanel      = new List<string>();
            var amount          = paths.Length;

            for (var i = 0; i < amount; i++)
            {
                var path    = paths[i];
                var depth   = AddCommandToPanel(path, currentPanel);

                if (!IsOtherPanel(depth)) continue;

                otherPanel.Add(path);
            }

            foreach (var item in currentPanel)
            {
                Spawn(m_prefabButton, m_parentMenuButton, (buttonObject) => InitializeNewButton(buttonObject, item));

                _buttonAmount ++;
            }

            ResponsiveMenu();
            DebugMenuRoot.m_instance.GeneratePanel(otherPanel, m_depth + 1);
        }

        private void InitializeNewButton(GameObject buttonObject, KeyValuePair<string, string> item)
        {
            var button = buttonObject.GetComponent<DebugButton>();
            var buttonText = button.GetComponentInChildren<TMP_Text>();

            buttonText.text = item.Key;
            button.m_path = item.Value;
            button.m_owner = this;
        }

        ///<summary>Add the command of the path to the panel and return the depth of that command.</summary>
        private int AddCommandToPanel(string path, Dictionary<string, string> currentPanel)
        {
            var splitedPath = path.Split('/');
            var commandName = splitedPath[m_depth + 1];

            if (!currentPanel.ContainsKey(commandName))
            {
                var nextPath = BuildNextPath(splitedPath);

                currentPanel.Add(commandName, nextPath);
            }

            return splitedPath.Length;
        }

        private string BuildNextPath(string[] paths)
        {
            var result = new StringBuilder();
            var nextDepth = m_depth + 1;

            for (int i = 0; i <= nextDepth; i++)
            {
                if (i != 0)
                    result.Append("/");
                
                result.Append(paths[i]);
            }

            return result.ToString();
        }

        private bool IsOtherPanel(int commandDepth) => 
            commandDepth - m_depth > 2;

        #endregion


        #region Private Properties

        private bool HasParent              => 
            !string.IsNullOrEmpty(ParentPath);
        private bool IsBelowDisplayCapacity => 
            _buttonAmount < m_displayCapacity;

        #endregion


        #region Private Members

        private int         _buttonAmount;
        private string      _parent;
        private bool        _isGenerated;
        private DebugButton _currentButton;
        private int         _currentButtonChildIndex;

        #endregion
    }
}