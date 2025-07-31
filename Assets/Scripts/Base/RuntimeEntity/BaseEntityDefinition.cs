using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base
{
    /// <summary>
    /// ScriptableObject-описание типа сущности.
    /// </summary>
    public abstract class BaseEntityDefinition : ScriptableObject
    {
        [SerializeField, HideInInspector] private string typeId;
        public string TypeId => typeId;

#if UNITY_EDITOR
        /// <summary>
        /// Зачем Guid в рантайме, а строка-slug в definition?
        /// Guid надёжнее для уникальности экземпляров; slug легко читать в сейвах, он никогда не меняется.
        /// </summary>
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(typeId))
            {
                typeId = GUID.Generate().ToString().ToUpper();
                EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}