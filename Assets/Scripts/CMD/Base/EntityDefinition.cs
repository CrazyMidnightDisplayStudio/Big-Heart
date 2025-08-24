using UnityEngine;

namespace CMD.Base
{
    /// <summary>Абстрактное определение сущности: только ключ.</summary>
    [CreateAssetMenu(menuName = "CMD.Core/Entity", fileName = "Entity_")]
    public class EntityDefinition : ScriptableObject
    {
        [SerializeField] private string key;
        public string Key => key;
    }
}
