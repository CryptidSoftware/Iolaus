using System.Collections.Generic;

namespace Iolaus.Utils
{
    public static class OptionUtils
    {
        public static IEnumerable<T> WhereSome<T>(this IEnumerable<Option<T>> collection)
        {
            var someList = new List<T>();
            foreach (var option in collection)
            {
                option.Match(
                    None: () => false,
                    Some: (t) => {
                        someList.Add(t);
                        return true;
                    }
                );
            } 
            return someList;
        }
    }
}