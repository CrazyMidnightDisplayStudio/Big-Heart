#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using ItemSystem;

namespace EditorTools
{
    public static class ItemDefinitionContext
    {
        [MenuItem("Assets/Spawn in Scene", true)]
        private static bool Validate() =>
            Selection.activeObject is ItemDefinition;

        [MenuItem("Assets/Spawn in Scene")]
        private static void SpawnFromContext()
        {
            var def = Selection.activeObject as ItemDefinition;
            if (!def) return;

            Vector3 pos = SceneView.lastActiveSceneView
                ? SceneView.lastActiveSceneView.pivot + Vector3.forward * 2
                : Vector3.zero;
            ItemSpawner.Spawn(def, pos);
        }
    }
}
#endif
