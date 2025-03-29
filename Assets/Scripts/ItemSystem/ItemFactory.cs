using System.Collections.Generic;
using System.Threading.Tasks;
using ItemSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemFactory
{
    private readonly Dictionary<string, GameObject> _prefabCache = new(); // Кэш префабов

    /// <summary>
    /// Загружает предмет из Addressables и создаёт его в сцене.
    /// </summary>
    public async Task<ItemMono> CreateItemAsync(string itemId, Vector3 position = default, Transform parent = null)
    {
        if (!_prefabCache.TryGetValue(itemId, out GameObject prefab))
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(itemId);
            prefab = await handle.Task;
            
            if (handle.Status != AsyncOperationStatus.Succeeded || prefab == null)
            {
                Debug.LogError($"[ItemFactory] Ошибка загрузки предмета {itemId}: handle.Status={handle.Status}");
                return null;
            }

            _prefabCache[itemId] = prefab; // Кешируем только успешную загрузку
        }
        else
        {
            Debug.Log($"[ItemFactory] Используем закешированный префаб для {itemId}");
        }

        // Создаём объект на сцене
        GameObject itemObject = Object.Instantiate(prefab, position, Quaternion.identity, parent);
        ItemMono newItem = itemObject.GetComponent<ItemMono>();

        if (newItem == null)
        {
            Debug.LogError($"[ItemFactory] Ошибка: У префаба {itemId} отсутствует ItemMono!");
            Object.Destroy(itemObject);
            return null;
        }

        Debug.Log($"[ItemFactory] Создан предмет {itemId}.");
        return newItem;
    }
}
