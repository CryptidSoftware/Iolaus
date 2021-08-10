using static Iolaus.F;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Iolaus.Utils
{
    internal static class JsonUtils
    {
        internal static Option<JObject> Parse(string json)
        {
            Option<JObject> result;
            try
            {
                result = Some(JObject.Parse(json));
            }
            catch(Exception)
            {
                result = None;
            }

            return result;
        }

        internal static Option<JObject> FromObject<T>(T source)
        {
            if (source is null)
            {
                return None;
            }
            try
            {
                return Some(JObject.FromObject(source));
            }
            catch(ArgumentException)
            {
                return None;
            }
            catch (JsonException)
            {
                return None;
            }
        }

        internal static Option<T> SelectToken<T>(this JObject source, string path)
        {
            Option<T> option;
            try
            {
                var token = path is null || path.Length == 0 ? null : source.SelectToken(path);
                var value = token is null ? default : token.Value<T>();
                option = value is null ? None : Some(value);
            }
            catch(FormatException)
            {
                option = None;
            }

            return option;
        }

        internal static Option<JToken> GetProperty(this JObject json, string property)
            => json.TryGetValue(property, out var token) ? Some(token) : None;
    }
}