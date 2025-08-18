using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace CMD.Entities
{
    public enum EEntityLocationKind { world, container }

    [Serializable]
    public class EntityLocation
    {
        public EEntityLocationKind kind;

        // World
        public string scene;
        public Vector3 position;
        public Quaternion rotation;

        // Container
        public string ownerId; // "Player"/"Chest_42"/"NPC_17"
        public string containerKey; // "Backpack"/"Equipment"/"StorageA"
        public int index; // индекс слота (для grid)
        public string slotKey; // для именованных слотов (опционально)
    }

    [Serializable]
    public class EntitySaveData
    {
        public string definitionKey; // стабильный ключ ItemDefinitionSO
        public string entityId; // Guid в строке (ToString("N"))
        public bool retired; // Объект вне игры - убран из игры
        public EntityLocation location; // Место нахождение предмета

        // Если нужен runtime-state предмета:
        public string stateJson; // JsonUtility.ToJson(state)
    }
}
