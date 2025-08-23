using System.IO;
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

            // register
            ServiceRegistry.Register<ItemCatalogService>(new ItemCatalogService());
            ServiceRegistry.Register<IEntityFactory<ItemRuntime, ItemDefinition>>(new ItemFactory());
            ServiceRegistry.Register<ISaveRepository>(saves);
            ServiceRegistry.Register<IEventBusService>(new EventBusService());
            ServiceRegistry.Register<ICoroutineService>(new CoroutineService());
            // ServiceRegistry.Register<IContainmentService>(new InventoryContainer());
            ServiceRegistry.Register(new BrainService());

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
