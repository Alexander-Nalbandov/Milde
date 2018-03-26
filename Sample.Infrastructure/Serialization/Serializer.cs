using System;
using System.Text;
using Newtonsoft.Json;

namespace Sample.Infrastructure.EventSourcing.Serialization
{
    public class Serializer : ISerializer
    {
        public string Serialize<T>(T obj) where T : class
        {
            return JsonConvert.SerializeObject(obj);
        }

        public byte[] SerializeToBytes(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        public T Deserialize<T>(string data) where T : class
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(data);
        }

        public object Deserialize(Type dataType, string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            return JsonConvert.DeserializeObject(data, dataType);
        }

        public string DeserializeFromBytes(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
