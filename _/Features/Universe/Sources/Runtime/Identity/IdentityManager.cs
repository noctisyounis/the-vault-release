using System.Collections.Generic;
using UnityEngine;
using System;

namespace Universe
{
    public class IdentityManager
    {
        #region Exposed
        
        static IdentityManager Instance;
        
        #endregion
        
        
        #region Structs
        
        private struct IdentityInfo
        {
            public GameObject m_go;

            public event Action<GameObject> OnAdd;
            public event Action OnRemove;

            public IdentityInfo(Identity comp)
            {
                m_go = comp.gameObject;
                OnRemove = null;
                OnAdd = null;
            }

            public void HandleAddCallback()
            {
                if (OnAdd != null)
                {
                    OnAdd(m_go);
                }
            }

            public void HandleRemoveCallback()
            {
                if (OnRemove != null)
                {
                    OnRemove();
                }
            }
        }
        
        #endregion
        
        
        #region Main Methods

        public static bool Add(Identity identity )
        {
            if (Instance == null)
            {
                Instance = new IdentityManager();
            }

            return Instance.InternalAdd(identity);
        }

        public static void Remove(System.Guid guid)
        {
            if (Instance == null)
            {
                Instance = new IdentityManager();
            }

            Instance.InternalRemove(guid);
        }
        public static GameObject ResolveGuid(System.Guid guid, Action<GameObject> onAddCallback, Action onRemoveCallback)
        {
            if (Instance == null)
            {
                Instance = new IdentityManager();
            }

            return Instance.ResolveGuidInternal(guid, onAddCallback, onRemoveCallback);
        }

        public static GameObject ResolveGuid(System.Guid guid, Action onDestroyCallback)
        {
            if (Instance == null)
            {
                Instance = new IdentityManager();
            }

            return Instance.ResolveGuidInternal(guid, null, onDestroyCallback);
        }

        public static GameObject ResolveGuid(System.Guid guid)
        {
            if (Instance == null)
            {
                Instance = new IdentityManager();
            }
            return Instance.ResolveGuidInternal(guid, null, null);
        }
        
        private Dictionary<System.Guid, IdentityInfo> guidToObjectMap;

        private IdentityManager()
        {
            guidToObjectMap = new Dictionary<System.Guid, IdentityInfo>();
        }

        private bool InternalAdd(Identity identity)
        {
            Guid guid = identity.GetGuid();

            IdentityInfo info = new IdentityInfo(identity);

            if (!guidToObjectMap.ContainsKey(guid))
            {
                guidToObjectMap.Add(guid, info);
                return true;
            }

            IdentityInfo existingInfo = guidToObjectMap[guid];
            if ( existingInfo.m_go != null && existingInfo.m_go != identity.gameObject )
            {
                if (Application.isPlaying)
                {
                    Debug.AssertFormat(false, identity, "Guid Collision Detected between {0} and {1}.\nAssigning new Guid. Consider tracking runtime instances using a direct reference or other method.", (guidToObjectMap[guid].m_go != null ? guidToObjectMap[guid].m_go.name : "NULL"), (identity != null ? identity.name : "NULL"));
                }
                else
                {
                    Debug.LogWarningFormat(identity, "Guid Collision Detected while creating {0}.\nAssigning new Guid.", (identity != null ? identity.name : "NULL"));
                }
                return false;
            }
            
            existingInfo.m_go = info.m_go;
            existingInfo.HandleAddCallback();
            guidToObjectMap[guid] = existingInfo;
            return true;
        }

        private void InternalRemove(System.Guid guid)
        {
            IdentityInfo info;
            if (guidToObjectMap.TryGetValue(guid, out info))
            {
                info.HandleRemoveCallback();
            }

            guidToObjectMap.Remove(guid);
        }
        
        private GameObject ResolveGuidInternal(System.Guid guid, Action<GameObject> onAddCallback, Action onRemoveCallback)
        {
            IdentityInfo info;
            if (guidToObjectMap.TryGetValue(guid, out info))
            {
                if (onAddCallback != null)
                {
                    info.OnAdd += onAddCallback;
                }

                if (onRemoveCallback != null)
                {
                    info.OnRemove += onRemoveCallback;
                }
                guidToObjectMap[guid] = info;
                return info.m_go;
            }

            if (onAddCallback != null)
            {
                info.OnAdd += onAddCallback;
            }

            if (onRemoveCallback != null)
            {
                info.OnRemove += onRemoveCallback;
            }

            guidToObjectMap.Add(guid, info);
            
            return null;
        }
        
        #endregion
    }
}