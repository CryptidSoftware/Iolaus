using static Iolaus.F;
using Iolaus.Utils;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Iolaus.Config
{
    public class RouteConfiguration
    {
        private readonly JObject _configuration;

        public RouteConfiguration[] Children {get;}

        public static Option<RouteConfiguration> Parse(string json)
            => JsonUtils
                .Parse(json)
                .Match<Option<RouteConfiguration>>(
                    None: () => None,
                    Some: (jObject) => Some(new RouteConfiguration(jObject)));

        private RouteConfiguration(JObject jObject)
        {
            _configuration = jObject;

            Children = _configuration
                .GetProperty("Children")
                .Match<RouteConfiguration[]>(
                    None: () => new RouteConfiguration[0],
                    Some: (t) => t.Select(j =>
                        RouteConfiguration
                            .Parse(j.ToString()))
                            .WhereSome()
                            .ToArray());
        }

        public string Type
            => _configuration
                .GetProperty("Type")
                .Match(
                    None: () => "",
                    Some: (t) => t.ToString()
                );

        public Option<T> SelectToken<T>(string property)
            => _configuration.SelectToken<T>(property);
    }
}