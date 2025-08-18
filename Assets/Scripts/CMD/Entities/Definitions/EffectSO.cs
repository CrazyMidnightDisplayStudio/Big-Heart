using CMD.Core;
using UnityEngine;

namespace CMD.Entities
{
    public abstract class EffectSO : ScriptableObject
    {
        public abstract IEffectRuntime CreateRuntime(BaseEntityRuntime host, IGameContext ctx);
    }

    public interface IEffectRuntime
    {
        void Execute();
    }
}
