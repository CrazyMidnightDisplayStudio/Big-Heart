using System;
using Entities.Item.Model;
namespace Entities.Item.Presenter
{
    public readonly struct ItemRuntime
    {
        public Guid Id { get; }
        public ItemDefinition Definition { get; }
        public ItemState State { get; }

        public ItemRuntime(Guid id, ItemDefinition definition, ItemState state)
        {
            Id = id; Definition = definition;  State = state;
        }
    }
}
