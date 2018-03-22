using System;
using Newtonsoft.Json;
using Sample.Infrastructure.Remoting.Contracts;

namespace Sample.Infrastructure.Remoting.Communication
{
    internal class RemoteResponse : IRemoteMessage
    {
        public RemoteResponse(object response)
        {
            ResponseType = response.GetType().FullName;
            Response = response;
            Headers = new MessageHeaders();
            IsFaulted = false;
        }

        public RemoteResponse(Exception exception)
        {
            ResponseType = exception.GetType().FullName;
            Headers = new MessageHeaders();
            IsFaulted = true;
            Exception = exception;
        }

        [JsonConstructor]
        public RemoteResponse(string responseType, object response, MessageHeaders headers, bool isFaulted,
            Exception exception)
        {
            ResponseType = responseType;
            Response = response;
            Headers = headers;
            IsFaulted = isFaulted;
            Exception = exception;
        }

        public string ResponseType { get; }

        public object Response { get; }

        public bool IsFaulted { get; }

        public Exception Exception { get; }

        public MessageHeaders Headers { get; }
    }
}