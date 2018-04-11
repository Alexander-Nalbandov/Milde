using Milde.Remoting.Contracts;
using Newtonsoft.Json;

namespace Milde.Remoting.Communication
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