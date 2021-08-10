using Iolaus.Config;
using Xunit;

namespace Iolaus.Tests.Config
{
    public class RouteConfigurationTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("invalid", false)]
        [InlineData("{'Type':'test'}", true)]
        public void Parse_RouteConfiguration(string json, bool expected)
        {
            bool isSome =
                RouteConfiguration
                    .Parse(json)
                    .Match(
                        None: () => false,
                        Some: r => true);
            
            Assert.Equal(expected, isSome);
        }

        [Fact]
        public void RouteConfiguration_Type()
        {
            const string json = "{'Type':'test'}";
            const string expected = "test";
            string type = RouteConfiguration
                .Parse(json)
                .Match(
                    None: () => "",
                    Some: r => r.Type);
            
            Assert.Equal(expected, type);
        }

        [Theory]
        [InlineData(null, "{'exists':true}", false)]
        [InlineData("", "{'exists':true}", false)]
        [InlineData("exists", "{'exists':'something'}", false)]
        [InlineData("not_exists", "{'exists':true}", false)]
        [InlineData("exists", "{'exists':true}", true)]
        public void RouteConfiguration_SelectToken(string path, string json, bool expected)
        {
            bool actual = RouteConfiguration
                .Parse(json)
                .Match(
                    None: () => false,
                    Some: r => r
                        .SelectToken<bool>(path)
                        .Match(
                            None: () => false,
                            Some: r => r));
            
            Assert.Equal(expected, actual);
        }
    }
}