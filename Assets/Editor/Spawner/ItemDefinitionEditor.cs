#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using ItemSystem;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace EditorTools
{
    [CustomEditor(typeof(ItemDefinition))]
    public class ItemDefinitionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Spawn in Scene"))
            {
                var def = (ItemDefinition)target;

                // Обычно держу один GenericItem prefab в Resources или Addressable
                var genericPrefab = Resources.Load<GameObject>("Prefabs/GenericItem");
                var factory = new ItemFactory(genericPrefab);

                var scenePos = SceneView.lastActiveSceneView.pivot;
                factory.Spawn(def, scenePos);

                // Чтобы сцена считалась изменённой
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }
    }
}
#endif