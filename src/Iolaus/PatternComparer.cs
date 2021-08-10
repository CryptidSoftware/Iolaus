using System;
using System.Collections.Generic;

namespace Iolaus
{
    internal class PatternComparer : IComparer<Pattern>
    {
        public int Compare(Pattern? a, Pattern? b)
        {
            if (a is null || b is null)
            {
                throw new ArgumentNullException("Both patterns must be set");
            }
            int comparison = 0;
            if (a.Count.CompareTo(b.Count) == 0)
            {
                comparison = a.PatternAsString.CompareTo(b.PatternAsString);
            }
            else
            {
                comparison = a.Count.CompareTo(b.Count);
            }
            //this will sort the list desc
            return -comparison;

        }
    }
}