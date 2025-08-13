using System;
using UnityEngine;

namespace Entities.Item.Factory
{
    public readonly struct SpawnResult<TPresenter> where TPresenter : Component
    {
        public TPresenter Presenter { get; }
        public Guid Id { get; }
        public SpawnResult(TPresenter presenter, Guid id)
        {
            Presenter = presenter;
            Id = id;
        }
    }
}
