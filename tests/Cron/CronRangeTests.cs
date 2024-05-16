using DG.Sculpt.Cron;
using FluentAssertions;
using Xunit;

namespace DG.Sculpt.Tests.Cron
{
    public class CronRangeTests
    {
        private readonly static CronValueParser _parser = new CronValueParser("test-field", 1, 10, 2, "ONE", "TWO");

        [Theory]
        [InlineData(2, null, null, "2")]
        [InlineData(null, null, null, "*")]
        [InlineData(2, 5, null, "2-5")]
        [InlineData(2, null, 4, "2/4")]
        [InlineData(2, 5, 4, "2-5/4")]
        [InlineData(null, null, 4, "*/4")]
        public void ToString_ReturnsCorrectValues(int? start, int? end, int? stepSize, string expected)
        {
            var cronRange = new CronRange(new CronValue(start), new CronValue(end), stepSize);

            string result = cronRange.ToString();

            result.Should().Be(expected);
        }

        [Fact]
        public void TryParse_Asterisk_ReturnsAny()
        {
            string s = "*";
            var result = CronRange.TryParse(s, _parser);

            result.HasResult.Should().BeTrue();
            var actual = result.GetResultOrThrow();
            actual.IsAny.Should().BeTrue();
        }

        [Theory]
        [InlineData("5", "5")]
        [InlineData("2-5", "2-5")]
        [InlineData("2/4", "2/4")]
        [InlineData("2-5/4", "2-5/4")]
        [InlineData("*/4", "*/4")]
        [InlineData("ONE-TWO", "ONE-TWO")]
        [InlineData("ONE", "ONE")]
        public void TryParse_Works(string input, string expected)
        {
            var result = CronRange.TryParse(input, _parser);

            result.HasResult.Should().BeTrue();
            var actual = result.GetResultOrThrow();
            actual.ToString().Should().Be(expected);
        }
        [Theory]
        [InlineData("5-")]
        [InlineData("*/")]
        [InlineData("2-5/ONE")]
        [InlineData("2-5/0")]
        [InlineData("2-5/13")]
        public void TryParse_Throws(string input)
        {
            var result = CronRange.TryParse(input, _parser);

            result.HasResult.Should().BeFalse();
        }
    }
}
