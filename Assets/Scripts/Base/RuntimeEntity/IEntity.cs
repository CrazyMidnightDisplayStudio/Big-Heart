namespace Base
{
    /// <summary>
    /// IEntity - интерфейс, который должен быть у всех объектов на сцене
    /// чтобы любой объект можно было найти по id
    /// </summary>
    public interface IEntity
    {
        string Id { get; } // уникальный в сессии
    }

    public interface IEntity<TDefinition> : IEntity
        where TDefinition : BaseEntityDefinition
    {
        TDefinition Definition { get; }
        void Init(TDefinition definition);
    }
}