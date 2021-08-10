using Iolaus.Observer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NATS.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Iolaus.Nats
{
    public class NatsListener : BackgroundService
    {
        private IConnection _connection;
        private ObserverRouter _router;
        private IServiceScopeFactory _scopeFactory;
        private NatsListenerOptions _options;

        public NatsListener(NatsListenerServices services, IOptions<NatsListenerOptions> options) => 
            (_connection, _router, _scopeFactory, _options) = 
            (services.Connection, services.Router, services.ScopeFactory, options.Value);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var subscription = _connection.SubscribeAsync(_options.Topic, HandleMessage);
            await stoppingToken.WhenCanceled();
            subscription.Drain();
        }

        private async void HandleMessage(object sender, MsgHandlerEventArgs args)
        {
            using var scope = _scopeFactory.CreateScope();
            var msg = Encoding.UTF8.GetString(args.Message.Data);
            var message = Message.Parse(msg).Unsafe();
            var handlerFunc = _router.GetFunc(scope.ServiceProvider, message);
            await handlerFunc(message, BuildReply(args.Message));
        }

        private Func<Message,Task> BuildReply(Msg natsMessage)
        {
            return (message) => 
            {
                natsMessage.Respond(Encoding.UTF8.GetBytes(message.ToString()));
                return Task.CompletedTask;
            };
        }
    }
}