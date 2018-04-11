using System.Text;
using Milde.Remoting.Contracts;
using Newtonsoft.Json;

namespace Milde.Remoting.Serialization
{
    internal class JsonSerializer : ISerializer
    {
        private readonly Encoding _encoding;

        public JsonSerializer() : this(Encoding.UTF8)
        {
        }

        public JsonSerializer(Encoding encoding)
        {
            _encoding = encoding;
        }

        public byte[] Serialize<TMessage>(TMessage message) where TMessage : IRemoteMessage
        {
            return _encoding.GetBytes(JsonConvert.SerializeObject(message, Formatting.None, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            }));
        }

        public TMessage Deserialize<TMessage>(byte[] body) where TMessage : IRemoteMessage
        {
            return JsonConvert.DeserializeObject<TMessage>(Encoding.UTF8.GetString(body), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }
    }
}