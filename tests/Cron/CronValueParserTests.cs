using DG.Sculpt.Cron;
using FluentAssertions;
using Xunit;

namespace DG.Sculpt.Tests.Cron
{
    public class CronValueParserTests
    {
        [Theory]
        [InlineData("1", true, 1)]
        [InlineData("10", true, 10)]
        [InlineData("0", false, 0)]
        [InlineData("-1", false, -1)]
        [InlineData("11", true, 1)]
        [InlineData("12", true, 2)]
        [InlineData("13", false, 13)]
        [InlineData("TWO", true, 2)]
        [InlineData("THREE", false, 3)]
        public void TryParse_Works(string value, bool result, int expected)
        {
            CronValueParser parser = new CronValueParser("test-field", 1, 10, 2, "ONE", "TWO");

            bool actualResult = parser.TryParse(value, out CronValue actual);
            actualResult.Should().Be(result);

            if (actualResult)
            {
                actual.Value.Should().Be(expected);
            }
        }
    }
}
