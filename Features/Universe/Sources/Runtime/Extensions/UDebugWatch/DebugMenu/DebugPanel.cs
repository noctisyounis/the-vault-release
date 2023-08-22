using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using TMPro;
using UnityEngine.UI;
using static Universe.DebugWatch.Runtime.DebugMenuRoot;

namespace Universe.DebugWatch.Runtime
{
    public class DebugPanel : UBehaviour
    {
        #region Public Members

        public bool m_invertedScrolling;

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
        public ScrollRect       m_scrollRect;

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

        public override void OnEnable()
        {
            base.OnEnable();
            if(m_parentMenuButton.childCount == 0) return;

            _currentButtonChildIndex = 0;
            UpdateElements();
        }

        #endregion


        #region UniverseAPI

        public override void OnUpdate(float deltatime)
        {
            UpdateScroll();
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
            _currentScroll = ComputeScrollAt(0);
            _currentUpperScroll = _currentScroll;
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

            var scroll = ComputeScrollAt(_currentButtonChildIndex);
            EvaluateNewScroll(scroll);
        }

        public void Scroll(float speed)
        {
            var delta = speed * UTime.UnscaledDeltaTime;
            if (m_invertedScrolling) delta *= -1;

            _currentScroll = Mathf.Clamp01(_currentScroll + delta);
            _currentLowerScroll = Mathf.Clamp(_currentLowerScroll + delta, 0.0f, 1.0f - _scrollSpan);
            _currentUpperScroll = Mathf.Clamp(_currentUpperScroll + delta, 0.0f + _scrollSpan, 1.0f);
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

        private void ResponsiveMenu()
        {
            var titleHeigth     = m_headerTitle.rectTransform.rect.height;
            var parentWidth     = m_parentMenuButton.rect.width;
            var backGroundWidth = m_backgroundMenu.rect.width;
            var displayedAmount = IsBelowDisplayCapacity ? _buttonAmount : m_displayCapacity;
            var menuSize        = m_buttonHeight * _buttonAmount;
            var maskSize        = m_buttonHeight * displayedAmount;
            
            m_backgroundMenu.sizeDelta      = new Vector2(backGroundWidth, titleHeigth + maskSize + m_textSpacing);
            m_mask.sizeDelta                = new Vector2(backGroundWidth, maskSize);
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
            
            _scrollOffsetStep = 1.0f;
            _scrollElementStep = 1.0f;
            
            if (_buttonAmount > (m_displayCapacity + 1)) _scrollOffsetStep /= (_buttonAmount - m_displayCapacity);
            if (_buttonAmount > 1) _scrollElementStep /= (_buttonAmount - 1);
            
            _scrollSpan = Mathf.Clamp01(Mathf.Floor(m_displayCapacity) / _buttonAmount);
            _currentLowerScroll = _currentUpperScroll - _scrollSpan;
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

        private void UpdateScroll()
        {
            var current = m_scrollRect.verticalNormalizedPosition;

            m_scrollRect.verticalNormalizedPosition = Mathf.Lerp(current, _currentScroll, 0.5f);
        }
        
        private float ComputeScrollAt(int index)
        {
            var result = 1 - (index * _scrollElementStep);
            
            return Mathf.Clamp01(result);
        }

        private void EvaluateNewScroll(float scroll)
        {
            if (scroll < _currentLowerScroll)
            {
                var delta = _currentLowerScroll - scroll;
                var steps = delta / _scrollElementStep;
                
                steps = Mathf.Clamp(steps, 1, _buttonAmount);
                _currentScroll = Mathf.Clamp01(_currentScroll - (steps * _scrollOffsetStep));
                _currentLowerScroll = Mathf.Clamp01(_currentLowerScroll - steps * _scrollElementStep);
                _currentUpperScroll = Mathf.Clamp01(_currentLowerScroll + _scrollSpan);
            }

            if (scroll > _currentUpperScroll)
            {
                var delta = scroll - _currentUpperScroll;
                var steps = delta / _scrollElementStep;
                
                steps = Mathf.Clamp(steps, 1, _buttonAmount);
                _currentScroll = Mathf.Clamp01(_currentScroll + (steps * _scrollOffsetStep));
                _currentUpperScroll = Mathf.Clamp01(_currentUpperScroll + (steps * _scrollElementStep));
                _currentLowerScroll = Mathf.Clamp01(_currentUpperScroll - _scrollSpan);
            }
        }

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
        
        private float       _currentLowerScroll;
        private float       _currentUpperScroll;
        private float       _scrollElementStep;
        private float       _scrollOffsetStep;
        private float       _scrollSpan;
        private float       _currentScroll;

        #endregion
    }
}