using Iolaus.Config;
using System;
using Xunit;

namespace Iolaus.Tests.Config
{
    public class ConfigurationTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("nope", false)]
        [InlineData("{'Pattern':{'$.Property':'value'}}", false)]
        [InlineData("{'Route':{'Type':'test'}}", false)]
        [InlineData("{'Pattern':{'$.Property':'value'},'Route':{'Type':'test'}}", true)]
        public void Parse_Configuration(string json, bool expected)
        {
            bool isSome =
                Configuration
                    .Parse(json)
                    .Match(
                        None: () => false,
                        Some: c => true);
            
            Assert.Equal(expected, isSome);
        }

        [Fact]
        public void Configuration_ToString()
        {
            const string json = "{\"Pattern\":{\"$.Property\":\"value\"},\"Route\":{\"Type\":\"test\"}}";
            var configuration = Configuration.Parse(json);

            var actual = configuration.Match(
                None: () => false,
                Some: (c) => String.Equals(c.ToString(), json));

            Assert.True(actual);
        }
    }
}
