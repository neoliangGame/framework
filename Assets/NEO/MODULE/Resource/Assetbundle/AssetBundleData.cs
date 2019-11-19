using UnityEngine;

namespace NEO.MODULE
{
    public class AssetBundleData : ScriptableObject
    {
        public Item[] items;

        [System.Serializable]
        public class Item
        {
            /// <summary>
            /// 资源唯一ID
            /// </summary>
            public ID id = ID.None;

            /// <summary>
            /// 资源的assetbundle的文件位置
            /// </summary>
            public string bundleName = "";

            /// <summary>
            /// 资源在对应assetbundle内部的路径，等于资源名称
            /// </summary>
            public string assetName = "";

            /// <summary>
            /// 原始资源的地址
            /// 用于AssetDatabase模式下直接加载原始资源
            /// </summary>
            public string path = "";

            /// <summary>
            /// 如果是prefab，则获取的是其实例化的对象物体；
            /// 如果不是prefab，则获取其asset的引用。
            /// </summary>
            public bool isPrefab = false;

            /// <summary>
            /// 是否预加载
            /// </summary>
            public bool preLoad = false;
        }

        public enum ID
        {
            None,
            Manifest,
            
            //以下内容根据实际扩展...
            Test1,
            Test2,
            Test3,
            Test4,
            test5,
        }
    }
}

