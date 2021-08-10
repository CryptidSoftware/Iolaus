using System;
using System.Collections.Generic;
using Iolaus.Config;
using static Iolaus.F;

namespace Iolaus
{
    using RouteBuilder = Func<IServiceProvider, RouteConfiguration, 
        Func<Message, IObservable<Option<Message>>>>;

    public class RouteRegistry
    {
        private Dictionary<string, RouteBuilder> _functionMap;
        public RouteRegistry()
        {
            _functionMap = new Dictionary<string, RouteBuilder>();
        }

        public Option<RouteBuilder> GetRouteFactory(string routeType)
            => _functionMap.ContainsKey(routeType) 
                ? Some(_functionMap[routeType])
                : None;

        public void Add(string routeType, RouteBuilder builderFunc)
            => _functionMap.Add(routeType, builderFunc);
    };
}