using UnityEngine;

namespace CMD.Entities
{
    /// <summary>Абстрактное определение сущности: только ключ.</summary>
    [CreateAssetMenu(menuName = "CMD.Core/Entity", fileName = "Entity_")]
    public class EntityDefinitionSO : ScriptableObject
    {
        [SerializeField] private string key;
        public string Key => key;
    }
}
