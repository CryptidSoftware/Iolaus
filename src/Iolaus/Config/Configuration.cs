using static Iolaus.F;
using Iolaus.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Iolaus.Config
{
    public class Configuration
    {
        public Pattern Pattern {get;}
        public RouteConfiguration Route {get;}

        private readonly JObject _configuration;

        public static Option<Configuration> Parse(string json)
            => JsonUtils.Parse(json).Match<Option<Configuration>>(
                None: () => None,
                Some: (jObject) => Parse(jObject));

        private static Option<Configuration> Parse(JObject config)
        {
            var pattern = config.GetProperty("Pattern").Match(
                None: () => None,
                Some: (p) => Pattern.Parse(p.ToString())
            );

            var route = config.GetProperty("Route").Match(
                None: () => None,
                Some: (r) => RouteConfiguration.Parse(r.ToString())
            );

            return pattern.Match<Option<Configuration>>(
                None: () => None,
                Some: (p) => route.Match<Option<Configuration>>(
                    None: () => None,
                    Some: (r) => Some(new Configuration(config, p, r))
                )
            );
        }

        private Configuration(JObject configuration, Pattern pattern, RouteConfiguration routeConfiguration)
        {
            _configuration = configuration;
            Pattern = pattern;
            Route = routeConfiguration;
        }

        public override string ToString()
            => _configuration.ToString(Formatting.None);
    }
}