using CMD.Base;
using CMD.Core;
using CMD.SaveLoadSystem;

namespace BigHeart
{

    public sealed class ItemRuntime : BaseEntityRuntime, ISaveLoadObject
    {
        public string ComponentSaveId => $"Item_{EntityId}";

        public SaveLoadData Capture()
        {
            var dto = new EntitySaveData
            {
                version = 1,
                entityId = EntityId,
                definitionKey = Definition ? Definition.Key : null,
                location = EntityLocation.BuildLocation(this),
            };

            return SaveLoadData.Create(ComponentSaveId, ESaveType.entity, dto, version: dto.version);
        }

        public void RestoreData(SaveLoadData data)
        {
            var dto = data.Read<EntitySaveData>();
            RestoreFromDto(dto, initDefinition: true);
        }

        // Новый быстрый путь — когда у тебя уже есть def и dto:
        public void RestoreFromDto(EntitySaveData dto, bool initDefinition)
        {
            if (initDefinition)
            {
                var catalog = ServiceRegistry.Get<ICatalog<EntityDefinition>>();
                var def = catalog.GetByKey(dto.definitionKey);
                Init(def);
            }

            if (dto.location.kind == EEntityLocationKind.world)
                transform.SetPositionAndRotation(dto.location.world.position, dto.location.world.rotation);
        }

    }
}
