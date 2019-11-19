using UnityEngine;
using UnityEngine.Events;

namespace NEO.MODULE
{
    public interface IAssetHandler
    {
        /// <summary>
        /// 查询是否初始化，如果未初始化则不可用
        /// </summary>
        /// <returns>是否初始化</returns>
        bool Initialized();

        void SetProgressListener(UnityAction<float, bool> progressListener);

        /// <summary>
        /// 通过ID获取资源配置信息
        /// </summary>
        /// <param name="id">资源ID</param>
        /// <returns>配置信息</returns>
        AssetBundleData.Item GetItem(AssetBundleData.ID id);

        /// <summary>
        /// 通过ID获取资源
        /// </summary>
        /// <param name="id">资源ID</param>
        /// <returns>资源</returns>
        T GetAsset<T>(AssetBundleData.ID id) where T : Object;

        /// <summary>
        /// 通过配置信息获取资源
        /// 1。如果配置信息id = None，则表明是自定义信息，则资源和bundle不缓存
        /// 2。如果配置信息id != None，则表明是已定义的信息，走系统流程
        /// </summary>
        /// <param name="item">资源配置信息</param>
        /// <param name="bundle">AssetBundle</param>
        /// <returns>资源</returns>
        T GetAsset<T>(AssetBundleData.Item item, ref AssetBundle bundle) where T : Object;

        /// <summary>
        /// 异步通过ID获取资源
        /// </summary>
        /// <param name="id">资源ID</param>
        /// <param name="OnComplete">资源加载完成的回调</param>
        void GetAssetAsync<T>(AssetBundleData.ID id, UnityAction<T> OnComplete) where T : Object;

        /// <summary>
        /// 异步通过资源配置信息获取资源
        /// 1。如果配置信息id = None，则表明是自定义信息，所有资源不缓存
        ///     AssetBundle资源位置：item.path + item.bundleName
        ///     Asset位置：item.assetName
        ///     AssetDatabase模式则直接读item.path
        /// 2。如果配置信息id != None，则表明已定义信息，走系统流程
        /// </summary>
        /// <param name="item">配置信息</param>
        /// <param name="bundle">AssetBundle，如果非空，则使用传入的</param>
        /// <param name="OnComplete">资源加载完成后的回调</param>
        void GetAssetAsync<T>(AssetBundleData.Item item, AssetBundle bundle, UnityAction<T,AssetBundle> OnComplete) where T : Object;

        /// <summary>
        /// 释放单个实例
        /// 1。如果是Prefab，则回收实例
        /// 2。如果是纯资源，引用计数-1
        /// </summary>
        /// <param name="asset">实例</param>
        void Release(Object asset);

        /// <summary>
        /// 清除单个资源
        /// 1。如果是Prefab，则回收所有实例
        /// 2。如果是纯资源，则引用计数清零
        /// </summary>
        /// <param name="id">资源ID</param>
        void Clear(AssetBundleData.ID id);

        /// <summary>
        /// 清除所有资源
        /// 1。如果是Prefab，则回收所有实例
        /// 2。如果是纯资源，则引用计数清零
        /// </summary>
        void Clear();

        /// <summary>
        /// 销毁单个资源
        /// 1。如果是Prefab，则消除所有实例和Prefab
        /// 2。如果是纯资源，清除引用计数和消除原始资源
        /// </summary>
        /// <param name="id"></param>
        void Destroy(AssetBundleData.ID id);

        /// <summary>
        /// 销毁整个资源池，包括所有实例和原始资源
        /// </summary>
        void Destroy();

        /// <summary>
        /// 清理OBJECTtoID映射表一些已经被销毁的内容
        /// </summary>
        /// 避免这个映射表膨胀
        void ClearOBJECTtoIDDirtyElements();
    }
}

