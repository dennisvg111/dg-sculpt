using System;

namespace DG.Sculpt.Cron.Extensions
{
    internal static class TimeSpanExtensions
    {
        public static TimeSpan Multiply(this TimeSpan timespan, int multiplier)
        {
            return TimeSpan.FromTicks(timespan.Ticks * multiplier);
        }
    }
}
