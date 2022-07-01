using System;

namespace Universe.DebugWatch.Runtime
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class DebugMenuAttribute : Attribute
    {
        #region Public Properties

        public string Path => _path;
        public string Tooltip => _tooltip;
        public bool IsQuickMenu { get; set; }

        #endregion


        #region Constructor

        public DebugMenuAttribute(string path, string tooltip = "")
        {
            _path = path;
            _tooltip = tooltip;
        }

        #endregion


        #region Private Members

        private string _path;
        private string _tooltip;

        #endregion
    }
}