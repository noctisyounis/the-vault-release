using System;

namespace Universe.DebugWatch.Runtime
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class DebugMenuAttribute : Attribute
    {
        #region Public Properties

        public string Path => _path;
        public string Tooltip => _tooltip;
        public int SortingOrder => _sortingOrder;
        public bool IsQuickMenu { get; set; }

        #endregion


        #region Constructor

        public DebugMenuAttribute(string path, string tooltip = "", int sortingOrder = 0)
        {
            _path = path;
            _tooltip = string.IsNullOrEmpty(tooltip) ? "" : tooltip;
            _sortingOrder = sortingOrder;
        }

        #endregion
        
        
        #region Main

        public virtual OptionData[] GetOptions() => 
            new OptionData[0];

        #endregion


        #region Private Members

        protected int _sortingOrder;
        protected string _path;
        protected string _tooltip;

        #endregion
    }
}