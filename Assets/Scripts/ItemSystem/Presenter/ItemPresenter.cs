using System;
using UnityEngine;
using Base;
using ItemSystem.Effects;

namespace ItemSystem
{
    [DisallowMultipleComponent]
    public class ItemPresenter : MonoBehaviour, IEntity<ItemDefinition>
    {
        public Guid              Id         => _model.Id;
        public ItemDefinition    Definition => _model.Definition;
        public ItemModel         Model      => _model;

        private ItemModel        _model;
        private ItemView         _view;
        private EffectContainer  _effects;

        public void Init(ItemDefinition def)
        {
            _effects = gameObject.AddComponent<EffectContainer>();
            _model   = new ItemModel(def, _effects);

            _view = gameObject.GetComponent<ItemView>() ?? gameObject.AddComponent<ItemView>();
            _view.Bind(def);

            EntityRegistry.Instance.Register(this);
        }

        /*──── Прокси для геймплея ────*/
        public void HandleEquip()     => _model.OnEquip();
        public void HandleUnEquip()   => _model.OnUnEquip();
        public void HandleDateStart() => _model.OnDateStart();
        public void HandleDateEnd()   => _model.OnDateEnd();

        private void OnDestroy() => EntityRegistry.Instance.Unregister(this);
    }
}
