using UnityEngine;

namespace CMD.Core
{
    public abstract class EffectSO : ScriptableObject
    {
        public abstract IEffectRuntime CreateRuntime(EntityRuntime host, IGameContext ctx);
    }

    public interface IEffectRuntime
    {
        void Execute();
    }
}
