using DG.Sculpt.Cron;
using DG.Sculpt.Cron.Clock;
using FluentAssertions;
using System;
using Xunit;

namespace DG.Sculpt.Tests.Cron
{
    public class CronClockTests
    {
        [Fact]
        public void TravelToNextOccurence_FindsNextHour()
        {
            string cron = "1 10 * * *";
            var currentDate = new DateTimeOffset(2024, 5, 17, 9, 2, 0, TimeSpan.Zero);

            var clock = new CronClock(CronExpression.Parse(cron), currentDate);
            clock.TravelToNextOccurence();

            clock.Time.Minute.Should().Be(1);
            clock.Time.Hour.Should().Be(10);
        }

        [Fact]
        public void TravelToNextOccurence_NewHour_ResetsMinutes()
        {
            string cron = "* 10 * * *";
            var currentDate = new DateTimeOffset(2024, 5, 17, 9, 50, 0, TimeSpan.Zero);

            var clock = new CronClock(CronExpression.Parse(cron), currentDate);
            clock.TravelToNextOccurence();

            clock.Time.Hour.Should().Be(10);
            clock.Time.Minute.Should().Be(0);
        }
    }
}
