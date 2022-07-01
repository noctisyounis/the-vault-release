using URPDebugViews;

namespace Universe.DebugWatchTools.Runtime
{
    public class DebugViews
    {
        #region Public API

        public static void ChangeView( int viewIndex )
        {
            var views = DebugViewsManager.Instance.AllAvailableDebugViews;
            var currentView = DebugViewsManager.Instance.CurrentViewData;
            var currentIndex = views.IndexOf(currentView);
            var view = views[viewIndex];
            var next = (currentIndex == viewIndex) ? null : view;

            DebugViewsManager.Instance.EnableView( next );
        }

        #endregion
    }
}