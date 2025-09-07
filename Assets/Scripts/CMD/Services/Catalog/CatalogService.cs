using System;
using System.Collections.Generic;
using UnityEngine;

namespace CMD.Services
{
    public class CatalogService : Service, ICatalog
    {
        private readonly Dictionary<Type, object> _folders = new(); // typeof(T) -> IFolderCatalog<T>

        public CatalogService() : base("Catalog")
        {
            Debug.Log("Usage: Get<UnityEngine.AudioClip>(Keys_CMD_Catalog_Audio.cartoon_jump_6462)");
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

        public T Get<T>(CatalogKey<T> catalogKey) where T : UnityEngine.Object => Get<T>(catalogKey.Value);
        public bool TryGet<T>(CatalogKey<T> catalogKey, out T asset) where T : UnityEngine.Object => TryGet<T>(catalogKey.Value, out asset);
    }
}
