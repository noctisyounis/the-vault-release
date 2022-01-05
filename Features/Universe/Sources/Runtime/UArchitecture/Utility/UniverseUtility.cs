namespace Universe
{
    public static class UniverseUtility
    {
        #region Public
        
        // Menu Order
        public const int FACT_MENU_ORDER = 121;
        public const int SIGNAL_MENU_ORDER = 122;
        public const int COLLECTION_MENU_ORDER = 123;
        
        // Sub Menus
        public const string FACT_SUBMENU = UNIVERSE_SUBMENU + "Facts/";
        public const string SET_SUBMENU = UNIVERSE_SUBMENU + "Sets/";
        public const string SIGNAL_SUBMENU = UNIVERSE_SUBMENU + "Signals/";
        
        // Add Component Menus
        public const string RECEPTOR_SUBMENU = ADD_COMPONENT_ROOT_MENU + "Receptor/";
        
        #endregion
        
        
        #region Private
        
        // Root Paths
        private const string UNIVERSE_SUBMENU = "Universe/";
        private const string ADD_COMPONENT_ROOT_MENU = "Universe/";
        
        #endregion
    }
}