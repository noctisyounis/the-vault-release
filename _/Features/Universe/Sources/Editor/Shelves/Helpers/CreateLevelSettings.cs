using UnityEditor.SceneTemplate;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Universe.Toolbar.Editor
{
    public class CreateLevelSettings : USettings
	{
        #region Exposed

        public string m_levelFolder = "Assets/_/Content/Levels/Gameplay";
		public string m_blockMeshTaskName = "BlockMesh";
		public string m_artTaskName = "Art";
		public string m_gameplayTaskName = "Gameplay";
		public string m_addressableGroupHelperName = "LevelHelper";
		public SceneTemplateAsset m_sceneTemplate;
        public AddressableAssetGroupTemplate m_addressableGroupTemplate;

        #endregion
    }
}