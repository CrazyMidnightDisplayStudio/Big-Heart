using CMD.Core;
using UnityEngine;

namespace CMD.Entities
{
    public abstract class TriggerSO : ScriptableObject
    {
        public abstract ITriggerRuntime CreateRuntime(BaseEntityRuntime host, IGameContext ctx);
    }

    public interface ITriggerRuntime
    {
        event System.Action Fired;
        void Install();
        void Uninstall();
    }
}
