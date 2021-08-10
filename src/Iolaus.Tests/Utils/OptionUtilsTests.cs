using static Iolaus.F;
using Iolaus.Utils;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Iolaus.Tests.Utils
{
    public class OptionUtilsTests
    {
        public static IEnumerable<object[]> Options()
        {
            yield return new object[]{new Option<string>[]{None, None, None}, 0};
            yield return new object[]{new Option<string>[]{None, None, Some("")}, 1};
            yield return new object[]{new Option<string>[]{None, Some(""), Some("")}, 2};
            yield return new object[]{new Option<string>[]{Some(""), Some(""), Some("")}, 3};
        }

        [Theory]
        [MemberData(nameof(Options))]
        public void WhereSome(Option<string>[] options, int expected)
        {
            var actual = options
                .WhereSome()
                .Count();

            Assert.Equal(expected, actual);
        }
    }
}