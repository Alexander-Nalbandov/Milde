using System;
using System.Collections.Generic;
using System.Text;
using Sample.Infrastructure.Remoting.Communication;

namespace Sample.Infrastructure.Remoting.Service
{
    public class MessageHandler<TInterface, TImplementation>
    {
        private readonly IListener<TInterface, RemoteRequest> _listener;
        private readonly ISender<TInterface, RemoteResponse> _sender;

        public MessageHandler(IListener<TInterface, RemoteRequest> listener, ISender<TInterface, RemoteResponse> sender)
        {
            _listener = listener;
            _sender = sender;
            _listener.AddHandler(this.HandleRequest);
        }

        private bool HandleRequest(RemoteRequest request)
        {
            return true;
        }
    }
}
