using System;
using Base;
using ItemSystem;
using UnityEngine;
using Object = UnityEngine.Object;

public sealed class ItemFactory : IEntityFactory<ItemMonoEntity, ItemDefinition>
{
    private readonly EntityRegistry _registry = EntityRegistry.Instance;

    public ItemMonoEntity Spawn(ItemDefinition def, Vector3 pos, Transform parent = null)
    {
        if (def == null) throw new ArgumentNullException(nameof(def));
        if (def.prefab == null) throw new Exception($"{def.name} prefab is null");

        var go = Object.Instantiate(def.prefab, pos, Quaternion.identity, parent);
        var item = go.GetComponent<ItemMonoEntity>() ?? go.AddComponent<ItemMonoEntity>();

        item.Init(def);
        _registry.Register(item);
        return item;
    }
}