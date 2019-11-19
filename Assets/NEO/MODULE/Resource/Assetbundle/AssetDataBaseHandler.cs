#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEditor;
using UnityEngine;
using NEO.SUPPORT;

namespace NEO.MODULE
{
    public class AssetDataBaseHandler : IAssetHandler
    {
        Dictionary<AssetBundleData.ID, AssetBundleData.Item> IDtoITEM;

        Dictionary<AssetBundleData.ID, IPool> pools;
        Dictionary<Object, AssetBundleData.ID> OBJECTtoID;

        bool initialized = false;

        UnityAction<float, bool> ProgressListener;

        public void Initialize(string listPath)
        {
            if (initialized)
                return;

            ProgressListener?.Invoke(0f, false);
            pools = new Dictionary<AssetBundleData.ID, IPool>();
            OBJECTtoID = new Dictionary<Object, AssetBundleData.ID>();

            IDtoITEM = new Dictionary<AssetBundleData.ID, AssetBundleData.Item>();
            AssetBundleData bundleList = Resources.Load<AssetBundleData>(listPath);
            for (int i = 0; i < bundleList.items.Length; i++)
            {
                IDtoITEM.Add(bundleList.items[i].id, bundleList.items[i]);
            }

            initialized = true;
            ProgressListener?.Invoke(1f, true);
        }

        public bool Initialized()
        {
            return initialized;
        }

        public void SetProgressListener(UnityAction<float, bool> progressListener)
        {
            ProgressListener = progressListener;
        }

        public AssetBundleData.Item GetItem(AssetBundleData.ID id)
        {
            if(IDtoITEM.TryGetValue(id, out AssetBundleData.Item item))
            {
                return item;
            }
            return null;
        }

        public T GetAsset<T>(AssetBundleData.ID id) where T : Object
        {
            if(pools.TryGetValue(id,out IPool pool) == false)
            {
                AssetBundleData.Item item = IDtoITEM[id];
                T originAsset = AssetDatabase.LoadAssetAtPath<T>(item.path);
                pool = CreatePool(originAsset, item.isPrefab);
                pools[id] = pool;
            }
            Object asset = pool.Instantiate();
            OBJECTtoID[asset] = id;
            return asset as T;
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

        public T GetAsset<T>(AssetBundleData.Item item, ref AssetBundle bundle) where T : Object
        {
            if(item.id != AssetBundleData.ID.None)
            {
                return GetAsset<T>(item.id);
            }

            T asset = AssetDatabase.LoadAssetAtPath<T>(item.path);
            return asset;
        }

        public void GetAssetAsync<T>(AssetBundleData.ID id, UnityAction<T> OnComplete) where T : Object
        {
            OnComplete(GetAsset<T>(id));
        }

        public void GetAssetAsync<T>(AssetBundleData.Item item, AssetBundle bundle, UnityAction<T, AssetBundle> OnComplete) where T : Object
        {
            OnComplete(GetAsset<T>(item,ref bundle),null);
        }

        public void Release(Object asset)
        {
            AssetBundleData.ID id = OBJECTtoID[asset];
            if(pools.TryGetValue(id, out IPool pool))
            {
                pool.Release(asset);
            }
        }

        public void Clear(AssetBundleData.ID id)
        {
            if (pools.TryGetValue(id, out IPool pool))
            {
                pool.Clear();
            }
        }

        public void Clear()
        {
            OBJECTtoID.Clear();
            Dictionary<AssetBundleData.ID, IPool>.Enumerator enutor = pools.GetEnumerator();
            while (enutor.MoveNext())
            {
                enutor.Current.Value.Clear();
            }
        }

        public void Destroy(AssetBundleData.ID id)
        {
            if (pools.TryGetValue(id, out IPool pool))
            {
                pool.Destroy();
                pools.Remove(id);
            }
        }

        public void Destroy()
        {
            initialized = false;
            IDtoITEM.Clear();
            IDtoITEM = null;

            Dictionary<AssetBundleData.ID, IPool>.Enumerator enutor = pools.GetEnumerator();
            while (enutor.MoveNext())
            {
                enutor.Current.Value.Destroy();
            }
            pools.Clear();
            pools = null;

            OBJECTtoID.Clear();
            OBJECTtoID = null;
        }

        public void ClearOBJECTtoIDDirtyElements()
        {
            Dictionary<Object, AssetBundleData.ID> newOBJECTtoID = new Dictionary<Object, AssetBundleData.ID>();
            Dictionary<Object, AssetBundleData.ID>.Enumerator enutor = OBJECTtoID.GetEnumerator();
            while (enutor.MoveNext())
            {
                if (enutor.Current.Key != null)
                {
                    newOBJECTtoID[enutor.Current.Key] = enutor.Current.Value;
                }
            }
            OBJECTtoID.Clear();
            OBJECTtoID = newOBJECTtoID;
        }
    }
}

#endif