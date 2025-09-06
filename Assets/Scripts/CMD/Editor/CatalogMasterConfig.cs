using System;
using System.Collections.Generic;
using UnityEngine;

namespace CMD
{
    [CreateAssetMenu(menuName = "CMD/Catalog/Catalog Master Config", fileName = "CatalogMasterConfig")]
    public sealed class CatalogMasterConfig : ScriptableObject
    {
        [Serializable]
        public sealed class Entry
        {
            [Tooltip("Путь ОТНОСИТЕЛЬНО Assets/Resources. Пример: CMD.Catalog/Audio")]
            public string resourcesPath;

            [Tooltip("Полное имя типа (желательно с namespace). Должен наследоваться от UnityEngine.Object.\n" +
                "Примеры: UnityEngine.AudioClip, UnityEngine.GameObject, BigHeart.ItemDefinition")]
            public string typeName;

            [Tooltip("Индексировать подпапки (если нужно).")]
            public bool recursive = false;
        }

        [SerializeField] public List<Entry> entries = new();
    }
}
