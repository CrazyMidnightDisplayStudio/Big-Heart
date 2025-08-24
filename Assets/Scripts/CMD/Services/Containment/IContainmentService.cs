using System;
using System.Collections.Generic;
using CMD.Base;
using CMD.Events.ContainmentEvents;
using CMD.SaveLoadSystem;

namespace CMD.Services
{
    public interface IContainmentService
    {
        // Резолв и перечисление
        IEntityContainer Get(string ownerId, string containerKey);
        void Register(IEntityContainer container);
        void Unregister(IEntityContainer container);
        bool TryGetContainerOf(BaseEntityRuntime entity, out IEntityContainer container, out int index);
        IEnumerable<IEntityContainer> AllContainers { get; }

        // Сахар-операции (предпочтительный способ модификации)
        bool Put(string ownerId, string containerKey, BaseEntityRuntime entity, int index = -1, string slotKey = null, EChangeOrigin origin = EChangeOrigin.gameplay);
        bool TryPut(string ownerId, string containerKey, BaseEntityRuntime entity,
            int index = -1, string slotKey = null,
            EChangeOrigin origin = EChangeOrigin.gameplay);
        bool TryPut(BaseEntityRuntime entity, ContainerData data,
            EChangeOrigin origin = EChangeOrigin.gameplay);
        bool TakeOut(string ownerId, string containerKey, BaseEntityRuntime entity, EChangeOrigin origin = EChangeOrigin.gameplay);
        bool MoveInside(string ownerId, string containerKey, BaseEntityRuntime entity, int newIndex, string newSlotKey = null, EChangeOrigin origin = EChangeOrigin.gameplay);

        // Нотификации — вызови их, если контейнер изменили напрямую (в обход Put/TakeOut/MoveInside)
        void NotifyAdded(BaseEntityRuntime entity, IEntityContainer container, int index, EChangeOrigin origin = EChangeOrigin.gameplay);
        void NotifyRemoved(BaseEntityRuntime entity, IEntityContainer container, EChangeOrigin origin = EChangeOrigin.gameplay);
        void NotifyMoved(BaseEntityRuntime entity, IEntityContainer container, int newIndex, EChangeOrigin origin = EChangeOrigin.gameplay);

        // Подавление событий (батч загрузки)
        IDisposable SuppressEventsScope();
    }
}
