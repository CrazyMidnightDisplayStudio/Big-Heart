using System.Collections.Generic;
using CMD.Base;

namespace CMD.Services
{
    public interface IContainmentService
    {
        // Резолв по владельцу+ключу
        IEntityContainer Get(string ownerId, string containerKey);

        void Register(IEntityContainer container);
        void Unregister(IEntityContainer container);

        // Получить нужный контейнер
        bool TryGetContainerOf(BaseEntityRuntime entity, out IEntityContainer container, out int index);
        IEnumerable<IEntityContainer> AllContainers { get; }

        // Сахар
        bool Put(string ownerId, string containerKey, BaseEntityRuntime entity, int index = -1, string slotKey = null);
        bool TakeOut(string ownerId, string containerKey, BaseEntityRuntime entity);
        bool MoveInside(string ownerId, string containerKey, BaseEntityRuntime entity, int newIndex, string newSlotKey = null);
    }
}
