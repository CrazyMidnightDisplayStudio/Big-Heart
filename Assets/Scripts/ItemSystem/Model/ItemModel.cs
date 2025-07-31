using System;
using Base;
using ItemSystem.Effects;

namespace ItemSystem
{
    /// <summary>
    /// Model — без MonoBehaviour.
    /// </summary>
    public class ItemModel : IEntity<ItemDefinition>
    {
        public Guid Id { get; }
        public ItemDefinition Definition { get; }

        private readonly EffectContainer _effects;

        public ItemModel(ItemDefinition definition, EffectContainer effects)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            _effects    = effects   ?? throw new ArgumentNullException(nameof(effects));
            Id = Guid.NewGuid();
        }

        /*──── Gameplay passthrough ────*/
        public void OnEquip()     => _effects.OnEquip();
        public void OnUnEquip()   => _effects.OnUnEquip();
        public void OnDateStart() => _effects.OnDateStart();
        public void OnDateEnd()   => _effects.OnDateEnd();
    }
}
