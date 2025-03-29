using Configs;
using ItemSystem;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;

namespace Editor
{
    [CustomEditor(typeof(ItemMono), true)]
    public class ItemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Отображаем стандартный инспектор
            DrawDefaultInspector();

            ItemMono itemMono = (ItemMono)target;
            ItemConfig itemConfig = itemMono.Config;

            if (itemConfig == null)
            {
                EditorGUILayout.HelpBox("У предмета не назначен ItemConfig!", MessageType.Warning);
                return;
            }

            // Получаем Addressables Settings
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                EditorGUILayout.HelpBox("Addressables не настроены в проекте!", MessageType.Warning);
                return;
            }

            // Находим или создаем группу "Items"
            AddressableAssetGroup group = settings.FindGroup("Items");
            if (group == null)
            {
                group = settings.CreateGroup("Items", false, false, true, settings.DefaultGroup.Schemas);
            }

            // Получаем путь к префабу предмета
            string assetPath = AssetDatabase.GetAssetPath(itemMono.gameObject);
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            // Проверяем, есть ли этот предмет в Addressables
            AddressableAssetEntry entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(assetPath));
            if (entry == null)
            {
                // Если предмета нет в Addressables, добавляем его
                entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(assetPath), group, false, false);
            }

            // Устанавливаем Addressable Name = itemId
            if (entry.address != itemConfig.itemId)
            {
                entry.SetAddress(itemConfig.itemId);
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, entry, true);
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.HelpBox($"Addressable Name обновлён: {itemConfig.itemId}", MessageType.Info);
        }
    }
}
