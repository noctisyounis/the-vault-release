using System;
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
        public AssetReference   m_prefabSelector;
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

        public DebugElement CurrentElement
        {
            get
            {
                if(!_currentElement)
                    UpdateElements();
                    
                return _currentElement;
            }
            set => _currentElement = value;
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
            UpdateElements();
        }

        #endregion


        #region Public API

        public void ReturnToParent()
        {
            if (!HasParent) return;

            s_instance.Execute(ParentPath);
        }
        public void SelectNextButton()
        {
            _currentButtonChildIndex++;
            ValidateCurrentButtonChildIndex();
            UpdateElements();
        }

        public void SelectPreviousButton()
        {
            _currentButtonChildIndex--;
            ValidateCurrentButtonChildIndex();
            UpdateElements();
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

        public void UpdateElements()
        {
            if(m_parentMenuButton.childCount == 0) return;

            if(_currentElement)
                _currentElement.OnDeselected();

            var currentChild = m_parentMenuButton.GetChild(_currentButtonChildIndex);

            _currentElement = currentChild.GetComponent<DebugElement>();
            _currentElement.OnSelected();
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
                var label = item.Key;
                var path = item.Value;

                path = Clean(path, out var elementType);

                if(elementType.Equals(DebugMenuDatabase.ELEMENT_SELECTOR))
                    Spawn(m_prefabSelector, m_parentMenuButton, (selector) => InitializeNewSelector(selector, path, label));
                else
                    Spawn(m_prefabButton, m_parentMenuButton, (button) => InitializeNewElement(button, path, label));

                _buttonAmount ++;
            }

            ResponsiveMenu();
            s_instance.GeneratePanel(otherPanel, m_depth + 1);
        }

        private string Clean(string path, out string type)
        {
            var splitedPath = path.Split('/');
            
            path = BuildNextPath(splitedPath);
            type = splitedPath[^1];
            
            return path;
        }

        private void InitializeNewElement(GameObject elementObject, string path, string label)
        {
            var element = elementObject.GetComponent<DebugElement>();
            var elementText = element.GetComponentInChildren<TMP_Text>();

            element.m_path = path;
            element.m_owner = this;
            element.OnDeselected();
            
            if (elementText == null) return;
            elementText.text = label;
        }

        private void InitializeNewSelector(GameObject selectorObject, string path, string label)
        {
            var options = s_instance.GetOptionsDatas(path);
            var selector = selectorObject.GetComponentInChildren<DebugSelector>();
            
            InitializeNewElement(selectorObject, path, label);

            if (!selector) return;

            selector.Options = options;
        }

        ///<summary>Add the command of the path to the panel and return the depth of that command.</summary>
        private int AddCommandToPanel(string path, Dictionary<string, string> currentPanel)
        {
            var splitedPath = path.Split('/');
            var commandName = splitedPath[m_depth + 1];

            if (!currentPanel.ContainsKey(commandName))
            {
                var nextPath = BuildPath(splitedPath, m_depth + 2);

                currentPanel.Add(commandName, nextPath);
            }

            return splitedPath.Length - 1;
        }

        private string BuildNextPath(string[] paths)
        {
            var nextDepth = m_depth + 1;
            
            return BuildPath(paths, nextDepth);
        }
        
        private string BuildPath(string[] paths, int depth)
        {
            var result = new StringBuilder();
            var length = paths.Length;

            for (int i = 0; (i <= depth && i < length); i++)
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
        private DebugElement _currentElement;
        private int         _currentButtonChildIndex;

        #endregion
    }
}