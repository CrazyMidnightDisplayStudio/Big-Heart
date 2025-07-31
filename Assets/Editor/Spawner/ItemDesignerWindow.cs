/*
 * ItemDesignerWindow.cs  (rev2)
 * Custom editor window for quickly authoring and testing ItemDefinition assets.
 * Drop this script into an Editor folder.
 */

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using ItemSystem;
using System.IO;

namespace EditorTools
{
    public class ItemDesignerWindow : EditorWindow
    {
        private const string DefaultSaveFolder = "Assets/Data/Item"; // will be created if missing

        [SerializeField] private ItemDefinition _workingCopy; // in‑memory ScriptableObject
        private SerializedObject _so;

        // Generic prefab to use when spawning (change the path if you move the prefab)
        private GameObject _genericPrefab;

        [MenuItem("Tools/Item Designer %&i", priority = 100)] // Ctrl+Alt+I
        public static void Open()
        {
            var wnd = GetWindow<ItemDesignerWindow>();
            wnd.titleContent = new GUIContent("Item Designer");
            wnd.minSize = new Vector2(380, 520);
        }

        private void OnEnable()
        {
            // Load once; keep reference – Addressables users can switch to async call
            _genericPrefab = Resources.Load<GameObject>("Prefabs/GenericItem");
        }

        /*────────────────────────────  GUI  ───────────────────────────*/
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Item Definition", EditorStyles.boldLabel);
            using (new EditorGUILayout.VerticalScope("box"))
            {
                DrawOrCreateWorkingCopy();
            }

            GUILayout.Space(8);

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = _workingCopy != null && !Application.isPlaying; // disable buttons in Play Mode

            if (GUILayout.Button("Spawn in Scene", GUILayout.Height(30)))
            {
                // отложенный вызов, чтобы не ломать GUILayout при исключениях
                EditorApplication.delayCall += Spawn;
            }

            if (GUILayout.Button("Save Asset", GUILayout.Height(30)))
            {
                EditorApplication.delayCall += SaveAsset;
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            if (Application.isPlaying)
                EditorGUILayout.HelpBox("Editor is in Play Mode — spawning and saving отключены.", MessageType.Info);
        }

        /*─────────────────  рабочая копия Definition  ─────────────────*/
        private void DrawOrCreateWorkingCopy()
        {
            if (_workingCopy == null)
            {
                EditorGUILayout.HelpBox("Click the button below to create a new ItemDefinition in memory.",
                    MessageType.Info);
                if (GUILayout.Button("Create New ItemDefinition"))
                {
                    _workingCopy = CreateInstance<ItemDefinition>();
                    _so = new SerializedObject(_workingCopy);
                }

                return;
            }

            _so.Update();
            SerializedProperty prop = _so.GetIterator();
            bool enterChildren = true;
            while (prop.NextVisible(enterChildren))
            {
                if (prop.name == "m_Script") continue; // hide script field
                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
            }

            _so.ApplyModifiedProperties();
        }

        /*───────────────────  действия кнопок  ───────────────────────*/
        private void Spawn()
        {
            if (_workingCopy == null) return;
            if (Application.isPlaying)
            {
                Debug.LogWarning("ItemDesigner: Cannot spawn while the Editor is in Play Mode. Exit play and retry.");
                return;
            }

            var factory = new ItemFactory(_genericPrefab);
            Vector3 pos = SceneView.lastActiveSceneView ? SceneView.lastActiveSceneView.pivot : Vector3.zero;
            factory.Spawn(_workingCopy, pos);

            // Mark scene dirty only in Edit Mode; API запрещён в Play Mode
            if (!Application.isPlaying)
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        private void SaveAsset()
        {
            if (_workingCopy == null) return;
            if (Application.isPlaying)
            {
                Debug.LogWarning("ItemDesigner: Cannot save asset while in Play Mode. Exit play and retry.");
                return;
            }

            EnsureFolderExists(DefaultSaveFolder);

            string fileName = string.IsNullOrWhiteSpace(_workingCopy.displayName)
                ? "NewItemDefinition"
                : MakeFileNameSafe(_workingCopy.displayName);
            string path = Path.Combine(DefaultSaveFolder, fileName + ".asset");
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            AssetDatabase.CreateAsset(_workingCopy, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = _workingCopy;

            _workingCopy = null;
            _so = null;
        }

        /*────────────────────── helpers ──────────────────────────────*/
        private static void EnsureFolderExists(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath)) return;

            string[] parts = folderPath.Split('/');
            string current = parts[0]; // "Assets"
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }

        private static string MakeFileNameSafe(string src)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                src = src.Replace(c, '_');
            return src.ToUpperInvariant();
        }
    }
}
#endif