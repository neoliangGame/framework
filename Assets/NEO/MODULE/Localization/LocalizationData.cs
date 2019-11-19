using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NEO.MODULE
{
    public class LocalizationData : ScriptableObject
    {
        public Item[] items;

        [System.Serializable]
        public class Item
        {
            /// <summary>
            /// 文本ID
            /// </summary>
            public ID id = ID.None;

            /// <summary>
            /// 文本内容
            /// </summary>
            public string content = "";
        }

        public enum ID
        {
            None,

            //以下自行根据实际添加或者修改.......
            Test1,
            Test2,
            Test3,
        }
    }
}

