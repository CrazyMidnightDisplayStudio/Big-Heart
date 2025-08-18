using UnityEngine;

namespace CMD.Entities
{
    public interface IEntityFactory
    {
        BaseEntityRuntime Create(EntityDefinitionSO definition, Vector3 position, Quaternion rotation);
        BaseEntityRuntime CreateFromSave(EntitySaveData saveData, IEntityCatalog catalog);
    }
}
