using CMD.Core;
using UnityEngine;

namespace CMD.Base
{
    public interface IEntityFactory<out TRuntime, TDefinition>
        where TRuntime : BaseEntityRuntime
        where TDefinition : EntityDefinitionSO
    {
        TRuntime Create(TDefinition definition, Vector3 position, Quaternion rotation);
        TRuntime CreateFromSave(EntitySaveData saveData, ICatalog<TDefinition> catalog);
    }
}
