using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace Services
{
    public static partial class ServiceRegistry
    {
        private static readonly Dictionary<Type, object> _map = new Dictionary<Type, object>();

        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (!_map.TryAdd(type, service))
            {
                throw new Exception($"Service {type.Name} already registered");
            }
        }

        public static T Resolve<T>() where T : class
        {
            if (_map.TryGetValue(typeof(T), out object srv) && srv is T cast)
            {
                return cast;
            }

// #if UNITY_EDITOR
//             // В редакторе, пока ИГРА НЕ запущена – отдаём безопасные стабы.
//             if (!Application.isPlaying && EditorStubs.TryGetStub(typeof(T), out object stub))
//             {
//                 return (T)stub;
//             }
// #endif

            throw new Exception($"Service {typeof(T).Name} not found");
        }
    }
}
