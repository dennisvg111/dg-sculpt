using DG.Sculpt.Cron;
using FluentAssertions;
using System;
using Xunit;

namespace DG.Sculpt.Tests.Cron
{
    public class CronClockTests
    {
        [Fact]
        public void MoveToNextOccurence_LowerMinute_FindsNextHour()
        {
            string cron = "1 * * * *";
            var currentDate = new DateTimeOffset(2024, 5, 17, 9, 2, 12, TimeSpan.Zero);

            var clock = new CronClock(CronSchedule.Parse(cron), currentDate);
            clock.MoveToNextOccurence();

            clock.Time.Minute.Should().Be(1);
            clock.Time.Hour.Should().Be(10);
        }

        [Fact]
        public void MoveToNextOccurence_NewHour_ResetsMinutes()
        {
            string cron = "* 10 * * *";
            var currentDate = new DateTimeOffset(2024, 5, 17, 9, 50, 0, TimeSpan.Zero);

            var clock = new CronClock(CronSchedule.Parse(cron), currentDate);
            clock.MoveToNextOccurence();

            clock.Time.Hour.Should().Be(10);
            clock.Time.Minute.Should().Be(0);
        }

        [Fact]
        public void MoveToNextOccurence_NewMonth_ResetsMinutes()
        {
            string cron = "* * * 6 *";
            var currentDate = new DateTimeOffset(2024, 5, 17, 9, 50, 0, TimeSpan.Zero);

            var clock = new CronClock(CronSchedule.Parse(cron), currentDate);
            clock.MoveToNextOccurence();

            clock.Time.Month.Should().Be(6);
            clock.Time.Minute.Should().Be(0);
        }

        [Fact]
        public void MoveToNextOccurence_NewMonth_RecalculatesMinutes()
        {
            string cron = "3,55 * * 6 *";
            var currentDate = new DateTimeOffset(2024, 5, 17, 9, 50, 0, TimeSpan.Zero);

            var clock = new CronClock(CronSchedule.Parse(cron), currentDate);
            clock.MoveToNextOccurence();

            clock.Time.Month.Should().Be(6);
            clock.Time.Minute.Should().Be(3);
        }
    }
}
