using System;
using System.Collections.Generic;
using UnityEngine;
using Base;
using ItemSystem.Effects;

namespace ItemSystem
{
    [DisallowMultipleComponent]
    public class ItemPresenter : MonoBehaviour, IEntity<ItemDefinition>
    {
        public Guid Id => _model.Id;
        public ItemDefinition Definition => _model.Definition;
        public ItemModel Model => _model;

        private ItemModel _model;
        private ItemView _view;
        private EffectContainer _effects;

        public void Init(ItemDefinition def)
        {
            // 1. создаём runtime-копии ассетов
            var runtimeEffects = new List<IEffect>(def.effects.Count);
            foreach (var asset in def.effects)
                runtimeEffects.Add(asset.BuildRuntime(null)); // owner пока не нужен

            // 2. контейнер эффектов
            _effects = new EffectContainer(runtimeEffects);

            // 3. модель
            _model = new ItemModel(def, _effects);

            _view = gameObject.GetComponent<ItemView>() ?? gameObject.AddComponent<ItemView>();
            _view.Bind(def);

            EntityRegistry.Instance.Register(this);
        }

        private void OnDestroy()
        {
            EntityRegistry.Instance.Unregister(this);
            _effects.Dispose();
        }
    }
}
