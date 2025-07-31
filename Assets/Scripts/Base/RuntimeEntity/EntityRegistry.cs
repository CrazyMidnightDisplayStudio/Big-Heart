using System;
using System.Collections.Generic;
using Base;

public sealed class EntityRegistry
{
    private static readonly EntityRegistry _instance = new();
    public  static EntityRegistry Instance => _instance;

    private readonly Dictionary<Guid, IEntity> _map = new();

    public event Action<IEntity> Added, Removed;

    public void Register(IEntity e)
    {
        if (!_map.TryAdd(e.Id, e))
        {
            throw new Exception($"Duplicate Id {e.Id}");
        }

        Added?.Invoke(e);
    }

    public void Unregister(IEntity e)
    {
        if (_map.Remove(e.Id))
        {
            Removed?.Invoke(e);
        }
    }

    public bool TryGet<T>(Guid id, out T entity) where T : class, IEntity
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
