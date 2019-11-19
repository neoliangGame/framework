using UnityEngine;

namespace NEO.SUPPORT
{
    /// <summary>
    /// 资源池，不可复制，只可引用
    /// </summary>
    public class AssetPool : IPool
    {
        Object asset;
        int count;

        public void Initialize(Object asset)
        {
            this.asset = asset;
            count = 0;
        }

        public Object Instantiate()
        {
            ++count;
            return asset;
        }

        public Object Asset()
        {
            return asset;
        }

        public bool IsUsing()
        {
            return count > 0;
        }

        public void Release(Object asset)
        {
            --count;
        }

        public void Clear()
        {
            count = 0;
        }

        public void Destroy()
        {
            Clear();
            asset = null;
        }
    }
}

