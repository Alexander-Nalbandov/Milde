using Newtonsoft.Json;

namespace Sample.Infrastructure.Remoting
{
    internal class RemoteResponse : IRemoteMessage
    {
        public RemoteResponse(object response)
        {
            this.ResponseType = response.GetType().FullName;
            this.Response = response;
        }

        [JsonConstructor]
        public RemoteResponse(string responseType, object response)
        {
            ResponseType = responseType;
            Response = response;
        }

        public string ResponseType { get; }

        public object Response { get; }
    }
}