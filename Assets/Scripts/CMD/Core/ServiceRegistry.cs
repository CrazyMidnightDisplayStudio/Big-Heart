using System;
using System.Collections.Generic;

namespace CMD.Core
{
    /// <summary>
    /// Глобальный сервис-реестр (DI для инди).
    /// </summary>
    public static class ServiceRegistry
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Register<T>(T instance) where T : class
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            _services[typeof(T)] = instance;
        }

        public static T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var s))
                return (T)s;

            throw new InvalidOperationException($"Service {typeof(T).Name} not registered");
        }

        public static bool TryGet<T>(out T instance) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var s))
            {
                instance = (T)s;
                return true;
            }
            instance = null;
            return false;
        }

        public static void Clear() => _services.Clear();
    }
}
