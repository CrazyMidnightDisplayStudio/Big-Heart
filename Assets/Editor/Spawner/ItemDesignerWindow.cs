#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditorInternal;
using System.IO;
using System.Collections.Generic;
using ItemSystem;
using ItemSystem.Effects;

namespace EditorTools
{
    /// <summary>
    ///   Rev‑4: warns only on Spawn, fixes null‑effects, cleaner UX.
    /// </summary>
    public class ItemDesignerWindow : EditorWindow
    {
        private const string DefaultFolder = "Assets/Data/Items";
        private const string GenericPrefabPath = "Prefabs/GenericItem"; // Resources path
        private const string EffectFilter = "t:BaseEffectAsset";

        [SerializeField] private ItemDefinition _draft;
        private SerializedObject _so;
        private ReorderableList _effectsList;
        private Vector2 _scroll;
        private GameObject _genericPrefab;
        

        #region ✦ Entry
        [MenuItem("Tools/Item Designer %#i", priority = 100)] // Ctrl+Shift+I
        private static void Open()
        {
            var wnd = GetWindow<ItemDesignerWindow>();
            wnd.titleContent = new GUIContent("Item Designer");
            wnd.minSize = new Vector2(420, 580);
        }

        private void OnEnable()
        {
            _genericPrefab = Resources.Load<GameObject>(GenericPrefabPath);
        }
        #endregion

        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            DrawDefinitionBlock();
            GUILayout.Space(10);
            DrawToolbar();
            EditorGUILayout.EndScrollView();
        }

        /*──────────────────── Item Definition block ───────────────────*/
        private void DrawDefinitionBlock()
        {
            EditorGUILayout.LabelField("Item Definition", EditorStyles.boldLabel);
            using (new EditorGUILayout.VerticalScope("box"))
            {
                if (_draft == null)
                {
                    if (GUILayout.Button("Create New Item"))
                    {
                        _draft = CreateInstance<ItemDefinition>();
                        _so = new SerializedObject(_draft);
                        SetupEffectsList();
                    }
                    return;
                }

                _so.Update();

                DrawProperty("displayName", "Display Name *");
                DrawProperty("description");
                DrawProperty("icon");
                DrawProperty("itemTag", "Item Tag");
                DrawProperty("slotType", "Slot Type");
                DrawEffectsReorderable();
                DrawProperty("overridePrefab");

                _so.ApplyModifiedProperties();
            }
        }

        private void DrawProperty(string prop, string label = null)
        {
            var sp = _so.FindProperty(prop);
            if (sp != null)
                EditorGUILayout.PropertyField(sp, new GUIContent(label ?? ObjectNames.NicifyVariableName(prop)), true);
        }

        /*──────────────────── Effects list ───────────────────*/
        private void SetupEffectsList()
        {
            var listProp = _so.FindProperty("effects");
            _effectsList = new ReorderableList(_so, listProp, true, true, true, true);

            _effectsList.drawHeaderCallback = rect => GUI.Label(rect, "Effects");
            _effectsList.drawElementCallback = (rect, index, _, _) =>
            {
                var element = listProp.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none);
            };
            _effectsList.onAddDropdownCallback = (rect, list) => ShowEffectDropdown(listProp);
        }

        private void ShowEffectDropdown(SerializedProperty listProp)
        {
            var menu = new GenericMenu();
            foreach (var ea in FindAllEffectAssets())
            {
                menu.AddItem(new GUIContent(ea.name), false, obj =>
                {
                    listProp.arraySize++;
                    var el = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);
                    el.objectReferenceValue = obj as BaseEffectAsset;
                    _so.ApplyModifiedProperties();
                }, ea);
            }
            menu.ShowAsContext();
        }

        private static IEnumerable<BaseEffectAsset> FindAllEffectAssets()
        {
            var guids = AssetDatabase.FindAssets(EffectFilter);
            foreach (var g in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                var ea = AssetDatabase.LoadAssetAtPath<BaseEffectAsset>(path);
                if (ea != null) yield return ea;
            }
        }

        private void DrawEffectsReorderable()
        {
            _effectsList ??= SetupAndReturn();
            _effectsList.DoLayoutList();

            ReorderableList SetupAndReturn()
            {
                SetupEffectsList();
                return _effectsList;
            }
        }

        /*──────────────────── Toolbar ───────────────────*/
        private void DrawToolbar()
        {
            if (_draft == null) return;
            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.enabled = !Application.isPlaying; // disable in Play Mode

                if (GUILayout.Button("Spawn", GUILayout.Height(28)))
                    EditorApplication.delayCall += Spawn;

                if (GUILayout.Button("Save Asset", GUILayout.Height(28)))
                    EditorApplication.delayCall += SaveAsset;

                if (GUILayout.Button("Duplicate", GUILayout.Height(28)))
                    DuplicateDraft();

                GUI.enabled = true;
            }

            if (Application.isPlaying)
                EditorGUILayout.HelpBox("Play Mode: window is read‑only", MessageType.Info);
        }

        /*──────────────────── Actions ───────────────────*/
        private void Spawn()
        {
            if (!Validate(out var msg))
            {
                EditorUtility.DisplayDialog("Item Designer – cannot spawn", msg, "OK");
                return;
            }

            var factory = new ItemFactory(_genericPrefab);
            Vector3 pos = SceneView.lastActiveSceneView ? SceneView.lastActiveSceneView.pivot : Vector3.zero;
            factory.Spawn(_draft, pos);

            if (!Application.isPlaying)
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        private void SaveAsset()
        {
            if (_draft == null) return;
            EnsureFolderExists(DefaultFolder);

            string fileName = MakeFileNameSafe(string.IsNullOrWhiteSpace(_draft.displayName)
                ? "NewItemDefinition" : _draft.displayName);
            string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(DefaultFolder, fileName + ".asset"));

            AssetDatabase.CreateAsset(_draft, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = _draft;
            _draft = null;
            _so = null;
            _effectsList = null;
        }

        private void DuplicateDraft()
        {
            var clone = Instantiate(_draft);
            clone.name = _draft.name + "_Copy";
            _draft = clone;
            _so = new SerializedObject(_draft);
            SetupEffectsList();
        }

        /*──────────────────── Validation ───────────────────*/
        private bool Validate(out string message)
        {
            var sb = new System.Text.StringBuilder();
            if (string.IsNullOrWhiteSpace(_draft.displayName))
                sb.AppendLine("• Display Name is empty");
            if (_draft.icon == null)
                sb.AppendLine("• Icon not set");
            if (_draft.effects == null || _draft.effects.Count == 0)
                sb.AppendLine("• Add at least one effect");

            message = sb.ToString();
            return message.Length == 0;
        }

        /*──────────────────── Helpers ───────────────────*/
        private static void EnsureFolderExists(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder)) return;
            var parts = folder.Split('/');
            string curr = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = curr + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(curr, parts[i]);
                curr = next;
            }
        }

        private static string MakeFileNameSafe(string src)
        {
            foreach (var c in Path.GetInvalidFileNameChars()) src = src.Replace(c, '_');
            return src.ToUpperInvariant();
        }
    }
}
#endif
