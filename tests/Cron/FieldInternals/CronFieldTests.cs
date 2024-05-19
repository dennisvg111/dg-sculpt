using DG.Sculpt.Cron.FieldInternals;
using FluentAssertions;
using Xunit;

namespace DG.Sculpt.Tests.Cron.FieldInternals
{
    public class CronFieldTests
    {
        private readonly static CronValueParser _parser = new CronValueParser("test-field", 1, 10, 2, "ONE", "TWO");

        [Fact]
        public void MultipleRanges_CombinesAllowedValues()
        {
            var range1 = new CronRange(new CronValue(2), new CronValue(4), null);   //2-4
            var range2 = new CronRange(new CronValue(6), new CronValue(10), 2);     //6-10/2

            var field = new CronField(1, 10, range1, range2);

            field.AllowedValues.Should().Equal(2, 3, 4, 6, 8, 10);
        }

        [Fact]
        public void AllowedValues_ShouldBeAscending()
        {
            var field = CronField.TryParse("8-1,3-4", _parser).GetResultOrThrow();

            field.AllowedValues.Should().Equal(1, 3, 4, 8, 9, 10);
        }

        [Fact]
        public void TryParse_CombinesAllowedValues()
        {
            var field = CronField.TryParse("1-3,6,9", _parser).GetResultOrThrow();

            field.AllowedValues.Should().Equal(1, 2, 3, 6, 9);
        }

        [Fact]
        public void CanBe_IsAllowed_ReturnsTrue()
        {
            var field = CronField.TryParse("6", _parser).GetResultOrThrow();

            bool result = field.CanBe(6);

            result.Should().BeTrue();
        }

        [Fact]
        public void CanBe_IsNotAllowed_ReturnsFalse()
        {
            var field = CronField.TryParse("6", _parser).GetResultOrThrow();

            bool result = field.CanBe(8);

            result.Should().BeFalse();
        }
    }
}
