using System;
using ItemSystem;
using UnityEngine;
namespace Services
{
    public class StubItemService : IItemService
    {

        public ItemPresenter Create(ItemDefinition def, Vector3 pos = default, Transform parent = null) => new();

        public void Destroy(Guid runtimeId)
        {
        }
        public bool Exists(Guid runtimeId) => false;
        public bool TryGet(Guid runtimeId, out ItemPresenter presenter)
        {
            presenter = null;
            return false;
        }
    }
}
