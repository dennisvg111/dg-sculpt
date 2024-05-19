using DG.Sculpt.Cron.FieldInternals;
using DG.Sculpt.Cron.Transformations;
using FluentAssertions;
using System;
using Xunit;

namespace DG.Sculpt.Tests.Cron.Transformations
{
    public class DayTransformationTests
    {
        private DayTransformation GetTransformation(string dayOfMonth, string dayOfWeek)
        {
            var dayOfMonthField = CronField.TryParse(dayOfMonth, CronValueParser.DayOfMonth).GetResultOrThrow();
            var dayOfWeekField = CronField.TryParse(dayOfWeek, CronValueParser.DayOfWeek).GetResultOrThrow();
            return new DayTransformation(dayOfMonthField, dayOfWeekField);
        }

        [Fact]
        public void MoveBackwardsToLowest_SetsTo_1()
        {
            var transformation = GetTransformation("*", "*");
            var time = new DateTimeOffset(2024, 3, 14, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveBackwardsToLowest(time);

            result.Day.Should().Be(1);
            result.Month.Should().Be(3);
        }

        [Fact]
        public void MoveForwardWhileNotValid_TargetCurrentMonthDay_ReturnsUnchanged()
        {
            var transformation = GetTransformation("14", "2");
            var time = new DateTimeOffset(2024, 3, 14, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeFalse();
            result.Time.Should().Be(time);
        }

        [Fact]
        public void MoveForwardWhileNotValid_TargetCurrentWeekDay_ReturnsUnchanged()
        {
            var transformation = GetTransformation("12", "4");
            var time = new DateTimeOffset(2024, 3, 14, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeFalse();
            result.Time.Should().Be(time);
        }

        [Fact]
        public void MoveForwardWhileNotValid_TargetHigherMonthDay_ReturnsChanged()
        {
            var transformation = GetTransformation("22", "*");
            var time = new DateTimeOffset(2024, 3, 14, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeTrue();
            result.Time.Day.Should().Be(22);
            result.Time.Month.Should().Be(3);
        }

        [Fact]
        public void MoveForwardWhileNotValid_TargetHigherWeekDay_ReturnsChanged()
        {
            var transformation = GetTransformation("*", "0");
            var time = new DateTimeOffset(2024, 3, 14, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeTrue();
            result.Time.Day.Should().Be(17);
            result.Time.Month.Should().Be(3);
        }

        [Fact]
        public void MoveForwardWhileNotValid_MonthDayBeforeWeekDay_StopsAtMonthDay()
        {
            var transformation = GetTransformation("16", "0");
            var time = new DateTimeOffset(2024, 3, 14, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeTrue();
            result.Time.Day.Should().Be(16);
            result.Time.Month.Should().Be(3);
        }

        [Fact]
        public void MoveForwardWhileNotValid_WeekDayBeforeMonthDay_StopsAtWeekDay()
        {
            var transformation = GetTransformation("20", "5");
            var time = new DateTimeOffset(2024, 3, 14, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeTrue();
            result.Time.Day.Should().Be(15);
            result.Time.Month.Should().Be(3);
        }

        [Fact]
        public void MoveForwardWhileNotValid_TargetLower_AddsMonth()
        {
            var transformation = GetTransformation("20", "*");
            var time = new DateTimeOffset(2024, 1, 30, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeTrue();
            result.Time.Day.Should().Be(20);
            result.Time.Month.Should().Be(2);
        }

        [Fact]
        public void MoveForwardWhileNotValid_DayNotInMonth_SkipsMonth()
        {
            var transformation = GetTransformation("30", "*");
            var time = new DateTimeOffset(2024, 1, 31, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeTrue();
            result.Time.Day.Should().Be(30);
            result.Time.Month.Should().Be(3); //february skipped.
        }

        [Fact]
        public void MoveForwardWhileNotValid_NoLeapDay_Skipped()
        {
            var transformation = GetTransformation("29", "*");
            var time = new DateTimeOffset(2023, 2, 1, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeTrue();
            result.Time.Day.Should().Be(29);
            result.Time.Month.Should().Be(3); //2023 is not a leap year, so february is skipped.
        }

        [Fact]
        public void MoveForwardWhileNotValid_LeapDay_CanBeFound()
        {
            var transformation = GetTransformation("29", "*");
            var time = new DateTimeOffset(2024, 2, 1, 18, 15, 23, TimeSpan.Zero);

            var result = transformation.MoveForwardWhileNotValid(time);

            result.IsChanged.Should().BeTrue();
            result.Time.Day.Should().Be(29);
            result.Time.Month.Should().Be(2); //2024 is a leap year, so february is not skipped.
        }
    }
}
