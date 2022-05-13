using UnityEngine;
using System;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Universe
{
    [Serializable]
    public class IdentityReference : ISerializationCallbackReceiver
    {
        #region Exposed
        
        public event Action<GameObject> OnGuidAdded = delegate (GameObject go) { };
        public event Action OnGuidRemoved = delegate() { };
        
        #endregion
        
        
        #region Properties
        
        public GameObject gameObject
        {
            get
            {
                if( _isCacheSet )
                {
                    return _cachedReference;
                }

                _cachedReference = IdentityManager.ResolveGuid( _guid, _addDelegate, _removeDelegate );
                _isCacheSet = true;
                return _cachedReference;
            }

            private set {}
        }
        
        #endregion
        
        
        #region Constructor

        public IdentityReference() { }

        public IdentityReference(Identity target)
        {
            _guid = target.GetGuid();
        }
        
        #endregion
        
        
        #region Main Methods

        private void GuidAdded(GameObject go)
        {
            _cachedReference = go;
            OnGuidAdded(go);
        }

        private void GuidRemoved()
        {
            _cachedReference = null;
            _isCacheSet = false;
            OnGuidRemoved();
        }
        
        public void OnBeforeSerialize()
        {
            _serializedGuid = _guid.ToByteArray();
        }
        
        public void OnAfterDeserialize()
        {
            _cachedReference = null;
            _isCacheSet = false;
            if (_serializedGuid == null || _serializedGuid.Length != 16)
            {
                _serializedGuid = new byte[16];
            }
            _guid = new System.Guid(_serializedGuid);
            _addDelegate = GuidAdded;
            _removeDelegate = GuidRemoved;

        }
        
        #endregion
        
        
        #region Private
        
        private GameObject _cachedReference;
        private bool _isCacheSet;
        
        [FormerlySerializedAs("serializedGuid")] [SerializeField]
        private byte[] _serializedGuid;
        private System.Guid _guid;
        
#if UNITY_EDITOR
        [FormerlySerializedAs("cachedName")] [SerializeField]
        private string _cachedName;
        [FormerlySerializedAs("cachedScene")] [SerializeField]
        private SceneAsset _cachedScene;
#endif
        
        private Action<GameObject> _addDelegate;
        private Action _removeDelegate;
        
        #endregion
    }
}