using CMD.Entities;
using CMD.Services;

namespace CMD.Core
{
    public interface IGameContext
    {
        IEntityCatalog EntityCatalog { get; }
        IEntityFactory EntityFactory { get; }
        ISaveRepository Saves { get; }
        IEventBusService Events { get; }
        ICoroutineService CoroutineService { get; }
        IContainmentService ContainmentService { get; }
    }
}
