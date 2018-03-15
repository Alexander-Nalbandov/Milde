using Newtonsoft.Json;
using Sample.Infrastructure.Remoting.Contracts;

namespace Sample.Infrastructure.Remoting.Communication
{
    internal class RemoteResponse : IRemoteMessage
    {
        public RemoteResponse(object response)
        {
            this.ResponseType = response.GetType().FullName;
            this.Response = response;
            Headers = new MessageHeaders();
        }

        [JsonConstructor]
        public RemoteResponse(string responseType, object response, MessageHeaders headers)
        {
            ResponseType = responseType;
            Response = response;
            Headers = headers;
        }

        public string ResponseType { get; }

        public object Response { get; }

        public MessageHeaders Headers { get; }
    }
}