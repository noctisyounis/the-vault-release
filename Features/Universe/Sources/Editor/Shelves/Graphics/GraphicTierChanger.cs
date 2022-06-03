using System.Linq;
using UnityEditor;
using Universe.Editor;

using static UnityEngine.GUILayout;
using static System.IO.Path;

namespace Universe.Toolbar.Editor
{
	public class GraphicTierChanger
	{
		#region Main

		public static void Draw()
		{
			if(!_settings) LoadSettings();

			_currentTargetGraphicTier = FindAssociatedTier(_settings.m_targetFolder);
			_currentFallbackGraphicTier = FindAssociatedTier(_settings.m_fallbackFolder);

			EditorGUI.BeginChangeCheck();
			Label("Target: ", Width(_labelWidth));
			_currentTargetGraphicTier = EditorGUILayout.Popup(_currentTargetGraphicTier, _graphicsTiers, Width(_popupWidth));
			Label("Default: ", Width(_labelWidth));
			_currentFallbackGraphicTier = EditorGUILayout.Popup(_currentFallbackGraphicTier, _graphicsTiers, Width(_popupWidth));

			if( !EditorGUI.EndChangeCheck() )
				return;

			_settings.m_targetFolder 	= Join(_settings.m_rootFolder, _graphicsTiers[_currentTargetGraphicTier]);
			_settings.m_fallbackFolder 	= Join(_settings.m_rootFolder, _graphicsTiers[_currentFallbackGraphicTier]);
			_settings.m_targetFolder 	= _settings.m_targetFolder.Replace(DirectorySeparatorChar, AltDirectorySeparatorChar);
			_settings.m_fallbackFolder	= _settings.m_fallbackFolder.Replace(DirectorySeparatorChar, AltDirectorySeparatorChar);

			_settings.SaveAsset();
		}

		#endregion


		#region Utils

		private static void LoadSettings()
		{
			_settings = USettingsHelper.GetSettings<UGraphicsSettings>();
		}

		private static int FindAssociatedTier(string path)
		{
			var name 	= GetFileName(path);
			var result 	= _graphicsTiers.ToList().IndexOf(name);

			return result < 0 ? 0 : result;
		}

		#endregion


		#region Private

		private static float _labelWidth = 50f;
		private static float _popupWidth = 100f;

		private static int _currentTargetGraphicTier;
		private static int _currentFallbackGraphicTier;
		private static string[] _graphicsTiers = {"Placeholder", "HD", "SD"};
		
		private static UGraphicsSettings _settings;

		#endregion
	}
}