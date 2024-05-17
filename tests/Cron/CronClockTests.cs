using DG.Sculpt.Cron;
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
    }
}
