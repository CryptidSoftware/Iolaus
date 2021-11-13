using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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

        public void LoadHandlers(IEnumerable<Assembly> assemblies)
        {
            var methods = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => x.GetCustomAttributes(typeof(HandlerAttribute), true).Length > 0)
                .SelectMany(x => x.GetMethods());

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

        public void LoadHandlers(Assembly assembly)
        {
            LoadHandlers(new[]{assembly});
        }

        public Func<Message, ReplyFunction, Task> GetFunc(IServiceProvider provider, Message message)
        {
            var handler = _patternStore.BestMatch(message).Match(
                Some: (m) => m,
                None: () => throw new NotImplementedException($"No handler was found for: {message}")
            );

            var instance = ActivatorUtilities.CreateInstance(provider,handler.ReflectedType);
            return handler.CreateDelegate<Func<Message, ReplyFunction, Task>>(instance);
        }
    }
}