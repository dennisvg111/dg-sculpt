using DG.Sculpt.Cron.Extensions;
using System;

namespace DG.Sculpt.Cron.Clock
{
    internal class TimeTransformation
    {
        private readonly TimeSpan _modifier;
        private readonly int _lowestValue;
        private readonly Func<DateTimeOffset, int> _findCurrent;
        private readonly TimeTransformation _overflowTransformation;

        public TimeTransformation(TimeSpan modifier, int lowestValue, Func<DateTimeOffset, int> findCurrent, TimeTransformation overflowTransformation)
        {
            _modifier = modifier;
            _lowestValue = lowestValue;
            _findCurrent = findCurrent;
            _overflowTransformation = overflowTransformation;
        }

        public virtual DateTimeOffset Transform(DateTimeOffset time, int target, int direction = 1)
        {
            var current = _findCurrent(time);
            if (target != current && _overflowTransformation != null)
            {
                time = _overflowTransformation.ResetToLowest(time);
                current = _findCurrent(time);
            }
            while (current != target)
            {
                time = time.Add(_modifier.Multiply(direction));
                current = _findCurrent(time);
            }
            return time;
        }

        public virtual DateTimeOffset ResetToLowest(DateTimeOffset time)
        {
            if (_findCurrent(time) == _lowestValue)
            {
                return time;
            }
            return Transform(time, _lowestValue, -1);
        }

        public static TimeTransformation ForMinutes(int lowestValue)
        {
            return new TimeTransformation(TimeSpan.FromMinutes(1), lowestValue, (t) => t.Minute, null);
        }

        public static TimeTransformation ForHours(int lowestValue, TimeTransformation minutesTransformation)
        {
            return new TimeTransformation(TimeSpan.FromHours(1), lowestValue, (t) => t.Hour, minutesTransformation);
        }

        public static TimeTransformation ForDays(int lowestValue, TimeTransformation hoursTransformation)
        {
            return new TimeTransformation(TimeSpan.FromHours(1), lowestValue, (t) => t.Day, hoursTransformation);
        }

        public static TimeTransformation ForMonths(int lowestValue, TimeTransformation daysTransformation)
        {
            return new TimeTransformation(TimeSpan.FromDays(1), lowestValue, (t) => t.Month, daysTransformation);
        }
    }
}
