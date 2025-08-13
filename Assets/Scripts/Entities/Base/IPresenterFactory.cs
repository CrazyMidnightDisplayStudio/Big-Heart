using System;
using UnityEngine;

namespace Entities.Base
{
    public interface IPresenterFactory<out T> where T : IPresenter
    {
        public T Spawn(BaseEntityDefinition def, Vector3 pos, Transform parent, Guid id, IEntityState state);
    }
}
