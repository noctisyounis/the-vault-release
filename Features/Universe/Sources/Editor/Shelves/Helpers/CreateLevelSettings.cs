using UnityEditor.SceneTemplate;
using UnityEditor.AddressableAssets.Settings;

namespace Universe.Toolbar.Editor
{
    public class CreateLevelSettings : UniverseScriptableObject
	{
        #region Exposed

        public string m_levelFolder = "Assets/_/Content/Levels/Gameplay";
        public string m_playerTaskName = "Player";
        public string m_audioTaskName = "Audio";
        public string m_situationName = "Situations";
        public string m_blockMeshTaskName = "BlockMesh";
		public string m_artTaskName = "Art";
		public string m_gameplayTaskName = "Gameplay";
		public string m_addressableGroupHelperName = "LevelHelper";
		public SceneTemplateAsset m_playerSceneTemplate;
		public SceneTemplateAsset m_audioSceneTemplate;
		public SceneTemplateAsset m_blockMeshSceneTemplate;
		public SceneTemplateAsset m_artSceneTemplate;
		public SceneTemplateAsset m_gameplaySceneTemplate;
        public AddressableAssetGroupTemplate m_addressableGroupTemplate;

        #endregion
    }
}