using UnityEngine;

namespace BigHeart
{
    /// <summary>Умеет отдавать префаб для спауна runtime-компонента.</summary>
    public interface IPrefabProvider
    {
        GameObject Prefab { get; }
    }
}
