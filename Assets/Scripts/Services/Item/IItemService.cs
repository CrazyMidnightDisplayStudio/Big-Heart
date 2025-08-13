using System;
using Entities.Item.Model;
using Entities.Item.Presenter;
using UnityEngine;

namespace Services
{
    public interface IItemService
    {
        ItemPresenter Create(ItemDefinition def, Vector3 pos = default, Transform parent = null);
        void Destroy(Guid runtimeId);
        bool Exists(Guid runtimeId);
        bool TryGet(Guid runtimeId, out ItemPresenter presenter);
    }
}
