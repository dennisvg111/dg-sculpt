using DG.Sculpt.Cron.FieldInternals;
using FluentAssertions;
using Xunit;

namespace DG.Sculpt.Tests.Cron.FieldInternals
{
    public class CronValueTests
    {
        [Fact]
        public void ToString_Null_ReturnsAsterisk()
        {
            var value = new CronValue(null, null);

            value.ToString().Should().Be("*");
        }

        [Fact]
        public void ToString_Unnamed_ReturnsNumber()
        {
            var value = new CronValue(5, null);

            value.ToString().Should().Be("5");
        }

        [Fact]
        public void ToString_Named_ReturnsName()
        {
            var value = new CronValue(3, "THREE");

            value.ToString().Should().Be("THREE");
        }

        [Fact]
        public void ToString_NamedWithoutValue_ReturnsAsterisk()
        {
            var value = new CronValue(null, "THREE");

            value.ToString().Should().Be("*");
        }
    }
}
