#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using ItemSystem;
using ItemSystem.Effects;
using System.IO;

namespace EditorTools
{
    public class ItemCreatorWindow : EditorWindow
    {
        /* ───── UI-состояние ───── */
        ItemDefinition _def; // текущий ассет
        ReorderableList _fxList; // красиво рисуем effects
        Vector2 _scroll;

        /* ───── меню ───── */
        [MenuItem("Tools/Item Creator %#n")] // Ctrl/Cmd+Shift+N
        static void Open() => GetWindow<ItemCreatorWindow>("Item Creator");

        /* ───── инициализация ───── */
        void OnEnable()
        {
            CreateTempDefinitionIfNeeded();
            BuildFxList();
        }

        /* ───── GUI ───── */
        void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            /* блок Definition-поля */
            EditorGUI.BeginChangeCheck();
            _def.itemTag = EditorGUILayout.TextField("Tag", _def.itemTag);
            _def.displayName = EditorGUILayout.TextField("Display name", _def.displayName);
            _def.slotType = (SlotType)EditorGUILayout.EnumPopup("Slot type", _def.slotType);

            EditorGUILayout.LabelField("Description");
            _def.description = EditorGUILayout.TextArea(_def.description, GUILayout.Height(60));

            _def.icon = (Sprite)EditorGUILayout.ObjectField("Icon", _def.icon, typeof(Sprite), false);
            _def.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", _def.prefab, typeof(GameObject), false);

            /* effects — ReorderableList */
            EditorGUILayout.Space(4);
            _fxList.DoLayoutList();

            /* если были изменения — помечаем ассет dirty */
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(_def);

            EditorGUILayout.Space(8);
            GUI.enabled = _def.prefab; // нельзя спавнить без prefab
            if (GUILayout.Button("Spawn @ (0,0,0)", GUILayout.Height(30)))
                SpawnNow();

            GUI.enabled = true;
            EditorGUILayout.EndScrollView();
        }

        /* ───── helpers ───── */

        void SpawnNow()
        {
            /* гарантируем, что ассет сохранён */
            AssetDatabase.SaveAssets();

            var item = EditorTools.ItemSpawner.Spawn(_def, Vector3.zero);
            Selection.activeObject = item.gameObject;
        }

        void CreateTempDefinitionIfNeeded()
        {
            if (_def) return;

            string dir = "Assets/Items";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            _def = ScriptableObject.CreateInstance<ItemDefinition>();
            _def.name = "TempItemDefinition";
            AssetDatabase.CreateAsset(_def, AssetDatabase.GenerateUniqueAssetPath($"{dir}/{_def.name}.asset"));
            AssetDatabase.SaveAssets();
        }

        void BuildFxList()
        {
            _fxList = new ReorderableList(_def.effects, typeof(EffectAsset), true, true, true, true);
            _fxList.drawHeaderCallback = rect => GUI.Label(rect, "Effects");
            _fxList.drawElementCallback = (rect, index, _, __) =>
            {
                _def.effects[index] =
                    (EffectAsset)EditorGUI.ObjectField(
                        rect, _def.effects[index], typeof(EffectAsset), false);
            };
        }
    }
}
#endif
