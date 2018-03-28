using Newtonsoft.Json;
using Sample.Infrastructure.Remoting.Contracts;

namespace Sample.Infrastructure.Remoting.Communication
{
    internal class RemoteRequest : IRemoteMessage
    {
        public RemoteRequest(string methodName, string[] args)
        {
            MethodName = methodName;
            Args = args;
            Headers = new MessageHeaders();
        }

        [JsonConstructor]
        public RemoteRequest(string methodName, string[] args, MessageHeaders headers)
        {
            MethodName = methodName;
            Args = args;
            Headers = headers;
        }

        public string MethodName { get; }
        public string[] Args { get; }
        public MessageHeaders Headers { get; }
    }
}