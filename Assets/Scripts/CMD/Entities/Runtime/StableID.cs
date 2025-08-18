using System;
using UnityEngine;

namespace CMD.Entities
{
    [DisallowMultipleComponent]
    public sealed class StableId : MonoBehaviour
    {
        [SerializeField] private string id;
        [SerializeField] private bool generateOnAwakeForRuntimeInstances = true;

        public string IdString => id;

        public Guid Id
        {
            get
            {
                if (!Guid.TryParse(id, out var guid))
                {
                    guid = Guid.NewGuid();
                    id = guid.ToString("N");
                }
                return guid;
            }
        }

        public void SetFromSave(Guid saved)
        {
            id = saved.ToString("N");
        }

        private void Awake()
        {
            if (Application.isPlaying && generateOnAwakeForRuntimeInstances && string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString("N");
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out _))
            {
                id = Guid.NewGuid().ToString("N");
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}
