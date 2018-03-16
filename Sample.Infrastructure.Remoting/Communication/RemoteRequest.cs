using Newtonsoft.Json;
using Sample.Infrastructure.Remoting.Contracts;

namespace Sample.Infrastructure.Remoting.Communication
{
    internal class RemoteRequest : IRemoteMessage
    {
        public RemoteRequest(string methodName, object[] args)
        {
            MethodName = methodName;
            Args = args;
            Headers = new MessageHeaders();
        }

        [JsonConstructor]
        public RemoteRequest(string methodName, object[] args, MessageHeaders headers)
        {
            MethodName = methodName;
            Args = args;
            Headers = headers;
        }

        public string MethodName { get; }
        public object[] Args { get; }
        public MessageHeaders Headers { get; }
    }
}