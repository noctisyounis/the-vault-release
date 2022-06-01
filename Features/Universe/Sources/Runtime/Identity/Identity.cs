using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Universe
{
    [ExecuteInEditMode, DisallowMultipleComponent]
    public class Identity : UBehaviour
    {
        #region Unity API

        public override void Awake()
        {
            base.Awake();
            CreateGuid();
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (IsAssetOnDisk())
            {
                serializedGuid = null;
                guid = System.Guid.Empty;
            }
            else
#endif
            {
                CreateGuid();
            }
        }
        
        public System.Guid GetGuid()
        {
            if (guid == System.Guid.Empty && serializedGuid != null && serializedGuid.Length == 16)
            {
                guid = new System.Guid(serializedGuid);
            }

            return guid;
        }
        
        public override void OnDestroy()
        {
            IdentityManager.Remove(guid);
        }

        #endregion
        
        
        #region Main
        
        public bool IsGuidAssigned()
        {
            return guid != System.Guid.Empty;
        }

        private void CreateGuid()
        {
            if (serializedGuid == null || serializedGuid.Length != 16)
            {
    #if UNITY_EDITOR
                
                if (IsAssetOnDisk())
                {
                    return;
                }
                Undo.RecordObject(this, "Added GUID");
    #endif
                guid = System.Guid.NewGuid();
                serializedGuid = guid.ToByteArray();

    #if UNITY_EDITOR
                
                if (PrefabUtility.IsPartOfNonAssetPrefabInstance(this))
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                }
    #endif
            }
            else if (guid == System.Guid.Empty)
            {
                guid = new System.Guid(serializedGuid);
            }
            
            if (guid != System.Guid.Empty)
            {
                if (!IdentityManager.Add(this))
                {
                    serializedGuid = null;
                    guid = System.Guid.Empty;
                    CreateGuid();
                }
            }
        }

    #if UNITY_EDITOR
        private bool IsEditingInPrefabMode()
        {
            if (EditorUtility.IsPersistent(this))
            {
                return true;
            }
            else
            {
                var mainStage = StageUtility.GetMainStageHandle();
                var currentStage = StageUtility.GetStageHandle(gameObject);
                if (currentStage != mainStage)
                {
                    var prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
                    if (prefabStage != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsAssetOnDisk() => 
            PrefabUtility.IsPartOfPrefabAsset(this) || IsEditingInPrefabMode();
        
    #endif
        
        protected override void OnBeforeSerialize()
        {
    #if UNITY_EDITOR
            if (IsAssetOnDisk())
            {
                serializedGuid = null;
                guid = System.Guid.Empty;
            }
            else
    #endif
            {
                if (guid != System.Guid.Empty)
                {
                    serializedGuid = guid.ToByteArray();
                }
            }
        }

        protected override void OnAfterDeserialize()
        {
            if (serializedGuid != null && serializedGuid.Length == 16)
            {
                guid = new System.Guid(serializedGuid);
            }
        }

        #endregion
        
        
        #region Private
        
        System.Guid guid = System.Guid.Empty;
        
        [SerializeField]
        private byte[] serializedGuid;
        
        #endregion
    }
}