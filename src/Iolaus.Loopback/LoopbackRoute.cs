using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Iolaus.Config;
using Iolaus.Observer;
using Microsoft.Extensions.DependencyInjection;

namespace Iolaus.Loopback
{
    public static class LoopbackRoute
    {
        public static Func<IServiceProvider, RouteConfiguration, Func<Message, IObservable<Option<Message>>>> Route =
            (provider, configuration) => (message) =>
            {
                //Get the ObserverRouter from DI and look up the appropriate handler
                var observerRouter = provider.GetRequiredService<ObserverRouter>();
                var handler = observerRouter.GetFunc(provider, message);
                
                //Return an observable that calls the handler and passes it a reply function
                //that calls the observer onNext for each reply
                return Observable.Create<Option<Message>>(async observer =>
                {
                    await handler(message, BuildReply(observer));
                    //Close the observer when we know we won't get any more replies
                    observer.OnCompleted();
                });
            };

        private static Func<Message, Task> BuildReply(IObserver<Option<Message>> replies)
        {
            return (msg) =>
            {
                replies.OnNext(msg);
                return Task.CompletedTask;
            };
        }
    }
}