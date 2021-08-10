using Iolaus.Observer;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;

namespace Iolaus.Nats
{
    public class NatsListenerServices
    {
        public IConnection Connection { get; set; }
        public IServiceScopeFactory ScopeFactory { get; set; }
        public ObserverRouter Router { get; set; }

        public NatsListenerServices(IConnection connection, IServiceScopeFactory serviceScopeFactory, ObserverRouter observerRouter) {
            Connection = connection;
            ScopeFactory = serviceScopeFactory;
            Router = observerRouter;
        }

    }
}