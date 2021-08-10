using Iolaus.Config;
using static Iolaus.F;
using System;
using System.Reactive.Linq;
using Xunit;

namespace Iolaus.Tests
{
    using RouteBuilder = Func<IServiceProvider, RouteConfiguration, 
        Func<Message, IObservable<Option<Message>>>>;
    public class RouteRegistryTests
    {
        private readonly RouteRegistry _routeRegistry;

        public RouteRegistryTests()
        {
            _routeRegistry = new RouteRegistry();
        }

        [Fact]
        public void Add_Route()
        {
            Func<Message, IObservable<Option<Message>>> test = (m)
                => Observable.Return<Option<Message>>(None);
            RouteBuilder testMethod = (sp, rc) => test;
            
            _routeRegistry.Add(nameof(Add_Route), testMethod);
        }

        [Fact]
        public void GetRouteFactory()
        {
            Func<Message, IObservable<Option<Message>>> test = (m)
                => Observable.Return<Option<Message>>(None);
            RouteBuilder testMethod = (sp, rc) => test;
            _routeRegistry.Add(nameof(GetRouteFactory), testMethod);

            bool actual = _routeRegistry
                .GetRouteFactory(nameof(GetRouteFactory))
                .Match(
                    None: () => false,
                    Some: (f) => true);

            Assert.True(actual);
        }
    }
}
