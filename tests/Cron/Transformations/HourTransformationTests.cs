using DG.Sculpt.Cron.FieldInternals;
using DG.Sculpt.Cron.Transformations;
using FluentAssertions;
using System;
using Xunit;

namespace DG.Sculpt.Tests.Cron.Transformations
{
    public class HourTransformationTests
    {
        private HourTransformation GetTransformation(string cronField)
        {
            return new HourTransformation(CronField.TryParse(cronField, CronValueParser.Hours).GetResultOrThrow());
        }

        [Fact]
        public void MoveBackwardsToLowest_SetsTo_0()
        {
            var transformation = GetTransformation("*");
            var time = new DateTimeOffset(2024, 3, 14, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveBackwardsToLowest(time);

            result.Hour.Should().Be(0);
            result.Day.Should().Be(14);
        }

        [Fact]
        public void MoveForwardWhileNotValid_TargetCurrent_ReturnsUnchanged()
        {
            var transformation = GetTransformation("18");
            var time = new DateTimeOffset(2024, 3, 14, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeFalse();
            result.Time.Should().Be(time);
        }

        [Fact]
        public void MoveForwardWhileNotValid_TargetHigher_ReturnsChanged()
        {
            var transformation = GetTransformation("22");
            var time = new DateTimeOffset(2024, 3, 14, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeTrue();
            result.Time.Hour.Should().Be(22);
            result.Time.Day.Should().Be(14);
        }

        [Fact]
        public void MoveForwardWhileNotValid_TargetLower_AddsDay()
        {
            var transformation = GetTransformation("17");
            var time = new DateTimeOffset(2024, 3, 14, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeTrue();
            result.Time.Hour.Should().Be(17);
            result.Time.Day.Should().Be(15);
        }
    }
}
