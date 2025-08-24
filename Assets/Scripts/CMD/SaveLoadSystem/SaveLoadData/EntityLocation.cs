using System;
using CMD.Base;
using CMD.Core;
using CMD.Services;
using UnityEngine;

namespace CMD.SaveLoadSystem
{
    [Serializable]
    public struct EntityLocation
    {
        public EEntityLocationKind kind;
        public WorldData world; // используется только при kind == World
        public ContainerData container; // используется только при kind == Container

        public static EntityLocation InContainer(string ownerId, string containerKey, int index)
            => new EntityLocation
            {
                kind = EEntityLocationKind.container, container = new ContainerData(ownerId, containerKey, index)
            };

        public static EntityLocation InWorld(Vector3 position, Quaternion rotation, string scene = null)
            => new EntityLocation
            {
                kind = EEntityLocationKind.world, world = new WorldData(scene, position, rotation)
            };

        public static EntityLocation BuildLocation(BaseEntityRuntime entity)
        {
            if (ServiceRegistry.TryGet<IContainmentService>(out var cs) &&
                cs.TryGetContainerOf(entity, out var container, out var index))
            {
                return EntityLocation.InContainer(container.OwnerId, container.ContainerKey, index);
            }

            // иначе предмет в мире:
            return EntityLocation.InWorld(entity.transform.position, entity.transform.rotation,
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
