using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace NEO.SUPPORT
{
    public partial class Utility
    {
        /// <summary>
        /// 创建可程序化资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">存放路径</param>
        public static void CreateAsset<T>(string path) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }
    }
}

#endif
