using CMD.Core;
using CMD.Entities;
using CMD.Services;
using UnityEngine;

[CreateAssetMenu(menuName="BigHeart/Triggers/While In Container")]
public sealed class WhileInContainerTriggerSO : TriggerSO
{
    public string requiredContainerKey = "Backpack";
    public float tickInterval = 1f; // 0 => одноразовый при добавлении

    private sealed class Runtime : ITriggerRuntime
    {
        readonly BaseEntityRuntime _host;
        readonly IGameContext _ctx;
        readonly string _key;
        readonly float _interval;
        IEntityContainer _container;
        bool _installed;
        Coroutine _co;

        public event System.Action Fired;

        public Runtime(BaseEntityRuntime host, IGameContext ctx, string key, float interval)
        { _host = host; _ctx = ctx; _key = key; _interval = interval; }

        public void Install()
        {
            _installed = true;
            // найдём контейнер, в котором сейчас лежит entity
            _container = FindCurrentContainer();
            if (_container == null) return;

            if (_interval <= 0f) Fired?.Invoke();
            else _co = _host.StartCoroutine(Co());
        }

        public void Uninstall()
        {
            _installed = false;
            if (_co != null) _host.StopCoroutine(_co);
            _co = null;
        }

        System.Collections.IEnumerator Co()
        {
            var w = new WaitForSeconds(_interval);
            while (_installed && _container != null && _container.Contains(_host))
            {
                Fired?.Invoke();
                yield return w;
            }
        }

        IEntityContainer FindCurrentContainer()
        {
            // Простой путь: перебрать все зарегистрированные и поискать host.
            // Для эффективности можно держать в IContainmentService быстрый индекс entityId -> (owner, container, index)
            // Ниже — простая версия:
            if (_ctx.ContainmentService is ContainmentService svc)
            {
                var field = typeof(ContainmentService).GetField("_map", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var map = field?.GetValue(svc) as System.Collections.IDictionary;
                if (map != null)
                {
                    foreach (System.Collections.DictionaryEntry de in map)
                    {
                        var c = de.Value as IEntityContainer;
                        if (c != null && c.ContainerKey == _key && c.Contains(_host))
                            return c;
                    }
                }
            }
            return null;
        }
    }

    public override ITriggerRuntime CreateRuntime(BaseEntityRuntime host, IGameContext ctx) =>
        new Runtime(host, ctx, requiredContainerKey, tickInterval);
}
