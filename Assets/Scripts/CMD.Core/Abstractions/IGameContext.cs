namespace CMD.Core
{
    public interface IGameContext
    {
        IEntityCatalog EntityCatalog { get; }
        IEntityFactory EntityFactory { get; }
        ISaveRepository Saves { get; }
        IEventBus Events { get; }
        IScheduler Scheduler { get; }
    }
}
