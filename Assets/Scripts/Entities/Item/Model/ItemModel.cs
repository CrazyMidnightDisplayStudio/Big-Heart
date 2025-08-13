using System;
using Entities.Base;

namespace Entities.Item.Model
{
    public sealed class ItemModel : IModel
    {
        public Guid Id { get; }
        public ItemDefinition Definition { get; }
        public ItemState State { get; private set; }

        public ItemModel(ItemDefinition definition, Guid id, ItemState initial = default)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            Id = (id != Guid.Empty) ? id : throw new ArgumentException("Instance id must be non-empty", nameof(id));
            State = initial;
        }

        public void ApplyState(IEntityState state)
        {
            if (state is ItemState itemState)
            {
                State = itemState;
            }
            else
            {
                throw new ArgumentException("Instance state must be of type ItemState", nameof(state));
            }
        }
    }
}
