using System.Collections.Generic;

namespace Universe
{
    public struct PooledSet
    {
        public int PoolSize => m_assets.Count;
        public List<PooledAsset> m_assets;
    }
}