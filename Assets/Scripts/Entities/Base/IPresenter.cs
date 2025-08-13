using System;

namespace Entities.Base
{
    public interface IPresenter
    {
        public void Init(BaseEntityDefinition definition, Guid instanceId, IEntityState state);
    }
}
