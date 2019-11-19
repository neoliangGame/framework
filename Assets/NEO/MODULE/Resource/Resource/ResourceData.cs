using UnityEngine;

namespace NEO.MODULE
{
    /// <summary>
    /// 资源ID与资源存储路径对应表
    /// 用于程序内读取，需要手动维护
    /// </summary>
    public class ResourceData : ScriptableObject
    {
        public Item[] items;

        [System.Serializable]
        public class Item
        {
            /// <summary>
            /// 资源ID，用于唯一标识资源
            /// 游戏内部逻辑直接通过ID对应获取资源
            /// </summary>
            public ID id = ID.None;

            /// <summary>
            /// 资源存放路径（在Resources文件夹下的相对路径）
            /// </summary>
            public string path = "";

            /// <summary>
            /// 是否是预制体
            /// 预制体和纯资源处理方式不一样
            /// 1。预制体，每次获取资源都是获取其实例
            /// 2。纯资源，获取其本身引用
            /// </summary>
            public bool isPrefab = false;

            /// <summary>
            /// 是否预加载资源
            /// 如果是，则在创建内存池管理的时候，会预先加载所有预加载资源
            /// </summary>
            public bool preLoad = false;
        }

        public enum ID
        {
            None,

            //以下根据实际项目资源设置内容......
            TestID1,
            TestID2,
            TestID3,
            TestID4,
        }
    }
}

