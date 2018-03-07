using Newtonsoft.Json;

namespace Sample.Infrastructure.Remoting
{
    internal class RemoteRequest : IRemoteMessage
    {
        [JsonConstructor]
        public RemoteRequest(string methodName, object[] args)
        {
            MethodName = methodName;
            Args = args;
        }

        public string MethodName { get; }
        public object[] Args { get; }
    }
}