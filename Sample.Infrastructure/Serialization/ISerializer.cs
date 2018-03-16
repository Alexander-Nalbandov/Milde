using System;

namespace Sample.Infrastructure.EventSourcing.Serialization
{
    public interface ISerializer
    {
        T Deserialize<T>(string data) where T : class;
        object Deserialize(Type dataType, string data);
        string DeserializeFromBytes(byte[] data);
        string Serialize<T>(T obj) where T : class;
        byte[] SerializeToBytes(string data);
    }
}