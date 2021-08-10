using Xunit;

namespace Iolaus.Tests
{
    public class PatternTests
    {
        [Theory]
        [InlineData("{}", 0)]
        [InlineData("{'$.a':'1'}", 1)]
        [InlineData("{'$.a':{'b':2}}", 1)]
        [InlineData("{'$.a':'1','$.b':'2'}", 2)]
        public void Pattern_Count(string json, int expected)
        {
            var pattern = Pattern.Parse(json);

            int actual = pattern
                .Match(
                    None: () => -1,
                    Some: (p) => p.Count);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("nope", false)]
        [InlineData("{'$.pattern':'something'}", true)]
        public void Parse_Pattern(string json, bool expected)
        {
            bool isSome = 
                Pattern.Parse(json)
                .Match<bool>(
                    None: () => false,
                    Some: (pattern => true)
                );

            Assert.Equal(expected, isSome);
        }

        [Theory]
        [InlineData("{'$.property':'value'}", "{'property':'value'}", true)]
        [InlineData("{'$.property':'*'}", "{'property':'value'}", true)]
        [InlineData("{'$.parent.child':'value'}", "{'parent':{'child':'value'}}", true)]
        [InlineData("{'$.property':'value'}", "{'no_match':'value'}", false)]
        public void Pattern_IsMatch(string pattern, string message, bool expected)
        {
            var p = Pattern.Parse(pattern);
            var m = Message.Parse(message);

            bool actual = p.Match(
                None: () => false,
                Some: (p) => m.Match(
                    None: () => false,
                    Some: (m) => p.IsMatch(m)));

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("", "{'$.propery':'value'}", false)]
        [InlineData("{'$.property':'value'}", "{'$.different':'value'}", false)]
        [InlineData("{'$.property':'value'}", "{'$.property':'value'}", true)]
        public void Pattern_Equal(string firstPattern, string secondPattern, bool expected)
        {
            var first = Pattern.Parse(firstPattern);
            var second = Pattern.Parse(secondPattern);

            bool actual = first.Match(
                None: () => false,
                Some: (f) => second.Match(
                    None: () => false,
                    Some: (s) => f.Equals(s)));

            Assert.Equal(expected, actual);
        }
    }
}