using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

using static UnityEngine.Color;
using static UnityEngine.Gizmos;
using static UnityEngine.Quaternion;
using static UnityEngine.Vector3;
using static Universe.UGraphicsManager;

namespace Universe
{
    public class UGraphics : UBehaviour
    {
        #region Exposed

        [Header("Settings")]
        public AssetReference m_asset;
        public Action<GameObject> OnAssetLoaded = delegate { };
        [Tooltip("When true, this UGraphics will find the parent UGraphics to fire the event OnAssetLoaded instead of here.")]
        public bool m_propagateEventUpwards = false;

        [Header("Spawn Transform Override"), Space(15.0f)]
        public bool m_overrideSpawnTransform;
        public VertexInfo m_spawnOverride;

        [Space(15.0f)]
        public Color m_gizmosColor   = blue;

        #endregion

        
        #region Unity API

        private void Start() 
        {
            ConvertAssetReferenceToPreferredGraphicTier();
            
            var callback = GetDesiredCallback();
            if( m_overrideSpawnTransform )
            {
                Spawn( _preferredAsset, m_spawnOverride.m_position, Euler( m_spawnOverride.m_rotation ), m_spawnOverride.m_scale, transform, callback );
                return;
            }
            
            Spawn( _preferredAsset, zero, identity, one, transform, callback );
        }

        #endregion
        
        #region Main
        
        private void ConvertAssetReferenceToPreferredGraphicTier()
        {
            _preferredAsset = m_asset;

            var pathTable = GetPathTable();
            var settings = GetSettings();
            var path = pathTable.GUIDToPath(m_asset.AssetGUID);

            path = path.Replace(settings.m_fallbackFolder, settings.m_targetFolder);
        
            var targetGuid = pathTable.PathToGUID(path);

            if(targetGuid.Length != 0)
            {
                _preferredAsset = new AssetReference(targetGuid);
            }
        }

        private Action<GameObject> GetDesiredCallback()
        {
            if (!m_propagateEventUpwards) return CallbackOnAssetLoaded;

            var parentCallback = GetComponentsInParent<UGraphics>();
            if (parentCallback.Length > 0)
            {
                return  parentCallback.Last().OnAssetLoaded;
            }

            throw new NullReferenceException($"UGraphcs {name} tried to propagate event upwards but no parent have UGraphics available");
        }
        
        #endregion


        #region Utils

        private void CallbackOnAssetLoaded(GameObject go) =>
            OnAssetLoaded.Invoke(go);

        #endregion


        #region Debug and Tools

        public void OnDrawGizmosSelected()
        {
            if( !GizmosVisible ) return;

            var position = transform.position;
            var positionOverride = m_overrideSpawnTransform ? m_spawnOverride.m_position : zero;
            var offsetPosition = position + positionOverride;
            var rotationOverride = m_overrideSpawnTransform ? m_spawnOverride.m_rotation : zero;
            var offsetForward = Euler(rotationOverride) * transform.forward;

            color = white;
            Gizmos.DrawLine( position, offsetPosition );

            color = m_gizmosColor;
            Gizmos.DrawLine( offsetPosition, offsetPosition + offsetForward );
            Gizmos.DrawSphere( offsetPosition, 0.02f );
        }

        #endregion


        #region Private

        private AssetReference _preferredAsset;

        #endregion
    }
}