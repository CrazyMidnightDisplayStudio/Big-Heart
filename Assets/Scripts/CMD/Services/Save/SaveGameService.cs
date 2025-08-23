using CMD.Base;
using UnityEngine;
namespace CMD.Services.Save
{
    public class SaveGameService : Service, ISaveGameService
    {
        private readonly ISaveRepository _repository;
        private readonly IContainmentService _containmentService;

        public SaveGameService(ISaveRepository repository, IContainmentService containmentService) : base("SaveGameService")
        {
            _repository = repository;
            _containmentService = containmentService;
        }

        public void SaveGame()
        {
            // World
            foreach (var entity in Object.FindObjectsOfType<BaseEntityRuntime>(includeInactive: true))
            {
                if (entity.IsRetired) continue;
                var snap = entity.CaptureData();
                _repository.Upsert(snap);
            }

            // Containers
            foreach (var container in _containmentService.AllContainers)
            {
                for (int i = 0; i < container.Capacity; i++)
                {
                    var entity = container.IndexOfSlot(i);
                    if (entity == null) continue;

                    _repository.Upsert(new EntitySaveData
                    {
                        definitionKey = entity.Definition.Key,
                        entityId = entity.EntityId,
                        retired = false,
                        location = new EntityLocation
                        {
                            kind = EEntityLocationKind.container,
                            ownerId = container.OwnerId,
                            containerKey = container.ContainerKey,
                            index = i
                        },
                        stateJson = JsonUtility.ToJson(entity.State)
                    });
                }
            }

            _repository.Flush();
        }
    }
}
