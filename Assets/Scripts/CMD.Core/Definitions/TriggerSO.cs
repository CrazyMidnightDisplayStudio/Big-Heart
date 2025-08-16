using UnityEngine;

namespace CMD.Core
{
    public abstract class TriggerSO : ScriptableObject
    {
        public abstract ITriggerRuntime CreateRuntime(EntityRuntime host, IGameContext ctx);
    }

    public interface ITriggerRuntime
    {
        event System.Action Fired;
        void Install();
        void Uninstall();
    }
}
