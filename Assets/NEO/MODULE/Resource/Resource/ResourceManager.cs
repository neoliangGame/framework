using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NEO.SUPPORT;

namespace NEO.MODULE
{
    /// <summary>
    /// 资源管理
    /// </summary>
    /// 0。资源配置表
    ///     首先需要生成ResourceData的asset文件，放在Resources文件夹下
    ///     并且对项目中需要加载的资源都添加到这个配置表中
    /// 1。初始化
    ///     外部传入ResourceData的asset文件的相对路径，直接通过Resources加载配置文件
    ///     通过配置文件信息，加载预加载资源
    /// 2。获取资源
    ///     Prefab：如果未加载，则首先加载资源，再创建对应资源池，从资源池中实例化一个对象返回
    ///     Asset：如果未加载，首先进行加载，再创建对应资源池，返回原始资源引用，引用计数+1
    /// 3。释放资源：
    ///     Prefab：回收实例
    ///     Asset：引用计数-1
    /// 4。清除资源：
    ///     Prefab：回收所有实例
    ///     Asset：引用计数清零
    /// 5。销毁资源：
    ///     Prefab：销毁所有实例，销毁此资源池
    ///     Asset：卸载原始资源，销毁此资源池
    public class ResourceManager : MonoBehaviour
    {
        Dictionary<ResourceData.ID, ResourceData.Item> IDtoITEM = null;
        Dictionary<ResourceData.ID, IPool> pools;
        Dictionary<Object, ResourceData.ID> OBJECTtoID;

        bool initialized = false;
        bool initializing = false;

        UnityAction<float,bool> ProgressListener = null;
        public void SetProgressListener(UnityAction<float,bool> progressListener)
        {
            ProgressListener = progressListener;
        }

        #region 初始化
        public ResourceManager()
        {
            initialized = false;
            IDtoITEM = new Dictionary<ResourceData.ID, ResourceData.Item>();
            pools = new Dictionary<ResourceData.ID, IPool>();
            OBJECTtoID = new Dictionary<Object, ResourceData.ID>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="listPath">资源配置表文件位置</param>
        public void Initialize(string listPath)
        {
            if (initialized || initializing)
                return;
            initializing = true;

            Load(listPath, out ResourceData list);
            List<ResourceData.Item> preLoadList = new List<ResourceData.Item>();
            for (int i = 0; i < list.items.Length; ++i)
            {
                IDtoITEM.Add(list.items[i].id, list.items[i]);
                if (list.items[i].preLoad)
                {
                    preLoadList.Add(list.items[i]);
                }
            }

            StartCoroutine(LoadAsync(preLoadList, 
                (assets) => {
                    for(int i = 0;i < assets.Length; ++i)
                    {
                        pools[preLoadList[i].id] = CreatePool(preLoadList[i], assets[i]);
                    }

                    initialized = true;
                    initializing = false;
                }));
        }

        IPool CreatePool(ResourceData.Item item, Object originAsset)
        {
            IPool pool = null;
            if (item.isPrefab)
            {
                pool = new PrefabPool();
            }
            else
            {
                pool = new AssetPool();
            }
            pool.Initialize(originAsset);
            return pool;
        }

        IEnumerator LoadAsync(List<ResourceData.Item> items, UnityAction<Object[]> OnComplete)
        {
            ResourceRequest[] requests = new ResourceRequest[items.Count];
            for (int i = 0; i < requests.Length; ++i)
            {
                requests[i] = Resources.LoadAsync(items[i].path);
            }
            bool allIsDone = false;
            int loadedCount = 0;
            float rate;
            while (allIsDone == false)
            {
                allIsDone = true;
                loadedCount = 0;
                for (int i = 0; i < requests.Length; ++i)
                {
                    if (requests[i].isDone == false)
                    {
                        allIsDone = false;
                    }
                    else
                    {
                        loadedCount++;
                    }
                }
                rate = (float)loadedCount / requests.Length;
                ProgressListener?.Invoke(rate, false);
                yield return null;
            }
            Object[] assets = new Object[requests.Length];
            for (int i = 0; i < assets.Length; ++i)
            {
                assets[i] = requests[i].asset;
            }
            OnComplete.Invoke(assets);
            ProgressListener?.Invoke(1f, true);
            yield return null;
        }

        /// <summary>
        /// 是否已经初始化了，未初始化是不可用的
        /// </summary>
        /// <returns></returns>
        public bool Initialized()
        {
            return initialized;
        }
        #endregion

        #region 加载
        /// <summary>
        /// 获取对应ID配置信息
        /// </summary>
        /// <param name="id">资源ID</param>
        /// <returns>配置信息</returns>
        public ResourceData.Item GetItem(ResourceData.ID id)
        {
            if(IDtoITEM.TryGetValue(id, out ResourceData.Item item))
            {
                return item;
            }
            return null;
        }

        /// <summary>
        /// 获取资源
        /// 1。如果是Asset，则获取其引用
        /// 2。如果是Prefab，则获取其实例
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="id">资源ID</param>
        /// <param name="asset">资源</param>
        public void Get<T>(ResourceData.ID id,out T asset) where T : Object
        {
            if (pools.TryGetValue(id,out IPool pool) == false)
            {
                ResourceData.Item item = IDtoITEM[id];
                Load(item.path, out T originAsset);
                pool = CreatePool(item, originAsset);
                pools[id] = pool;
            }
            asset = pool.Instantiate() as T;
            OBJECTtoID[asset] = id;
        }

        /// <summary>
        /// 直接通过路径加载资源，以防有一些大量细碎资源不适合用ID
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源相对路径</param>
        /// <param name="asset">返回资源</param>
        public static void Load<T>(string path,out T asset) where T : Object
        {
            asset = Resources.Load<T>(path);
        }

        /// <summary>
        /// 异步获取资源
        /// 1。如果是纯资源，则获取其引用
        /// 2。如果是预制体，则获取其实例
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="id">资源ID</param>-
        /// <param name="OnComplete">资源加载完成后的回调</param>
        public void GetAsync<T>(ResourceData.ID id, UnityAction<T> OnComplete) where T : Object
        {
            if (pools.TryGetValue(id, out IPool pool) == false)
            {
                ResourceData.Item item = IDtoITEM[id];

                StartCoroutine(LoadAsync<T>(item.path, (originAsset) => {
                    pool = CreatePool(item, originAsset);
                    pools[id] = pool;

                    T asset = pool.Instantiate() as T;
                    OBJECTtoID[asset] = id;

                    OnComplete(asset);
                }));
            }
            else
            {
                T asset = pool.Instantiate() as T;
                OBJECTtoID[asset] = id;

                OnComplete(asset);
            }
        }

        IEnumerator LoadAsync<T>(string path, UnityAction<T> OnComplete) where T : Object
        {
            ResourceRequest request = Resources.LoadAsync<T>(path);
            while(request.isDone == false)
            {
                yield return null;
            }
            OnComplete.Invoke(request.asset as T);
            yield return null;
        }
        #endregion

        #region 卸载
        /// <summary>
        /// 卸载单个实例资源
        /// 1。如果是纯资源，减少引用计数
        /// 2。如果是预制体实例，则回收实例
        /// </summary>
        /// <param name="asset">要卸载的资源</param>
        public void Release(Object asset)
        {
            ResourceData.ID id = OBJECTtoID[asset];
            if(pools.TryGetValue(id, out IPool pool))
            {
                pool.Release(asset);
            }
        }

        public void Clear(ResourceData.ID id)
        {
            if (pools.TryGetValue(id, out IPool pool))
            {
                pool.Clear();
                ClearOBJECTtoIDDirtyElements();
            }
        }

        public void Clear()
        {
            Dictionary<ResourceData.ID, IPool>.Enumerator enutor = pools.GetEnumerator();
            while (enutor.MoveNext())
            {
                enutor.Current.Value.Clear();
            }
            pools.Clear();

            OBJECTtoID.Clear();
        }

        /// <summary>
        /// 卸载一种资源的所有，包括原始资源和所有实例
        /// </summary>
        /// <param name="id">资源ID</param>
        public void Destroy(ResourceData.ID id)
        {
            if (pools.TryGetValue(id,out IPool pool))
            {
                if (IDtoITEM[id].isPrefab == false)
                {
                    Resources.UnloadAsset(pool.Asset());
                }
                pool.Destroy();
                pools.Remove(id);
                ClearOBJECTtoIDDirtyElements();
            }
        }

        /// <summary>
        /// 销毁此内存池中的所有资源，包括所有原始资源和实例资源
        /// </summary>
        public void Destroy()
        {
            initialized = false;

            IDtoITEM.Clear();
            IDtoITEM = null;

            Dictionary<ResourceData.ID, IPool>.Enumerator enutor = pools.GetEnumerator();
            while (enutor.MoveNext())
            {
                if (IDtoITEM[enutor.Current.Key].isPrefab == false)
                {
                    Resources.UnloadAsset(enutor.Current.Value.Asset());
                }
                enutor.Current.Value.Destroy();
            }
            pools.Clear();
            pools = null;

            OBJECTtoID.Clear();
            OBJECTtoID = null;
        }

        /// <summary>
        /// 清理内存中所有已经无引用的资源
        /// 一般是在清理不干净的情况下调用
        /// </summary>
        public static void ClearUnused()
        {
            Resources.UnloadUnusedAssets();
        }

        void ClearOBJECTtoIDDirtyElements()
        {
            Dictionary<Object, ResourceData.ID> newOBJECTtoID = new Dictionary<Object, ResourceData.ID>();
            Dictionary<Object, ResourceData.ID>.Enumerator enutor = OBJECTtoID.GetEnumerator();
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
        #endregion
    }
}

