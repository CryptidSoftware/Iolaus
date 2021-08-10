using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Iolaus.Config;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Text;
using static Iolaus.F;

namespace Iolaus.Http
{
    public static class HttpRoute
    {
        public static Func<IServiceProvider, RouteConfiguration, Func<Message, IObservable<Option<Message>>>> Route = (serviceProvider, config) => (message) =>
        {
            var clientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var destinationUrl = config.SelectToken<string>("Url").Unsafe();
            var client = clientFactory.CreateClient();

            async Task<Option<Message>> Post(Message m)
            {
                using(var client = clientFactory.CreateClient())
                {
                    var body = new StringContent(m.ToString(), Encoding.UTF8, "application/json");
                    var result = await client.PostAsync(destinationUrl, body);
                    if(!result.IsSuccessStatusCode)
                    {
                        return None;
                    }
                    var content = await result.Content.ReadAsStringAsync();
                    return Message.Parse(content);
                }

            }

            return Observable.FromAsync(async () => await Post(message));
        };
        
    }
}
