using DG.Sculpt.Cron.FieldInternals;
using DG.Sculpt.Cron.Transformations;
using FluentAssertions;
using System;
using Xunit;

namespace DG.Sculpt.Tests.Cron.Transformations
{
    public class MonthTransformationTests
    {
        private MonthTransformation GetTransformation(string cronField)
        {
            return new MonthTransformation(CronField.TryParse(cronField, CronValueParser.Months).GetResultOrThrow());
        }

        [Fact]
        public void MoveBackwardsToLowest_SetsTo_1()
        {
            var transformation = GetTransformation("*");
            var time = new DateTimeOffset(2024, 3, 14, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveBackwardsToLowest(time);

            result.Month.Should().Be(1);
            result.Year.Should().Be(2024);
        }

        [Fact]
        public void MoveForwardWhileNotValid_TargetCurrent_ReturnsUnchanged()
        {
            var transformation = GetTransformation("3");
            var time = new DateTimeOffset(2024, 3, 14, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeFalse();
            result.Time.Should().Be(time);
        }

        [Fact]
        public void MoveForwardWhileNotValid_TargetHigher_ReturnsChanged()
        {
            var transformation = GetTransformation("5");
            var time = new DateTimeOffset(2024, 3, 14, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeTrue();
            result.Time.Month.Should().Be(5);
            result.Time.Year.Should().Be(2024);
        }

        [Fact]
        public void MoveForwardWhileNotValid_TargetHigher_NeverSkips()
        {
            var transformation = GetTransformation("2");
            var time = new DateTimeOffset(2023, 1, 31, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeTrue();
            result.Time.Month.Should().Be(2);
            result.Time.Day.Should().BeLessThanOrEqualTo(28);
            result.Time.Year.Should().Be(2023);
        }

        [Fact]
        public void MoveForwardWhileNotValid_TargetLower_AddsYear()
        {
            var transformation = GetTransformation("1");
            var time = new DateTimeOffset(2024, 3, 14, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeTrue();
            result.Time.Month.Should().Be(1);
            result.Time.Year.Should().Be(2025);
        }
    }
}
