using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base
{
    /// <summary> ScriptableObject-дескриптор любого runtime-Entity. </summary>
    public abstract class BaseEntityDefinition : ScriptableObject
    {
        [SerializeField, HideInInspector]          // уникальный TypeId (константа)
        private string typeId;
        public string TypeId => typeId;

        [Header("Runtime prefab (drag-and-drop)")]
        [Tooltip("Prefab, который будет инстанцирован фабрикой")]
        public GameObject prefab;

        /*──────────────────────── Editor-утилиты ───────────────────────*/
#if UNITY_EDITOR
        /* Context-меню */
        [ContextMenu("Generate TypeId / GUID")]
        private void GenerateGuid() => SetTypeId(Guid.NewGuid().ToString("N").ToUpper());

        [ContextMenu("Generate TypeId / From file name")]
        private void GenerateSlug()
        {
            string file = System.IO.Path.GetFileNameWithoutExtension(
                              AssetDatabase.GetAssetPath(this));
            SetTypeId(Slugify(file));
        }

        /* Auto-assign */
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(typeId))
                GenerateGuid();

            if (prefab == null)
            {
                Debug.LogWarning($"{name}: prefab reference is <null>");
            }
            else if (!prefab.scene.IsValid() && prefab.GetComponentInChildren<IView>() == null)
            {
                Debug.LogWarning($"{name}: prefab не содержит IViewMarker (проверь иерархию)");
            }
        }

        /* Helpers */
        private void SetTypeId(string id)
        {
            typeId = id;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        private static string Slugify(string src)
        {
            var sb = new System.Text.StringBuilder();
            foreach (char c in src.ToUpperInvariant())
            {
                if (char.IsLetterOrDigit(c) || c == '_') sb.Append(c);
            }
            return sb.ToString();
        }
    }
#endif
}
