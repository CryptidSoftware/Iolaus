using Iolaus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Iolaus.Observer
{
    public class LocalRoute
    {
        private Dictionary<Pattern, MethodInfo> _routes;
        //Doesn't seem like the right name for this
        public LocalRoute()
        {
            _routes = new Dictionary<Pattern, MethodInfo>();
        }

        //This should be moved
        public void LoadHandlers(Assembly assembly)
        {
            var types = assembly
                .GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(HandlerAttribute), true).Length > 0);

            var methods = types.SelectMany(t => t.GetMethods());

            foreach(var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(PatternAttribute), true);

                if (!attributes.Any())
                {
                    continue;
                }

                var patternAttribute = attributes[0] as PatternAttribute;
                var pattern = Pattern.Parse(patternAttribute.Pattern);
                Add(pattern.Unsafe(), method);
            }
        }

        public void Add(Pattern pattern, MethodInfo method)
        {
            if (_routes.ContainsKey(pattern))
            {
                throw new Exception("Already contains pattern");
            }
            
            _routes.Add(pattern, method);
        }
    }
}