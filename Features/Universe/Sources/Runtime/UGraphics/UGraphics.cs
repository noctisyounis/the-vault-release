using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Universe
{
    public class UGraphics : UBehaviour
    {
        #region Public

        [Header("Settings")]
        public AssetReference m_asset;

        #endregion


        #region Unity API

        private void Start() 
        {
            UpdateAsset();
            Spawn(_preferedAsset, transform.position, transform.rotation, transform, 0);
        }

        #endregion


        #region Utils

        private void UpdateAsset()
        {
            _preferedAsset = m_asset;

            var pathTable = UGraphicsManager.GetPathTable();
            var settings = UGraphicsManager.GetSettings();
            var path = pathTable.GUIDToPath(m_asset.AssetGUID);

            path = path.Replace(settings.m_fallbackFolder, settings.m_targetFolder);
        
            var targetGuid = pathTable.PathToGUID(path);

            if(targetGuid.Length != 0)
            {
                _preferedAsset = new AssetReference(targetGuid);
            }
        }

        #endregion


        #region Private

        private AssetReference _preferedAsset;

        #endregion
    }
}