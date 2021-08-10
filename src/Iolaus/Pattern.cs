using static Iolaus.F;
using Iolaus.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Iolaus
{
    public class Pattern
    {
        private readonly string _pattern;
        private readonly Dictionary<string, string> _terms;

        public int Count => _terms.Count;

        public string PatternAsString => _pattern;

        public static Option<Pattern> Parse(string json)
            => JsonUtils.Parse(json).Match<Option<Pattern>>(
                None: () => None,
                Some: (jObject) => Some(new Pattern(json, jObject)));

        private Pattern(string json, JObject jObject)
        {
            _pattern = json;
            _terms = new Dictionary<string, string>();

            foreach (var pair in jObject)
            {
                _terms.Add(pair.Key, pair.Value?.ToString() ?? "");
            }
        }

        public bool IsMatch(Message message)
        {
            bool isShorterThanPattern = message.Count < Count;

            return !isShorterThanPattern &&
                !_terms.Any(term => message
                    .SelectToken<string>(term.Key)
                        .Match(
                            None: () => true,
                            Some: (patternTerm) => !String.Equals(term.Value, "*") && !String.Equals(patternTerm, term.Value)));
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Pattern) obj);
        }
        
        protected bool Equals(Pattern other)
        {
            return _terms.Count == other._terms.Count && !_terms.Except(other._terms).Any();
        }

        public override int GetHashCode()
        {
            //TODO: Improve hash code implementation
            return _terms.Aggregate(0, (value, pair) => value ^ pair.GetHashCode());
        }
    }
}