using UnityEngine;

namespace Base
{
    public interface IEntityFactory<out TEntity, in TDefinition>
        where TEntity : IEntity<TDefinition>
        where TDefinition : BaseEntityDefinition
    {
        TEntity Spawn(TDefinition def, Vector3 pos, Transform parent = null);
    }
}