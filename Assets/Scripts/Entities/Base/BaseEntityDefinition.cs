#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Entities.Base
{
    public abstract class BaseEntityDefinition : ScriptableObject
    {
        [SerializeField, HideInInspector] private string assetId;
        public string AssetId => assetId;

    #if UNITY_EDITOR
        private void OnValidate()
        {
            var path = AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(path))
            {
                var guid = AssetDatabase.AssetPathToGUID(path);
                if (assetId != guid)
                {
                    assetId = guid;
                    EditorUtility.SetDirty(this);
                }
            }
        }
    #endif
    }
}
