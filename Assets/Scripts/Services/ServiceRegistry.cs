using System;
using System.Collections.Generic;

namespace Services
{
    public static class ServiceRegistry
    {
        private static readonly Dictionary<Type, object> _map = new();

        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (_map.ContainsKey(type))
            {
                throw new Exception($"Service {type.Name} already registered");
            }

            _map[type] = service;
        }

        public static T Resolve<T>() where T : class
        {
            if (_map.TryGetValue(typeof(T), out var srv) && srv is T cast)
                return cast;

            throw new Exception($"Service {typeof(T).Name} not found");
        }
    }
}