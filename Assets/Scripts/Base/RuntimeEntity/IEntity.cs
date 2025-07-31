using System;

namespace Base
{
    /// <summary>
    /// Маркер для любого объекта на сцене: нужен только Id.
    /// Работать через него будут: EntityRegistry, системы сохранения и т.д.
    /// </summary>
    public interface IEntity
    {
        Guid Id { get; }
    }

    /// <summary>
    /// Сильный интерфейс с привязкой к ScriptableObject-дефиниции.
    /// Используется там, где нужна типобезопасность (фабрики, геймплей-логика).
    /// </summary>
    public interface IEntity<out TDefinition> : IEntity
        where TDefinition : BaseEntityDefinition
    {
        TDefinition Definition { get; }
    }
}
