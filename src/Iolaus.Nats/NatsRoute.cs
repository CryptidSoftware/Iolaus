using Iolaus.Config;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;
using NATS.Client.Rx;
using NATS.Client.Rx.Ops;
using System;
using System.Reactive.Linq;
using System.Text;


namespace Iolaus.Nats
{   
    //docker run --rm -p 4222:4222 -p 8222:8222 -p 6222:6222 --name nats-server -ti nats:latest
    public static class NatsRoute
    {
        public static Func<IServiceProvider, RouteConfiguration, Func<Message, IObservable<Option<Message>>>> Route = (IServiceProvider serviceProvider, RouteConfiguration configuration) => (Message message) =>
        {
            var connection = serviceProvider.GetRequiredService<IConnection>();
            var inbox = connection.NewInbox();
            var observable = connection.Observe(inbox);
            var connectable = observable.Replay();
            connectable.Connect();
            var completable = connectable.Finally(() => observable.Dispose());
            connection.Publish(configuration.SelectToken<string>("Subject").Unsafe(), inbox, Encoding.UTF8.GetBytes(message.ToString()));

            return completable.Select(m => Message.Parse(Encoding.UTF8.GetString(m.Data)));
        };
    }
}
