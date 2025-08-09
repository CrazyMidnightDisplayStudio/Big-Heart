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

        [Header("Definition (asset & runtime)")]
        [SerializeField] private ItemDefinition _definitionAsset; // ассет из Project
        [NonSerialized] private ItemDefinition _definitionRuntime; // клон на время игры

        private ItemModel _model;
        private ItemView _view;
        private EffectContainer _effects;

        /// <summary>Инициализация из фабрики/дизайнерского окна.</summary>
        public void Init(ItemDefinition asset)
        {
            _definitionAsset = asset;
            // В EditMode просто соберём «лайт» версию (у тебя уже стоят editor-stubs; если нет — см. ниже).
            BuildFromDefinition(cloneForRuntime: Application.isPlaying);
        }

        private void Awake()
        {
            // Если предмет был заспавнен в EditMode, при входе в Play переподнимем всё на живых сервисах.
            if (_definitionAsset != null && Application.isPlaying)
            {
                BuildFromDefinition(cloneForRuntime: true);
            }
        }

        /// <summary>Заменить ассет в рантайме (или из инспектора) и пересобрать.</summary>
        public void AssignDefinition(ItemDefinition asset, bool cloneForRuntime = true)
        {
            _definitionAsset = asset;
            BuildFromDefinition(cloneForRuntime);
        }

        /// <summary>Доступ к текущему редактируемому дефинишену в рантайме.</summary>
        public ItemDefinition CurrentDefinition => _definitionRuntime ?? _definitionAsset;

        /// <summary>Изменить runtime-копию на лету (без затрагивания ассета).</summary>
        public void MutateRuntimeDefinition(Action<ItemDefinition> change)
        {
            if (!Application.isPlaying || _definitionAsset == null)
            {
                return;
            }
            if (_definitionRuntime == null)
            {
                _definitionRuntime = Instantiate(_definitionAsset);
                _definitionRuntime.name = _definitionAsset.name + " (Runtime)";
                _definitionRuntime.hideFlags = HideFlags.DontSave;
            }
            change?.Invoke(_definitionRuntime);
            _view?.Bind(_definitionRuntime); // если поменяли иконку/текст — обновим View
        }

        private void BuildFromDefinition(bool cloneForRuntime)
        {
            var defToUse = _definitionAsset;
            if (cloneForRuntime && _definitionAsset != null)
            {
                _definitionRuntime = Instantiate(_definitionAsset);
                _definitionRuntime.name = _definitionAsset.name + " (Runtime)";
                _definitionRuntime.hideFlags = HideFlags.DontSave;
                defToUse = _definitionRuntime;
            }
            else
            {
                _definitionRuntime = null;
            }

            // 1) runtime-эффекты
            var runtimeEffects = new List<IEffect>(defToUse.effects?.Count ?? 0);
            if (defToUse.effects != null)
            {
                foreach (var asset in defToUse.effects)
                {
                    runtimeEffects.Add(asset.BuildRuntime(null)); // owner пока не нужен
                }
            }

            // 2) контейнер (пересобираем безопасно)
            _effects?.Dispose();
            _effects = new EffectContainer(runtimeEffects);

            // 3) модель
            _model = new ItemModel(defToUse, _effects);

            // 4) view
            _view = gameObject.GetComponent<ItemView>() ?? gameObject.AddComponent<ItemView>();
            _view.Bind(defToUse);

            EntityRegistry.Instance.Register(this);
        }

        private void OnDestroy()
        {
            EntityRegistry.Instance.Unregister(this);
            _effects?.Dispose();
        }
    }
}
