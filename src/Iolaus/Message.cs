using static Iolaus.F;
using Iolaus.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Iolaus
{
    public class Message
    {
        private JObject _message;
        public int Count => _message.Count;

        public static Option<Message> Parse(string message)
            => JsonUtils.Parse(message).Match<Option<Message>>(
                None: () => None,
                Some: (jObject) => Some(new Message(jObject)));

        public static Option<Message> FromObject<T>(T source)
            => JsonUtils.FromObject(source).Match<Option<Message>>(
                None: () => None,
                Some: (jObject) => Some(new Message(jObject)));

        private Message(JObject jObject)
        {
            _message = jObject;
        }

        public bool ContainsKey(string key)
            => _message.ContainsKey(key);

        public Option<T> SelectToken<T>(string path)
            => _message.SelectToken<T>(path);

        public override string ToString()
            => _message.ToString(Formatting.None);
    }
}
