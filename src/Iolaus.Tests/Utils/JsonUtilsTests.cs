using Iolaus.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Xunit;

namespace Iolaus.Tests.Utils
{
    public class JsonUtilsTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("{}", true)]
        [InlineData("{'a':'b'}", true)]
        public void Parse_Json(string json, bool expected)
        {
            var option = JsonUtils.Parse(json);

            var actual = option
                .Match(
                    None: () => false,
                    Some: (o) => o.GetType() == typeof(JObject));

            Assert.Equal(expected, actual);
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
        public void JObject_FromObject(object o, bool expected)
        {
            bool actual = JsonUtils
                .FromObject(o)
                .Match(
                    None: () => false,
                    Some: (jObject) => jObject is JObject);
            
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, "{'exists':true}", false)]
        [InlineData("", "{'exists':true}", false)]
        [InlineData("$.exists", "{'exists':'something'}", false)]
        [InlineData("$.not_exists", "{'exists':true}", false)]
        [InlineData("$.exists", "{'exists':true}", true)]
        public void SelectToken_From_JObject(string path, string json, bool expected)
        {
            var jObject = JObject.Parse(json);

            var actual = jObject
                .SelectToken<bool>(path)
                .Match(
                    None: () => false,
                    Some: (token) => token);

            Assert.Equal(expected, actual);
        }
    }
}