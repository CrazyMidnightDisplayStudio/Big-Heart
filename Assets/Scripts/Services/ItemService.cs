using System.Collections.Generic;
using Base;
using ItemSystem;
using UnityEngine;

namespace Services
{
    /// <summary> Единая точка, через которую игровой код спавнит и удаляет предметы. </summary>
    public class ItemService : BaseServiceSingleton<ItemService>
    {
        // глобальный реестр уникальных Id
        private EntityRegistry _registry = EntityRegistry.Instance;

        // отвечает за Instantiate + Init
        private ItemFactory _factory = new();

        public override void Init()
        {
            base.Init();
            Debug.Log("<color=green>ItemService ready</color>");
        }

        /*──────────────────────── API для остального кода ─────────────────────*/
        public ItemMono Create(ItemDefinition def, Vector3 pos = default, Transform parent = null)
            => _factory.Spawn(def, pos, parent);

        public void Destroy(string instanceId)
        {
            if (!_registry.TryGet(instanceId, out ItemMono item)) return;

            _registry.Unregister(item); // убираем из реестра
            Object.Destroy(item.gameObject); // уничтожаем в сцене
        }

        /// Получить ссылку на Item по Id. Возвращает true, если нашёл.
        public bool TryGet(string instanceId, out ItemMono item)
            => _registry.TryGet(instanceId, out item);

        /// Проверка существования
        public bool Exists(string instanceId) => _registry.TryGet<ItemMono>(instanceId, out _);

        // TODO: сделать поиск всех предметов по тэгу
        // public IEnumerable<ItemMono> AllWithTag(string tag)
        // {
        //     // Требуется свойство .All в EntityRegistry: IReadOnlyCollection<IEntity> All
        //     foreach (var e in _registry.All)
        //         if (e is ItemMono it && it.Tag == tag)
        //             yield return it;
        // }
    }
}