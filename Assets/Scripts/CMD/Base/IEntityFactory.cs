using CMD.Services;
using UnityEngine;

namespace CMD.Base
{
    public interface IEntityFactory<out TRuntime, TDefinition>
        where TRuntime : BaseEntityRuntime
        where TDefinition : EntityDefinition
    {
        TRuntime Create(TDefinition definition, Vector3 position, Quaternion rotation);
        TRuntime CreateFromSave(SaveLoadSystem.SaveLoadData saveData, ICatalog catalog);
    }
}
