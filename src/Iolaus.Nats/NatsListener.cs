using Iolaus.Observer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        private ILogger<NatsListener> _logger;

        public NatsListener(NatsListenerServices services, IOptions<NatsListenerOptions> options, ILogger<NatsListener> logger) => 
            (_connection, _router, _scopeFactory, _options, _logger) = 
            (services.Connection, services.Router, services.ScopeFactory, options.Value, logger);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var subscription = _connection.SubscribeAsync(_options.Topic, HandleMessage);
            await stoppingToken.WhenCanceled();
            subscription.Drain();
        }

        private async void HandleMessage(object sender, MsgHandlerEventArgs args)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var msg = Encoding.UTF8.GetString(args.Message.Data);
                var message = Message.Parse(msg).Unsafe();
                var optionHandler = _router.GetFunc(scope.ServiceProvider, message);

                await optionHandler.Match(
                    None: async () => await Task.CompletedTask,
                    Some: async (handler) => await handler(message, BuildReply(args.Message))
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Could not process message: {e.Message}");
            }
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