#if UNITY_EDITOR
using ItemSystem;
using UnityEditor;
using UnityEngine;

namespace EditorTools
{
    [CustomEditor(typeof(ItemPresenter))]
    [CanEditMultipleObjects]
    public class ItemPresenterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var p = (ItemSystem.ItemPresenter)target;

            serializedObject.Update();

            // поле из ItemPresenter: [SerializeField] private ItemDefinition _definitionAsset;
            var defProp = serializedObject.FindProperty("_definitionAsset");

            EditorGUI.BeginChangeCheck();
            var picked = (ItemSystem.ItemDefinition)EditorGUILayout.ObjectField(
                new GUIContent("Definition Asset"),
                defProp.objectReferenceValue, // текущее значение (может быть null)
                typeof(ItemSystem.ItemDefinition),
                false); // не позволяем scene-объекты

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(p, "Assign Definition");
                defProp.objectReferenceValue = picked;
                serializedObject.ApplyModifiedProperties();

                if (picked != null)
                {
                    p.AssignDefinition(picked, cloneForRuntime: Application.isPlaying);
                }
            }

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField(
                    new GUIContent("Runtime Copy (Play Only)"),
                    p.CurrentDefinition, typeof(ItemSystem.ItemDefinition), false);
            }

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Rebuild From Asset"))
                {
                    var asset = (ItemSystem.ItemDefinition)(defProp.objectReferenceValue ?? p.CurrentDefinition);
                    if (asset != null)
                    {
                        p.AssignDefinition(asset, cloneForRuntime: true);
                    }
                }
            }
        }
    }
}
#endif
