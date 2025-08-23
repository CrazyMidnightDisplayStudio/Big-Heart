using System;
using UnityEngine;

namespace CMD.Base
{
    [Serializable]
    public class EntityRuntimeState : IEntityState
    {
        [SerializeField] private int version;
        public int Version => version;

        /// <summary>
        /// Все изменения класса должны так же тригерить Changed, чтобы про
        /// </summary>
        [field: NonSerialized] public event Action Changed;
    }
}
