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
    }
}
