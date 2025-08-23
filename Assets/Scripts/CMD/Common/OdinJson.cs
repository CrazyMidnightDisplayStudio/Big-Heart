using Sirenix.Serialization;
using System.Text;

namespace CMD.Common
{
    /// <summary>Обёртка над Odin Serializer для строкового JSON.</summary>
    public static class OdinJson
    {
        public static string ToJson<T>(T value) =>
            Encoding.UTF8.GetString(SerializationUtility.SerializeValue(value, DataFormat.JSON));

        public static T FromJson<T>(string json) =>
            SerializationUtility.DeserializeValue<T>(Encoding.UTF8.GetBytes(json), DataFormat.JSON);
    }
}
