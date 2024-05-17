using System;

namespace DG.Sculpt.Cron.Clock
{
    public class CronClock
    {
        private readonly CronExpression _cronExpression;
        private DateTimeOffset _time;

        private readonly TimeTransformation _minutesTransformation;
        private readonly TimeTransformation _hoursTransformation;
        private readonly TimeTransformation _daysTransformation;
        private readonly TimeTransformation _monthsTransformation;

        public DateTimeOffset Time => _time;

        private int _minutes => _time.Minute;
        private int _hour => _time.Hour;
        private int _dayOfWeek => (int)_time.DayOfWeek;
        private int _dayOfMonth => _time.Day;
        private int _month => _time.Month;

        public CronClock(CronExpression cronExpression, DateTimeOffset time)
        {
            _cronExpression = cronExpression;
            _minutesTransformation = TimeTransformation.ForMinutes(_cronExpression.Minutes.GetLowestValue());
            _hoursTransformation = TimeTransformation.ForHours(_cronExpression.Hours.GetLowestValue());

            _time = time;
        }

        private static int GetLowestDayValue()
        {

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


        }
    }
}
