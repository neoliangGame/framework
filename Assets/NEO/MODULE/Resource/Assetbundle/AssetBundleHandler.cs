using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace NEO.MODULE
{
    /// <summary>
    /// Assetbundle资源管理解决方案
    /// </summary>
    public partial class AssetBundleHandler : MonoBehaviour, IAssetHandler
    {
        //资源对应表，每个资源手动配置
        Dictionary<AssetBundleData.ID, AssetBundleData.Item> IDtoITEM = null;
        //AssetBundle依赖列表
        AssetBundleManifest manifest;

        //assetbundle存放根目录，所有bundle都放在这个文件夹下
        //TODO:如果有热更新，需要区分一下是本地(StreamingAssets)还是下载的(沙盒目录)
        string bundleRootPath;
        //所有已加载的AssetBundle与名称的对应表，包含依赖包
        //用于判断是否已加载，避免重复加载失败
        Dictionary<string, AssetBundle> NAMEtoBUNDLE;

        //所有已加载的ID对应的bundle池，通过ID索引到bundle，再索引到具体资源
        Dictionary<AssetBundleData.ID, AssetBundlePool> pools;

        //所有已使用的资源对应资源ID映射表，用于资源回收的时候快速定位
        Dictionary<Object, AssetBundleData.ID> OBJECTtoID;

        bool initialized = false;//是否已经初始化
        bool initializing = false;//是否正在初始化

        UnityAction<float, bool> ProgressListener;//多个资源加载进度（暂时只有预加载）
        
#region 初始化
        public void Initialize(string listPath,string bundleRoot)
        {
            if (initialized || initializing)
                return;
            initializing = true;

            IDtoITEM = new Dictionary<AssetBundleData.ID, AssetBundleData.Item>();

            bundleRootPath = bundleRoot;
            NAMEtoBUNDLE = new Dictionary<string, AssetBundle>();

            pools = new Dictionary<AssetBundleData.ID, AssetBundlePool>();

            OBJECTtoID = new Dictionary<Object, AssetBundleData.ID>();

            StartCoroutine(Initialization(listPath));
        }
        IEnumerator Initialization(string listPath)
        {
            ProgressListener?.Invoke(0f, false);
            //加载资源配置表
            ResourceRequest listRequest = Resources.LoadAsync<AssetBundleData>(listPath);
            while (listRequest.isDone == false)
            {
                yield return null;
            }
            AssetBundleData bundleList = listRequest.asset as AssetBundleData;
            for (int i = 0; i < bundleList.items.Length; ++i)
            {
                IDtoITEM.Add(bundleList.items[i].id, bundleList.items[i]);
            }
            //加载依赖描述文件：AssetBundleManifest
            AssetBundleData.Item manifestItem = IDtoITEM[AssetBundleData.ID.Manifest];
            AssetBundleCreateRequest manifestBundleRequest = AssetBundle.LoadFromFileAsync(Path.Combine(bundleRootPath, manifestItem.bundleName));
            while (manifestBundleRequest.isDone == false)
            {
                yield return null;
            }
            AssetBundleRequest manifestRequest = manifestBundleRequest.assetBundle.LoadAssetAsync(manifestItem.assetName);
            while (manifestRequest.isDone == false)
            {
                yield return null;
            }
            manifest = manifestRequest.asset as AssetBundleManifest;
            //预加载Bundle，包括其所有依赖的Bundle
            Dictionary<string, bool> bundleAdded = new Dictionary<string, bool>();
            AssetBundleCreateRequest preBundleRequest;
            List<AssetBundleData.Item> preLoadItems = new List<AssetBundleData.Item>();
            List<AssetBundleCreateRequest> preBundleRequests = new List<AssetBundleCreateRequest>();
            for (int i = 0; i < bundleList.items.Length; ++i)
            {
                if (bundleList.items[i].preLoad)
                {
                    preLoadItems.Add(bundleList.items[i]);
                    if(bundleAdded.ContainsKey(bundleList.items[i].bundleName) == false)
                    {
                        preBundleRequest = AssetBundle.LoadFromFileAsync(Path.Combine(bundleRootPath, bundleList.items[i].bundleName));
                        preBundleRequests.Add(preBundleRequest);
                        bundleAdded[bundleList.items[i].bundleName] = true;
                    }
                }
            }
            if (preBundleRequests.Count > 0)
            {
                string[] dependencies;
                for (int i = 0; i < preLoadItems.Count; ++i)
                {
                    dependencies = manifest.GetAllDependencies(preLoadItems[i].bundleName);
                    for (int d = 0; d < dependencies.Length; ++d)
                    {
                        if (bundleAdded.ContainsKey(dependencies[d]) == false)
                        {
                            bundleAdded[dependencies[d]] = true;
                            preBundleRequest = AssetBundle.LoadFromFileAsync(Path.Combine(bundleRootPath, dependencies[d]));
                            preBundleRequests.Add(preBundleRequest);
                        }
                    }
                }
                bool allLoadedDone = false;
                float progress = 0f;
                int loadedCount = 0;
                while (allLoadedDone == false)
                {
                    loadedCount = 0;
                    allLoadedDone = true;
                    for (int i = 0; i < preBundleRequests.Count; ++i)
                    {
                        if (preBundleRequests[i].isDone)
                        {
                            ++loadedCount;
                        }
                        else
                        {
                            allLoadedDone = false;
                        }
                    }
                    progress = (float)loadedCount / preBundleRequests.Count;
                    ProgressListener?.Invoke(progress * 0.5f, false);
                    yield return null;
                }
                for (int i = 0; i < preBundleRequests.Count; ++i)
                {
                    NAMEtoBUNDLE[preBundleRequests[i].assetBundle.name] = preBundleRequests[i].assetBundle;
                }
                ProgressListener?.Invoke(0.5f, false);
                //预加载Asset
                AssetBundleRequest[] preAssetRequests = new AssetBundleRequest[preLoadItems.Count];
                AssetBundle preLoadBundle;
                for (int i = 0; i < preLoadItems.Count; ++i)
                {
                    preLoadBundle = NAMEtoBUNDLE[preLoadItems[i].bundleName];
                    preAssetRequests[i] = preLoadBundle.LoadAssetAsync(preLoadItems[i].assetName);
                }
                allLoadedDone = false;
                while (allLoadedDone == false)
                {
                    allLoadedDone = true;
                    loadedCount = 0;
                    for (int i = 0; i < preAssetRequests.Length; ++i)
                    {
                        if (preAssetRequests[i].isDone)
                        {
                            ++loadedCount;
                        }
                        else
                        {
                            allLoadedDone = false;
                        }
                    }
                    progress = (float)loadedCount / preAssetRequests.Length;
                    ProgressListener?.Invoke(progress * 0.5f + 0.5f, false);
                    yield return null;
                }
                for (int i = 0; i < preAssetRequests.Length; ++i)
                {
                    if(pools.TryGetValue(preLoadItems[i].id,out AssetBundlePool pool) == false)
                    {
                        preLoadBundle = NAMEtoBUNDLE[preLoadItems[i].bundleName];
                        pool = new AssetBundlePool(preLoadBundle);
                        pools.Add(preLoadItems[i].id, pool);
                    }
                    pool.AddAsset(preLoadItems[i].id, preAssetRequests[i].asset, preLoadItems[i].isPrefab);
                }
            }

            initializing = false;
            initialized = true;
            ProgressListener?.Invoke(1f, true);
            yield return null;
        }

        public bool Initialized()
        {
            return initialized;
        }

        public void SetProgressListener(UnityAction<float,bool> progressListener)
        {
            ProgressListener = progressListener;
        }
#endregion

#region 加载资源
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
            AssetBundleData.Item item = IDtoITEM[id];
            Object asset = null;
            if (pools.TryGetValue(id, out AssetBundlePool pool))
            {
                if (pool.TryGet(id, out asset) == false)
                {
                    Object originAsset = pool.GetBundle().LoadAsset(item.assetName);
                    pool.AddAsset(id, originAsset, item.isPrefab);
                    pool.TryGet(id, out asset);
                }
            }
            else
            {
                AssetBundle bundle = LoadBundle(item);
                pool = new AssetBundlePool(bundle);
                pools[id] = pool;

                Object originAsset = bundle.LoadAsset(item.assetName);
                pool.AddAsset(id, originAsset, item.isPrefab);
                pool.TryGet(id, out asset);
            }

            OBJECTtoID[asset] = id;
            return asset as T;
        }

        public T GetAsset<T>(AssetBundleData.Item item,ref AssetBundle bundle) where T : Object
        {
            if(item.id != AssetBundleData.ID.None)
            {
                return GetAsset<T>(item.id);
            }

            if(bundle == null)
            {
                string bundlePath = Path.Combine(item.path, item.bundleName);
                bundle = AssetBundle.LoadFromFile(bundlePath);
            }

            Object asset = bundle.LoadAsset(item.assetName);
            return asset as T;
        }

        public void GetAssetAsync<T>(AssetBundleData.ID id, UnityAction<T> OnComplete) where T : Object
        {
            AssetBundleData.Item item = IDtoITEM[id];
            if (pools.TryGetValue(id, out AssetBundlePool pool))
            {
                if (pool.TryGet(id, out Object asset))
                {
                    OBJECTtoID[asset] = id;
                    OnComplete(asset as T);
                }
                else
                {
                    GetAssetFromBundleAsync(item, pool, OnComplete);
                }
            }
            else
            {
                StartCoroutine(LoadBundleAsync(item, (newBundle) =>
                {
                    pool = new AssetBundlePool(newBundle);
                    pools.Add(id, pool);
                    GetAssetFromBundleAsync(item, pool, OnComplete);
                }));
            }
        }

        void GetAssetFromBundleAsync<T>(AssetBundleData.Item item, AssetBundlePool pool, UnityAction<T> OnComplete) where T : Object
        {
            StartCoroutine(LoadAssetAsync(item, pool.GetBundle(), (originAsset) =>
            {
                pool.AddAsset(item.id, originAsset, item.isPrefab);
                pool.TryGet(item.id, out Object asset);
                OBJECTtoID[asset] = item.id;
                OnComplete(asset as T);
            }));
        }

        public void GetAssetAsync<T>(AssetBundleData.Item item, AssetBundle bundle, UnityAction<T, AssetBundle> OnComplete) where T : Object
        {
            if(item.id != AssetBundleData.ID.None)
            {
                GetAssetAsync<T>(item.id, (asset) => { OnComplete.Invoke(asset, null); });
            }

            if(bundle == null)
            {
                string bundlePath = Path.Combine(item.path,item.bundleName);
                StartCoroutine(LoadBundleAsync(bundlePath, (newBundle) => {
                    StartCoroutine(LoadAssetAsync(item, newBundle, (asset) => {
                        OnComplete(asset as T, newBundle);
                    }));
                }));
            }
            else
            {
                StartCoroutine(LoadAssetAsync(item, bundle, (asset) => {
                    OnComplete(asset as T, bundle);
                }));
            }
        }

        AssetBundle LoadBundle(AssetBundleData.Item item)
        {
            if (NAMEtoBUNDLE.TryGetValue(item.bundleName, out AssetBundle bundle) == false)
            {
                bundle = AssetBundle.LoadFromFile(Path.Combine(bundleRootPath, item.bundleName));
                NAMEtoBUNDLE.Add(item.bundleName, bundle);
            }

            string[] dependencies = manifest.GetAllDependencies(item.bundleName);
            AssetBundle dependencyBundle;
            for (int i = 0; i < dependencies.Length; ++i)
            {
                if (NAMEtoBUNDLE.ContainsKey(dependencies[i]) == false)
                {
                    dependencyBundle = AssetBundle.LoadFromFile(Path.Combine(bundleRootPath, dependencies[i]));
                    NAMEtoBUNDLE.Add(dependencies[i], dependencyBundle);
                }
            }

            return bundle;
        }

        IEnumerator LoadBundleAsync(AssetBundleData.Item item, UnityAction<AssetBundle> OnComplete)
        {
            if (NAMEtoBUNDLE.TryGetValue(item.bundleName, out AssetBundle bundle) == false)
            {
                AssetBundleCreateRequest ABCRequest = AssetBundle.LoadFromFileAsync(Path.Combine(bundleRootPath, item.bundleName));
                while (ABCRequest.isDone == false)
                {
                    yield return null;
                }
                bundle = ABCRequest.assetBundle;

                NAMEtoBUNDLE.Add(item.bundleName,bundle);
            }

            string[] dependencies = manifest.GetAllDependencies(item.bundleName);
            List<AssetBundleCreateRequest> ABCRequestList = new List<AssetBundleCreateRequest>();
            for (int i = 0; i < dependencies.Length; ++i)
            {
                if (NAMEtoBUNDLE.ContainsKey(dependencies[i]) == false)
                {
                    ABCRequestList.Add(AssetBundle.LoadFromFileAsync(Path.Combine(bundleRootPath, dependencies[i])));
                }
            }
            bool allIsDone = false;
            while (allIsDone == false)
            {
                allIsDone = true;
                for (int i = 0; i < ABCRequestList.Count; ++i)
                {
                    if (ABCRequestList[i].isDone == false)
                    {
                        allIsDone = false;
                        break;
                    }
                }
                yield return null;
            }
            for (int i = 0; i < ABCRequestList.Count; ++i)
            {
                NAMEtoBUNDLE.Add(ABCRequestList[i].assetBundle.name, ABCRequestList[i].assetBundle);
            }

            OnComplete(bundle);
            yield return null;
        }

        IEnumerator LoadBundleAsync(string bundlePath, UnityAction<AssetBundle> OnComplete)
        {
            AssetBundleCreateRequest ABCRequest = AssetBundle.LoadFromFileAsync(bundlePath);
            while(ABCRequest.isDone == false)
            {
                yield return null;
            }
            OnComplete(ABCRequest.assetBundle);
        }

        IEnumerator LoadAssetAsync(AssetBundleData.Item item, AssetBundle bundle, UnityAction<Object> OnComplete)
        {
            AssetBundleRequest request = bundle.LoadAssetAsync(item.assetName);
            while (request.isDone == false)
            {
                yield return null;
            }
            OnComplete(request.asset);
            yield return null;
        }
        #endregion

#region 卸载
        public void Release(Object asset)
        {
            AssetBundleData.ID id = OBJECTtoID[asset];
            if(pools.TryGetValue(id, out AssetBundlePool pool))
            {
                pool.Release(id, asset);
            }
        }

        public void Clear(AssetBundleData.ID id)
        {
            if (pools.TryGetValue(id, out AssetBundlePool pool))
            {
                pool.Clear(id);
            }
        }

        public void Clear()
        {
            IDtoITEM.Clear();

            Dictionary<AssetBundleData.ID, AssetBundlePool>.Enumerator enutor = pools.GetEnumerator();
            while (enutor.MoveNext())
            {
                enutor.Current.Value.Clear();
            }

            OBJECTtoID.Clear();
        }

        public void Destroy(AssetBundleData.ID id)
        {
            if (pools.TryGetValue(id, out AssetBundlePool pool))
            {
                NAMEtoBUNDLE.Remove(pool.GetBundle().name);
                pool.Destroy();
                pools.Remove(id);
            }
        }

        public void Destroy()
        {
            IDtoITEM.Clear();
            IDtoITEM = null;
            manifest = null;

            bundleRootPath = null;
            Dictionary<string, AssetBundle>.Enumerator bundleEnutor = NAMEtoBUNDLE.GetEnumerator();
            while (bundleEnutor.MoveNext())
            {
                bundleEnutor.Current.Value.Unload(true);
            }
            NAMEtoBUNDLE.Clear();
            NAMEtoBUNDLE = null;

            Dictionary<AssetBundleData.ID, AssetBundlePool>.Enumerator enutor = pools.GetEnumerator();
            while (enutor.MoveNext())
            {
                enutor.Current.Value.Destroy();
            }
            pools.Clear();
            pools = null;

            OBJECTtoID.Clear();
            OBJECTtoID = null;

            initialized = false;
        }

        public void ClearOBJECTtoIDDirtyElements()
        {
            StartCoroutine(DelayClearOBJECTtoIDDirtyElements(5));
        }
        IEnumerator DelayClearOBJECTtoIDDirtyElements(int frame)
        {
            while(frame > 0)
            {
                --frame;
                yield return null;
            }
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
            yield return null;
        }
#endregion
    }
}

