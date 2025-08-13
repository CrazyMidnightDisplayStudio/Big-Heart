using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Base;
using Entities.Item.Effect;
using Entities.Item.Model;
using Entities.Item.View;
using Services;
using UnityEngine;

namespace Entities.Item.Presenter
{
    /// <summary>
    /// Cценный носитель модели (без реестров)
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ItemPresenter : MonoBehaviour, IPresenter
    {
        public ItemModel Model => _model;
        public ItemDefinition Definition => _definitionAsset;

        [Header("Definition (asset & runtime)")]
        [SerializeField] private ItemDefinition _definitionAsset; // ассет из Project
        [SerializeField, HideInInspector] private string _serializedId; // пережить Edit→Play

        private ItemModel _model;
        private ItemView _view;
        private EffectContainer _effects;

        private IEventService _eventService;
        private ICoroutineService _coroutineService;

        /// <summary>Инициализация конкретного экземпляра (id обязателен).</summary>
        public void Init(BaseEntityDefinition asset, Guid instanceId, IEntityState state)
        {
            if (instanceId == Guid.Empty && asset is ItemDefinition definition && state is ItemState itemState)
            {
                _definitionAsset = definition;
                _serializedId = instanceId.ToString();
                _eventService = ServiceRegistry.Resolve<IEventService>();
                _coroutineService = ServiceRegistry.Resolve<ICoroutineService>();
                BuildFromDefinition(instanceId, itemState);
            }
            else
            {
                throw new ArgumentException("Wrong arguments!");
            }
        }

        public void ApplyState(ItemState state)
        {
            _model?.ApplyState(state);
            _view.Refresh(Definition, state);
        }

        void BuildFromDefinition(Guid id, ItemState initial)
        {
            var def = _definitionAsset;

            // 1) model
            _model = new ItemModel(def, id, initial);

            // 2) effects -> to runtime asset
            var runtimeEffects = new List<IEffect>(def.effects?.Count ?? 0);
            if (def.effects != null)
            {
                runtimeEffects.AddRange(def.effects.Select(asset => asset.BuildRuntime(_model)));
            }

            // 3) effect container
            _effects?.Dispose();
            _effects = new EffectContainer(runtimeEffects, _eventService, _coroutineService);

            // 4) view
            _view = GetComponent<ItemView>() ?? gameObject.AddComponent<ItemView>();
            _view?.Bind(def);
        }

        void OnDestroy()
        {
            _effects?.Dispose();
        }
    }
}
