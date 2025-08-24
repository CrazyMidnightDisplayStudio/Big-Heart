using System;
using System.Collections.Generic;
using CMD.Base;
using CMD.Core;
using CMD.Events.ContainmentEvents;
using CMD.SaveLoadSystem;

namespace CMD.Services
{
    public sealed class ContainmentService : Service, IContainmentService
    {
        private readonly Dictionary<(string owner, string key), IEntityContainer> _containers = new();
        private readonly Dictionary<string, (IEntityContainer container, int index)> _indexes = new();
        private int _suppress;

        private IEventBusService Bus => ServiceRegistry.Get<IEventBusService>(); // твой EventBus

        public ContainmentService() : base("ContainmentService") { }

        public IEnumerable<IEntityContainer> AllContainers => _containers.Values;

        public bool TryGetContainerOf(BaseEntityRuntime e, out IEntityContainer container, out int index)
        {
            if (_indexes.TryGetValue(e.EntityId, out var tuple))
            {
                container = tuple.container;
                index = tuple.index;
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

        public IDisposable SuppressEventsScope()
        {
            _suppress++;
            return new Scope(() => _suppress--);
        }

        // --------- Сахар-операции (делаем изменение + индексацию + событие) ---------
        public bool Put(string ownerId, string containerKey, BaseEntityRuntime e, int index = -1, string slotKey = null, EChangeOrigin origin = EChangeOrigin.gameplay)
        {
            var c = Get(ownerId, containerKey);
            if (c == null) return false;
            var ok = c.Add(e, index, slotKey);
            if (!ok) return false;

            // индексация и событие
            CommitAdded(e, c, index >= 0 ? index : TryGetIndex(e, c), origin);
            return true;
        }

        public bool TryPut(string ownerId, string containerKey, BaseEntityRuntime e,
            int index = -1, string slotKey = null,
            EChangeOrigin origin = EChangeOrigin.gameplay)
            => Put(ownerId, containerKey, e, index, slotKey, origin);

        public bool TryPut(BaseEntityRuntime e, ContainerData data,
            EChangeOrigin origin = EChangeOrigin.gameplay)
            => Put(data.ownerId, data.containerKey, e, data.index, null, origin);

        public bool TakeOut(string ownerId, string containerKey, BaseEntityRuntime e, EChangeOrigin origin = EChangeOrigin.gameplay)
        {
            var c = Get(ownerId, containerKey);
            if (c == null) return false;
            var ok = c.Remove(e);
            if (!ok) return false;

            CommitRemoved(e, c, origin);
            return true;
        }

        public bool MoveInside(string ownerId, string containerKey, BaseEntityRuntime e, int newIndex, string newSlotKey = null, EChangeOrigin origin = EChangeOrigin.gameplay)
        {
            var c = Get(ownerId, containerKey);
            if (c == null) return false;

            // найдем старый индекс ДО перемещения
            var from = TryGetIndex(e, c);

            var ok = c.Move(e, newIndex, newSlotKey);
            if (!ok) return false;

            CommitMoved(e, c, from, newIndex, origin);
            return true;
        }

        // --------- Нотификации от контейнеров (если их дернули напрямую) ---------
#region notifications
        public void NotifyAdded(BaseEntityRuntime e, IEntityContainer c, int index, EChangeOrigin origin = EChangeOrigin.gameplay)
            => CommitAdded(e, c, index, origin);

        public void NotifyRemoved(BaseEntityRuntime e, IEntityContainer c, EChangeOrigin origin = EChangeOrigin.gameplay)
            => CommitRemoved(e, c, origin);

        public void NotifyMoved(BaseEntityRuntime e, IEntityContainer c, int newIndex, EChangeOrigin origin = EChangeOrigin.gameplay)
        {
            var from = TryGetIndex(e, c);
            CommitMoved(e, c, from, newIndex, origin);
        }
#endregion notifications

        // --------- Внутренние коммиты индекса + публикации ---------

        private void CommitAdded(BaseEntityRuntime e, IEntityContainer c, int index, EChangeOrigin origin)
        {
            _indexes[e.EntityId] = (c, index);
            if (_suppress == 0) Bus.Publish(new AddedToContainer(c.OwnerId, c.ContainerKey, e.EntityId, index, origin));
        }

        private void CommitRemoved(BaseEntityRuntime e, IEntityContainer c, EChangeOrigin origin)
        {
            _indexes.Remove(e.EntityId);
            if (_suppress == 0) Bus.Publish(new RemovedFromContainer(c.OwnerId, c.ContainerKey, e.EntityId, origin));
        }

        private void CommitMoved(BaseEntityRuntime e, IEntityContainer c, int from, int to, EChangeOrigin origin)
        {
            _indexes[e.EntityId] = (c, to);
            if (_suppress == 0) Bus.Publish(new MovedIntoContainer(c.OwnerId, c.ContainerKey, e.EntityId, from, to, origin));
        }

        private int TryGetIndex(BaseEntityRuntime e, IEntityContainer c)
        {
            // если контейнер умеет отдать индекс — хорошо; иначе берём из кэша
            if (_indexes.TryGetValue(e.EntityId, out var t) && t.container == c) return t.index;
            return c.IndexOf(e); // добавь такой метод в IEntityContainer, если нужно
        }

        private sealed class Scope : IDisposable
        {
            private readonly Action _onDispose;
            public Scope(Action onDispose) => _onDispose = onDispose;
            public void Dispose() => _onDispose?.Invoke();
        }
    }
}
