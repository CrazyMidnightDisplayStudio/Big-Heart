using System;
using UnityEngine;

namespace CMD.Core
{
    [Serializable]
    public class EntitySaveData
    {
        public string definitionKey; // стабильный ключ ItemDefinitionSO
        public string scene; // для фильтрации по сценам (опц.)
        public string ownerId; // если предмет в инвентаре (опц.)
        public string instanceId; // Guid в строке (ToString("N"))
        public Vector3 position;
        public Quaternion rotation;
        public bool consumed;
        // при желании — CustomKV[] custom; // см. примечание ниже
    }
}
