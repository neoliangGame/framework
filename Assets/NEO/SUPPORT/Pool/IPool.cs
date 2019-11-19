using UnityEngine;

namespace NEO.SUPPORT
{
    public interface IPool
    {
        void Initialize(Object asset);

        Object Instantiate();

        Object Asset();

        bool IsUsing();

        void Release(Object asset);

        void Clear();

        void Destroy();
    }
}

