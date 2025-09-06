using System.Collections.Generic;

namespace CMD.Services
{
    public interface IFolderCatalog<T> where T : UnityEngine.Object
    {
        // сам каталог знает, как грузить и какие у него ключи
        bool TryGet(string key, out T asset);
        IEnumerable<string> Keys { get; } // опционально
    }
}
