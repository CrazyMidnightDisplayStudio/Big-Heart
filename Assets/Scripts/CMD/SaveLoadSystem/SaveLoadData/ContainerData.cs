using System;

namespace CMD.SaveLoadSystem
{
    [Serializable]
    public struct ContainerData
    {
        public string ownerId; // StableId владельца контейнера (например, игрока)
        public string containerKey; // стабильный ключ контейнера: "Backpack", "Equipment.Head", "Chest#1"
        public int index; // индекс ячейки в контейнере

        public ContainerData(string ownerId, string containerKey, int index)
        {
            this.ownerId = ownerId;
            this.containerKey = containerKey;
            this.index = index;
        }
    }
}
