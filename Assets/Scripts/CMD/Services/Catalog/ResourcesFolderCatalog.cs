using System;
using System.Collections.Generic;
using UnityEngine;

namespace CMD.Services
{
    public sealed class ResourcesFolderCatalog<T> : IFolderCatalog<T> where T : UnityEngine.Object
    {
        private readonly string _resourcesPath;
        private readonly Dictionary<string, T> _byKey = new(StringComparer.Ordinal);
        private bool _loaded;

        public ResourcesFolderCatalog(string resourcesPath) => _resourcesPath = resourcesPath;

        private void EnsureLoaded()
        {
            if (_loaded) return;
            _loaded = true;
            foreach (var obj in Resources.LoadAll<T>(_resourcesPath))
            {
                if (!obj) continue;
                var key = obj.name; // ключ = имя файла без расширения
                _byKey[key] = obj;
            }
        }

        public bool TryGet(string key, out T asset)
        {
            EnsureLoaded();
            return _byKey.TryGetValue(key, out asset) && asset;
        }

        public IEnumerable<string> Keys
        {
            get
            {
                EnsureLoaded();
                return _byKey.Keys;
            }
        }
    }
}
