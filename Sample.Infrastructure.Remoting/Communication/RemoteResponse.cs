﻿using Newtonsoft.Json;
using Sample.Infrastructure.Remoting.Contracts;

namespace Sample.Infrastructure.Remoting.Communication
{
    public class RemoteResponse : IRemoteMessage
    {
        public RemoteResponse(object response)
        {
            this.ResponseType = response.GetType().FullName;
            this.Response = response;
            Headers = new MessageHeaders();
        }

        [JsonConstructor]
        public RemoteResponse(string responseType, object response)
        {
            ResponseType = responseType;
            Response = response;
            Headers = new MessageHeaders();
        }

        public string ResponseType { get; }

        public object Response { get; }

        public MessageHeaders Headers { get; }
    }
}