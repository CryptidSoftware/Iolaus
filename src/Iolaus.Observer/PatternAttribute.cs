using System;

namespace Iolaus.Observer
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PatternAttribute : Attribute
    {
        public string Pattern { get; }

        public PatternAttribute(string pattern)
        {
            Pattern = pattern;
        }
    }
}