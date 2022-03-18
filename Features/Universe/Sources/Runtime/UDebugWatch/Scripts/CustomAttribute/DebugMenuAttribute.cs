using System;

namespace Universe.DebugWatch.Runtime
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class DebugMenuAttribute : Attribute
    {
        #region Public Properties

        public string Path => _path;
        public bool IsQuickMenu { get; set; }

        #endregion


        #region Constructor

        public DebugMenuAttribute(string path)
        {
            _path = path;
        }

        #endregion


        #region Private Members

        private string _path;

        #endregion
    }
}