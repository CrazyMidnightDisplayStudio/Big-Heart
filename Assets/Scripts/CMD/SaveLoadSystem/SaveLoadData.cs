using System;
using CMD.Common;

namespace CMD.SaveLoadSystem
{
    /// <summary>Один «чанк» сохранения.</summary>
    [Serializable]
    public sealed class SaveLoadData
    {
        public string Id; // ComponentSaveId
        public int Version = 1; // для миграций
        public ESaveType Type; // Entity/Component/Custom
        public string PayloadJson; // строковый JSON (Odin)

        public static SaveLoadData Create<T>(string id, ESaveType type, T payload, int version = 1)
            => new SaveLoadData
            {
                Id = id,
                Type = type,
                Version = version,
                PayloadJson = OdinJson.ToJson(payload)
            };

        public T Read<T>() => OdinJson.FromJson<T>(PayloadJson);
    }
}
