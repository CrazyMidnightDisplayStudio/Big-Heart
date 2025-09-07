using BigHeart.Services;
using UnityEngine;
using CMD.Core;
using CMD.Services;
using CMD.Base;

namespace BigHeart
{
    [DefaultExecutionOrder(-1000)]
    public sealed class GameBootstrap : MonoBehaviour
    {
        private static bool _bootstrapped;

        private void Awake()
        {
            if (_bootstrapped) { Destroy(gameObject); return; }
            _bootstrapped = true;
            DontDestroyOnLoad(gameObject);

            // ───── EventBus ─────
            ServiceRegistry.Register<IEventBusService>(new EventBusService());

            // ───── Containment ─────
            ServiceRegistry.Register<IContainmentService>(new ContainmentService());

            // ───── Catalog ─────
            var catalog = new CatalogService();
            ServiceRegistry.Register<ICatalog>(catalog);
            // Автопоиск всех ICatalogRegistrar (включая сгенерированные)
            CatalogAutoRegistrar.Run(catalog);

            // ───── Фабрики ─────
            ServiceRegistry.Register<IEntityFactory<ItemRuntime, ItemDefinition>>(new ItemFactory());

            // ───── Save/Load ─────
            var strategy = new OdinFileStrategy(fileName: "BigHeart.json", subFolder: "Saves");
            ServiceRegistry.Register<ISaveLoadStrategy>(strategy);
            ServiceRegistry.Register<ISaveLoadService>(new SaveLoadService(strategy));

            // ───── Короутины/брейн (если нужно) ─────
            ServiceRegistry.Register<ICoroutineService>(new CoroutineService());
            ServiceRegistry.Register<BrainService>(new BrainService());

            Debug.Log("<color=green>Bootstrap: services registered</color>");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void _CheckBootstrapPresence()
        {
            if (_bootstrapped) return;
            Debug.LogWarning("<color=yellow>GameBootstrap not found in first scene! Core services were NOT initialised.</color>");
        }
    }
}
