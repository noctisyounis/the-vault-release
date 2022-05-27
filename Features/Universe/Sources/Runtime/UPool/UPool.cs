using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Universe
{
    public class UPool 
    {
        #region Public API

        public static GameObject GetOrCreate(AssetReference assetReference)
        {
            return _pool.ContainsKey( assetReference ) ? Get(assetReference) : Create(assetReference);
        }

        private static GameObject Get(AssetReference assetReference)
        {
            // var pooledAssets  = _pool[assetReference];
            // foreach (var asset in pooledAssets)
            // {
            //     foreach (var t in asset.m_assets)
            //     {
            //         var pooledAsset = t;
            //         if (pooledAsset.m_used != false) continue;
            //
            //         pooledAsset.m_used = true;
            //         return pooledAsset.m_gameobject;
            //     }
            // }
            //
            // Debug.LogError($"Can't Find {assetReference} and will not spawn");
            // return null;

            return new GameObject();
        }

        private static GameObject Create(AssetReference assetReference)
        {
            return new GameObject();
        }
        
        #endregion

        
        #region Private
        
        private static Dictionary<AssetReference, PooledSet> _pool = new Dictionary<AssetReference, PooledSet>();
        
        #endregion
    }
}