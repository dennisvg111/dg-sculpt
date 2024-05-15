using DG.Sculpt.Cron;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace DG.Sculpt.Tests.Cron
{
    public class CronFieldTests
    {
        [Theory]
        [InlineData(2, null, null, null, "2")]
        [InlineData(null, null, null, null, "*")]
        [InlineData(2, null, null, new int[] { 3, 5, 8 }, "2,3,5,8")]
        [InlineData(2, 5, null, null, "2-5")]
        [InlineData(2, null, 4, null, "2/4")]
        [InlineData(2, 5, 4, null, "2-5/4")]
        [InlineData(null, null, 4, null, "*/4")]
        public void ToString_ReturnsCorrectValues(int? value, int? rangeUntil, int? step, int[] otherValues, string expected)
        {
            var cronValue = new CronField(new CronValue(value), new CronValue(rangeUntil), step, otherValues?.Select(v => new CronValue(v)).ToList());

            string result = cronValue.ToString();

            result.Should().Be(expected);
        }
    }
}
