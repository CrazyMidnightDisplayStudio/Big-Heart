using System;
using Base;
using ItemSystem;
using UnityEngine;
using Object = UnityEngine.Object;

public sealed class ItemFactory : IEntityFactory<ItemMono, ItemDefinition>
{
    private readonly EntityRegistry _registry = EntityRegistry.Instance;

    public ItemMono Spawn(ItemDefinition def, Vector3 pos, Transform parent = null)
    {
        if (def == null) throw new ArgumentNullException(nameof(def));
        if (def.prefab == null) throw new Exception($"{def.name} prefab is null");

        var go = Object.Instantiate(def.prefab, pos, Quaternion.identity, parent);
        var item = go.GetComponent<ItemMono>() ?? go.AddComponent<ItemMono>();

        item.Init(def);
        _registry.Register(item);
        return item;
    }
}