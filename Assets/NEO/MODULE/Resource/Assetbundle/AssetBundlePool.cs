using System.Collections.Generic;
using UnityEngine;
using NEO.SUPPORT;

namespace NEO.MODULE
{
    public class AssetBundlePool
    {
        AssetBundle bundle;
        Dictionary<AssetBundleData.ID, IPool> IDtoPool;

        public AssetBundlePool(AssetBundle bundle)
        {
            IDtoPool = new Dictionary<AssetBundleData.ID, IPool>();
            this.bundle = bundle;
        }

        public AssetBundle GetBundle()
        {
            return bundle;
        }

        public void AddAsset(AssetBundleData.ID id, Object asset,bool isPrefab)
        {
            IDtoPool.Add(id, CreatePool(asset,isPrefab));
        }

        IPool CreatePool(Object asset, bool isPrefab)
        {
            IPool pool = null;
            if (isPrefab)
            {
                pool = new PrefabPool();
            }
            else
            {
                pool = new AssetPool();
            }
            pool.Initialize(asset);
            return pool;
        }

        public bool TryGet(AssetBundleData.ID id, out Object asset)
        {
            if(IDtoPool.TryGetValue(id, out IPool pool))
            {
                asset = pool.Instantiate();
                return true;
            }

            asset = null;
            return false;
        }

        public bool IsUsing()
        {
            Dictionary<AssetBundleData.ID, IPool>.Enumerator enutor = IDtoPool.GetEnumerator();
            while (enutor.MoveNext())
            {
                if (enutor.Current.Value.IsUsing())
                    return true;
            }
            return false;
        }

        public bool Release(AssetBundleData.ID id, Object asset)
        {
            if(IDtoPool.TryGetValue(id,out IPool pool))
            {
                pool.Release(asset);
                return true;
            }
            return false;
        }

        public void Clear(AssetBundleData.ID id)
        {
            if (IDtoPool.TryGetValue(id, out IPool pool))
            {
                pool.Clear();
            }
        }

        public void Clear()
        {
            Dictionary<AssetBundleData.ID, IPool>.Enumerator IDenutor = IDtoPool.GetEnumerator();
            while (IDenutor.MoveNext())
            {
                IDenutor.Current.Value.Clear();
            }
        }

        public void Destroy()
        {
            Dictionary<AssetBundleData.ID, IPool>.Enumerator IDenutor = IDtoPool.GetEnumerator();
            while (IDenutor.MoveNext())
            {
                IDenutor.Current.Value.Destroy();
            }
            IDtoPool.Clear();
            bundle.Unload(true);
            bundle = null;
            IDtoPool = null;
        }
    }
}

