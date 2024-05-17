using System;

namespace DG.Sculpt.Cron
{
    public class CronClock
    {
        private readonly CronExpression _cronExpression;
        private DateTimeOffset _time;

        public DateTimeOffset Time => _time;

        private int _minutes => _time.Minute;
        private int _hour => _time.Hour;
        private int _dayOfWeek => (int)_time.DayOfWeek;
        private int _dayOfMonth => _time.Day;
        private int _month => _time.Month;

        public CronClock(CronExpression cronExpression, DateTimeOffset time)
        {
            _cronExpression = cronExpression;
            _time = time;
        }

        /// <summary>
        /// Indicates if the current <see cref="Time"/> is valid according to the used <see cref="CronExpression"/>.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return _cronExpression.Minutes.CanBe(_minutes)
                && _cronExpression.Hours.CanBe(_hour)
                && _cronExpression.Months.CanBe(_month)
                && (_cronExpression.DayOfWeek.CanBe(_dayOfWeek) || _cronExpression.DayOfMonth.CanBe(_dayOfMonth));
        }

        public void TravelToNextOccurence()
        {
            _time = _time.AddMinutes(1);

            TravelToClosestMinutes();
        }

        private void TravelToClosestMinutes()
        {
            if (!_cronExpression.Minutes.TryGetLowestOfAtLeast(_minutes, out int nextMinute))
            {
                nextMinute = _cronExpression.Minutes.GetLowestValue();
            }
            int minutesNeeded = nextMinute - _minutes;
            if (nextMinute < _minutes)
            {
                minutesNeeded += 60;
            }
            _time = _time.AddMinutes(minutesNeeded);
        }
    }
}
