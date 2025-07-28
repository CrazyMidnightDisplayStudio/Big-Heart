#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using ItemSystem;
using Base;

namespace EditorTools
{
    public static class ItemSpawner
    {
        /// Инстанцирует префаб, инициализирует ItemMono, кладёт в реестр + Undo.
        public static ItemMonoEntity Spawn(ItemDefinition def, Vector3 pos, Transform parent = null)
        {
            if (!def)          throw new System.ArgumentNullException(nameof(def));
            if (!def.prefab)   throw new System.Exception($"{def.name}: prefab reference is null");

            // 1. Instantiate через PrefabUtility, чтобы связь с префабом сохранилась
            var go = (GameObject)PrefabUtility.InstantiatePrefab(def.prefab, parent);
            go.transform.position = pos;
            Undo.RegisterCreatedObjectUndo(go, "Spawn Item");

            // 2. Init + Registry
            var item = go.GetComponent<ItemMonoEntity>() ?? go.AddComponent<ItemMonoEntity>();
            item.Init(def);
            EntityRegistry.Instance.Register(item);

            Selection.activeObject = go;   // фокус на новый объект
            return item;
        }
    }
}
#endif
