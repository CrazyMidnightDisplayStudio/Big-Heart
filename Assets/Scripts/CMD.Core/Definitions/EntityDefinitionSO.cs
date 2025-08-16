using System.Collections.Generic;
using UnityEngine;

namespace CMD.Core
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

        [Header("Rules")]
        public List<RuleSO> rules = new();
    }

    [System.Serializable]
    public class RuleSO
    {
        public TriggerSO trigger;
        public List<EffectSO> effects = new();
    }
}
