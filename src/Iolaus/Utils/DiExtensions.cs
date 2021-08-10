using System;
using Microsoft.Extensions.DependencyInjection;

namespace Iolaus.Utils
{
    public static class DiExtensions
    {
        public static IServiceCollection AddRouteRegistry(this IServiceCollection sc, Action<RouteRegistry> setup)
        {
            sc.AddSingleton<RouteRegistry>((provider) =>
            {
                var rr = new RouteRegistry();
                setup(rr);
                return rr;
            });
            return sc;
        }
    }
}