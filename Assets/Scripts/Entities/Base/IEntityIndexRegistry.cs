using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;
namespace Entities.Base
{
    public interface IEntityIndexRegistry
    {
        void Register(Guid id, UnityEngine.Object obj);
        void Unregister(Guid id, UnityEngine.Object obj);
        bool TryGet<T>(Guid id, out T comp) where T : UnityEngine.Object;
        IEnumerable<T> All<T>() where T : UnityEngine.Object;
        void Clear();
    }
}
