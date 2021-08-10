using System.Collections.Generic;
using Xunit;

namespace Iolaus.Tests
{
    public class PatternStoreTests
    {
        public static IEnumerable<object[]> FromObjects()
        {
            var oneTerm = Pattern.Parse("{'$.type':'dice'}").Unsafe();
            var twoTerm = Pattern.Parse("{'$.type':'dice','$.cmd':'roll'}").Unsafe();
            var threeTerm = Pattern.Parse("{'$.type':'dice','$.cmd':'roll','$.stats':true}").Unsafe();
            var patterns = new []{(oneTerm, "one"), (twoTerm, "two"), (threeTerm, "three")};
            yield return new object[]{patterns, Message.Parse("{}").Unsafe(), ""};
            yield return new object[]{patterns, Message.Parse("{'type':'dice'}").Unsafe(), "one"};
            yield return new object[]{patterns, Message.Parse("{'type':'dice','cmd':'roll'}").Unsafe(), "two"};
            yield return new object[]{patterns, Message.Parse("{'type':'dice','cmd':'roll','stats':true}").Unsafe(), "three"};
        }

        [Theory]
        [MemberData(nameof(FromObjects))]
        public void PatternStore_BestMatch((Pattern, string)[] patterns, Message message, string expected)
        {
            var patternStore = new PatternStore<string>();
            foreach(var pattern in patterns)
            {
                patternStore.Add(pattern.Item1, pattern.Item2);
            }

            var actual = patternStore
                .BestMatch(message)
                .Match(
                    None: () => "",
                    Some: (s) => s);

            Assert.Equal(expected, actual);
        }
    }
}