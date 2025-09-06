using System;
using System.Collections.Generic;

namespace CMD.Services
{
    public class CatalogService : Service, ICatalog
    {
        private readonly Dictionary<Type, object> _folders = new(); // typeof(T) -> IFolderCatalog<T>

        public CatalogService() : base("Catalog")
        {
        }

        public void RegisterFolder<T>(IFolderCatalog<T> folder) where T : UnityEngine.Object
        {
            _folders[typeof(T)] = folder;
        }

        private IFolderCatalog<T> Folder<T>() where T : UnityEngine.Object
        {
            if (_folders.TryGetValue(typeof(T), out object f))
            {
                return (IFolderCatalog<T>)f;
            }

            throw new InvalidOperationException($"Folder for type {typeof(T).Name} is not registered.");
        }

        public T Get<T>(string key) where T : UnityEngine.Object
        {
            var folder = Folder<T>();
            if (folder.TryGet(key, out var a))
            {
                return a;
            }

            throw new KeyNotFoundException($"Key '{key}' not found in {typeof(T).Name} folder.");
        }

        public bool TryGet<T>(string key, out T asset) where T : UnityEngine.Object
        {
            return Folder<T>().TryGet(key, out asset);
        }
    }
}
