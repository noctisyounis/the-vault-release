using System.Collections.Generic;
using CSharpExtensions.Runtime;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Universe.XRDeviceType;

namespace Universe
{
    public class XRProvider : UBehaviour
    {
        #region Public

        public AssetReference m_asset;
        
        #endregion
        
        
        #region Public API

        public XRAnchorPulledData GetPlayAreaData() =>
            GetDataOf( PLAY_AREA );

        public XRAnchorPulledData GetHeadsetData() =>
            GetDataOf( HEADSET );

        public XRAnchorPulledData GetLeftControllerData() =>
            GetDataOf( LEFT_CONTROLLER );
        
        public XRAnchorPulledData GetRightControllerData() =>
            GetDataOf( RIGHT_CONTROLLER );

        #endregion

        
        #region Utils
        
        public void SetXRRig(GameObject spawnedObject)
        {
            if (!spawnedObject.TryGetComponent(out PrefabHierarchyHolder holder)) return;

            _trackedXRDico.Add( PLAY_AREA, new XRAnchorPulledData(spawnedObject.transform));
            _trackedXRDico.Add( HEADSET, new XRAnchorPulledData(holder.GetGameObject("Camera").transform));
            _trackedXRDico.Add( LEFT_CONTROLLER, new XRAnchorPulledData(holder.GetGameObject("LeftHand").transform));
            _trackedXRDico.Add( RIGHT_CONTROLLER, new XRAnchorPulledData(holder.GetGameObject("RightHand").transform));
        }

        private XRAnchorPulledData GetDataOf(XRDeviceType deviceType) =>
            _trackedXRDico[deviceType];
        
        #endregion
        

        #region Private & Protected

        protected Dictionary<XRDeviceType, XRAnchorPulledData> _trackedXRDico = new();

        #endregion
    }
}