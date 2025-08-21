using CMD.Entities;

namespace BigHeart.Events.Inventory
{
    public readonly struct OnAddedEvent
    {
        public BaseEntityRuntime Entity { get; }
        public IEntityContainer Container { get; }
        public int Index { get; }


        public OnAddedEvent(BaseEntityRuntime entity, IEntityContainer container, int index)
        {
            Entity = entity;
            Container = container;
            Index = index;
        }
    }
}
