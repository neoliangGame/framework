using UnityEngine;
using UnityEngine.Events;

namespace NEO.MODULE
{
    public class AssetBundleManager
    {
        Mode mode = Mode.None;
        IAssetHandler handler;

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="listPath">资源配置表文件路径</param>
        /// <param name="bundleRoot">AssetBundle存放根目录</param>
        /// <param name="mode">AssetBundle模式还是AssetDatabase模式</param>
        /// <param name="handlerRoot">脚本挂载点</param>
        /// <param name="ProgressListener">异步加载进度</param>
        public AssetBundleManager(string listPath,string bundleRoot, Mode mode, Transform handlerRoot = null, UnityAction<float, bool> ProgressListener = null)
        {
            if (this.mode != Mode.None || mode == Mode.None)
                return;
            this.mode = mode;

#if UNITY_EDITOR
            if(mode == Mode.AssetDataBase)
            {
                AssetDataBaseHandler assetDataBaseHandler = new AssetDataBaseHandler();
                assetDataBaseHandler.SetProgressListener(ProgressListener);
                assetDataBaseHandler.Initialize(listPath);
                handler = assetDataBaseHandler;
            }
            else
#endif
            {
                GameObject handlerGO = new GameObject("AssetBundleHandler");
                if (handlerRoot != null)
                    handlerGO.transform.parent = handlerRoot;

                AssetBundleHandler assetBundleHandler = handlerGO.AddComponent<AssetBundleHandler>();
                assetBundleHandler.SetProgressListener(ProgressListener);
                assetBundleHandler.Initialize(listPath, bundleRoot);
                handler = assetBundleHandler;
            }
        }

        /// <summary>
        /// 查询是否初始化
        /// </summary>
        /// <returns></returns>
        public bool Initialized()
        {
            return handler.Initialized();
        }

        public void SetProgressHandler(UnityAction<float, bool> ProgressHandler)
        {
            handler.SetProgressListener(ProgressHandler);
        }
#endregion

#region 获取资源
        /// <summary>
        /// 通过ID获取对应资源配置信息
        /// </summary>
        /// <param name="id">资源ID</param>
        /// <returns>配置信息</returns>
        public AssetBundleData.Item GetItem(AssetBundleData.ID id)
        {
            return handler.GetItem(id);
        }

        /// <summary>
        /// 通过ID获取资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="id">资源ID</param>
        /// <returns>资源</returns>
        public T GetAsset<T>(AssetBundleData.ID id) where T : Object
        {
            return handler.GetAsset<T>(id);
        }

        /// <summary>
        /// 通过资源配置信息获取资源
        /// 1。如果item.id = None，则表示是自定义信息：
        ///     不缓存
        ///     AssetBundle文件地址：path+bundleName
        ///     asset名称：assetName
        /// 2。如果item.id != None，则是系统内的资源，一律缓存，走正常流程
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="item">资源配置信息</param>
        /// <param name="bundle">if null:加载bundle并且赋值；if != null，用此bundle</param>
        /// <returns>资源</returns>
        public T GetAsset<T>(AssetBundleData.Item item, ref AssetBundle bundle) where T : Object
        {
            return handler.GetAsset<T>(item,ref bundle);
        }

        /// <summary>
        /// 异步通过ID获取资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="id">资源ID</param>
        /// <param name="OnComplete">资源加载完成后的回调</param>
        public void GetAssetAsync<T>(AssetBundleData.ID id, UnityAction<T> OnComplete) where T : Object
        {
            handler.GetAssetAsync<T>(id, (asset)=> { OnComplete(asset); });
        }

        /// <summary>
        /// 异步通过资源配置信息获取资源
        /// 1。如果item.id = None，则表示是自定义信息：
        ///     不缓存
        ///     AssetBundle文件地址：path+bundleName
        ///     asset名称：assetName
        /// 2。如果item.id != None，则是系统内的资源，一律缓存，走正常流程
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="item">资源配置信息</param>
        /// <param name="bundle">if null, 加载bundle并赋值；if != null,用此bundle</param>
        /// <param name="OnComplete">加载完成后的回调</param>
        public void GetAssetAsync<T>(AssetBundleData.Item item, AssetBundle bundle, UnityAction<T,AssetBundle> OnComplete) where T : Object
        {
            handler.GetAssetAsync<T>(item, bundle, (asset,newBundle) => { OnComplete(asset,newBundle); });
        }
#endregion

#region 释放资源
        /// <summary>
        /// 释放单个实例
        /// </summary>
        /// <param name="asset">实例资源</param>
        public void Release(Object asset)
        {
            handler.Release(asset);
        }

        /// <summary>
        /// 清除此ID的所有实例
        /// </summary>
        /// <param name="id">资源ID</param>
        public void Clear(AssetBundleData.ID id)
        {
            handler.Clear(id);
            handler.ClearOBJECTtoIDDirtyElements();
        }

        /// <summary>
        /// 清除所有实例
        /// </summary>
        public void Clear()
        {
            handler.Clear();
        }

        /// <summary>
        /// 销毁此ID所有实例和原始资源
        /// </summary>
        /// <param name="id">资源ID</param>
        public void Destroy(AssetBundleData.ID id)
        {
            handler.Destroy(id);
            handler.ClearOBJECTtoIDDirtyElements();
        }

        /// <summary>
        /// 销毁整个内存池，包括所有实例和原始资源
        /// </summary>
        public void Destroy()
        {
            handler.Destroy();
            mode = Mode.None;
        }

        /// <summary>
        /// 清除未引用的资源
        /// </summary>
        /// 简单封装，可用原来的
        /// 用的时候得慎重，会造成卡顿
        public static void ClearUnused()
        {
            Resources.UnloadUnusedAssets();
        }
#endregion

        public enum Mode
        {
            None,
            AssetBundle,
            AssetDataBase,
        }
    }
}

