using System.Collections.Generic;
using UnityEngine;

namespace NEO.SUPPORT
{
    /// <summary>
    /// 对象池(一般为Prefab)，只可复制，不可引用源资源
    /// </summary>
    public class PrefabPool : IPool
    {
        Object asset;

        List<Object> inUse;
        List<Object> available;

        public void Initialize(Object asset)
        {
            this.asset = asset;
            inUse = new List<Object>();
            available = new List<Object>();
        }

        public Object Instantiate()
        {
            Object newAsset;
            if (available.Count == 0)
            {
                newAsset = Object.Instantiate(asset);
                available.Add(newAsset);
            }
            int lastIndex = available.Count - 1;
            newAsset = available[lastIndex];
            available.RemoveAt(lastIndex);
            inUse.Add(newAsset);
            return newAsset;
        }

        public Object Asset()
        {
            return asset;
        }

        public bool IsUsing()
        {
            return inUse.Count > 0;
        }

        public void Release(Object asset)
        {
            inUse.Remove(asset);
            available.Add(asset);
        }

        public void Clear()
        {
            for (int i = 0; i < available.Count; ++i)
            {
                Object.Destroy(available[i]);
            }
            available.Clear();
            for (int i = 0; i < inUse.Count; ++i)
            {
                Object.Destroy(inUse[i]);
            }
            inUse.Clear();
        }

        public void Destroy()
        {
            Clear();
            inUse = null;
            available = null;
            asset = null;
        }
    }
}

