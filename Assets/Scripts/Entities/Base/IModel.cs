using System;

namespace Entities.Base
{
    public interface IModel
    {
        public Guid Id { get; }

        public void ApplyState(IEntityState state);
    }
}
