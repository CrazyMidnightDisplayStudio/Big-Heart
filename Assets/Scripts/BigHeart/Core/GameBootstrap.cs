using System.IO;
using BigHeart.Services;
using UnityEngine;
using CMD.Core;
using CMD.Services;
using CMD.Entities;

namespace BigHeart
{
    [DefaultExecutionOrder(-1000)]
    public sealed class GameBootstrap : MonoBehaviour
    {
        private static bool _bootstrapped;

        [SerializeField] private ScriptableObject catalogAsset; // твой ItemCatalogSO

        private void Awake()
        {
            if (_bootstrapped)
            {
                Destroy(gameObject);
                return;
            }
            _bootstrapped = true;
            DontDestroyOnLoad(gameObject);

            // core services
            var saves = new JsonSaveRepository(Path.Combine(Application.persistentDataPath, "save.json"));
            var eventsBus = new StubEventService();
            var coroutines = new StubCoroutineService();
            var containment = new ContainmentService("Containment");
            var catalog = (IEntityCatalog)catalogAsset;
            var factory = new PrefabEntityFactory();

            // register
            ServiceRegistry.Register<IEntityCatalog>(catalog);
            ServiceRegistry.Register<IEntityFactory>(factory);
            ServiceRegistry.Register<ISaveRepository>(saves);
            ServiceRegistry.Register<IEventBusService>(eventsBus);
            ServiceRegistry.Register<ICoroutineService>(coroutines);
            ServiceRegistry.Register<IContainmentService>(containment);

            // глобальное состояние игры — твоё
            ServiceRegistry.Register(new GameContextService());

            Debug.Log("<color=green>Bootstrap: services registered</color>");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void _CheckBootstrapPresence()
        {
            if (_bootstrapped) return;
            Debug.LogWarning(
                "<color=yellow>GameBootstrap not found in first scene! " +
                "Core services were NOT initialised.</color>");
        }
    }
}
