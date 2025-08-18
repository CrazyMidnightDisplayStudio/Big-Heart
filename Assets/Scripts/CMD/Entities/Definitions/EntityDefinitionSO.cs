using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CMD.Entities
{
    [CreateAssetMenu(menuName = "CMD.Core/Entity", fileName = "Entity_")]
    public class EntityDefinitionSO : ScriptableObject
    {
        [Header("Key (stable & unique)")]
        [SerializeField] private string key;
        public string Key => key;

        [Header("UI")]
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;

        [Header("View")]
        public GameObject viewPrefab; // визуал (2D спрайт/анимация)

        [FormerlySerializedAs("rules")]
        public List<GameplayRuleSO> gameplayRules = new();
    }

    [System.Serializable]
    public class GameplayRuleSO
    {
        public TriggerSO trigger;
        public List<EffectSO> effects = new();
    }
}
