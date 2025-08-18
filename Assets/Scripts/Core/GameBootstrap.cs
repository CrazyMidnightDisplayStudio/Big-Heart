using UnityEngine;

namespace Core
{
    [DefaultExecutionOrder(-1000)] // запускаемся раньше большинства скриптов
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Fallback prefab")] [SerializeField]
        private GameObject genericItemPrefab; // drag-and-drop из Project

        private static bool _bootstrapped; // статика == глобальный флаг

        /*──────────────────────────── Bootstrap ───────────────────────────*/
        private void Awake()
        {
            /* ─── Анти-дублирование ─── */
            if (_bootstrapped)
            {
                Debug.LogWarning(
                    $"<color=yellow>Duplicate GameBootstrap on {name} — destroying</color>");
                Destroy(gameObject);
                return;
            }

            _bootstrapped = true;
            DontDestroyOnLoad(gameObject); // живём между сценами

            /* ─── Сборка зависимостей ─── */
            // var coroutineService = new CoroutineService();
            // ServiceRegistry.Register<ICoroutineService>(coroutineService);
            //
            // var eventService = new EventService();
            // ServiceRegistry.Register<IEventBusService>(eventService);
            //
            // var itemFactory = new ItemFactory(eventService, coroutineService);
            // var itemService = new DebugItemSpawnerService(itemFactory);
            // ServiceRegistry.Register<IItemService>(itemService);
            //
            // var inventoryService = new InventoryService();
            // ServiceRegistry.Register<IInventoryService>(inventoryService);
            //
            // var dateProgressService = new DateProgressService(eventService);
            // ServiceRegistry.Register<IDateProgressService>(dateProgressService);

            Debug.Log("<color=green>GameBootstrap: All services registered</color>");
        }

        /*──────────────────── Проверка «забыли-ли мы Bootstrap» ─────────────*/
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
