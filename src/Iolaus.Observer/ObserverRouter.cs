using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Iolaus.Observer
{
    using ReplyFunction = Func<Message, Task>;

    public class ObserverRouter
    {
        private readonly PatternStore<MethodInfo> _patternStore;

        public ObserverRouter(IServiceProvider serviceProvider)
        {
            _patternStore = new PatternStore<MethodInfo>();
        }

        public void LoadHandlers(Assembly assembly)
        {
            var types = assembly
                .GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(HandlerAttribute), true).Length > 0);

            var methods = types.SelectMany(t => t.GetMethods());

            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(PatternAttribute), true);

                if (!attributes.Any())
                {
                    continue;
                }

                var patternAttribute = attributes[0] as PatternAttribute;
                var pattern = Pattern.Parse(patternAttribute.Pattern);
                _patternStore.Add(pattern.Unsafe(), method);
            }
        }

        public Func<Message, ReplyFunction, Task> GetFunc(IServiceProvider provider, Message message)
        {
            var handler = _patternStore.BestMatch(message).Match(
                Some: (m) => m,
                None: () => throw new ArgumentException("Oh no")
            );

            var instance = ActivatorUtilities.CreateInstance(provider,handler.ReflectedType);
            var x = handler.CreateDelegate<Func<Message, ReplyFunction, Task>>(instance);
            return x;
        }

    }
}