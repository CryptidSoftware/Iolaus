using Iolaus.Config;
using System;

namespace Iolaus
{
    public class Router
    {
        private PatternStore<Func<Message, IObservable<Option<Message>>>> _routes;

        private readonly IServiceProvider _serviceProvider;
        private readonly Configuration[] _configurations;
        private readonly RouteRegistry _registry;

        public Router(IServiceProvider serviceProvider, IConfigurationProvider provider, RouteRegistry registry)
        {
            _serviceProvider = serviceProvider;
            _configurations = provider.GetConfigurations();
            _routes = new PatternStore<Func<Message, IObservable<Option<Message>>>>();
            _registry = registry;
            
            foreach(var config in _configurations)
            {
                Add(config);
            }
            
        }

        //TODO: IConfigurationProvider shouldn't be passed in and iterated. We'll need to get this working
        // public Router(IServiceProvider serviceProvider, RouteRegistry registry)
        // {
        //     _serviceProvider = serviceProvider;
        //     _registry = registry;
        //     _routes = new Dictionary<Pattern, Func<Message, IObservable<Option<Message>>>>();
        // }

        public void Add(Configuration config)
            => _registry
                .GetRouteFactory(config.Route.Type)
                .Match(
                    Some: (function) => {
                        _routes.Add(config.Pattern, function(_serviceProvider, config.Route));
                        return true;
                    },
                    None: () => false);
        
        public void Add(Pattern pattern, Func<IServiceProvider, Func<Message, IObservable<Option<Message>>>> f)
            => Add(pattern, f(_serviceProvider));
        
        public void Add(Pattern pattern, Func<Message, IObservable<Option<Message>>> f)
            => _routes.Add(pattern, f);

        public Func<Message, IObservable<Option<Message>>> GetFunc(Message message)
            => _routes.
                BestMatch(message)
                .Match(
                    None: () => throw new NotImplementedException("No function was found for the specified message"),
                    Some: (f) => f);
    }
}