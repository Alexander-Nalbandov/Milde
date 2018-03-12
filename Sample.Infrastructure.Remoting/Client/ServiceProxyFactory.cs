using System;
using System.Collections.Generic;
using System.Text;
using Sample.Infrastructure.Remoting.Communication;

namespace Sample.Infrastructure.Remoting.Client
{
    public class ServiceProxyFactory
    {
        public static TInterface Create<TInterface>()
        {
            var proxy = new ServiceProxy<TInterface>(new RemoteProcedureExecutor<RemoteRequest, RemoteResponse>());
            proxy
        }
    }
}
