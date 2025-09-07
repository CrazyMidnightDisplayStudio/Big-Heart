#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using CMD.Base;
using CMD.Common;
using CMD.Core;
using CMD.GD;
using CMD.Services;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace BigHeart.GD
{
    [Serializable]
    public sealed class SpawnerTab : IToolsTab
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
        public string ownerId = "";

        [ShowIf("@Where == SpawnWhere.Container")]
        [BoxGroup("Container Spawn"), LabelWidth(110)]
        [ValueDropdown(nameof(CommonContainerKeys))]
        public string containerKey = "Backpack";

        [ShowIf("@Where == SpawnWhere.Container")]
        [BoxGroup("Container Spawn"), LabelWidth(110)]
        [MinValue(0)]
        public int index = 0;

        [PropertySpace(12)]
        [Button(ButtonSizes.Large), GUIColor(0.2f, 0.8f, 0.3f)]
        private void Spawn()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Play Mode required",
                    "Запусти игру (Play), чтобы спавнить через runtime-сервисы.", "OK");
                return;
            }
            if (string.IsNullOrEmpty(definitionKey))
            {
                EditorUtility.DisplayDialog("No definition", "Выбери Item Definition.", "OK");
                return;
            }

            // Достаем сервисы (обязательно проверяем на null)
            var catalog = ServiceRegistry.Get<ICatalog>();
            var factory = ServiceRegistry.Get<IEntityFactory<ItemRuntime, ItemDefinition>>();
            if (catalog == null || factory == null)
            {
                EditorUtility.DisplayDialog("Services not ready",
                    "ServiceRegistry не вернул нужные сервисы.", "OK");
                return;
            }

            var def = catalog.Get<BigHeart.ItemDefinition>(definitionKey);
            if (def == null)
            {
                EditorUtility.DisplayDialog("Unknown definition",
                    $"Definition '{definitionKey}' не найден.", "OK");
                return;
            }

            if (Where == SpawnWhere.World)
            {
                var item = factory.Create(def, position, Quaternion.Euler(euler));
                if (item) Selection.activeGameObject = item.gameObject;
            }
            else
            {
                if (string.IsNullOrEmpty(ownerId))
                {
                    EditorUtility.DisplayDialog("Owner required",
                        "Укажи ownerId или нажми 'Pick Selected'.", "OK");
                    return;
                }

                var item = factory.Create(def, Vector3.zero, Quaternion.identity);
                var contain = ServiceRegistry.Get<IContainmentService>();
                if (contain == null || !contain.TryPut(ownerId, containerKey, item, index))
                {
                    if (item) UnityEngine.Object.DestroyImmediate(item.gameObject);
                    EditorUtility.DisplayDialog("Add failed",
                        $"Не удалось положить в {ownerId}/{containerKey}[{index}].", "OK");
                    return;
                }
                Selection.activeGameObject = item.gameObject;
            }
        }

        // ─ helpers (НЕ бросаем исключения, НЕ возвращаем null коллекции) ─

        private IEnumerable<ValueDropdownItem<string>> GetDefinitionKeys()
        {
            var list = new List<ValueDropdownItem<string>>();
            try
            {
                var guids = AssetDatabase.FindAssets("t:ItemDefinition");
                foreach (var g in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(g);
                    var a = AssetDatabase.LoadAssetAtPath<ItemDefinition>(path);
                    if (a != null && !string.IsNullOrEmpty(a.Key))
                        list.Add(new ValueDropdownItem<string>($"{a.Key}   ({a.name})", a.Key));
                }
            }
            catch
            { /* игнорим редакторные ошибки */
            }
            return list.OrderBy(i => i.Text ?? string.Empty);
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
            if (Selection.activeObject is ItemDefinition obj && !string.IsNullOrEmpty(obj.Key))
                definitionKey = obj.Key;
        }

        public enum SpawnWhere { World, Container }

        // ─ IToolsTab ─
        [NonSerialized] private bool _inited;
        public void EnsureInit()
        {
            if (_inited) return;
            _inited = true;
            // никаких вызовов runtime здесь — только дефолты/кэш GUI
        }
        public void SetGroup(string g)
        { /* при желании логируй */
        }
    }
}
#endif
