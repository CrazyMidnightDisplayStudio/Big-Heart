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
        private readonly BaseEntityRuntime _host;
        private readonly string _key;
        private readonly float _interval;
        private IEntityContainer _container;
        private bool _installed;
        private Coroutine _co;

        private IContainmentService containmentService;

        public event System.Action Fired;

        public Runtime(BaseEntityRuntime host, string key, float interval)
        { _host = host; _key = key; _interval = interval; }

        public void Install()
        {
            _installed = true;
            // найдём контейнер, в котором сейчас лежит entity
            _container = FindCurrentContainer();
            containmentService = ServiceRegistry.Get<IContainmentService>();
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
            if (containmentService is ContainmentService service)
            {
                var field = typeof(ContainmentService).GetField("_map", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var map = field?.GetValue(service) as System.Collections.IDictionary;
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

    public override ITriggerRuntime CreateRuntime(BaseEntityRuntime host) =>
        new Runtime(host, requiredContainerKey, tickInterval);
}
