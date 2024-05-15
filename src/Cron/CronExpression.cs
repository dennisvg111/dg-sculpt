using System;

namespace DG.Sculpt.Cron
{
    public class CronExpression
    {
        private readonly CronField _minutes;
        private readonly CronField _hours;
        private readonly CronField _dayOfMonth;
        private readonly CronField _months;
        private readonly CronField _dayOfWeek;

        public CronField Minutes => _minutes;
        public CronField Hours => _hours;
        public CronField DayOfMonth => _dayOfMonth;
        public CronField Months => _months;
        public CronField DayOfWeek => _dayOfWeek;

        public CronExpression(CronField minutes, CronField hours, CronField dayOfMonth, CronField month, CronField dayOfWeek)
        {
            _minutes = minutes;
            _hours = hours;
            _dayOfMonth = dayOfMonth;
            _months = month;
            _dayOfWeek = dayOfWeek;
        }

        public static CronExpression Parse(string value)
        {
            return null;
        }

        public static bool TryParse(string value, out CronExpression expression)
        {
            expression = null;
            return false;
        }

        public override string ToString()
        {
            return $"{_minutes} {_hours} {_dayOfMonth} {_months} {_dayOfWeek}";
        }

        #region Static instances
        private CronExpression(int? minutes, int? hours, int? dayOfMonth, int? month, int? dayOfWeek)
            : this(new CronField(new CronValue(minutes), CronValue.Any, null, null),
                  new CronField(new CronValue(hours), CronValue.Any, null, null),
                  new CronField(new CronValue(dayOfMonth), CronValue.Any, null, null),
                  new CronField(new CronValue(month), CronValue.Any, null, null),
                  new CronField(new CronValue(dayOfWeek), CronValue.Any, null, null))
        { }

        private static readonly Lazy<CronExpression> _yearly = new Lazy<CronExpression>(() => new CronExpression(0, 0, 1, 1, null));
        private static readonly Lazy<CronExpression> _monthly = new Lazy<CronExpression>(() => new CronExpression(0, 0, 1, null, null));
        private static readonly Lazy<CronExpression> _weekly = new Lazy<CronExpression>(() => new CronExpression(0, 0, null, null, 0));
        private static readonly Lazy<CronExpression> _daily = new Lazy<CronExpression>(() => new CronExpression(0, 0, null, null, null));
        private static readonly Lazy<CronExpression> _hourly = new Lazy<CronExpression>(() => new CronExpression(0, null, null, null, null));

        /// <summary>
        /// Run once a year at midnight of 1 January.
        /// </summary>
        public static CronExpression Yearly => _yearly.Value;

        /// <summary>
        /// Run once a month at midnight of the first day of the month.
        /// </summary>
        public static CronExpression Monthly => _monthly.Value;

        /// <summary>
        /// Run once a week at midnight on Sunday.
        /// </summary>
        public static CronExpression Weekly => _weekly.Value;

        /// <summary>
        /// Run once a day at midnight.
        /// </summary>
        public static CronExpression Daily => _daily.Value;

        /// <summary>
        /// Run once an hour at the beginning of the hour.
        /// </summary>
        public static CronExpression Hourly => _hourly.Value;
        #endregion
    }
}
