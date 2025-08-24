using System;
using UnityEngine;

namespace CMD.SaveLoadSystem
{
    [Serializable]
    public struct WorldData
    {
        public string scene; // опционально
        public Vector3 position;
        public Quaternion rotation;

        public WorldData(string scene, Vector3 position, Quaternion rotation)
        {
            this.scene = scene;
            this.position = position;
            this.rotation = rotation;
        }
    }
}
