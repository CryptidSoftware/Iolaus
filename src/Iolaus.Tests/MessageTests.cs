using System;
using System.Collections.Generic;
using Xunit;

namespace Iolaus.Tests
{
    public class MessageTest
    {
        [Theory]
        [InlineData("{}", 0)]
        [InlineData("{'a':'1'}", 1)]
        [InlineData("{'a':{'b':2}}", 1)]
        [InlineData("{'a':'1','b':'2'}", 2)]
        public void Message_Count(string json, int expected)
        {
            var message = Message.Parse(json);
            
            int actual = message
                .Match(
                    None: () => -1,
                    Some: (m) => m.Count);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("nope", false)]
        [InlineData("{'a':'message'}", true)]
        public void Parse_Message(string json, bool expected)
        {
            bool isSome = 
                Message.Parse(json)
                .Match<bool>(
                    None: () => false,
                    Some: (pattern => true));

            Assert.Equal(expected, isSome);
        }

        public static IEnumerable<object[]> FromObjects()
        {
            yield return new object[]{null, false};
            yield return new object[]{"", false};
            yield return new object[]{32, false};
            yield return new object[]{false, false};
            yield return new object[]{new[]{"item"}, false};
            yield return new object[]{new{Property="Property"}, true};
        }

        [Theory]
        [MemberData(nameof(FromObjects))]
        public void Message_FromObject(object o, bool expected)
        {
            bool actual = Message
                .FromObject(o)
                .Match(
                    None: () => false,
                    Some: (m) => m is Message);
            
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("exists", true)]
        [InlineData("missing", false)]
        public void Message_ContainsKey(string key, bool expected)
        {
            const string json = "{'exists': true}";
            var message = Message.Parse(json);

            bool actual = message.Match(
                None: () => false,
                Some: (msg) => msg.ContainsKey(key));

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, "{'exists':true}", false)]
        [InlineData("", "{'exists':true}", false)]
        [InlineData("$.exists", "{'exists':'something'}", false)]
        [InlineData("$.not_exists", "{'exists':true}", false)]
        [InlineData("$.exists", "{'exists':true}", true)]
        public void SelectToken_From_Message(string path, string json, bool expected)
        {
            var message = Message.Parse(json);

            var actual = message.Match(
                None: () => false,
                Some: (msg) => msg
                    .SelectToken<bool>(path)
                    .Match(
                        None: () => false,
                        Some: (val) => val));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Message_ToString()
        {
            const string json = "{\"valid\":\"message\"}";
            var message = Message.Parse(json);

            var actual = message.Match(
                None: () => false,
                Some: (m) => String.Equals(m.ToString(), json));

            Assert.True(actual);
        }
    }
}
