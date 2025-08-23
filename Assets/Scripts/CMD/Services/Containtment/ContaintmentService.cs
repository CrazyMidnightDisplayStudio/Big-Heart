using System.Collections.Generic;
using CMD.Base;
using UnityEngine;

namespace CMD.Services
{
    public sealed class ContainmentService : Service, IContainmentService
    {
        private readonly Dictionary<(string owner, string key), IEntityContainer> _containers = new();
        private readonly Dictionary<string, (IEntityContainer container, int index)> _indexes = new();

        public ContainmentService() : base("ContainmentService") { }

        public IEnumerable<IEntityContainer> AllContainers => _containers.Values;
        public bool TryGetContainerOf(BaseEntityRuntime e, out IEntityContainer container, out int index)
        {
            if (_indexes.TryGetValue(e.EntityId, out var tuple))
            {
                container = tuple.container; index = tuple.index;
                return true;
            }
            container = null;
            index = -1;
            return false;
        }

        public void Register(IEntityContainer c) => _containers[(c.OwnerId, c.ContainerKey)] = c;
        public void Unregister(IEntityContainer c) => _containers.Remove((c.OwnerId, c.ContainerKey));

        public IEntityContainer Get(string ownerId, string containerKey)
        {
            _containers.TryGetValue((ownerId, containerKey), out var c);
            return c;
        }

        internal void OnAdded(BaseEntityRuntime entity, IEntityContainer container, int index) => _indexes[entity.EntityId] = (container, index);
        internal void OnRemoved(BaseEntityRuntime entity, IEntityContainer container) => _indexes.Remove(entity.EntityId);
        internal void OnMoved(BaseEntityRuntime entity, IEntityContainer container, int index) => _indexes[entity.EntityId] = (container, index);

        public bool Put(string ownerId, string containerKey, BaseEntityRuntime e, int index = -1, string slotKey = null) =>
            Get(ownerId, containerKey)?.Add(e, index, slotKey) == true;

        public bool TakeOut(string ownerId, string containerKey, BaseEntityRuntime e) =>
            Get(ownerId, containerKey)?.Remove(e) == true;

        public bool MoveInside(string ownerId, string containerKey, BaseEntityRuntime e, int newIndex, string newSlotKey = null) =>
            Get(ownerId, containerKey)?.Move(e, newIndex, newSlotKey) == true;
    }
}
