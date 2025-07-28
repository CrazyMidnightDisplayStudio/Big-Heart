using Configs;
using ItemSystem;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;

namespace EditorTools
{
    [CustomEditor(typeof(ItemMonoEntity), true)]
    public class ItemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Отображаем стандартный инспектор
            DrawDefaultInspector();

            ItemMonoEntity itemMonoEntity = (ItemMonoEntity)target;
            ItemDefinition itemDefinition = itemMonoEntity.Definition;

            if (itemDefinition == null)
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
            string assetPath = AssetDatabase.GetAssetPath(itemMonoEntity.gameObject);
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
            if (entry.address != itemDefinition.itemTag)
            {
                entry.SetAddress(itemDefinition.itemTag);
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, entry, true);
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.HelpBox($"Addressable Name обновлён: {itemDefinition.itemTag}", MessageType.Info);
        }
    }
}
