using System;
using Entities.Base;

namespace Entities.Item.Model
{
    [Serializable]
    public struct ItemState : IEntityState
    {
        /// <summary>
        /// количество раундов в котором участвовал предмет
        /// может понадобится для аналитики, либо для геймплейных механик
        /// </summary>
        public int roundsCompleted;
    }
}
