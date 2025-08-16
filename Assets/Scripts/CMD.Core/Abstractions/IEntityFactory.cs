using UnityEngine;

namespace CMD.Core
{
    public interface IEntityFactory
    {
        EntityRuntime Create(EntityDefinitionSO def, Vector3 pos, Quaternion rot);
        EntityRuntime CreateFromSave(EntitySaveData s, IEntityCatalog catalog);
    }
}
