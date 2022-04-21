using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Universe
{

    public class UGraphics : UBehaviour
    {
        #region Public

        [Header("Settings")]
        public AssetReference m_asset;
        public Action<GameObject> OnAssetLoaded = delegate { };

        [Space(15.0f)]
        public bool m_overrideSpawnTransform;
        public VertexInfo m_spawnOverride;

        [Space(15.0f)]
        public Color m_gizmosColor   = Color.blue;

        #endregion

        
        #region Unity API

        private void Start() 
        {
            UpdateAsset();
            
            if( m_overrideSpawnTransform )
            {
                Spawn( _preferedAsset, m_spawnOverride.m_position, Quaternion.Euler( m_spawnOverride.m_rotation ), m_spawnOverride.m_scale, transform, CallbackOnAssetLoaded );
                return;
            }
            
            Spawn( _preferedAsset, Vector3.zero, Quaternion.identity, Vector3.one, transform, CallbackOnAssetLoaded );
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

        private void CallbackOnAssetLoaded(GameObject go) =>
            OnAssetLoaded.Invoke(go);

        #endregion


        #region Debug and Tools

        public void OnDrawGizmosSelected()
        {
            if( !GizmosVisible )
                return;

            var position = transform.position;
            var positionOverride = m_overrideSpawnTransform ? m_spawnOverride.m_position : Vector3.zero;
            var offsetPosition = position + positionOverride;
            var rotationOverride = m_overrideSpawnTransform ? m_spawnOverride.m_rotation : Vector3.zero;
            var offsetForward = Quaternion.Euler(rotationOverride) * transform.forward;

            Gizmos.color = Color.white;
            Gizmos.DrawLine( position, offsetPosition );

            Gizmos.color = m_gizmosColor;
            Gizmos.DrawLine( offsetPosition, offsetPosition + offsetForward );
            Gizmos.DrawSphere( offsetPosition, 0.02f );
        }

        #endregion


        #region Private

        private AssetReference _preferedAsset;

        #endregion
    }
}