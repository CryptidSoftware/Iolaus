using static Iolaus.F;
using System.Collections.Generic;

namespace Iolaus
{
    public class PatternStore<T> : SortedList<Pattern, T>
    {
        public PatternStore() : base(new PatternComparer())
        {
        }

        public Option<T> BestMatch(Message message)
        {
            Option<T> bestMatch = None;
            foreach (var pair in this)
            {
                if (pair.Key.IsMatch(message))
                {
                    bestMatch = Some(pair.Value);
                    break;
                }
            }

            return bestMatch;
        }
    }
}