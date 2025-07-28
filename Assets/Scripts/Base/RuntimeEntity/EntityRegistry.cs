using System;
using System.Collections.Generic;

namespace Base
{
    /// <summary>
    /// Общий реестр всех предметов игре
    /// </summary>
    public sealed class EntityRegistry
    {
        private static readonly EntityRegistry _instance = new EntityRegistry();
        public static EntityRegistry Instance => _instance;

        private EntityRegistry()
        {
        }

        // TODO: заменить Dictionary<string, IEntity> на Dictionary<Guid, IEntity>
        private readonly Dictionary<string, IEntity> _map = new();

        public event Action<IEntity> Added, Removed;

        public void Register(IEntity e)
        {
            if (!_map.TryAdd(e.Id, e))
                throw new Exception($"Duplicate Id {e.Id}");
            Added?.Invoke(e);
        }

        public void Unregister(IEntity e)
        {
            if (_map.Remove(e.Id))
                Removed?.Invoke(e);
        }


        public bool TryGet<T>(string id, out T entity) where T : class, IEntity
        {
            if (_map.TryGetValue(id, out var raw) && raw is T cast)
            {
                entity = cast;
                return true;
            }

            entity = null;
            return false;
        }
    }
}