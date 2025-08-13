using System;
using Entities.Base;
using Entities.Item.Model;
using Entities.Item.Presenter;
using Services;
using UnityEngine;

namespace Entities.Item.Factory
{
    public sealed class ItemFactory : IPresenterFactory<ItemPresenter>
    {
        private readonly IEventService _eventService;
        private readonly ICoroutineService _coroutineService;
        public ItemFactory(IEventService eventService, ICoroutineService coroutineService)
        {
            _eventService = eventService;
            _coroutineService = coroutineService;
        }

        public ItemPresenter Spawn(BaseEntityDefinition definition, Vector3 pos, Transform parent, Guid id, IEntityState state)
        {
            if (definition is not ItemDefinition def)
            {
                throw new ArgumentException($"{nameof(definition)} is not {nameof(ItemDefinition)}");
            }
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Instance id must be non-empty", nameof(id));
            }

            GameObject go = new GameObject($"Item_{(string.IsNullOrWhiteSpace(def.displayName) ? def.name : def.displayName)}",
                typeof(SpriteRenderer));
            go.transform.SetPositionAndRotation(pos, Quaternion.identity);

            var p = go.GetComponent<ItemPresenter>() ?? go.AddComponent<ItemPresenter>();
            p.Init(def, id, state);
            return p;
        }
    }
}
