namespace Base
{
    /// <summary>
    /// Маркер для любого объекта на сцене: нужен только Id.
    /// Работать через него будут: EntityRegistry, системы сохранения и т.д.
    /// </summary>
    public interface IEntity
    {
        string Id { get; }        // уникален в сессии / сейве
    }

    /// <summary>
    /// Сильный интерфейс с привязкой к ScriptableObject-дефиниции.
    /// Используется там, где нужна типобезопасность (фабрики, геймплей-логика).
    /// </summary>
    public interface IEntity<TDefinition> : IEntity
        where TDefinition : BaseEntityDefinition
    {
        TDefinition Definition { get; }
        void Init(TDefinition definition);
    }
}
