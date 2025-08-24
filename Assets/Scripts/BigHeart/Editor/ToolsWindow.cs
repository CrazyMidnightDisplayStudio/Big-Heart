#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using BigHeart;
using BigHeart.Services;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using CMD.Services;
using CMD.Base;
using CMD.Common;
using CMD.Core;

public sealed class ToolsWindow : OdinEditorWindow
{
    [MenuItem("BigHeart/Tools %#g")] // Cmd/Ctrl+Shift+G
    public static void Open() => GetWindow<ToolsWindow>("BigHeart Tools");

    [TabGroup("Spawner")] public SpawnerTab Spawner = new();
    [TabGroup("Save / Load")] public SaveLoadTab SaveLoad = new();

    // ───────────────────────────────── Spawner ─────────────────────────────────
    public sealed class SpawnerTab
    {
        [PropertySpace]
        [LabelText("Item Definition")]
        [ValueDropdown(nameof(GetDefinitionKeys), DropdownWidth = 320)]
        [InlineButton(nameof(PickSelectedDefinition), "Pick Selected")]
        public string definitionKey;

        [PropertySpace]
        [EnumToggleButtons, LabelWidth(80)]
        public SpawnWhere Where = SpawnWhere.World;

        [ShowIf("@Where == SpawnWhere.World")]
        [BoxGroup("World Spawn"), LabelWidth(90)]
        public Vector3 position = Vector3.zero;

        [ShowIf("@Where == SpawnWhere.World")]
        [BoxGroup("World Spawn"), LabelWidth(90)]
        public Vector3 euler = Vector3.zero;

        [ShowIf("@Where == SpawnWhere.Container")]
        [BoxGroup("Container Spawn"), LabelWidth(110)]
        [InlineButton(nameof(PickOwnerFromSelection), "Pick Selected")]
        public string ownerId = ""; // StableId владельца (например, игрока)

        [ShowIf("@Where == SpawnWhere.Container")]
        [BoxGroup("Container Spawn"), LabelWidth(110)]
        [ValueDropdown(nameof(CommonContainerKeys))]
        public string containerKey = "Backpack";

        [ShowIf("@Where == SpawnWhere.Container")]
        [BoxGroup("Container Spawn"), LabelWidth(110)]
        public int index = 0;

        [PropertySpace(12)]
        [Button(ButtonSizes.Large), GUIColor(0.2f, 0.8f, 0.3f)]
        private void Spawn()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Play Mode required",
                    "Запусти игру (Play), чтобы спавнить через runtime-сервисы и корректно присвоить StableId.",
                    "OK");
                return;
            }

            if (string.IsNullOrEmpty(definitionKey))
            {
                EditorUtility.DisplayDialog("No definition", "Выбери Item Definition.", "OK");
                return;
            }

            // достаём сервисы
            var catalog = ServiceRegistry.Get<ICatalog<ItemDefinition>>();
            var factory = ServiceRegistry.Get<IEntityFactory<ItemRuntime, ItemDefinition>>();

            var def = catalog.GetByKey(definitionKey);
            if (def == null)
            {
                EditorUtility.DisplayDialog("Unknown definition",
                    $"Definition '{definitionKey}' не найден в каталоге.", "OK");
                return;
            }

            if (Where == SpawnWhere.World)
            {
                var item = factory.Create(def, position, Quaternion.Euler(euler));
                Selection.activeGameObject = item.gameObject;
                return;
            }
            else
            {
                if (string.IsNullOrEmpty(ownerId))
                {
                    EditorUtility.DisplayDialog("Owner required",
                        "Укажи ownerId (StableId) владельца контейнера или нажми 'Pick Selected'.",
                        "OK");
                    return;
                }

                var item = factory.Create(def, Vector3.zero, Quaternion.identity);
                var contain = ServiceRegistry.Get<IContainmentService>();
                if (!contain.TryPut(ownerId, containerKey, item, index))
                {
                    Object.DestroyImmediate(item.gameObject);
                    EditorUtility.DisplayDialog("Add failed",
                        $"Не удалось положить предмет в {ownerId}/{containerKey}[{index}] (слот занят или контейнер недоступен).",
                        "OK");
                    return;
                }

                Selection.activeGameObject = item.gameObject;
            }
        }

        // — helpers —

        private IEnumerable<ValueDropdownItem<string>> GetDefinitionKeys()
        {
            // в редакторе достанем ключи из ассетов (работает и в Play, и вне Play)
            var guids = AssetDatabase.FindAssets("t:ItemDefinition");
            return guids.Select(g => AssetDatabase.GUIDToAssetPath(g))
                .Select(p => AssetDatabase.LoadAssetAtPath<ItemDefinition>(p))
                .Where(a => a != null && !string.IsNullOrEmpty(a.Key))
                .OrderBy(a => a.Key)
                .Select(a => new ValueDropdownItem<string>($"{a.Key}   ({a.name})", a.Key));
        }

        private IEnumerable<string> CommonContainerKeys()
            => new[]
            {
                "Backpack",
                "Equipment.Head",
                "Equipment.Body",
                "Chest#1"
            };

        private void PickOwnerFromSelection()
        {
            var go = Selection.activeGameObject;
            if (!go) return;
            var stable = go.GetComponent<StableId>();
            if (stable) ownerId = stable.IdString;
        }

        private void PickSelectedDefinition()
        {
            var obj = Selection.activeObject as ItemDefinition;
            if (obj != null && !string.IsNullOrEmpty(obj.Key))
                definitionKey = obj.Key;
        }

        public enum SpawnWhere { World, Container }
    }

    // ───────────────────────────────── Save / Load ─────────────────────────────────
    public sealed class SaveLoadTab
    {
        [InfoBox("Сохранение/загрузка работают только в Play Mode, так как зависят от ServiceRegistry.", InfoMessageType.None)]
        [ReadOnly, LabelText("persistentDataPath")]
        public string p = Application.persistentDataPath;

        [PropertySpace(6)]
        [Button("Save (F5)"), GUIColor(0.2f, 0.7f, 1f)]
        private void Save()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Play Mode required", "Запусти игру.", "OK");
                return;
            }
            ServiceRegistry.Get<ISaveLoadService>().SaveGame();
        }

        [Button("Load (F9)"), GUIColor(1f, 0.5f, 0.2f)]
        private void Load()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Play Mode required", "Запусти игру.", "OK");
                return;
            }
            ServiceRegistry.Get<ISaveLoadService>().LoadGameFresh();
        }
    }
}
#endif
