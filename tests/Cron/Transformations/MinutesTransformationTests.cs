using DG.Sculpt.Cron.FieldInternals;
using DG.Sculpt.Cron.Transformations;
using FluentAssertions;
using System;
using Xunit;

namespace DG.Sculpt.Tests.Cron.Transformations
{
    public class MinutesTransformationTests
    {
        private MinutesTransformation GetTransformation(string cronField)
        {
            return new MinutesTransformation(CronField.TryParse(cronField, CronValueParser.Minutes).GetResultOrThrow());
        }

        [Fact]
        public void MoveBackwardsToLowest_SetsTo_0()
        {
            var transformation = GetTransformation("*");
            var time = new DateTimeOffset(2024, 1, 1, 15, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveBackwardsToLowest(time);

            result.Minute.Should().Be(0);
            result.Hour.Should().Be(15);
        }

        [Fact]
        public void MoveForwardWhileNotValid_TargetCurrent_ReturnsUnchanged()
        {
            var transformation = GetTransformation("30");
            var time = new DateTimeOffset(2024, 1, 1, 0, 30, 0, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeFalse();
            result.Time.Should().Be(time);
        }

        [Fact]
        public void MoveForwardWhileNotValid_TargetHigher_ReturnsChanged()
        {
            var transformation = GetTransformation("40");
            var time = new DateTimeOffset(2024, 1, 1, 0, 30, 0, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeTrue();
            result.Time.Minute.Should().Be(40);
            result.Time.Hour.Should().Be(0);
        }

        [Fact]
        public void MoveForwardWhileNotValid_TargetLower_AddsHour()
        {
            var transformation = GetTransformation("30");
            var time = new DateTimeOffset(2024, 1, 1, 0, 40, 0, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeTrue();
            result.Time.Minute.Should().Be(30);
            result.Time.Hour.Should().Be(1);
        }
    }
}
