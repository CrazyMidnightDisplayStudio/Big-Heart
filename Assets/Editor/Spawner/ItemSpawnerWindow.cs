#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using ItemSystem;
using System.Collections.Generic;

namespace EditorTools
{
    public class ItemSpawnerWindow : EditorWindow
    {
        private string _search = "";
        private Vector2 _scroll;
        private List<ItemDefinition> _definitions;

        [MenuItem("Tools/Item Spawner %#i")] // Ctrl/Cmd+Shift+I
        public static void Open()
        {
            GetWindow<ItemSpawnerWindow>("Item Spawner").Show();
        }

        private void OnEnable()
        {
            RefreshList();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(3);
            EditorGUI.BeginChangeCheck();
            _search = EditorGUILayout.TextField("Search", _search);
            if (EditorGUI.EndChangeCheck()) RefreshList();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (var def in _definitions)
                DrawDefinitionRow(def);
            EditorGUILayout.EndScrollView();
        }

        private void DrawDefinitionRow(ItemDefinition def)
        {
            using (new EditorGUILayout.HorizontalScope("box"))
            {
                GUILayout.Label(def.icon ? def.icon.texture : Texture2D.grayTexture,
                    GUILayout.Width(32), GUILayout.Height(32));
                GUILayout.Label(def.name, GUILayout.Width(180));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Spawn", GUILayout.Width(60)))
                {
                    Vector3 spawnPos = SceneView.lastActiveSceneView
                        ? SceneView.lastActiveSceneView.pivot + Vector3.forward * 2
                        : Vector3.zero;
                    ItemSpawner.Spawn(def, spawnPos);
                }
            }
        }

        private void RefreshList()
        {
            string filter = string.IsNullOrEmpty(_search) ? "" : _search.ToLowerInvariant();
            var guids = AssetDatabase.FindAssets("t:ItemDefinition");
            _definitions = new List<ItemDefinition>();
            foreach (var g in guids)
            {
                var def = AssetDatabase.LoadAssetAtPath<ItemDefinition>(AssetDatabase.GUIDToAssetPath(g));
                if (def && (filter == "" || def.name.ToLowerInvariant().Contains(filter)))
                    _definitions.Add(def);
            }
        }
    }
}
#endif
