using Entities.Item.Model;
using UnityEngine;

namespace Entities.Item.Effect.Base
{
    public abstract class BaseEffectAsset : ScriptableObject
    {
        public abstract IEffect BuildRuntime(ItemModel owner);
    }
}
